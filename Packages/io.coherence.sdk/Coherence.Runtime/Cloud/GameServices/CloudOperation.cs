// Copyright (c) coherence ApS.
// See the license file in the package root for more information.

#if UNITY_5_3_OR_NEWER
#define UNITY
#endif

namespace Coherence.Cloud
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using Common;

    /// <summary>
    /// Represents an asynchronous operation used to communicate with coherence Cloud with a result of type
    /// <see typeparamref="TResult"/>.
    /// <remarks>
    /// <para>
    /// An <see langword="async"/> method can <see langword="await"/> the <see cref="CloudOperation{TResult, TError}"/> to wait until the operation has completed.
    /// </para>
    /// <para>
    /// Similarly, a <see cref="UnityEngine.Coroutine"/> can <see langword="yield"/> the <see cref="CloudOperation{TResult, TError}"/> to wait for it to complete.
    /// </para>
    /// <para>
    /// <see cref="ContinueWith(Action, TaskContinuationOptions)"/> can also be used to perform an action after the operation has completed.
    /// </para>
    /// </remarks>
    /// </summary>
    /// <typeparam name="TResult"> Type of object returned if the operation succeeds. </typeparam>
    /// <typeparam name="TError"> Type of object returned if the operation fails. </typeparam>
    public abstract class CloudOperation<TResult, TError> : CloudOperation<TError> where TError : CoherenceError
    {
        /// <summary>
        /// The result of the operation, if it has <see cref="CloudOperation{TError}.IsCompletedSuccessfully">
        /// completed successfully</see>; otherwise, the default value of <see cref="TResult"/>.
        /// </summary>
        [MaybeNull]
        public TResult Result
        {
            get
            {
                MarkErrorAsObserved();
                return task.IsCompletedSuccessfully ? task.Result : default;
            }
        }

        protected internal new Task<TResult> task => (Task<TResult>)base.task;

        protected CloudOperation(Task<TResult> task) : base(task) { }
        protected CloudOperation(Task<TResult> task, CancellationToken cancellationToken) : base(task, cancellationToken) { }

        /// <summary>
        /// Specify an action to perform after the operation has completed
        /// (<see cref="CloudOperation{TError}.IsCompleted"/> becomes <see langword="true"/>.
        /// </summary>
        /// <param name="action"> Reference to a function to execute when the operation has completed. </param>
        /// <param name="continuationOptions"> Options for when the continuation should be scheduled and how it behaves. </param>
        public new CloudOperation<TResult, TError> ContinueWith([DisallowNull] Action action, TaskContinuationOptions continuationOptions = TaskContinuationOptions.NotOnCanceled)
        {
            if (IsCompleted)
            {
                if (!continuationOptions.HasFlag(TaskContinuationOptions.NotOnFaulted))
                {
                    MarkErrorAsObserved();
                }

                action();
                return this;
            }

            task.ContinueWith(_ =>
            {
                if (!continuationOptions.HasFlag(TaskContinuationOptions.NotOnFaulted))
                {
                    MarkErrorAsObserved();
                }

                action();
            }, cancellationToken, continuationOptions, TaskUtils.Scheduler);
            return this;
        }

        /// <summary>
        /// Specify an action to perform if the operation completes successfully
        /// (<see cref="CloudOperation{TError}.IsCompletedSuccessfully"/> becomes <see langword="true"/>.
        /// </summary>
        /// <param name="action">
        /// Reference to a function to execute if and when the operation has completed successfully.
        /// </param>
        public CloudOperation<TResult, TError> OnSuccess([DisallowNull] Action<TResult> action)
        {
            if (IsCompletedSuccessfully)
            {
                action(Result);
                return this;
            }

            task.ContinueWith(completedTask => action(completedTask.Result), TaskContinuationOptions.OnlyOnRanToCompletion);
            return this;
        }

        /// <summary>
        /// Specify an action to perform if the operation fails (<see cref="CloudOperation{TError}.HasFailed"/> becomes
        /// <see langword="true"/>.
        /// </summary>
        /// <param name="action">
        /// Reference to a function to execute if and when the operation has failed.
        /// </param>
        public new CloudOperation<TResult, TError> OnFail([DisallowNull] Action<TError> action)
        {
            base.OnFail(action);
            return this;
        }

        public void Deconstruct(out TResult result, out TError error)
        {
            result = Result;
            error = Error;
        }

        /// <inheritdoc cref="CloudOperation{TError}.GetAwaiter()"/>
        public new TaskAwaiter<CloudOperation<TResult, TError>> GetAwaiter() => GetAwaiter(this);

        public sealed override string ToString()
        {
            MarkErrorAsObserved();

            if (!IsCompleted)
            {
                return task.Status.ToString();
            }

            if (HasFailed)
            {
                if (error is not null && error.ToString() is { Length: > 0 })
                {
                    return task.Status + ": " + error;
                }

                return task.Status.ToString();
            }

            return "Result: " + (task.Result is { } result ? ResultToString(result) : "null");
        }

        protected virtual string ResultToString([DisallowNull] TResult result) => result.ToString();

        public static implicit operator TResult(CloudOperation<TResult, TError> operation) => operation.Result;
    }

    /// <summary>
    /// Represents an asynchronous operation used to communicate with coherence Cloud.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Async methods can <see langword="await"/> to wait until the operation finishes.
    /// </para>
    /// <para>
    /// Coroutines can <see langword="yield"/> to wait until the operation finishes.
    /// </para>
    /// </remarks>
    /// <typeparam name="TError"> Type of object returned if the operation fails. </typeparam>
    public abstract class CloudOperation<TError>
#if UNITY
    : UnityEngine.CustomYieldInstruction
#endif
    where TError : CoherenceError
    {
        public TError Error
        {
            get
            {
                MarkErrorAsObserved();
                return error;
            }
        }

        /// <summary>
        /// Gets whether the operation has completed.
        /// <remarks>
        /// <see langword="true"/> if the operation <see cref="IsCompletedSuccessfully">completed successfully</see>,
        /// <see cref="HasFailed">failed</see> or was <see cref="IsCanceled">canceled</see>; otherwise, <see langword="false"/>.
        /// or was
        /// </remarks>
        /// </summary>
        public bool IsCompleted => task.IsCompleted;

        /// <summary>
        /// Gets whether the operation has completed successfully.
        /// </summary>
        public bool IsCompletedSuccessfully
        {
            get
            {
                MarkErrorAsObserved();
                return task.IsCompletedSuccessfully;
            }
        }

        /// <summary>
        /// Gets whether the operation has failed.
        /// <remarks>
        /// If <see cref="HasFailed"/> is <see langword="true"/>, then <see cref="Error"/> will be non-null.
        /// </remarks>
        /// </summary>
        public bool HasFailed
        {
            get
            {
                MarkErrorAsObserved();
                return task.IsFaulted;
            }
        }

        /// <summary>
        /// Gets whether this operation has completed execution due to being canceled.
        /// </summary>
        public bool IsCanceled
        {
            get
            {
                if (task.IsCanceled)
                {
                    MarkErrorAsObserved();
                    return true;
                }

                return false;
            }
        }

        public
#if UNITY
        override
#endif
        bool keepWaiting => !task.IsCompleted && !cancellationToken.IsCancellationRequested;

        internal TError error;
        protected readonly Task task;
        internal readonly CancellationToken cancellationToken;
        internal bool errorHasBeenObserved;

        protected CloudOperation(Task task) : this(task,
        #if UNITY && UNITY_6000_0_OR_NEWER
            UnityEngine.Application.exitCancellationToken)
        #else
            default)
        #endif
        { }

        protected CloudOperation(Task task, CancellationToken cancellationToken)
        {
            this.task = task;
            this.cancellationToken = cancellationToken;

            if (task.IsCompleted)
            {
                SetError();
            }
            else
            {
                task.GetAwaiter().OnCompleted(SetError);
            }

            void SetError()
            {
                if (task.IsFaulted && task.Exception?.InnerException is { } exception and not OperationCanceledException)
                {
                    errorHasBeenObserved |= this.cancellationToken.IsCancellationRequested;
                    error = CreateError(exception);
                }
                else
                {
                    error = null;
                }
            }
        }

        /// <summary>
        /// Generates an object that can be awaited to wait for the completion of the operation.
        /// <remarks>
        /// Awaiting the operation never results in an exception being thrown. The user is excepted to either:
        /// 1. Use <see cref="OnFail"/>.
        /// 2. Manually check if <see cref="HasFailed"/> is true.
        /// 3. Manually check if <see cref="Error"/> is not null.
        /// 4. Do nothing, in which case the error will (eventually) automatically get logged to the Console.
        /// </remarks>
        /// </summary>
        /// <param name="operation"> The operation being <see langword="await">awaited</see>. </param>
        /// <typeparam name="TOperation"> Type of the operation being awaited. </typeparam>
        /// <returns>
        /// A new TaskAwaiter that completes when the operation has completed successfully,
        /// failed or been cancelled, and returns the operation itself as the result.
        /// </returns>
        private protected TaskAwaiter<TOperation> GetAwaiter<TOperation>(TOperation operation) where TOperation : CloudOperation<TError>
        {
            var taskCompletionSource = new TaskCompletionSource<TOperation>(operation.cancellationToken);
            operation.task.ContinueWith(SetResult, default, TaskContinuationOptions.ExecuteSynchronously, TaskUtils.Scheduler);
            return taskCompletionSource.Task.GetAwaiter();

            void SetResult(Task completedTask)
            {
                // NOTE: Intentionally not using SetException, even if the task is faulted, so that awaiting the
                // operation never results in an exception being thrown.
                taskCompletionSource.SetResult(operation);
            }
        }

        /// <summary>
        /// Gets an object that can be used to <see langword="await"/> for this operation to complete.
        /// <remarks>
        /// Awaiting the result of this method never causes an exception to be thrown. To handle errors you can either:
        /// <list type="number">
        /// <item>
        /// <description>
        /// Check if <see cref="HasFailed"/> is <see langword="true"/> and then examine the <see cref="Error"/>
        /// property for more details about what went wrong.
        /// </description>
        /// </item>
        /// <item>
        /// <description>
        /// Use the <see cref="OnFail"/> method to specify an action to perform if the operation fails.
        /// </description>
        /// </item>
        /// </list>
        /// </remarks>
        /// </summary>
        /// <returns> A new task awaiter instance. </returns>
        public TaskAwaiter<CloudOperation<TError>> GetAwaiter() => GetAwaiter(this);

        /// <summary>
        /// Specify an action to perform after the operation has completed (<see cref="IsCompleted"/> is
        /// <see langword="true"/>.
        /// </summary>
        /// <param name="action"> Reference to a function to execute when the operation has completed. </param>
        /// <param name="continuationOptions"> Options for when the continuation should be scheduled and how it behaves. </param>
        public CloudOperation<TError> ContinueWith([DisallowNull] Action action, TaskContinuationOptions continuationOptions = TaskContinuationOptions.NotOnCanceled)
        {
            if (IsCompleted)
            {
                if (!continuationOptions.HasFlag(TaskContinuationOptions.NotOnFaulted))
                {
                    MarkErrorAsObserved();
                }

                action();
                return this;
            }

            task.ContinueWith(_ =>
            {
                if (!continuationOptions.HasFlag(TaskContinuationOptions.NotOnFaulted))
                {
                    MarkErrorAsObserved();
                }

                action();
            }, continuationOptions);
            return this;
        }

        /// <summary>
        /// Specify an action to perform if the operation completes successfully (<see cref="IsCompletedSuccessfully"/>
        /// is <see langword="true"/>.
        /// </summary>
        /// <param name="action">
        /// Reference to a function to execute if and when the operation has completed successfully.
        /// </param>
        public CloudOperation<TError> OnSuccess([DisallowNull] Action action)
        {
            if (IsCompletedSuccessfully)
            {
                action();
                return this;
            }

            task.ContinueWith(_ => action(), TaskContinuationOptions.OnlyOnRanToCompletion);
            return this;
        }

        /// <summary>
        /// Specify an action to perform if the operation fails (<see cref="HasFailed"/> is <see langword="true"/>.
        /// </summary>
        /// <param name="action">
        /// Reference to a function to execute if and when the operation has failed.
        /// </param>
        public CloudOperation<TError> OnFail([DisallowNull] Action<TError> action)
        {
            MarkErrorAsObserved();

            if (HasFailed)
            {
                action(error);
                return this;
            }

            task.ContinueWith(failedTask =>
            {
                if (error is null)
                {
                    errorHasBeenObserved = true;
                    error = CreateError(failedTask.Exception);
                }
                else
                {
                    error.Ignore();
                }

                action(error);
            }, TaskContinuationOptions.OnlyOnFaulted);
            return this;
        }

        [return: MaybeNull]
        internal abstract TError CreateError([AllowNull] Exception exception, object args = null);

        internal void MarkErrorAsObserved()
        {
            errorHasBeenObserved = true;

            if (error is not null)
            {
                error.Ignore();
                return;
            }

            if (task.IsFaulted)
            {
                error = CreateError(task.Exception);
            }
        }

        public override string ToString()
        {
            if (error is not null && error.ToString() is { Length: > 0 })
            {
                return task.Status + ": " + error;
            }

            return task.Status.ToString();
        }

        public static implicit operator Exception(CloudOperation<TError> operation) => operation.task.Exception;
        public static implicit operator Task(CloudOperation<TError> operation) => operation.task;
    }
}
