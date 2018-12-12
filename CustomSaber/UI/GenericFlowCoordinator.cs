using System;
using VRUI;
using CustomUI.BeatSaber;

namespace CustomSaber
{
    class GenericFlowCoordinator<TCONT, TRIGHT> : FlowCoordinator where TCONT : VRUIViewController where TRIGHT : VRUIViewController
    {
        private TCONT _contentViewController;
        public TRIGHT _rightViewController;
        public Func<TCONT, string> OnContentCreated;

        CustomSaberUI ui;

        public MainFlowCoordinator mainFlowCoordinator;

        protected override void DidActivate(bool firstActivation, ActivationType activationType)
        {
            if (firstActivation)
            {
                ui = CustomSaberUI._instance;
                _contentViewController = BeatSaberUI.CreateViewController<TCONT>();
                _rightViewController = BeatSaberUI.CreateViewController<TRIGHT>();
                title = OnContentCreated(_contentViewController);
            }
            if (activationType == FlowCoordinator.ActivationType.AddedToHierarchy)
            {
                ProvideInitialViewControllers(_contentViewController, null, _rightViewController);
            }
        }


        /*
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
        }*/

        void Dismiss()
        {
            ReflectionUtil.InvokePrivateMethod((mainFlowCoordinator as FlowCoordinator), "DismissFlowCoordinator", new object[] { this, null, false });
        }

        protected override void DidDeactivate(DeactivationType type)
        {
        }
    }
}