using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperShop.Web.Data;
using SuperShop.Web.Helpers.Interfaces;
using SuperShop.Web.ViewModels;

namespace SuperShop.Web.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly IUnitOfWork _unitOfWork;

        public OrdersController(IUserHelper userHelper, IUnitOfWork unitOfWork)
        {
            _userHelper = userHelper;
            _unitOfWork = unitOfWork;
        }


        public async Task<IActionResult> Index()
        {
            var user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);
            if (user == null) return NotFound();

            var model = await _userHelper.IsInRoleAsync(user, "Admin") ?
                _unitOfWork.OrderRepository.GetOrdersAdmin() :
                _unitOfWork.OrderRepository.GetOrders(user);

            return View(model);
        }

        public async Task<IActionResult> AddProduct()
        {
            var model = new AddItemViewModel
            {
                Products = await _unitOfWork.ProductRepository.GetComboProductsAsync(),
                Quantity = 1
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(AddItemViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);
                if (user == null) return NotFound();

                await _unitOfWork.OrderRepository.AddItemToOrderAsync(model, user);
                return RedirectToAction(nameof(Create));
            }

            ModelState.AddModelError(string.Empty, "Could not add Product.");
            return View(model);
        }

        public async Task<IActionResult> ConfirmOrder()
        {
            var user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);
            if (user == null) return NotFound();

            var success = await _unitOfWork.OrderRepository.ConfirmOrderAsync(user);

            if (success)
            {
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Create));
        }

        public async Task<IActionResult> Create()
        {
            var user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);
            if (user == null) return NotFound();

            var model = _unitOfWork.OrderRepository.GetOrderDetailsTemp(user);
            return View(model);
        }

        public async Task<IActionResult> Decrement(int? id)
        {
            if (id == null) return NotFound();

            await _unitOfWork.OrderRepository.ModifyOrderDetailTempQuantityAsync(id.Value, -1);
            return RedirectToAction(nameof(Create));
        }

        public async Task<IActionResult> DeleteItem(int? id)
        {
            if (id == null) return NotFound();

            await _unitOfWork.OrderRepository.DeleteDetailTempAsync(id.Value);
            return RedirectToAction(nameof(Create));
        }

        public async Task<IActionResult> Deliver(int? id)
        {
            if (id == null)
                return NotFound();

            var order = await _unitOfWork.OrderRepository.GetOrderAsync(id.Value);
            if (order == null)
                return NotFound();

            var model = new DeliveryViewModel
            {
                Id = order.Id,
                DeliveryDate = System.DateTime.Today
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Deliver(DeliveryViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWork.OrderRepository.DeliverOrder(model);
                TempData["UserMessage"] = "Delivered.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, "Could not deliver order.");
            return View(model);
        }

        public async Task<IActionResult> Increment(int? id)
        {
            if (id == null) return NotFound();

            await _unitOfWork.OrderRepository.ModifyOrderDetailTempQuantityAsync(id.Value, +1);
            return RedirectToAction(nameof(Create));
        }
    }
}
