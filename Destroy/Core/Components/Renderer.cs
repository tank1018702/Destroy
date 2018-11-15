namespace Destroy
{
    public class Renderer : Component
    {
        /// <summary>
        /// 原数组
        /// </summary>
        public RendererData Data;

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init(RendererData data)
        {
            Data = data;
            Initialized = true;
        }
    }
}