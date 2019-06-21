using System;
using System.Linq;
using CustomUI.MenuButton;
using TMPro;
using LogLevel = IPA.Logging.Logger.Level;
using UnityEngine;
using UnityEngine.UI;

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
        public AssetBundle AssetBundle { get; set; }
        public GameObject GameObject { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string Author { get; set; }
        // TODO: prop for Image
    }

    class CustomSaberUI : MonoBehaviour
    {
        //private RectTransform _mainMenuRectTransform;
        //private MainMenuViewController _mainMenuViewController;
        private MainFlowCoordinator _mainFlowCoordinator;
        public SaberListFlowCoordinator _saberListFlowCoordinator;
        public static CustomSaberUI _instance;

        public class SaberListFlowCoordinator : GenericFlowCoordinator<SaberListViewController, SaberPreviewController> { };

        internal static void OnLoad()
        {
            if (_instance != null)
            {
                return;
            }

            new GameObject("CustomSaberUI").AddComponent<CustomSaberUI>();
        }

        private void Awake()
        {
            _instance = this;
            try
            {
                //_buttonInstance = Resources.FindObjectsOfTypeAll<Button>().First(x => (x.name == "QuitButton"));
                //_backButtonInstance = Resources.FindObjectsOfTypeAll<Button>().First(x => (x.name == "BackArrowButton"));
                //_mainMenuViewController = Resources.FindObjectsOfTypeAll<MainMenuViewController>().First();
                _mainFlowCoordinator = Resources.FindObjectsOfTypeAll<MainFlowCoordinator>().First();
                //_mainMenuRectTransform = _buttonInstance.transform.parent as RectTransform;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }

            CreateCustomSaberButton();
        }

        private void CreateCustomSaberButton()
        {
            Logger.Log("Adding custom saber button", LogLevel.Debug);

            MenuButtonUI.AddButton("Saber Menu", delegate ()
            {
                if (_saberListFlowCoordinator == null)
                {
                    _saberListFlowCoordinator = new GameObject("SaberListFlowCoordinator").AddComponent<SaberListFlowCoordinator>();
                    _saberListFlowCoordinator.mainFlowCoordinator = _mainFlowCoordinator;
                    _saberListFlowCoordinator.OnContentCreated = (content) =>
                    {
                        content.backButtonPressed = () =>
                        {
                            _mainFlowCoordinator.InvokePrivateMethod("DismissFlowCoordinator", new object[] { _saberListFlowCoordinator, null, false });
                        };
                        return "Saber Select";
                    };
                    //_mainFlowCoordinator
                }
                ReflectionUtil.InvokePrivateMethod(_mainFlowCoordinator, "PresentFlowCoordinator", new object[] { _saberListFlowCoordinator, null, false, false });
            });
        }
    }
}
