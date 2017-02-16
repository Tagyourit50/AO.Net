using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using AO.Common;
using AO.Core;
using AO.Memory;

namespace Testing
{
    class Program
    {
        static void Main(string[] args)
        {
            MemoryManager mem = new MemoryManager(Process.GetProcessesByName("AnarchyOnline")[0]);

            byte[] desu = mem.Read<byte[]>(new IntPtr(0x34AF8CE8), 10);
            Console.WriteLine(desu);

            Console.ReadLine();
        }
    }

    public struct Test
    {
        float blah1;
        float blah2;
        int blah3;
    }
}
