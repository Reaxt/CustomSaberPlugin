using CustomUI.BeatSaber;
using HMUI;
using IPA.Utilities;
using System.Linq;
using TMPro;
using UnityEngine;

namespace CustomSaber
{
    class SaberListViewController : CustomListViewController
    {
        private int selected = 0;

        private bool CustomColorsPresent = IPA.Loader.PluginManager.Plugins.Any(x => x.Name == "CustomColorsEdit" || x.Name == "Custom Colors")
            || IPA.Loader.PluginManager.AllPlugins.Any(x => x.Metadata.Id == "Custom Colors");

        private MenuShockwave menuShockwave = Resources.FindObjectsOfTypeAll<MenuShockwave>().FirstOrDefault();

        private bool menuShockwaveOriginalState;

        public override void __Activate(ActivationType activationType)
        {
            for (var i = 0; i < SaberLoader.AllSabers.Count; i++)
            {
                if (SaberLoader.AllSabers[i].Path == Plugin._currentSaberName)
                {
                    selected = i;
                }
            }

            base.__Activate(activationType);
            _customListTableView.SelectCellWithIdx(selected);

            if (_backButton == null)
            {
                _backButton = BeatSaberUI.CreateBackButton(rectTransform as RectTransform);
                _backButton.onClick.AddListener(delegate ()
                {
                    backButtonPressed?.Invoke();

                    SaberPreviewController.Instance.DestroyPreview();
                    _customListTableView.didSelectCellWithIdxEvent -= _sabersTableView_DidSelectRowEvent;
                    if (menuShockwave)
                    {
                        menuShockwave.enabled = menuShockwaveOriginalState;
                    }

                    if (CustomColorsPresent)
                    {
                        SaberPreviewController.Instance.CallCustomColors(false);
                    }
                });
            }

            PreviewCurrent();

            if (menuShockwave)
            {
                menuShockwaveOriginalState = menuShockwave.enabled;
                menuShockwave.enabled = false;
            }

            var _versionNumber = BeatSaberUI.CreateText(rectTransform, "Text", new Vector2(-10f, -10f));

            (_versionNumber.transform as RectTransform).anchoredPosition = new Vector2(-10f, 10f);
            (_versionNumber.transform as RectTransform).anchorMax = new Vector2(1f, 0f);
            (_versionNumber.transform as RectTransform).anchorMin = new Vector2(1f, 0f);

            _versionNumber.text = $"v{Plugin.PluginVersion}";
            _versionNumber.fontSize = 5;
            _versionNumber.color = Color.white;

            _customListTableView.didSelectCellWithIdxEvent += _sabersTableView_DidSelectRowEvent;
            _customListTableView.ScrollToCellWithIdx(SaberLoader.FindSaberByName(Plugin._currentSaberName), TableView.ScrollPositionType.Beginning, false);
            _customListTableView.SelectCellWithIdx(SaberLoader.FindSaberByName(Plugin._currentSaberName), false);
        }

        protected override void DidDeactivate(DeactivationType type) => base.DidDeactivate(type);

        private void PreviewCurrent()
        {
            if (selected != 0)
            {
                SaberPreviewController.Instance.GeneratePreview(selected);
            }
            else
            {
                SaberPreviewController.Instance.DestroyPreview();
            }
        }

        public override float CellSize() => 8.5f;

        public override int NumberOfCells() => SaberLoader.AllSabers?.Count ?? 0;

        private void _sabersTableView_DidSelectRowEvent(TableView sender, int row)
        {
            Plugin._currentSaberName = SaberLoader.AllSabers[row].Name;
            selected = row;
            if (row == 0)
            {
                SaberPreviewController.Instance.DestroyPreview();
            }
            else
            {
                SaberPreviewController.Instance.GeneratePreview(row);
            }
        }

        public override TableCell CellForIdx(int idx)
        {
            var saber = SaberLoader.AllSabers[idx];
            var _tableCell = GetTableCell(false);
            _tableCell.GetPrivateField<TextMeshProUGUI>("_songNameText").text = saber.Name;
            _tableCell.GetPrivateField<TextMeshProUGUI>("_authorText").text = saber.Author;
            _tableCell.GetPrivateField<UnityEngine.UI.RawImage>("_coverRawImage").texture = saber.CoverImage.texture;
            _tableCell.reuseIdentifier = "CustomSaberListCell";
            return _tableCell;
        }
    }
}