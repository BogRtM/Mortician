using Skillstates.Morris;
using System.Collections.Generic;
using System;
using SkillStates.Morris;

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
        }
    }
}