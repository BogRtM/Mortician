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
            Modules.Content.AddEntityState(typeof(SpawnState));
            Modules.Content.AddEntityState(typeof(GhoulMelee));
            Modules.Content.AddEntityState(typeof(BileSpit));
            Modules.Content.AddEntityState(typeof(SkillStates.Ghoul.LaunchedState));
            Modules.Content.AddEntityState(typeof(ClingState));
            Modules.Content.AddEntityState(typeof(DeathExplosion));
            #endregion

            #region Tombstone
            Modules.Content.AddEntityState(typeof(TombstoneMain));
            Modules.Content.AddEntityState(typeof(SkillStates.Tombstone.LaunchedState));
            #endregion
        }
    }
}