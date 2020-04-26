using CustomSaber.Settings.Utilities;
using CustomSaber.Utilities;
using IPA.Config;
using IPA.Config.Stores;
using System;

namespace CustomSaber.Settings
{
    public class Configuration
    {
        public static string CurrentlySelectedSaber { get; internal set; }
        public static TrailType TrailType { get; internal set; }
        public static bool CustomEventsEnabled { get; internal set; }
        public static bool RandomSabersEnabled { get; internal set; }
        public static bool ShowSabersInSaberMenu { get; internal set; }
        public static bool OverrideTrailLength { get; internal set; }
        public static float TrailLength { get; internal set; }

        internal static void Init(Config config)
        {
            PluginConfig.Instance = config.Generated<PluginConfig>();
        }

        internal static void Load()
        {
            CurrentlySelectedSaber = PluginConfig.Instance.lastSaber;
            TrailType = Enum.TryParse(PluginConfig.Instance.trailType, out TrailType trailType) ? trailType : TrailType.Custom;
            CustomEventsEnabled = PluginConfig.Instance.customEventsEnabled;
            RandomSabersEnabled = PluginConfig.Instance.randomSabersEnabled;
            ShowSabersInSaberMenu = PluginConfig.Instance.showSabersInSaberMenu;
            OverrideTrailLength = PluginConfig.Instance.overrideCustomTrailLength;
            TrailLength = PluginConfig.Instance.trailLength;
        }

        internal static void Save()
        {
            PluginConfig.Instance.lastSaber = CurrentlySelectedSaber;
            PluginConfig.Instance.trailType = TrailType.ToString();
            PluginConfig.Instance.customEventsEnabled = CustomEventsEnabled;
            PluginConfig.Instance.randomSabersEnabled = RandomSabersEnabled;
            PluginConfig.Instance.showSabersInSaberMenu = ShowSabersInSaberMenu;
            PluginConfig.Instance.overrideCustomTrailLength = OverrideTrailLength;
            PluginConfig.Instance.trailLength = TrailLength;
        }
    }
}
