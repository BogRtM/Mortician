using Skillstates.Morris;
using System.Collections.Generic;
using System;

namespace Morris.Modules
{
    public static class States
    {
        internal static void RegisterStates()
        {
            #region Morris
            Modules.Content.AddEntityState(typeof(SkillTemplate));
            /*
            Modules.Content.AddEntityState(typeof(MorrisMainState));
            Modules.Content.AddEntityState(typeof(TriggerTap));

            Modules.Content.AddEntityState(typeof(GunSling));
            
            Modules.Content.AddEntityState(typeof(ShootingStar));

            Modules.Content.AddEntityState(typeof(CometDash));
            Modules.Content.AddEntityState(typeof(CometBounce));

            Modules.Content.AddEntityState(typeof(BulletHeaven));
            Modules.Content.AddEntityState(typeof(BulletHeavenExit));
            Modules.Content.AddEntityState(typeof(BulletHeavenLoop));
            */
            #endregion
        }
    }
}