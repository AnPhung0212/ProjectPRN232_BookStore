using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BookStore.BusinessObject.Models;

public partial class Role
{
    [Key]
    public int RoleID { get; set; }

    [StringLength(100)]
    public string RoleName { get; set; } = null!;

    [StringLength(255)]
    public string? Description { get; set; }

    [InverseProperty("Role")]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
