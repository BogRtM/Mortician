using RoR2;
using RoR2.Projectile;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;
using HG;
using UnityEngine.UIElements;
using EntityStates.Commando.CommandoWeapon;
using System.Linq;
using Deputy.Modules;

namespace Deputy.Components
{
    [RequireComponent(typeof(ProjectileController))]
    internal class RevolverProjectileBehavior : MonoBehaviour
    {
        public static int maxShots = 6;
        public static float bulletDamage = 0.7f;
        public static float fireInterval = 0.3f;
        public static float blastDamage = 4f;
        public static float searchRadius = 30f;

        public static float lifeTime = 7f;

        private ProjectileController projectileController;
        private ProjectileDamage projectileDamage;
        private ProjectileExplosion projectileExplosion;
        private TeamFilter teamFilter;

        private GameObject owner;
        private CharacterBody ownerBody;

        private SphereSearch search;

        //internal List<HurtBox> candidates;
        internal HurtBox bestCandidate;

        private float lifeStopwatch;
        private float fireStopwatch;

        private int currentShots;

        public void Start()
        {
            projectileController = base.GetComponent<ProjectileController>();
            owner = projectileController.owner;
            ownerBody = owner.GetComponent<CharacterBody>();

            projectileDamage = base.GetComponent<ProjectileDamage>();
            projectileExplosion = base.GetComponent<ProjectileExplosion>();
            teamFilter = base.GetComponent<TeamFilter>();

            search = new SphereSearch();
        }

        public void FixedUpdate()
        {
            lifeStopwatch += Time.fixedDeltaTime;
            fireStopwatch += Time.fixedDeltaTime;

            if(fireStopwatch >= fireInterval && currentShots < maxShots)
            {
                fireStopwatch = 0f;

                DoSearch();
                if(bestCandidate && bestCandidate.healthComponent.alive)
                    FireBullet();
                /*
                if (bestCandidate && bestCandidate.healthComponent.alive)
                {
                    FireBullet();
                }
                else
                {
                    DoSearch();
                    FireBullet();
                }
                */
            }

            if(lifeStopwatch >= lifeTime || (currentShots >= maxShots && fireStopwatch >= fireInterval))
            {
                projectileExplosion.Detonate();
            }
        }

        public void FireBullet()
        {
            if (!owner || !ownerBody) return;
            if (!bestCandidate) return;

            currentShots++;

            Vector3 shootVector = (bestCandidate.transform.position - base.transform.position).normalized;

            Util.PlaySound(FireBarrage.fireBarrageSoundString, base.gameObject);

            if (Util.HasEffectiveAuthority(owner))
            {
                BulletAttack bulletAttack = new BulletAttack
                {
                    owner = owner,
                    weapon = base.gameObject,
                    origin = base.transform.position,
                    procCoefficient = 0.8f,
                    aimVector = shootVector,
                    minSpread = 0f,
                    maxSpread = 0f,
                    damage = bulletDamage * ownerBody.damage,
                    force = FirePistol2.force,
                    tracerEffectPrefab = FireBarrage.tracerEffectPrefab,
                    hitEffectPrefab = FirePistol2.hitEffectPrefab,
                    isCrit = Util.CheckRoll(ownerBody.crit, ownerBody.master),
                    radius = 2f,
                    smartCollision = true,
                    damageType = DamageType.Generic,
                    maxDistance = 80f,
                    falloffModel = BulletAttack.FalloffModel.None,
                    stopperMask = LayerIndex.entityPrecise.mask
                };

                bulletAttack.Fire();
            }
        }

        public void DoSearch()
        {
            search.origin = base.transform.position;
            search.mask = LayerIndex.entityPrecise.mask;
            search.radius = searchRadius;
            search.RefreshCandidates();
            search.FilterCandidatesByHurtBoxTeam(TeamMask.GetEnemyTeams(teamFilter.teamIndex));
            search.OrderCandidatesByDistance();
            search.FilterCandidatesByDistinctHurtBoxEntities();
            bestCandidate = search.GetHurtBoxes().FirstOrDefault<HurtBox>();
            search.ClearCandidates();
        }
    }
}
