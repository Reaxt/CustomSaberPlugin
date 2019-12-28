using CustomSaber.Settings;
using CustomSaber.Settings.UI;
using CustomSaber.Utilities;
using IPA;
using IPA.Config;
using IPA.Loader;
using IPA.Utilities;
using System.IO;
using UnityEngine.SceneManagement;
using IPALogger = IPA.Logging.Logger;

namespace CustomSaber
{
    public class Plugin : IBeatSaberPlugin
    {
        public static string PluginName => "Custom Sabers";
        public static SemVer.Version PluginVersion { get; private set; } = new SemVer.Version("0.0.0"); // Default.
        public static string PluginAssetPath => Path.Combine(BeatSaber.InstallPath, "CustomSabers");

        public void Init(IPALogger logger, [Config.Prefer("json")] IConfigProvider cfgProvider, PluginLoader.PluginMetadata metadata)
        {
            Logger.log = logger;
            Configuration.Init(cfgProvider);

            if (metadata != null)
            {
                PluginVersion = metadata.Version;
            }
        }

        public void OnApplicationStart() => Load();
        public void OnApplicationQuit() => Unload();

        public void OnActiveSceneChanged(Scene prevScene, Scene nextScene)
        {
            if (nextScene.name == "GameCore")
            {
                SaberScript.Load();
            }
        }

        public void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode) { }
        public void OnSceneUnloaded(Scene scene) { }
        public void OnUpdate() { }
        public void OnFixedUpdate() { }

        private void Load()
        {
            Configuration.Load();
            SaberAssetLoader.LoadCustomSabers();
            SettingsUI.CreateMenu();
            Logger.log.Info($"{PluginName} v.{PluginVersion} has started.");
        }

        private void Unload()
        {
            Configuration.Save();
        }
    }
}
