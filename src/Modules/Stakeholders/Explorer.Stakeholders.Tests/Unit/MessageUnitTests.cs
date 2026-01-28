using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Stakeholders.Core.Domain;
using Shouldly;

namespace Explorer.Stakeholders.Tests.Unit
{
    public class MessageUnitTests
    {
        [Fact]
        public void Creates_private_message()
        {
            // koristimo novu factory metodu za private message
            var msg = Message.CreatePrivate(senderId: 1, receiverId: 2, content: "Hello");

            msg.SenderId.ShouldBe(1);
            msg.ReceiverId.ShouldBe(2);
            msg.ChatId.ShouldBeNull(); // private message nema chat
            msg.Content.ShouldBe("Hello");
            msg.CreatedAt.ShouldNotBe(default);
        }

        [Fact]
        public void Creates_chat_message()
        {
            // klub/group chat
            var msg = Message.CreateChat(chatId: 10, senderId: 1, content: "Hello Group");

            msg.SenderId.ShouldBe(1);
            msg.ChatId.ShouldBe(10);
            msg.ReceiverId.ShouldBeNull(); // club/group chat nema receiver
            msg.Content.ShouldBe("Hello Group");
            msg.CreatedAt.ShouldNotBe(default);
        }

        [Fact]
        public void Edits_message()
        {
            var msg = Message.CreatePrivate(1, 2, "Old");
            msg.Edit(1, "New");

            msg.Content.ShouldBe("New");
            msg.UpdatedAt.ShouldNotBeNull();
        }

        [Fact]
        public void Delete_message_marks_as_deleted()
        {
            var msg = Message.CreatePrivate(1, 2, "Hello");
            msg.Delete(1);

            msg.IsDeleted.ShouldBeTrue();
        }

        [Fact]
        public void Edit_throws_if_not_sender()
        {
            var msg = Message.CreatePrivate(1, 2, "Old");
            Should.Throw<InvalidOperationException>(() => msg.Edit(2, "New"));
        }

        [Fact]
        public void Delete_throws_if_not_sender()
        {
            var msg = Message.CreatePrivate(1, 2, "Hello");
            Should.Throw<InvalidOperationException>(() => msg.Delete(2));
        }
    }
}

