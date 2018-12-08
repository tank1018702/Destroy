namespace Destroy
{
    using System.IO;
    
    /// <summary>
    /// TODO Class
    /// </summary>
    public static class Setting
    {
        private class CameraSetting
        {
            public int Width;
            public int Height;
            public int CharWidth;
        }

        public static void LoadSetting()
        {
            string path = Path.Combine(Application.ProgramDirectory, "Setting.json");
        }
    }
}