using System;
using System.Collections.Generic;
using System.Linq;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain;

public class ShoppingCart : Entity
{
    public long UserId { get; private set; }
    public List<CartItem> Items { get; private set; } = new();

    protected ShoppingCart() { }

    public ShoppingCart(long userId)
    {
        if (userId == 0) throw new ArgumentException("Invalid user id.");
        UserId = userId;
    }

    public void AddItem(long tourId, int quantity)
    {
        if (tourId == 0) throw new Exception("Invalid tour id.");
        if (quantity < 0) throw new Exception("Invalid quantity.");

        var existing = Items.FirstOrDefault(x => x.TourId == tourId);
        if (existing == null)
            Items.Add(new CartItem(tourId, quantity));
        else
            existing.Increase(quantity);
    }

    public void RemoveItem(long tourId)
    {
        Items.RemoveAll(x => x.TourId == tourId);
    }

    public decimal CalculateTotal(Func<long, decimal> priceResolver)
    {
        return Items.Sum(i => priceResolver(i.TourId) * i.Quantity);
    }
}