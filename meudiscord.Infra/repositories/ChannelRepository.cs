using System.Threading.Channels;
using Microsoft.EntityFrameworkCore;

public class ChannelRepository : IChannelRepository
{
    private readonly AppDbContext _appDbContext;

    public ChannelRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<ChannelEntity> CreateChannel(ChannelEntity channel)
    {
        await _appDbContext.channels.AddAsync(channel);
        await _appDbContext.SaveChangesAsync();
        return channel;
    }

    public List<GetAllChannelsLinq> GetAllChannels(int serverId)
    {
        return _appDbContext.channels.Where(c => c.serverId == serverId)
        .Select(c => new GetAllChannelsLinq{
            channelName = c.name,
            channelExternalId = c.externalId,
        }).ToList();
    }

    public ChannelEntity GetChannelByExternalId(Guid externalId)
    {
        return _appDbContext.channels.Where(c => c.externalId == externalId).FirstOrDefault();
    }
}