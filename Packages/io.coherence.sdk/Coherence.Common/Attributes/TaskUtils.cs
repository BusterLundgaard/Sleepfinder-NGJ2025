// Copyright (c) coherence ApS.
// See the license file in the package root for more information.

namespace Coherence.Common
{
    using System.Threading.Tasks;

    /// <summary>
    /// Utility methods related to <see cref="Task"/>.
    /// </summary>
    public static class TaskUtils
    {
        /// <summary>
        /// Gets the TaskScheduler from the current synchronization context.
        /// <para>
        /// In Unity platforms, this is the UnitySynchronizationContext.
        /// </para>
        /// <remarks>
        /// This should be passed to <see cref="Task.ContinueWith(System.Action{Task}, TaskScheduler)"/>
        /// to have WebGL support.
        /// </remarks>
        /// </summary>
        public static readonly TaskScheduler Scheduler = TaskScheduler.FromCurrentSynchronizationContext();
    }
}
