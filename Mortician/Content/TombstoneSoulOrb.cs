using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using RoR2;
using RoR2.Orbs;

namespace Morris.Content
{
    internal class TombstoneSoulOrb : GenericDamageOrb
    {
        public static float orbSpeed = 100f;
        public static float blastRadius = 10f;

        public override void Begin()
        {
            this.speed = orbSpeed;
            base.Begin();
        }

        public override GameObject GetOrbEffect()
        {
            return Addressables.LoadAssetAsync<GameObject>("RoR2/Junk/EliteHaunted/HauntOrbEffect.prefab").WaitForCompletion();
        }

        public override void OnArrival()
        {
            var blastEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/LunarGolem/LunarGolemTwinShotExplosion.prefab").WaitForCompletion();
            var effectData = new EffectData()
            {
                origin = base.target.transform.position,
                scale = blastRadius
            };

            EffectManager.SpawnEffect(blastEffect, effectData, true);

            if (this.attacker)
            {
                new BlastAttack
                {
                    attacker = this.attacker,
                    baseDamage = this.damageValue,
                    baseForce = 0f,
                    bonusForce = Vector3.down * 1000f,
                    crit = this.isCrit,
                    damageColorIndex = DamageColorIndex.Item,
                    damageType = DamageType.Generic,
                    falloffModel = BlastAttack.FalloffModel.None,
                    inflictor = null,
                    position = this.target.transform.position,
                    procChainMask = this.procChainMask,
                    procCoefficient = this.procCoefficient,
                    radius = blastRadius,
                    teamIndex = TeamComponent.GetObjectTeam(this.attacker)
                }.Fire();
            }
        }
    }
}
