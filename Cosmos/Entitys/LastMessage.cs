using System;

using DSharpPlus.Entities;


namespace Hanna.Cosmos.Entitys
{
    public class LastMessage {

		public LastMessage(DiscordMessage msg) {
			this.Id = msg.Id;
			this.ChannelId = msg.Channel.Id;
			this.ReferenceId = msg.ReferencedMessage == null ? 0 : msg.ReferencedMessage.Id;
			this.Timestamp = msg.Timestamp;
			this.AttachmentsUrl = new string[msg.Attachments.Count];

			foreach (Attachment item in collection)
			{

			}
		}
        public ulong Id { get; init; }
		public ulong AuthorId { get; init; }
		public WCUser Author { get; init; }
		public ulong ChannelId { get; init; }
		public ulong ReferenceId { get; init; }
		public string Content { get; init; }
        public string[] AttachmentsUrl { get; init; }
		public DateTimeOffset Timestamp { get; init; }
	}
}