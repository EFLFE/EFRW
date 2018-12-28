using System;
using Partiality.Modloader;
using UnityEngine;

namespace EFRW
{
    public class EFSlugcat : PartialityMod
    {
        public override void Init()
        {
            base.Init();
            ModID = "EFRW";
        }

        public override void OnLoad()
        {
            base.OnLoad();
            var go = new GameObject("EfCat");
            go.AddComponent<EFSlugcatScript>();
            UnityEngine.Object.DontDestroyOnLoad(go);
        }
    }

    public class EFSlugcatScript : MonoBehaviour
    {
        Player player;
        Player.InputPackage input;

        public void Update()
        {
            if (player == null)
            {
                player = RustyMachine.modInstance.script.players[0];
            }
            input = player.input[0];

            if (player.animation == Player.AnimationIndex.Flip)
            {
                player.mushroomEffect = 1f;
            }
            else
            {
                player.mushroomEffect = 0f;
            }

        }
    }

}
