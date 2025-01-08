using ClipEditor.Core.Dialogs;
using ClipEditor.Core.Resources;
using ClipEditor.Core.Tools;
using ClipEditor.Model;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using Xceed.Wpf.Toolkit;

namespace ClipEditor.View
{
    /// <summary>
    /// Interaction logic for VideoView.xaml
    /// </summary>
    public partial class VideoView : UserControl
    {
        private readonly DispatcherTimer _positionTimer = new();

        private bool _isPlaying = true;
        private bool _wasPlayingBeforeDrag;

        public ICommand SaveClipCommand
        {
            get { return (ICommand)GetValue(SaveClipCommandProperty); }
            set { SetValue(SaveClipCommandProperty, value); }
        }
        public static readonly DependencyProperty SaveClipCommandProperty =
            DependencyProperty.Register("SaveClipCommand", typeof(ICommand), typeof(VideoView), new PropertyMetadata(null));

        public ICommand DragDropFileCommand
        {
            get { return (ICommand)GetValue(DragDropFileCommandProperty); }
            set { SetValue(DragDropFileCommandProperty, value); }
        }
        public static readonly DependencyProperty DragDropFileCommandProperty =
            DependencyProperty.Register("DragDropFileCommand", typeof(ICommand), typeof(VideoView), new PropertyMetadata(null));


        public VideoView()
        {
            _positionTimer.Interval = TimeSpan.FromMilliseconds(10);
            _positionTimer.Tick += PositionTimer_Tick;
            InitializeComponent();
        }

        private void PlayVideo()
        {
            try
            {
                if (timelineSlider.Value >= clipRangeSlider.HigherValue)
                {
                    timelineSlider.Value = clipRangeSlider.LowerValue;
                    SetVideoPositionToSliderValue();
                }

                _positionTimer?.Start();
                mainVideo.Play();
                mainVideo.IsMuted = false;
                _isPlaying = true;
                playButtonImage.Source = UIResources.PauseImage;
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorDialog(ex.Message);
            }
        }

        private void PauseVideo()
        {
            try
            {
                _positionTimer?.Stop();
                mainVideo.Pause();
                mainVideo.IsMuted = true;
                _isPlaying = false;
                playButtonImage.Source = UIResources.PlayImage;
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorDialog(ex.Message);
            }
        }

        private void SetVideoPositionToSliderValue()
        {
            TimeSpan ts = new(0, 0, 0, 0, (int)timelineSlider.Value);
            mainVideo.Position = ts;
            vidCurrent.Text = Tools.GetTimeStringFromTimeSpan(ts);
        }

        private void VideoView_Loaded(object sender, RoutedEventArgs e)
        {
            timelineSlider.AddHandler(PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler((sender, e) =>
            {
                _wasPlayingBeforeDrag = _isPlaying;
                PauseVideo();
                SetVideoPositionToSliderValue();

                Track? track = timelineSlider.Template.FindName("PART_Track", timelineSlider) as Track;
                if (!timelineSlider.IsMoveToPointEnabled || track == null || track.Thumb == null || track.Thumb.IsMouseOver)
                    return;

                track.Thumb.UpdateLayout();
                track.Thumb.RaiseEvent(new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, MouseButton.Left)
                {
                    RoutedEvent = MouseLeftButtonDownEvent,
                    Source = track.Thumb
                });
            }), true);

            Slider? lowerSlider = clipRangeSlider.Template.FindName("PART_LowerSlider", clipRangeSlider) as Slider;
            Slider? higherSlider = clipRangeSlider.Template.FindName("PART_HigherSlider", clipRangeSlider) as Slider;
            if (lowerSlider == null || higherSlider == null)
                return;

            Track? lowerTrack = lowerSlider.Template.FindName("PART_Track", lowerSlider) as Track;
            Track? higherTrack = higherSlider.Template.FindName("PART_Track", higherSlider) as Track;
            if (lowerTrack == null || lowerTrack.Thumb == null || higherTrack == null || higherTrack.Thumb == null)
                return;

            var leftButtonDownEventHandler = new MouseButtonEventHandler((sender, e) =>
            {
                _wasPlayingBeforeDrag = _isPlaying;
                PauseVideo();
            });

            var leftButtonUpEventHandler = new MouseButtonEventHandler((sender, e) => 
            {
                if (_wasPlayingBeforeDrag)
                    PlayVideo();
            });

            lowerTrack.Thumb.AddHandler(PreviewMouseLeftButtonDownEvent, leftButtonDownEventHandler, true);
            lowerTrack.Thumb.AddHandler(PreviewMouseLeftButtonUpEvent, leftButtonUpEventHandler, true);
            higherTrack.Thumb.AddHandler(PreviewMouseLeftButtonDownEvent, leftButtonDownEventHandler, true);
            higherTrack.Thumb.AddHandler(PreviewMouseLeftButtonUpEvent, leftButtonUpEventHandler, true);
        }

        private void MainVideo_MediaOpened(object sender, RoutedEventArgs e)
        {
            clipRangeSlider.LowerValue = 0;
            clipRangeSlider.HigherValue = clipRangeSlider.Maximum;

            mainVideo.Volume = 0.5;
            volumeSlider.Value = 50;
            volumeButtonImage.Source = UIResources.GetImageFromVolume(mainVideo.Volume);
            innerVolumeButtonImage.Source = UIResources.GetImageFromVolume(mainVideo.Volume);

            bitrateSlider.Value = 0;

            timelineSlider.IsEnabled = true;
            clipRangeSlider.IsEnabled = true;
            saveClipButton.IsEnabled = true;
            playButton.IsEnabled = true;
            noVidText.Visibility = Visibility.Collapsed;

            mainVideo.LoadedBehavior = MediaState.Manual;
            PlayVideo();
        }

        private void MainVideo_MediaEnded(object sender, RoutedEventArgs e)
        {
            mainVideo.Position = new(0, 0, 0, 0, (int)clipRangeSlider.LowerValue);
            timelineSlider.Value = 0;
            //_positionTimer?.Stop();
            mainVideo.Play();
        }

        private void PositionTimer_Tick(object sender, EventArgs e)
        {
            vidCurrent.Text = Tools.GetTimeStringFromTimeSpan(mainVideo.Position);
            timelineSlider.Value = mainVideo.Position.TotalMilliseconds;

            if (timelineSlider.Value >= clipRangeSlider.HigherValue)
            {
                PauseVideo();
            }
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isPlaying)
                PauseVideo();
            else
                PlayVideo();
        }

        private void TimelineSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (timelineSlider.Value > clipRangeSlider.HigherValue)
            {
                e.Handled = true;
                timelineSlider.Value = clipRangeSlider.HigherValue;
            }

            if (timelineSlider.Value < clipRangeSlider.LowerValue)
            {
                e.Handled = true;
                timelineSlider.Value = clipRangeSlider.LowerValue;
            }

            if (!_isPlaying)
            {
                SetVideoPositionToSliderValue();
            }
        }

        private void TimelineSlider_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_wasPlayingBeforeDrag)
                PlayVideo();
        }

        private void ClipRangeSlider_LowerValueChanged(object sender, RoutedEventArgs e)
        {
            timelineSlider.Value = clipRangeSlider.LowerValue;
        }

        private void ClipRangeSlider_HigherValueChanged(object sender, RoutedEventArgs e)
        {
            timelineSlider.Value = clipRangeSlider.HigherValue;
        }

        private void SaveClipButton_Click(object sender, RoutedEventArgs e)
        {
            TimeSpan startPos = new(0, 0, 0, 0, (int)clipRangeSlider.LowerValue);
            TimeSpan endPos = new(0, 0, 0, 0, (int)clipRangeSlider.HigherValue);
            ClipData clipData = new(startPos, endPos, (int)bitrateSlider.Value);

            SaveClipCommand?.Execute(clipData);
        }

        private void UserControl_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                DragDropFileCommand?.Execute(files[0]);
            }
        }

        private void ClipRangeSlider_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is RangeSlider rangeSlider)
            {
                var clickedElement = e.OriginalSource as FrameworkElement;
                if (clickedElement == null)
                    return;

                if (Tools.GetParentByName(clickedElement, "PART_LowerSlider") != null)
                {
                    PauseVideo();
                    timelineSlider.Value = rangeSlider.LowerValue;
                    return;
                }

                if (Tools.GetParentByName(clickedElement, "PART_HigherSlider") != null)
                {
                    PauseVideo();
                    timelineSlider.Value = rangeSlider.HigherValue;
                    return;
                }
            }
        }

        private void VolumeButton_Click(object sender, RoutedEventArgs e)
        {
            volumePopup.IsOpen = !volumePopup.IsOpen;
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            volumeText.Text = ((int)volumeSlider.Value).ToString();
            mainVideo.Volume = volumeSlider.Value / 100f;

            volumeButtonImage.Source = UIResources.GetImageFromVolume(mainVideo.Volume);
            innerVolumeButtonImage.Source = UIResources.GetImageFromVolume(mainVideo.Volume);
        }

        private void InnerVolumeButton_Click(object sender, RoutedEventArgs e)
        {
            if (mainVideo.Volume > 0)
            {
                mainVideo.Volume = 0;
                volumeButtonImage.Source = UIResources.VolumeMuteImage;
                innerVolumeButtonImage.Source = UIResources.VolumeMuteImage;
            }
            else
            {
                mainVideo.Volume = volumeSlider.Value / 100f;
                volumeButtonImage.Source = UIResources.GetImageFromVolume(mainVideo.Volume);
                innerVolumeButtonImage.Source = UIResources.GetImageFromVolume(mainVideo.Volume);
            }
        }

        private void BitrateButton_Click(object sender, RoutedEventArgs e)
        {
            bitratePopup.IsOpen = !bitratePopup.IsOpen;
        }

        private void BitrateSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (bitrateSlider.Value == 0)
            {
                bitrateText.Text = "Def.";
            }
            else
            {
                bitrateText.Text = $"{bitrateSlider.Value}M";
            }
        }
    }
}
