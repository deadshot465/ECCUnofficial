using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DotNetEnv;
using ECCUnofficial.Structures;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Utf8Json;

namespace ECCUnofficial.Services
{
    public class CommandHandlingService
    {
        private readonly CommandService _commands;
        private readonly DiscordSocketClient _client;
        private readonly PersistenceService _persistence;
        private readonly IServiceProvider _services;
        private readonly HttpClient _http;

        private DateTime _lastFetchedTime = DateTime.Now;

        public CommandHandlingService(IServiceProvider services)
        {
            _commands = services.GetRequiredService<CommandService>();
            _client = services.GetRequiredService<DiscordSocketClient>();
            _persistence = services.GetRequiredService<PersistenceService>();
            _services = services;

            _http = new HttpClient();

            _client.MessageReceived += MessageReceivedAsync;
            _client.LatencyUpdated += LatencyUpdatedAsync;
        }

        public async Task InitializeAsync()
        {
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        private async Task MessageReceivedAsync(SocketMessage message)
        {
            if (!(message is SocketUserMessage userMessage)) return;
            if (userMessage.Author.IsBot) return;

            var argPos = 0;
            if (!userMessage.HasStringPrefix(Env.GetString("PREFIX"), ref argPos)) return;

            var context = new SocketCommandContext(_client, userMessage);
            await _commands.ExecuteAsync(context, argPos, _services);
        }

        private async Task LatencyUpdatedAsync(int oldValue, int newValue)
        {
            if ((DateTime.Now - _lastFetchedTime).TotalMinutes < 5) return;

            _lastFetchedTime = DateTime.Now;

            var request = new HttpRequestMessage
            {
                Headers =
                {
                    { HttpRequestHeader.ContentType.ToString(), "application/json" },
                    { HttpRequestHeader.Accept.ToString(), "application/json" }
                },
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://tetsukizone.com/api/slack/"),
            };

            var response = await _http.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();

                foreach (var userId in _persistence.RegisteredIds)
                {
                    var user = _client.GetUser(userId);
                    _ = user.SendMessageAsync($"Fetch failed: Error - {error}");
                }

                foreach (var channelId in _persistence.RegisteredChannels)
                {
                    var channel = _client.GetChannel(channelId) as SocketTextChannel;
                    _ = channel.SendMessageAsync($"Fetch failed: Error - {error}");
                }

                return;
            }

            using (var content = await response.Content.ReadAsStreamAsync())
            {
                var msgItems = JsonSerializer.Deserialize<List<SlackMessageItem>>(content);

                if (msgItems == null || msgItems.Count <= 0) return;

                foreach (var msg in msgItems
                    .Where(msg => (DateTime.Now - msg.DateTime).TotalMinutes <= 5))
                {
                    foreach (var userId in _persistence.RegisteredIds)
                    {
                        if (_persistence.DisabledChannelAndIds["GO2A"]["Ids"].Contains(userId))
                            continue;

                        var user = _client.GetUser(userId);
                        _ = user.SendMessageAsync($"チャンネル：{msg.Channel} - {msg.UserName}: {msg.Text}\nタイムスタンプ：{msg.Timestamp}");
                    }

                    foreach (var channelId in _persistence.RegisteredChannels)
                    {
                        if (_persistence.DisabledChannelAndIds["GO2A"]["Channels"].Contains(channelId))
                            continue;

                        var channel = _client.GetChannel(channelId) as SocketTextChannel;
                        _ = channel.SendMessageAsync($"チャンネル：{msg.Channel} - {msg.UserName}: {msg.Text}\nタイムスタンプ：{msg.Timestamp}");
                    }
                }
            }

            await _persistence.WriteToStorage();
        }
    }
}
