using Discord;
using Discord.Commands;
using ECCUnofficial.Services;
using System.Linq;
using System.Threading.Tasks;

namespace ECCUnofficial.Commands
{
    public class Remove : ModuleBase<SocketCommandContext>
    {
        public PersistenceService PersistenceService { get; set; }

        [Command("remove")]
        [Priority(10)]
        public async Task RemoveAsync(IGuildUser user)
        {
            PersistenceService.RemoveId(user.Id);
            await Context.Channel.SendMessageAsync("使用者の登録は削除されました。");
        }

        [Command("remove")]
        [Priority(7)]
        public async Task RemoveAsync(IGuildChannel channel)
        {
            PersistenceService.RemoveChannel(channel.Id);
            await Context.Channel.SendMessageAsync("チャンネルの登録は削除されました。");
        }

        [Command("remove")]
        [Priority(5)]
        public async Task RemoveAsync(ulong id)
        {
            var user = Context.Guild.Users.First(usr => usr.Id == id);

            if (user == null)
            {
                var channel = Context.Guild.Channels.First(chn => chn.Id == id);
                if (channel == null)
                {
                    await Context.Channel.SendMessageAsync("この使用者・チャンネルは存在していません。");
                    return;
                }
                else
                    await RemoveAsync(channel);
            }
            else
                await RemoveAsync(user);
        }
    }
}
