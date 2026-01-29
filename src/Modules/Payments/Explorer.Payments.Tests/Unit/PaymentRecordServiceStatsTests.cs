using System;
using System.Collections.Generic;
using System.Reflection;
using Moq;
using Shouldly;
using Xunit;
using Explorer.Payments.Core.UseCases;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Explorer.Payments.Core.Domain.ShoppingCarts;
using Explorer.Tours.API.Internal;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.Core.Domain.ShoppingCarts;

namespace Explorer.Payments.Tests.Unit
{
    public class PaymentRecordServiceTourStatsTests
    {
        private readonly Mock<IPaymentRecordRepository> _paymentRecordRepo = new();
        private readonly Mock<IShoppingCartRepository> _cartRepo = new();
        private readonly Mock<ITourStatisticsService> _tourStats = new();

        private PaymentRecordService CreateSut()
            => new PaymentRecordService(_paymentRecordRepo.Object, _cartRepo.Object, _tourStats.Object);

        [Fact]
        public void Checkout_throws_when_cart_not_found()
        {
            _cartRepo.Setup(x => x.GetByTouristId(1)).Returns((ShoppingCart)null);

            var sut = CreateSut();

            Should.Throw<KeyNotFoundException>(() => sut.Checkout(1));
        }

        [Fact]
        public void Checkout_throws_when_cart_empty()
        {
            var cart = new ShoppingCart(1);
            SetPropertyIfExists(cart, "Id", 123L);

            _cartRepo.Setup(x => x.GetByTouristId(1)).Returns(cart);

            var sut = CreateSut();

            Should.Throw<InvalidOperationException>(() => sut.Checkout(1));
        }

        [Fact]
        public void Checkout_increments_purchases_for_single_tour_item()
        {
            var cart = new ShoppingCart(1);
            SetPropertyIfExists(cart, "Id", 123L);

            var price = new Money(10m, "AC");

            cart.AddItem(
                tourId: 5,
                tourName: "Tour",
                price: price,
                tourStatus: "Published",
                authorId: 11
            );

            _cartRepo.Setup(x => x.GetByTouristId(1)).Returns(cart);

            var sut = CreateSut();

            sut.Checkout(1);

            _paymentRecordRepo.Verify(x => x.Create(It.IsAny<PaymentRecord>()), Times.Once);
            _tourStats.Verify(x => x.IncrementPurchases(5), Times.Once);
            _cartRepo.Verify(x => x.Update(It.IsAny<ShoppingCart>()), Times.Once);

            cart.Items.Count.ShouldBe(0);
        }

        [Fact]
        public void Checkout_increments_purchases_for_each_tour_in_bundle_item()
        {
            var cart = new ShoppingCart(1);
            SetPropertyIfExists(cart, "Id", 123L);

            var price = new Money(30m, "AC");
            var tourIds = new List<long> { 10, 11, 12 };

            cart.AddBundleItem(
                bundleId: 99,
                bundleName: "Bundle",
                price: price,
                tourIds: tourIds,
                authorId: 7
            );

            _cartRepo.Setup(x => x.GetByTouristId(1)).Returns(cart);

            var sut = CreateSut();

            sut.Checkout(1);

            _paymentRecordRepo.Verify(x => x.Create(It.IsAny<PaymentRecord>()), Times.Once);
            _tourStats.Verify(x => x.IncrementPurchases(10), Times.Once);
            _tourStats.Verify(x => x.IncrementPurchases(11), Times.Once);
            _tourStats.Verify(x => x.IncrementPurchases(12), Times.Once);
            _cartRepo.Verify(x => x.Update(It.IsAny<ShoppingCart>()), Times.Once);

            cart.Items.Count.ShouldBe(0);
        }

        private static void SetPropertyIfExists(object obj, string propName, object value)
        {
            var p = obj.GetType().GetProperty(propName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (p != null && p.CanWrite)
            {
                p.SetValue(obj, value);
                return;
            }

            var f = obj.GetType().GetField($"<{propName}>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
            if (f != null)
            {
                f.SetValue(obj, value);
                return;
            }

            f = obj.GetType().GetField(propName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (f != null)
                f.SetValue(obj, value);
        }
    }
}
