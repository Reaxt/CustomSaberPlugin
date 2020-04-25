using System.Linq;
using UnityEngine;
using IPA.Utilities;

namespace CustomSaber.Utilities
{
    class DummySaber : MonoBehaviour
    {
        void Start()
        {
            var playerDataModel = Resources.FindObjectsOfTypeAll<PlayerDataModel>().FirstOrDefault();
            var playerData = ReflectionUtil.GetField<PlayerData, PlayerDataModel>(playerDataModel, "_playerData");
            var colorSchemeSettings = playerData.colorSchemesSettings;
            var colorScheme = (colorSchemeSettings.overrideDefaultColors) ? colorSchemeSettings.GetColorSchemeForId(colorSchemeSettings.selectedColorSchemeId) : GetDefaultColorScheme();

            var color = (gameObject.name == "LeftSaber") ? colorScheme.saberAColor : colorScheme.saberBColor;

            if (SaberAssetLoader.SelectedSaber == 0)
            {
                foreach (var r in gameObject.GetComponentsInChildren<Renderer>())
                {
                    foreach (var m in r.materials)
                    {
                        m.color = color;
                        if (m.HasProperty("_Color")) m.SetColor("_Color", color);
                        if (m.HasProperty("_TintColor")) m.SetColor("_TintColor", color);
                        if (m.HasProperty("_AddColor")) m.SetColor("_AddColor", (color * 0.5f).ColorWithAlpha(0f));
                    }
                }
            }
            else
            {
                SaberScript.ApplyColorsToSaber(this.gameObject, color);
            }
            var trails = gameObject.GetComponentsInChildren<Xft.XWeaponTrail>();
            foreach (var trail in trails) trail.color = color;
        }

        private ColorScheme GetDefaultColorScheme()
        {
            var colorManager = Resources.FindObjectsOfTypeAll<ColorManager>().FirstOrDefault();

            return ReflectionUtil.GetField<ColorScheme, ColorManager>(colorManager, "_colorScheme");
        }
    }
}
