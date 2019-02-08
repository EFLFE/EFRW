using System;
using System.Collections.Generic;
using OptionalUI;
using Partiality.Modloader;
using UnityEngine;

namespace EFRW.Src
{
    public sealed class EFConfig : OptionInterface
    {
        public bool SlowBackMotion { get; private set; }

        public bool MoveAcc { get; private set; }

        public bool AirInLungsX2 { get; private set; }

        public bool BrokenTeleport { get; private set; }

        public bool ClearWater { get; private set; }

        public bool MaxCycleTime { get; private set; }

        public bool UnlimitedRedCycles { get; private set; }

        public bool NightMode { get; private set; }

        public EFConfig(PartialityMod mod) : base(mod)
        { }

        public override bool Configuable() => true;

        // my UI
        public override void Initialize()
        {
            base.Initialize();

            Tabs = new OpTab[1];
            Tabs[0] = new OpTab("EFTAB");
            Tabs[0].AddItem(new OpLabel(new Vector2(100f, 550f), new Vector2(400f, 40f), "EFRW - EFLFE mod config", FLabelAlignment.Center, true));
            float y = 460f;

            // OpKeyBinder

            // general features
            AddCheckBox(ref y, nameof(SlowBackMotion), true, "Slow motion on back flip.");
            AddCheckBox(ref y, nameof(MoveAcc), true, "Smooth acceleration of running speed.");
            AddCheckBox(ref y, nameof(AirInLungsX2), true, "Second breath underwater (AirInLungs x2).");
            AddCheckBox(ref y, nameof(BrokenTeleport), true, "Broken teleportation (key C).");

            // misc
            AddCheckBox(ref y, nameof(ClearWater), false, "When under water, make it transparent.");
            AddCheckBox(ref y, nameof(MaxCycleTime), false, "Max cycle time (no random).");
            AddCheckBox(ref y, nameof(NightMode), false, "Night time mode (experimental).");
#if DEBUG
            AddCheckBox(ref y, nameof(UnlimitedRedCycles), false, "Unlimited cycles for Hunter.");
#endif
        }

        void AddCheckBox(ref float y, string key, bool defaultBool, string desc)
        {
            Tabs[0].AddItem(new OpCheckBox(new Vector2(100f, y), key, defaultBool) { description = desc });
            Tabs[0].AddItem(new OpLabel(   new Vector2(100f + 32f, y - 6f), new Vector2(200f, 40f), desc, FLabelAlignment.Left));
            y -= 40f;
        }

        public override void ConfigOnChange()
        {
            base.ConfigOnChange();

            string val;

            if (config.TryGetValue(nameof(SlowBackMotion), out val))     SlowBackMotion = val == "true";
            if (config.TryGetValue(nameof(MoveAcc), out val))            MoveAcc = val == "true";
            if (config.TryGetValue(nameof(AirInLungsX2), out val))       AirInLungsX2 = val == "true";
            if (config.TryGetValue(nameof(BrokenTeleport), out val))     BrokenTeleport = val == "true";
            if (config.TryGetValue(nameof(ClearWater), out val))         ClearWater = val == "true";
            if (config.TryGetValue(nameof(MaxCycleTime), out val))       MaxCycleTime = val == "true";
            if (config.TryGetValue(nameof(UnlimitedRedCycles), out val)) UnlimitedRedCycles = val == "true";
            if (config.TryGetValue(nameof(NightMode), out val))          NightMode = val == "true";
        }

    }
}
