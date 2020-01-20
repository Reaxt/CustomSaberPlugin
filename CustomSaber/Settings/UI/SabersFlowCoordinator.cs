using BeatSaberMarkupLanguage;
using HMUI;
using IPA.Utilities;
using System;

namespace CustomSaber.Settings.UI
{
    internal class SabersFlowCoordinator : FlowCoordinator
    {
        private SaberListView saberListView;
        private SaberPreviewView saberPreviewView;

        public void Awake()
        {
            if (saberListView == null)
            {
                saberListView = BeatSaberUI.CreateViewController<SaberListView>();
            }

            if (saberPreviewView == null)
            {
                saberPreviewView = BeatSaberUI.CreateViewController<SaberPreviewView>();
            }
        }

        protected override void DidActivate(bool firstActivation, ActivationType activationType)
        {
            try
            {
                if (firstActivation)
                {
                    title = "Custom Sabers";
                    showBackButton = true;
                    ProvideInitialViewControllers(saberListView, null, saberPreviewView);
                }
            }
            catch (Exception ex)
            {
                Logger.log.Error(ex);
            }
        }

        protected override void BackButtonWasPressed(ViewController topViewController)
        {
            // Dismiss ourselves
            MainFlowCoordinator mainFlow = BeatSaberUI.MainFlowCoordinator;
            mainFlow.InvokePrivateMethod("DismissFlowCoordinator", this, null, false);
        }
    }
}
