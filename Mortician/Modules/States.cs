using System.Collections.Generic;
using System;
using SkillStates.Morris;
using SkillStates.Ghoul;
using SkillStates.SharedStates;
using SkillStates.Tombstone;

namespace Morris.Modules
{
    public static class States
    {
        internal static void RegisterStates()
        {
            #region Morris
            Modules.Content.AddEntityState(typeof(SwingShovel));
            Modules.Content.AddEntityState(typeof(SpawnGhoul));
            Modules.Content.AddEntityState(typeof(Sacrifice));
            Modules.Content.AddEntityState(typeof(PlaceTombstone));
            #endregion

            #region Shared
            Modules.Content.AddEntityState(typeof(BaseLaunchedState));
            #endregion

            #region Ghoul
            Modules.Content.AddEntityState(typeof(GhoulSpawnState));
            Modules.Content.AddEntityState(typeof(GhoulMelee));
            Modules.Content.AddEntityState(typeof(BileSpit));
            Modules.Content.AddEntityState(typeof(GhoulLaunchedState));
            Modules.Content.AddEntityState(typeof(ClingState));
            Modules.Content.AddEntityState(typeof(GhoulDeathState));
            #endregion

            #region Tombstone
            Modules.Content.AddEntityState(typeof(TombstoneMainState));
            Modules.Content.AddEntityState(typeof(TombstoneLaunchedState));
            #endregion
        }
    }
}