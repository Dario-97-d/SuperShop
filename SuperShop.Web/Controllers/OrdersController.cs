using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperShop.Web.Data;
using SuperShop.Web.Models;

namespace SuperShop.Web.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;

        public OrdersController(IOrderRepository orderRepository, IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
        }


        public async Task<IActionResult> Index()
        {
            var model = await _orderRepository.GetOrdersAsync(User.Identity.Name);
            return View(model);
        }

        public IActionResult AddProduct()
        {
            var model = new AddItemViewModel
            {
                Products = _productRepository.GetComboProducts(),
                Quantity = 1
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(AddItemViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _orderRepository.AddItemToOrderAsync(model, User.Identity.Name);
                return RedirectToAction(nameof(Create));
            }

            ModelState.AddModelError(string.Empty, "Could not add Product.");
            return View(model);
        }

        public async Task<IActionResult> Create()
        {
            var model = await _orderRepository.GetOrderDetailsTempAsync(User.Identity.Name);
            return View(model);
        }

        public async Task<IActionResult> Decrement(int? id)
        {
            if (id == null) return NotFound();

            await _orderRepository.ModifyOrderDetailTempQuantityAsync(id.Value, -1);
            return RedirectToAction(nameof(Create));
        }

        public async Task<IActionResult> DeleteItem(int? id)
        {
            if (id == null) return NotFound();

            await _orderRepository.DeleteDetailTempAsync(id.Value);
            return RedirectToAction(nameof(Create));
        }

        public async Task<IActionResult> Increment(int? id)
        {
            if (id == null) return NotFound();

            await _orderRepository.ModifyOrderDetailTempQuantityAsync(id.Value, 1);
            return RedirectToAction(nameof(Create));
        }
    }
}
