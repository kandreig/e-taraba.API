using System.ComponentModel.DataAnnotations.Schema;

namespace e_taraba.API.Entities
{
    [Table("tOrders", Schema = "taraba")]
    public class Order
    {
        public int Id { get; set; }
        public string CustomerFirstName { get; set; }
        public string CustomerLastName { get; set; }
        public string CustomerPhone { get; set; }
        public decimal Total { get; set; }

        public ICollection<Product> Products { get; set; }
        public List<ProductOrder> ProductOrders { get; set; }


        /* 
         id int identity(1,1) primary key,
         customerFirstName varchar(100) not null,
         customerLastName varchar(100) not null,
         customerPhone varchar(20) not null,
         total decimal(9,2) not null
         */
    }
}
