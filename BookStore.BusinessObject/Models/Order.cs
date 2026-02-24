using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BookStore.BusinessObject.Models;

public partial class Order
{
    [Key]
    public int OrderID { get; set; }

    public int? UserID { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? OrderDate { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? TotalAmount { get; set; }

    public int? StatusID { get; set; }

    [StringLength(255)]
    public string? ShippingAddress { get; set; }

    [StringLength(100)]
    public string? PaymentMethod { get; set; }

    [StringLength(20)]
    public string? Phone { get; set; }

    [InverseProperty("Order")]
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    [ForeignKey("StatusID")]
    [InverseProperty("Orders")]
    public virtual OrderStatus? Status { get; set; }

    [ForeignKey("UserID")]
    [InverseProperty("Orders")]
    public virtual User? User { get; set; }
}
