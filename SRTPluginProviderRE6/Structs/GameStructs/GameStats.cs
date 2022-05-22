using System.Runtime.InteropServices;

namespace SRTPluginProviderRE6.Structs.GameStructs
{
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x4140)]

    public struct GameStats
    {
        [FieldOffset(0x4120)] private int dALeon;
        [FieldOffset(0x4124)] private int dAHelena;
        [FieldOffset(0x4128)] private int dAChris;
        [FieldOffset(0x412C)] private int dAPiers;
        [FieldOffset(0x4130)] private int dAJake;
        [FieldOffset(0x4134)] private int dASherry;
        [FieldOffset(0x4138)] private int dAAda;
        [FieldOffset(0x413C)] private int dAHunk;
        //[FieldOffset(0x0)] private int statusPoints;

        public int DALeon => dALeon;
        public int DAHelena => dAHelena;
        public int DAChris => dAChris;
        public int DAPiers => dAPiers;
        public int DAJake => dAJake;
        public int DASherry => dASherry;
        public int DAAda => dAAda;
        public int DAHunk => dAHunk;

        //public int StatusPoints => statusPoints;
    }
}