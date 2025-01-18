using System;
using System.Threading;
using Avalonia;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;

namespace AvaloniaApplication1.Views.Templates;

public partial class AnimatedPopup : ContentControl
{
    #region Private members

    private Control mUnderlayControl;
    
    private bool mOpacityCaptured;
    
    private double mOriginalOpacity;

    private bool mFirstAnimation = true;
    
    private Size mDesiredSize;
    
    private TimeSpan mFrameRate = TimeSpan.FromSeconds(1/60.0);
    
    private int mTotalTicks => (int)(_animationTime.TotalSeconds / mFrameRate.TotalSeconds);
    
    private Size mEmptySize = new Size();

    private bool mAnimating;
    
    private DispatcherTimer mAnimationTimer;
    
    private Timer mSizingTimer;

    private int mCurrentAnimationTick;
    
    private bool mSizeFound;

    #endregion
    
    #region Public properties
    
    #region Open


    private bool _open;

    public static readonly DirectProperty<AnimatedPopup, bool> OpenProperty = AvaloniaProperty.RegisterDirect<AnimatedPopup, bool>(
        nameof(Open), o => o.Open, (o, v) => o.Open = v);

    public bool Open
    {
        get => _open;
        set  {
            if (_open == value)
                return;
            if (value)
            {
                //Underlay
                if (Parent is Grid grid)
                {
                    mUnderlayControl.IsVisible = true;
                
                    if (grid.RowDefinitions?.Count > 0)
                        mUnderlayControl.SetValue(Grid.RowSpanProperty, grid.RowDefinitions.Count);
                    if (grid.ColumnDefinitions?.Count > 0)
                        mUnderlayControl.SetValue(Grid.ColumnSpanProperty, grid.ColumnDefinitions.Count);
                    if (!grid.Children.Contains(mUnderlayControl))
                        grid.Children.Insert(0, mUnderlayControl);
                }
            }
            else
            {
                if (IsOpened)
                    UpdateDesiredSize();
            }
            
            UpdateAnimation();
            
            SetAndRaise(OpenProperty, ref _open, value);
        }
    }
    
    #endregion
    
    public bool IsOpened => mCurrentAnimationTick>=mTotalTicks;

    private TimeSpan _animationTime = TimeSpan.FromSeconds(1);

    #region AnimationTime
    public static readonly DirectProperty<AnimatedPopup, TimeSpan> AnimationTimeProperty = AvaloniaProperty.RegisterDirect<AnimatedPopup, TimeSpan>(
        nameof(AnimationTime), o => o.AnimationTime, (o, v) => o.AnimationTime = v);
    
    public TimeSpan AnimationTime
    {
        get => _animationTime;
        set => SetAndRaise(AnimationTimeProperty, ref _animationTime, value);
    }
    #endregion
    
    #region animateOpacity
    private bool _animateOpacity = true;

    public static readonly DirectProperty<AnimatedPopup, bool> AnimateOpacityProperty = AvaloniaProperty.RegisterDirect<AnimatedPopup, bool>(
        nameof(AnimateOpacity), o => o.AnimateOpacity, (o, v) => o.AnimateOpacity = v);

    public bool AnimateOpacity
    {
        get => _animateOpacity;
        set => SetAndRaise(AnimateOpacityProperty, ref _animateOpacity, value);
    }
    
    #endregion
    
    #endregion
    
    #region Public commands
    
    [RelayCommand]
    public void BeginOpen()
    {
        Open = true;
    }

    [RelayCommand]
    public void BeginClose()
    {
        Open = false;
    }
    
    #endregion
    
    public AnimatedPopup()
    {
        mUnderlayControl = new Border()
        {
            IsVisible = false,
            Background = Brushes.Transparent,
            ZIndex = 9,
        };
        mUnderlayControl.PointerPressed += (s,e) => BeginClose();
        mOriginalOpacity = Opacity;

        Opacity = 0;
        
        mAnimationTimer = new DispatcherTimer
        {
            Interval = mFrameRate
        };

        mSizingTimer = new Timer(t =>
        {
            if (mSizeFound)
            {
                return;
            }
            mSizeFound = true;
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                UpdateDesiredSize();
                UpdateAnimation();
            });

        });
        
        mAnimationTimer.Tick += (s, e) => AnimationTick();
        
    }

    #region Private methods
    //updates based on current des size because of expanders
    private void UpdateDesiredSize()
    {
        mDesiredSize = DesiredSize - Margin;
    }
    
    private void UpdateAnimation()
    {
        if (!mSizeFound)
        {
            return;
        }
        mAnimationTimer.Start();
    }

    private void AnimationComplete()
    {
        if (_open)
        {
            Width = double.NaN;
            Height = double.NaN;
            
            Opacity = mOriginalOpacity;
        }
        else
        {
            Width = 0;
            Height = 0;
            
            //Remove Underlay
            if (Parent is Grid grid)
            {
                if (grid.Children.Contains(mUnderlayControl))
                {
                    Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        grid.Children.Remove(mUnderlayControl);
                    });
                }
            }
        }
    }
    private void AnimationTick()
    {
        if (mFirstAnimation)
        {
            mFirstAnimation = false;
            
            mAnimationTimer.Stop();

            AnimationComplete();
            
            
            Opacity = mOriginalOpacity;
            
            return;
        }
        
            
        if ((_open && mCurrentAnimationTick > mTotalTicks) || (!_open && mCurrentAnimationTick == 0))
        {
            mAnimationTimer.Stop();

            AnimationComplete();
            
            mAnimating = false;
                
            return;
        } 
        mAnimating = true;
        
        mCurrentAnimationTick += _open ? 1 : -1;
        
        var percentageAnimated = (float)mCurrentAnimationTick / (float)mTotalTicks;
            
        //easing
        var easing = new QuadraticEaseInOut();
            
        var finalWidth = mDesiredSize.Width * easing.Ease(percentageAnimated);
        var finalHeight = mDesiredSize.Height * easing.Ease(percentageAnimated);
        
        //animate opacity of popup
        if (AnimateOpacity)
            Opacity = mOriginalOpacity * easing.Ease(percentageAnimated);
            
        Width = finalWidth;
        Height = finalHeight;
    }

    #endregion
    
    public override void Render(DrawingContext context)
    {
        if (!mSizeFound) mSizingTimer.Change(100, int.MaxValue );

        base.Render(context);
    }
}