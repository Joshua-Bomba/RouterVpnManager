using System;
using System.Drawing;
using System.Threading.Tasks;
using CoreGraphics;
using Foundation;
using RouterVpnManagerClientLibrary;
using UIKit;

namespace RouterVpnManagerClient
{
    public class UIThreadHook
    {
        
        public const int UI_LOCK_TIMEOUT = -1;

        //private static UIThreadHook instance_;

        //public UIThreadHook Instance
        //{
        //    get
        //    {
        //        if (instance_ == null)
        //        {
        //            instance_ = new UIThreadHook();
        //        }

        //        return instance_;
        //    }
        //}


        //private bool inMethod;
        //private UIThreadHook()
        //{
        //    inMethod = false;
        //}

        //public void Run(Action<UIThreadHook> task)
        //{
        //    if (inMethod)
        //    {
        //        inMethod = true;
        //        Task.Run(() =>
        //        {
        //            HookOntoGuiThead(() =>
        //            {
        //                task(this);
        //            });
        //        });
        //    }
            
        //}


        public static void HookOntoGuiThead(Action action)
        {

            HasCallbackBeenRecieved sync = new HasCallbackBeenRecieved();

            NSRunLoop.Main.BeginInvokeOnMainThread(() =>
            {
                action();
                sync.SignalEvent.Set();
            });
            sync.SetupAsyncSignal();
            sync.Wait(UI_LOCK_TIMEOUT);
        }
    }
}