namespace Destroy
{
    using System.IO;

    /// <summary>
    /// TODO Class
    /// </summary>
    internal static class Setting
    {
        private class Json
        {
            public int Width;
            public int Height;
            public int CharWidth;
        }

        public static void SetStandardSetting()
        {
            Json json = new Json();
            json.CharWidth = 2;
            json.Height = 20;
            json.Width = 20;
            string path = Path.Combine(Application.ProgramDirectory, "Setting.json");
            Storage.SaveToJson(json, path);
        }

        public static void LoadSetting()
        {
            string path = Path.Combine(Application.ProgramDirectory, "Setting.json");
        }
    }
}