using System.Collections.Generic;
using System.Threading.Tasks;
using AvaloniaApplication1.DataModels;

namespace AvaloniaApplication1.Services;

public interface IAudioInterfaceService
{
    //fetch the chanel configuration
    Task<List<ChannelConfigurationItem>> GetChannelConfigurationsAsync();
}