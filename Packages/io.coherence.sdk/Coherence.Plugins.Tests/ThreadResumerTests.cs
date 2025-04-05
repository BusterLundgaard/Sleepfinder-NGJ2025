// Copyright (c) coherence ApS.
// See the license file in the package root for more information.

namespace Coherence.Plugins.Tests
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using NativeUtils;
    using NUnit.Framework;
    using UnityEngine;
    using Debug = UnityEngine.Debug;

    public class ThreadResumerTests
    {
        [Test]
        [Description("Tests that the thread resumer can find and resume a suspended thread.")]
        public void ResumesSuspendedThreads()
        {
            if (Application.platform != RuntimePlatform.WindowsEditor)
            {
                Assert.Ignore("This test is only for Windows.");
                return;
            }

            // Arrange
            var timeout = TimeSpan.FromSeconds(10);

            var cts = new CancellationTokenSource(timeout);
            var pid = Process.GetCurrentProcess().Id;
            var settings = new ThreadResumerSettings()
            {
                Enabled = false, // Ticking it manually
                SearchIntervalMs = 10,
                WarnOnSuspension = true
            };
            var resumer = new ThreadResumer(pid, cts.Token, settings);

            var threadSetEvent = new ManualResetEventSlim(false);
            var barrier = new Barrier(2);
            ulong threadId = 0;

            var thread = new Thread(() =>
            {
                try
                {
                    threadId = InteropAPI.TRGetCurrentThreadId();
                    threadSetEvent.Set();

                    Debug.Log($"Thread started: {threadId}");
                    Assert.That(barrier.SignalAndWait(timeout));
                    Debug.Log($"Thread finished: {threadId}");
                }
                catch (Exception exception)
                {
                    Assert.Fail(exception.ToString());
                }
            });
            thread.Start();

            Assert.That(threadSetEvent.Wait(timeout));
            Assert.That(threadId, Is.Not.EqualTo(0));

            var spinWait = new SpinWait();
            while (barrier.ParticipantsRemaining != 1)
            {
                spinWait.SpinOnce();
            }

            // Act & Assert
            InteropAPI.TRSuspendThread(threadId);
            Assert.That(barrier.SignalAndWait(timeout));

            Assert.That(resumer.FindAndResumeSuspendedThreads(), Is.EqualTo(1));
            Assert.That(resumer.FindAndResumeSuspendedThreads(), Is.EqualTo(1));
            Assert.That(resumer.FindAndResumeSuspendedThreads(), Is.EqualTo(0));

            Assert.That(thread.Join(timeout));
        }
    }
}
