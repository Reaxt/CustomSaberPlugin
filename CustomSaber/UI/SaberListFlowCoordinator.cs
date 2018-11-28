using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRUI;
using CustomUI.BeatSaber;
using UnityEngine;

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

        void Dismiss()
        {
            ReflectionUtil.InvokePrivateMethod((mainFlowCoordinator as FlowCoordinator), "DismissFlowCoordinator", new object[] { this, null, false });
        }

        protected override void DidDeactivate(DeactivationType type)
        {
        }
    }
}
