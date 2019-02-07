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

            AddCheckBox(ref y, nameof(SlowBackMotion), true, "Slow motion on back flip.");
            AddCheckBox(ref y, nameof(MoveAcc), true, "Smooth acceleration of running speed.");
            AddCheckBox(ref y, nameof(AirInLungsX2), true, "Second breath underwater (AirInLungs x2).");
            AddCheckBox(ref y, nameof(BrokenTeleport), true, "Broken teleportation (key C).");
            AddCheckBox(ref y, nameof(ClearWater), false, "When under water, make it transparent.");
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

            SlowBackMotion = config[nameof(SlowBackMotion)] == "true";
            MoveAcc        = config[nameof(MoveAcc)]        == "true";
            AirInLungsX2   = config[nameof(AirInLungsX2)]   == "true";
            BrokenTeleport = config[nameof(BrokenTeleport)] == "true";
            ClearWater     = config[nameof(ClearWater)]     == "true";
        }

    }
}
