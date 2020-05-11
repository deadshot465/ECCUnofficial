using Discord;
using Discord.Commands;
using ECCUnofficial.Services;
using System.Linq;
using System.Threading.Tasks;

namespace ECCUnofficial.Commands
{
    public class Add : ModuleBase<SocketCommandContext>
    {
        public PersistenceService PersistenceService { get; set; }

        [Command("add")]
        [Priority(10)]
        public async Task AddAsync(IGuildUser user)
        {
            PersistenceService.AddId(user.Id);
            await Context.Channel.SendMessageAsync("使用者は追加されました。");
        }

        [Command("add")]
        [Priority(7)]
        public async Task AddAsync(IGuildChannel channel)
        {
            PersistenceService.AddChannel(channel.Id);
            await Context.Channel.SendMessageAsync("チャンネルは追加されました。");
        }

        [Command("add")]
        [Priority(5)]
        public async Task AddAsync(ulong id)
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
                    await AddAsync(channel);
            }
            else
                await AddAsync(user);
        }
    }
}
