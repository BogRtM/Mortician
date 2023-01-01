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

        private const float restYaw = 0f;
        private const float combatYaw = 1f;
        private const float combatRunYaw = 2f;
        private const float sprintCombatGroundYaw = 3f;
        private const float sprintCombatAirYaw = 4f;

        private bool inCombat;
        private bool isMoving;
        private bool isSprinting;
        private bool isGrounded;

        private bool sprintYawSet;

        private float smoothDampVelocity = 0f;

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
            else if(!isSprinting && sprintYawSet)
            {
                sprintYawSet = false;
                aimAnimator.giveupDuration = 3f;
                aimAnimator.yawRangeMin = -80f;
                aimAnimator.yawRangeMax = 80f;
            }

            if(isSprinting && inCombat && !isGrounded)
            {
                modelAnimator.SetFloat("yawControl", sprintCombatAirYaw);
            }
            else if(isSprinting && inCombat && isGrounded)
            {
                modelAnimator.SetFloat("yawControl", sprintCombatGroundYaw);
            }
            else if(!isSprinting && isMoving && inCombat)
            {
                modelAnimator.SetFloat("yawControl", combatRunYaw);
            }
            else if(!isMoving && !isSprinting && inCombat)
            {
                modelAnimator.SetFloat("yawControl", combatYaw);
            }
            else if (!inCombat)
            {
                modelAnimator.SetFloat("yawControl", restYaw);
            }
        }

        private void SetPitch()
        {
            if(isMoving && !isSprinting)
            {
                modelAnimator.SetFloat("pitchControl", 1f);
            }
        }

        public void SetCombatWeight(bool enterCombat)
        {
            if (enterCombat)
            {
                modelAnimator.SetLayerWeight(combatLayerIndex, 1f);
            }
            else
            {
                modelAnimator.SetLayerWeight(combatLayerIndex, 0f);
            }
        }
    }
}
