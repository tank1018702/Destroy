namespace Destroy
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    internal static class InvokeSystem
    {
        public class InvokeRequest
        {
            public object Instance;
            public string MethodName;
            public float DelayTime;

            public InvokeRequest(object instance, string methodName, float delayTime)
            {
                Instance = instance;
                MethodName = methodName;
                DelayTime = delayTime;
            }
        }

        public static List<InvokeRequest> Requests = new List<InvokeRequest>();

        public static void Update()
        {
            for (int i = 0; i < Requests.Count; i++)
            {
                InvokeRequest request = Requests[i];
                request.DelayTime -= Time.DeltaTime;
                if (request.DelayTime <= 0 && request.Instance != null)
                {
                    MethodInfo method = request.Instance.GetType().GetMethod(request.MethodName);
                    try
                    {
                        method?.Invoke(request.Instance, null);
                    }
                    catch (Exception) { }
                    Requests.Remove(request);
                }
            }
        }
    }
}