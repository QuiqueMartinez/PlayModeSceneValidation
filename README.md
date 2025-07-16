# Visual Scene Verification with Spherical Harmonics in Unity

This project provides a lightweight, automated method for detecting visual regressions in Unity scenes using **Spherical Harmonics (SH)** as a low-frequency signature of the environment.

## 🔍 What It Does

- Captures a **cubemap** from a specified camera position.
- Projects the cubemap onto the **Spherical Harmonics (L2)** basis.
- Stores the result in a **ScriptableObject** as a reference.
- Provides **Play Mode tests** that re-capture and compare the SH projection against the reference.
- Logs per-coefficient deltas and RMSE per SH degree (`L1`, `L2`) to a `.csv` file.

This method is robust to minor pixel-level variations and highly sensitive to meaningful structural or lighting changes.

## 💡 Why Use SH?

Spherical Harmonics are solutions to **Laplace’s equation** on the sphere and form an orthonormal basis for representing directional functions. In this context, SH provides:

- A compact representation of the global appearance from a point in space.
- Sensitivity to changes in large-scale geometry, lighting, occlusion, or object presence.
- A basis for consistent **scene fingerprinting** across builds, branches, or platforms.

## 🛠️ Usage

1. **Install the scripts** in a Unity project.
2. Attach the `SHCaptureComponent` to a GameObject in your scene.
3. Press the “Capture SH from Main Camera” button in the inspector to generate a reference `SHProbeAsset`.
4. Run the Play Mode test:
   ```csharp
   SHValidationTests.CapturaCubemapYGeneraSH()
   ```
   This will:
   - Load the target scene.
   - Capture the SH projection from the main camera.
   - Compare it against the stored reference.
   - Write a `.csv` report with all deltas.

## 📜 License

MIT License — use freely in production, research, or education.
