using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;

namespace BookStore.API.Middleware
{
    public static class EnableAuthorizationRoute
    {
        // Danh sách các API cho phép truy cập tự do
        private static readonly List<string> PublicRoutes = new()
        {
            "/api/User/login",
            "/api/User/register",
            "/api/Product",         // Cho phép xem sản phẩm, category, v.v.
            "/api/Category",
            "/swagger",             // Cho phép vào trang tài liệu API
            "/favicon.ico",
            "/api/User/verify-email"

        };

        public static bool IsPublicRoute(HttpContext httpContext)
        {
            var path = httpContext.Request.Path.Value;
            if (string.IsNullOrEmpty(path)) return false;

            // Kiểm tra xem path hiện tại có nằm trong danh sách hoặc là sub-path của swagger không
            return PublicRoutes.Any(r => 
                path.Equals(r, System.StringComparison.OrdinalIgnoreCase) || 
                path.StartsWith(r + "/", System.StringComparison.OrdinalIgnoreCase));
        }
    }
}