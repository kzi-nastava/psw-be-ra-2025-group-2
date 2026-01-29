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
        public long? ChatId {  get; private set; }
        public long SenderId { get; private set; }
        public long? ReceiverId { get; private set; } //nema potrebe vise za ovim jer sve ide preko Chata
        public string Content { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public bool IsDeleted { get; private set; }

        private Message() { }

        //private message
        public Message(long senderId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Message content cannot be empty.");
            SenderId = senderId;
            Content = content;
            CreatedAt = DateTime.UtcNow;
            IsDeleted = false;
        }

        // PRIVATE CHAT
        public static Message CreatePrivate(long senderId, long receiverId, string content)
        {
            var message = new Message(senderId, content);
            message.ReceiverId = receiverId;
            message.ChatId = null;
            return message;
        }

        // CLUB / GROUP CHAT
        public static Message CreateChat(long chatId, long senderId, string content)
        {
            var message = new Message(senderId, content);
            message.ChatId = chatId;
            message.ReceiverId = null;
            return message;
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
