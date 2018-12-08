namespace Destroy
{
    public abstract class Script : Component
    {
        internal bool Started;

        public Script() => Started = false;

        public virtual void Start() { }

        public virtual void Update() { }

        public virtual void OnCollision(Collider collision) { }

        /// <summary>
        /// 延迟调用一个方法(该方法必须为实例无参public方法)
        /// </summary>
        public void Invoke(string methodName, float delayTime)
        {
            InvokeSystem.AddInvokeRequest(this, methodName, delayTime);
        }

        /// <summary>
        /// 取消一个延迟调用的方法
        /// </summary>
        public void CancleInvoke(string methodName)
        {
            InvokeSystem.CancleInvokeRequest(this, methodName);
        }

        /// <summary>
        /// 该方法是否在延迟调用
        /// </summary>
        public bool IsInvoking(string methodName)
        {
            return InvokeSystem.IsInvoking(this, methodName);
        }

        [System.Obsolete("Dont use this for now")]
        /// 重复延迟调用一个方法(该方法必须为实例无参public方法)
        public void InvokeRepeating(string methodName, float delayTime, float repeatTime)
        {
            Invoke(methodName, delayTime);

            InvokeSystem.AddDelayAction(() =>
            {
                Invoke(methodName, repeatTime);

                void Repeat()
                {
                    Invoke(methodName, repeatTime);
                    InvokeSystem.AddDelayAction(Repeat, repeatTime);
                }

                InvokeSystem.AddDelayAction(() =>
                {
                    Repeat();
                }, repeatTime);

            }, delayTime);
        }
    }
}