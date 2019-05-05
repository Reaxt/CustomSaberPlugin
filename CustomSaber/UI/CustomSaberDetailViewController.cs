using System.Linq;
using TMPro;
using VRUI;
using UnityEngine;

namespace CustomSaber.UI
{
    internal class CustomSaberDetailViewController : VRUIViewController
    {
        TextMeshProUGUI songNameText;
        TextMeshProUGUI authorNameText;

        protected override void DidActivate(bool firstActivation, ActivationType type)
        {
            if (firstActivation)
            {
                RemoveCustomUIElements(rectTransform);

                RectTransform _levelDetails = GetComponentsInChildren<RectTransform>().First(x => x.name == "LevelDetails");
                RectTransform _yourStats = GetComponentsInChildren<RectTransform>(true).First(x => x.name == "YourStats");
                _yourStats.gameObject.SetActive(false);

                TextMeshProUGUI[] _textComponents = GetComponentsInChildren<TextMeshProUGUI>(true);

                songNameText = _textComponents.First(x => x.name == "SongNameText");

                authorNameText = _textComponents.First(x => x.name == "NotesCountValueText");
                _textComponents.First(x => x.name == "NotesCountText").text = "Author";

                authorNameText.rectTransform.sizeDelta = new Vector2(16f, 3f);
                //authorNameText.alignment = TextAlignmentOptions.CaplineRight;

                if (_textComponents.Any(x => x.name == "BPMText"))
                {
                    _textComponents.First(x => x.name == "BPMText").gameObject.SetActive(false);
                    _textComponents.First(x => x.name == "BPMValueText").gameObject.SetActive(false);
                }
                if (_textComponents.Any(x => x.name == "DurationText"))
                {
                    _textComponents.First(x => x.name == "DurationText").gameObject.SetActive(false);
                    _textComponents.First(x => x.name == "DurationValueText").gameObject.SetActive(false);
                }
                if (_textComponents.Any(x => x.name == "ObstaclesCountText"))
                {
                    _textComponents.First(x => x.name == "ObstaclesCountText").gameObject.SetActive(false);
                    _textComponents.First(x => x.name == "ObstaclesCountValueText").gameObject.SetActive(false);
                }

            }
        }

        public void UpdateContent(CustomSaber saberInfo)
        {
            songNameText.text = saberInfo.Name;
            authorNameText.text = saberInfo.Author;
        }

        void RemoveCustomUIElements(Transform parent)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                Transform child = parent.GetChild(i);

                if (child.name.Contains("CustomUI"))
                {
                    Destroy(child.gameObject);
                }
                if (child.childCount > 0)
                {
                    RemoveCustomUIElements(child);
                }
            }
        }

    }
}
