using AutoMapper;
using e_taraba.API.DTOs;
using e_taraba.API.SearchParameters;
using e_taraba.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace e_taraba.API.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IRepository repository;
        private readonly IMapper mapper;

        public ProductsController(IRepository _repository, IMapper _mapper)
        {
            this.repository = _repository ?? throw new ArgumentNullException(nameof(repository));
            this.mapper = _mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts(
            [FromQuery] ProductSearchParameters searchParameters, [FromQuery] Pagination pagination)
        {
            var (products, pagi) = await repository.GetProductsASync(searchParameters,pagination);
            var productsToReturn = mapper.Map<IEnumerable<ProductWithoutOrdersDto>>(products);

            var dataToReturn = new
            {
                products = productsToReturn,
                currentPageNumber = pagi.CurrentPageNumber,
                itemsOnPage = pagi.ItemsOnPage,
                totalItems = pagi.TotalItemsNumber,
                totalPages = pagi.TotalPagesNumber
            };

            return Ok(dataToReturn);
        }

        [HttpGet("{id}",Name = "GetProduct")]
        public async Task<IActionResult> GetProduct(int id, bool includeOrders = false)
        {
            var product = await repository.GetProductASync(id, includeOrders);
            if(product is null)
            {
                return NotFound();
            }
            if (includeOrders)
            {
                return Ok(mapper.Map<ProductWithOrdersDto>(product));
            }
            return Ok(mapper.Map<ProductWithoutOrdersDto>(product));
        }


        [HttpGet("order/{idOrder}")]
        public async Task<ActionResult<IEnumerable<ProductWithoutOrdersDto>>> GetProductsForOrder(int idOrder)
        {
            if(!await repository.OrderExistsAsync(idOrder))
            {
                return NotFound();
            }

            var productsOfOrder = await repository.GetProductsForOrderASync(idOrder);
            var productsToReturn = mapper.Map<IEnumerable<ProductWithoutOrdersDto>>(productsOfOrder);

            return Ok(productsToReturn);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ProductForCreationDto>> CreateProduct(
            [FromForm]ProductForCreationDto productFromBody, IFormFile? Image = null)
        {



            var folderPath = Path.Combine(Directory.GetCurrentDirectory(),"Images");
            productFromBody.PhotoFolderPath = folderPath;
            productFromBody.PhotoId = Guid.NewGuid().ToString();



            var photoIdWithExtension = productFromBody.PhotoId + ".jpg";

            var imagePath = Path.Combine(folderPath, photoIdWithExtension);

            using (FileStream stream = new FileStream(imagePath, FileMode.Create, FileAccess.ReadWrite))
            {
                await Image.CopyToAsync(stream);
                stream.Close();
            }



            var productToCreate = mapper.Map<Entities.Product>(productFromBody);

            await repository.CreateProductASync(productToCreate);
            await repository.SaveChangesASync();

            var createdProductToReturn = mapper.Map<ProductWithoutOrdersDto>(productToCreate);

            return CreatedAtRoute("GetProduct",
                new
                {
                    id = createdProductToReturn.Id
                },
                createdProductToReturn);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateProduct(int id, [FromForm] ProductForUpdateDto productForUpdate, IFormFile? Image = null)
        {

            if (!await repository.ProductExistsAsync(id))
            {
                return NotFound("Product was not found");
            }

            var productFromDb = await repository.GetProductASync(id, false);

            if (Image != null)
            {
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Images");
                productForUpdate.PhotoFolderPath = folderPath;
                productForUpdate.PhotoId = Guid.NewGuid().ToString();

                var photoIdWithExtension = productForUpdate.PhotoId + ".jpg";

                var imagePath = Path.Combine(folderPath, photoIdWithExtension);

                using (FileStream stream = new FileStream(imagePath, FileMode.Create, FileAccess.ReadWrite))
                {
                    await Image.CopyToAsync(stream);
                    stream.Close();
                }

            }
            else
            {
                productForUpdate.PhotoId = productFromDb.PhotoId;
                productForUpdate.PhotoFolderPath = productFromDb.PhotoFolderPath;
            }



            mapper.Map(productForUpdate, productFromDb);
            await repository.SaveChangesASync();

            return NoContent();

        }


        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var productToDelete = await repository.GetProductASync(id, false);
            if(productToDelete is null)
            {
                return NotFound();
            }
            repository.DeleteProductASync(productToDelete);
            await repository.SaveChangesASync();

            return NoContent();
        }
    }
}
