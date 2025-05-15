using System.Runtime.InteropServices;

namespace SRTPluginProviderRE6.Structs.GameStructs
{
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x250C)]

    public struct GamePlayer2
    {
        [FieldOffset(0xF10)] private short currentHP;
        [FieldOffset(0xF12)] private short maxHP;

        public short CurrentHP => currentHP;
        public short MaxHP => maxHP;
        public bool IsAlive => CurrentHP != 0 && MaxHP != 0 && CurrentHP > 0 && CurrentHP <= MaxHP;
        public float PercentageHP => IsAlive ? (float)CurrentHP / (float)MaxHP : 0f;
        public PlayerState HealthState
        {
            get =>
                !IsAlive ? PlayerState.Dead :
                PercentageHP >= 0.66f ? PlayerState.Fine :
                PercentageHP >= 0.33f ? PlayerState.Caution :
                PlayerState.Danger;
        }
        public string CurrentHealthState => HealthState.ToString();
    }
}