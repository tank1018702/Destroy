namespace Destroy
{
    using System;
    using System.IO;
    using System.Text;
    using System.Reflection;
    using System.Collections.Generic;

    internal static class Setting
    {
        public class Config
        {
            public int CameraPosX;
            public int CameraPosY;
            public int CameraWidth;
            public int CameraHeight;
            public int CharWidth;
            public bool UseNet;
            public int ClientSyncRate;
            public int ServerBroadcastRate;
            public bool DebugMode;
        }

        private static Config SaveStandard(string path)
        {
            Config config = new Config();
            Type type = config.GetType();

            config.CameraWidth = 30;
            config.CameraHeight = 30;
            config.CameraPosX = 0;
            config.CameraPosY = 0;
            config.CharWidth = 2;
            config.UseNet = false;
            config.ServerBroadcastRate = 20;
            config.ClientSyncRate = 50;
            config.DebugMode = false;

            List<string> lines = new List<string>();
            foreach (var field in type.GetFields())
            {
                string key = field.Name;
                string value = Convert.ToString(field.GetValue(config));

                string keyValue = $"{key} : {value}";
                lines.Add(keyValue);
            }

            File.WriteAllLines(path, lines.ToArray());
            return config;
        }

        public static Config Load()
        {
            string path = Path.Combine(Application.ProgramDirectory, "Setting.txt");

            Config config = new Config();
            Type type = config.GetType();

            try
            {
                string[] lines = File.ReadAllLines(path, Encoding.UTF8);

                var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
                for (int i = 0; i < fields.Length; i++)
                {
                    FieldInfo field = fields[i];
                    string line = lines[i];

                    if (!line.Contains(":")) //必须包含:符号
                        continue;
                    string[] keyValue = line.Split(':'); //通过:拆成Key-Value
                    //去除首位空格
                    string key = keyValue[0].Trim(' ');
                    string value = keyValue[1].Trim(' ');

                    //转换value的类型
                    object obj = null;
                    //支持4中类型
                    switch (field.FieldType.Name)
                    {
                        case "String":
                            obj = value;
                            break;
                        case "Int32":
                            obj = int.Parse(value);
                            break;
                        case "Boolean":
                            obj = bool.Parse(value);
                            break;
                        case "Single":
                            obj = float.Parse(value);
                            break;
                    }

                    field.SetValue(config, obj);
                }
            }
            catch (Exception)
            {
                config = SaveStandard(path); //写入标准配置
            }
            return config;
        }
    }
}