using System;
using Partiality.Modloader;
using UnityEngine;

namespace EFRW.Src
{
    public class EFMod : PartialityMod
    {
        const float mushCap = 0.17f;

        public override void Init()
        {
            base.Init();
            ModID = "EFMod";
            author = "EFLFE";
        }

        public override void OnLoad()
        {
            base.OnLoad();
            On.Player.Update += Player_Update;
            On.Player.MovementUpdate += Player_MovementUpdate;
        }

        private void Player_MovementUpdate(On.Player.orig_MovementUpdate orig, Player self, bool eu)
        {


            orig(self, eu);
        }

        public void Player_Update(On.Player.orig_Update orig, Player player, bool eu)
        {
            if (player.mushroomCounter == 0)
            {
                if (player.animation == Player.AnimationIndex.Flip)
                {
                    player.mushroomEffect += (1f - player.mushroomEffect) / 4f;
                }
                else if (player.mushroomEffect > 0f)
                {
                    player.mushroomEffect -= 0.025f;
                }
            }

            orig(player, eu);
        }

    }
}
