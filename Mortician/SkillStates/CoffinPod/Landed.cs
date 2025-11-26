using UnityEngine;
using RoR2;
using EntityStates.SurvivorPod;
using EntityStates;
using System;

namespace SkillStates.CoffinPod
{
    internal class Landed : SurvivorPodBaseState
    {
        public override void OnEnter()
        {
            base.OnEnter();

            base.vehicleSeat.handleVehicleExitRequestServer.AddCallback(new CallbackCheck<bool, GameObject>.CallbackDelegate(this.HandleVehicleExitRequest));
        }

        private void HandleVehicleExitRequest(GameObject arg, ref bool? resultOverride)
        {
            base.survivorPodController.exitAllowed = false;
            this.outer.SetNextState(new Release());
            resultOverride = new bool?(true);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if(base.fixedAge > 0f)
            {
                base.survivorPodController.exitAllowed = true;
            }
        }

        public override void OnExit()
        {
            base.vehicleSeat.handleVehicleExitRequestServer.RemoveCallback(new CallbackCheck<bool, GameObject>.CallbackDelegate(this.HandleVehicleExitRequest));
            base.survivorPodController.exitAllowed = false;
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
