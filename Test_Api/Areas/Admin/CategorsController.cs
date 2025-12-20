using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Test_Api.Datebase;
using Test_Api.Models;
using Test_Api.Rpository.Irepository;
using Test_Api.Utility;

namespace Test_Api.Areas.Admin
{
    [Route("[Area]/[controller]")]
    [ApiController]
    [Area("Admin")]
    [Authorize(Roles = $"{SD.Role_Admin},{SD.Role_Employee}")]
    public class CategorsController : ControllerBase
    {
        private readonly ApplicationDBcontext _context;
        private readonly IRepository<Category> _Categoryrepository;
        public CategorsController(ApplicationDBcontext context, IRepository<Category> categoryrepository)
        {
            _context = context;
            _Categoryrepository = categoryrepository;
        }
        [HttpGet("GetAll")]
        [Authorize(Roles = $"{SD.Role_Admin},{SD.Role_Employee}")]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var categors = await _Categoryrepository.GetAllAsync(tracked:false, cancellationToken:cancellationToken);
            if (categors is null)
            {
                return NotFound(new
                {
                    msg = "No categors found",
                });
            }
            return Ok(categors);
        }
        [HttpGet("Get/{id}")]
        [Authorize(Roles = $"{SD.Role_Admin},{SD.Role_Employee}")]
        public async Task<IActionResult> Get(int id,CancellationToken cancellationToken)
        {
            var categors =  await _Categoryrepository.GetOneAsync(e => e.Id == id,cancellationToken:cancellationToken,tracked:false);
            if (categors is null)
            {
                return NotFound(new
                {
                    msg = "No categors found",
                });
            }
            return Ok(categors);
        }
        [HttpPost("Creat")]
        [Authorize(Roles = $"{SD.Role_Admin},{SD.Role_Employee}")]
        public async Task<IActionResult> Creat(Category cagtegorys ,CancellationToken cancellationToken)
        {
            if (cagtegorys is null)
            {
                return BadRequest();
            }
           await _Categoryrepository.AddAsync(cagtegorys,cancellationToken);
            await _Categoryrepository.ComitSaveAsync(cancellationToken);

            return CreatedAtAction(nameof(Get), new { id = cagtegorys.Id }, new
            {
                msg="Categors created successfully",
            });
        }
        [HttpPut("Edit/{id}")]
        [Authorize(Roles = $"{SD.Role_Admin},${SD.Role_Employee}")]
        public async Task<IActionResult> Edit(int id, Category cagtegorys,CancellationToken cancellationToken)
        {
            if (cagtegorys is null)
            {
                return BadRequest();
            }
            var existCategors = await _Categoryrepository.GetOneAsync(e => e.Id == id, cancellationToken: cancellationToken);
            if (existCategors is null)
            {
                return NotFound(new
                {
                    msg = "No categors found",
                });
            }

            existCategors.Name = cagtegorys.Name;
          await _Categoryrepository.ComitSaveAsync(cancellationToken);

            return NoContent();
        }
        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = $"{SD.Role_Admin}")]
        public async Task<IActionResult> Delete(int id ,CancellationToken cancellationToken)
        {
            var existCategors = await _Categoryrepository.GetOneAsync(e => e.Id == id, cancellationToken: cancellationToken);
            if (existCategors is null)
            {
                return NotFound(new
                {
                    msg = "No categors found",
                });
            }
           _Categoryrepository.Delete(existCategors);
           await _Categoryrepository.ComitSaveAsync(cancellationToken);

            return NoContent();
        }
    }
}
