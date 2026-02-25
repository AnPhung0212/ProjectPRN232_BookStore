using BookStore.Services.Implement.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Services.Interfaces
{
    public interface IRegisterService
    {
        Task<string> RegisterUserAsync(RegisterInput input);
        Task<(int StatusCode, string Message)> VerifyUserAsync(string token); // thêm hàm verify
    }
}
