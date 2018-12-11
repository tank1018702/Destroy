namespace Destroy
{
    using System;
    using System.IO;
    using System.Text;
    using System.Reflection;
    using System.Collections.Generic;

    public static class Setting
    {
        public class Config
        {
            public int CameraPosX;
            public int CameraPosY;
            public int CameraWidth;
            public int CameraHeight;
            public int CharWidth;
            public bool Client;
            public bool Server;
        }

        private static Config SaveStandard(string path)
        {
            Config config = new Config();
            Type type = config.GetType();
            config.CameraWidth = 30;
            config.CameraHeight = 30;
            config.CameraPosX = -config.CameraWidth / 2;
            config.CameraPosY = -config.CameraHeight / 2;
            config.CharWidth = 2;
            config.Client = false;
            config.Server = false;

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
                foreach (var line in lines)
                {
                    if (!line.Contains(":")) //必须包含:符号
                        continue;
                    string[] keyValue = line.Split(':'); //通过:拆成Key-Value
                    //去除首位空格
                    string key = keyValue[0].Trim(' ');
                    string value = keyValue[1].Trim(' ');
                    //赋值
                    FieldInfo fieldInfo = type.GetField(key, BindingFlags.Instance | BindingFlags.Public);

                    //转换value的类型
                    object obj = null;
                    //支持4中类型
                    switch (fieldInfo.FieldType.Name)
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

                    fieldInfo.SetValue(config, obj);
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