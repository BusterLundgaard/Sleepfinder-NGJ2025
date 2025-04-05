// Copyright (c) coherence ApS.
// See the license file in the package root for more information.

namespace Coherence.Runtime.Tests
{
    using System.Threading.Tasks;
    using Cloud;
    using Coherence.Tests;
    using Editor.ReplicationServer;
    using NUnit.Framework;
    using Toolkit.ReplicationServer;
    using UnityEngine;
    using UnityEngine.TestTools;

    [TestFixture, UnityPlatform(RuntimePlatform.WindowsEditor)]
    public class ReplicationServerUtilsTests : CoherenceTest
    {
        private static readonly ReplicationServerConfig replicationServerConfig = EditorLauncher.CreateLocalRoomsConfig();
        private static IReplicationServer replicationServer;

        private static TestCaseData[] testCases =
        {
            new TestCaseData("localhost", replicationServerConfig.APIPort, true)
                .SetName("Valid Port and Host")
                .SetDescription($"localhost:{replicationServerConfig.APIPort}"),
            new TestCaseData("localhost", 9999, false)
                .SetName("Invalid Port")
                .SetDescription("localhost:9999"),
            new TestCaseData("192.168.0.0", replicationServerConfig.APIPort, false)
                .SetName("Invalid Host")
                .SetDescription($"foobar:{replicationServerConfig.APIPort}"),
            new TestCaseData("192.168.0.0", 9999, false)
                .SetName("Invalid Port and Host")
                .SetDescription("192.168.0.0:9999"),
        };

        public override void OneTimeSetUp()
        {
            base.OneTimeSetUp();
            ReplicationServerUtils.Timeout = 1;
            replicationServer = Launcher.Create(replicationServerConfig);
            replicationServer.Start();
        }

        public override void OneTimeTearDown()
        {
            base.OneTimeTearDown();
            replicationServer.Stop();
        }

        [Test, Timeout(1500), TestCaseSource(nameof(testCases))]
        public async Task PingHttpServer_Ends(string host, int port, bool shouldSucceed)
        {
            var done = false;
            var success = false;
            ReplicationServerUtils.PingHttpServer(host, port, ok =>
            {
                done = true;
                success = ok;
            });

            while (!done)
            {
                await Task.Yield();
            }

            Assert.IsTrue(success == shouldSucceed);
        }

        [Test, Timeout(1500), TestCaseSource(nameof(testCases))]
        public async Task PingHttpServerAsync_Ends(string host, int port, bool shouldSucceed)
        {
            var success = await ReplicationServerUtils.PingHttpServerAsync(host, port);
            Assert.IsTrue(success == shouldSucceed);
        }
    }
}
