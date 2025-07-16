using UnityEngine;
using UnityEngine.Rendering;

namespace SHCapture
{
    public static class SHComparer
    {
        public struct Result
        {
            public string Csv;
            public float RmseL1;
            public float RmseL2;
            public float[] PerCoeffMaxDelta; // size 27
        }

        public static Result CompareAndLogCsv(SphericalHarmonicsL2 a, SphericalHarmonicsL2 b)
        {
            float[] sumSq = new float[3];   // L0, L1, L2
            float[] maxDeltaByDegree = new float[3];
            float[] maxDeltaByCoeff = new float[27];
            string csv = "Channel,Degree,CoeffIndex,A,B,Delta\n";

            for (int ch = 0; ch < 3; ch++)
            {
                for (int i = 0; i < 9; i++)
                {
                    int index = ch * 9 + i;
                    float va = a[ch, i];
                    float vb = b[ch, i];
                    float delta = Mathf.Abs(va - vb);
                    maxDeltaByCoeff[index] = delta;

                    int degree = GetDegreeFromIndex(i);
                    sumSq[degree] += delta * delta;
                    maxDeltaByDegree[degree] = Mathf.Max(maxDeltaByDegree[degree], delta);

                    csv += $"{ChannelName(ch)},{degree},L{i},{va},{vb},{delta}\n";
                }
            }

            float rmseL1 = Mathf.Sqrt(sumSq[1] / (3f * 3)); // 3×L1
            float rmseL2 = Mathf.Sqrt(sumSq[2] / (3f * 5)); // 3×L2

            Debug.Log($"RMSE L1: {rmseL1}, L2: {rmseL2}");
            Debug.Log(csv);

            return new Result
            {
                Csv = csv,
                RmseL1 = rmseL1,
                RmseL2 = rmseL2,
                PerCoeffMaxDelta = maxDeltaByCoeff
            };
        }

        private static int GetDegreeFromIndex(int i) => i switch
        {
            0 => 0,
            <= 3 => 1,
            _ => 2
        };

        private static string ChannelName(int c) => c switch
        {
            0 => "R",
            1 => "G",
            2 => "B",
            _ => "?"
        };
    }
}
