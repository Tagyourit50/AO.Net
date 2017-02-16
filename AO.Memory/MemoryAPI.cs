using System;
using System.Runtime.InteropServices;

namespace AO.Memory
{
    internal class MemoryAPI
    {
        public const uint PROCESS_VM_READ = (0x0010);
        public const uint PROCESS_VM_ALL_ACCESS = (0x1F0FFF);

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(UInt32 dwDesiredAccess, Int32 bInheritHandle, UInt32 dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern Int32 ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [In, Out] byte[] buffer, UInt32 size, out IntPtr lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, UInt32 nSize, out IntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        public static extern bool VirtualProtectEx(
            IntPtr hProcess,
            uint dwAddress, //IntPtr lpAddress,
            int nSize,      //UIntPtr dwSize,
            uint flNewProtect,
            out uint lpflOldProtect);

        [DllImport("kernel32.dll")]
        public static extern Int32 CloseHandle(IntPtr hObject);
    }
}
