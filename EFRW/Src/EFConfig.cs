using OptionalUI;
using Partiality.Modloader;
using UnityEngine;

namespace EFRW.Src
{
    public sealed class EFConfig : OptionInterface
    {
        public bool SlowBackMotion;
        public bool MoveAcc;
        public bool AirInLungsX2;
        public bool BrokenTeleport;
        public bool ClearWater;
        public bool MaxCycleTime;
        //public bool UnlimitedRedCycles;
        public bool NightMode;
        public bool CheatMode;
        public bool RandomColor;

        // keys
        public string ScavengerBombKey;
        public string PuffBallKey;
        public string GiveSpearKey;
        public string GiveExplosiveSpearKey;
        public string NeedleEggKey;
        public string TeleportKey;

        public EFConfig(PartialityMod mod) : base(mod)
        { }

        public override bool Configuable() => true;

        // my UI
        public override void Initialize()
        {
            base.Initialize();

            Tabs = new OpTab[2];
            Tabs[0] = new OpTab("Basic");
            Tabs[0].AddItem(new OpLabel(new Vector2(100f, 550f), new Vector2(400f, 40f), "EFRW - EFLFE mod config v" + mod.Version, FLabelAlignment.Center, true));
            float y = 460f;

            // general features
            AddCheckBox(ref y, nameof(SlowBackMotion), true, "Slow motion on back flip.");
            AddCheckBox(ref y, nameof(MoveAcc), true, "Smooth acceleration of running speed.");
            AddCheckBox(ref y, nameof(AirInLungsX2), true, "The time under water is doubled.");
            AddCheckBox(ref y, nameof(BrokenTeleport), true, "Broken teleportation.");

            // misc
            AddCheckBox(ref y, nameof(ClearWater), false, "Being under water, make it transparent.");
            AddCheckBox(ref y, nameof(MaxCycleTime), false, "Max cycle time.");
            AddCheckBox(ref y, nameof(NightMode), false, "Night time mode.");
            AddCheckBox(ref y, nameof(CheatMode), false, "Enable cheat mode.");
            AddCheckBox(ref y, nameof(RandomColor), false, "Random slugcat color.");
#if DEBUG
            AddCheckBox(ref y, nameof(UnlimitedRedCycles), false, "Unlimited cycles for Hunter.");
#endif
            Tabs[1] = new OpTab("Key bindings");
            Tabs[1].AddItem(new OpLabel(new Vector2(100f, 550f), new Vector2(400f, 40f), "EFRW - EFLFE mod config v" + mod.Version, FLabelAlignment.Center, true));
            y = 460f;

            // OpKeyBinder
            AddTextInputkBox(ref y, nameof(TeleportKey), KeyCode.C, "Teleport key.");
            AddTextInputkBox(ref y, nameof(ScavengerBombKey), KeyCode.Q, "Give Scavenger Bomb (cheat mode).");
            AddTextInputkBox(ref y, nameof(PuffBallKey), KeyCode.W, "Give Puff Ball (cheat mode).");
            AddTextInputkBox(ref y, nameof(GiveSpearKey), KeyCode.E, "Give Spear (cheat mode).");
            AddTextInputkBox(ref y, nameof(GiveExplosiveSpearKey), KeyCode.R, "Give Explosive Spear (cheat mode).");
            AddTextInputkBox(ref y, nameof(NeedleEggKey), KeyCode.T, "Give Needle Egg (cheat mode).");

        }

        void AddCheckBox(ref float y, string key, bool defaultBool, string desc)
        {
            Tabs[0].AddItem(new OpCheckBox(new Vector2(100f, y), key, defaultBool) { description = desc });
            Tabs[0].AddItem(new OpLabel(new Vector2(100f + 32f, y - 6f), new Vector2(200f, 40f), desc, FLabelAlignment.Left));
            y -= 40f;
        }

        void AddTextInputkBox(ref float y, string key, KeyCode defKey, string desc)
        {
            Tabs[1].AddItem(new OpKeyBinder(new Vector2(100f, y), new Vector2(120, 30), mod, key, defKey.ToString(), false));
            Tabs[1].AddItem(new OpLabel(new Vector2(100f + 128f, y - 6f), new Vector2(200f, 40f), desc, FLabelAlignment.Left));
            y -= 40f;
        }

        public override void ConfigOnChange()
        {
            base.ConfigOnChange();

            string val;

            if (config.TryGetValue(nameof(SlowBackMotion), out val)) SlowBackMotion = val == "true";
            if (config.TryGetValue(nameof(MoveAcc), out val)) MoveAcc = val == "true";
            if (config.TryGetValue(nameof(AirInLungsX2), out val)) AirInLungsX2 = val == "true";
            if (config.TryGetValue(nameof(BrokenTeleport), out val)) BrokenTeleport = val == "true";
            if (config.TryGetValue(nameof(ClearWater), out val)) ClearWater = val == "true";
            if (config.TryGetValue(nameof(MaxCycleTime), out val)) MaxCycleTime = val == "true";
            //if (config.TryGetValue(nameof(UnlimitedRedCycles), out val)) UnlimitedRedCycles = val == "true";
            if (config.TryGetValue(nameof(NightMode), out val)) NightMode = val == "true";
            if (config.TryGetValue(nameof(CheatMode), out val)) CheatMode = val == "true";
            if (config.TryGetValue(nameof(RandomColor), out val)) RandomColor = val == "true";

            if (config.TryGetValue(nameof(ScavengerBombKey), out val)) ScavengerBombKey = val;
            if (config.TryGetValue(nameof(PuffBallKey), out val)) PuffBallKey = val;
            if (config.TryGetValue(nameof(GiveSpearKey), out val)) GiveSpearKey = val;
            if (config.TryGetValue(nameof(GiveExplosiveSpearKey), out val)) GiveExplosiveSpearKey = val;
            if (config.TryGetValue(nameof(NeedleEggKey), out val)) NeedleEggKey = val;
            if (config.TryGetValue(nameof(TeleportKey), out val)) TeleportKey = val;
        }

    }
}
