namespace e_taraba.API.DTOs
{
    public class ProductWithOrdersDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public string PhotoId { get; set; }
        public string PhotoFolderPath { get; set; }
        public decimal Price { get; set; }

        public ICollection<OrderWithoutProductsDto> Orders { get; set; }
        public ProductWithOrdersDto()
        {
            Orders = new List<OrderWithoutProductsDto>();
        }
    }
}
