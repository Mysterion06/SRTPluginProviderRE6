using ProcessMemory;
using static ProcessMemory.Extensions;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using SRTPluginProviderRE6.Structs;
using SRTPluginProviderRE6.Structs.GameStructs;

namespace SRTPluginProviderRE6
{
    internal class GameMemoryRE6Scanner : IDisposable
    {
        private static readonly int MAX_ENTITIES = 12;

        // Variables
        private ProcessMemoryHandler memoryAccess;
        private GameMemoryRE6 gameMemoryValues;
        public bool HasScanned;
        public bool ProcessRunning => memoryAccess != null && memoryAccess.ProcessRunning;
        public int ProcessExitCode => (memoryAccess != null) ? memoryAccess.ProcessExitCode : 0;

        // Pointer Address Variables
        private int pointerAddressEnemyHP;
        private int pointerAddressDA;
        private int pointerAddressPlayerHP;
        private int pointerAddressPlayerID;

        // Pointer Classes
        private IntPtr BaseAddress { get; set; }
        private MultilevelPointer PointerPlayerHP { get; set; }
        private MultilevelPointer PointerPlayerHP2 { get; set; }
        private MultilevelPointer PointerPlayerID { get; set; }
        private MultilevelPointer PointerPlayerID2 { get; set; }
        private MultilevelPointer PointerDA { get; set; }
        private MultilevelPointer PointerPlayerState { get; set; }
        private MultilevelPointer PointerGameStats { get; set; }
        private MultilevelPointer[] PointerEnemyHP { get; set; }
        
        internal GameMemoryRE6Scanner(Process process = null)
        {
            gameMemoryValues = new GameMemoryRE6();
            if (process != null)
                Initialize(process);
        }

        internal unsafe void Initialize(Process process)
        {
            if (process == null)
                return; // Do not continue if this is null.

            SelectPointerAddresses(GameHashes.DetectVersion(process.MainModule.FileName));

            //if (!SelectPointerAddresses(GameHashes.DetectVersion(process.MainModule.FileName)))
            //    return; // Unknown version.

            int pid = GetProcessId(process).Value;
            memoryAccess = new ProcessMemoryHandler(pid);
            if (ProcessRunning)
            {
                BaseAddress = NativeWrappers.GetProcessBaseAddress(pid, PInvoke.ListModules.LIST_MODULES_64BIT); // Bypass .NET's managed solution for getting this and attempt to get this info ourselves via PInvoke since some users are getting 299 PARTIAL COPY when they seemingly shouldn't.

                // Setup the pointers.
                PointerPlayerHP = new MultilevelPointer(memoryAccess, IntPtr.Add(BaseAddress, pointerAddressPlayerHP), 0x208, 0x20C);
                PointerPlayerHP2 = new MultilevelPointer(memoryAccess, IntPtr.Add(BaseAddress, pointerAddressPlayerHP), 0x208, 0x210);
                PointerPlayerID = new MultilevelPointer(memoryAccess, IntPtr.Add(BaseAddress, pointerAddressPlayerID));
                PointerGameStats = new MultilevelPointer(memoryAccess, IntPtr.Add(BaseAddress, pointerAddressDA));

                gameMemoryValues._enemyHealth = new EnemyHP[MAX_ENTITIES];
                for (int i = 0; i < MAX_ENTITIES; ++i)
                    gameMemoryValues._enemyHealth[i] = new EnemyHP();

                GenerateEnemyEntries();
            }
        }

        private void SelectPointerAddresses(GameVersion gv)
        {
            if (gv == GameVersion.RE6_1_0_6)
            {
                gameMemoryValues._gameInfo = "Old Patch";
                pointerAddressPlayerHP = 0x14645F4;
                pointerAddressDA = 0x013C6468;
                pointerAddressEnemyHP = 0x013C540C;
                pointerAddressPlayerID = 0x13B8CF0;
            }
            else if (gv == GameVersion.RE6_1_1_0)
            {
                gameMemoryValues._gameInfo = "Latest Release";
                pointerAddressPlayerHP = 0x146E5FC;
                pointerAddressDA = 0x13D0468;
                pointerAddressEnemyHP = 0x13CF40C;
                pointerAddressPlayerID = 0x13C2CF0;
            }
            else
            {
                gameMemoryValues._gameInfo = "Unknown Release May Not Work. Contact Developers";
            }
        }

        private unsafe void GenerateEnemyEntries()
        {
            if(PointerEnemyHP == null)
            {   
                PointerEnemyHP = new MultilevelPointer[MAX_ENTITIES];
                for (int i = 0; i < MAX_ENTITIES; ++i)
                    PointerEnemyHP[i] = new MultilevelPointer(memoryAccess, IntPtr.Add(BaseAddress, pointerAddressEnemyHP), 0x50, 0x0 + (i * 0x4));
            }
        }
        
        internal void UpdatePointers()
        {
            PointerPlayerHP.UpdatePointers();
            PointerPlayerHP2.UpdatePointers();
            PointerPlayerID.UpdatePointers();
            PointerGameStats.UpdatePointers();

            GenerateEnemyEntries();
            for (int i = 0; i < MAX_ENTITIES; i++)
                PointerEnemyHP[i].UpdatePointers();
        }

        internal unsafe IGameMemoryRE6 Refresh()
        {   
            // Player 1
            gameMemoryValues._player = PointerPlayerHP.Deref<GamePlayer>(0x0);
            //gameMemoryValues._player = memoryAccess.GetAt<GamePlayer>(IntPtr.Add(BaseAddress, pointerAddressPlayerHP));
            // Player 2
            gameMemoryValues._player2 = PointerPlayerHP2.Deref<GamePlayer2>(0x0);

            // Player ID
            gameMemoryValues._playerID = PointerPlayerID.DerefByte(0x81F5A4);

            // Game Stats
            gameMemoryValues._stats = PointerGameStats.Deref<GameStats>(0x0);

            // Enemy HP
            for (int i = 0; i < gameMemoryValues._enemyHealth.Length; ++i)
            {
                try
                {
                    // Check to see if the pointer is currently valid. It can become invalid when rooms are changed.
                    if (PointerEnemyHP[i].Address != IntPtr.Zero)
                    {
                        gameMemoryValues.EnemyHealth[i]._currentHP = PointerEnemyHP[i].DerefShort(0xF10);
                        gameMemoryValues.EnemyHealth[i]._maximumHP = PointerEnemyHP[i].DerefShort(0xF12);
                    }
                    else
                    {
                        // Clear these values out so stale data isn't left behind when the pointer address is no longer value and nothing valid gets read.
                        // This happens when the game removes pointers from the table (map/room change).
                        gameMemoryValues.EnemyHealth[i]._maximumHP = 0;
                        gameMemoryValues.EnemyHealth[i]._currentHP = 0;
                    }
                }
                catch
                {
                    gameMemoryValues.EnemyHealth[i]._maximumHP = 0;
                    gameMemoryValues.EnemyHealth[i]._currentHP = 0;
                }
            }

            HasScanned = true;
            return gameMemoryValues;
        }

        private int? GetProcessId(Process process) => process?.Id;

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls


        private unsafe bool SafeReadByteArray(IntPtr address, int size, out byte[] readBytes)
        {
            readBytes = new byte[size];
            fixed (byte* p = readBytes)
            {
                return memoryAccess.TryGetByteArrayAt(address, size, p);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    if (memoryAccess != null)
                        memoryAccess.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~REmake1Memory() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}