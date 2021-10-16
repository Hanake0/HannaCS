using System.ComponentModel.DataAnnotations;

using DSharpPlus.Entities;


namespace Hanna.Cosmos.Entitys {
	public class WCUser {

		public WCUser(ulong id, DiscordMessage lastMessage) {
			this.Id = id;
			this.Wallet = new Wallet() {
				Coins = 0,
				Gems = 0,
			};
			this.Inventory = new Inventory() { };
			this.LastMessage = new LastMessage(lastMessage);
		}

        [Required]
		public ulong Id { get; init; }

		[Required]
		public LastMessage LastMessage { get; init;}

		[Required]
		public Wallet Wallet { get; init; }

		[Required]
		public Inventory Inventory { get; init; }
	}
}