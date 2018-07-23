using System;
using System.Linq;
using CustomSaber.UI;
using IllusionPlugin;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VRUI;
using Toggle = UnityEngine.UI.Toggle;

namespace CustomSaber
{
    class SaberSelection
    {
        public Toggle Toggle;
        public TextMeshProUGUI Text;
        public string PrettyName;

        public SaberSelection(Toggle toggle, TextMeshProUGUI text, string prettyName)
        {
            Toggle = toggle;
            Text = text;
            PrettyName = prettyName;
        }
    }

    class CustomSaber
    {

        public string Name { get; set; }
        public string Path { get; set; }
        public string Author { get; set; }
        // TODO: prop for Image
    }

    class CustomSaberMasterViewController : VRUINavigationController
    {
        CustomSaberUI _customSaberUI;

        private Button _backButton;

        public CustomSaberListViewController customSaberListViewController;
        public CustomSaberDetailViewController customSaberDetailsViewController;
        public bool ModDetailsPushed = false;

        protected override void DidActivate(bool firstActivation, ActivationType activationType)
        {
            _customSaberUI = CustomSaberUI.Instance;

            _backButton = _customSaberUI.CreateBackButton(rectTransform);
            (_backButton.transform as RectTransform).anchorMin = new Vector2(0, 0);
            (_backButton.transform as RectTransform).anchorMin = new Vector2(0, 0);
            (_backButton.transform as RectTransform).anchoredPosition = new Vector2(0, 0.5f);
            _backButton.onClick.AddListener(delegate ()
            {
                DismissModalViewController(null, false);
            });
            if (customSaberListViewController == null)
            {
                customSaberListViewController = _customSaberUI.CreateViewController<CustomSaberListViewController>();

                customSaberListViewController.rectTransform.anchorMin = new Vector2(0.3f, 0f);
                customSaberListViewController.rectTransform.anchorMax = new Vector2(0.7f, 1f);
            }

            PushViewController(customSaberListViewController, true);

            Resources.FindObjectsOfTypeAll<MainMenuViewController>().First().didFinishEvent += MainMenuNavigated;
        }

        private void MainMenuNavigated(MainMenuViewController sender, MainMenuViewController.MenuButton subMenuType)
        {
            VRUIViewController parent = parentViewController;
            DismissModalViewController(null, true);
            parent.gameObject.SetActive(false);
        }

        protected override void DidDeactivate(DeactivationType type)
        {
            ModDetailsPushed = false;
            if(customSaberListViewController.preview != null)
            {
                customSaberListViewController.preview.Unload(true);
            }

        }




    }
}
