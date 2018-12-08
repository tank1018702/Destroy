namespace Destroy
{
    using System.IO;
    

    public static class Setting
    {
        public static void LoadSetting()
        {
            string path = Path.Combine(Application.ProgramDirectory, "Setting.txt");

        }
    }
}