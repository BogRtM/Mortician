using R2API;
using RoR2;
using RoR2.Projectile;
using Deputy.Components;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using System;

namespace Deputy.Modules
{
    internal static class Projectiles
    {
        //internal static GameObject bombPrefab;
        //internal static GameObject javelinPrefab;
        internal static GameObject revolverProjectile;
        internal static void RegisterProjectiles()
        {
            CreateRevolverProjectile();
            AddProjectile(revolverProjectile);
        }

        private static void CreateRevolverProjectile()
        {
            GameObject baseProjectile = Addressables.LoadAsset<GameObject>("RoR2/Base/Toolbot/ToolbotGrenadeLauncherProjectile.prefab").WaitForCompletion();
            revolverProjectile = PrefabAPI.InstantiateClone(baseProjectile, "RevolverProjectile");

            ProjectileController projectileController = revolverProjectile.GetComponent<ProjectileController>();
            GameObject revolverGhost = CreateGhostPrefab("RevolverProjectileGhost");

            RotateObject rotateObject = revolverGhost.AddComponent<RotateObject>();
            rotateObject.rotationSpeed = new Vector3(0f, 40f, 0f);

            projectileController.ghostPrefab = revolverGhost;
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