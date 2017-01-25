﻿using System.Windows.Threading;

namespace ZerO
{
    public class DispatcherHelper
    {
        private static readonly DispatcherOperationCallback ExitFrameCallback = ExitFrame;

        /// <summary>
        /// Processes all UI messages currently in the message queue.
        /// </summary>
        public static void WaitForPriority()
        {
            // Create new nested message pump.
            DispatcherFrame nestedFrame = new DispatcherFrame();

            // Dispatch a callback to the current message queue, when getting called,
            // this callback will end the nested message loop.
            // The priority of this callback should be lower than that of event message you want to process.
            DispatcherOperation exitOperation = Dispatcher.CurrentDispatcher.BeginInvoke(
                DispatcherPriority.ApplicationIdle, ExitFrameCallback, nestedFrame);

            // pump the nested message loop, the nested message loop will immediately
            // process the messages left inside the message queue.
            Dispatcher.PushFrame(nestedFrame);

            // If the "exitFrame" callback is not finished, abort it.
            if (exitOperation.Status != DispatcherOperationStatus.Completed)
            {
                exitOperation.Abort();
            }
        }

        private static object ExitFrame(object state)
        {
            DispatcherFrame frame = state as DispatcherFrame;

            // Exit the nested message loop.
            if (frame != null) frame.Continue = false;
            return null;
        }
    }
}
