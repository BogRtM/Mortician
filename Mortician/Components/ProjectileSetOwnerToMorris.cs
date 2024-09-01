using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using RoR2;
using RoR2.Projectile;

namespace Morris.Components
{
    internal class ProjectileSetOwnerToMorris : MonoBehaviour
    {
        ProjectileController projectileController;
        ProjectileSimple simple;

        private void Start()
        {
            projectileController = base.GetComponent<ProjectileController>();

            GameObject owner = projectileController.owner;
            MorrisMinionController minionController = owner.GetComponent<MorrisMinionController>();
            if(minionController && minionController.owner)
            {
                projectileController.owner = minionController.owner;
            }
        }
    }
}
