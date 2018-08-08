using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Http;

namespace AtriumWebApp.Web.Base.Library
{
    public class AppCodeSessionProperty<T>
    {
        private string _Key;
        private ISession _SessionState;

        public AppCodeSessionProperty(string key, HttpContext context)
        {
            _Key = key;
            _SessionState = context.Session;
        }

        public T this[string appCode]
        {
            get
            {
                T obj;
                _SessionState.TryGetObject(appCode + _Key,out obj);
                return obj;
            }
            set
            {
                _SessionState.SetItem(appCode + _Key, value);
            }
        }

        public bool TryGet(string appCode, out T value)
        {
            try
            {
                value = this[appCode];
                return !EqualityComparer<T>.Default.Equals(value, default(T));
            }
            catch (InvalidCastException)
            { }
            catch (NullReferenceException)
            { }
            value = default(T);
            return false;
        }
    }
}
