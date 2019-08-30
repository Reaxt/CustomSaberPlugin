using CustomUI.BeatSaber;
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using LogLevel = IPA.Logging.Logger.Level;

namespace CustomSaber
{
    public class SaberPreviewController : CustomListViewController
    {
        public static SaberPreviewController Instance;

        public GameObject _saberPreview;
        private GameObject PreviewSaber;
        private GameObject _previewParent;
        public GameObject _saberPreviewA;
        public GameObject _saberPreviewB;
        public GameObject _saberPreviewAParent;
        public GameObject _saberPreviewBParent;

        private bool PreviewStatus;

        protected override void DidActivate(bool firstActivation, ActivationType activationType)
        {
            if (firstActivation) FirstActivation();
        }

        private void FirstActivation()
        {
            Instance = this;
            var container = new GameObject("SaberPreviewContainer", typeof(RectTransform)).transform as RectTransform;
            container.SetParent(rectTransform, false);
            container.anchorMin = new Vector2(0.05f, 0.0f);
            container.anchorMax = new Vector2(0.95f, 1.0f);
            container.sizeDelta = new Vector2(0, 0);

            System.Action<RectTransform, float, float, float, float, float> relative_layout =
                (RectTransform rt, float x, float y, float w, float h, float pivotx) =>
                {
                    rt.anchorMin = new Vector2(x, y);
                    rt.anchorMax = new Vector2(x + w, y + h);
                    rt.pivot = new Vector2(pivotx, 1f);
                    rt.sizeDelta = Vector2.zero;
                    rt.anchoredPosition = Vector2.zero;
                };

            var text = BeatSaberUI.CreateText(container, "Preview", Vector2.zero);
            text.fontSize = 6.0f;
            text.alignment = TextAlignmentOptions.Center;
            relative_layout(text.rectTransform, 0f, 0.85f, 1f, 0.166f, 0.5f);
        }

        public void GeneratePreview(int SaberIndex)
        {
            var selected = SaberIndex;
            Logger.Log($"Selected saber {SaberLoader.AllSabers[SaberIndex].Name} created by {SaberLoader.AllSabers[SaberIndex].Author}");

            if (PreviewStatus)
            {
                return;
            }

            PreviewStatus = true;
            DestroyPreview();

            if (SaberLoader.AllSabers[SaberIndex] != null)
            {
                try
                {
                    PreviewSaber = SaberLoader.AllSabers[SaberIndex].GameObject;

                    _previewParent = new GameObject();
                    _previewParent.transform.Translate(2.2f, 1.3f, 0.75f);
                    _previewParent.transform.Rotate(0, -30, 0);

                    if (PreviewSaber)
                    {
                        _saberPreview = Instantiate(PreviewSaber, _previewParent.transform);
                        _saberPreview.name = "Saber Preview";
                        _saberPreview.transform.Find("LeftSaber").transform.localPosition = new Vector3(0, 0, 0);
                        _saberPreview.transform.Find("RightSaber").transform.localPosition = new Vector3(0, 0, 0);
                        _saberPreview.transform.Find("RightSaber").transform.Translate(0, 0.5f, 0);

                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }
            }
            else
            {
                Logger.Log($"Failed to load preview. {SaberLoader.AllSabers[SaberIndex].Name}", LogLevel.Warning);
            }
            PreviewStatus = false;
        }

        public void DestroyPreview()
        {
            if (_saberPreview)
            {
                _saberPreview.name = "";
                Destroy(_saberPreview);
            }

            PreviewSaber = null;
            if (_previewParent)
            {
                Destroy(_previewParent);
            }
        }

    }
}
