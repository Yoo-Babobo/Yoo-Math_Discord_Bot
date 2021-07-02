using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System.Globalization;

namespace yoo_math
{
    public class CommandHandler
    {
        private readonly IConfiguration _config;
        private readonly CommandService _commands;
        private readonly DiscordSocketClient _client;
        private readonly IServiceProvider _services;
        private static readonly string logo = Program.logo;

        public CommandHandler(IServiceProvider services)
        {
            _config = services.GetRequiredService<IConfiguration>();
            _commands = services.GetRequiredService<CommandService>();
            _client = services.GetRequiredService<DiscordSocketClient>();
            _services = services;
            _commands.CommandExecuted += CommandExecutedAsync;
            _client.JoinedGuild += Joined;
            _client.UserJoined += UserJoined;
            _client.UserLeft += UserLeft;
            _client.MessageReceived += MessageReceivedAsync;
        }

        public async Task InitializeAsync() { await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services); }

        public async Task MessageReceivedAsync(SocketMessage rawMessage)
        {
            if (!(rawMessage is SocketUserMessage message)) return;
            if (message.Source != MessageSource.User) return;
            if (message.Channel is IDMChannel) Console.WriteLine("Receaved DM from [" + message.Author.Username + "]");
            var argPos = 0;
            string prefix = _config["Prefix"];
            var context = new SocketCommandContext(_client, message);
            if (new CultureInfo("es-ES", false).CompareInfo.IndexOf(message.Content, "the yule", CompareOptions.IgnoreCase) >= 0) await message.DeleteAsync();
            if (!message.HasStringPrefix(prefix, ref argPos)) return;
            await _commands.ExecuteAsync(context, argPos, _services);
        }

        public async Task UserJoined(SocketGuildUser user)
        {
            var embed = new EmbedBuilder();
            embed.WithTitle(user.Username + " has joined " + user.Guild.Name + "!");
            embed.WithColor(Color.Gold);
            embed.WithThumbnailUrl(user.GetAvatarUrl());
            embed.WithDescription("Welcome to " + user.Guild.Name + "!\nWe hope you enjoy the server!");
            await user.Guild.SystemChannel.SendMessageAsync(null, false, embed.Build());
            return;
        }

        public async Task UserLeft(SocketGuildUser user)
        {
            var embed = new EmbedBuilder();
            embed.WithTitle(user.Username + " has left " + user.Guild.Name + " :(");
            embed.WithColor(Color.Gold);
            embed.WithThumbnailUrl(user.GetAvatarUrl());
            embed.WithDescription("Goodbye " + user.Username + ", we're sad to see you go...");
            await user.Guild.SystemChannel.SendMessageAsync(null, false, embed.Build());
            return;
        }

        public async Task Joined(SocketGuild guild)
        {
            var embed = new EmbedBuilder();
            embed.WithTitle("Yoo-Math");
            embed.WithColor(Color.Gold);
            embed.WithThumbnailUrl(logo);
            embed.WithDescription("The *future* of math.\n\n[Yoo-Math](https://www.math.yoo-babobo.com) was made by [Yoo-Babobo](https://www.yoo-babobo.com).\nYou can download the official app [here](https://www.math.yoo-babobo.com/download).");
            embed.WithImageUrl(logo);
            await guild.SystemChannel.SendMessageAsync(null, false, embed.Build());
            return;
        }

        public async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            var embed = new EmbedBuilder();
            embed.WithColor(Color.Red);
            if (!command.IsSpecified)
            {
                if (result.ErrorReason == "Unknown command.")
                {
                    embed.WithTitle("Unknown Command");
                    embed.WithColor(Color.Red);
                    embed.WithDescription("Hey silly, that command doesn't exist!");
                    await context.Channel.SendMessageAsync(null, false, embed.Build());
                }
                Console.WriteLine($"Command failed to execute for [{context.User.Username}] <-> [{result.ErrorReason}]!");
                return;
            }
            if (result.IsSuccess)
            {
                Console.WriteLine($"Command [{command.Value.Name} => executed for -> [{context.User.Username}]");
                return;
            }
            embed.WithTitle("Error");
            embed.WithColor(Color.Red);
            embed.WithDescription("Something went wrong. Here's the error:\n`" + result + "`");
            await context.Channel.SendMessageAsync(null, false, embed.Build());
        }

        public static int Compute(string s, string t)
        {
            if (string.IsNullOrEmpty(s))
            {
                if (string.IsNullOrEmpty(t)) return 0;
                return t.Length;
            }
            if (string.IsNullOrEmpty(t)) return s.Length;
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];
            for (int i = 0; i <= n; d[i, 0] = i++);
            for (int j = 1; j <= m; d[0, j] = j++);
            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                    int min1 = d[i - 1, j] + 1;
                    int min2 = d[i, j - 1] + 1;
                    int min3 = d[i - 1, j - 1] + cost;
                    d[i, j] = Math.Min(Math.Min(min1, min2), min3);
                }
            }
            return d[n, m];
        }
    }
}