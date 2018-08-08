using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Session;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AtriumWebApp.Web.Base.Library
{
    public static class HttpContextExtension
    {

        //Microsoft.AspNetCore.Http
        public static string UrlPath(this HttpContext context)
        {

            string pathBase = context.Request.PathBase;
            string path = context.Request.Path;
            if (string.IsNullOrEmpty(pathBase))
            {
                return path;
            }
            else
            {
                return pathBase + path;
            }
        }
        public static string BaseUrlPath(this IUrlHelper helper)
        {
            HttpContext context = helper.ActionContext.HttpContext;
            string controller = helper.ActionContext.RouteData.Values["controller"].ToString();
            string path = context.UrlPath();
            if (path.Contains(controller))
            {
                path = path.Substring(0, path.LastIndexOf(controller));
            }
            return path;
        }
    }

    public static class SessionExtension
    {
        public static void SetItem(this ISession session, string key, object item)
        {
            var converter = new IsoDateTimeConverter()
            {
                DateTimeStyles = DateTimeStyles.AdjustToUniversal
            };
            List<JsonConverter> converters = new List<JsonConverter>();
            var settings = new JsonSerializerSettings
            {
                Converters = converters,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            if(item == null)
            {
                session.Remove(key);
                return;
            }
            Type itemType = item.GetType();
            string jsonItem = JsonConvert.SerializeObject(item, settings);
            session.SetString(key, jsonItem);
        }

        public static bool Contains(this ISession session, string key)
        {
            if (!session.Keys.Contains(key))
            {
                return false;
            }
            return true;
        }

        public static bool TryGetObject<T>(this ISession session, string key, out T item)
        {
            try
            {
                if (!session.Keys.Contains(key))
                {
                    item = default(T);
                    return false;
                }
                else
                {
                    string jsonItem = session.GetString(key);
                    item = JsonConvert.DeserializeObject<T>(jsonItem);
                    return true;
                }
            }
            catch (Exception ex)
            {
                item = default(T);
                return false;
            }
        }

        public static void SetTimeout(this ISession session,int time)
        {
            TimeSpan span = new TimeSpan(0, time, 0);
        
            SetInstanceField(typeof(DistributedSession), session, "_idleTimeout",span);
        }
        internal static void SetInstanceField(Type type, object instance, string fieldName,object value)
        {
            BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                | BindingFlags.Static;
            FieldInfo field = type.GetField(fieldName, bindFlags);
            field.SetValue(instance,value);
        }

        public static object GetTimeout(this ISession session)
        {
            var span = (TimeSpan)GetInstanceField(typeof(DistributedSession), session, "_idleTimeout");
            return span.TotalSeconds;
        }

        internal static object GetInstanceField(Type type, object instance, string fieldName)
        {
            BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                | BindingFlags.Static;
            FieldInfo field = type.GetField(fieldName, bindFlags);
            return field.GetValue(instance);
        }
    }
}
