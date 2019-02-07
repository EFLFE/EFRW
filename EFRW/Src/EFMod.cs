using System;
using OptionalUI;
using Partiality.Modloader;
using UnityEngine;

namespace EFRW.Src
{
    public class EFMod : PartialityMod
    {
        // new features
        const bool EXPERIMENTAL = true;

        static EFMod instance;
        static EFConfig config;

        // back flip slow
        //const float mushCap = 0.17f;
        bool initMovementUpdate;
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

        public override void Init()
        {
            instance = this;
            base.Init();
            ModID = "EFMod";
            author = "EFLFE";
            Version = "4.0";
            introText = "EFMod for RainWorld (v4.0)".ToCharArray();
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
            On.Player.MovementUpdate += Player_MovementUpdate;
            On.Water.DrawSprites += Water_DrawSprites;
        }

        private void Water_DrawSprites(On.Water.orig_DrawSprites orig, Water self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            sLeaser.sprites[1].scale = (config.ClearWater && slugcatUnderWater) ? 0f : 1f;
            orig(self, sLeaser, rCam, timeStacker, camPos);
        }

        private void Player_MovementUpdate(On.Player.orig_MovementUpdate orig, Player self, bool eu)
        {
            // init
            if (!initMovementUpdate)
            {
                if (EXPERIMENTAL)
                {
                    introLabel = new FLabel("font", "");
                    introLabel.SetPosition(120, 240);
                    Futile.stage.AddChild(introLabel);
                }

                //CompletelyOptional.OptionScript
                //self.redsIllness.cycle

                acc = self.slugcatStats.runspeedFac;
                minAcc = acc;
                maxAcc = acc * 1.75f;

                hookPos = new Vector2[self.bodyChunks.Length];

                initMovementUpdate = true;
                //Debug.Log("acc: " + acc.ToString() + " min: " + minAcc.ToString() + " max: " + maxAcc.ToString());
            }

            if (EXPERIMENTAL)
            {
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
            }

            BrokenTeleportation(self);
            BackFlipSlowmotion(self);
            AccMovement(self);
            AirInLungsX2(self);

            // set data
            slugcatUnderWater = self.submerged;

            orig(self, eu);
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
