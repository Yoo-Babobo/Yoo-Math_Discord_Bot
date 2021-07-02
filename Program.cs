using System;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace yoo_math_bot
{
    public class Program
    {
        private readonly IConfiguration _config;
        public static DiscordSocketClient _client;

        public static readonly ulong owner_id = 646461212267249674;
        public static readonly string logo = "https://www.media.yoo-babobo.com/images/logos/Yoo-Math.png";

        public static void Main(string[] args)
        {
            Console.Title = "The Yoo-Math Bot";
            new Program().MainAsync().GetAwaiter().GetResult();
        }

        public Program()
        {
            var _builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile(path: "config.yoo");
            _config = _builder.Build();
        }

        public async Task MainAsync()
        {
            using var services = ConfigureServices();
            var client = services.GetRequiredService<DiscordSocketClient>();
            _client = client;
            client.Log += LogAsync;
            client.Ready += ReadyAsync;
            services.GetRequiredService<CommandService>().Log += LogAsync;
            await client.LoginAsync(TokenType.Bot, _config["Token"]);
            await client.StartAsync();
            await client.SetStatusAsync(UserStatus.Online);
            await client.SetGameAsync("^help | Math is EVERYTHING...");
            await services.GetRequiredService<CommandHandler>().InitializeAsync();
            await Task.Delay(-1);
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }

        private Task ReadyAsync() { return Task.CompletedTask; }

        private ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton(_config)
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandler>()
                .BuildServiceProvider();
        }

        public static bool Is_owner(ulong id) { if (id == owner_id) return true; else return false; }
    }
}