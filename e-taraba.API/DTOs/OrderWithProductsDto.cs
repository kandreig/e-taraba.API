﻿namespace e_taraba.API.DTOs
{
    public class OrderWithProductsDto
    {
        public int Id { get; set; }
        public string CustomerFirstName { get; set; }
        public string CustomerLastName { get; set; }
        public string CustomerPhone { get; set; }
        public decimal Total { get; set; }

        public ICollection<ProductWithoutOrdersDto> Products { get; set; }
    }
}
