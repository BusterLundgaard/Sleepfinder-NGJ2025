using System.Collections.Generic;
using Coherence.Cloud;
using Coherence.Toolkit;
using UnityEngine;

public class AutoConnectFirstWorldCoherence : MonoBehaviour
{
    public CoherenceBridge bridge;

    private CloudService cloudService;

    void Start()
    {
        cloudService = bridge.CloudService;

        JoinFirstAvailableWorld();
    }

    private async void JoinFirstAvailableWorld()
    {
        // Wait for connection to be established with the coherence Cloud, authenticated as a Guest User.
        await cloudService.WaitForCloudServiceLoginAsync(1000);

        // Fetch all available worlds from our Project
        IReadOnlyList<WorldData> availableWorlds = await cloudService.Worlds.FetchWorldsAsync();

        // Join the first available world, if one exists
        if (availableWorlds.Count > 0)
        {
            bridge.JoinWorld(availableWorlds[0]);
        }
    }

}
