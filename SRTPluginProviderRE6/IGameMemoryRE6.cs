using System;
using SRTPluginProviderRE6.Structs;
using SRTPluginProviderRE6.Structs.GameStructs;

namespace SRTPluginProviderRE6
{
    public interface IGameMemoryRE6
    {
        // Gamename
        string GameName { get; }

        // Player
        GamePlayer Player { get; set; }
        GamePlayer2 Player2 { get; set; }
        byte PlayerID { get; set; }
        GameStats Stats { get; set; }
        EnemyHP[] EnemyHealth { get; set; }

        // Versioninfo
        string VersionInfo { get; }

        // GameInfo
        string GameInfo { get; set; }
    }
}