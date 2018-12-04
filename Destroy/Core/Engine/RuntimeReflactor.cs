﻿namespace Destroy
{
    using System.Reflection;

    internal static class RuntimeReflector
    {
        private static Assembly assembly;

        public static Assembly GetAssembly
        {
            get
            {
                if (assembly == null)
                    assembly = Assembly.GetEntryAssembly();
                return assembly;
            }
        }

        public static void SetPublicStaticProperty(object instance, string propertyName, object value)
        {
            PropertyInfo property = instance.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Static);
            property.SetValue(instance, value);
        }

        public static void SetPrivateStaticField(object instance, string fieldName, object value)
        {
            FieldInfo field = instance.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Static);
            field.SetValue(instance, value);
        }

        public static void SetPrivateInstanceField(object instance, string fieldName, object value)
        {
            FieldInfo field = instance.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            field.SetValue(instance, value);
        }

        public static object GetPrivateInstanceField(object instance, string fieldName)
        {
            FieldInfo field = instance.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            return field.GetValue(instance);
        }

        public static void InvokePublicInstanceMethod(object instance, string methodName, params object[] parameters)
        {
            instance.GetType().GetMethod(methodName).Invoke(instance, BindingFlags.Public | BindingFlags.Instance, null, parameters, null);
        }
    }
}