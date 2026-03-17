using _2380600659_HieuNguyen.Models;
using _2380600659_HieuNguyen.Repositories;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;

namespace _2380600659_HieuNguyen.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        public ProductController(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }
        // Hiển thị danh sách sản phẩm
        // Thêm tham số int? categoryId
        public async Task<IActionResult> Index(int? categoryId)
        {
            var products = await _productRepository.GetAllAsync();

            // 1. Lọc sản phẩm theo danh mục
            if (categoryId != null && categoryId > 0)
            {
                products = products.Where(p => p.CategoryId == categoryId);
            }

            // 2. Lấy danh sách danh mục gửi ra View làm Sidebar
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = categories;
            ViewBag.CurrentCategoryId = categoryId; // Để highlight danh mục đang chọn

            return View(products);
        }



        //Nhớ tạo folder images trong wwwroot
        // Hiển thị thông tin chi tiết sản phẩm
        public async Task<IActionResult> Display(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
        
        
    }
}
