using CustomSaber.Settings.Utilities;
using IPA.Config;
using IPA.Config.Stores;

namespace CustomSaber.Settings
{
    public class Configuration
    {
        public static string CurrentlySelectedSaber { get; internal set; }

        internal static void Init(Config config)
        {
            PluginConfig.Instance = config.Generated<PluginConfig>();
        }

        internal static void Load()
        {
            CurrentlySelectedSaber = PluginConfig.Instance.lastSaber;
        }

        internal static void Save()
        {
            PluginConfig.Instance.lastSaber = CurrentlySelectedSaber;
        }
    }
}
