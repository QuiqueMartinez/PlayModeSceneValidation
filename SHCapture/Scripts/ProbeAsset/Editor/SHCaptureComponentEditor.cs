using UnityEditor;
using UnityEngine;

namespace SHCapture
{
    // Allows capturing Spherical Harmonics from the main camera and saving it to a ScriptableObject
    [CustomEditor(typeof(SHCaptureComponent))]
    public class SHCaptureComponentEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Capture SH from Main Camera"))
            {
                var component = (SHCaptureComponent)target;
                Camera cam = Camera.main;
                if (!cam)
                {
                    Debug.LogError("Main Camera not found");
                    return;
                }

                var sh = SHCapturieUtility.GetSHFromCamera(cam);

                if (component.outputAsset == null)
                {
                    string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
                    string path = $"Assets/SH_{sceneName}.asset";
                    component.outputAsset = ScriptableObject.CreateInstance<SHProbeAsset>();
                    AssetDatabase.CreateAsset(component.outputAsset, path);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }

                component.outputAsset.data = sh;
                EditorUtility.SetDirty(component.outputAsset);
                Debug.Log("SH capture saved to ScriptableObject.");
            }
        }
    }
}
