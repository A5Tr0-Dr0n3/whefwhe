using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace TButt.InputMaps
{
    public class TBSteamVRInput : TBSDKInputBase<TBInput.Button>
    {
        protected static TBSteamVRInput _instance;
        private readonly SteamVRHandController left = new SteamVRHandController(XRNode.LeftHand, TBInput.Controller.LHandController, false);
        private readonly SteamVRHandController right = new SteamVRHandController(XRNode.RightHand, TBInput.Controller.RHandController, true);

        public static TBSteamVRInput instance
        {
            get { if (_instance == null) _instance = new TBSteamVRInput(); return _instance; }
        }

        public override void Initialize()
        {
            base.Initialize();
            TBInput.SetActiveControlType(TBInput.ControlType.HandControllers);
            Poll();
        }

        protected override void LoadHandControllers()
        {
            controller_LHand = left;
            controller_RHand = right;
        }

        public override void Update() { Poll(); }
        private void Poll() { left.Poll(); right.Poll(); }

        public override TBInput.Controller GetActiveController()
        {
            if (right.IsConnected) return TBInput.Controller.RHandController;
            if (left.IsConnected) return TBInput.Controller.LHandController;
            return TBInput.Controller.RHandController;
        }

        private SteamVRHandController Hand(TBInput.Controller c)
        {
            if (c == TBInput.Controller.Active) c = GetActiveController();
            return c == TBInput.Controller.LHandController ? left : right;
        }

        public override Vector3 GetRawPosition(TBInput.Controller c) { return Hand(c).Position; }
        public override Quaternion GetRawRotation(TBInput.Controller c) { return Hand(c).Rotation; }
        public override Vector3 GetVelocity(TBInput.Controller c) { return Hand(c).Velocity; }
        public override Vector3 GetAngularVelocity(TBInput.Controller c) { return Hand(c).AngularVelocity; }

        private sealed class SteamVRHandController : TBControllerBase<TBInput.Button>
        {
            private readonly XRNode node;
            private readonly bool isRight;
            private InputDevice device;
            private readonly Dictionary<TBInput.Button, bool> now = new Dictionary<TBInput.Button, bool>();
            private readonly Dictionary<TBInput.Button, bool> before = new Dictionary<TBInput.Button, bool>();
            private readonly Dictionary<TBInput.Button, float> axis = new Dictionary<TBInput.Button, float>();
            private Vector2 stick;

            public bool IsConnected { get { return device.isValid; } }
            public Vector3 Position { get { return InputTracking.GetLocalPosition(node); } }
            public Quaternion Rotation { get { return InputTracking.GetLocalRotation(node); } }
            public Vector3 Velocity { get { Vector3 v; return device.isValid && device.TryGetFeatureValue(CommonUsages.deviceVelocity, out v) ? v : Vector3.zero; } }
            public Vector3 AngularVelocity { get { Vector3 v; return device.isValid && device.TryGetFeatureValue(CommonUsages.deviceAngularVelocity, out v) ? v : Vector3.zero; } }

            public SteamVRHandController(XRNode n, TBInput.Controller t, bool rightHand)
            {
                node = n; type = t; isRight = rightHand;
                name = rightHand ? "SteamVR Touch-compatible Right" : "SteamVR Touch-compatible Left";
                fileName = rightHand ? "Maps_SteamVR_TouchRight" : "Maps_SteamVR_TouchLeft";
                Initialize();
            }

            public override List<TBInput.ButtonDef<TBInput.Button>> GetDefaultDefs()
            {
                var d = new List<TBInput.ButtonDef<TBInput.Button>>();
                Add(d, TBInput.Button.Action1, "Primary face button", false, false);
                Add(d, TBInput.Button.Action2, "Secondary face button", false, false);
                Add(d, TBInput.Button.PrimaryTrigger, "Index trigger", true, false);
                Add(d, TBInput.Button.SecondaryTrigger, "Grip", true, false);
                Add(d, TBInput.Button.Joystick, "Thumbstick", false, true);
                Add(d, TBInput.Button.Options, "Menu", false, false);
                return d;
            }

            private static void Add(List<TBInput.ButtonDef<TBInput.Button>> d, TBInput.Button b, string label, bool analog, bool stickAxis)
            {
                d.Add(new TBInput.ButtonDef<TBInput.Button> {
                    rawButton = b, virtualButtons = new TBInput.Button[] { b }, name = label,
                    supportsButton = true, supportsTouch = false,
                    supportsAxis1D = analog, supportsAxis2D = stickAxis
                });
            }

            public void Poll()
            {
                if (!device.isValid) device = InputDevices.GetDeviceAtXRNode(node);
                TBInput.Button[] buttons = { TBInput.Button.Action1, TBInput.Button.Action2, TBInput.Button.PrimaryTrigger,
                    TBInput.Button.SecondaryTrigger, TBInput.Button.Joystick, TBInput.Button.Options };
                foreach (TBInput.Button b in buttons)
                {
                    bool old; now.TryGetValue(b, out old); before[b] = old;
                    now[b] = ReadButton(b);
                }
                axis[TBInput.Button.PrimaryTrigger] = ReadFloat(CommonUsages.trigger);
                axis[TBInput.Button.SecondaryTrigger] = ReadFloat(CommonUsages.grip);
                Vector2 v;
                stick = device.isValid && device.TryGetFeatureValue(CommonUsages.primary2DAxis, out v) ? v : Vector2.zero;
            }

            private bool ReadButton(TBInput.Button b)
            {
                if (!device.isValid) return false;
                bool v;
                switch (b)
                {
                    case TBInput.Button.Action1: return device.TryGetFeatureValue(CommonUsages.primaryButton, out v) && v;
                    case TBInput.Button.Action2: return device.TryGetFeatureValue(CommonUsages.secondaryButton, out v) && v;
                    case TBInput.Button.PrimaryTrigger: return device.TryGetFeatureValue(CommonUsages.triggerButton, out v) && v;
                    case TBInput.Button.SecondaryTrigger: return device.TryGetFeatureValue(CommonUsages.gripButton, out v) && v;
                    case TBInput.Button.Joystick: return device.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out v) && v;
                    case TBInput.Button.Options: return device.TryGetFeatureValue(CommonUsages.menuButton, out v) && v;
                    default: return false;
                }
            }

            private float ReadFloat(InputFeatureUsage<float> usage)
            {
                float v; return device.isValid && device.TryGetFeatureValue(usage, out v) ? v : 0f;
            }

            public override bool ResolveButton(TBInput.Button b, TBInput.Controller c) { bool v; return now.TryGetValue(b, out v) && v; }
            public override bool ResolveButtonDown(TBInput.Button b, TBInput.Controller c)
            { bool a, z; now.TryGetValue(b, out a); before.TryGetValue(b, out z); return a && !z; }
            public override bool ResolveButtonUp(TBInput.Button b, TBInput.Controller c)
            { bool a, z; now.TryGetValue(b, out a); before.TryGetValue(b, out z); return !a && z; }
            public override float ResolveAxis1D(TBInput.Button b, TBInput.Controller c) { float v; return axis.TryGetValue(b, out v) ? v : 0f; }
            public override Vector2 ResolveAxis2D(TBInput.Button b, TBInput.Controller c) { return b == TBInput.Button.Joystick ? stick : Vector2.zero; }
        }
    }
}
