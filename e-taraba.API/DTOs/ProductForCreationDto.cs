﻿namespace e_taraba.API.DTOs
{
    public class ProductForCreationDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public string? PhotoId { get; set; }
        public string? PhotoFolderPath { get; set; }
        public decimal Price { get; set; }
    }
}
