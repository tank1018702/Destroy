namespace Destroy
{
    using System;

    public abstract class Script : Component
    {
        public bool Started;

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

        public override Component Clone()
        {
            throw new NotImplementedException("不要调用该方法!");
        }
    }
}