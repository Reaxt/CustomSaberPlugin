using CustomSaber.Data;
using CustomSaber.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CustomSaber.Utilities
{
    public class SaberAssetLoader
    {
        public static bool IsLoaded { get; private set; }
        public static int SelectedSaber { get; internal set; } = 0;
        public static IList<CustomSaberData> CustomSabers { get; private set; } = new List<CustomSaberData>();
        public static IEnumerable<string> CustomSaberFiles { get; private set; } = Enumerable.Empty<string>();

        internal static void LoadCustomSabers()
        {
            if (!IsLoaded)
            {
                Directory.CreateDirectory(Plugin.PluginAssetPath);

                IEnumerable<string> saberFilter = new List<string> { "*.saber" };
                CustomSaberFiles = Utils.GetFileNames(Plugin.PluginAssetPath, saberFilter, SearchOption.AllDirectories, true);
                Logger.log.Debug($"{CustomSaberFiles.Count()} saber(s) found.");

                CustomSabers = LoadCustomSabers(CustomSaberFiles);
                Logger.log.Debug($"{CustomSabers.Count} saber(s) loaded.");

                if (Configuration.CurrentlySelectedSaber != null)
                {
                    for (int i = 0; i < CustomSabers.Count; i++)
                    {
                        if (CustomSabers[i].FileName == Configuration.CurrentlySelectedSaber)
                        {
                            SelectedSaber = i;
                            break;
                        }
                    }
                }

                IsLoaded = true;
            }
        }

        private static IList<CustomSaberData> LoadCustomSabers(IEnumerable<string> customSaberFiles)
        {
            IList<CustomSaberData> customSabers = new List<CustomSaberData>
            {
                new CustomSaberData("DefaultSabers"),
            };

            foreach (string customSaberFile in customSaberFiles)
            {
                try
                {
                    CustomSaberData newSaber = new CustomSaberData(customSaberFile);
                    if (newSaber.AssetBundle != null)
                    {
                        customSabers.Add(newSaber);
                    }
                }
                catch (Exception ex)
                {
                    Logger.log.Warn($"Failed to Load Custom Saber with name '{customSaberFile}'.");
                    Logger.log.Warn(ex);
                }
            }

            return customSabers;
        }
    }
}
