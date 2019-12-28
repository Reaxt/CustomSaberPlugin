using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class CompileSaberWindow : EditorWindow
{
    private SaberDescriptor[] sabers;

    [MenuItem("Window/Saber Exporter")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(CompileSaberWindow), false, "Saber Exporter");
    }

    private void OnGUI()
    {
        GUILayout.Label("Sabers", EditorStyles.boldLabel);
        GUILayout.Space(20);

        foreach (SaberDescriptor saber in sabers)
        {
            GUILayout.Label("GameObject : " + saber.gameObject.name, EditorStyles.boldLabel);
            saber.AuthorName = EditorGUILayout.TextField("Author name", saber.AuthorName);
            saber.SaberName = EditorGUILayout.TextField("Saber name", saber.SaberName);
            saber.CoverImage = (Sprite)EditorGUILayout.ObjectField("Cover Image", saber.CoverImage, typeof(Sprite), false);

            EditorGUI.BeginDisabledGroup(saber.transform.Find("LeftSaber") == null || saber.transform.Find("RightSaber") == null);
            if (GUILayout.Button("Export " + saber.SaberName))
            {
                GameObject saberObject = saber.gameObject;
                if (saberObject != null && saber != null)
                {
                    string path = EditorUtility.SaveFilePanel("Save saber file", "", saber.SaberName + ".saber", "saber");
                    Debug.Log($"Is path empty?: ({string.IsNullOrWhiteSpace(path)})");

                    if (!string.IsNullOrWhiteSpace(path))
                    {
                        string fileName = Path.GetFileName(path);
                        string folderPath = Path.GetDirectoryName(path);

                        Selection.activeObject = saberObject;
                        EditorUtility.SetDirty(saber);
                        EditorSceneManager.MarkSceneDirty(saberObject.scene);
                        EditorSceneManager.SaveScene(saberObject.scene);
                        PrefabUtility.CreatePrefab("Assets/_CustomSaber.prefab", Selection.activeObject as GameObject);
                        AssetBundleBuild assetBundleBuild = default(AssetBundleBuild);
                        assetBundleBuild.assetNames = new string[] {
                            "Assets/_CustomSaber.prefab"
                        };

                        assetBundleBuild.assetBundleName = fileName;

                        BuildTargetGroup selectedBuildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
                        BuildTarget activeBuildTarget = EditorUserBuildSettings.activeBuildTarget;

                        BuildPipeline.BuildAssetBundles(Application.temporaryCachePath, new AssetBundleBuild[] { assetBundleBuild }, 0, EditorUserBuildSettings.activeBuildTarget);
                        EditorPrefs.SetString("currentBuildingAssetBundlePath", folderPath);
                        EditorUserBuildSettings.SwitchActiveBuildTarget(selectedBuildTargetGroup, activeBuildTarget);
                        AssetDatabase.DeleteAsset("Assets/_CustomSaber.prefab");

                        if (File.Exists(path))
                        {
                            bool isDirectory = (File.GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory;
                            if (!isDirectory)
                            {
                                File.Delete(path);
                            }
                        }

                        File.Move(Path.Combine(Application.temporaryCachePath, fileName), path);
                        AssetDatabase.Refresh();
                        EditorUtility.DisplayDialog("Export Successful!", "Export Successful!", "OK");
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("Export Failed!", "Path is invalid.", "OK");
                    }
                }
                else
                {
                    EditorUtility.DisplayDialog("Export Failed!", "Saber GameObject is missing.", "OK");
                }
            }

            EditorGUI.EndDisabledGroup();

            if (saber.transform.Find("LeftSaber") == null)
            {
                GUILayout.Label("LeftSaber gameObject is missing", EditorStyles.boldLabel);
            }

            if (saber.transform.Find("RightSaber") == null)
            {
                GUILayout.Label("RightSaber gameObject is missing", EditorStyles.boldLabel);
            }

            if (GetObjectBounds(saber.gameObject).extents.z * 2.0 > 2.0)
            {
                GUILayout.Label("The saber might be too long", EditorStyles.boldLabel);
            }

            if (GetObjectBounds(saber.gameObject).extents.x * 2.0 > 1.0)
            {
                GUILayout.Label("The saber might be too large", EditorStyles.boldLabel);
            }

            GUILayout.Space(20);
        }
    }

    private void OnFocus()
    {
        sabers = GameObject.FindObjectsOfType<SaberDescriptor>();
    }

    private Bounds GetObjectBounds(GameObject gameObject)
    {
        Bounds bounds = new Bounds(gameObject.transform.position, Vector3.zero);
        foreach (Renderer renderer in gameObject.GetComponentsInChildren<Renderer>())
        {
            bounds.Encapsulate(renderer.bounds);
        }

        return bounds;
    }
}
