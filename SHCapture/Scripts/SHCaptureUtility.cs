using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace SHCapture
{
    public class SHCapturieUtility : ScriptableObject
    {
        public static SphericalHarmonicsL2 GetSHFromCamera(Camera camera, int resolution = 128)
        {

            // Make a copy of the camera to avoid modifying the original camera settingsç
            Camera tempCamera = CloneCamera(camera);
            Cubemap cubemap = new Cubemap(resolution, TextureFormat.RGBAHalf, false);
            tempCamera.RenderToCubemap(cubemap);
            Destroy(tempCamera.gameObject);

            var sh = new SphericalHarmonicsL2();
            var step = 1 / resolution;
            foreach (var face in faces)
            {
                // Render each face of the cubemap
                for (int i = 0; i < resolution; i++)
                {
                    for (int j = 0; j < resolution; j++)
                    {
                        // Get pixel color from the cubemap
                        Color pixelColor = cubemap.GetPixel(face.Key, i, j);
                        // Process pixelColor if needed
                        var weightedDirection = face.Value(i, j, resolution);
                        // Evita que las esquinas estén sobrerepresentadas
                        sh.AddDirectionalLight(weightedDirection.direction, pixelColor * weightedDirection.weight, weightedDirection.weight);
                    }
                }
            }
            return sh;
        }

        static Camera CloneCamera(Camera original)
        {
            var go = new GameObject("TempCamera");
            go.transform.position = original.transform.position;
            go.transform.rotation = original.transform.rotation;

            Camera copy = go.AddComponent<Camera>();

            // Copiar parámetros básicos
            copy.fieldOfView = original.fieldOfView;
            copy.nearClipPlane = original.nearClipPlane;
            copy.farClipPlane = original.farClipPlane;
            copy.clearFlags = original.clearFlags;
            copy.backgroundColor = original.backgroundColor;
            copy.cullingMask = original.cullingMask;
            copy.allowHDR = original.allowHDR;
            copy.allowMSAA = original.allowMSAA;
            copy.allowDynamicResolution = original.allowDynamicResolution;

            // Deactivate to avoid interferences
            copy.enabled = false;

            return copy;
        }

        // Dictionary to map cubemap faces to their respective direction functions.
        // Values are functions that take row, column, and size to return a direction vector and a weight.
        static readonly Dictionary<CubemapFace, Func<int, int, int, (Vector3 direction, float weight)>> faces =
            new()
        {
    { CubemapFace.PositiveX, MakeDirectionFunc((u, v) => new Vector3(1f, -v, -u)) },
    { CubemapFace.NegativeX, MakeDirectionFunc((u, v) => new Vector3(-1f, -v, u)) },
    { CubemapFace.PositiveY, MakeDirectionFunc((u, v) => new Vector3(u, 1f, v)) },
    { CubemapFace.NegativeY, MakeDirectionFunc((u, v) => new Vector3(u, -1f, -v)) },
    { CubemapFace.PositiveZ, MakeDirectionFunc((u, v) => new Vector3(u, -v, 1f)) },
    { CubemapFace.NegativeZ, MakeDirectionFunc((u, v) => new Vector3(-u, -v, -1f)) },
        };

        static Func<int, int, int, (Vector3 direction, float weight)> MakeDirectionFunc(Func<float, float, Vector3> makeVec)
        {
            return (row, col, size) =>
            {
                float u = (col + 0.5f) / size * 2f - 1f;
                float v = (row + 0.5f) / size * 2f - 1f;
                Vector3 vec = makeVec(u, v);
                // Normalize the vector and intorduce a weight to avoid corners being overrepresented
                return (vec.normalized, 1f / (vec.sqrMagnitude + 1e-6f));
            };
        }
    }
}
