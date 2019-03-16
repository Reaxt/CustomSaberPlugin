using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
namespace CustomSaber
{
    internal static class ReflectionUtil
    {
        public static void SetPrivateField(this object obj, string fieldName, object value)
        {
            var field = obj.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            field.SetValue(obj, value);
        }

        public static T GetPrivateField<T>(this object obj, string fieldName)
        {
            var field = obj.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            var value = field.GetValue(obj);
            return (T)value;
        }

        public static object GetPrivateField(Type type, object obj, string fieldName)
        {
            var field = obj.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            var value = field.GetValue(obj);
            return value;
        }

        public static void InvokePrivateMethod(this object obj, string methodName, object[] methodParams)
        {
            var method = obj.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
            method.Invoke(obj, methodParams);
        }
        public static Component CopyComponent(Component original, Type originalType, Type overridingType, GameObject destination)
        {
            var copy = destination.AddComponent(overridingType);

            Type type = originalType;
            while (type != typeof(MonoBehaviour))
            {
                CopyForType(type, original, copy);
                type = type.BaseType;
            }

            return copy;
        }

        private static void CopyForType(Type type, Component source, Component destination)
        {
            FieldInfo[] myObjectFields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetField);

            foreach (FieldInfo fi in myObjectFields)
            {
                fi.SetValue(destination, fi.GetValue(source));
            }
        }
    }
}
