using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SuperShop.Web.Data;
using SuperShop.Web.Data.Entities;
using SuperShop.Web.Helpers;
using SuperShop.Web.Models;

namespace SuperShop.Web.Controllers
{
    public class ProductsController : Controller
    {
        readonly IProductRepository _productRepository;
        readonly IUserHelper _userHelper;

        public ProductsController(IProductRepository productRepository, IUserHelper userHelper)
        {
            _productRepository = productRepository;
            _userHelper = userHelper;
        }

        // GET: Products
        [Route("Products")]
        [Route("Products/Index/{param}")]
        public IActionResult Index(string? param)
        {
            return View(_productRepository.GetAll().OrderBy(SortBy(param)));
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _productRepository.GetByIdAsync(id.Value);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel productViewModel)
        {
            if (ModelState.IsValid)
            {
                var path = await SaveImageFileAsync(productViewModel);

                var product = ToProduct(productViewModel, path);

                // TODO: Update user -> logged user
                product.User = await _userHelper.GetUserByEmailAsync("dario@e.mail");

                await _productRepository.CreateAsync(product);

                return RedirectToAction(nameof(Index));
            }
            return View(productViewModel);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _productRepository.GetByIdAsync(id.Value);
            if (product == null)
            {
                return NotFound();
            }

            var productViewModel = ToProductViewModel(product);

            return View(productViewModel);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductViewModel productViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var path = await SaveImageFileAsync(productViewModel);

                    var product = ToProduct(productViewModel, path);

                    await _productRepository.UpdateAsync(product);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await ProductExists(productViewModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(productViewModel);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _productRepository.GetByIdAsync(id.Value);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            await _productRepository.DeleteAsync(product);
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> ProductExists(int id)
        {
            return await _productRepository.ExistsAsync(id);
        }

        static async Task<string> SaveImageFileAsync(ProductViewModel productViewModel)
        {
            if (productViewModel.ImageFile == null || productViewModel.ImageFile.Length < 1)
                return productViewModel.ImageUrl ?? string.Empty;

            var guid = Guid.NewGuid().ToString();
            var file = $"{guid}.jpg";

            var path = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot\\images\\products",
                        file);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await productViewModel.ImageFile.CopyToAsync(stream);
            }

            return $"~/images/products/{file}";
        }

        static Func<Product, object> SortBy(string? param)
        {
            if (param == null)
                return p => p.Id;

            return param switch
            {
                nameof(Product.Name) => p => p.Name,
                nameof(Product.Price) => p => p.Price,
                nameof(Product.Stock) => p => p.Stock,
                _ => p => p.Id
            };
        }

        static ProductViewModel ToProductViewModel(Product product)
        {
            return new()
            {
                Id = product.Id,
                ImageUrl = product.ImageUrl,
                IsAvailable = product.IsAvailable,
                LastPurchase = product.LastPurchase,
                LastSale = product.LastSale,
                Name = product.Name,
                Price = product.Price,
                Stock = product.Stock,
                User = product.User
            };
        }

        static Product ToProduct(ProductViewModel productViewModel, string path)
        {
            //var p = (Product)productViewModel;
            return new Product()
            {
                Id = productViewModel.Id,
                ImageUrl = path,
                IsAvailable = productViewModel.IsAvailable,
                LastPurchase = productViewModel.LastPurchase,
                LastSale = productViewModel.LastSale,
                Name = productViewModel.Name,
                Price = productViewModel.Price,
                Stock = productViewModel.Stock,
                User = productViewModel.User
            };
        }
    }
}
