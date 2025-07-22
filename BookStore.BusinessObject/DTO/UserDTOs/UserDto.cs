using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.BusinessObject.DTO.UserDTOs
{
    public class UserDto
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string? FullName { get; set; }
        public string Email { get; set; }
        public string? Address { get; set; }
        public decimal? TotalAmount { get; set; }
        public string? RoleName { get; set; }
    }
}
