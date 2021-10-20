using System;
using System.Collections.Generic;

using DSharpPlus.Entities;

using Microsoft.EntityFrameworkCore;

namespace Hanna.Cosmos.Entitys
{
    public class LastMessage {

		public LastMessage() { }
		public LastMessage(DiscordMessage msg) {
			this.LastMessageId = msg.Id;
			this.AuthorId = msg.Author.Id;
			this.ChannelId = msg.Channel.Id;
			this.ReferenceId = msg.ReferencedMessage == null ? 0 : msg.ReferencedMessage.Id;
			this.Content = msg.Content;
			this.Timestamp = msg.Timestamp;

			this.Attachments = new List<LastMessageAttachment>();
			foreach (DiscordAttachment attachment in msg.Attachments)
				this.Attachments
					.Add(new LastMessageAttachment(attachment));

		}
		public ulong LastMessageId { get; init; }
		public ulong AuthorId { get; init; }
		public WCUser Author { get; init; }
		public ulong ChannelId { get; init; }
		public ulong ReferenceId { get; init; }
		public string Content { get; init; }
        public List<LastMessageAttachment> Attachments { get; init; }
		public DateTimeOffset Timestamp { get; init; }

		[Owned]
		public class LastMessageAttachment {
			public LastMessageAttachment() { }
			public LastMessageAttachment(DiscordAttachment attachment) {
				this.Url = attachment.Url;
				this.ProxyUrl = attachment.ProxyUrl;
			}

			public string Url { get; init; }
			public string ProxyUrl { get; init; }
		}
	}
}