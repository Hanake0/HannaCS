using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Hanna.Cosmos.Entitys {
	public class Wallet {

        public ulong WalletId { get; init; }

		public WCUser User { get; init; }

        [Required]
        public long Coins { get; set; }

        [Required]
        public long Gems { get; set; }

	}
}