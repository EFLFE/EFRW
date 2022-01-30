using System;

namespace EFRW.Src
{
    public struct NightPalette
    {
        public int PalA;
        public int PalB; // fade
        public float Blend;

        public void GenNext()
        {
            PalA = 10; // 8?
            PalB = UnityEngine.Random.Range(7, 33);
            Blend = UnityEngine.Random.Range(0.1f, 0.5f);
        }

        public override string ToString()
        {
            return $"NP: {PalA.ToString()} {PalB.ToString()} | {Blend.ToString()}";
        }

    }
}
