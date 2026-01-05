using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Payments.Core.Domain.Wallets
{
    public class Notification : Entity
    {
        public long TouristId { get; private set; }
        public string Message { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public bool IsRead { get; private set; }
        public NotificationType Type { get; private set; }

        private Notification() { }

        public Notification(long touristId, string message, NotificationType type)
        {
            if (touristId == 0)
                throw new ArgumentException("Invalid tourist id");

            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Message cannot be empty");

            TouristId = touristId;
            Message = message;
            Type = type;
            CreatedAt = DateTime.UtcNow;
            IsRead = false;
        }

        public void MarkAsRead()
        {
            IsRead = true;
        }
    }

    public enum NotificationType
    {
        WalletDeposit,
        General
    }
}
