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

        private const float sprintCombatGroundYaw = 1.0f;

        private bool inCombat;
        private bool isMoving;
        private bool isSprinting;
        private bool isGrounded;

        private bool sprintYawSet;

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
            inCombat = modelAnimator.GetLayerWeight(combatLayerIndex) == 1f;
            isMoving = modelAnimator.GetBool("isMoving");
            isSprinting = modelAnimator.GetBool("isSprinting");
            isGrounded = modelAnimator.GetBool("isGrounded");

            SetYaw();
            SetPitch();
        }

        private void SetYaw()
        {

            if (isSprinting && inCombat && !sprintYawSet)
            {
                sprintYawSet = true;
                aimAnimator.giveupDuration = float.PositiveInfinity;
                aimAnimator.yawRangeMin = -175f;
                aimAnimator.yawRangeMax = 175f;
            }
            else if(sprintYawSet)
            {
                sprintYawSet = false;
                aimAnimator.giveupDuration = 3f;
                aimAnimator.yawRangeMin = -80f;
                aimAnimator.yawRangeMax = 80f;
            }

            if(isSprinting && inCombat && !isGrounded)
            {
                modelAnimator.SetFloat("yawControl", 3f);
            }
            else if(isSprinting && inCombat && isGrounded)
            {
                modelAnimator.SetFloat("yawControl", 2f);
            }
            else if(!isSprinting && inCombat)
            {
                modelAnimator.SetFloat("yawControl", 1f);
            }
            else
            {
                modelAnimator.SetFloat("yawControl", 0f);
            }
        }

        private void SetPitch()
        {
            if(isMoving && !isSprinting)
            {
                modelAnimator.SetFloat("pitchControl", 1f);
            }
        }

        public void SetCombatWeight(float weight)
        {
            modelAnimator.SetLayerWeight(combatLayerIndex, weight);
        }
    }
}
