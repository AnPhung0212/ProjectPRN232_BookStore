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
            var file = request.file;
            var folder = request.folder;

            if (file == null || file.Length == 0)
            {
                return BadRequest("File is required.");
            }
            if (file.Length > 5 * 1024 * 1024)
            {
                return BadRequest("File must small than 5 MB.");
            }

            if (string.IsNullOrWhiteSpace(folder))
            {
                folder = "others";
            }

            var extension = Path.GetExtension(file.FileName);
            var uniqueName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var path = $"{folder.TrimEnd('/')}/{uniqueName}";

            // Đọc stream thành byte[]
            byte[] bytes;
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                bytes = memoryStream.ToArray();
            }

            await _supabaseClient.Storage
                .From(_settings.Bucket)
                .Upload(bytes, path);

            var publicUrl = _supabaseClient.Storage
                .From(_settings.Bucket)
                .GetPublicUrl(path);

            return Ok(new
            {
                path,
                url = publicUrl
            });
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

            // Nếu là full URL thì cắt thành relative path
            var bucketBaseUrl = _supabaseClient.Storage
                .From(_settings.Bucket)
                .GetPublicUrl(string.Empty);

            var relativePath = filePath.StartsWith(bucketBaseUrl, StringComparison.OrdinalIgnoreCase)
                ? filePath.Substring(bucketBaseUrl.Length).TrimStart('/')
                : filePath;

            await _supabaseClient.Storage
                .From(_settings.Bucket)
                .Remove(relativePath);

            return NoContent();
        }

        public class UploadRequest
        {
            public IFormFile file { get; set; } = null!;
            public string folder { get; set; } = "others";
        }
    }
}
