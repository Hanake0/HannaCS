using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using Hanna.Configuration;
using Hanna.Util;
using Hanna.Util.JsonModels;

namespace Hanna.Commands {
	class APIs : BaseCommandModule {
	
		[Command("httpcats"), Aliases("http")]
		public async Task HttpCats(CommandContext ctx, int código = 0) {
			if (código == 0)
				código = HttpCatsAPI.ResponseCodes[
					new Random().Next(0, HttpCatsAPI.ResponseCodes.Length)
				];

			else if (!HttpCatsAPI.ResponseCodes.Contains(código))
				código = 404;

			DiscordEmbedBuilder builder = new DiscordEmbedBuilder()
				.WithAuthor("HTTP Cats", HttpCatsAPI.Link)
				.WithImageUrl(HttpCatsAPI.Link + código);

			await ctx.TriggerTypingAsync();
			await ctx.RespondAsync(builder);
		}

		[Command("randomfox"), Aliases("fox")]
		public async Task RandomFox(CommandContext ctx) {
			RandomFoxAPIResponse randomFox = await WebClient.GetRandomFoxImage();

			DiscordEmbedBuilder builder = new DiscordEmbedBuilder()
				.WithAuthor("Random Fox", randomFox.link, RandomFoxAPIConfig.Logo)
				.WithImageUrl(randomFox.image);

			await ctx.TriggerTypingAsync();
			await ctx.RespondAsync(builder);
		}
	}
}
