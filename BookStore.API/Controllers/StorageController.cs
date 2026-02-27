using Azure;
using BookStore.BusinessObject.Config;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Supabase;

namespace BookStore.API.Controllers
{
    [Route("api/storage")]
    [ApiController]
    public class StorageController : ControllerBase
    {
        private readonly Client _supabaseClient;
        private readonly SupabaseSettings _settings;

        public StorageController(Client supabaseClient, IOptions<SupabaseSettings> settings)
        {
            _supabaseClient = supabaseClient;
            _settings = settings.Value;
        }

        /// <summary>
        /// Upload file (sách, avatar, ...) lên Supabase.
        /// folder: "books", "avatars", ...
        /// </summary>
        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] UploadRequest request)
        {
            try
            {
                var file = request.file;

                if (file == null || file.Length == 0)
                {
                    return BadRequest("File is required.");
                }
                if (file.Length > 5 * 1024 * 1024)
                {
                    return BadRequest("File must small than 5 MB.");
                }

                var uniqueName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                var path = uniqueName;

                byte[] bytes;
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    bytes = memoryStream.ToArray();
                }

                // điểm dễ lỗi nhất: Supabase client / bucket
                await _supabaseClient.Storage
                    .From(_settings.Bucket)
                    .Upload(bytes, path);

                var publicUrl = _supabaseClient.Storage
                    .From(_settings.Bucket)
                    .GetPublicUrl(path);

                return Ok(new { path, url = publicUrl });
            }
            catch (Exception ex)
            {
                // tạm thời log đơn giản
                return StatusCode(500, $"Storage upload error: {ex.Message}");
            }
        }

        /// <summary>
        /// Xóa file trên Supabase theo path hoặc full URL.
        /// </summary>
        [HttpDelete("delete")]
        public async Task<IActionResult> Delete([FromQuery] string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return BadRequest("filePath is required.");
            }

            // Chuẩn hóa baseUrl, bỏ gạch chéo cuối
            var bucketBaseUrl = _supabaseClient.Storage
                .From(_settings.Bucket)
                .GetPublicUrl(string.Empty).TrimEnd('/');

            string relativePath;

            if (filePath.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                // Nếu là full URL
                relativePath = filePath.StartsWith(bucketBaseUrl, StringComparison.OrdinalIgnoreCase)
                    ? filePath.Substring(bucketBaseUrl.Length).TrimStart('/')
                    : throw new ArgumentException("filePath is not a valid URL for this bucket.");
            }
            else
            {
                // Đã là relative path
                relativePath = filePath.TrimStart('/');
            }

            await _supabaseClient.Storage
                .From(_settings.Bucket)
                .Remove(relativePath);

            return NoContent();
        }

        public class UploadRequest
        {
            public IFormFile file { get; set; } = null!;
        }
    }
}
