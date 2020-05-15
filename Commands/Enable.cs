using Discord;
using Discord.Commands;
using ECCUnofficial.Interfaces;
using ECCUnofficial.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ECCUnofficial.Commands
{
    public class Enable : ModuleBase<SocketCommandContext>, IErrorHandler
    {
        public PersistenceService PersistenceService { get; set; }

        public enum EnableError
        {
            ArgumentNotSufficient, CategoryNotAvailable
        }

        [Command("enable")]
        [Priority(10)]
        public async Task EnableAsync(string category, IGuildUser user)
        {
            var _category = category.ToUpper().Trim();

            if (_category != "ECC" &&
                _category != "GO2A")
            {
                await HandleErrorAsync(EnableError.CategoryNotAvailable);
                return;
            }

            PersistenceService.EnableId(_category, user.Id);
            await Context.Channel
                .SendMessageAsync($"使用者はこれから{_category}の通知を受けられるようになりました。");
        }

        [Command("enable")]
        [Priority(7)]
        public async Task EnableAsync(string category, IGuildChannel channel)
        {
            var _category = category.ToUpper().Trim();

            if (_category != "ECC" &&
                _category != "GO2A")
            {
                await HandleErrorAsync(EnableError.CategoryNotAvailable);
                return;
            }

            PersistenceService.EnableChannel(_category, channel.Id);
            await Context.Channel
                .SendMessageAsync($"チャンネルはこれから{_category}の通知を受けられるようになりました。");
        }

        [Command("enable")]
        [Priority(5)]
        public async Task EnableAsync(string category, ulong id)
        {
            var user = Context.Guild.Users.First(usr => usr.Id == id);
            var _category = category.ToUpper().Trim();

            if (_category != "ECC" &&
                _category != "GO2A")
            {
                await HandleErrorAsync(EnableError.CategoryNotAvailable);
                return;
            }

            if (user == null)
            {
                var channel = Context.Guild.Channels.First(chn => chn.Id == id);
                if (channel == null)
                {
                    await Context.Channel.SendMessageAsync("この使用者・チャンネルは存在していません。");
                    return;
                }
                else
                    await EnableAsync(category, channel);
            }
            else
                await EnableAsync(category, user);
        }

        [Command("enable")]
        [Priority(3)]
        public async Task EnableAsync([Remainder] string _)
            => await HandleErrorAsync(EnableError.ArgumentNotSufficient);

        public async Task HandleErrorAsync(Enum error)
        {
            var err = (EnableError)error;

            var msg = err switch
            {
                EnableError.ArgumentNotSufficient => "この命令は二つの引数が必要です：ecc!disable <ECCまたはGO2A> <ID、使用者またはチャンネル>。",
                EnableError.CategoryNotAvailable => "この命令はECCもしくはGO2Aしかサポートしておりません。",
                _ => string.Empty
            };

            if (!string.IsNullOrEmpty(msg))
                await Context.Channel.SendMessageAsync(msg);
        }
    }
}
