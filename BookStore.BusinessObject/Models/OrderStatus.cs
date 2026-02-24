using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BookStore.BusinessObject.Models;

public partial class OrderStatus
{
    [Key]
    public int StatusID { get; set; }

    [StringLength(100)]
    public string StatusName { get; set; } = null!;

    [StringLength(255)]
    public string? Description { get; set; }

    [InverseProperty("Status")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
