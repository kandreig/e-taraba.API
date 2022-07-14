using e_taraba.API.DbContexts;
using e_taraba.API.Entities;
using e_taraba.API.SearchParameters;
using Microsoft.EntityFrameworkCore;

namespace e_taraba.API.Services
{
    public class Repository : IRepository
    {
        private readonly e_tarabaContext context;
        const int maxItemsPerPage = 50;
        const int defaultItemsPerPage = 20;


        public Repository(e_tarabaContext _context)
        {
            this.context = _context ?? throw new ArgumentNullException(nameof(_context));
        }
        public async Task<(IEnumerable<Product>, Pagination)> GetProductsASync(ProductSearchParameters searchParams, Pagination pagination)
        {
            var collection = context.Products as IQueryable<Product>;

            if (pagination.ItemsOnPage == 0)
            {
                pagination.ItemsOnPage = defaultItemsPerPage;

            }
            if (pagination.CurrentPageNumber == 0)
            {
                pagination.CurrentPageNumber = 1;
            }

            if (pagination.ItemsOnPage > maxItemsPerPage)
            {
                pagination.ItemsOnPage = maxItemsPerPage;
            }

            if (searchParams.SearchQuery != null)
            {
                searchParams.SearchQuery.Trim();
                collection = collection
                    .Where(p =>
                        p.Name.Contains(searchParams.SearchQuery) ||
                        p.Description.Contains(searchParams.SearchQuery)
                        );
            }
            ////////////
            if(searchParams.PriceMin != 0 && searchParams.PriceMax != 0)
            {
                collection = collection
                    .Where( p=>
                        p.Price > searchParams.PriceMin &&
                        p.Price < searchParams.PriceMax
                        );
            }else if(searchParams.PriceMin != 0)
            {
                collection = collection
                    .Where(p =>
                        p.Price > searchParams.PriceMin    
                    );
            }
            else if(searchParams.PriceMax != 0)
            {
                collection = collection
                    .Where(p =>
                        p.Price < searchParams.PriceMax
                    );
            }

            pagination.TotalItemsNumber = await collection.CountAsync();

            var collectionToReturn = await collection
                                                .Skip((int)(pagination.ItemsOnPage * (pagination.CurrentPageNumber - 1)))
                                                .Take((int)pagination.ItemsOnPage)
                                                .ToListAsync();

            return (collectionToReturn, pagination);

        }
        public async Task<Product?> GetProductASync(int productId, bool includeOrders = false)
        {
            if (includeOrders)
            {
                return await context.Products
                    .Include(o => o.Orders)
                    .Where(p => p.Id == productId)
                    .FirstOrDefaultAsync();
            }
            return await context.Products.Where(p => p.Id == productId).FirstOrDefaultAsync();
        }
        public async Task<(IEnumerable<Order>, Pagination)> GetOrdersASync(OrderSearchParameters searchParams, Pagination pagination)
        {
            var collection = context.Orders as IQueryable<Order>;

            if (pagination.ItemsOnPage == 0)
            {
                pagination.ItemsOnPage = defaultItemsPerPage;

            }
            if (pagination.CurrentPageNumber == 0)
            {
                pagination.CurrentPageNumber = 1;
            }
            if (pagination.ItemsOnPage > maxItemsPerPage)
            {
                pagination.ItemsOnPage = maxItemsPerPage;
            }

            if (searchParams.FirstName != null && searchParams.LastName != null)
            {
                searchParams.FirstName.Trim();
                searchParams.LastName.Trim();
                collection = collection
                    .Where(p =>
                        p.CustomerFirstName == searchParams.FirstName &&
                        p.CustomerLastName == searchParams.LastName
                        );
            }
            else if(searchParams.FirstName != null)
            {
                searchParams.FirstName.Trim();
                collection = collection
                    .Where(p =>
                        p.CustomerFirstName == searchParams.FirstName
                        );
            }
            else if(searchParams.LastName != null)
            {
                searchParams.LastName.Trim();
                collection = collection
                    .Where(p =>
                        p.CustomerLastName == searchParams.LastName
                        );
            }

            if(searchParams.Phone != null)
            {
                searchParams.Phone.Trim();
                collection = collection
                    .Where(p =>
                        p.CustomerPhone == searchParams.Phone
                        );
            }
            
            pagination.TotalItemsNumber = await collection.CountAsync();

            var collectionToReturn = await collection
                                                .Skip((int)(pagination.ItemsOnPage * (pagination.CurrentPageNumber - 1)))
                                                .Take((int)pagination.ItemsOnPage)
                                                .ToListAsync();

            return (collectionToReturn, pagination);
        }
        public async Task<Order?> GetOrderASync(int orderId, bool includeProducts)
        {
            if (includeProducts)
            {
                return await context.Orders.Include(p => p.Products).Where(o => o.Id == orderId).FirstOrDefaultAsync();
            }
            return await context.Orders.Where(o => o.Id == orderId).FirstOrDefaultAsync();
        }
        public async Task<IEnumerable<Product>> GetProductsForOrderASync(int orderId)
        {
            var order = await context.Orders.Include(o => o.Products).Where(o => o.Id == orderId).FirstOrDefaultAsync();

            var productsOfOrder = order.Products;


            return productsOfOrder;
        }
        public async Task<ProductOrder?> GetProductOrderASync(int productOrderId)
        {
            var productOrder = await context.ProductOrders.Where(po => po.Id == productOrderId).FirstOrDefaultAsync();

            return productOrder;
        }
        public async Task<RefreshToken?> GetRefreshTokenByStringASync(string rToken)
        {
            return await context.RefreshTokens.Where(rt => rt.RToken == rToken).FirstOrDefaultAsync();
        }
        public async Task<bool>  OrderExistsAsync(int orderId)
        {
            return await context.Orders.AnyAsync(o => o.Id == orderId);
        }
        public async Task<bool> ProductExistsAsync(int productId)
        {
            return await context.Products.AnyAsync(p => p.Id == productId);
        }
        public async Task CreateProductASync(Product product)
        {
            context.Products.Add(product);
        }
        public async Task CreateOrderASync(Order order)
        {
            context.Orders.Add(order);
        }
        public async Task CreateProductOrderDetailsASync(ProductOrder productOrderDetails)
        {
            context.ProductOrders.Add(productOrderDetails);
        }
        public async Task CreateRefreshTokenASync(RefreshToken refreshToken)
        {
            context.RefreshTokens.Add(refreshToken);
        }
        public async void DeleteProductASync(Product product)
        {
            context.Products.Remove(product);
        }
        public async Task<IEnumerable<ProductOrder>> GetProductOrderDetailsForOrderASync(int orderId)
        {
            var order = await context.Orders.Include(o => o.ProductOrders).Where(o => o.Id == orderId).FirstOrDefaultAsync();
            var productOrderDetailsToReturn = order.ProductOrders;

            return productOrderDetailsToReturn;
        }
        public async Task<bool> SaveChangesASync()
        {
            return await context.SaveChangesAsync() >= 0;
        }

        public async Task<bool> UserExistsASync(string username)
        {
            return await context.Users.AnyAsync(u => u.Username == username);
        }
        public async Task<User?> GetUserByUsernameASync(string username, bool includeRToken = false)
        {
            if (includeRToken)
                return await context.Users.Include(u => u.RefreshTokens).Where(u => u.Username == username).FirstOrDefaultAsync();
            else 
                return await context.Users.Where(u => u.Username == username).FirstOrDefaultAsync();
        }
        public async Task<User?> GetUserASync(int userId, bool includeRToken)
        {
            if (includeRToken)
                return await context.Users.Include(u => u.RefreshTokens).Where(u => u.Id == userId).FirstOrDefaultAsync();
            else
                return await context.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();
        }
        public async Task CreateUserASync(User user)
        {
            context.Users.Add(user);
        }
        public async void DeleteUserASync(User user)
        {
            context.Users.Remove(user);
        }
    }
}
