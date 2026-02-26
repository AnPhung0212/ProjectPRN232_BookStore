using BookStore.BusinessObject.DTO;
using BookStore.MVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace BookStore.MVC.Controllers
{
    public class ProductManageController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public ProductManageController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("BookStoreAPI");
            var products = await client.GetFromJsonAsync<List<ProductDTO>>("Product");
            var categories = await client.GetFromJsonAsync<List<CategoryDTO>>("Category");
            foreach (var p in products)
            {
                var category = categories.FirstOrDefault(c => c.CategoryId == p.CategoryId);
                if (category != null)
                {
                    p.CategoryName = category.CategoryName; // Đảm bảo ProductDTO có thêm thuộc tính CategoryName
                }
            }
            return View(products);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var client = _httpClientFactory.CreateClient("BookStoreAPI");
            var categories = await client.GetFromJsonAsync<List<CategoryDTO>>("Category");

            var model = new ProductCreateViewModel
            {
                Categories = categories
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var client = _httpClientFactory.CreateClient("BookStoreAPI");
                model.Categories = await client.GetFromJsonAsync<List<CategoryDTO>>("Category");
              
                return View(model);
            }

            var clientPost = _httpClientFactory.CreateClient("BookStoreAPI");
            // Lấy token từ Session
            var token = HttpContext.Session.GetString("JWToken");
            if (!string.IsNullOrEmpty(token))
            {
                clientPost.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            // 1. Upload ảnh nếu có
            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                using var form = new MultipartFormDataContent();

                await using var stream = model.ImageFile.OpenReadStream();
                var fileContent = new StreamContent(stream);
                fileContent.Headers.ContentType =
                    new MediaTypeHeaderValue(model.ImageFile.ContentType);

                form.Add(fileContent, "file", model.ImageFile.FileName);
                form.Add(new StringContent("books"), "folder");

                var uploadResponse = await clientPost.PostAsync("Storage/upload", form);
                if (!uploadResponse.IsSuccessStatusCode)
                {
                    ModelState.AddModelError(string.Empty, "Upload ảnh thất bại.");
                    model.Categories = await clientPost.GetFromJsonAsync<List<CategoryDTO>>("Category");
                    return View(model);
                }

                var uploadResult =
                    await uploadResponse.Content.ReadFromJsonAsync<UploadResult>();

                if (uploadResult != null)
                {
                    // Gán URL ảnh vào DTO gửi sang Product API
                    model.Product.ImageUrl = uploadResult.url;
                }
            }
            var response = await clientPost.PostAsJsonAsync("Product", model.Product);
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Tạo sản phẩm thành công";
                return RedirectToAction("Index");
            }

            // Nếu lỗi từ API
            var errorContent = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, $"Tạo sản phẩm thất bại ({response.StatusCode}): {errorContent}");
            var categories = await clientPost.GetFromJsonAsync<List<CategoryDTO>>("Category");
            model.Categories = categories;
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var client = _httpClientFactory.CreateClient("BookStoreAPI");
            var token = HttpContext.Session.GetString("JWToken");
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            var response = await client.DeleteAsync($"Product/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Xóa sản phẩm thành công!";
            }
            else
            {
                TempData["Error"] = "Xóa sản phẩm thất bại.";
            }

            return RedirectToAction("Index");
        }

        [HttpGet()]
        public async Task<IActionResult> Edit(int id)
        {
            var token = HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "User");

            var client = _httpClientFactory.CreateClient("BookStoreAPI");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync($"Product/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var productJson = await response.Content.ReadAsStringAsync();
            var product = JsonConvert.DeserializeObject<UpdateProductDTO>(productJson);

            var categories = await client.GetFromJsonAsync<List<CategoryDTO>>("Category");

            var viewModel = new ProductCreateViewModel
            {
                ProductEdit = product,
                Categories = categories
            };

            return View(viewModel);
        }

        [HttpPost()]
        public async Task<IActionResult> Edit(int id, ProductCreateViewModel model)
        {
            var token = HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "User");

            var client = _httpClientFactory.CreateClient("BookStoreAPI");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            // Nếu user chọn ảnh mới thì upload và gán lại ImageUrl
            if (model.ImageFile != null && model.ImageFile.Length > 0 && model.ProductEdit != null)
            {
                using var form = new MultipartFormDataContent();

                await using var stream = model.ImageFile.OpenReadStream();
                var fileContent = new StreamContent(stream);
                fileContent.Headers.ContentType =
                    new MediaTypeHeaderValue(model.ImageFile.ContentType);

                form.Add(fileContent, "file", model.ImageFile.FileName);
                form.Add(new StringContent("books"), "folder");

                var uploadResponse = await client.PostAsync("Storage/upload", form);
                if (!uploadResponse.IsSuccessStatusCode)
                {
                    ModelState.AddModelError(string.Empty, "Upload ảnh thất bại.");
                    model.Categories = await client.GetFromJsonAsync<List<CategoryDTO>>("Category");
                    return View(model);
                }

                var uploadResult =
                    await uploadResponse.Content.ReadFromJsonAsync<UploadResult>();

                if (uploadResult != null)
                {
                    model.ProductEdit.ImageUrl = uploadResult.url;
                }
            }
            //update logic
            var json = JsonConvert.SerializeObject(model.ProductEdit);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"Product/{model.ProductEdit.ProductId}", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Cập nhật sản phẩm thành công!";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError(string.Empty, "Cập nhật sản phẩm thất bại");
            // nạp lại categories để view hiển thị dropdown đầy đủ
            model.Categories = await client.GetFromJsonAsync<List<CategoryDTO>>("Category");
            return View(model);
        }

    }
    internal sealed class UploadResult
    {
        public string path { get; set; } = string.Empty;
        public string url { get; set; } = string.Empty;
    }
}
