using ClipEditor.Model;
using System.IO;

namespace ClipEditor.Core
{
    internal static class SettingsManager
    {
        public static OutputSaveFolderType OutputSaveType
        {
            get => (OutputSaveFolderType)Properties.Settings.Default.OutputSaveType;
            set
            {
                Properties.Settings.Default.OutputSaveType = (int)value;
                SaveSettings();
                if (value == OutputSaveFolderType.Custom && CustomOutputPath == string.Empty)
                {
                    CustomOutputPath = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
                }
            }
        }
        public static string CustomOutputPath
        {
            get => Properties.Settings.Default.CustomOutputPath;
            set
            {
                Properties.Settings.Default.CustomOutputPath = value;
                SaveSettings();
            }
        }

        public static bool CheckForUpdates
        {
            get => Properties.Settings.Default.CheckForUpdates;
            set
            {
                Properties.Settings.Default.CheckForUpdates = value;
                SaveSettings();
            }
        }

        private static void SaveSettings() => Properties.Settings.Default.Save();

        public static string GetOutputFolderPath(Video input)
        {
            string outputFileName = $"{input.FileName}_output{input.FileExtension}";
            switch (OutputSaveType)
            {
                default:
                case OutputSaveFolderType.Default:
                    return Path.GetFullPath($"out\\{outputFileName}");

                case OutputSaveFolderType.SameAsInputFile:
                    string dir = Path.GetDirectoryName(input.FilePath) ?? Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
                    return Path.GetFullPath($"{dir}\\{outputFileName}");

                case OutputSaveFolderType.Custom:
                    return Path.GetFullPath($"{CustomOutputPath}\\{outputFileName}");
            }
        }
    }

    public enum OutputSaveFolderType
    {
        Default = 0,
        SameAsInputFile = 1,
        Custom = 2
    }
}
