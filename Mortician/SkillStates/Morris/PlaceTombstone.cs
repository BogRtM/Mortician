using UnityEngine;
using RoR2;
using EntityStates;
using Morris.Modules;
using System;
namespace SkillStates.Morris
{
    internal class PlaceTombstone : BaseState
    {
        public static GameObject tombstoneMasterPrefab;

        private const float tombstoneHeight = 7f;
        private const float tombstoneRadius = 1f;

        private float entryDelay = 0.1f;
        private float exitDelay = 0.25f;
        private bool exitPending;

        private BlueprintController blueprintController;
        private PlaceTombstone.PlacementInfo currentPlacementInfo;
        public override void OnEnter()
        {
            base.OnEnter();

            if(base.isAuthority)
            {
                currentPlacementInfo = GetPlacementInfo();
                blueprintController = UnityEngine.Object.Instantiate<GameObject>(Assets.TombstoneBlueprintsPrefab, currentPlacementInfo.position,
                    currentPlacementInfo.rotation).GetComponent<BlueprintController>();
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.isAuthority)
            {
                entryDelay -= Time.fixedDeltaTime;

                if(entryDelay <= 0f && !exitPending)
                {
                    if((base.inputBank.skill1.down || base.inputBank.skill4.justPressed) && currentPlacementInfo.ok)
                    {
                        PlayCrossfade("Gesture, Override", "LanternRaise", 0.05f);

                        base.characterBody.SendConstructTurret(base.characterBody,
                        currentPlacementInfo.position,
                        currentPlacementInfo.rotation,
                        MasterCatalog.FindMasterIndex(tombstoneMasterPrefab));

                        base.StartAimMode(2f, false);

                        base.skillLocator.special.DeductStock(1);

                        DestroyBlueprints();
                        exitPending = true;
                    }

                    if ((base.inputBank.skill2.justPressed))
                    {
                        DestroyBlueprints();
                        exitPending = true;
                    }
                }
                else if(exitPending)
                {
                    exitDelay -= Time.fixedDeltaTime;
                    if(exitDelay <= 0f)
                    {
                        this.outer.SetNextStateToMain();
                    }
                }
            }
        }

        public override void Update()
        {
            base.Update();
            this.currentPlacementInfo = this.GetPlacementInfo();
            if (this.blueprintController)
            {
                this.blueprintController.PushState(this.currentPlacementInfo.position, this.currentPlacementInfo.rotation, this.currentPlacementInfo.ok);
            }
        }

        private void DestroyBlueprints()
        {
            EntityState.Destroy(blueprintController.gameObject);
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

        private PlaceTombstone.PlacementInfo GetPlacementInfo()
        {
            Ray aimRay = base.GetAimRay();
            Vector3 direction = aimRay.direction;
            direction.y = 0f;
            direction.Normalize();
            aimRay.direction = direction;
            PlaceTombstone.PlacementInfo placementInfo = default(PlaceTombstone.PlacementInfo);
            placementInfo.ok = false;
            placementInfo.rotation = Util.QuaternionSafeLookRotation(-direction);

            Ray ray = new Ray(aimRay.GetPoint(3f) + Vector3.up * 2f, Vector3.down);
            RaycastHit raycastHit;

            float raycastDistance = 8f;
            float placementDistance = raycastDistance;
            if (Physics.SphereCast(ray, tombstoneRadius, out raycastHit, raycastDistance, LayerIndex.world.mask) && raycastHit.normal.y > 0.5f)
            {
                placementDistance = raycastHit.distance;
                placementInfo.ok = true;
            }

            Vector3 point = ray.GetPoint(placementDistance + tombstoneRadius);
            placementInfo.position = point;

            if(placementInfo.ok)
            {
                if(Physics.CheckCapsule(placementInfo.position + Vector3.up * tombstoneRadius, placementInfo.position + Vector3.up * tombstoneHeight, 
                    tombstoneRadius * 0.9f, LayerIndex.world.mask | LayerIndex.defaultLayer.mask)) 
                {
                    placementInfo.ok = false;
                }
            }

            return placementInfo;
        }

        private struct PlacementInfo
        {
            public bool ok;

            public Vector3 position;

            public Quaternion rotation;
        }
    }
}
