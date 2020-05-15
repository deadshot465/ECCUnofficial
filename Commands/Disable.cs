using Discord;
using Discord.Commands;
using ECCUnofficial.Interfaces;
using ECCUnofficial.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ECCUnofficial.Commands
{
    public class Disable : ModuleBase<SocketCommandContext>, IErrorHandler
    {
        public PersistenceService PersistenceService { get; set; }

        public enum DisableError
        {
            ArgumentNotSufficient, CategoryNotAvailable
        }

        [Command("disable")]
        [Priority(10)]
        public async Task DisableAsync(string category, IGuildUser user)
        {
            var _category = category.ToUpper().Trim();

            if (_category != "ECC" &&
                _category != "GO2A")
            {
                await HandleErrorAsync(DisableError.CategoryNotAvailable);
                return;
            }

            PersistenceService.DisableId(_category, user.Id);
            await Context.Channel
                .SendMessageAsync($"使用者はこれから{_category}の通知を受けなくなりました。");
        }

        [Command("disable")]
        [Priority(7)]
        public async Task DisableAsync(string category, IGuildChannel channel)
        {
            var _category = category.ToUpper().Trim();

            if (_category != "ECC" &&
                _category != "GO2A")
            {
                await HandleErrorAsync(DisableError.CategoryNotAvailable);
                return;
            }

            PersistenceService.DisableChannel(_category, channel.Id);
            await Context.Channel
                .SendMessageAsync($"チャンネルはこれから{_category}の通知を受けなくなりました。");
        }

        [Command("disable")]
        [Priority(5)]
        public async Task DisableAsync(string category, ulong id)
        {
            var user = Context.Guild.Users.First(usr => usr.Id == id);
            var _category = category.ToUpper().Trim();

            if (_category != "ECC" &&
                _category != "GO2A")
            {
                await HandleErrorAsync(DisableError.CategoryNotAvailable);
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
                    await DisableAsync(category, channel);
            }
            else
                await DisableAsync(category, user);
        }

        [Command("disable")]
        [Priority(3)]
        public async Task DisableAsync([Remainder] string _)
            => await HandleErrorAsync(DisableError.ArgumentNotSufficient);

        public async Task HandleErrorAsync(Enum error)
        {
            var err = (DisableError)error;

            var msg = err switch
            {
                DisableError.ArgumentNotSufficient => "この命令は二つの引数が必要です：ecc!disable <ECCまたはGO2A> <ID、使用者またはチャンネル>。",
                DisableError.CategoryNotAvailable => "この命令はECCもしくはGO2Aしかサポートしておりません。",
                _ => string.Empty
            };

            if (!string.IsNullOrEmpty(msg))
                await Context.Channel.SendMessageAsync(msg);
        }
    }
}
