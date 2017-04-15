using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Threading;

namespace SalamanderWnmp.Tool
{

    public static class DispatcherHelper
    {
        public static Dispatcher UIDispatcher
        {
            get;
            private set;
        }

        private static int MainThreadId =  Thread.CurrentThread.ManagedThreadId; 

       

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
