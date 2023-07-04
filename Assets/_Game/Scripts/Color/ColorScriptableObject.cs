using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new ColorScriptableObject", menuName = "ScriptableObjects/ColorScriptableObject", order = 1)]

public class ColorScriptableObject : ScriptableObject
{
    [SerializeField] private List<Material> materials;

    public Material GetRandomMaterrial()
    {
        return materials[Random.Range(0, materials.Count)];
    }

    public Material GetMaterial(ColorType color)
    {
        return materials[(int)color];
    }
}

public enum ColorType
{
    Red = 0,
    Green = 1,
    Blue = 2,
    Orange = 3,
    Yellow = 4,
    Gray = 5,
    Black = 6,
    Brown = 7,
    White = 8,
    Transperent = 9
}
