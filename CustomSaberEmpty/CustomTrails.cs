using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CustomSaber
{
    public class CustomTrail : MonoBehaviour
    {

        public Transform PointStart;
        public Transform PointEnd;
        public Material TrailMaterial;
        public Color TrailColor = new Color(1.0f,1.0f,1.0f,1.0f);
        public int Length = 14;

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
