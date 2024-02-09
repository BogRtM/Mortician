using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using RoR2;

namespace Morris.Components
{
    internal class TombstoneLocator : MonoBehaviour
    {
        public TombstoneController activeTombstone { get; private set; }

        public void SetActiveTombstone(TombstoneController tombstone)
        {
            activeTombstone = tombstone;
        }
    }
}
