using System;

namespace Protobuf2CS
{
    internal class Program
    {
        private static void Prompt(out string protoName)
        {
            string tip1 = @"请确认:";
            string tip2 = @"1.你的.proto文件放在同级目录下";
            string tip3 = @"2.你的protoc.exe放在同级目录下";
            string message = $"{tip1}\n{tip2}\n{tip3}";
            string message2 = "\n输入你的.proto文件的文件名:程序会自动把.proto文件转换成同名.cs文件";

            Console.WriteLine(message);
            Console.WriteLine(message2);

            protoName = Console.ReadLine().Trim();
        }

        private static void Main()
        {
            char diskName = Environment.CurrentDirectory[0];
            Prompt(out string protoName);

            Cmd cmd = new Cmd("protobuf2cs");

            cmd.Add($"{diskName}:").
                Add($"cd {Environment.CurrentDirectory}").
                Add($"protoc.exe --csharp_out=./ {protoName}.proto");

            string result = cmd.Return();

            Console.WriteLine($"\n生成的cmd指令:\n{result}\n");

            CmdRunner.Execute(result, out string info);

            Console.WriteLine(info);
            Console.WriteLine("程序结束");

            Console.ReadKey();
        }
    }
}