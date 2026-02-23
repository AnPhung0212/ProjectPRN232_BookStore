using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Helper.EmailTemplate
{

    public static class EmailTemplateHelper
    {

        public static string BodyRegister => _bodyRegister;
        private const string _bodyRegister = @"
    <!DOCTYPE html>
    <html lang=""vi"">
    <head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Xác thực tài khoản</title>
    <style>
        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            line-height: 1.6;
            color: #333333;
            margin: 0;
            padding: 0;
            background-color: #f7f7f7;
        }
        .email-container {
            max-width: 600px;
            margin: 0 auto;
            background-color: #ffffff;
        }
        .email-header {
            background: linear-gradient(135deg, #4b6cb7 0%, #182848 100%);
            padding: 30px 20px; text-align: center; border-radius: 5px 5px 0 0;
        }
        .email-header h1 {
            color: white; margin: 0; font-size: 24px; font-weight: 600;
        }
        .email-body {
            padding: 30px;
        }
        .button {
            display: inline-block; padding: 12px 24px; background-color: #4b6cb7; color: white; text-decoration: none; border-radius: 4px; font-weight: 600; margin: 20px 0;
        }
    </style>
    </head>
    <body>
    <div class=""email-container"">
        <div class=""email-header"">
            <h1>Xác thực tài khoản</h1>
        </div>
        
        <div class=""email-body"">
            <p>Xin chào,</p>
            <p>Cảm ơn bạn đã đăng ký tài khoản tại BookStore. Để hoàn tất, vui lòng xác thực tài khoản của bạn:</p>
            
            <div style=""text-align: center;"">
                <a href=""{{confirmUrl}}"" class=""button"">Xác thực tài khoản</a>
            </div>
            
            <p>Hoặc copy liên kết: <a href='{{confirmUrl}}'>{{confirmUrl}}</a></p>
        </div>
    </div>
    </body>
    </html>";

        public static string BodyResetEmail => _bodyResetEmail;
        private const string _bodyResetEmail = @"
    <!DOCTYPE html>
    <html lang=""vi"">
    <head>
        <meta charset=""UTF-8"">
        <title>Đặt lại mật khẩu</title>
        <style>
            body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; line-height: 1.6; color: #333; }
            .otp-code { font-size: 24px; font-weight: bold; color: #4b6cb7; letter-spacing: 5px; background: #f0f0f0; padding: 10px; border-radius: 5px; display: inline-block; margin: 10px 0; }
        </style>
    </head>
    <body>
        <h2>Đặt lại mật khẩu BookStore</h2>
        <p>Xin chào,</p>
        <p>Bạn vừa yêu cầu đặt lại mật khẩu. Vui lòng sử dụng mã dưới đây để thực hiện:</p>
        <div class=""otp-code"">{{otp}}</div>
        <p>Mã này sẽ hết hạn sau 15 phút. Nếu bạn không yêu cầu hành động này, hãy bỏ qua email này.</p>
    </body>
    </html>";
    }
}
