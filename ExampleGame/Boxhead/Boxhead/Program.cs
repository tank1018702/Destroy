﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Destroy;

class Program
{
    static void Main(string[] args)
    {
        RuntimeEngine runtimeEngine = new RuntimeEngine(20, true);
        runtimeEngine.Run();

    }
}