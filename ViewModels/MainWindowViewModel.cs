using System;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using AvaloniaApplication1.DataModels;
using AvaloniaApplication1.Services;
using CommunityToolkit.Mvvm.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AvaloniaApplication1.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    #region Private Properties

    private IAudioInterfaceService _audioInterfaceService;

    #endregion
    
    #region Public Properties
    
    [ObservableProperty]
    private string _boldTitle = "AVALONIA";
    
    [ObservableProperty]
    private string _regularTitle = "LOUDNESS METER";
    
    [ObservableProperty]
    private bool _chanelConfigListIsOpen = false;
    
    [ObservableProperty]
    private ObservableGroupedCollection<string, ChannelConfigurationItem> _channelConfigurations = default!;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ChannelConfigBtnText))]
    private ChannelConfigurationItem? _selectedChannelConfigurationItem;

    [ObservableProperty] private double _volumePercentPosition;
    
    [ObservableProperty] private double _volumeContainerSize;
    
    public string ChannelConfigBtnText => SelectedChannelConfigurationItem?.ShortText ?? "Select a channel";
    
    #endregion
    
    #region Public Commands
    [RelayCommand]
    private void ChannelConfigBtnPressed() => ChanelConfigListIsOpen = !ChanelConfigListIsOpen;

    [RelayCommand]
    private void ChanelConfigItemPressed(ChannelConfigurationItem channelConfigurationItem)
    {
        //Update selected item
        SelectedChannelConfigurationItem = channelConfigurationItem;
        
        //close menu
        ChanelConfigListIsOpen = false;
    }

    [RelayCommand]
    private async Task LoadSettingsAsync()
    {
        var channnelConfigurations  = await _audioInterfaceService.GetChannelConfigurationsAsync();

        ChannelConfigurations =
            new ObservableGroupedCollection<string, ChannelConfigurationItem>(
                channnelConfigurations.GroupBy(item => item.Group));
    }
    
    #endregion

    #region Constructor
    public MainWindowViewModel(IAudioInterfaceService audioInterfaceService)
    {
        _audioInterfaceService = audioInterfaceService;
        
        Initialize();
    }
    
    //constructor for designer
    public MainWindowViewModel()
    {
        _audioInterfaceService = new DummyAudioInterfaceService();

        
    }

    private void Initialize()
    {
        var tick = 0;
        var input = 0.0;

        var tempTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1/60.0)
        };

        tempTimer.Tick += (s, e) =>
        {
            tick++;

            input = tick / 20f;

            var scale = _volumeContainerSize / 2f;

            VolumePercentPosition = (Math.Sin(input)+1) * scale;
        };
        
        tempTimer.Start();
    }
    #endregion
    
}