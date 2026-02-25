namespace BookStore.Helper.Template
{
    public static class EmailTemplate
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
            padding: 30px 20px;
            text-align: center;
            border-radius: 5px 5px 0 0;
        }
        .email-header h1 {
            color: white;
            margin: 0;
            font-size: 24px;
            font-weight: 600;
        }
        .email-body {
            padding: 30px;
        }
        .email-footer {
            background-color: #f1f1f1;
            padding: 20px;
            text-align: center;
            font-size: 12px;
            color: #666666;
            border-radius: 0 0 5px 5px;
        }
        .button {
            display: inline-block;
            padding: 12px 24px;
            background-color: #4b6cb7;
            color: white;
            text-decoration: none;
            border-radius: 4px;
            font-weight: 600;
            margin: 20px 0;
        }
        .divider {
            border-top: 1px solid #eeeeee;
            margin: 25px 0;
        }
        .logo {
            max-width: 180px;
            margin-bottom: 15px;
        }
        .social-links a {
            margin: 0 10px;
            text-decoration: none;
            color: #4b6cb7;
        }
        @media screen and (max-width: 600px) {
            .email-body {
                padding: 20px;
            }
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
            <p>Cảm ơn bạn đã đăng ký tài khoản. Để hoàn tất quá trình đăng ký, vui lòng xác thực tài khoản của bạn bằng cách nhấp vào nút bên dưới:</p>
            
            <div style=""text-align: center;"">
                <a href=""{{confirmUrl}}"" class=""button"">Xác thực tài khoản</a>
            </div>
            
            <p>Nếu nút trên không hoạt động, bạn có thể sao chép và dán đường liên kết sau vào trình duyệt:</p>
            <p style=""word-break: break-all; color: #4b6cb7; background-color: #f9f9f9; padding: 10px; border-radius: 4px;"">
            <a href='{{confirmUrl}}'>Xác thực tài khoản</a>
            </p>
            <p>Bạn nhận được email này vì bạn đã đăng ký dịch vụ của chúng tôi.</p>
        </div>
    </div>
    </body>
    </html>
";
    }
}
