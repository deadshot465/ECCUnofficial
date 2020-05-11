using Discord.Commands;
using ECCUnofficial.Interfaces;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ECCUnofficial.Commands
{
    public class React : ModuleBase<SocketCommandContext>, IErrorHandler
    {
        private const string _reactionUrl = "https://tetsukizone.com/api/slack/react";
        private readonly HttpClient _http;

        public enum ReactionError
        {
            LengthTooShort, LengthTooLong
        }

        public React() : base()
        {
            _http = new HttpClient();
        }

        [Command("react")]
        [Alias("reaction")]
        public async Task ReactAsync()
            => await HandleErrorAsync(ReactionError.LengthTooShort);

        [Command("react")]
        [Alias("reaction")]
        [Priority(5)]
        public async Task ReactAsync(string channelId, string timestamp)
        {
            var response = await _http
                .PostAsync(_reactionUrl + $"/{channelId}/{timestamp}", null);

            if (!response.IsSuccessStatusCode)
            {
                var errorMsg = await response.Content.ReadAsStringAsync();
                Console.Error.WriteLine($"Reaction failed.\nError Message: {errorMsg}\nStatus Code: {response.StatusCode}");
                return;
            }
        }

        [Command("react")]
        [Alias("reaction")]
        [Priority(3)]
        public async Task ReactAsync([Remainder] string _)
            => await HandleErrorAsync(ReactionError.LengthTooLong);

        public async Task HandleErrorAsync(Enum error)
        {
            var msg = error switch
            {
                ReactionError.LengthTooShort => "チャンネルIDとメッセージIDが必要です。",
                ReactionError.LengthTooLong => "チャンネルIDとメッセージIDだけを入力ください。",
                _ => string.Empty
            };

            if (!string.IsNullOrEmpty(msg))
                await Context.Channel.SendMessageAsync(msg);
        }
    }
}
