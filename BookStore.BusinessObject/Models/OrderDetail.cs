using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BookStore.BusinessObject.Models;

public partial class OrderDetail
{
    [Key]
    public int OrderDetailID { get; set; }

    public int? OrderID { get; set; }

    public int? ProductID { get; set; }

    public int Quantity { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal UnitPrice { get; set; }

    [ForeignKey("OrderID")]
    [InverseProperty("OrderDetails")]
    public virtual Order? Order { get; set; }

    [ForeignKey("ProductID")]
    [InverseProperty("OrderDetails")]
    public virtual Product? Product { get; set; }
}
