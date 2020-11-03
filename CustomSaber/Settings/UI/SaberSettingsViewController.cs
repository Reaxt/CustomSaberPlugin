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

        [UIValue("sabers-in-menu")]
        public bool ShowSabersInSaberMenu
        {
            get => Configuration.ShowSabersInSaberMenu;
            set => Configuration.ShowSabersInSaberMenu = value;
        }

        [UIAction("sabers-in-menu-changed")]
        public void OnSabersInMenuChanged(bool value)
        {
            if (value)
            {
                SaberListViewController.Instance?.GenerateHandheldSaberPreview();
            }
            else
            {
                SaberListViewController.Instance?.ClearHandheldSabers();
                SaberListViewController.Instance?.ShowMenuHandles();
            }
        }

        [UIValue("disable-whitestep")]
        public bool DisableWhitestep
        {
            get => Configuration.DisableWhitestep;
            set => Configuration.DisableWhitestep = value;
        }

        [UIValue("override-trail-length")]
        public bool OverrideTrailLength
        {
            get => Configuration.OverrideTrailLength;
            set => Configuration.OverrideTrailLength = value;
        }

        [UIValue("trail-length")]
        public float TrailLength
        {
            get => Configuration.TrailLength;
            set => Configuration.TrailLength = value;
        }

        [UIValue("saber-width-adjust")]
        public float SaberWidthAdjust
        {
            get => Configuration.SaberWidthAdjust;
            set => Configuration.SaberWidthAdjust = value;
        }

        [UIAction("percent-formatter")]
        public string OnFormatPercent(float obj) => $"{obj * 100}%";

        [UIAction("refreshPreview")]
        public void RefreshPreview()
        {
            StartCoroutine(SaberListViewController.Instance.GenerateSaberPreview(SaberAssetLoader.SelectedSaber));
        }
    }
}
