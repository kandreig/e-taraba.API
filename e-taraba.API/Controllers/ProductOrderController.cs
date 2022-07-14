using AutoMapper;
using e_taraba.API.DTOs;
using e_taraba.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace e_taraba.API.Controllers
{
    [Route("api/productorder")]
    [ApiController]
    public class ProductOrderController : ControllerBase
    {
        private readonly IRepository repository;
        private readonly IMapper mapper;

        public ProductOrderController(IRepository repository, IMapper mapper)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet("{id}", Name = "GetProductOrder")]
        public async Task<ActionResult<ProductOrderDto>> GetProductOrder(int id)
        {
            var productOrder = await repository.GetProductOrderASync(id);

            if(productOrder is null)
            {
                return NotFound();
            }

            return Ok(mapper.Map<ProductOrderDto>(productOrder));
        }

        [HttpGet("order/{id}")]
        public async Task<ActionResult<IEnumerable<ProductOrderDto>>> GetProductOrderDetailsForOrder(int id)
        {
            if(!await repository.OrderExistsAsync(id))
            {
                return NotFound();
            }

            var productOrderForOrder = await repository.GetProductOrderDetailsForOrderASync(id);
            var productOrderForOrderToReturn = mapper.Map<IEnumerable<ProductOrderDto>>(productOrderForOrder);

            return Ok(productOrderForOrderToReturn);

        }

        [HttpPost]
        public async Task<IActionResult> CreateProductOrderDetails(
            [FromBody] ProductOrderForCreation productOrderDetailsFromBody)
        {
            var productOrderDetailsToCreate = mapper.Map<Entities.ProductOrder>(productOrderDetailsFromBody);

            await repository.CreateProductOrderDetailsASync(productOrderDetailsToCreate);
            await repository.SaveChangesASync();

            var createProductOrderDetailsToReturn = mapper.Map<ProductOrderDto>(productOrderDetailsToCreate);

            return CreatedAtRoute("GetProductOrder",
                new
                {
                    id = createProductOrderDetailsToReturn.Id
                },
                createProductOrderDetailsToReturn);
        }

    }
}
