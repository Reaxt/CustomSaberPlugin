using HMUI;
using IPA.Utilities;
using System.Linq;
using TMPro;
using UnityEngine;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using System;
namespace CustomSaber
{
    class SaberListView : BeatSaberMarkupLanguage.ViewControllers.BSMLResourceViewController
    {
        public override string ResourceName => "CustomSaber.UI.saberList.bsml";
        [UIComponent("saberList")]
        public CustomListTableData customListTableData;
        int selectedSaber = 0;

        public GameObject _saberPreview;
        private GameObject PreviewSaber;
        private GameObject _previewParent;
        public GameObject _saberPreviewA;
        public GameObject _saberPreviewB;
        public GameObject _saberPreviewAParent;
        public GameObject _saberPreviewBParent;
        private bool PreviewStatus;


        [UIAction("saberSelect")]
        internal void SelectSaber(TableView tableView, int row)
        {
            Plugin._currentSaberName = SaberLoader.AllSabers[row].Name;
            selectedSaber = row;
            PreviewCurrent();

        }

        protected override void DidDeactivate(DeactivationType deactivationType)
        {
            base.DidDeactivate(deactivationType);
            DestroyPreview();
        }

        [UIAction("#post-parse")]
        internal void SetupSaberList()
        {
            customListTableData.data.Clear();
            foreach (CustomSaber saber in SaberLoader.AllSabers)
            {
                customListTableData.data.Add(new CustomListTableData.CustomCellInfo(saber.Name, saber.Author, saber.CoverImage.texture));
            }
            customListTableData.tableView.ReloadData();
            selectedSaber = SaberLoader.FindSaberByName(Plugin._currentSaberName);

            customListTableData.tableView.ScrollToCellWithIdx(selectedSaber, HMUI.TableViewScroller.ScrollPositionType.Beginning, false);
            customListTableData.tableView.SelectCellWithIdx(selectedSaber); //(0, HMUI.TableViewScroller.ScrollPositionType.Beginning, false);
            PreviewCurrent();

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
                    Logger.Log($"{ex.Message}\n{ex.StackTrace}", Logger.LogLevel.Error);
                }
            }
            else
            {
                Logger.Log($"Failed to load preview. {SaberLoader.AllSabers[SaberIndex].Name}", Logger.LogLevel.Warning);
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

        private void PreviewCurrent()
        {
            if (selectedSaber != 0)
            {
                GeneratePreview(selectedSaber);
            }
            else
            {
                DestroyPreview();
            }
        }
    }

    /*
    class SaberListViewController : CustomListViewController
    {
        private int selected = 0;


        private MenuShockwave menuShockwave = Resources.FindObjectsOfTypeAll<MenuShockwave>().FirstOrDefault();

        private bool menuShockwaveOriginalState;

        public override void __Activate(ActivationType activationType)
        {
            selected = SaberLoader.FindSaberByName(Plugin._currentSaberName);

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
            _customListTableView.ScrollToCellWithIdx(selected, TableViewScroller.ScrollPositionType.Beginning, false);
            _customListTableView.SelectCellWithIdx(selected, false);
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

        public override TableCell CellForIdx(TableView tableView, int row)
        {
            var saber = SaberLoader.AllSabers[row];
            var _tableCell = GetTableCell(false);
            _tableCell.GetPrivateField<TextMeshProUGUI>("_songNameText").text = saber.Name;
            _tableCell.GetPrivateField<TextMeshProUGUI>("_authorText").text = saber.Author;
            _tableCell.GetPrivateField<UnityEngine.UI.RawImage>("_coverRawImage").texture = saber.CoverImage.texture;
            _tableCell.reuseIdentifier = "CustomSaberListCell";
            return _tableCell;
        }
    }
    */
}