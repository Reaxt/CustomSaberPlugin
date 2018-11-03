using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CustomSaber
{
    public enum ColorType
    {
        LeftSaber,
        RightSaber,
        CustomColor
    }

    public class CustomTrail : MonoBehaviour
    {

        public Transform PointStart;
        public Transform PointEnd;
        public Material TrailMaterial;
        public ColorType colorType;
        public Color TrailColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        public Color MultiplierColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        public int Length = 20;

        private void Start()
        {
        }

        public void EnableTrail(bool enable)
        {
        }

        public void SetMaterial(Material newMat)
        {
        }

        public void SetColor(Color newColor)
        {
        }

        public static GameObject FindParentWithName(GameObject childObject, string name)
        {
            return null;
        }
    }
}
