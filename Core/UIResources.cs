using System.Windows.Media.Imaging;

namespace ClipEditor.Core.Resources
{
    internal static class UIResources
    {
        public const string BitrateImagePath = "..\\Images\\UI_Bitrate.png";
        public const string PlayImagePath = "..\\Images\\UI_Play.png";
        public const string PauseImagePath = "..\\Images\\UI_Pause.png";
        public const string VolumeLowImagePath = "..\\Images\\UI_Volume_Low.png";
        public const string VolumeMidImagePath = "..\\Images\\UI_Volume_Mid.png";
        public const string VolumeHighImagePath = "..\\Images\\UI_Volume_High.png";
        public const string VolumeMuteImagePath = "..\\Images\\UI_Volume_Mute.png";

        public static readonly BitmapImage BitrateImage = new(new Uri(BitrateImagePath, UriKind.Relative));
        public static readonly BitmapImage PlayImage = new(new Uri(PlayImagePath, UriKind.Relative));
        public static readonly BitmapImage PauseImage = new(new Uri(PauseImagePath, UriKind.Relative));
        public static readonly BitmapImage VolumeLowImage = new(new Uri(VolumeLowImagePath, UriKind.Relative));
        public static readonly BitmapImage VolumeMidImage = new(new Uri(VolumeMidImagePath, UriKind.Relative));
        public static readonly BitmapImage VolumeHighImage = new(new Uri(VolumeHighImagePath, UriKind.Relative));
        public static readonly BitmapImage VolumeMuteImage = new(new Uri(VolumeMuteImagePath, UriKind.Relative));

        public static BitmapImage GetImageFromVolume(double volume)
        {
            switch (volume * 100)
            {
                case >= 66: 
                    return VolumeHighImage;
                case >= 33: 
                    return VolumeMidImage;
                case > 0: 
                    return VolumeLowImage;
                default: 
                    return VolumeMuteImage;
            }
        }
    }
}
