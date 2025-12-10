using Explorer.ShoppingCart.Core.Domain;
using Explorer.ShoppingCart.Core.Interfaces;
using Explorer.Tours.Infrastructure.Database; 
using Microsoft.EntityFrameworkCore;       

namespace Explorer.ShoppingCart.Infrastructure.Repositories
{
    public class ShoppingCartRepository : IShoppingCartRepository
    {
        private readonly ToursContext _dbContext;

        public ShoppingCartRepository(ToursContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Cart Create(Cart shoppingCart)
        {
            _dbContext.ShoppingCarts.Add(shoppingCart);
            _dbContext.SaveChanges();
            return shoppingCart;
        }

        public Cart GetByTouristId(long touristId)
        {
            return _dbContext.ShoppingCarts
                .Include(sc => sc.Items)
                .FirstOrDefault(sc => sc.TouristId == touristId);
        }

        public Cart Update(Cart shoppingCart)
        {
            _dbContext.ShoppingCarts.Update(shoppingCart);
            _dbContext.SaveChanges();
            return shoppingCart;
        }
    }
}