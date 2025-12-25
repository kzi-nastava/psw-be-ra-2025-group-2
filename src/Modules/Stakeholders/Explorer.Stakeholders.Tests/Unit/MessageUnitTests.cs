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
        public void Creates_message()
        {
            var msg = new Message(1, 2, "Hello");

            msg.SenderId.ShouldBe(1);
            msg.ReceiverId.ShouldBe(2);
            msg.Content.ShouldBe("Hello");
            msg.CreatedAt.ShouldNotBe(default);
        }

        [Fact]
        public void Edits_message()
        {
            var msg = new Message(1, 2, "Old");
            msg.Edit(1, "New");

            msg.Content.ShouldBe("New");
            msg.UpdatedAt.ShouldNotBeNull();
        }
    }
}

