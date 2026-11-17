# VVR1 OpenVR / Quest Touch controller patch 1.0.1

This patch replaces `TButt.InputMaps/TBSteamVRInput.cs` with an OpenVR-backed implementation for the game's existing `VRPlatform.SteamVR` path.

## What it changes

- Removes `UnityEngine.XR.InputDevice`, `InputFeatureUsage<T>`, `InputDevices`, and `CommonUsages` usage. The installed game does not contain `UnityEngine.XRModule.dll`, so those newer XR input-device APIs are not available there.
- Uses the game's existing `Valve.VRRenderingPackage.OpenVR` wrapper and `CVRSystem` interface.
- Uses `GetTrackedDeviceIndexForControllerRole`, `GetControllerStateWithPose`, and `GetControllerState` to read the left/right controller state and tracked pose.
- Implements the `ResolveButton*`, `ResolveTouch*`, and `ResolveAxis*` overrides on `TBSteamVRInput`, where `TBSDKInputBase<T>` actually calls them. The earlier draft put these overrides on the inner controller class, which is the wrong base class.
- Maps the legacy SteamVR Oculus Touch buttons using the existing Oculus Touch defaults:
  - Left X / Right A: `k_EButton_A` (bit 7) -> `Action1`
  - Left Y / Right B: `k_EButton_ApplicationMenu` (bit 1) -> `Action2`
  - Grip: `k_EButton_Grip` (bit 2) -> `SecondaryTrigger`
  - Index trigger: axis 1 plus `k_EButton_SteamVR_Trigger` -> `PrimaryTrigger`
  - Thumbstick: axis 0 and its button/touch bit -> `Joystick`
  - Left controller menu: `k_EButton_System` (bit 0) -> `Options`
- Uses the same controller tracking offset as the game's Oculus Touch controller definitions.
- Loads `openvr_api.dll` from the game's plugin folder, Steam's registered installation, the normal SteamVR `bin\win64` folder, or the process PATH if it is not already loaded.

## Important runtime behaviour

The patch does **not** call `OpenVR.Init()` or `OpenVR.Shutdown()`. The game uses Unity's XR loader to own the runtime lifecycle. Calling a second OpenVR initialisation from this input class could conflict with the active Unity runtime.

The game's `TBCore.SetActivePlatform()` selects `TBSteamVRInput` only when `UnityEngine.XR.XRSettings.loadedDeviceName` is `OpenVR`. When it is `Oculus`, the game selects `TBOculusInput` instead. Installing this source patch does not force the game from Oculus mode into SteamVR mode. Meta Horizon Link can be the headset connection while SteamVR is active, but the game still needs to be running on its OpenVR/SteamVR XR path for this backend to be selected.

The current controller-state wrapper only exposes the legacy OpenVR button model. It does not expose SteamVR Input action sets. Grip is therefore represented as a digital press (0 or 1), not a continuous analog squeeze. Haptics are not implemented in this patch.

The controller visual/model renderer is separate from `TBSteamVRInput`; this patch does not promise to fix the black-circle controller visual by itself.

## Build on the VVR1 source project

1. Extract this ZIP somewhere **outside** the decompiled source project folder.
2. Close VVR1 and any Unity/ILSpy editing session that may hold files open.
3. Open PowerShell 5.1 and run:

   ```powershell
   Set-ExecutionPolicy -Scope Process Bypass
   .\Build-VVR1-OpenVR.ps1 -ProjectRoot "E:\A5Tr0\Documents\GitHub\whefwhe\New folder (2)"
   ```

4. The script backs up the previous `TBSteamVRInput.cs` outside the project folder, replaces the source, and runs MSBuild in Release mode.
5. On success, it copies the built `Assembly-CSharp.dll` to this patch folder's `BuildOutput` directory. It does **not** install the DLL into the game.
6. Keep the source backup and the original installed game DLL. Do not replace the game DLL unless the build succeeds and you have made a separate backup of the installed DLL.

The build expects the source project to already reference the game's installed Unity assemblies and to include the existing `Valve.VRRenderingPackage` source files. It does not add a duplicate copy of those OpenVR wrapper types.

## Build status in this delivery

Version 1.0.1 fixes .NET Framework 3.5 compatibility errors in the OpenVR DLL path discovery code: it uses nested two-argument `Path.Combine` calls and reads `ProgramFiles(x86)` from the environment instead of using the unavailable `SpecialFolder.ProgramFilesX86` enum member.

No compiled `Assembly-CSharp.dll` is included. The environment used to prepare this patch has no `dotnet`, `msbuild`, `csc`, or `mcs` executable and does not contain the user's local Windows game Managed assemblies. I checked for these compilers and attempted the available command-line checks; an actual compile could not be run in this environment. The included PowerShell script is designed to build against the user's existing Windows source project and local Unity references.
