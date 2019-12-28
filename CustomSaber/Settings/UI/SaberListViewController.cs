using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using CustomSaber.Data;
using CustomSaber.Utilities;
using HMUI;
using System;
using UnityEngine;

namespace CustomSaber.Settings.UI
{
    internal class SaberListView : BSMLResourceViewController
    {
        public override string ResourceName => "CustomSaber.Settings.UI.Views.saberList.bsml";

        private bool isGeneratingPreview;
        private GameObject preview;

        // Sabers
        private GameObject sabers;

        // SaberPositions (Local to the previewer)
        private Vector3 sabersPos = new Vector3(0, 0, 0);
        private Vector3 saberLeftPos = new Vector3(0, 0, 0);
        private Vector3 saberRightPos = new Vector3(0, 0.5f, 0);

        [UIComponent("saberList")]
        public CustomListTableData customListTableData;

        [UIAction("saberSelect")]
        internal void Select(TableView _, int row)
        {
            SaberAssetLoader.SelectedSaber = row;
            Configuration.CurrentlySelectedSaber = SaberAssetLoader.CustomSabers[row].FileName;

            GenerateSaberPreview(row);
        }

        [UIAction("#post-parse")]
        public void SetupList()
        {
            customListTableData.data.Clear();
            foreach (CustomSaberData saber in SaberAssetLoader.CustomSabers)
            {
                CustomListTableData.CustomCellInfo customCellInfo = new CustomListTableData.CustomCellInfo(saber.SaberDescriptor.SaberName, saber.SaberDescriptor.AuthorName, saber.SaberDescriptor.CoverImage.texture);
                customListTableData.data.Add(customCellInfo);
            }

            customListTableData.tableView.ReloadData();
            int selectedSaber = SaberAssetLoader.SelectedSaber;

            customListTableData.tableView.ScrollToCellWithIdx(selectedSaber, TableViewScroller.ScrollPositionType.Beginning, false);
            customListTableData.tableView.SelectCellWithIdx(selectedSaber);
        }

        protected override void DidActivate(bool firstActivation, ActivationType type)
        {
            base.DidActivate(firstActivation, type);

            if (!preview)
            {
                preview = new GameObject();
                preview.transform.position = new Vector3(2.2f, 1.3f, 1.0f);
                preview.transform.Rotate(0.0f, 330.0f, 0.0f);
            }

            Select(customListTableData.tableView, SaberAssetLoader.SelectedSaber);
        }

        protected override void DidDeactivate(DeactivationType deactivationType)
        {
            base.DidDeactivate(deactivationType);
            ClearPreview();
        }

        public void GenerateSaberPreview(int selectedSaber)
        {
            if (!isGeneratingPreview)
            {
                try
                {
                    isGeneratingPreview = true;
                    ClearSabers();

                    CustomSaberData customSaber = SaberAssetLoader.CustomSabers[selectedSaber];
                    if (customSaber != null)
                    {
                        sabers = CreatePreviewSaber(customSaber.Sabers, preview.transform, sabersPos);
                        PositionPreviewSaber(saberLeftPos, sabers?.transform.Find("LeftSaber").gameObject);
                        PositionPreviewSaber(saberRightPos, sabers?.transform.Find("RightSaber").gameObject);
                    }
                }
                catch (Exception ex)
                {
                    Logger.log.Error(ex);
                }
                finally
                {
                    isGeneratingPreview = false;
                }
            }
        }

        private GameObject CreatePreviewSaber(GameObject saber, Transform transform, Vector3 localPosition)
        {
            GameObject saberObject = InstantiateGameObject(saber, transform);
            PositionPreviewSaber(localPosition, saberObject);
            return saberObject;
        }

        private GameObject InstantiateGameObject(GameObject gameObject, Transform transform = null)
        {
            if (gameObject)
            {
                return transform ? Instantiate(gameObject, transform) : Instantiate(gameObject);
            }

            return null;
        }

        private void PositionPreviewSaber(Vector3 vector, GameObject saberObject)
        {
            if (saberObject && vector != null)
            {
                saberObject.transform.localPosition = vector;
            }
        }

        private void ClearPreview()
        {
            DestroyGameObject(ref preview);
            ClearSabers();
        }

        private void ClearSabers()
        {
            DestroyGameObject(ref sabers);
        }

        private void DestroyGameObject(ref GameObject gameObject)
        {
            if (gameObject)
            {
                Destroy(gameObject);
                gameObject = null;
            }
        }
    }
}
