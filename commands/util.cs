using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hanna
{
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
        public async Task Teste(CommandContext ctx, int número, string outroNúmero, DiscordUser usuário)
        {
            await ctx.TriggerTypingAsync().ConfigureAwait(false);
            await ctx.RespondAsync($"parece que você escreveu {número} e {outroNúmero}, além de mencionar {usuário.Mention}");
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
