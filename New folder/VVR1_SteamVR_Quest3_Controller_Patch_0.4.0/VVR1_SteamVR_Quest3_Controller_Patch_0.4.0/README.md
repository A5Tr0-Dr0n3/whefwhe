# VVR1 Quest 3 / SteamVR input patch 0.4.0

This replaces the empty `TBSteamVRInput` source with an implementation based on Unity's XR `InputDevice` API. It keeps the game's `OpenVR`/`SteamVR` platform path and does not force `OculusPC` or call `OVRInput`.

It reads left/right XR node poses and common XR features:
- primary/secondary face buttons -> Action1/Action2
- trigger -> PrimaryTrigger
- grip -> SecondaryTrigger
- primary 2D axis/click -> Joystick
- menu -> Options

## Important: not yet verified in-game
This is a source patch, not a prebuilt game DLL. I cannot run VVR1 with your Quest/Steam Link hardware here, so I cannot honestly guarantee runtime success. XR runtimes can expose different feature sets. The patch also does not replace the controller model renderer, so the black-circle visual may need a separate fix.

## Build/apply
1. Back up the installed `Assembly-CSharp.dll`.
2. Open the decompiled `Assembly-CSharp.csproj` on the PC with VVR1 installed.
3. Replace `TButt.InputMaps/TBSteamVRInput.cs` with the included file.
4. Ensure the project's existing references point to the installed game's `VVR_1716_Data/Managed` folder, then build.
5. Only if the build succeeds, replace the installed `Assembly-CSharp.dll` with the newly built assembly.
6. Test headset pose, each controller pose, face buttons, trigger, grip, stick, menu, then grabbing/selecting.
7. To roll back, restore the original DLL.

If compilation fails, send the exact compiler error(s); do not replace the game DLL with a failed build.
