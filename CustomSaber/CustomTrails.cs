using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Xft;

namespace CustomSaber
{
    class CustomTrail : MonoBehaviour
    {

        public Transform PointStart;
        public Transform PointEnd;
        public Material TrailMaterial;
        public Color TrailColor = new Color(1.0f,1.0f,1.0f,1.0f);
        public int Length = 14;

        private XWeaponTrail trail;

        private void Start()
        {
            Console.WriteLine("Replacing Trail");

            if (gameObject.name != "LeftSaber" && gameObject.name != "RightSaber")
            {
                Console.WriteLine("Parent not LeftSaber or RightSaber");
                Destroy(this);
            }

            GameObject _saber = FindParentWithName(this.gameObject, "Saber");

            if (_saber == null)
            {
                Console.WriteLine("Saber not found");
                Destroy(this);
            }

            trail = _saber.GetComponent<XWeaponTrail>();
            if (trail != null)
            {
                ReflectionUtil.SetPrivateField(trail, "PointStart", PointStart);
                ReflectionUtil.SetPrivateField(trail, "PointEnd", PointEnd);
                ReflectionUtil.SetPrivateField(trail, "MyColor", TrailColor);
                ReflectionUtil.SetPrivateField(trail, "MyMaterial", TrailMaterial);
                ReflectionUtil.SetPrivateField(trail, "MaxFrame", Length);

                XWeaponTrailRenderer trailRenderer = ReflectionUtil.GetPrivateField<XWeaponTrailRenderer>(trail, "_trailRenderer");
                MeshRenderer meshRenderer = ReflectionUtil.GetPrivateField<MeshRenderer>(trailRenderer, "_meshRenderer");
                meshRenderer.material = TrailMaterial;

                trail.UpdateHeadElem();
                trail.UpdateVertex();
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
