using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BookStore.BusinessObject.Models;

public partial class CartItem
{
    [Key]
    public int CartItemID { get; set; }

    public int? CartID { get; set; }

    public int? ProductID { get; set; }

    public int Quantity { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? AddedDate { get; set; }

    [ForeignKey("CartID")]
    [InverseProperty("CartItems")]
    public virtual Cart? Cart { get; set; }

    [ForeignKey("ProductID")]
    [InverseProperty("CartItems")]
    public virtual Product? Product { get; set; }
}
