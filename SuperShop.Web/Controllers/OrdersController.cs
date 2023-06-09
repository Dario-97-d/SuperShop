﻿using System.Threading.Tasks;
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

        public async Task<IActionResult> AddProduct()
        {
            var model = new AddItemViewModel
            {
                Products = await _productRepository.GetComboProductsAsync(),
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

        public async Task<IActionResult> ConfirmOrder()
        {
            var success = await _orderRepository.ConfirmOrderAsync(User.Identity.Name);

            if (success)
            {
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Create));
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

        public async Task<IActionResult> Deliver(int? id)
        {
            if (id == null)
                return NotFound();

            var order = await _orderRepository.GetOrderAsync(id.Value);
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
                await _orderRepository.DeliverOrder(model);
                TempData["UserMessage"] = "Delivered.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, "Could not deliver order.");
            return View(model);
        }

        public async Task<IActionResult> Increment(int? id)
        {
            if (id == null) return NotFound();

            await _orderRepository.ModifyOrderDetailTempQuantityAsync(id.Value, 1);
            return RedirectToAction(nameof(Create));
        }
    }
}
