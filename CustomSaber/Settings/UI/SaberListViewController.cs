using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using CustomSaber.Data;
using CustomSaber.Utilities;
using HMUI;
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.XR;

namespace CustomSaber.Settings.UI
{
    internal class SaberListViewController : BSMLResourceViewController
    {
        public override string ResourceName => "CustomSaber.Settings.UI.Views.saberList.bsml";

        public static SaberListViewController Instance;

        private bool isGeneratingPreview;
        private GameObject preview;

        // Sabers
        private GameObject previewSabers;
        private GameObject leftSaber;
        private GameObject rightSaber;

        // SaberPositions (Local to the previewer)
        private Vector3 sabersPos = new Vector3(0, 0, 0);
        private Vector3 saberLeftPos = new Vector3(0, 0, 0);
        private Vector3 saberRightPos = new Vector3(0, 0.5f, 0);

        public Action<CustomSaberData> customSaberChanged;

        [UIComponent("saberList")]
        public CustomListTableData customListTableData;

        [UIAction("saberSelect")]
        public void Select(TableView _, int row)
        {
            SaberAssetLoader.SelectedSaber = row;
            Configuration.CurrentlySelectedSaber = SaberAssetLoader.CustomSabers[row].FileName;

            GenerateSaberPreview(row);
        }

        [UIAction("reloadSabers")]
        public void ReloadMaterials()
        {
            SaberAssetLoader.Reload();
            SetupList();
            Select(customListTableData.tableView, SaberAssetLoader.SelectedSaber);
        }

        [UIAction("deleteSaber")]
        public void DeleteCurrentSaber()
        {
            var deletedSaber = SaberAssetLoader.DeleteCurrentSaber();

            if (deletedSaber == 0) return;

            SetupList();
            Select(customListTableData.tableView, SaberAssetLoader.SelectedSaber);
        }

        [UIAction("update-confirmation")]
        public void UpdateDeleteConfirmation() => confirmationText.text = $"Are you sure you want to delete\n<color=\"red\">{SaberAssetLoader.CustomSabers[SaberAssetLoader.SelectedSaber].Descriptor.SaberName}</color>?";

        [UIComponent("delete-saber-confirmation-text")]
        public TextMeshProUGUI confirmationText;

        [UIAction("#post-parse")]
        public void SetupList()
        {
            customListTableData.data.Clear();
            foreach (var saber in SaberAssetLoader.CustomSabers)
            {
                var customCellInfo = new CustomListTableData.CustomCellInfo(saber.Descriptor.SaberName, saber.Descriptor.AuthorName, saber.Descriptor.CoverImage?.texture);
                customListTableData.data.Add(customCellInfo);
            }

            customListTableData.tableView.ReloadData();
            var selectedSaber = SaberAssetLoader.SelectedSaber;

            customListTableData.tableView.SelectCellWithIdx(selectedSaber);
            if (!customListTableData.tableView.visibleCells.Where(x => x.selected).Any())
                customListTableData.tableView.ScrollToCellWithIdx(selectedSaber, TableViewScroller.ScrollPositionType.Beginning, true);
        }

        protected override void DidActivate(bool firstActivation, ActivationType type)
        {
            base.DidActivate(firstActivation, type);

            Instance = this;

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

                    var customSaber = SaberAssetLoader.CustomSabers[selectedSaber];
                    if (customSaber != null)
                    {
                        customSaberChanged?.Invoke(customSaber);

                        previewSabers = CreatePreviewSaber(customSaber.Sabers, preview.transform, sabersPos);
                        PositionPreviewSaber(saberLeftPos, previewSabers?.transform.Find("LeftSaber").gameObject);
                        PositionPreviewSaber(saberRightPos, previewSabers?.transform.Find("RightSaber").gameObject);

                        if (Configuration.ShowSabersInSaberMenu)
                            GenerateHandheldSaberPreview();
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
            var saberObject = InstantiateGameObject(saber, transform);
            PositionPreviewSaber(localPosition, saberObject);
            return saberObject;
        }

        public void GenerateHandheldSaberPreview()
        {
            if (SaberAssetLoader.SelectedSaber == 0) return;
            var customSaber = SaberAssetLoader.CustomSabers[SaberAssetLoader.SelectedSaber];
            var controllers = Resources.FindObjectsOfTypeAll<VRController>();
            var sabers = CreatePreviewSaber(customSaber.Sabers, preview.transform, sabersPos);

            foreach (var controller in controllers)
            {
                if (controller.node == XRNode.LeftHand)
                {
                    leftSaber = sabers?.transform.Find("LeftSaber").gameObject;

                    leftSaber.transform.parent = controller.transform;
                    leftSaber.transform.position = controller.transform.position;
                    leftSaber.transform.rotation = controller.transform.rotation;

                    controller.transform.Find("MenuHandle").gameObject.SetActive(false);
                }
                else if (controller.node == XRNode.RightHand)
                {
                    rightSaber = sabers?.transform.Find("RightSaber").gameObject;

                    rightSaber.transform.parent = controller.transform;
                    rightSaber.transform.position = controller.transform.position;
                    rightSaber.transform.rotation = controller.transform.rotation;

                    controller.transform.Find("MenuHandle").gameObject.SetActive(false);
                }
            }
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
            ClearSabers();
            DestroyGameObject(ref preview);
            ShowMenuHandles();
        }

        private void ClearSabers()
        {
            DestroyGameObject(ref previewSabers);
            ClearHandheldSabers();
        }

        public void ClearHandheldSabers()
        {
            DestroyGameObject(ref leftSaber);
            DestroyGameObject(ref rightSaber);
        }

        public void ShowMenuHandles()
        {
            foreach (var controller in Resources.FindObjectsOfTypeAll<VRController>())
            {
                controller.transform.Find("MenuHandle").gameObject.SetActive(true);
            }
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
