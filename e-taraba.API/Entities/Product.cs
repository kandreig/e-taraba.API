using System.ComponentModel.DataAnnotations.Schema;

namespace e_taraba.API.Entities
{
    [Table("tProducts", Schema = "taraba")]
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public string PhotoId { get; set; }
        public string PhotoFolderPath { get; set; }
        public decimal Price { get; set; }

        public ICollection<Order> Orders { get; set; }
        public List<ProductOrder> ProductOrders { get; set; }
    }
}
