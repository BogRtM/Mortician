using RoR2;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;

namespace Deputy.Components
{
    internal class DeputyAnimatorController : MonoBehaviour
    {
        private Animator modelAnimator;
        private int combatLayerIndex;

        private AimAnimator aimAnimator;

        private CharacterBody characterBody;
        private InputBankTest inputBank;

        private void Awake()
        {
            modelAnimator = base.GetComponentInChildren<Animator>();
            combatLayerIndex = modelAnimator.GetLayerIndex("Body, Combat");
        }

        private void Start()
        {
            aimAnimator = base.GetComponentInChildren<AimAnimator>();
            characterBody = base.GetComponent<CharacterBody>();
            inputBank = base.GetComponent<InputBankTest>();
        }

        public void UpdateAnimationParams()
        {
            SetSprintYaw();
            SetMovingPitch();
        }

        private void SetSprintYaw()
        {
            if (characterBody.isSprinting && !aimAnimator.fullYaw)
            {
                Chat.AddMessage("Setting full yaw");
                    aimAnimator.fullYaw = true;
                    aimAnimator.yawRangeMin = -175f;
                    aimAnimator.yawRangeMax = 175f;
            }
            else if (!characterBody.isSprinting && aimAnimator.fullYaw)
            {
                Chat.AddMessage("Stopping full yaw");
                    aimAnimator.fullYaw = false;
                    aimAnimator.yawRangeMin = -80f;
                    aimAnimator.yawRangeMax = 80f;
            }

            if(modelAnimator.GetLayerWeight(combatLayerIndex) == 1f && characterBody.isSprinting)
            {
                modelAnimator.SetFloat("yawControl", 1f);
            }
            else
            {
                modelAnimator.SetFloat("yawControl", 0f);
            }
        }

        private void SetMovingPitch()
        {
            if (inputBank.moveVector != Vector3.zero)
            {
                modelAnimator.SetFloat("movingPitch", 1f, 0.1f, Time.deltaTime);
            }
            else
            {
                modelAnimator.SetFloat("movingPitch", 0f);
            }
        }

        public void SetCombatWeight(float weight)
        {
            modelAnimator.SetLayerWeight(combatLayerIndex, weight);
        }
    }
}
