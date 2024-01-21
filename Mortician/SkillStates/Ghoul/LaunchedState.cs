﻿using UnityEngine;
using RoR2;
using EntityStates;
using SkillStates.SharedStates;
using RoR2.CharacterAI;
using EntityStates.ParentEgg;

namespace SkillStates.Ghoul
{
    internal class LaunchedState : BaseLaunchedState
    {
        private Transform launchTrail;

        public override void OnEnter()
        {
            base.OnEnter();

            launchTrail = base.FindModelChild("LaunchTrail");
            launchTrail.gameObject.SetActive(true);
        }

        public override void OnHitLargeEnemy(HurtBox target)
        {
            ClingState nextState = new ClingState()
            {
                initialTarget = target,
                targetHealthComponent = target.healthComponent
            };
            
            this.outer.SetNextState(nextState);
        }

        public override void PlayLaunchEntry()
        {
            base.PlayAnimation("FullBody, Override", "LaunchedLoop");
        }

        public override void PlayLaunchExit()
        {
            base.PlayAnimation("FullBody, Override", "BufferEmpty");
        }

        public override void OnExit()
        {
            launchTrail.gameObject.SetActive(false);

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
