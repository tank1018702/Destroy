namespace Destroy
{
    using System;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class CreatGameObject : Attribute
    {
        public Type[] RequiredComponents;

        public CreatGameObject(params Type[] requiredComponents) => RequiredComponents = requiredComponents;
    }
}