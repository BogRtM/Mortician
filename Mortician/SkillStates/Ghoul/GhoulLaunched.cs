using UnityEngine;
using RoR2;
using EntityStates;
using SkillStates.SharedStates;
using RoR2.CharacterAI;
using EntityStates.ParentEgg;
using Morris.Modules;

namespace SkillStates.Ghoul
{
    internal class GhoulLaunched : BaseLaunchedState
    {
        private Transform launchTrail;
        private Transform launchRings;

        private bool willCling;

        public override void OnEnter()
        {
            launchPower = 85f;

            impactVFX = Assets.OmniImpactVFXGhoul;

            base.OnEnter();

            gameObject.layer = LayerIndex.debris.intVal;
            characterMotor.Motor.RebuildCollidableLayers();

            launchTrail = base.FindModelChild("LaunchTrail");
            launchTrail.gameObject.SetActive(true);

            launchRings = base.FindModelChild("LaunchRings");
            launchRings.gameObject.SetActive(true);
        }

        public override void OnHitLargeEnemy(HurtBox target)
        {
            if (base.isAuthority)
            {
                willCling = true;

                ClingState nextState = new ClingState()
                {
                    initialTarget = target,
                    //targetHealthComponent = target.healthComponent
                };

                this.outer.SetNextState(nextState);
            }
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
            if (!willCling)
            {
                gameObject.layer = LayerIndex.fakeActor.intVal;
                characterMotor.Motor.RebuildCollidableLayers();
            }

            launchTrail.gameObject.SetActive(false);
            launchRings.gameObject.SetActive(false);

            if (base.isAuthority)
            {
                if (base.healthComponent.alive)
                {
                    GameObject masterObject = base.characterBody.masterObject;
                    BaseAI baseAI = masterObject.GetComponent<BaseAI>();

                    if (baseAI)
                    {
                        baseAI.currentEnemy.Reset();
                        baseAI.ForceAcquireNearestEnemyIfNoCurrentEnemy();
                    }
                }
            }

            base.OnExit();
        }
    }
}
