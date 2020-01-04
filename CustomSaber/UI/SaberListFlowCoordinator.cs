using IPA.Utilities;
using HMUI;
using System;
using BeatSaberMarkupLanguage;
namespace CustomSaber
{
    class SaberUIFlowCoordinator : FlowCoordinator
    {
        private SaberListView _saberListView;
        private SaberPreviewView _saberPreviewView;
        public void Awake()
        {
            if(_saberListView == null && _saberPreviewView == null)
            {
                _saberListView = BeatSaberUI.CreateViewController<SaberListView>();
                _saberPreviewView = BeatSaberUI.CreateViewController<SaberPreviewView>();
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
                    ProvideInitialViewControllers(_saberListView, null, _saberPreviewView);
                }
                if (activationType == ActivationType.AddedToHierarchy)
                {
                    
                }

            }
            catch (Exception ex)
            {
                Logger.logger.Error(ex);
            }
        }

        protected override void BackButtonWasPressed(ViewController topViewController)
        {
            // dismiss ourselves
            var mainFlow = CustomSaberUI._instance.MainFlowCoordinator;
            mainFlow.InvokePrivateMethod("DismissFlowCoordinator", this, null, false);
        }
    }
    /*
    class SaberListFlowCoordinator : FlowCoordinator
    {
        CustomSaberUI ui;

        public SaberListViewController _saberListViewController;
        public MainFlowCoordinator mainFlowCoordinator;

        protected override void DidActivate(bool firstActivation, ActivationType activationType)
        {
            if (firstActivation)
            {
                title = "Saber Select";

                ui = CustomSaberUI._instance;
         //       _saberListViewController = BeatSaberUI.CreateViewController<SaberListViewController>();
         //       _saberListViewController.backButtonPressed += Dismiss;
            }

            if (activationType == FlowCoordinator.ActivationType.AddedToHierarchy)
            {
         //       ProvideInitialViewControllers(_saberListViewController, null, null);
            }
        }

        void Dismiss() => ReflectionUtil.InvokePrivateMethod((mainFlowCoordinator as FlowCoordinator), "DismissFlowCoordinator", new object[] { this, null, false });

        protected override void DidDeactivate(DeactivationType type)
        {
        }
    }
    */
}
