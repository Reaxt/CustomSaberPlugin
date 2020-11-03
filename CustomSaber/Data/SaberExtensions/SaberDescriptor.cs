﻿using UnityEngine;

// Class has to be in this namespace due to compatibility
[AddComponentMenu("Custom Sabers/Saber Descriptor")]
public class SaberDescriptor : MonoBehaviour
{
    public string SaberName = "saber";
    public string AuthorName = "author";
    public string Description = string.Empty;
    public Sprite CoverImage = null;
}
