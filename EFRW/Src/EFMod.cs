using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using On.RWCustom;
using OptionalUI;
using Partiality.Modloader;
using UnityEngine;

namespace EFRW.Src
{
    public class EFMod : PartialityMod
    {
        static EFMod instance;
        static EFConfig config;

        bool needToInitCat;

        float acc;
        float minAcc;
        float maxAcc;

        FLabel introLabel;
        char[] introText;
        int introTimer;
        int introIndex;

        // teleport
        bool hasPosHook;
        Vector2[] hookPos;
        Room hookRoom;
        int tpInTime;

        bool slugcatUnderWater;

        Color? rndCatColor;

        NightPalette nightPalette = default(NightPalette);

        public override void Init()
        {
            instance = this;
            base.Init();
            ModID = "EFMod";
            author = "EFLFE";
            Version = "4.1";
            introText = "EFMod for RainWorld (v4.1)".ToCharArray();
        }

        public static OptionInterface LoadOI()
        {
            config = new EFConfig(instance);
            return config;
        }

        //public override void OnEnable()
        //{
        //    config.Initialize();
        //}

        public override void OnLoad()
        {
            base.OnLoad();
            nightPalette.GenNext();
            On.Player.ctor                 += Player_ctor;
            On.Player.MovementUpdate       += Player_MovementUpdate;
            On.Player.Update               += Player_Update;
            On.PlayerGraphics.SlugcatColor += PlayerGraphics_SlugcatColor;
            On.Water.DrawSprites           += Water_DrawSprites;
            On.RainCycle.ctor              += RainCycle_ctor;
            On.RoomCamera.LoadPalette      += RoomCamera_LoadPalette;

            On.SoundLoader.CheckIfFileExistsAsUnityResource += SoundLoader_CheckIfFileExistsAsUnityResource;
        }

        private bool SoundLoader_CheckIfFileExistsAsUnityResource(On.SoundLoader.orig_CheckIfFileExistsAsUnityResource orig, SoundLoader self, string name)
        {
            if (CheckIfFileExistsAsExternal(name))
            {
                // Если есть файл в "Rain World\SoundEffects\" то заставить игру загрузить от туда.
                // По умолчанию сначала вызывается CheckIfFileExistsAsUnityResource.
                Debug.Log("[EFLFE] On.SoundLoader, force external sound: " + name);
                return false;
            }

            return orig(self, name);
        }
        private bool CheckIfFileExistsAsExternal(string name)
        {
            name = RootFolderDirectory() +
                "Rain World" + // miss
                Path.DirectorySeparatorChar.ToString() +
                "SoundEffects" +
                Path.DirectorySeparatorChar.ToString() +
                name;

            if (File.Exists(name + ".wav"))
                return true;
            return File.Exists(name + "_1.wav");
        }

        public static string RootFolderDirectory()
        {
            string[] strArray = Assembly.GetExecutingAssembly().Location.Split(Path.DirectorySeparatorChar);
            string str = string.Empty;
            for (int index = 0; index < strArray.Length - 3; ++index)
                str = str + strArray[index] + (object)Path.DirectorySeparatorChar;
            return str;
        }

        private Color PlayerGraphics_SlugcatColor(On.PlayerGraphics.orig_SlugcatColor orig, int i)
        {
            if (!config.RandomColor)
            {
                return orig(i);
            }
            // red: 255, 144, 133
            // mask 191, 134, 145 | 134, 36, 46
            //return new Color(0.5254901960784314f, 0.1411764705882353f, 0.1803921568627451f);

            if (!rndCatColor.HasValue)
                rndCatColor = new Color(UnityEngine.Random.Range(0.5f, 1f), UnityEngine.Random.Range(0.5f, 1f), UnityEngine.Random.Range(0.5f, 1f));

            return rndCatColor.Value;
            //return new Color(.6f, 1f, 1f);
        }

        private void Player_Update(On.Player.orig_Update orig, Player player, bool eu)
        {
            orig(player, eu);

            if (player.dead)
                rndCatColor = null;

            if (config.CheatMode)
            {
                try
                {
                    if (Input.GetKeyDown(KeyCode.Q))
                    {
                        // ScavengerBomb
                        if (player.grasps[0] == null)
                        {
                            var imp = new AbstractPhysicalObject(player.room.world, AbstractPhysicalObject.AbstractObjectType.ScavengerBomb, null, player.coord, player.room.game.GetNewID());
                            imp.RealizeInRoom();
                            player.Grab(imp.realizedObject, 0, 0, Creature.Grasp.Shareability.CanNotShare, 1f, true, false);
                        }
                    }
                    else if (Input.GetKeyDown(KeyCode.W))
                    {
                        if (player.grasps[0] == null)
                        {
                            var imp = new AbstractConsumable(player.room.world, AbstractPhysicalObject.AbstractObjectType.PuffBall, null, player.coord, player.room.game.GetNewID(), -1, -1, null);
                            imp.RealizeInRoom();
                            player.Grab(imp.realizedObject, 0, 0, Creature.Grasp.Shareability.CanNotShare, 1f, true, false);
                        }
                    }
                    else if (Input.GetKeyDown(KeyCode.E))
                    {
                        if (player.grasps[0] == null)
                        {
                            var imp = new AbstractSpear(player.room.world, null, player.coord, player.room.game.GetNewID(), false);
                            imp.RealizeInRoom();
                            player.Grab(imp.realizedObject, 0, 0, Creature.Grasp.Shareability.CanNotShare, 1f, true, false);
                        }
                    }
                    else if (Input.GetKeyDown(KeyCode.R))
                    {
                        if (player.grasps[0] == null)
                        {
                            var imp = new AbstractSpear(player.room.world, null, player.coord, player.room.game.GetNewID(), true);
                            imp.RealizeInRoom();
                            player.Grab(imp.realizedObject, 0, 0, Creature.Grasp.Shareability.CanNotShare, 1f, true, false);
                        }
                    }
                    else if (Input.GetKeyDown(KeyCode.T))
                    {
                        if (player.grasps[0] == null)
                        {
                            var imp = new AbstractConsumable(player.room.world, AbstractPhysicalObject.AbstractObjectType.NeedleEgg, null, player.coord, player.room.game.GetNewID(), -1, -1, null);
                            imp.RealizeInRoom();
                            player.Grab(imp.realizedObject, 0, 0, Creature.Grasp.Shareability.CanNotShare, 1f, true, false);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError("player.Grab Exception: " + ex.ToString());
                    if (player != null)
                        PlaySound(player, SoundID.Zapper_Zap);
                }
            }
        }

        private void RoomCamera_LoadPalette(On.RoomCamera.orig_LoadPalette orig, RoomCamera self, int pal, ref Texture2D texture)
        {
            if (config.NightMode)
            {
                orig(self, nightPalette.PalA, ref texture);
            }
            else
            {
                orig(self, pal, ref texture);
            }
        }

        private void RainCycle_ctor(On.RainCycle.orig_ctor orig, RainCycle self, World world, float minutes)
        {
            orig(self, world, minutes);
            if (config.MaxCycleTime)
                self.cycleLength = 33600;
        }

        private void Player_ctor(On.Player.orig_ctor orig, Player self, AbstractCreature abstractCreature, World world)
        {
            orig(self, abstractCreature, world);
            needToInitCat = true; // or get NullReferenceException
            nightPalette.GenNext();
        }

        private void Water_DrawSprites(On.Water.orig_DrawSprites orig, Water self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            sLeaser.sprites[1].scale = (config.ClearWater && slugcatUnderWater) ? 0f : 1f;
            orig(self, sLeaser, rCam, timeStacker, camPos);
        }

        private void Player_MovementUpdate(On.Player.orig_MovementUpdate orig, Player player, bool eu)
        {
#if DEBUG
            // intro
            if (introIndex < introText.Length)
            {
                introLabel.text += introText[introIndex].ToString();
                introIndex++;
            }
            else if (introTimer++ == 120)
            {
                introLabel.isVisible = false;
            }
#endif
            if (needToInitCat)
            {
                needToInitCat = false;
#if DEBUG
                if (introLabel == null)
                {
                    introLabel = new FLabel("font", "");
                    introLabel.SetPosition(120, 240);
                    Futile.stage.AddChild(introLabel);
                }

                if (config.UnlimitedRedCycles && self.slugcatStats.name == SlugcatStats.Name.Red)
                {
                    self.redsIllness.cycle = 9999;
                }
#endif
                acc = player.slugcatStats.runspeedFac;
                minAcc = acc;
                maxAcc = acc * 1.75f;
                hookPos = new Vector2[player.bodyChunks.Length];
            }

            BrokenTeleportation(player);
            BackFlipSlowmotion(player);
            AccMovement(player);
            AirInLungsX2(player);

            slugcatUnderWater = player.submerged;

            orig(player, eu);
        }

        void BackFlipSlowmotion(Player self)
        {
            if (!config.SlowBackMotion)
                return;

            if (self.mushroomCounter == 0)
            {
                if (self.animation == Player.AnimationIndex.Flip)
                {
                    self.mushroomEffect += (1f - self.mushroomEffect) / 4f;
                }
                else if (self.mushroomEffect > 0f)
                {
                    self.mushroomEffect -= 0.025f;
                }
            }
        }

        void AccMovement(Player self)
        {
            if (!config.MoveAcc)
                return;

            if (self.animation == Player.AnimationIndex.None
                && self.input[0].x != 0f
                && self.input[0].y == 0f)
            {
                if (acc < maxAcc && !self.input[0].jmp)
                    acc += 0.009f;
            }
            else
            {
                acc = minAcc;
            }

            self.slugcatStats.runspeedFac = acc;
        }

        void AirInLungsX2(Player self)
        {
            if (!config.AirInLungsX2)
                return;

            if (self.submerged && self.airInLungs < 0.9f && self.airInLungs > 0.006f)
            {
                self.airInLungs += 0.00105f; // +0.0021
            }
        }

        void BrokenTeleportation(Player self)
        {
            if (!config.BrokenTeleport)
                return;

            if (hookRoom != null && hookRoom != self.room)
            {
                // auto reset in new room
                hasPosHook = false;
            }

            if (!self.dead)
            {
                /*
                if (showLabel)
                {
                    label.text = "mainBodyChunk " + self.mainBodyChunk.pos.ToString() + " | " + self.bodyChunks.Length.ToString();
                    if (hasPosHook)
                    {
                        label.text += "\nhook: " + hookPos[0].ToString();
                    }
                }
                */
                if (Input.GetKeyDown(KeyCode.C))
                {
                    if (hasPosHook && hookRoom != null && self.room == hookRoom)
                    {
                        // tp
                        tpInTime = 10;
                        PlaySound(self, SoundID.Zapper_Zap);
                    }
                    else
                    {
                        // hook
                        int i = 0;
                        foreach (BodyChunk item in self.bodyChunks)
                        {
                            hookPos[i] = item.pos;
                            i++;
                        }

                        hookRoom = self.room;
                        //self.room.AddObject()

                        // sizzle
                        PlaySound(self, SoundID.Zapper_Disrupted_LOOP);
                        hasPosHook = true;
                    }
                }

                if (tpInTime > 0)
                {
                    int i = 0;
                    foreach (BodyChunk item in self.bodyChunks)
                    {
                        //item.vel = DegToVec(UnityEngine.Random.value * 360f) * 12f;
                        item.vel = Vector2.zero;
                        item.pos = hookPos[i];
                        item.lastPos = hookPos[i];
                        item.setPos = hookPos[i];

                        i++;
                    }

                    tpInTime--;
                    if (tpInTime == 0)
                        hasPosHook = false;
                }
            }
        }

        void PlaySound(Player player, SoundID soundID)
        {
            player.room.PlaySound(
                soundId: soundID,
                chunk: player.mainBodyChunk,
                loop: false,
                vol: 1f,
                pitch: 1f,
                randomStartPosition: false);
        }

    }
}
