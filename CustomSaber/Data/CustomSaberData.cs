using CustomSaber.Utilities;
using System.IO;
using UnityEngine;

namespace CustomSaber.Data
{
    public class CustomSaberData
    {
        public string FileName { get; }
        public AssetBundle AssetBundle { get; }
        public SaberDescriptor Descriptor { get; }
        public GameObject Sabers { get; }
        public string ErrorMessage { get; } = string.Empty;

        public CustomSaberData(string fileName)
        {
            FileName = fileName;

            if (fileName != "DefaultSabers")
            {
                try
                {
                    AssetBundle = AssetBundle.LoadFromFile(Path.Combine(Plugin.PluginAssetPath, fileName));
                    Sabers = AssetBundle.LoadAsset<GameObject>("_CustomSaber");
                    Descriptor = Sabers.GetComponent<SaberDescriptor>();
                    Descriptor.CoverImage = Descriptor.CoverImage ?? Utils.GetDefaultCoverImage();
                }
                catch
                {
                    Logger.log.Warn($"Something went wrong getting the AssetBundle for '{fileName}'!");

                    Descriptor = new SaberDescriptor
                    {
                        SaberName = "Invalid Saber (Delete it)",
                        AuthorName = fileName,
                        CoverImage = Utils.GetErrorCoverImage()
                    };

                    ErrorMessage = $"File: '{fileName}'" +
                                    "\n\nThis file failed to load." +
                                    "\n\nThis may have been caused by having duplicated files," +
                                    " another saber with the same name already exists or that the custom saber is simply just broken." +
                                    "\n\nThe best thing is probably just to delete it!";

                    FileName = "DefaultSabers";
                }
            }
            else
            {
                Descriptor = new SaberDescriptor
                {
                    SaberName = "Default",
                    AuthorName = "Beat Saber",
                    Description = "This is the default sabers. (No preview available)",
                    CoverImage = Utils.GetRandomCoverImage(),
                };
            }
        }

        public CustomSaberData(GameObject leftSaber, GameObject rightSaber)
        {
            FileName = "DefaultSabers";

            Descriptor = new SaberDescriptor
            {
                SaberName = "Default",
                AuthorName = "Beat Games",
                Description = "This is the default sabers.",
                CoverImage = Utils.GetRandomCoverImage(),
            };

            GameObject saberParent = new GameObject();
            if (saberParent)
            {
                leftSaber.transform.SetParent(saberParent.transform);
                rightSaber.transform.SetParent(saberParent.transform);
            }

            Sabers = saberParent;
        }

        public void Destroy()
        {
            if (AssetBundle != null)
            {
                AssetBundle.Unload(true);
            }
            else
            {
                Object.Destroy(Descriptor);
            }
        }
    }
}
