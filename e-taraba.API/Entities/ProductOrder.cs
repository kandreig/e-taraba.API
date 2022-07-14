using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace e_taraba.API.Entities
{
    [Table("tProductOrderDetails", Schema = "taraba")]
    public class ProductOrder
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public Product? Product { get; set; }
        [Column("idProduct")]
        public int ProductId { get; set; }
        public Order? Order { get; set; }
        [Column("idOrder")]
        public int OrderId { get; set; }

        /*
         id int identity(1,1) primary key,
        idProduct int constraint fk_ProductOrder foreign key(idProduct) references e_taraba.products(id),
        idOrder int constraint fk_OrderProduct foreign key(idOrder) references e_taraba.orders(id),
        price decimal(9,2) not null,
        quantity int not null
         */
    }
}
