using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Test_Api.Datebase;
using Test_Api.Dtos.Request;
using Test_Api.Models;
using Test_Api.Rpository.Irepository;
using Test_Api.Utility;

namespace Test_Api.Areas.Admin
{
    [Route("[Area]/[controller]")]
    [ApiController]
    [Area("Admin")]
    [Authorize(Roles = $"{SD.Role_Admin},{SD.Role_Employee}")]
    public class InestractorsController : ControllerBase
    {
        private readonly ApplicationDBcontext _context ;
        private readonly IRepository<Instructor> _instractorrepository;
        public InestractorsController(ApplicationDBcontext context, IRepository<Instructor> instractorrepository)
        {
            _context = context;
            _instractorrepository = instractorrepository;
        }
        [HttpGet("GetAll")]
        [Authorize(Roles = $"{SD.Role_Admin},${SD.Role_Employee}")]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var inestractors =await _instractorrepository.GetAllAsync(tracked:false,cancellationToken:cancellationToken);
            return Ok(inestractors);
        }
        [HttpGet("Get/{id}")]
        [Authorize(Roles = $"{SD.Role_Admin},{SD.Role_Employee}")]
        public async Task<IActionResult> Get(int id,CancellationToken cancellationToken)
        {
            var inestractor = await _instractorrepository.GetOneAsync(e=>e.Id == id , tracked:false, cancellationToken:cancellationToken);
            if (inestractor is null)
            {
                return NotFound(new
                {
                    msg = "No inestractor found",
                });
            }
            return Ok(inestractor);
        }
        [HttpPost("Creat")]
        [Authorize(Roles = $"{SD.Role_Admin},{SD.Role_Employee}")]
        public async Task<IActionResult> Creat(CreateInstractorRequest createInstractorRequest ,CancellationToken cancellationToken)
        {
            if (createInstractorRequest is null)
            {
                return BadRequest();
            }
            var inestractor = new Models.Instructor()
            {
                Name = createInstractorRequest.Name,
                ProfileUrl = createInstractorRequest.ProfileUrl,
                Age = createInstractorRequest.Age,
            };
            await _instractorrepository.AddAsync(inestractor,cancellationToken);
            if (createInstractorRequest.Image is not null && createInstractorRequest.Image.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(createInstractorRequest.Image.FileName);
                var filePath = Path.Combine("wwwroot", "InstructorsImages", fileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    createInstractorRequest.Image.CopyTo(stream);
                }
                inestractor.Image = fileName;
            }
          await _instractorrepository.ComitSaveAsync(cancellationToken);
            return Ok(inestractor);
        }
        [HttpPut("Update/{id}")]
        [Authorize(Roles = $"{SD.Role_Admin},{SD.Role_Employee}")]
        public async Task<IActionResult> Update(int id, EditInstructorRequest editInstructorRequest,CancellationToken cancellationToken)
        {
            var inestractor = await _instractorrepository.GetOneAsync(e => e.Id == id, cancellationToken: cancellationToken);
            if (inestractor is null)
            {
                return NotFound(new
                {
                    msg = "No inestractor found",
                });
            }
            inestractor.Name = editInstructorRequest.Name;
            inestractor.Age = editInstructorRequest.Age;
            inestractor.ProfileUrl = editInstructorRequest.ProfileUrl;
            if (editInstructorRequest.NewImage is not null && editInstructorRequest.NewImage.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(editInstructorRequest.NewImage.FileName);
                var filePath = Path.Combine("wwwroot", "InestractorImg", fileName);
                var oldImagePath = Path.Combine("wwwroot", "InestractorImg", inestractor.Image ?? string.Empty);
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }

                using (var stream = System.IO.File.Create(filePath))
                {
                    editInstructorRequest.NewImage.CopyTo(stream);
                }
                inestractor.Image = fileName;
            }
           await _instractorrepository.ComitSaveAsync(cancellationToken);
            return NoContent();
        }
        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = $"{SD.Role_Admin}")]
        public async Task<IActionResult> Delete(int id,CancellationToken cancellationToken)
        {
            var inestractor =await _instractorrepository.GetOneAsync(e=>e.Id == id ,cancellationToken:cancellationToken);
            if (inestractor is null)
            {
                return NotFound(new
                {
                    msg = "No inestractor found",
                });
            }
            var oldImagePath = Path.Combine("wwwroot", "InestractorImg", inestractor.Image ?? string.Empty);
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }
            _instractorrepository.Delete(inestractor);
            await _instractorrepository.ComitSaveAsync(cancellationToken);
            return NoContent();
        }
    }
}
