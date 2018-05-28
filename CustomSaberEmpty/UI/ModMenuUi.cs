using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace CustomSaber
{
    class ModMenuUi : MonoBehaviour
    {
        static RectTransform _rightPos;
        internal static ModMenuUi Instance;

        public static List<Sprite> Icons = new List<Sprite>();

        // Custom view controller
        private static ModMenuMasterViewController _modMenuController;

        private Button _buttonInstance;
        private Button _cogWheelButtonInstance;
        private Button _backButtonInstance;

        private Button _upArrowBtn;
        private Button _downArrowBtn;
        private RectTransform _mainMenuRectTransform;
        void Update()
        {
            //DEBUG LINE<
            if (Input.GetKeyDown(KeyCode.Home))
            {
            }
        }
        public static void OnLoad()
        {

            if (ModMenuUi.Instance != null)
            {
                ModMenuUi.Instance.Awake();
                return;
            }
            if (GameObject.FindObjectOfType<ModMenuUi>() != null)
            {
                return;
            }
            new GameObject("modmenu").AddComponent<ModMenuUi>();
        }

        void Awake()
        {

            Instance = this;
            //DontDestroyOnLoad(gameObject);

            foreach (Sprite sprite in Resources.FindObjectsOfTypeAll<Sprite>())
            {
                Icons.Add(sprite);
            }

            try
            {
                // Get necessary button instances and main menu VC
                var allButtons = Resources.FindObjectsOfTypeAll<Button>();

                _buttonInstance = Resources.FindObjectsOfTypeAll<Button>().First(x => x.name == "QuitButton");
                _cogWheelButtonInstance = allButtons.FirstOrDefault(x => x.name == "SettingsButton");
                _downArrowBtn = allButtons.First(x => x.name == "PageDownButton");
                _upArrowBtn = allButtons.First(x => x.name == "PageUpButton");
                _backButtonInstance = allButtons.First(x => x.name == "BackArrowButton");
                _mainMenuRectTransform = (RectTransform)_buttonInstance.transform.parent;

                AddModMenuButton();
            }
            catch (Exception ex)
            {
            }
        }

        private void AddModMenuButton()
        {
            
        }

        public Button CreateButton(RectTransform parent)
        {
            return null;
        }

        public Button CreateButton(RectTransform parent, string templateButtonName)
        {
            return null;
        }

        public Button CreateDownButton(RectTransform parent)
        {
            return null;
        }

        public Button CreateUpButton(RectTransform parent)
        {
            return null;
        }

        public void SetButtonText(ref Button button, string text)
        {
        }

        public void SetButtonIcon(ref Button button, Sprite icon)
        {
        }

        public void SetButtonBackground(ref Button button, Sprite background)
        {
        }

        public Button CreateBackButton(RectTransform parent)
        {
            return null;
        }

    }
}