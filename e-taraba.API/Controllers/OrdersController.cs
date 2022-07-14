using AutoMapper;
using e_taraba.API.DTOs;
using e_taraba.API.Entities;
using e_taraba.API.SearchParameters;
using e_taraba.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace e_taraba.API.Controllers
{

    [Route("api/orders")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IRepository repository;
        private readonly IMapper mapper;

        public OrdersController(IRepository repository, IMapper mapper)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }


        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetOrders(
            [FromQuery] OrderSearchParameters searchParameters, [FromQuery] Pagination pagination)
        {
            var (orders, pagi) = await repository.GetOrdersASync(searchParameters, pagination);

            var ordersToReturn = mapper.Map<IEnumerable<OrderWithoutProductsDto>>(orders);

            var dataToReturn = new
            {
                orders = ordersToReturn,
                currentPageNumber = pagi.CurrentPageNumber,
                itemsOnPage = pagi.ItemsOnPage,
                totalItems = pagi.TotalItemsNumber,
                totalPages = pagi.TotalPagesNumber
            };

            return Ok(dataToReturn);
        }

        [HttpGet("{id}", Name = "GetOrder")]
        public async Task<IActionResult> GetOrder(int id, bool includeProducts = false)
        {
            var order = await repository.GetOrderASync(id, includeProducts);
            if(order == null)
            {
                return NotFound();
            }
            if (includeProducts)
            {
                return Ok(mapper.Map<OrderWithProductsDto>(order));
            }
            return Ok(mapper.Map<OrderWithoutProductsDto>(order));
        }

        [HttpPost]
        public async Task<ActionResult> CreateOrder(
            [FromBody]OrderForCreationDto orderFromBody)
        {
            var orderToCreate = mapper.Map<Entities.Order>(orderFromBody);
            await repository.CreateOrderASync(orderToCreate);
            await repository.SaveChangesASync();

            var createdOrderToReturn = mapper.Map<OrderWithoutProductsDto>(orderToCreate);

            return CreatedAtRoute("GetOrder",
                new
                {
                    id = createdOrderToReturn.Id
                },
                createdOrderToReturn);
        }
    }
}
