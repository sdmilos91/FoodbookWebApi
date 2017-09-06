using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Azure.NotificationHubs;
using System.Web.Http;

namespace Foodbook.WebApi.Utils
{
    public class NotificationHubHelper
    {
        public static async void SendNotificationAsync(string pns, string message, string userTag)
        {
            NotificationOutcome outcome = null;
            NotificationHubClient hub = null;

            try
            {
                hub = NotificationHubClient.CreateClientFromConnectionString("Endpoint=sb://foodbooknotificationhubns.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=gE9eD1tSAepYbSe4oGbW0C4ZMSmFeeAU8bdVIIeGiOg=", "FoodBookNotificationHub");
                                                                            
            }
            catch (Exception ex)
            {               
            }

            switch (pns.ToLower())
            {
                case "apns":
                    // iOS
                    string alert = "{\"aps\":{\"alert\":\"" + message + "\"}}";
                    outcome = await hub.SendAppleNativeNotificationAsync(alert, userTag);
                    break;
                case "gcm":

                    var notifJson = new { data = new { message = message } };
                    string notif = Newtonsoft.Json.JsonConvert.SerializeObject(notifJson);

                    outcome = await hub.SendGcmNativeNotificationAsync(notif, userTag);

                    break;
            }

            if (outcome != null)
            {
                if (!((outcome.State == NotificationOutcomeState.Abandoned) || (outcome.State == NotificationOutcomeState.Unknown)))
                {
                    int ret = 1;
                }
            }
        }
    }
}