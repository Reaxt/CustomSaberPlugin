using System;
using IllusionPlugin;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VRUI;
using Toggle = UnityEngine.UI.Toggle;

namespace CustomSaber
{
    /**
     * ModUI Framework implementation ideas:
     *
     * - Mod references BSModUI.dll
     * - Inherits ModGui baseclass
     * - Calls Setup UI base function with name and ver
     * - Toggle is added by default with override for custom page
     * - USING BSMODUI FOR THIS SORRY FRIENDS
     */

    class ModSelection
    {
        public Toggle Toggle;
        public TextMeshProUGUI Text;
        public string PrettyName;

        public ModSelection(Toggle toggle, TextMeshProUGUI text, string prettyName)
        {
            Toggle = toggle;
            Text = text;
            PrettyName = prettyName;
        }
    }

    class CustSaber
    {

        public string Name { get; set; }
        public string Path { get; set; }
        public string Author { get; set; }
        // TODO: prop for Image
    }

    class ModMenuMasterViewController : VRUINavigationController
    {
        private ModMenuUi _modMenuUi = FindObjectOfType<ModMenuUi>();


        private Button _backButton;

        public ModsListViewController ModsListViewController;
        public StandardLevelDetailViewController ModDetailsViewController;
        public bool ModDetailsPushed = false;

        private ModMenuMasterViewController _modList;

        protected override void DidActivate(bool firstActivation, ActivationType activationType)
        {
            // Find ModMenuUi object and create back button on screen

            _modMenuUi = FindObjectOfType<ModMenuUi>();
            try
            {

                _backButton = _modMenuUi.CreateBackButton(rectTransform);
                (_backButton.transform as RectTransform).anchorMin = new Vector2(0, 0);
                (_backButton.transform as RectTransform).anchorMin = new Vector2(0, 0);
                (_backButton.transform as RectTransform).anchoredPosition = new Vector2(0, 0.5f);
                _backButton.onClick.AddListener(delegate ()
                {
                    DismissModalViewController(null, false);
                });
                if (ModsListViewController == null)
                {
                    ModsListViewController = _modMenuUi.CreateViewController<ModsListViewController>();
                }


                ModsListViewController.rectTransform.anchorMin = new Vector2(0.3f, 0f);
                ModsListViewController.rectTransform.anchorMax = new Vector2(0.7f, 1f);

                PushViewController(ModsListViewController, true);

                Console.WriteLine("View Controller activated");



                //base.DidActivate();
            }
            catch (Exception ex)
            {
            }
        }


        protected override void DidDeactivate(DeactivationType type)
        {
            ModDetailsPushed = false;
            if(ModsListViewController.preview != null)
            {
                ModsListViewController.preview.Unload(true);
            }
            Console.WriteLine("deactivated");
        }




    }
}
