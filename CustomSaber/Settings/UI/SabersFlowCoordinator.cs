using BeatSaberMarkupLanguage;
using HMUI;
using System;

namespace CustomSaber.Settings.UI
{
    internal class SabersFlowCoordinator : FlowCoordinator
    {
        private SaberListViewController saberListView;
        private SaberPreviewViewController saberPreviewView;
        private SaberDetailsViewController saberDetailsView;

        public void Awake()
        {
            if (!saberPreviewView)
            {
                saberPreviewView = BeatSaberUI.CreateViewController<SaberPreviewViewController>();
            }

            if (!saberDetailsView)
            {
                saberDetailsView = BeatSaberUI.CreateViewController<SaberDetailsViewController>();
            }

            if (!saberListView)
            {
                saberListView = BeatSaberUI.CreateViewController<SaberListViewController>();
                saberListView.customSaberChanged += saberDetailsView.OnSaberWasChanged;
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
                    ProvideInitialViewControllers(saberListView, saberDetailsView, saberPreviewView);
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
            BeatSaberUI.MainFlowCoordinator.DismissFlowCoordinator(this, null, false);
        }
    }
}
