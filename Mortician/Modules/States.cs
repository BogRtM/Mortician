using System.Collections.Generic;
using System;
using Skillstates.Morris;
using Skillstates.Ghoul;

namespace Morris.Modules
{
    public static class States
    {
        internal static void RegisterStates()
        {
            #region Morris
            Modules.Content.AddEntityState(typeof(SwingShovel));
            Modules.Content.AddEntityState(typeof(SpawnGhoul));
            Modules.Content.AddEntityState(typeof(LanternSkillState));
            #endregion

            #region Ghoul
            Modules.Content.AddEntityState(typeof(GhoulSpawnState));
            Modules.Content.AddEntityState(typeof(GhoulMelee));
            Modules.Content.AddEntityState(typeof(BileSpit));
            Modules.Content.AddEntityState(typeof(LaunchedState));
            Modules.Content.AddEntityState(typeof(GhoulDeathState));
            #endregion
        }
    }
}