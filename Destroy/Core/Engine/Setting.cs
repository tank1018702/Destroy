namespace Destroy
{
    using System.IO;
    

    internal static class Setting
    {
        public static void LoadSetting()
        {
            string path = Path.Combine(Application.ProgramDirectory, "Setting.txt");

        }
    }
}