using System.Collections.Generic;
using System.Threading.Tasks;
using AvaloniaApplication1.DataModels;

namespace AvaloniaApplication1.Services;

public class DummyAudioInterfaceService : IAudioInterfaceService
{
    public Task<List<ChannelConfigurationItem>> GetChannelConfigurationsAsync() => 
    Task.FromResult(new List<ChannelConfigurationItem>(new []
    {
        new ChannelConfigurationItem("Mono Stereo Config", "Mono", "Mono"),
        new ChannelConfigurationItem("Mono Stereo Config", "Stereo", "Stereo"),
        new ChannelConfigurationItem("5.1 Surround", "5.1 DTS - (L, R, Ls, Rs, C, LFE)", "5.1 DTS"),
        new ChannelConfigurationItem("5.1 Surround", "5.1 DTS - (L, R, C, LFE, Ls, Rs)", "5.1 ITU"),
        new ChannelConfigurationItem("5.1 Surround", "5.1 DTS - (L, R, Ls, Rs, C, LFE)", "5.1 FILM"),
    }));
}