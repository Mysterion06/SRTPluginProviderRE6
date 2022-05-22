﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SRTPluginProviderRE6.Structs
{
    [DebuggerDisplay("{_DebuggerDisplay,nq}")]
    public struct EnemyHP
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string _DebuggerDisplay
        {
            get
            {
                if (IsTrigger)
                    return string.Format("TRIGGER", CurrentHP, MaximumHP, Percentage);
                else if (IsAlive)
                    return string.Format("{0} / {1} ({2:P1})", CurrentHP, MaximumHP, Percentage);
                else
                    return "DEAD / DEAD (0%)";
            }
        }
        public short MaximumHP { get => _maximumHP; }
        internal short _maximumHP;

        public short CurrentHP { get => _currentHP; }
        internal short _currentHP;

        public bool IsTrigger => MaximumHP > 30000 || MaximumHP == 1;
        public bool IsAlive => !IsTrigger && MaximumHP > 0 && CurrentHP > 0;
        public float Percentage => ((IsAlive) ? (float)CurrentHP / (float)MaximumHP : 0f);
    }
}
