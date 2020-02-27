using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using CustomSaber.Data;
using CustomSaber.Utilities;
using HMUI;

namespace CustomSaber.Settings.UI
{
    internal class SaberDetailsViewController : BSMLResourceViewController
    {
        public override string ResourceName => "CustomSaber.Settings.UI.Views.saberDetails.bsml";

        [UIComponent("saber-description")]
        public TextPageScrollView materialDescription;

        public void OnSaberWasChanged(CustomSaberData customSaber)
        {
            materialDescription.SetText($"{customSaber.Descriptor.SaberName}:\n\n{Utils.SafeUnescape(customSaber.Descriptor.Description)}");
        }
    }
}
