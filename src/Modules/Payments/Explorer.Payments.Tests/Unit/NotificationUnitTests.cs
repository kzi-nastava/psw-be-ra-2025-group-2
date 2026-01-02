using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Payments.Core.Domain.Wallets;
using Shouldly;

namespace Explorer.Payments.Tests.Unit
{
    public class NotificationUnitTests
    {
        [Fact]
        public void Creates_with_valid_parameters()
        {
            var notification = new Notification(
                -21,
                "You received 100 Adventure Coins!",
                NotificationType.WalletDeposit
            );

            notification.TouristId.ShouldBe(-21);
            notification.Message.ShouldBe("You received 100 Adventure Coins!");
            notification.Type.ShouldBe(NotificationType.WalletDeposit);
            notification.IsRead.ShouldBeFalse();
            notification.CreatedAt.ShouldBeInRange(
                DateTime.UtcNow.AddSeconds(-5),
                DateTime.UtcNow.AddSeconds(5)
            );
        }

        [Fact]
        public void Creates_as_unread_by_default()
        {
            var notification = new Notification(
                -21,
                "Test message",
                NotificationType.General
            );

            notification.IsRead.ShouldBeFalse();
        }

        [Fact]
        public void Creation_fails_for_invalid_tourist_id()
        {
            Should.Throw<ArgumentException>(() =>
                new Notification(0, "Message", NotificationType.General));
        }

        [Fact]
        public void Creation_fails_for_empty_message()
        {
            Should.Throw<ArgumentException>(() =>
                new Notification(-21, "", NotificationType.General));
        }

        [Fact]
        public void Creation_fails_for_null_message()
        {
            Should.Throw<ArgumentException>(() =>
                new Notification(-21, null!, NotificationType.General));
        }

        [Fact]
        public void Creation_fails_for_whitespace_message()
        {
            Should.Throw<ArgumentException>(() =>
                new Notification(-21, "   ", NotificationType.General));
        }

        [Fact]
        public void Marks_as_read()
        {
            var notification = new Notification(
                -21,
                "Test message",
                NotificationType.WalletDeposit
            );

            notification.IsRead.ShouldBeFalse();

            notification.MarkAsRead();

            notification.IsRead.ShouldBeTrue();
        }

        [Fact]
        public void Mark_as_read_is_idempotent()
        {
            var notification = new Notification(
                -21,
                "Test message",
                NotificationType.General
            );

            notification.MarkAsRead();
            notification.IsRead.ShouldBeTrue();

            notification.MarkAsRead();
            notification.IsRead.ShouldBeTrue();
        }

        [Fact]
        public void Supports_different_notification_types()
        {
            var walletNotification = new Notification(
                -21,
                "Wallet update",
                NotificationType.WalletDeposit
            );

            var generalNotification = new Notification(
                -21,
                "General message",
                NotificationType.General
            );

            walletNotification.Type.ShouldBe(NotificationType.WalletDeposit);
            generalNotification.Type.ShouldBe(NotificationType.General);
        }

        [Fact]
        public void CreatedAt_is_set_automatically()
        {
            var before = DateTime.UtcNow;

            var notification = new Notification(
                -21,
                "Test message",
                NotificationType.General
            );

            var after = DateTime.UtcNow;

            notification.CreatedAt.ShouldBeGreaterThanOrEqualTo(before);
            notification.CreatedAt.ShouldBeLessThanOrEqualTo(after);
        }

        [Fact]
        public void Different_notifications_have_different_timestamps()
        {
            var notification1 = new Notification(
                -21,
                "First",
                NotificationType.General
            );

            System.Threading.Thread.Sleep(10); // Mali delay

            var notification2 = new Notification(
                -21,
                "Second",
                NotificationType.General
            );

            notification2.CreatedAt.ShouldBeGreaterThanOrEqualTo(notification1.CreatedAt);
        }

        [Fact]
        public void Notification_properties_are_immutable_except_IsRead()
        {
            var notification = new Notification(
                -21,
                "Original message",
                NotificationType.WalletDeposit
            );

            var originalTouristId = notification.TouristId;
            var originalMessage = notification.Message;
            var originalType = notification.Type;
            var originalCreatedAt = notification.CreatedAt;

            notification.MarkAsRead();

            // Sve osim IsRead treba da ostane isto
            notification.TouristId.ShouldBe(originalTouristId);
            notification.Message.ShouldBe(originalMessage);
            notification.Type.ShouldBe(originalType);
            notification.CreatedAt.ShouldBe(originalCreatedAt);
            notification.IsRead.ShouldBeTrue();
        }
    }
}
