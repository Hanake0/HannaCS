using DSharpPlus.Entities;

namespace Hanna.Configuration {
	public static class Embeds {
		public static class Colors {
			public static readonly DiscordColor DefaultGreen = new("#91FF84"); // Verde clarinho
			public static readonly DiscordColor DefaultOrange = new("#FFFD84"); // Laranja clarinho
			public static readonly DiscordColor DefaultRed = new("#FF8484"); // Vermelho clarinho
		}

		public static class Images {
			public static readonly string GreenCheck = "https://garticbot.gg/images/icons/hit.png";
			public static readonly string OrangeExclamation = "https://garticbot.gg/images/icons/alert.png";
			public static readonly string RedCross = "https://garticbot.gg/images/icons/error.png";
		}
	}
}
