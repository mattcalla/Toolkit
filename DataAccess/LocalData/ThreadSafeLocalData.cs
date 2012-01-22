using System;
using System.Collections;
using System.Web;

namespace Toolkit.DataAccess.LocalData
{
    public interface ILocalData
    {
        object this[object key] { get; set; }
        int Count { get; }
        void Clear();
    }

    public static class ThreadSafeLocalData
    {
        static readonly ILocalData _data = new LocalData();

        public static ILocalData Data
        {
            get { return _data; }
        }

        private class LocalData : ILocalData
        {
            [ThreadStatic]
            private static Hashtable _localData;

            private static readonly object LocalDataHashtableKey = new object();

            private static Hashtable LocalHashtable
            {
                get
                {
                    if (IsRunningInWeb)
                    {
                        var webHashtable = HttpContext.Current.Items[LocalDataHashtableKey] as Hashtable;
                        if (webHashtable == null)
                        {
                            webHashtable = new Hashtable();
                            HttpContext.Current.Items[LocalDataHashtableKey] = webHashtable;
                        }
                        return webHashtable;
                        
                    }
                    else //Windows Desktop
                    {
                        return _localData ?? (_localData = new Hashtable());
                    }
                }
            }

            public object this[object key]
            {
                get { return LocalHashtable[key]; }
                set { LocalHashtable[key] = value; }
            }

            public int Count
            {
                get { return LocalHashtable.Count; }
            }

            public void Clear()
            {
                LocalHashtable.Clear();
            }

            private static bool IsRunningInWeb
            {
                get { return HttpContext.Current != null; }
            }
        }
    }
}
