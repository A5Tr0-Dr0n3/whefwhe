using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using UnityEngine;
using Valve.VRRenderingPackage;

namespace TButt.InputMaps
{
    /// <summary>
    /// Legacy OpenVR input backend for the game's SteamVR platform path.
    /// It deliberately does not initialise or shut down OpenVR: Unity's OpenVR
    /// XR loader is expected to own the runtime session.
    /// </summary>
    public class TBSteamVRInput : TBSDKInputBase<TBInput.Button>
    {
        protected static TBSteamVRInput _instance;

        private readonly SteamVRHandController left = new SteamVRHandController(
            ETrackedControllerRole.LeftHand, TBInput.Controller.LHandController, false);
        private readonly SteamVRHandController right = new SteamVRHandController(
            ETrackedControllerRole.RightHand, TBInput.Controller.RHandController, true);

        private int lastPollFrame = -1;
        private bool warnedOpenVRFailure;

        public static TBSteamVRInput instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TBSteamVRInput();
                }
                return _instance;
            }
        }

        public override void Initialize()
        {
            // Set this before the base initialiser asks us for the active hand.
            TBInput.SetActiveControlType(TBInput.ControlType.HandControllers);
            base.Initialize();
            PollIfNeeded();
        }

        protected override void LoadHandControllers()
        {
            controller_LHand = left;
            controller_RHand = right;
        }

        public override void Update()
        {
            PollIfNeeded();
        }

        // TBInput.Update may run before this backend's Update delegate. Polling
        // lazily from the resolver keeps button events and pose reads current,
        // while the frame guard prevents multiple state transitions per frame.
        private void PollIfNeeded()
        {
            int frame = Time.frameCount;
            if (lastPollFrame == frame)
            {
                return;
            }

            lastPollFrame = frame;
            PollControllers();
        }

        private void PollControllers()
        {
            CVRSystem system = null;
            try
            {
                OpenVRNativeLoader.EnsureLoaded();
                // OpenVR.System obtains the existing IVRSystem interface. Do NOT
                // call OpenVR.Init() here; Unity's OpenVR loader owns Init/Shutdown.
                system = OpenVR.System;
            }
            catch (DllNotFoundException ex)
            {
                WarnOpenVRFailure("openvr_api.dll could not be loaded", ex);
            }
            catch (EntryPointNotFoundException ex)
            {
                WarnOpenVRFailure("The installed OpenVR runtime does not expose the expected API", ex);
            }
            catch (BadImageFormatException ex)
            {
                WarnOpenVRFailure("The loaded openvr_api.dll has the wrong architecture", ex);
            }
            catch (Exception ex)
            {
                WarnOpenVRFailure("Could not obtain SteamVR's existing OpenVR system interface", ex);
            }

            if (system == null)
            {
                left.Disconnect();
                right.Disconnect();
                return;
            }

            try
            {
                left.Poll(system);
                right.Poll(system);
            }
            catch (Exception ex)
            {
                // Keep the game alive if SteamVR temporarily drops/restarts.
                left.Disconnect();
                right.Disconnect();
                WarnOpenVRFailure("Reading SteamVR controller state failed", ex);
            }
        }

        private void WarnOpenVRFailure(string message, Exception ex)
        {
            if (warnedOpenVRFailure)
            {
                return;
            }
            warnedOpenVRFailure = true;
            Debug.LogWarning("[VVR1 OpenVR input] " + message + ": " + ex.GetType().Name + " - " + ex.Message);
        }

        public override TBInput.Controller GetActiveController()
        {
            PollIfNeeded();
            if (right.IsConnected)
            {
                return TBInput.Controller.RHandController;
            }
            if (left.IsConnected)
            {
                return TBInput.Controller.LHandController;
            }
            return TBInput.Controller.RHandController;
        }

        private SteamVRHandController GetHand(TBInput.Controller controller)
        {
            if (controller == TBInput.Controller.Active)
            {
                controller = GetActiveController();
            }
            return controller == TBInput.Controller.LHandController ? left : right;
        }

        public override Vector3 GetRawPosition(TBInput.Controller controller)
        {
            PollIfNeeded();
            return GetHand(controller).Position;
        }

        public override Quaternion GetRawRotation(TBInput.Controller controller)
        {
            PollIfNeeded();
            return GetHand(controller).Rotation;
        }

        public override Vector3 GetVelocity(TBInput.Controller controller)
        {
            PollIfNeeded();
            return GetHand(controller).Velocity;
        }

        public override Vector3 GetAngularVelocity(TBInput.Controller controller)
        {
            PollIfNeeded();
            return GetHand(controller).AngularVelocity;
        }

        // These Resolve* overrides belong on TBSDKInputBase<T>, not on the
        // individual TBControllerBase<T> objects. The base class calls them
        // after mapping each virtual TBInput button to its raw button definition.
        public override bool ResolveButton(TBInput.Button button, TBInput.Controller controller)
        {
            PollIfNeeded();
            return GetHand(controller).GetButton(button);
        }

        public override bool ResolveButtonDown(TBInput.Button button, TBInput.Controller controller)
        {
            PollIfNeeded();
            return GetHand(controller).GetButtonDown(button);
        }

        public override bool ResolveButtonUp(TBInput.Button button, TBInput.Controller controller)
        {
            PollIfNeeded();
            return GetHand(controller).GetButtonUp(button);
        }

        public override bool ResolveTouch(TBInput.Button button, TBInput.Controller controller)
        {
            PollIfNeeded();
            return GetHand(controller).GetTouch(button);
        }

        public override bool ResolveTouchDown(TBInput.Button button, TBInput.Controller controller)
        {
            PollIfNeeded();
            return GetHand(controller).GetTouchDown(button);
        }

        public override bool ResolveTouchUp(TBInput.Button button, TBInput.Controller controller)
        {
            PollIfNeeded();
            return GetHand(controller).GetTouchUp(button);
        }

        public override float ResolveAxis1D(TBInput.Button button, TBInput.Controller controller)
        {
            PollIfNeeded();
            return GetHand(controller).GetAxis1D(button);
        }

        public override Vector2 ResolveAxis2D(TBInput.Button button, TBInput.Controller controller)
        {
            PollIfNeeded();
            return GetHand(controller).GetAxis2D(button);
        }

        private sealed class SteamVRHandController : TBControllerBase<TBInput.Button>
        {
            private const float TriggerPressThreshold = 0.55f;

            private readonly ETrackedControllerRole role;
            private readonly bool isRightHand;

            private bool connected;
            private ulong previousPressed;
            private ulong currentPressed;
            private ulong previousTouched;
            private ulong currentTouched;
            private float previousTriggerAxis;
            private float triggerAxis;
            private Vector2 stickAxis;
            private TrackedDevicePose_t trackedPose;
            private bool poseValid;

            public bool IsConnected { get { return connected; } }

            public Vector3 Position
            {
                get
                {
                    if (!poseValid) return Vector3.zero;
                    HmdMatrix34_t m = trackedPose.mDeviceToAbsoluteTracking;
                    return new Vector3(m.m3, m.m7, -m.m11);
                }
            }

            public Quaternion Rotation
            {
                get
                {
                    if (!poseValid) return Quaternion.identity;
                    HmdMatrix34_t m = trackedPose.mDeviceToAbsoluteTracking;
                    Vector3 forward = new Vector3(-m.m2, -m.m6, m.m10);
                    Vector3 up = new Vector3(m.m1, m.m5, -m.m9);
                    if (forward.sqrMagnitude < 0.000001f || up.sqrMagnitude < 0.000001f)
                    {
                        return Quaternion.identity;
                    }
                    return Quaternion.LookRotation(forward, up);
                }
            }

            public Vector3 Velocity
            {
                get
                {
                    if (!poseValid) return Vector3.zero;
                    HmdVector3_t v = trackedPose.vVelocity;
                    return new Vector3(v.v0, v.v1, -v.v2);
                }
            }

            public Vector3 AngularVelocity
            {
                get
                {
                    if (!poseValid) return Vector3.zero;
                    HmdVector3_t v = trackedPose.vAngularVelocity;
                    return new Vector3(v.v0, v.v1, -v.v2);
                }
            }

            public SteamVRHandController(ETrackedControllerRole controllerRole,
                TBInput.Controller controllerType, bool rightHand)
            {
                role = controllerRole;
                type = controllerType;
                isRightHand = rightHand;
                name = rightHand ? "SteamVR Touch-compatible Right" : "SteamVR Touch-compatible Left";
                fileName = rightHand ? "Maps_SteamVR_TouchRight" : "Maps_SteamVR_TouchLeft";
                Initialize();
            }

            public override List<TBInput.ButtonDef<TBInput.Button>> GetDefaultDefs()
            {
                List<TBInput.ButtonDef<TBInput.Button>> defs = new List<TBInput.ButtonDef<TBInput.Button>>();

                Add(defs, TBInput.Button.Action1, isRightHand ? "A Button" : "X Button", true, false, false);
                Add(defs, TBInput.Button.Action2, isRightHand ? "B Button" : "Y Button", true, false, false);
                Add(defs, TBInput.Button.Joystick, "Thumbstick", true, false, true);
                Add(defs, TBInput.Button.PrimaryTrigger, "Index Trigger", true, true, false);
                Add(defs, TBInput.Button.SecondaryTrigger, "Hand Trigger / Grip", false, true, false);

                // Oculus Touch's left Enter/menu button is OpenVR System (bit 0).
                // The right Touch controller has no equivalent Start/Menu button.
                if (!isRightHand)
                {
                    Add(defs, TBInput.Button.Options, "Menu", false, false, false);
                }

                return defs;
            }

            private static void Add(List<TBInput.ButtonDef<TBInput.Button>> defs,
                TBInput.Button rawButton, string label, bool supportsTouch,
                bool supportsAxis1D, bool supportsAxis2D)
            {
                defs.Add(new TBInput.ButtonDef<TBInput.Button>
                {
                    rawButton = rawButton,
                    virtualButtons = new TBInput.Button[] { rawButton },
                    name = label,
                    supportsButton = true,
                    supportsTouch = supportsTouch,
                    supportsAxis1D = supportsAxis1D,
                    supportsAxis2D = supportsAxis2D
                });
            }

            protected override HandTrackingOffsets GetDefaultTrackingOffsets()
            {
                // Match the game's own Oculus Touch controller defaults.
                return new HandTrackingOffsets
                {
                    positionOffset = new Vector3(0f, -0.02f, -0.07f),
                    rotationOffset = Vector3.zero
                };
            }

            public void Poll(CVRSystem system)
            {
                uint index = system.GetTrackedDeviceIndexForControllerRole(role);
                if (index == uint.MaxValue || !system.IsTrackedDeviceConnected(index) ||
                    system.GetTrackedDeviceClass(index) != ETrackedDeviceClass.Controller)
                {
                    Disconnect();
                    return;
                }

                VRControllerState_t state = default(VRControllerState_t);
                TrackedDevicePose_t pose = default(TrackedDevicePose_t);
                bool stateOk = system.GetControllerStateWithPose(
                    ETrackingUniverseOrigin.TrackingUniverseStanding, index, ref state, ref pose);

                if (!stateOk)
                {
                    // Some runtimes expose controller state even when the
                    // combined state+pose call is temporarily unavailable.
                    state = default(VRControllerState_t);
                    stateOk = system.GetControllerState(index, ref state);
                    pose = default(TrackedDevicePose_t);
                }

                if (!stateOk)
                {
                    Disconnect();
                    return;
                }

                connected = true;
                previousPressed = currentPressed;
                previousTouched = currentTouched;
                previousTriggerAxis = triggerAxis;

                currentPressed = state.ulButtonPressed;
                currentTouched = state.ulButtonTouched;
                triggerAxis = Clamp01(state.rAxis1.x);
                stickAxis = new Vector2(state.rAxis0.x, state.rAxis0.y);

                trackedPose = pose;
                poseValid = pose.bPoseIsValid && pose.bDeviceIsConnected;
            }

            public void Disconnect()
            {
                previousPressed = currentPressed;
                previousTouched = currentTouched;
                previousTriggerAxis = triggerAxis;
                currentPressed = 0UL;
                currentTouched = 0UL;
                triggerAxis = 0f;
                stickAxis = Vector2.zero;
                trackedPose = default(TrackedDevicePose_t);
                poseValid = false;
                connected = false;
            }

            public bool GetButton(TBInput.Button button)
            {
                if (!connected) return false;
                return IsPressed(currentPressed, triggerAxis, button);
            }

            public bool GetButtonDown(TBInput.Button button)
            {
                if (!connected) return false;
                bool now = IsPressed(currentPressed, triggerAxis, button);
                bool before = IsPressed(previousPressed, previousTriggerAxis, button);
                return now && !before;
            }

            public bool GetButtonUp(TBInput.Button button)
            {
                bool now = connected && IsPressed(currentPressed, triggerAxis, button);
                bool before = IsPressed(previousPressed, previousTriggerAxis, button);
                return !now && before;
            }

            public bool GetTouch(TBInput.Button button)
            {
                if (!connected) return false;
                return IsTouched(currentTouched, button);
            }

            public bool GetTouchDown(TBInput.Button button)
            {
                if (!connected) return false;
                return IsTouched(currentTouched, button) && !IsTouched(previousTouched, button);
            }

            public bool GetTouchUp(TBInput.Button button)
            {
                bool now = connected && IsTouched(currentTouched, button);
                bool before = IsTouched(previousTouched, button);
                return !now && before;
            }

            public float GetAxis1D(TBInput.Button button)
            {
                if (!connected) return 0f;
                if (button == TBInput.Button.PrimaryTrigger) return triggerAxis;
                if (button == TBInput.Button.SecondaryTrigger)
                {
                    return HasButton(currentPressed, EVRButtonId.k_EButton_Grip) ? 1f : 0f;
                }
                return 0f;
            }

            public Vector2 GetAxis2D(TBInput.Button button)
            {
                if (!connected || button != TBInput.Button.Joystick) return Vector2.zero;
                return stickAxis;
            }

            private bool IsPressed(ulong mask, float trigger, TBInput.Button button)
            {
                switch (button)
                {
                    case TBInput.Button.Action1:
                        return HasButton(mask, EVRButtonId.k_EButton_A);
                    case TBInput.Button.Action2:
                        // SteamVR's Oculus Touch driver maps B/Y to ApplicationMenu.
                        return HasButton(mask, EVRButtonId.k_EButton_ApplicationMenu);
                    case TBInput.Button.PrimaryTrigger:
                        return HasButton(mask, EVRButtonId.k_EButton_SteamVR_Trigger) || trigger >= TriggerPressThreshold;
                    case TBInput.Button.SecondaryTrigger:
                        return HasButton(mask, EVRButtonId.k_EButton_Grip);
                    case TBInput.Button.Joystick:
                        return HasButton(mask, EVRButtonId.k_EButton_Axis0);
                    case TBInput.Button.Options:
                        return !isRightHand && HasButton(mask, EVRButtonId.k_EButton_System);
                    default:
                        return false;
                }
            }

            private bool IsTouched(ulong mask, TBInput.Button button)
            {
                switch (button)
                {
                    case TBInput.Button.Action1:
                        return HasButton(mask, EVRButtonId.k_EButton_A);
                    case TBInput.Button.Action2:
                        return HasButton(mask, EVRButtonId.k_EButton_ApplicationMenu);
                    case TBInput.Button.PrimaryTrigger:
                        return HasButton(mask, EVRButtonId.k_EButton_SteamVR_Trigger);
                    case TBInput.Button.Joystick:
                        return HasButton(mask, EVRButtonId.k_EButton_Axis0);
                    default:
                        return false;
                }
            }

            private static bool HasButton(ulong mask, EVRButtonId button)
            {
                return (mask & (1UL << (int)button)) != 0UL;
            }

            private static float Clamp01(float value)
            {
                if (value < 0f) return 0f;
                if (value > 1f) return 1f;
                return value;
            }
        }

        /// <summary>
        /// Loads SteamVR's native OpenVR client DLL when it is not already loaded.
        /// The runtime itself is still initialised exclusively by Unity's XR loader.
        /// </summary>
        private static class OpenVRNativeLoader
        {
            private static bool attempted;
            private static IntPtr loadedModule = IntPtr.Zero;

            [DllImport("kernel32.dll", EntryPoint = "GetModuleHandleW", CharSet = CharSet.Unicode, SetLastError = true)]
            private static extern IntPtr GetModuleHandle(string moduleName);

            [DllImport("kernel32.dll", EntryPoint = "LoadLibraryW", CharSet = CharSet.Unicode, SetLastError = true)]
            private static extern IntPtr LoadLibrary(string fileName);

            public static void EnsureLoaded()
            {
                if (attempted) return;
                attempted = true;

                try
                {
                    if (GetModuleHandle("openvr_api.dll") != IntPtr.Zero)
                    {
                        return;
                    }
                }
                catch
                {
                    return;
                }

                List<string> candidates = new List<string>();
                AddCandidate(candidates, Path.Combine(Application.dataPath, "Plugins", "x86_64", "openvr_api.dll"));
                AddCandidate(candidates, Path.Combine(Application.dataPath, "Plugins", "openvr_api.dll"));
                AddCandidate(candidates, Path.Combine(Application.dataPath, "..", "openvr_api.dll"));
                AddCandidate(candidates, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "openvr_api.dll"));

                string steamPath = ReadSteamPath();
                if (!String.IsNullOrEmpty(steamPath))
                {
                    AddSteamCandidates(candidates, steamPath);
                }

                string programFilesX86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
                if (!String.IsNullOrEmpty(programFilesX86))
                {
                    AddSteamCandidates(candidates, Path.Combine(programFilesX86, "Steam"));
                }

                string pathValue = Environment.GetEnvironmentVariable("PATH");
                if (!String.IsNullOrEmpty(pathValue))
                {
                    string[] pathParts = pathValue.Split(Path.PathSeparator);
                    for (int i = 0; i < pathParts.Length; i++)
                    {
                        if (!String.IsNullOrEmpty(pathParts[i]))
                        {
                            AddCandidate(candidates, Path.Combine(pathParts[i], "openvr_api.dll"));
                        }
                    }
                }

                for (int i = 0; i < candidates.Count; i++)
                {
                    try
                    {
                        if (File.Exists(candidates[i]))
                        {
                            loadedModule = LoadLibrary(candidates[i]);
                            if (loadedModule != IntPtr.Zero)
                            {
                                return;
                            }
                        }
                    }
                    catch
                    {
                        // Try the next known SteamVR installation location.
                    }
                }
            }

            private static void AddSteamCandidates(List<string> candidates, string steamPath)
            {
                AddCandidate(candidates, Path.Combine(steamPath, "openvr_api.dll"));
                AddCandidate(candidates, Path.Combine(steamPath, "steamapps", "common", "SteamVR", "bin", "win64", "openvr_api.dll"));
            }

            private static void AddCandidate(List<string> candidates, string path)
            {
                try
                {
                    string fullPath = Path.GetFullPath(path);
                    for (int i = 0; i < candidates.Count; i++)
                    {
                        if (String.Equals(candidates[i], fullPath, StringComparison.OrdinalIgnoreCase)) return;
                    }
                    candidates.Add(fullPath);
                }
                catch
                {
                }
            }

            private static string ReadSteamPath()
            {
                try
                {
                    using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Valve\Steam"))
                    {
                        if (key != null)
                        {
                            string value = Convert.ToString(key.GetValue("SteamPath"));
                            if (!String.IsNullOrEmpty(value)) return value.Replace('/', Path.DirectorySeparatorChar);
                        }
                    }
                }
                catch
                {
                }
                return null;
            }
        }
    }
}
