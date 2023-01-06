using RoR2;
using R2API;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;

namespace Deputy.Components
{
    internal class ResetUtilityOnKill : MonoBehaviour, IOnKilledOtherServerReceiver
    {
        private SkillLocator skillLocator;

        private void Start()
        {
            skillLocator = base.GetComponent<SkillLocator>();
        }

        public void OnKilledOtherServer(DamageReport damageReport)
        {
            if (damageReport.damageInfo.HasModdedDamageType(DeputyPlugin.resetUtilityOnKill))
            {
                skillLocator.utility.RestockSteplike();
            }
        }
    }
}
