﻿using System;
using System.Drawing;
using System.Threading.Tasks;
using CoreGraphics;
using Foundation;
using RouterVpnManagerClientLibrary;
using UIKit;

namespace RouterVpnManagerClient
{
    public class Global
    {
        private const int GUI_LOCK_TIMEOUT = -1;

        //Stole this from a xamarin example & modified it a bit

        public static UIAlertController BasicNotificationAlert(string title, string description, UIViewController controller, string ex = null)
        {
            UIAlertController alert = UIAlertController.Create(title, description, UIAlertControllerStyle.Alert);

            //Configure the alert
            alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Default, (action) => { }));

            if (ex != null)
            {
                alert.AddAction(UIAlertAction.Create("Big Scary ExceptionMessage",UIAlertActionStyle.Destructive,(action =>
                {
                    UIAlertController a = UIAlertController.Create("Exception", ex, UIAlertControllerStyle.Alert);

                    a.AddAction(UIAlertAction.Create("Cool", UIAlertActionStyle.Default, (ac) => { }));
                    controller.PresentViewController(a, true, null);

                })));
            }

            controller.PresentViewController(alert, true, null);

            return alert;
        }

        public static void HookOntoGuiThead(Action action)
        {
            
            HasCallbackBeenRecieved sync = new HasCallbackBeenRecieved();

            NSRunLoop.Main.BeginInvokeOnMainThread(() =>
            {
                action();
                sync.SignalEvent.Set();
            });
            sync.SetupAsyncSignal();
            //sync.Wait(GUI_LOCK_TIMEOUT);
        }
    }
}