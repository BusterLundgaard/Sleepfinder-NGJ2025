namespace Polyfunks
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using ToyBoxHHH;

    public class PolyfunksGraphControl : MonoBehaviour
    {
        public PolyfunksGraph graph;

        public PolyfunksControl controlPrefab;
        public Transform controlSpawnParent;
        public List<PolyfunksControl> controlsSpawned = new List<PolyfunksControl>();

        public void ClearAll()
        {
            for (int i = 0; i < controlsSpawned.Count; i++)
            {
                if (controlsSpawned[i] != null)
                {
                    Destroy(controlsSpawned[i].gameObject);
                }
            }
            controlsSpawned.Clear();
        }

        [DebugButton]
        public void SpawnControls()
        {
            ClearAll();
            var controls = graph.GetControlNodes();
            foreach (var c in controls)
            {
                if (c.Exposed)
                {
                    // spawn the associated control at position.
                    // all data forthe control sohuld be serialized in the node, this way it is always possible to load it up/change it.
                    var s = Instantiate(controlPrefab, controlSpawnParent);
                    s.nodeRef = c;

                    controlsSpawned.Add(s);

                }
            }
        }

    }
}