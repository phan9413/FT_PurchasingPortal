using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using Newtonsoft.Json.Linq;
using System;
using System.Globalization;

namespace PRWebApi.Helpers
{
    public static class JsonPopulateObjectHelper
    {
        public static void PopulateObject(string json, Session session, PersistentBase obj)
        {
            PopulateObject(json, session, obj.ClassInfo, obj);
        }

        public static void PopulateObject(string json, Session session, XPClassInfo classInfo, object obj)
        {
            PopulateObject(JObject.Parse(json), session, classInfo, obj);
        }

        public static T PopulateObject<T>(string json, Session session) where T : PersistentBase
        {
            return (T)PopulateObject(json, session, session.Dictionary.GetClassInfo(typeof(T)));
        }

        public static object PopulateObject(string json, Session session, XPClassInfo classInfo)
        {
            return PopulateObject(JObject.Parse(json), session, classInfo);
        }

        static object PopulateObject(JObject jobject, Session session, XPClassInfo classInfo)
        {
            object obj = classInfo.CreateObject(session);
            PopulateObject(jobject, session, classInfo, obj);
            return obj;
        }
        //private static void ParseAssociationProperty(JObject jObject, object obj, XPMemberInfo memberInfo, Session session)
        //{
        //    string keyPropertyName = memberInfo.ReferenceType.KeyProperty.Name;
        //    JToken keyToken = jObject[memberInfo.Name][keyPropertyName];
        //    object keyValue = ConvertType((JValue)keyToken, memberInfo.ReferenceType.KeyProperty);
        //    object value = session.GetObjectByKey(memberInfo.MemberType, keyValue);
        //    memberInfo.SetValue(obj, value);
        //}

        static void PopulateObject(JObject jobject, Session session, XPClassInfo classInfo, object obj)
        {
            foreach (XPMemberInfo mi in classInfo.Members)
            {
                if (!jobject.ContainsKey(mi.Name))
                {
                    continue;
                }
                if (mi.ReferenceType != null && !mi.IsCollection)
                {
                    PopulateReferenceProperty(jobject, obj, mi, session);
                }
                else if (mi.IsCollection)
                {
                    //throw new NotImplementedException();
                    PopulateCollectionProperty(jobject, obj, mi, session);
                }
                else if (!mi.IsAliased && !mi.IsReadOnly)
                {
                    PopulateProperty(jobject, obj, mi);
                }
            }
        }
        static void PopulateCollectionProperty(JObject jobject, object obj, XPMemberInfo memberInfo, Session session)
        {
            JArray jarray = (JArray)jobject[memberInfo.Name];
            foreach (JObject dtl in jarray.Children())
            {
                try
                {
                    XPClassInfo classinfo = session.Dictionary.GetClassInfo(PRWebApi.Helpers.HelperXaf.xafAssembly, PRWebApi.Helpers.HelperXaf.xafAssembly + "." + memberInfo.Name);
                    PopulateObject(dtl, session, classinfo);
                }
                catch (Exception ex)
                {

                }
            }
        }

        static void PopulateProperty(JObject jobject, object obj, XPMemberInfo memberInfo)
        {
            JValue jvalue = (JValue)jobject[memberInfo.Name];
            object value = jvalue.Value;
            if (value != null)
            {
                if (value.GetType() != memberInfo.StorageType)
                {
                    value = Convert.ChangeType(value, memberInfo.StorageType, CultureInfo.InvariantCulture);
                }
            }
            memberInfo.SetValue(obj, value);
        }

        static void PopulateReferenceProperty(JObject jobject, object obj, XPMemberInfo memberInfo, Session session)
        {
            JObject refJObject = null;
            XPMemberInfo keyMemberInfo = memberInfo.ReferenceType.KeyProperty;
            if (jobject[memberInfo.Name] is JValue referenceShort)
            {
                dynamic nestedJObject = new JObject();
                nestedJObject[keyMemberInfo.Name] = referenceShort;
                refJObject = nestedJObject;
            }
            else if (jobject[memberInfo.Name] is JObject referenceLong)
            {
                refJObject = referenceLong;
            }
            else if (refJObject == null)
            {
                throw new ArgumentException("Unknown JSON format for reference properties! Short and long formats are supported: '{{ReferenceName: KeyValue}}' or {{ReferenceName: {{KeyName: KeyValue}}}}.", "jobject");
            }
            object refObject = memberInfo.GetValue(obj);
            if (refJObject != null)
            {
                JToken keyToken = refJObject[memberInfo.ReferenceType.KeyProperty.Name];
                object keyValue = ((JValue)keyToken).Value;
                if (keyValue != null)
                {
                    if (keyValue.GetType() != keyMemberInfo.MemberType)
                    {
                        keyValue = Convert.ChangeType(keyValue, keyMemberInfo.MemberType, CultureInfo.InvariantCulture);
                    }
                    refObject = session.GetObjectByKey(memberInfo.ReferenceType, keyValue);
                }
            }
            else
            {
                refObject = null;
            }
            if (refObject != null)
            {
                PopulateObject(refJObject, session, memberInfo.ReferenceType, refObject);
            }
            memberInfo.SetValue(obj, refObject);
        }
        private static object ConvertType(JValue jValue, XPMemberInfo memberInfo)
        {
            object value = jValue.Value;
            if (value != null)
            {
                if (value.GetType() != memberInfo.MemberType)
                {
                    if (value is string && memberInfo.MemberType == typeof(Guid))
                    {
                        value = Guid.Parse((string)value);
                    }
                    else
                    {
                        value = Convert.ChangeType(value, memberInfo.MemberType, CultureInfo.InvariantCulture);
                    }
                }
            }
            return value;
        }
    }
}