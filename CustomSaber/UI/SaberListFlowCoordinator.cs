using CustomUI.BeatSaber;
using IPA.Utilities;
using VRUI;

namespace CustomSaber
{
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
                _saberListViewController = BeatSaberUI.CreateViewController<SaberListViewController>();
                _saberListViewController.backButtonPressed += Dismiss;
            }

            if (activationType == FlowCoordinator.ActivationType.AddedToHierarchy)
            {
                ProvideInitialViewControllers(_saberListViewController, null, null);
            }
        }

        void Dismiss() => ReflectionUtil.InvokePrivateMethod((mainFlowCoordinator as FlowCoordinator), "DismissFlowCoordinator", new object[] { this, null, false });

        protected override void DidDeactivate(DeactivationType type)
        {
        }
    }
}
