using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SuperShop.Web.Data.Entities;
using SuperShop.Web.Data.Repository.Interfaces;
using SuperShop.Web.Helpers.Interfaces;
using SuperShop.Web.Utils;
using SuperShop.Web.ViewModels;

namespace SuperShop.Web.Controllers
{
    public class ProductsController : Controller
    {
        // Repository
        readonly IProductRepository _productRepository;

        // Helpers

        readonly IBlobHelper _blobHelper;
        readonly IConverterHelper _converterHelper;
        readonly IUserHelper _userHelper;

        public ProductsController(IProductRepository productRepository,
            IBlobHelper blobHelper, IConverterHelper converterHelper, IUserHelper userHelper)
        {
            _productRepository = productRepository;
            _converterHelper = converterHelper;
            _blobHelper = blobHelper;
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
                return new NotFoundViewResult("ProductNotFound");
            }

            var product = await _productRepository.GetByIdAsync(id.Value);
            if (product == null)
            {
                return new NotFoundViewResult("ProductNotFound");
            }

            return View(product);
        }

        // GET: Products/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel productViewModel)
        {
            if (ModelState.IsValid)
            {
                var product = await PrepareForCreateOrUpdate(productViewModel);

                await _productRepository.CreateAsync(product);

                return RedirectToAction(nameof(Index));
            }
            return View(productViewModel);
        }

        // GET: Products/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("ProductNotFound");
            }

            var product = await _productRepository.GetByIdAsync(id.Value);
            if (product == null)
            {
                return new NotFoundViewResult("ProductNotFound");
            }

            var productViewModel = _converterHelper.ToProductViewModel(product);

            return View(productViewModel);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductViewModel productViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var product = await PrepareForCreateOrUpdate(productViewModel);

                    await _productRepository.UpdateAsync(product);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await ProductExists(productViewModel.Id))
                    {
                        return new NotFoundViewResult("ProductNotFound");
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("ProductNotFound"); ;
            }

            var product = await _productRepository.GetByIdAsync(id.Value);
            if (product == null)
            {
                return new NotFoundViewResult("ProductNotFound");
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);

            try
            {
                await _productRepository.DeleteAsync(product);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                if (ex.InnerException?.Message.Contains("DELETE") ?? false)
                {
                    ViewBag.ErrorTitle = $"Cannot delete this {nameof(Product)}";
                    ViewBag.ErrorMessage =
                        $"This {product.Name} is included in some object relationship." +
                        $" Check and clear any relationships with this {product.Name} and try again." +
                        $"<br />" +
                        $"<br />" +
                        $"Possible relationships include those with:" +
                        $"<br />" +
                        $"- {nameof(Order)}s";
                }

                return View("Error");
            }
        }

        private async Task<bool> ProductExists(int id)
        {
            return await _productRepository.ExistsAsync(id);
        }

        async Task<Product> PrepareForCreateOrUpdate(ProductViewModel productViewModel)
        {
            var imageId = await SaveImageFileAsync(productViewModel.ImageFile);

            var user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);

            return new Product(productViewModel)
            {
                ImageId = imageId,
                User = user
            };
        }

        async Task<Guid> SaveImageFileAsync(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length < 1)
                return Guid.Empty;

            return await _blobHelper.UploadBlobAsync(imageFile, "products");
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

        public IActionResult ProductNotFound()
        {
            return View();
        }

    }
}
