using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DotNetEnv;
using ECCUnofficial.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace ECCUnofficial
{
    public class Program
    {
        static void Main(string[] args)
        {
            Env.Load();
            new Program().MainAsync().GetAwaiter().GetResult();
        }

        public async Task MainAsync()
        {
            using (var services = ConfigureServices())
            {
                var client = services.GetRequiredService<DiscordSocketClient>();

                client.Log += LogAsync;
                services.GetRequiredService<CommandService>().Log += LogAsync;

                await client.LoginAsync(TokenType.Bot, Env.GetString("TOKEN"));
                await client.StartAsync();
                await services.GetRequiredService<CommandHandlingService>().InitializeAsync();
                await Task.Delay(-1);
            }
        }

        private ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .AddSingleton<PersistenceService>()
                .BuildServiceProvider();
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }
    }
}
