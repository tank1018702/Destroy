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

        public static void CancleInvokeRequest(object instance, string methodName)
        {
            for (int i = 0; i < requests.Count; i++)
            {
                InvokeRequest request = requests[i];
                if (request.Instance == instance && request.MethodName == methodName)
                    requests.Remove(request);
            }
        }

        public static bool IsInvoking(object instance, string methodName)
        {
            foreach (var request in requests)
            {
                if (request.Instance == instance && request.MethodName == methodName)
                    return true;
            }
            return false;
        }

        private class DelayAction
        {
            public Action Action;
            public float DelayTime;

            public DelayAction(Action action, float delayTime)
            {
                Action = action;
                DelayTime = delayTime;
            }
        }

        private static List<DelayAction> delayActions = new List<DelayAction>();

        public static void AddDelayAction(Action action, float delayTime)
        {
            delayActions.Add(new DelayAction(action, delayTime));
        }

        public static void RemoveDelayAction(Action action)
        {
            for (int i = 0; i < delayActions.Count; i++)
            {
                DelayAction delayAction = delayActions[i];
                if (delayAction.Action == action)
                    delayActions.Remove(delayAction);
            }
        }

        public static bool IsDelaying(Action action)
        {
            foreach (var delayAction in delayActions)
            {
                if (delayAction.Action == action)
                    return true;
            }
            return false;
        }

        public static void Update()
        {
            List<InvokeRequest> removeRequests = new List<InvokeRequest>();
            for (int i = 0; i < requests.Count; i++)
            {
                InvokeRequest request = requests[i];

                request.DelayTime -= Time.DeltaTime;
                if (request.DelayTime > 0)
                    continue;
                removeRequests.Add(request); //准备移除
                if (request.Instance == null)
                    continue;
                //调用
                MethodInfo methodInfo = request.Instance.GetType().GetMethod(request.MethodName);
                methodInfo?.Invoke(request.Instance, null);
            }
            //移除
            foreach (InvokeRequest request in removeRequests)
                requests.Remove(request);

            List<DelayAction> removeActions = new List<DelayAction>();
            for (int i = 0; i < delayActions.Count; i++)
            {
                DelayAction delayAction = delayActions[i];

                delayAction.DelayTime -= Time.DeltaTime;
                if (delayAction.DelayTime > 0)
                    continue;
                removeActions.Add(delayAction); //准备移除
                if (delayAction.Action == null)
                    continue;
                delayAction.Action();           //调用
            }
            //移除
            foreach (DelayAction delayAction in removeActions)
                delayActions.Remove(delayAction);
        }
    }
}