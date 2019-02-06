using System;
using Partiality.Modloader;
using UnityEngine;

namespace EFRW.Src
{
    public class EFMod : PartialityMod
    {
        const bool EXPERIMENTAL = false;

        // back flip slow
        //const float mushCap = 0.17f;
        bool initMovementUpdate;
        float acc;
        float minAcc;
        float maxAcc;
        //FLabel label;

        FLabel introLabel;
        char[] introText;
        int introTimer;
        int introIndex;

        // teleport
        bool hasPosHook;
        Vector2[] hookPos;
        Room hookRoom;
        int tpInTime;

        bool clearWater;

        public override void Init()
        {
            base.Init();
            ModID = "EFMod";
            author = "EFLFE";
            introText = "EFMod for RainWorld (v3.4)".ToCharArray();
            //FSprite sprite = new FSprite("pixel");
            //sprite.scale
        }

        public override void OnLoad()
        {
            base.OnLoad();
            On.Player.MovementUpdate += Player_MovementUpdate;

            if (EXPERIMENTAL)
            {
                On.Water.DrawSprites += Water_DrawSprites;
            }
        }

        private void Water_DrawSprites(On.Water.orig_DrawSprites orig, Water self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            // not easy. where the alpha?
            if (clearWater)
            {
                sLeaser.sprites[1].scale = 0f;
            }
            else
                sLeaser.sprites[1].scale = 1f;

            //sLeaser.sprites[1].scale = clearWater ? 0f : 1f;
            orig(self, sLeaser, rCam, timeStacker, camPos);
        }

        private void Player_MovementUpdate(On.Player.orig_MovementUpdate orig, Player self, bool eu)
        {
            // init
            if (!initMovementUpdate)
            {
                //label = new FLabel("font", "text");
                //label.SetPosition(120, 120);
                //Futile.stage.AddChild(label);

                introLabel = new FLabel("font", "");
                introLabel.SetPosition(120, 240);
                Futile.stage.AddChild(introLabel);

                initMovementUpdate = true;
                acc = self.slugcatStats.runspeedFac;
                minAcc = acc;
                maxAcc = acc * 1.75f;

                hookPos = new Vector2[self.bodyChunks.Length];

                Debug.Log("acc: " + acc.ToString() + " min: " + minAcc.ToString() + " max: " + maxAcc.ToString());
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

            // back flip slowmotion
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
            // === ===

            // acc+
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

            // air In Lungs x2
            if (self.submerged && self.airInLungs < 0.9f && self.airInLungs > 0.006f)
            {
                self.airInLungs += 0.00105f; // +0.0021
            }

            // set
            self.slugcatStats.runspeedFac = acc;
            //label.text = "airInLungs: " + self.airInLungs.ToString() + "\nlungsFac:" + self.slugcatStats.lungsFac.ToString();

            /*
            int airPer = (int)Math.Round((self.airInLungs) * 100.0, MidpointRounding.ToEven);

            if (airPer < 50 && self.submerged)
            {
                label.text = "air " + airPer.ToString();
                label.alpha = 1f - (self.airInLungs * 2f);
            }
            else
            {
                label.text = string.Empty;
            }
            */

            clearWater = self.submerged;

            orig(self, eu);
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
