using UnityEngine;
using RoR2;
using EntityStates;
using SkillStates.SharedStates;
using RoR2.CharacterAI;

namespace SkillStates.Ghoul
{
    internal class LaunchedState : BaseLaunchedState
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

        public override void OnExit()
        {
            if (base.healthComponent.alive)
            {
                GameObject masterObject = base.characterBody.masterObject;
                BaseAI baseAI = masterObject.GetComponent<BaseAI>();

                baseAI.currentEnemy.Reset();
                baseAI.ForceAcquireNearestEnemyIfNoCurrentEnemy();
            }

            base.OnExit();
        }
    }
}
