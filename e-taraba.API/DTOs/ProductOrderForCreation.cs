namespace e_taraba.API.DTOs
{
    public class ProductOrderForCreation
    {
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int ProductId { get; set; }
        public int OrderId { get; set; }
    }
}
