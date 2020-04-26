namespace CustomSaber.Settings.Utilities
{
    public class PluginConfig
    {
        public static PluginConfig Instance;

        public string lastSaber;
        public string trailType;
        public bool customEventsEnabled = true;
        public bool randomSabersEnabled = false;
        public bool showSabersInSaberMenu = false;
        public bool overrideCustomTrailLength = false;
        public float trailLength = 1f;
        public float saberWidthAdjust = 1f;
    }
}
