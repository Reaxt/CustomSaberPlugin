using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using CustomSaber.Data;
using CustomSaber.Utilities;
using HMUI;

namespace CustomSaber.Settings.UI
{
    internal class SaberPreviewViewController : BSMLResourceViewController
    {
        public override string ResourceName => "CustomSaber.Settings.UI.Views.saberPreview.bsml";

        [UIComponent("error-description")]
        public TextPageScrollView errorDescription;

        public void OnSaberWasChanged(CustomSaberData customSaber)
        {
            if (!string.IsNullOrWhiteSpace(customSaber?.ErrorMessage))
            {
                errorDescription.gameObject.SetActive(true);
                errorDescription.SetText($"{customSaber.Descriptor?.SaberName}:\n\n{Utils.SafeUnescape(customSaber.ErrorMessage)}");
            }
            else
            {
                errorDescription.gameObject.SetActive(false);
            }
        }
    }
}
