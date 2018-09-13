#if __ANDROID__ || __IOS__

using System;
using System.Timers;
using Uno.Extensions;
using Uno.Logging;
using Uno.UI.Converters;
using Windows.Media.Playback;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;

namespace Windows.UI.Xaml.Controls
{
	[TemplatePart(Name = "RootGrid", Type = typeof(Grid))]
	[TemplatePart(Name = "PlayPauseButtonName", Type = typeof(Button))]
	[TemplatePart(Name = "PlayPauseButtonOnLeftName", Type = typeof(Button))]
	[TemplatePart(Name = "VolumeMuteButtonName", Type = typeof(Button))]
	[TemplatePart(Name = "AudioMuteButtonName", Type = typeof(Button))]
	[TemplatePart(Name = "VolumeSliderName", Type = typeof(Slider))]
	[TemplatePart(Name = "FullWindowButtonName", Type = typeof(Button))]
	[TemplatePart(Name = "CastButtonName", Type = typeof(Button))]
	[TemplatePart(Name = "ZoomButtonName", Type = typeof(Button))]
	[TemplatePart(Name = "PlaybackRateButtonName", Type = typeof(Button))]
	[TemplatePart(Name = "PlaybackRateButtonName", Type = typeof(Button))]
	[TemplatePart(Name = "SkipForwardButtonName", Type = typeof(Button))]
	[TemplatePart(Name = "NextTrackButtonName", Type = typeof(Button))]
	[TemplatePart(Name = "FastForwardButtonName", Type = typeof(Button))]
	[TemplatePart(Name = "RewindButtonName", Type = typeof(Button))]
	[TemplatePart(Name = "PreviousTrackButtonName", Type = typeof(Button))]
	[TemplatePart(Name = "SkipBackwardButtonName", Type = typeof(Button))]
	[TemplatePart(Name = "StopButtonName", Type = typeof(Button))]
	[TemplatePart(Name = "AudioTracksSelectionButtonName", Type = typeof(Button))]
	[TemplatePart(Name = "CCSelectionButtonName", Type = typeof(Button))]
	[TemplatePart(Name = "TimeElapsedElement", Type = typeof(TextBlock))]
	[TemplatePart(Name = "TimeRemainingElement", Type = typeof(TextBlock))]
	[TemplatePart(Name = "ProgressSlider", Type = typeof(Slider))]
	[TemplatePart(Name = "BufferingProgressBar", Type = typeof(ProgressBar))]
	public partial class MediaTransportControls : Control
	{
		private const string RootGridName = "RootGrid";
		private const string PlayPauseButtonName = "PlayPauseButton";
		private const string PlayPauseButtonOnLeftName = "PlayPauseButtonOnLeft";
		private const string VolumeMuteButtonName = "VolumeMuteButton";
		private const string AudioMuteButtonName = "AudioMuteButton";
		private const string VolumeSliderName = "VolumeSlider";
		private const string FullWindowButtonName = "FullWindowButton";
		private const string CastButtonName = "CastButton";
		private const string ZoomButtonName = "ZoomButton";
		private const string PlaybackRateButtonName = "PlaybackRateButton";
		private const string SkipForwardButtonName = "SkipForwardButton";
		private const string NextTrackButtonName = "NextTrackButton";
		private const string FastForwardButtonName = "FastForwardButton";
		private const string RewindButtonName = "RewindButton";
		private const string PreviousTrackButtonName = "PreviousTrackButton";
		private const string SkipBackwardButtonName = "SkipBackwardButton";
		private const string StopButtonName = "StopButton";
		private const string AudioTracksSelectionButtonName = "AudioTracksSelectionButton";
		private const string CCSelectionButtonName = "CCSelectionButton";
		private const string TimeElapsedElementName = "TimeElapsedElement";
		private const string TimeRemainingElementName = "TimeRemainingElement";
		private const string ProgressSliderName = "ProgressSlider";
		private const string BufferingProgressBarName = "BufferingProgressBar";

		private Grid _rootGrid;
		private Button _playPauseButton;
		private Button _playPauseButtonOnLeft;
		private Button _volumeMuteButton;
		private Button _audioMuteButton;
		private Slider _volumeSlider;
		private Button _fullWindowButton;
		private Button _castButton;
		private Button _zoomButton;
		private Button _playbackRateButton;
		private Button _skipForwardButton;
		private Button _nextTrackButton;
		private Button _fastForwardButton;
		private Button _rewindButton;
		private Button _previousTrackButton;
		private Button _skipBackwardButton;
		private Button _stopButton;
		private Button _audioTracksSelectionButton;
		private Button _ccSelectionButton;
		private TextBlock _timeElapsedElement;
		private TextBlock _timeRemainingElement;
		private Slider _progressSlider;
		private ProgressBar _bufferingProgressBar;

		private Timer _controlsVisibilityTimer;

		public MediaTransportControls() : base()
		{
			_controlsVisibilityTimer = new Timer()
			{
				AutoReset = false,
				Interval = 3000
			};

			_controlsVisibilityTimer.Elapsed += ControlsVisibilityTimerElapsed;
		}

		private void ControlsVisibilityTimerElapsed(object sender, ElapsedEventArgs args)
		{
			if (ShowAndHideAutomatically)
			{
				Hide();
			}

			_controlsVisibilityTimer.Stop();
		}

		private void ResetControlsVisibilityTimer()
		{
			if (ShowAndHideAutomatically)
			{
				_controlsVisibilityTimer.Stop();
				_controlsVisibilityTimer.Start();
			}
		}

		private void CancelControlsVisibilityTimer()
		{
			Show();
			_controlsVisibilityTimer.Stop();
		}

		protected override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			var trueToVisible = new FromNullableBoolToVisibilityConverter()
			{
				VisibilityIfTrue = VisibilityIfTrue.Visible
			};

			_playPauseButton = this.GetTemplateChild(PlayPauseButtonName) as Button;

			_playPauseButtonOnLeft = this.GetTemplateChild(PlayPauseButtonOnLeftName) as Button;

			_volumeMuteButton = this.GetTemplateChild(VolumeMuteButtonName) as Button;
			_volumeMuteButton?.SetBinding(Button.VisibilityProperty, new Binding { Path = "IsVolumeButtonVisible", Source = this, Mode = BindingMode.OneWay, FallbackValue = Visibility.Collapsed, Converter = trueToVisible });
			_volumeMuteButton?.SetBinding(Button.IsEnabledProperty, new Binding { Path = "IsVolumeEnabled", Source = this, Mode = BindingMode.OneWay, FallbackValue = true });

			_audioMuteButton = this.GetTemplateChild(AudioMuteButtonName) as Button;

			_volumeSlider = this.GetTemplateChild(VolumeSliderName) as Slider;

			_fullWindowButton = this.GetTemplateChild(FullWindowButtonName) as Button;
			_fullWindowButton?.SetBinding(Button.VisibilityProperty, new Binding { Path = "IsFullWindowButtonVisible", Source = this, Mode = BindingMode.OneWay, FallbackValue = Visibility.Collapsed, Converter = trueToVisible });
			_fullWindowButton?.SetBinding(Button.IsEnabledProperty, new Binding { Path = "IsFullWindowEnabled", Source = this, Mode = BindingMode.OneWay, FallbackValue = true });

			_castButton = this.GetTemplateChild(CastButtonName) as Button;

			_zoomButton = this.GetTemplateChild(ZoomButtonName) as Button;
			_zoomButton?.SetBinding(Button.VisibilityProperty, new Binding { Path = "IsZoomButtonVisible", Source = this, Mode = BindingMode.OneWay, FallbackValue = Visibility.Collapsed, Converter = trueToVisible });
			_zoomButton?.SetBinding(Button.IsEnabledProperty, new Binding { Path = "IsZoomEnabled", Source = this, Mode = BindingMode.OneWay, FallbackValue = true });

			_playbackRateButton = this.GetTemplateChild(PlaybackRateButtonName) as Button;
			_playbackRateButton?.SetBinding(Button.VisibilityProperty, new Binding { Path = "IsPlaybackRateButtonVisible", Source = this, Mode = BindingMode.OneWay, FallbackValue = Visibility.Collapsed, Converter = trueToVisible });
			_playbackRateButton?.SetBinding(Button.IsEnabledProperty, new Binding { Path = "IsPlaybackRateEnabled", Source = this, Mode = BindingMode.OneWay, FallbackValue = true });

			_skipForwardButton = this.GetTemplateChild(SkipForwardButtonName) as Button;
			_skipForwardButton?.SetBinding(Button.VisibilityProperty, new Binding { Path = "IsSkipForwardButtonVisible", Source = this, Mode = BindingMode.OneWay, FallbackValue = Visibility.Collapsed, Converter = trueToVisible });
			_skipForwardButton?.SetBinding(Button.IsEnabledProperty, new Binding { Path = "IsSkipForwardEnabled", Source = this, Mode = BindingMode.OneWay, FallbackValue = true });

			_nextTrackButton = this.GetTemplateChild(NextTrackButtonName) as Button;
			_nextTrackButton?.SetBinding(Button.VisibilityProperty, new Binding { Path = "IsNextTrackButtonVisible", Source = this, Mode = BindingMode.OneWay, FallbackValue = Visibility.Collapsed, Converter = trueToVisible });
			
			_fastForwardButton = this.GetTemplateChild(FastForwardButtonName) as Button;
			_fastForwardButton?.SetBinding(Button.VisibilityProperty, new Binding { Path = "IsFastForwardButtonVisible", Source = this, Mode = BindingMode.OneWay, FallbackValue = Visibility.Collapsed, Converter = trueToVisible });
			_fastForwardButton?.SetBinding(Button.IsEnabledProperty, new Binding { Path = "IsFastForwardEnabled", Source = this, Mode = BindingMode.OneWay, FallbackValue = true });

			_rewindButton = this.GetTemplateChild(RewindButtonName) as Button;
			_rewindButton?.SetBinding(Button.VisibilityProperty, new Binding { Path = "IsFastRewindButtonVisible", Source = this, Mode = BindingMode.OneWay, FallbackValue = Visibility.Collapsed, Converter = trueToVisible });
			_rewindButton?.SetBinding(Button.IsEnabledProperty, new Binding { Path = "IsFastRewindEnabled", Source = this, Mode = BindingMode.OneWay, FallbackValue = true });

			_previousTrackButton = this.GetTemplateChild(PreviousTrackButtonName) as Button;
			_previousTrackButton?.SetBinding(Button.VisibilityProperty, new Binding { Path = "IsPreviousTrackButtonVisible", Source = this, Mode = BindingMode.OneWay, FallbackValue = Visibility.Collapsed, Converter = trueToVisible });
			
			_skipBackwardButton = this.GetTemplateChild(SkipBackwardButtonName) as Button;
			_skipBackwardButton?.SetBinding(Button.VisibilityProperty, new Binding { Path = "IsSkipBackwardButtonVisible", Source = this, Mode = BindingMode.OneWay, FallbackValue = Visibility.Collapsed, Converter = trueToVisible });
			_skipBackwardButton?.SetBinding(Button.IsEnabledProperty, new Binding { Path = "IsSkipBackwardEnabled", Source = this, Mode = BindingMode.OneWay, FallbackValue = true });

			_stopButton = this.GetTemplateChild(StopButtonName) as Button;
			_stopButton?.SetBinding(Button.VisibilityProperty, new Binding { Path = "IsStopButtonVisible", Source = this, Mode = BindingMode.OneWay, FallbackValue = Visibility.Collapsed, Converter = trueToVisible });
			_stopButton?.SetBinding(Button.IsEnabledProperty, new Binding { Path = "IsStopEnabled", Source = this, Mode = BindingMode.OneWay, FallbackValue = true });

			_audioTracksSelectionButton = this.GetTemplateChild(AudioTracksSelectionButtonName) as Button;

			_ccSelectionButton = this.GetTemplateChild(CCSelectionButtonName) as Button;

			_timeElapsedElement = this.GetTemplateChild(TimeElapsedElementName) as TextBlock;

			_timeRemainingElement = this.GetTemplateChild(TimeRemainingElementName) as TextBlock;

			_progressSlider = this.GetTemplateChild(ProgressSliderName) as Slider;

			_bufferingProgressBar = this.GetTemplateChild(BufferingProgressBarName) as ProgressBar;

			_rootGrid = this.GetTemplateChild(RootGridName) as Grid;
			_rootGrid.Tapped -= OnRootGridTapped;
			_rootGrid.Tapped += OnRootGridTapped;

			if (_mediaPlayer != null)
			{
				BindMediaPlayer();
			}
		}

		public void Show()
		{
			Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
			{
				VisualStateManager.GoToState(this, "ControlPanelFadeIn", false);
			});
		}
		
		public void Hide()
		{
			Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
			{
				VisualStateManager.GoToState(this, "ControlPanelFadeOut", false);
			});
		}

		private void OnRootGridTapped(object sender, TappedRoutedEventArgs e)
		{
			Show();

			if (ShowAndHideAutomatically)
			{
				ResetControlsVisibilityTimer();
			}
		}

		private static void OnIsCompactChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
		{
			VisualStateManager.GoToState((MediaTransportControls)dependencyObject, (bool)args.NewValue ? "CompactMode" : "NormalMode", false);
		}

		private static void OnShowAndHideAutomaticallyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
		{
			if ((bool)args.NewValue)
			{
				((MediaTransportControls)dependencyObject).ResetControlsVisibilityTimer();
			}
			else
			{
				((MediaTransportControls)dependencyObject).CancelControlsVisibilityTimer();
			}
		}
	}
}
#endif
