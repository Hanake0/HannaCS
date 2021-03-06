﻿using DSharpPlus.CommandsNext;

using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using System.IO;
using System.Net;
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
			for(; ; )
			{
				WebRequest request = WebRequest.Create("https://musentm.vercel.app/api/test");
				
				HttpWebResponse response = (HttpWebResponse)request.GetResponse();
				request.Credentials = CredentialCache.DefaultCredentials;


				string responseString;
				using (Stream dataStream = response.GetResponseStream())
				{
					StreamReader reader = new StreamReader(dataStream);
					responseString = reader.ReadToEnd();
				}

				await ctx.RespondAsync($"StatusCode: {response.StatusCode}: `{response.StatusDescription}`\n" +
					$"Tipo de resposta: {response.ContentType}\n" +
					$"Tamanho da resposta: {response.ContentLength}\n" +
					$"Resposta: {responseString}\n");
				response.Close();

				await Task.Delay(System.TimeSpan.FromSeconds(10));
			}
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
