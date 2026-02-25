using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Services.Implement.Input
{
    public class RegisterInput
    {
        public required string Email { get; set; }

        public required string Password { get; set; }

        public required string UserName { get; set; }
        public required string Address { get; set; }

    }
}
