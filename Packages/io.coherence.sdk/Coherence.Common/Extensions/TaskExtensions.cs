// Copyright (c) coherence ApS.
// See the license file in the package root for more information.

namespace Coherence.Common
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Extension methods for <see cref="Task"/>.
    /// </summary>
    internal static class TaskExtensions
    {
        /// <summary>
        /// Creates a continuation that executes asynchronously when the target Task completes.
        /// <remarks>
        /// Same as using <see cref="Task.ContinueWith(Action{Task}, CancellationToken)"/>, except that it uses the task scheduler
        /// from the current synchronization context (to have WebGL support), and <see cref="UnityEngine.Application.exitCancellationToken"/>
        /// as the cancellation token by default.
        /// </remarks>>
        /// </summary>
        /// <param name="task"> Task whose completion to wait for. </param>
        /// <param name="action"> Delegate to execute when the task completes. </param>
        /// <param name="cancellationToken"> Token for canceling the continuation action. </param>
        public static void Then([DisallowNull] this Task task, [DisallowNull] Action action, CancellationToken cancellationToken = default)
        {
#if UNITY && UNITY_6000_0_OR_NEWER
            if (cancellationToken == CancellationToken.None)
            {
                cancellationToken = UnityEngine.Application.exitCancellationToken;
            }
#endif

            task.ContinueWith(_ => action(), cancellationToken, TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
        }

        /// <summary>
        /// Creates a continuation that executes asynchronously when the target Task completes.
        /// <remarks>
        /// Same as using <see cref="Task.ContinueWith(Action{Task}, CancellationToken)"/>, except that it uses the task scheduler
        /// from the current synchronization context (to have WebGL support), and <see cref="UnityEngine.Application.exitCancellationToken"/>
        /// as the cancellation token by default.
        /// </remarks>>
        /// </summary>
        /// <param name="task"> Task whose completion to wait for. </param>
        /// <param name="action"> Delegate to execute when the task completes. </param>
        /// <param name="taskContinuationOptions"> Options for when the continuation is scheduled and how it behaves. </param>
        /// <param name="cancellationToken"> Token for canceling the continuation action. </param>
        public static void Then([DisallowNull] this Task task, [DisallowNull] Action action, TaskContinuationOptions taskContinuationOptions, CancellationToken cancellationToken = default)
        {
#if UNITY && UNITY_6000_0_OR_NEWER
            if (cancellationToken == CancellationToken.None)
            {
                cancellationToken = UnityEngine.Application.exitCancellationToken;
            }
#endif

            task.ContinueWith(_ => action(), cancellationToken, taskContinuationOptions, TaskScheduler.FromCurrentSynchronizationContext());
        }

        /// <summary>
        /// Creates a continuation that executes asynchronously when the target Task completes.
        /// <remarks>
        /// Same as using <see cref="Task.ContinueWith(Action{Task}, CancellationToken)"/>, except that it uses the task scheduler
        /// from the current synchronization context (to have WebGL support), and <see cref="UnityEngine.Application.exitCancellationToken"/>
        /// as the cancellation token by default.
        /// </remarks>>
        /// </summary>
        /// <param name="task"> Task whose completion to wait for. </param>
        /// <param name="action"> Delegate to execute when the task completes. </param>
        /// <param name="cancellationToken"> Token for canceling the continuation action. </param>
        public static void Then([DisallowNull] this Task task, [DisallowNull] Action<Task> action, CancellationToken cancellationToken = default)
        {
#if UNITY && UNITY_6000_0_OR_NEWER
            if (cancellationToken == CancellationToken.None)
            {
                cancellationToken = UnityEngine.Application.exitCancellationToken;
            }
#endif

            task.ContinueWith(action, cancellationToken, TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
        }

        /// <summary>
        /// Creates a continuation that executes asynchronously when the target Task completes.
        /// <remarks>
        /// Same as using <see cref="Task.ContinueWith(Action{Task}, CancellationToken)"/>, except that it uses the task scheduler
        /// from the current synchronization context (to have WebGL support), and <see cref="UnityEngine.Application.exitCancellationToken"/>
        /// as the cancellation token by default.
        /// </remarks>>
        /// </summary>
        /// <param name="task"> Task whose completion to wait for. </param>
        /// <param name="action"> Delegate to execute when the task completes. </param>
        /// <param name="taskContinuationOptions"> Options for when the continuation is scheduled and how it behaves. </param>
        /// <param name="cancellationToken"> Token for canceling the continuation action. </param>
        public static void Then([DisallowNull] this Task task, [DisallowNull] Action<Task> action, TaskContinuationOptions taskContinuationOptions, CancellationToken cancellationToken = default)
        {
#if UNITY && UNITY_6000_0_OR_NEWER
            if (cancellationToken == CancellationToken.None)
            {
                cancellationToken = UnityEngine.Application.exitCancellationToken;
            }
#endif

            task.ContinueWith(action, cancellationToken, taskContinuationOptions, TaskScheduler.FromCurrentSynchronizationContext());
        }

        /// <summary>
        /// Creates a continuation that executes asynchronously when the target Task completes.
        /// <remarks>
        /// Same as using <see cref="Task.ContinueWith(Action{Task}, CancellationToken)"/>, except that it uses the task scheduler
        /// from the current synchronization context (to have WebGL support), and <see cref="UnityEngine.Application.exitCancellationToken"/>
        /// as the cancellation token by default.
        /// </remarks>>
        /// </summary>
        /// <param name="task"> Task whose completion to wait for. </param>
        /// <param name="action"> Delegate to execute when the task completes. </param>
        /// <param name="cancellationToken"> Token for canceling the continuation action. </param>
        public static void Then<TResult>([DisallowNull] this Task<TResult> task, [DisallowNull] Action<Task<TResult>> action, CancellationToken cancellationToken = default)
        {
#if UNITY && UNITY_6000_0_OR_NEWER
            if (cancellationToken == CancellationToken.None)
            {
                cancellationToken = UnityEngine.Application.exitCancellationToken;
            }
#endif

            task.ContinueWith(action, cancellationToken, TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
        }

        /// <summary>
        /// Creates a continuation that executes asynchronously when the target Task completes.
        /// <remarks>
        /// Same as using <see cref="Task.ContinueWith(Action{Task}, CancellationToken)"/>, except that it uses the task scheduler
        /// from the current synchronization context (to have WebGL support), and <see cref="UnityEngine.Application.exitCancellationToken"/>
        /// as the cancellation token by default.
        /// </remarks>>
        /// </summary>
        /// <param name="task"> Task whose completion to wait for. </param>
        /// <param name="action"> Delegate to execute when the task completes. </param>
        /// <param name="taskContinuationOptions"> Options for when the continuation is scheduled and how it behaves. </param>
        /// <param name="cancellationToken"> Token for canceling the continuation action. </param>
        public static void Then<TResult>([DisallowNull] this Task<TResult> task, [DisallowNull] Action<Task<TResult>> action, TaskContinuationOptions taskContinuationOptions, CancellationToken cancellationToken = default)
        {
#if UNITY && UNITY_6000_0_OR_NEWER
            if (cancellationToken == CancellationToken.None)
            {
                cancellationToken = UnityEngine.Application.exitCancellationToken;
            }
#endif

            task.ContinueWith(action, cancellationToken, taskContinuationOptions, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}
