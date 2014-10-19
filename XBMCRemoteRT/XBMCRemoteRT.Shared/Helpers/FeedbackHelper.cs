using System;
using System.Collections.Generic;
using System.Text;
using Windows.System;

namespace XBMCRemoteRT.Helpers
{
    public static class FeedbackHelper
    {
        private static string recepient = "akshay2000@hotmail.com";
        private static string subject = "Kodi Assist for Windows";
        public static async void SendFeedback(string feedbackMessage)
        {
            var uri = new Uri("mailto:?to=" + recepient + "&subject=" + subject + "&body=" + feedbackMessage);
            await Launcher.LaunchUriAsync(uri);
        }
    }
}
