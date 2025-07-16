using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace SHCapture
{
    public class SHValidationTests
    {
        const string SceneToTest = "SHCaptureTestScene";

        // Asset thresholds for SH comparison
        const float PER_COEFF_MAX_DELTA = 1.01f;
        const float RMSE_THRESHOLD_L1 = 1.0f;
        const float RMSE_THRESHOLD_L2 = 1.01f;

        [UnityTest]
        public IEnumerator SHFromCubemapIsConsistentWithTemplate()
        {
            SceneManager.LoadScene(SceneToTest);
            yield return null;
            while (!SceneManager.GetActiveScene().isLoaded)
                yield return null;

            Camera cam = Camera.main;
            Assert.IsNotNull(cam, "Main Camera not found");

            // Find SH template
            var refComponent = GameObject
                .FindObjectsByType<SHCaptureComponent>(FindObjectsSortMode.None)
                .FirstOrDefault(c => c.outputAsset != null);

            Assert.IsNotNull(refComponent, "SHCaptureComponent not found");
            Assert.IsNotNull(refComponent.outputAsset, "outputAsset is empty");

            var actualSH = SHCapturieUtility.GetSHFromCamera(cam);

            // Obtain metrics
            var templateSH = refComponent.outputAsset.data;
            SHComparer.Result result = SHComparer.CompareAndLogCsv(templateSH, actualSH);

            // Assert results against thresholds
            Assert.LessOrEqual(result.RmseL1, RMSE_THRESHOLD_L1, $"RMSE L1 ({result.RmseL1}) exceeds threshold{RMSE_THRESHOLD_L1}");
            Assert.LessOrEqual(result.RmseL2, RMSE_THRESHOLD_L2, $"RMSE L2 ({result.RmseL2}) exceeds threshold{RMSE_THRESHOLD_L2}");
            for (int i = 0; i < result.PerCoeffMaxDelta.Length; i++)
            {
                Assert.LessOrEqual(result.PerCoeffMaxDelta[i], PER_COEFF_MAX_DELTA,
                    $"Max delta in coef[{i}] ({result.PerCoeffMaxDelta[i]}) exceeds threshold {PER_COEFF_MAX_DELTA}");
            }
            yield return null;
        }
    }
}