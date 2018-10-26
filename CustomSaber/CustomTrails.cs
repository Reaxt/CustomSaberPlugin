using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Xft;

namespace CustomSaber
{
    public class CustomTrail : MonoBehaviour
    {

        public Transform PointStart;
        public Transform PointEnd;
        public Material TrailMaterial;
        public Color TrailColor = new Color(1.0f,1.0f,1.0f,1.0f);
        public int Length = 14;

        private SaberWeaponTrail trail;
        private Saber saber;

        private TrailColorManager trailColorManager;

        public void Init(Saber parentSaber)
        {
            Console.WriteLine("Replacing Trail");

            saber = parentSaber;

            trailColorManager = new TrailColorManager();
            trailColorManager.trailColor = TrailColor;

            if (gameObject.name != "LeftSaber" && gameObject.name != "RightSaber")
            {
                Console.WriteLine("Parent not LeftSaber or RightSaber");
                Destroy(this);
            }

            if (saber == null)
            {
                Console.WriteLine("Saber not found");
                Destroy(this);
            }

            trail = saber.GetComponent<SaberWeaponTrail>();
            if (trail != null)
            {
                try
                {
                    ReflectionUtil.SetPrivateField(trail, "_colorManager", trailColorManager);

                    if (PointStart != null)
                        ReflectionUtil.SetPrivateField(trail, "_pointStart", PointStart);
                    if (PointEnd != null)
                        ReflectionUtil.SetPrivateField(trail, "_pointEnd", PointEnd);

                    ReflectionUtil.SetPrivateField(trail, "_color", TrailColor);
                    ReflectionUtil.SetPrivateField(trail, "_maxFrame", Length);
                    ReflectionUtil.SetPrivateField(trail, "_multiplierSaberColor", new Color(1f, 1f, 1f, 1f));

                    XWeaponTrailRenderer trailRenderer = ReflectionUtil.GetPrivateField<XWeaponTrailRenderer>(trail, "_trailRenderer");
                    MeshRenderer meshRenderer = ReflectionUtil.GetPrivateField<MeshRenderer>(trailRenderer, "_meshRenderer");
                    meshRenderer.material = TrailMaterial;

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    Console.WriteLine(e.Message);
                    throw;
                }
            }
            else
            {
                Console.WriteLine("Trail not found");
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
            ReflectionUtil.SetPrivateField(trail, "MyMaterial", TrailMaterial);
        }

        public void SetColor(Color newColor)
        {
            TrailColor = newColor;
            ReflectionUtil.SetPrivateField(trail, "MyColor", TrailColor);
        }

        public static GameObject FindParentWithName(GameObject childObject, string name)
        {
            Transform t = childObject.transform;
            while (t.parent != null)
            {
                if (t.parent.gameObject.name == name)
                {
                    return t.parent.gameObject;
                }
                t = t.parent.transform;
            }
            return null;
        }
    }
}
