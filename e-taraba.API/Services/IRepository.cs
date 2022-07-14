using e_taraba.API.Entities;
using e_taraba.API.SearchParameters;

namespace e_taraba.API.Services
{
    public interface IRepository
    {
        Task<(IEnumerable<Product>, Pagination)> GetProductsASync(ProductSearchParameters searchParams, Pagination pagination);
        Task<Product?> GetProductASync(int productId, bool includeOrders = false);
        Task<(IEnumerable<Order>, Pagination)> GetOrdersASync(OrderSearchParameters searchParams, Pagination pagination);
        Task<Order?> GetOrderASync(int orderId, bool includeProducts = false);
        Task<IEnumerable<Product>> GetProductsForOrderASync(int orderId);
        Task<ProductOrder?> GetProductOrderASync(int productOrderId);
        Task<RefreshToken?> GetRefreshTokenByStringASync(string rToken);
        Task<bool> OrderExistsAsync(int orderId);
        Task<bool> ProductExistsAsync(int productId);
        Task CreateProductASync(Product product);
        Task CreateOrderASync(Order order);
        Task CreateProductOrderDetailsASync(ProductOrder productOrderDetails);
        Task CreateRefreshTokenASync(RefreshToken refreshToken);
        void DeleteProductASync(Product product);
        Task<IEnumerable<ProductOrder>> GetProductOrderDetailsForOrderASync(int orderId);
        Task<bool> SaveChangesASync();

        Task<bool> UserExistsASync(string username);
        Task<User?> GetUserASync(int userId, bool includeRToken = false);
        Task<User?> GetUserByUsernameASync(string username, bool includeRToken = false);
        Task CreateUserASync(User user);
        void DeleteUserASync(User user);
    }
}
