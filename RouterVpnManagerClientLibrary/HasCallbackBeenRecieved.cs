using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RouterVpnManagerClientLibrary
{
    public class HasCallbackBeenRecieved
    {

        public HasCallbackBeenRecieved()
        {
            SignalEvent = new ManualResetEvent(false);
            ResponseState = false;
            Task = null;
        }


        private Task<bool> Task { get; set; }

        public ManualResetEvent SignalEvent { get; set; }

        public bool ResponseState { get; set; }

        public void Wait(int timeOut)
        {
            bool? timeout = Task?.Wait(timeOut);
            if (timeout.HasValue && timeout.Value == false)
            {
                throw new TimeoutException();
            }
        }

        public void SetupAsyncSignal()
        {

            //https://stackoverflow.com/questions/18756354/wrapping-manualresetevent-as-awaitable-task
            //Most of this below is copy pasta black magic


            var tcs = new TaskCompletionSource<bool>();

            var registration = ThreadPool.RegisterWaitForSingleObject(SignalEvent, (state, timedOut) =>
            {
                var localTcs = (TaskCompletionSource<bool>)state;
                if (timedOut)
                    localTcs.TrySetCanceled();
                else
                    localTcs.TrySetResult(ResponseState);
            }, tcs, Timeout.InfiniteTimeSpan, executeOnlyOnce: true);

            tcs.Task.ContinueWith((_, state) => ((RegisteredWaitHandle)state).Unregister(null), registration, TaskScheduler.Default);
            Task = tcs.Task;
        }
    }
}
