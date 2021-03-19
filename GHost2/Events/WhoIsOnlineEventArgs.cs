using System;
using System.Collections.Specialized;

namespace MajorBBS.GHost
{
    // TODOX Only used to get a list of users who are online for the TranslateMCI -- refactor it out
    public class WhoIsOnlineEventArgs : EventArgs
    {
        public StringDictionary WhoIsOnline { get; private set; }

        public WhoIsOnlineEventArgs()
        {
            WhoIsOnline = new StringDictionary();
        }
    }
}
