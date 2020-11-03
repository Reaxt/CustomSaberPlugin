using CustomSaber.Settings;
using CustomSaber.Settings.UI;
using CustomSaber.Utilities;
using IPA;
using IPA.Config;
using IPA.Loader;
using IPA.Utilities;
using System.IO;
using IPALogger = IPA.Logging.Logger;

namespace CustomSaber
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        public static string PluginName => "Custom Sabers";
        public static SemVer.Version PluginVersion { get; private set; } = new SemVer.Version("0.0.0"); // Default.
        public static string PluginAssetPath => Path.Combine(UnityGame.InstallPath, "CustomSabers");

        [Init]
        public void Init(IPALogger logger, Config config, PluginMetadata metadata)
        {
            Logger.log = logger;
            Configuration.Init(config);

            if (metadata != null)
            {
                PluginVersion = metadata.Version;
            }
        }

        [OnStart]
        public void OnApplicationStart() => Load();
        [OnExit]
        public void OnApplicationQuit() => Unload();

        private void OnGameSceneLoaded()
        {
            SaberScript.Load();
        }

        private void Load()
        {
            Configuration.Load();
            SaberAssetLoader.Load();
            SettingsUI.CreateMenu();
            AddEvents();
            Logger.log.Info($"{PluginName} v.{PluginVersion} has started.");
        }

        private void Unload()
        {
            Configuration.Save();
            SaberAssetLoader.Clear();
            RemoveEvents();
        }

        private void AddEvents()
        {
            RemoveEvents();
            BS_Utils.Utilities.BSEvents.gameSceneLoaded += OnGameSceneLoaded;
        }

        private void RemoveEvents()
        {
            BS_Utils.Utilities.BSEvents.gameSceneLoaded -= OnGameSceneLoaded;
        }
    }
}
