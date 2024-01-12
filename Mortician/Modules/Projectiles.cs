using R2API;
using RoR2;
using RoR2.Projectile;
using Morris.Components;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using System;

namespace Morris.Modules
{
    internal static class Projectiles
    {
        internal static GameObject ghoulBilePrefab;
        internal static void RegisterProjectiles()
        {
            CreateBileProjectile();
            AddProjectile(ghoulBilePrefab);
        }

        private static void CreateBileProjectile()
        {
            GameObject baseProjectile = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/FlyingVermin/VerminSpitProjectile.prefab").WaitForCompletion();
            ghoulBilePrefab = PrefabAPI.InstantiateClone(baseProjectile, "GhoulBilePrefab");

            ProjectileDamage projectileDamage = ghoulBilePrefab.GetComponent<ProjectileDamage>();
            projectileDamage.damageType = DamageType.BlightOnHit;
        }

        internal static void AddProjectile(GameObject projectileToAdd)
        {
            Modules.Content.AddProjectilePrefab(projectileToAdd);
        }

        private static GameObject CreateGhostPrefab(string ghostName)
        {
            GameObject ghostPrefab = Modules.Assets.mainAssetBundle.LoadAsset<GameObject>(ghostName);
            if (!ghostPrefab.GetComponent<NetworkIdentity>()) ghostPrefab.AddComponent<NetworkIdentity>();
            if (!ghostPrefab.GetComponent<ProjectileGhostController>()) ghostPrefab.AddComponent<ProjectileGhostController>();

            //Modules.Assets.ConvertAllRenderersToHopooShader(ghostPrefab);

            return ghostPrefab;
        }

        private static GameObject CloneProjectilePrefab(string prefabName, string newPrefabName)
        {
            GameObject newPrefab = PrefabAPI.InstantiateClone(RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/" + prefabName), newPrefabName);
            return newPrefab;
        }
    }
}