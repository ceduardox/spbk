using UnityEngine;

/// <summary>
/// Legacy placeholder to restore serialized planet data (GUID 12a544...).
/// Keeps old scene/prefab references valid without changing gameplay flow.
/// </summary>
public sealed class PlanetLegacySettings : MonoBehaviour
{
    public bool mobileShader;
    public bool atmosphere = true;
    public bool updateChangeInRealTime = true;
    public Material PlanetMaterial;
    public Material ringMaterial;
    public float hdrExposure = 1f;
    public Color atmoColor = Color.white;
    public float atmoStrength = 10f;
}
