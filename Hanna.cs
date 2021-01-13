using System;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

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
            DiscordConfiguration clientConfigs = new DiscordConfiguration
            {
                Token = Environment.GetEnvironmentVariable("AUTH_TOKEN"),
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                MinimumLogLevel = LogLevel.Debug,
            };
            Client = new DiscordClient(clientConfigs);
            Client.Ready += OnReady;

            // Configurações do command handler
            CommandsNextConfiguration commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = new string[] { Environment.GetEnvironmentVariable("PREFIX") },
                EnableMentionPrefix = true,
                EnableDms = true,
            };
            Commands = Client.UseCommandsNext(commandsConfig);

            await Client.ConnectAsync();

            await Task.Delay(-1);
        }

        private async Task OnReady(DiscordClient client, ReadyEventArgs args)
        {
            DiscordChannel channel = await client.GetChannelAsync(796029220307337266);

            await channel.SendMessageAsync("on the line :sunglasses:");
        }

    }
}
