using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BookStore.BusinessObject.Models;

[Index("Username", Name = "UQ__Users__536C85E4810C4776", IsUnique = true)]
public partial class User
{
    [Key]
    public int UserID { get; set; }

    public int? RoleID { get; set; }

    [StringLength(100)]
    public string Username { get; set; } = null!;

    [StringLength(255)]
    public string Password { get; set; } = null!;

    [StringLength(255)]
    public string? FullName { get; set; }

    [StringLength(255)]
    public string? Email { get; set; }

    [StringLength(255)]
    public string? Address { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? TotalAmount { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    public bool IsActive { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    [InverseProperty("User")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    [ForeignKey("RoleID")]
    [InverseProperty("Users")]
    public virtual Role? Role { get; set; }
}
