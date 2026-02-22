using System.Threading.Tasks;

namespace SuperShop.Web.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _context;

        private CountryRepository _countryRepository;
        private OrderRepository _orderRepository;
        private ProductRepository _productRepository;

        public UnitOfWork(DataContext context)
        {
            _context = context;
        }

        public CountryRepository CountryRepository => _countryRepository ??= new(_context);
        public OrderRepository OrderRepository => _orderRepository ??= new(_context);
        public ProductRepository ProductRepository => _productRepository ??= new(_context);

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
