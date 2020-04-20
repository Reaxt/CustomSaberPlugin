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

        internal static void Load()
        {
            if (!IsLoaded)
            {
                Directory.CreateDirectory(Plugin.PluginAssetPath);

                IEnumerable<string> saberFilter = new List<string> { "*.saber" };
                CustomSaberFiles = Utils.GetFileNames(Plugin.PluginAssetPath, saberFilter, SearchOption.AllDirectories, true);
                Logger.log.Debug($"{CustomSaberFiles.Count()} external saber(s) found.");

                CustomSabers = LoadCustomSabers(CustomSaberFiles);
                Logger.log.Debug($"{CustomSabers.Count} total saber(s) loaded.");

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

        /// <summary>
        /// Reload all CustomMaterials
        /// </summary>
        internal static void Reload()
        {
            Logger.log.Debug("Reloading the SaberAssetLoader");
            Clear();
            Load();
        }

        /// <summary>
        /// Clear all loaded CustomMaterials
        /// </summary>
        internal static void Clear()
        {
            int numberOfObjects = CustomSabers.Count;
            for (int i = 0; i < numberOfObjects; i++)
            {
                CustomSabers[i].Destroy();
                CustomSabers[i] = null;
            }

            IsLoaded = false;
            SelectedSaber = 0;
            CustomSabers = new List<CustomSaberData>();
            CustomSaberFiles = Enumerable.Empty<string>();
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
                    if (newSaber != null)
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

        public static CustomSaberData GetRandomSaber()
        {
            UnityEngine.Random.InitState(System.DateTime.Now.Millisecond);
            return CustomSabers[UnityEngine.Random.Range(0, CustomSabers.Count)];
        }

        public static int DeleteCurrentSaber()
        {
            if (SelectedSaber == 0) return 0;

            var filePath = Path.Combine(Plugin.PluginAssetPath, CustomSabers[SelectedSaber].FileName);
            var saberToDelete = SelectedSaber;

            try
            {
                File.Delete(filePath);
            }
            catch (Exception e)
            {
                Logger.log.Error($"Failed to delete Saber: {CustomSabers[SelectedSaber].Descriptor.SaberName} - {filePath}");
                Logger.log.Error($"{e.Message} - {e.StackTrace}");
            }

            CustomSabers[SelectedSaber].Destroy();
            CustomSabers.RemoveAt(SelectedSaber);

            SelectedSaber = (SelectedSaber < CustomSabers.Count - 1) ? SelectedSaber : SelectedSaber - 1;
            Configuration.CurrentlySelectedSaber = CustomSabers[SelectedSaber].FileName;
            return saberToDelete;
        }
    }
}
