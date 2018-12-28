using System;
using MonoMod.ModInterop;
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
            On.Player.Update += HookedUpdate;
        }

        public void HookedUpdate(On.Player.orig_Update orig, Player player, bool eu)
        {
            //Debug.Log("player.mushroomEffect: " + player.mushroomEffect.ToString());

            if (player.animation == Player.AnimationIndex.Flip)
            {
                player.mushroomEffect = mushCap;
            }
            else if (player.mushroomEffect > 0f)
            {
                player.mushroomEffect -= 0.025f;
            }

            orig(player, eu);
        }

    }
}
