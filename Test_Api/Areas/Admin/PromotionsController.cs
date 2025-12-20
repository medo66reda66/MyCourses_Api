using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Test_Api.Dtos.Request;
using Test_Api.Models;
using Test_Api.Rpository.Irepository;
using Test_Api.Utility;

namespace Test_Api.Areas.Admin
{
    [Route("api/[Area]/[controller]")]
    [ApiController]
    [Area("Admin")]
    [Authorize(Roles =$"{SD.Role_Admin},{SD.Role_Employee}")]
    public class PromotionsController : ControllerBase
    {
      private readonly IRepository<Promotion> _promotionRepository;
        public PromotionsController(IRepository<Promotion> promotionRepository)
        {
            _promotionRepository = promotionRepository;
        }

        [HttpGet("Gepromotion")]
        public async Task<IActionResult> Gepromotion(CancellationToken cancellationToken)
        {
            var promotions = await _promotionRepository.GetAllAsync(tracked: false,cancellationToken:cancellationToken);
            return Ok(promotions);
        }

        [HttpGet("Getonepro/{coursid}")]
        public async Task<IActionResult> Getonepro(int coursid , CancellationToken cancellationToken)
        {
            var promotion =await _promotionRepository.GetOneAsync(e => e.CourseId == coursid, tracked: false, cancellationToken: cancellationToken);
            return Ok(promotion);
        }

        [HttpPost("Create")]
       public async Task<IActionResult> Create(promotionRequest promotionRequest,CancellationToken cancellationToken)
        {
            var promotion = new Promotion
            {
                CourseId = promotionRequest.CourseId,
                DiscountPercentage = promotionRequest.DiscountPercentage,
                StartDate =DateTime.UtcNow,
                Code = promotionRequest.code,
                EndDate = DateTime.UtcNow.AddMonths(1),
                IsActive = promotionRequest.IsActive
            };
            await _promotionRepository.AddAsync(promotion,cancellationToken);
           await _promotionRepository.ComitSaveAsync(cancellationToken);

            return Ok(new
            {
                msg = "Promotion created successfully",
                promotion
            });
        }

    }
}
