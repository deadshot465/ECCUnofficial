using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Utf8Json;

namespace ECCUnofficial.Services
{
    public class PersistenceService
    {
        public HashSet<ulong> RegisteredIds { get; private set; } = new HashSet<ulong>();
        public HashSet<ulong> RegisteredChannels { get; private set; } = new HashSet<ulong>();

        private const string _basePath = "./persistence";
        private const string _idPath = "./persistence/ids.json";
        private const string _channelPath = "./persistence/channels.json";

        public PersistenceService()
        {
            if (!Directory.Exists(_basePath))
                Directory.CreateDirectory(_basePath);

            if (File.Exists(_idPath))
            {
                var rawString = File.ReadAllText(_idPath);
                RegisteredIds = JsonSerializer.Deserialize<HashSet<ulong>>(rawString);
            }

            if (File.Exists(_channelPath))
            {
                var rawString = File.ReadAllText(_channelPath);
                RegisteredChannels = JsonSerializer.Deserialize<HashSet<ulong>>(rawString);
            }
        }

        public async Task WriteToStorage()
        {
            var serializedIds = JsonSerializer.ToJsonString(RegisteredIds);
            var serializedChannels = JsonSerializer.ToJsonString(RegisteredChannels);

            await File.WriteAllTextAsync(_idPath, serializedIds);
            await File.WriteAllTextAsync(_channelPath, serializedChannels);
        }

        public bool AddId(ulong userId)
            => RegisteredIds.Add(userId);

        public bool RemoveId(ulong userId)
            => RegisteredIds.Remove(userId);

        public bool AddChannel(ulong channelId)
            => RegisteredChannels.Add(channelId);

        public bool RemoveChannel(ulong channelId)
            => RegisteredChannels.Remove(channelId);
    }
}
