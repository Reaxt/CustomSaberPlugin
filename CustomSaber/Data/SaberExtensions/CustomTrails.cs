#if PLUGIN
using CustomSaber.Settings;
using CustomSaber.Utilities;
using IPA.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Xft;
#endif
using UnityEngine;

// Class has to be in this namespace due to compatibility
namespace CustomSaber
{
    public enum ColorType
    {
        LeftSaber,
        RightSaber,
        CustomColor
    }

    [UnityEngine.AddComponentMenu("Custom Sabers/Custom Trail")]
    public class CustomTrail : MonoBehaviour
    {
        public Transform PointStart;
        public Transform PointEnd;
        public Material TrailMaterial;
        public ColorType colorType = ColorType.CustomColor;
        public Color TrailColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        public Color MultiplierColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        public int Length = 20;

#if PLUGIN
        private CustomWeaponTrail trail;
        private XWeaponTrailRenderer oldTrailRendererPrefab;

        public void Init(Saber saber, ColorManager colorManager)
        {
            Logger.log.Debug($"Replacing Trail for '{saber?.saberType}'");

            if (gameObject.name != "LeftSaber" && gameObject.name != "RightSaber")
            {
                Logger.log.Warn("Parent not LeftSaber or RightSaber");
                Destroy(this);
            }

            if (!saber)
            {
                Logger.log.Warn("Saber not found");
                Destroy(this);
            }

            IEnumerable<XWeaponTrail> trails = Resources.FindObjectsOfTypeAll<XWeaponTrail>();
            foreach (XWeaponTrail trail in trails)
            {
                ReflectionUtil.SetField(trail, "_trailWidth", 0f);
            }

            XWeaponTrail oldtrail = Resources.FindObjectsOfTypeAll<GameCoreSceneSetup>().FirstOrDefault()
                ?.GetField<BasicSaberModelController, GameCoreSceneSetup>("_basicSaberModelControllerPrefab")
                ?.GetField<XWeaponTrail, BasicSaberModelController>("_saberWeaponTrail");

            if (oldtrail)
            {
                try
                {
                    oldTrailRendererPrefab = ReflectionUtil.GetField<XWeaponTrailRenderer, XWeaponTrail>(oldtrail, "_trailRendererPrefab");
                }
                catch (Exception ex)
                {
                    Logger.log.Error(ex);
                    throw;
                }

                if (Configuration.OverrideTrailLength) Length = (int) (Length * Configuration.TrailLength);

                if (Length > 1)
                {
                    trail = gameObject.AddComponent<CustomWeaponTrail>();
                    trail.Init(oldTrailRendererPrefab, colorManager, PointStart, PointEnd, TrailMaterial, TrailColor, Length, MultiplierColor, colorType);
                }
            }
            else
            {
                Logger.log.Debug($"Trail not found for '{saber?.saberType}'");
                Destroy(this);
            }
        }

        public void EnableTrail(bool enable)
        {
            trail.enabled = enable;
        }

        public void SetMaterial(Material newMat)
        {
            TrailMaterial = newMat;
            trail.SetMaterial(newMat);
        }

        public void SetColor(Color newColor)
        {
            TrailColor = newColor;
            trail.SetColor(newColor);
        }
#endif
    }
}
