﻿
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace WebApiXafSecurity.Helpers
{
	public static class JsonParser
	{
		public static void ParseJObjectXPO<T>(JObject jObject, object obj, IObjectSpace objectSpace) where T : XPObject
		{
			ITypeInfo typeInfo = objectSpace.TypesInfo.FindTypeInfo(typeof(T));
			List<string> memberNameList = jObject.Properties().Select(p => p.Name).ToList();
			foreach (string memberName in memberNameList)
			{
				IMemberInfo memberInfo = typeInfo.FindMember(memberName);
				if (memberInfo.IsAssociation)
				{
					ParseAssociationProperty(jObject, obj, memberInfo, objectSpace);
				}
				else
				{
					ParseSimpleProperty(jObject, obj, memberInfo);
				}
			}
		}

		public static void ParseJObject<T>(JObject jObject, object obj, IObjectSpace objectSpace) where T : BaseObject
		{
			ITypeInfo typeInfo = objectSpace.TypesInfo.FindTypeInfo(typeof(T));
			List<string> memberNameList = jObject.Properties().Select(p => p.Name).ToList();
			foreach (string memberName in memberNameList)
			{
				IMemberInfo memberInfo = typeInfo.FindMember(memberName);
				if (memberInfo.IsAssociation)
				{
					ParseAssociationProperty(jObject, obj, memberInfo, objectSpace);
				}
				else
				{
					ParseSimpleProperty(jObject, obj, memberInfo);
				}
			}
		}
		private static void ParseSimpleProperty(JObject jObject, object obj, IMemberInfo memberInfo)
		{
			JValue jValue = (JValue)jObject[memberInfo.Name];
			object value = ConvertType(jValue, memberInfo);
			memberInfo.SetValue(obj, value);
		}
		private static void ParseAssociationProperty(JObject jObject, object obj, IMemberInfo memberInfo, IObjectSpace objectSpace)
		{
			string keyPropertyName = memberInfo.MemberTypeInfo.KeyMember.Name;
			JToken keyToken = jObject[memberInfo.Name][keyPropertyName];
			object keyValue = ConvertType((JValue)keyToken, memberInfo.MemberTypeInfo.KeyMember);
			object value = objectSpace.GetObjectByKey(memberInfo.MemberType, keyValue);
			memberInfo.SetValue(obj, value);
		}
		private static object ConvertType(JValue jValue, IMemberInfo memberInfo)
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
