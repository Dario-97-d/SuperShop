using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
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
        [Authorize]
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

            var productViewModel = _converterHelper.ToProductViewModel(product);

            return View(productViewModel);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
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
        [Authorize]
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
        [Authorize]
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

        async Task<Product> PrepareForCreateOrUpdate(ProductViewModel productViewModel)
        {
            var imageId = await SaveImageFileAsync(productViewModel.ImageFile);
            // TODO: Update user -> logged user
            var user = await _userHelper.GetUserByEmailAsync("dario@e.mail");

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

    }
}
