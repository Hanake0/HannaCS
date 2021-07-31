using System;
using DSharpPlus;

using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;

using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

using DSharpPlus.Interactivity.Extensions;

namespace Hanna {
	public class Bot {
		// Client e extensões
		public DiscordClient Client { get; private set; }
		public InteractivityExtension Interactivity { get; private set; }
		public CommandsNextExtension Commands { get; private set; }

		public async Task RunAsync() {

			// Define o User-Agent do WebClient
			Util.WebClient.Client.DefaultRequestHeaders
				.Add("User-Agent", "UnReal - Hanna Bot");

			// Configurações do client
			this.Client = new DiscordClient(new DiscordConfiguration {
				Token = Environment.GetEnvironmentVariable("DISCORD_TOKEN"),
				TokenType = TokenType.Bot,
				AutoReconnect = true,
				MinimumLogLevel = LogLevel.Information,
				LogTimestampFormat = "MMM dd yyyy - hh:mm:ss tt",
			});
			this.Client.Ready += this.OnReady;

			// Configurações do command handler
			this.Commands = this.Client.UseCommandsNext(new CommandsNextConfiguration {
				StringPrefixes = new string[] { Environment.GetEnvironmentVariable("PREFIX") },
				EnableMentionPrefix = true,
				EnableDms = true,
				CaseSensitive = false,
				IgnoreExtraArguments = true,

			});

			this.Interactivity = this.Client.UseInteractivity(new InteractivityConfiguration {
				Timeout = TimeSpan.FromMinutes(1),
			});

			// Registra as classes de comandos
			this.Commands.RegisterCommands<Commands.Util>();
			this.Commands.RegisterCommands<Commands.ShopCommand>();
			this.Commands.RegisterCommands<Commands.Suggestion>();
			this.Commands.RegisterCommands<Commands.APIs>();

			await this.Client.ConnectAsync();
			await Task.Delay(-1);
		}

		private Task OnReady(DiscordClient client, ReadyEventArgs args) {
			DiscordUser user = this.Client.CurrentUser;

			this.Client.Logger
				.LogInformation(LoggerEvents.Startup, 
					$"Logado como {user.Username}#{user.Discriminator}" +
					$"({user.Id}) com sucesso!");

			return Task.CompletedTask;
		}
	}
}
