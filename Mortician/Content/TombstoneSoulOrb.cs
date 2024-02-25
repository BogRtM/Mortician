using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using RoR2;
using RoR2.Orbs;
using Morris.Modules;

namespace Morris.Content
{
    internal class TombstoneSoulOrb : GenericDamageOrb
    {
        public static string explodeSoundString = "Play_engi_M2_explo";
        public static float orbSpeed = 100f;
        public static float blastRadius = 10f;

        public override void Begin()
        {
            this.speed = orbSpeed;
            base.Begin();
        }

        public override GameObject GetOrbEffect()
        {
            //return Addressables.LoadAssetAsync<GameObject>("RoR2/Junk/EliteHaunted/HauntOrbEffect.prefab").WaitForCompletion();
            return Assets.SoulOrbTrailEffect;
        }

        public override void OnArrival()
        {
            var blastEffect = Assets.SoulOrbExplosion;
            var effectData = new EffectData()
            {
                origin = base.target.transform.position,
                scale = blastRadius
            };

            EffectManager.SpawnEffect(blastEffect, effectData, true);
            Util.PlaySound(explodeSoundString, target.gameObject);

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
