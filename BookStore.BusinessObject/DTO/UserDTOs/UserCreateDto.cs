using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.BusinessObject.DTO.UserDTOs
{
    public class UserCreateDto
    {
        public string Username { get; set; } = null!;
        [MinLength(6)]
        public string Password { get; set; } = null!;
        public string? FullName { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string? Address { get; set; }
        public int RoleId { get; set; }
    }
}
