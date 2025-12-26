using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain
{
    public class Message : AggregateRoot
    {
        public long SenderId { get; private set; }
        public long ReceiverId { get; private set; }
        public string Content { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public bool IsDeleted { get; private set; }

        private Message() { }

        public Message(long senderId, long receiverId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Message content cannot be empty.");

            SenderId = senderId;
            ReceiverId = receiverId;
            Content = content;
            CreatedAt = DateTime.UtcNow;
            IsDeleted = false;
        }

        public void Edit(long userId, string newContent)
        {
            if (userId != SenderId)
                throw new InvalidOperationException("Only sender can edit message");

            if (IsDeleted)
                throw new InvalidOperationException("Cannot edit deleted message");

            Content = newContent;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Delete(long userId)
        {
            if (userId != SenderId)
                throw new InvalidOperationException("Only sender can delete message.");

            IsDeleted = true;
        }
    }
}
