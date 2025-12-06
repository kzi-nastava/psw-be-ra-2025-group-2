using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.Core.Domain.ShoppingCarts;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories;

public class ShoppingCartRepository : IShoppingCartRepository
{
    private readonly StakeholdersContext _context;

    public ShoppingCartRepository(StakeholdersContext context)
    {
        _context = context;
    }

    public ShoppingCart? GetByTouristId(long touristId)
    {
        return _context.ShoppingCarts
            .Include(c => c.Items) 
            .FirstOrDefault(c => c.TouristId == touristId);
    }

    public ShoppingCart Create(ShoppingCart cart)
    {
        _context.ShoppingCarts.Add(cart);
        _context.SaveChanges();
        return cart;
    }

    public ShoppingCart Update(ShoppingCart cart)
    {
        _context.ShoppingCarts.Update(cart);
        _context.SaveChanges();
        return cart;
    }

    public void Delete(ShoppingCart cart)
    {
        _context.ShoppingCarts.Remove(cart);
        _context.SaveChanges();
    }
}