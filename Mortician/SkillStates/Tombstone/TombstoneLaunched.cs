﻿using UnityEngine;
using RoR2;
using EntityStates;
using SkillStates.SharedStates;
using Morris.Modules;

namespace SkillStates.Tombstone
{
    internal class TombstoneLaunched : BaseLaunchedState
    {
        public override void OnEnter()
        {
            launchPower = 60f;
            damageType = DamageType.Stun1s;
            impactVFX = Assets.OmniImpactVFXTombstone;

            base.OnEnter();
            gameObject.layer = LayerIndex.fakeActor.intVal;
            characterMotor.Motor.RebuildCollidableLayers();
            base.characterMotor.muteWalkMotion = false;
        }

        public override void PlayLaunchEntry()
        {
            base.PlayAnimation("FullBody, Override", "ForwardSpin");
        }

        public override void PlayLaunchExit()
        {
            base.PlayAnimation("FullBody, Override", "BufferEmpty");
        }

        public override void OnExit()
        {
            gameObject.layer = LayerIndex.defaultLayer.intVal;
            characterMotor.Motor.RebuildCollidableLayers();

            base.characterMotor.muteWalkMotion = true;
            base.OnExit();
        }
    }
}
