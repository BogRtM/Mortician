using UnityEngine;
using RoR2;
using EntityStates;
using SkillStates.SharedStates;

namespace SkillStates.Tombstone
{
    internal class LaunchedState : BaseLaunchedState
    {
        public override void OnEnter()
        {
            base.characterMotor.muteWalkMotion = false;

            base.OnEnter();
            base.PlayAnimation("FullBody, Override", "ForwardSpin");
        }

        public override void OnExit()
        {
            base.characterMotor.muteWalkMotion = true;
            base.PlayAnimation("FullBody, Override", "BufferEmpty");
            base.OnExit();
        }
    }
}
