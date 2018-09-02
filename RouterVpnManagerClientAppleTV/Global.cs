using System;
using System.Drawing;

using CoreGraphics;
using Foundation;
using UIKit;

namespace RouterVpnManagerClient
{
    public class Global
    {
        //Stole this from a xamarin example
        public static UIAlertController BasicNotificationAlert(string title, string description, UIViewController controller)
        {
            // No, inform the user that they must create a home first
            UIAlertController alert = UIAlertController.Create(title, description, UIAlertControllerStyle.Alert);

            // Configure the alert
            alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (action) => { }));

            // Display the alert
            controller.PresentViewController(alert, true, null);

            // Return created controller
            return alert;
        }
    }
}