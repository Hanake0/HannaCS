using DSharpPlus.CommandsNext;

using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using System.Threading.Tasks;

namespace Hanna.Commands
{
	[Group("Util")]
	[Description("Comandos úteis")]
	public class Util : BaseCommandModule
	{
		[Command("ping")]
		[Description("p-pong?!?")]
		public async Task Ping(CommandContext ctx)
		{
			await ctx.TriggerTypingAsync().ConfigureAwait(false);
			await ctx.RespondAsync("eita:flushed:... pong?").ConfigureAwait(false);
		}

		[Command("teste")]
		[Description("Comandinho de teste uwu")]
		public async Task Teste(CommandContext ctx)
		{
			await ctx.TriggerTypingAsync().ConfigureAwait(false);
			DiscordEmbedBuilder embed = new DiscordEmbedBuilder()
				.WithAuthor("au au", null, "https://www.hojeemdia.com.br/polopoly_fs/1.570196!/image/image.jpg_gen/derivatives/landscape_653/image.jpg")
				.WithColor(new DiscordColor("#0e820a"))
				.WithDescription("olha! uma descrição!")
				.WithFooter("olha! um texto de footer!")
				.WithTimestamp(System.DateTime.Now)
				.AddField("olha, um nome de campo", "olha um valor");

			await ctx.RespondAsync(embed);
		}

		[Command("avatar"), Aliases("perfil")]
		[Description("Envia a imagem de perfil do usuário")]
		public async Task Avatar(CommandContext ctx)
		{
			await ctx.TriggerTypingAsync().ConfigureAwait(false);
			await ctx.RespondAsync(ctx.User.AvatarUrl);
		}

		[Command("avatar")]
		public async Task Avatar(CommandContext ctx,
			[Description("O dono da imagem")] DiscordUser usuário)
		{
			await ctx.TriggerTypingAsync().ConfigureAwait(false);
			await ctx.RespondAsync(usuário.AvatarUrl);
		}
	}
}
