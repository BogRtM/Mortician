using Skillstates.Morris;
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
            Modules.Content.AddEntityState(typeof(SkillTemplate));
            Modules.Content.AddEntityState(typeof(SwingShovel));
            Modules.Content.AddEntityState(typeof(SpawnGhoul));
            #endregion

            #region Ghoul
            Modules.Content.AddEntityState(typeof(GhoulMelee));
            #endregion
        }
    }
}