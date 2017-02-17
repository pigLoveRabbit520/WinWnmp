using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;

namespace SalamanderWnmp
{

    public static class DispatcherHelper
    {
        public static Dispatcher UIDispatcher
        {
            get;
            private set;
        }

       

        public static void Initialize()
        {
            if (UIDispatcher != null)
            {
                return;
            }

#if SILVERLIGHT
            UIDispatcher = Deployment.Current.Dispatcher;
#else
            UIDispatcher = Dispatcher.CurrentDispatcher;
#endif
        }
    }
}
