namespace Destroy
{
    using System.Reflection;

    public class RuntimeReflector
    {
        public static void SetStaticPrivateProperty(object instance, string propertyName, object value)
        {
            PropertyInfo property = instance.GetType().GetProperty(propertyName);
            property.SetValue(instance, value);
        }

        public static void SetPrivateField(object instance, string fieldName, object value)
        {
            FieldInfo field = instance.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            field.SetValue(instance, value);
        }

        public static object GetPrivateField(object instance, string fieldName)
        {
            FieldInfo field = instance.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            return field.GetValue(instance);
        }
    }
}