using System.Collections.Generic;
using System.Text;

namespace Protobuf2CS
{
    internal class Cmd
    {
        private string operation;
        private List<string> list;

        internal Cmd(string operation)
        {
            this.operation = operation;
            list = new List<string>();
        }

        internal Cmd Add(string operation)
        {
            if (!string.IsNullOrEmpty(operation))
                list.Add(operation + " && ");
            return this;
        }

        internal string Return()
        {
            if (list.Count == 0)
                return string.Empty;

            string[] array = list.ToArray();
            StringBuilder builder = new StringBuilder();

            foreach (var each in array)
            {
                builder.Append(each);
            }

            string s = builder.ToString();
            s = s.Trim().TrimEnd('&');

            return s;
        }
    }
}