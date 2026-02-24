using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BookStore.BusinessObject.Models;

public partial class Product
{
    [Key]
    public int ProductID { get; set; }

    public int? CategoryID { get; set; }

    [StringLength(255)]
    public string Title { get; set; } = null!;

    [StringLength(255)]
    public string? Author { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Price { get; set; }

    public string? Description { get; set; }

    public int? Stock { get; set; }

    [StringLength(500)]
    public string? ImageURL { get; set; }

    [InverseProperty("Product")]
    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    [ForeignKey("CategoryID")]
    [InverseProperty("Products")]
    public virtual Category? Category { get; set; }

    [InverseProperty("Product")]
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
