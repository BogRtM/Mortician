using Skillstates.Deputy;
using System.Collections.Generic;
using System;

namespace Deputy.Modules
{
    public static class States
    {
        internal static void RegisterStates()
        {
            #region Deputy
            Modules.Content.AddEntityState(typeof(DeputyMainState));
            Modules.Content.AddEntityState(typeof(VigorValor));
            #endregion
        }
    }
}