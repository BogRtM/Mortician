using UnityEngine;
using RoR2;
using EntityStates;
using SkillStates.SharedStates;

namespace SkillStates.Ghoul
{
    internal class GhoulLaunchedState : BaseLaunchedState
    {
        public override void OnHitLargeEnemy(HurtBox target)
        {
            ClingState nextState = new ClingState()
            {
                initialTarget = target,
                targetHealthComponent = target.healthComponent
            };
            
            this.outer.SetNextState(nextState);
        }
    }
}
