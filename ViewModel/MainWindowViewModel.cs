using ClipEditor.Core;
using ClipEditor.Core.Dialogs;
using ClipEditor.Core.Tools;
using ClipEditor.Model;
using ClipEditor.View;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Xabe.FFmpeg;

namespace ClipEditor.ViewModel
{
    internal class MainWindowViewModel : ViewModelBase
    {
        public string AppVersion => Tools.GetAppVersion();

        private Video _video = new();
        public Video Video
        {
            get { return _video; }
            set
            {
                _video = value;
                OnPropertyChanged();
            }
        }

        private OutputClip _outputClip = new(string.Empty);
        public OutputClip OutputClip
        {
            get { return _outputClip; }
            set
            {
                _outputClip = value;
                OnPropertyChanged();
            }
        }

        private CancellationTokenSource CancellationTokenSource = new();

        public string ProgressString
        {
            get { return _outputClip.ProgressString; }
            set
            {
                _outputClip.ProgressString = value;
                OnPropertyChanged();
            }
        }

        public int Progress
        {
            get { return _outputClip.Progress; }
            set
            {
                _outputClip.Progress = value;
                OnPropertyChanged();
            }
        }

        public ClipStatus Status
        {
            get { return _outputClip.Status; }
            set
            {
                _outputClip.Status = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand OpenFileCommand => new(execute => OpenFile());
        public RelayCommand DragDropFileCommand => new(execute => DragDropFile((string)execute));
        public RelayCommand SaveClipCommand => new(execute => ClipVideo((ClipData)execute));
        public RelayCommand OpenFolderCommand => new(execute => OpenFolder());
        public RelayCommand CancelTaskCommand => new(execute => CancelTask());

        private async void OpenFile()
        {
            OpenFileDialog opf = new()
            {
                Filter = "Video (*.mp4;*.mkv;*.avi)|*.MP4;*.MKV;*.AVI"
            };
            bool? success = opf.ShowDialog();
            if(success == true)
            {
                try
                {
                    var mediaInfo = await FFmpeg.GetMediaInfo(opf.FileName);
                    Video = new Video(opf.FileName, mediaInfo.Duration);
                }
                catch (Exception ex)
                {
                    DialogManager.ShowErrorDialog(ex.Message);
                }
            }
        }

        private async void DragDropFile(string path)
        {
            if (!path.EndsWithAny(".mp4", ".mkv", ".avi"))
            {
                DialogManager.ShowErrorDialog("Unsupported file format.");
                return;
            }

            if (File.Exists(path))
            {
                try
                {
                    var mediaInfo = await FFmpeg.GetMediaInfo(path);
                    Video = new Video(path, mediaInfo.Duration);
                }
                catch (Exception ex)
                {
                    DialogManager.ShowErrorDialog(ex.Message);
                }
            }
        }

        private async void ClipVideo(ClipData data)
        {
            OutputClip.SetDefaultProperties();
            OnPropertyChanged("OutputClip");

            try
            {
                if (!File.Exists(Video.FilePath))
                {
                    DialogManager.ShowErrorDialog($"Cannot find the input file at \"{Video.FilePath}\"");
                    return;
                }

                Application.Current.MainWindow.IsEnabled = false;
                var progressWindow = new ProgressWindow()
                {
                    DataContext = this
                };
                progressWindow.Owner = Application.Current.MainWindow;
                progressWindow.Closed += (s, a) => Application.Current.MainWindow.IsEnabled = true;
                progressWindow.Show();
                progressWindow.Focus();

                var mediaInfo = await FFmpeg.GetMediaInfo(Video.FilePath);
                string seekValue = data.StartPosition.ToFFmpegRounded();
                string toValue = data.EndPosition.ToFFmpegRounded();
                string timeValue = (data.EndPosition.RoundMilliseconds() - data.StartPosition.RoundMilliseconds()).ToFFmpeg();
                //var clipPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "out", $"{Video.FileName}_output{Video.FileExtension}");
                var clipPath = Path.GetFullPath($"out\\{Video.FileName}_output{Video.FileExtension}");  // replace later when settings for output folders are added
                OutputClip = new(clipPath);

                IConversion conversion = FFmpeg.Conversions.New()
                        .AddStream(mediaInfo.Streams)
                        .AddParameter($"-ss {seekValue}", ParameterPosition.PreInput)
                        .AddParameter($"-to {timeValue}")
                        .SetOutput(OutputClip.FullPath)
                        .SetOverwriteOutput(true);

                if (mediaInfo.AudioStreams.Count() == 2)
                {
                    conversion.AddParameter($"-filter_complex \"[0:a:0][0:a:1]amix=inputs=2[a]\" -map \"[a]\" -map -0:a:0 -map -0:a:1");
                    conversion.AddParameter($"-c:a aac -b:a 160k");
                }
                else
                {
                    conversion.AddParameter($"-c:a copy");
                }

                if (data.Bitrate == 0)
                {
                    conversion.AddParameter($"-c:v copy");
                }
                else
                {
                    conversion.AddParameter($"-b:v {data.Bitrate}M");
                }

                conversion.OnProgress += (sender, args) =>
                {
                    Progress = (int)(Math.Round(args.Duration.TotalSeconds / data.ClipLength.TotalSeconds, 2) * 100);
                    ProgressString = $"[{args.Duration} / {data.ClipLength.ToString(@"hh\:mm\:ss")}]";
                };

                CancellationTokenSource = new();
                await conversion.Start(CancellationTokenSource.Token);

                Progress = 100;
                ProgressString = "Done!";
                Status = ClipStatus.Finished;
            }
            catch (OperationCanceledException)
            {
                Stopwatch sw = Stopwatch.StartNew();

                while (Status == ClipStatus.InCancellation && sw.Elapsed.TotalSeconds < 5)
                {
                    try
                    {
                        File.Delete(OutputClip.FullPath);
                        ProgressString = "Canceled";
                        Status = ClipStatus.Canceled;
                        return;
                    }
                    catch { }

                    await Task.Delay(500);
                }

                if (Status == ClipStatus.InCancellation)
                {
                    DialogManager.ShowErrorDialog("The output file of canceled operation could not be removed. Please check the output folder to remove it manually.");
                    ProgressString = "Canceled";
                    Status = ClipStatus.Canceled;
                }
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorDialog(ex.Message);
            }
        }

        private void CancelTask()
        {
            try
            {
                using (CancellationTokenSource)
                {
                    ProgressString = "Cancelling...";
                    Status = ClipStatus.InCancellation;
                    CancellationTokenSource.Cancel();
                }
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorDialog(ex.Message);
            }
        }

        private void OpenFolder()
        {
            try
            {
                Process.Start("explorer.exe", @$"{OutputClip.Folder}");
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorDialog(ex.Message);
            }
        }
    }
}
