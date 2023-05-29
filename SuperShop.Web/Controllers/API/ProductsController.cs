using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SuperShop.Web.Data;

namespace SuperShop.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : Controller
    {
        readonly IProductRepository _productRepository;

        public ProductsController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_productRepository.GetAll());
        }
    }
}
