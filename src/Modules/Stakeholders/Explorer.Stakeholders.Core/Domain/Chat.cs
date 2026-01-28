using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain
{
    public class Chat : AggregateRoot
    {
        public ChatType ChatType { get; private set; }
        public long? ClubId { get; private set; }

        private Chat() { }
        private Chat(ChatType chatType, long? clubId)
        {
            ChatType = chatType;
            ClubId = clubId;
        }

        public static Chat CreatePrivate() => new Chat(ChatType.Private, null);

        public static Chat CreateClub(long clubId) => new Chat(ChatType.Club, clubId);
    }
}
