using System;
using System.Linq;
using TMPro;
using LogLevel = CustomSaber.Logger.LogLevel;
using UnityEngine;
using UnityEngine.UI;
using IPA.Utilities;
using BeatSaberMarkupLanguage.MenuButtons;
using BS_Utils.Utilities;
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
        public Sprite CoverImage { get; set; }
        // TODO: prop for Image
    }

    class CustomSaberUI : MonoBehaviour
    {
        //private RectTransform _mainMenuRectTransform;
        //private MainMenuViewController _mainMenuViewController;
        internal SaberUIFlowCoordinator _saberFlowCoordinator;
        private MainFlowCoordinator _mainFlowCoordinator;
        internal MainFlowCoordinator MainFlowCoordinator
        {
            get
            {
                if (_mainFlowCoordinator == null)
                    _mainFlowCoordinator = Resources.FindObjectsOfTypeAll<MainFlowCoordinator>().FirstOrDefault();
                return _mainFlowCoordinator;
            }
        }
        //  public SaberListFlowCoordinator _saberListFlowCoordinator;
        public static CustomSaberUI _instance;

     //   public class SaberListFlowCoordinator : GenericFlowCoordinator<SaberListViewController, SaberPreviewController> { };

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
                //_mainMenuRectTransform = _buttonInstance.transform.parent as RectTransform;
            }
            catch (Exception ex)
            {
                Logger.Log($"{ex.Message}\n{ex.StackTrace}", LogLevel.Error);
            }
            GameObject.DontDestroyOnLoad(this);
            CreateCustomSaberButton();
        }

        private void CreateCustomSaberButton()
        {
            Logger.Log("Adding custom saber button", LogLevel.Debug);
            MenuButtons.instance.RegisterButton(new MenuButton("Custom Sabers", "Change Custom Sabers Here!", SaberMenuButtonPressed, true));

        }

        internal void ShowSaberFlow()
        {
            if (_saberFlowCoordinator == null)
                _saberFlowCoordinator = BeatSaberMarkupLanguage.BeatSaberUI.CreateFlowCoordinator<SaberUIFlowCoordinator>();
            MainFlowCoordinator.InvokeMethod("PresentFlowCoordinator", _saberFlowCoordinator, null, false, false);
        }

        private void SaberMenuButtonPressed()
        {
          //  Logger.logger.Info("Saber Menu Button Pressed");
            ShowSaberFlow();
        }
    }
}
