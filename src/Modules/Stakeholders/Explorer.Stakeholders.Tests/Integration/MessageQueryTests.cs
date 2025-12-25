using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.API.Controllers.Stakeholders;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Security.Claims;
using Explorer.Stakeholders.API.Dtos.Messages;

namespace Explorer.Stakeholders.Tests.Integration
{
    public class MessageQueryTests : BaseStakeholdersIntegrationTest
    {
        public MessageQueryTests(StakeholdersTestFactory factory) : base(factory) { }

        [Fact]
        public void Gets_my_messages()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "1");

            var result = controller.GetMyMessages().Result.ShouldBeOfType<OkObjectResult>();
            var messages = result.Value.ShouldBeOfType<List<MessageDto>>();

            messages.Count.ShouldBeGreaterThan(0);
            messages.Any(m => m.SenderId == 1 || m.ReceiverId == 1).ShouldBeTrue();
        }

        [Fact]
        public void Gets_conversation_ordered()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "1");

            var result = controller.Conversation(2).Result.ShouldBeOfType<OkObjectResult>();
            var messages = result.Value.ShouldBeOfType<List<MessageDto>>();

            messages.Select(m => m.CreatedAt).ShouldBeInOrder();
        }

        private static MessageController CreateController(IServiceScope scope, string userId)
        {
            return new MessageController(
                scope.ServiceProvider.GetRequiredService<IMessageService>())
            {
                ControllerContext = BuildContext(userId)
            };
        }

        private static ControllerContext BuildContext(string id)
        {
            var claims = new List<Claim>
            {
                new Claim("personId", id),
                new Claim("id", id)
            };

            return new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(claims, "test"))
                }
            };
        }
    }
}

