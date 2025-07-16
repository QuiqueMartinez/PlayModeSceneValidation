using UnityEngine;
using UnityEngine.Rendering;


namespace SHCapture
{
    [CreateAssetMenu(menuName = "Lighting/SHProbeAsset")]
    public class SHProbeAsset : ScriptableObject
    {
        public SphericalHarmonicsL2 data;
    }
}