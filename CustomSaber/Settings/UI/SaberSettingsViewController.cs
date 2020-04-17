using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using CustomSaber.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CustomSaber.Settings.UI
{
    internal class SaberSettingsViewController : BSMLResourceViewController
    {
        public override string ResourceName => "CustomSaber.Settings.UI.Views.saberSettings.bsml";

        [UIValue("trail-type")]
        public string TrailType
        {
            get => Configuration.TrailType.ToString();
            set => Configuration.TrailType = Enum.TryParse(value, out TrailType trailType) ? trailType : Configuration.TrailType;
        }

        [UIValue("trail-type-list")]
        public List<object> trailType = Enum.GetNames(typeof(TrailType)).ToList<object>();

        [UIValue("custom-events-enabled")]
        public bool CustomEventsEnabled
        {
            get => Configuration.CustomEventsEnabled;
            set => Configuration.CustomEventsEnabled = value;
        }

        [UIValue("random-sabers-enabled")]
        public bool RandomSabersEnabled
        {
            get => Configuration.RandomSabersEnabled;
            set => Configuration.RandomSabersEnabled = value;
        }
    }
}
