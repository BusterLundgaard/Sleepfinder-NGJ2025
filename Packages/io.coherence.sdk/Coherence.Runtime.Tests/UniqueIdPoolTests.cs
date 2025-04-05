// Copyright (c) coherence ApS.
// See the license file in the package root for more information.

namespace Coherence.Runtime.Tests
{
    using System.Collections.Generic;
    using Cloud;
    using Coherence.Tests;
    using NUnit.Framework;

    public class UniqueIdPoolTests : CoherenceTest
    {
        private static readonly string ProjectId = "proj-unittests";

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            UniqueIdPool.RemoveProjectPool(ProjectId);
        }

        [TearDown]
        public override void TearDown()
        {
            base.TearDown();

            UniqueIdPool.RemoveProjectPool(ProjectId);
        }

        [Test(Description="Get a unique id from the pool")]
        public void Get_ReturnsUniqueId()
        {
            var uniqueId = UniqueIdPool.Get(ProjectId);

            Assert.That(uniqueId, Is.Not.Null);
        }

        [Test(Description="Verifies that unique ids are recycled")]
        public void Get_AfterRelease_ReturnsSameUniqueId()
        {
            var uniqueId = UniqueIdPool.Get(ProjectId);
            UniqueIdPool.Release(ProjectId, uniqueId);

            var newUniqueId = UniqueIdPool.Get(ProjectId);

            Assert.That(uniqueId, Is.EqualTo(newUniqueId));
        }

        [Test(Description="Verifies that unique ids are recycled, and new ones are created when needed")]
        public void Get_ReturnsNewUniqueId_WhenPoolEmpty()
        {
            List<UserGuid> uniqueIds = new ();
            uniqueIds.Add(UniqueIdPool.Get(ProjectId));
            uniqueIds.Add(UniqueIdPool.Get(ProjectId));

            foreach (var uid in uniqueIds)
            {
                UniqueIdPool.Release(ProjectId, uid);
            }

            for (var i = 0; i < uniqueIds.Count; ++i)
            {
                var recycledId = UniqueIdPool.Get(ProjectId);
                Assert.That(uniqueIds, Has.Exactly(1).Matches<UserGuid>(x => x == recycledId));
            }

            var newId = UniqueIdPool.Get(ProjectId);
            Assert.That(uniqueIds, Has.None.Matches<UserGuid>(x => x == newId));
        }
    }
}
