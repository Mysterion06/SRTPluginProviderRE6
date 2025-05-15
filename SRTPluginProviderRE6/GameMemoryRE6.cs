using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Reflection;
using SRTPluginProviderRE6.Structs;
using SRTPluginProviderRE6.Structs.GameStructs;

namespace SRTPluginProviderRE6
{
    public class GameMemoryRE6 : IGameMemoryRE6
    {
        // Gamename
        public string GameName => "RE6";

        // Player 1
        public GamePlayer Player { get => _player; set => _player = value; }
        internal GamePlayer _player;

        // Player 2
        public GamePlayer2 Player2 { get => _player2; set => _player2 = value; }
        internal GamePlayer2 _player2;

        // PlayerID
        public byte PlayerID { get => _playerID; set => _playerID = value; }
        internal byte _playerID;

        // Game Stats
        public GameStats Stats { get => _stats; set => _stats = value; }
        internal GameStats _stats;

        //Player Points
        public int StatusPoints { get => _statusPoints; set => _statusPoints = value; }
        internal int _statusPoints;
        public int StatusPointsCur { get => _statusPointsCur; set => _statusPointsCur = value; }
        internal int _statusPointsCur;

        // Areas
        public short Areas { get => _areas; set => _areas = value; }
        internal short _areas;
        // Enemy HP
        public EnemyHP[] EnemyHealth { get => _enemyHealth; set => _enemyHealth = value; }
        internal EnemyHP[] _enemyHealth;

        // Versioninfo
        public string VersionInfo => FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;

        // GameInfo
        public string GameInfo { get =>_gameInfo; set => _gameInfo = value; }
        internal string _gameInfo;
    }
}
