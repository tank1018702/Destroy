namespace Destroy
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    internal static class InvokeSystem
    {
        private class InvokeRequest
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

        private class CancleRequest
        {
            public object Instance;
            public string MethodName;

            public CancleRequest(object instance, string methodName)
            {
                Instance = instance;
                MethodName = methodName;
            }
        }

        private static List<InvokeRequest> requests = new List<InvokeRequest>();

        public static void AddInvokeRequest(object instance, string methodName, float delayTime)
        {
            requests.Add(new InvokeRequest(instance, methodName, delayTime));
        }

        public static void CancleInvokeRequest(object instance,string methodName)
        {
            for (int i = 0; i < requests.Count; i++)
            {
                InvokeRequest request = requests[i];
                if (request.Instance == instance && request.MethodName == methodName)
                    requests.Remove(request);
            }
        }

        public static bool IsInvoking(object instance,string methodName)
        {
            foreach (var request in requests)
            {
                if (request.Instance == instance && request.MethodName == methodName)
                    return true;
            }
            return false;
        }

        public static void Update()
        {
            for (int i = 0; i < requests.Count; i++)
            {
                InvokeRequest request = requests[i];
                request.DelayTime -= Time.DeltaTime;
                if (request.DelayTime <= 0 && request.Instance != null)
                {
                    MethodInfo method = request.Instance.GetType().GetMethod(request.MethodName);
                    try
                    {
                        method?.Invoke(request.Instance, null);
                    }
                    catch (Exception) { }
                    requests.Remove(request);
                }
            }
        }
    }
}