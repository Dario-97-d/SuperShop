﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SuperShop.Web.Data.Entities;
using SuperShop.Web.Helpers;
using SuperShop.Web.Models;

namespace SuperShop.Web.Data
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        private readonly DataContext _context;

        public OrderRepository(DataContext context) : base(context)
        {
            _context = context;
        }


        public async Task AddItemToOrderAsync(AddItemViewModel model, User user)
        {
            // Get Product

            var product = await _context.Products.FindAsync(model.ProductId);
            if (product == null) return;


            // Get Temporary Order Detail of Product

            var orderDetailTemp = await _context.OrderDetailsTemp
                .Where(odt => odt.User.Id == user.Id && odt.Product.Id == product.Id)
                .FirstOrDefaultAsync();


            // Check Temporary Order Details exists

            if (orderDetailTemp == null)
            {
                // If not exists

                orderDetailTemp = new OrderDetailTemp
                {
                    User = user,
                    Product = product,
                    Price = product.Price,
                    Quantity = model.Quantity
                };

                await _context.OrderDetailsTemp.AddAsync(orderDetailTemp);
            }
            else
            {
                // If exists

                orderDetailTemp.Quantity += model.Quantity;
                _context.OrderDetailsTemp.Update(orderDetailTemp);
            }

            await base.SaveAsync();
        }

        public async Task<bool> ConfirmOrderAsync(User user)
        {
            // Get Temporary Order Details

            var orderDetailsTemp = await _context.OrderDetailsTemp
                .Where(odt => odt.User.Id == user.Id)
                .Include(odt => odt.Product)
                .ToListAsync();

            // Check Temporary Order Details is null or empty

            if (orderDetailsTemp == null || orderDetailsTemp.Count == 0)
                return false;

            // Define Order Details for new Order from Temporary Order Details

            var orderDetails = orderDetailsTemp.Select(odt => new OrderDetail
            {
                Product = odt.Product,
                Price = odt.Price,
                Quantity = odt.Quantity
            }).ToList();

            // Define Order

            var order = new Order
            {
                User = user,
                OrderDate = DateTime.UtcNow,
                Items = orderDetails
            };

            // Create Order and Remove Temporary Order Details

            await base.CreateAsync(order);
            _context.OrderDetailsTemp.RemoveRange(orderDetailsTemp);

            // Save changes

            await base.SaveAsync();

            return true;
        }

        public async Task DeleteDetailTempAsync(int id)
        {
            var orderDetailTemp = await _context.OrderDetailsTemp.FindAsync(id);
            if (orderDetailTemp == null) return;

            _context.OrderDetailsTemp.Remove(orderDetailTemp);
            await base.SaveAsync();
        }

        public async Task DeliverOrder(DeliveryViewModel model)
        {
            var order = await _context.Orders.FindAsync(model.Id);
            if (order != null)
            {
                order.DeliveryDate = model.DeliveryDate;
                await base.SaveAsync();
            }
        }

        public async Task<Order> GetOrderAsync(int id)
        {
            return await _context.Orders.FindAsync(id);
        }

        public IQueryable<OrderDetailTemp> GetOrderDetailsTemp(User user)
        {
            return _context.OrderDetailsTemp
                .AsNoTracking()
                .Where(odt => odt.User.Id == user.Id)
                .Include(odt => odt.Product)
                .OrderBy(odt => odt.Product.Name);
        }

        public IQueryable<Order> GetOrders(User user)
        {
            return _context.Orders.AsNoTracking()
                .Where(o => o.User.Id == user.Id)
                .OrderByDescending(o => o.OrderDate)
                .Include(o => o.Items)
                .ThenInclude(i => i.Product);
        }

        public IQueryable<Order> GetOrdersAdmin()
        {
            return _context.Orders.AsNoTracking()
                .OrderByDescending(o => o.OrderDate)
                .Include(o => o.User)
                .Include(o => o.Items)
                .ThenInclude(i => i.Product);
        }

        public async Task ModifyOrderDetailTempQuantityAsync(int id, double quantity)
        {
            var orderDetailTemp = await _context.OrderDetailsTemp.FindAsync(id);
            if (orderDetailTemp == null)
                return;

            orderDetailTemp.Quantity += quantity;
            if (orderDetailTemp.Quantity > 0)
            {
                _context.OrderDetailsTemp.Update(orderDetailTemp);
                await base.SaveAsync();
            }
        }
    }
}
