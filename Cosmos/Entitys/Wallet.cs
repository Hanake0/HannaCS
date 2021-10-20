using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Hanna.Cosmos.Entitys {
	public class Wallet {

		public Wallet() { }
		public Wallet(WCUser user) {
			this.User = user;
			this.Coins = 0;
			this.Gems = 0;
		}

        public ulong WalletId { get; init; }

		public WCUser User { get; init; }

        [Required]
        public long Coins { get; set; }

        [Required]
        public long Gems { get; set; }

	}
}