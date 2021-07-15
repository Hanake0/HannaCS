using System;
using DSharpPlus;

using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;

using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

using Hanna.Commands;
using DSharpPlus.Interactivity.Extensions;

namespace Hanna
{
	public class Bot
	{
		// Client e extensões
		public DiscordClient Client { get; private set; }
		public InteractivityExtension Interactivity { get; private set; }
		public CommandsNextExtension Commands { get; private set; }

		public async Task RunAsync()
		{
			// Configurações do client
			this.Client = new DiscordClient(new DiscordConfiguration
			{
				Token = Environment.GetEnvironmentVariable("AUTH_TOKEN"),
				TokenType = TokenType.Bot,
				AutoReconnect = true,
				MinimumLogLevel = LogLevel.Information,

			});
			this.Client.Ready += this.OnReady;

			// Configurações do command handler
			this.Commands = this.Client.UseCommandsNext(new CommandsNextConfiguration
			{
				StringPrefixes = new string[] { Environment.GetEnvironmentVariable("PREFIX") },
				EnableMentionPrefix = true,
				EnableDms = true,
				CaseSensitive = false,
				IgnoreExtraArguments = true,

			});

			this.Interactivity = this.Client.UseInteractivity(new InteractivityConfiguration
			{
				Timeout = TimeSpan.FromMinutes(1),
			});

			// Registra as classes de comandos
			this.Commands.RegisterCommands<ShopCommand>();
			this.Commands.RegisterCommands<Hanna.Commands.Util>();

			await this.Client.ConnectAsync();
			await Task.Delay(-1);
		}

		private Task OnReady(DiscordClient client, ReadyEventArgs args)
		{
			return Task.Run(async () =>
			{
				DiscordChannel channel = await client.GetChannelAsync(828494435469623356);

				await channel.SendMessageAsync("on the line :sunglasses:");
			});
		}
	}
}
