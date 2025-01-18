using System;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using AvaloniaApplication1.Services;
using AvaloniaApplication1.ViewModels;

namespace AvaloniaApplication1.Views;

public partial class MainWindow : Window
{
    #region Private Members

    private Control mChanelConfigPopup;
    private Control mChanelConfigBtn;
    private Control mMainGrid;

    private Timer mSizingTimer;
    
    private Control mVolumeContainer;

    #endregion
    public MainWindow()
    {
        InitializeComponent();

        mSizingTimer = new Timer((t) =>
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                UpdateSizes();
            });
        });
        
        mChanelConfigPopup = this.FindControl<Control>("ChanelConfigPopup")?? throw new Exception("Cannot find ChanelConfigPopup by name");
        mChanelConfigBtn = this.FindControl<Control>("ChannelConfigBtn")?? throw new Exception("Cannot find ChannelConfigBtn by name");
        mMainGrid = this.FindControl<Control>("MainGrid")?? throw new Exception("Cannot find MainGrid by name");
        
        mVolumeContainer = this.FindControl<Control>("VolumeContainer")?? throw new Exception("Cannot find VolumeContainer by name");
        
        SizeChanged += OnWindowSizeChanged;
    }

    private void UpdateSizes()
    {
        ((MainWindowViewModel)DataContext).VolumeContainerSize = mVolumeContainer.Bounds.Height;
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        ((MainWindowViewModel)DataContext).LoadSettingsCommand.Execute(null);

        //outpot all devices then select

        foreach (RecordingDevice device in RecordingDevice.Enumerate())
        {
            Console.WriteLine($"{device?.Index}: {device?.Name}");
        }
        
        base.OnLoaded(e);
    }

    private void OnWindowSizeChanged(object? sender, EventArgs e)
    {
        mSizingTimer.Change(100, int.MaxValue);
        
        var position = mChanelConfigBtn.TranslatePoint(new Point(), mMainGrid);
        if (position != null)
        {
            mChanelConfigPopup.Margin = new Thickness(position.Value.X, 0, 0, mMainGrid.Bounds.Height - position.Value.Y);
        }
    }

    private void InputElement_OnPointerPressed(object? sender, PointerPressedEventArgs e) =>
        ((MainWindowViewModel)DataContext).ChannelConfigBtnPressedCommand.Execute(null);
}