using System;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace AO.Memory
{
    public class MemoryManager
    {
        private Process m_Process = null;
        private IntPtr m_hProcess = IntPtr.Zero;

        public MemoryManager(Process process)
        {
            m_Process = process;
            OpenProcess();
        }

        ~MemoryManager()
        {
            CloseProcess();
        }

        public void OpenProcess()
        {
            m_hProcess = MemoryAPI.OpenProcess(MemoryAPI.PROCESS_VM_ALL_ACCESS, 1, (uint)m_Process.Id);
        }

        public void CloseProcess()
        {
            int iRetValue = MemoryAPI.CloseHandle(m_hProcess);
            if (iRetValue == 0)
                throw new Exception("CloseHandle failed");
        }

        public IntPtr GetModuleBaseAddress(string moduleName)
        {
            IntPtr baseAddress = IntPtr.Zero;
            ProcessModule myProcessModule = null;

            ProcessModuleCollection myProcessModuleCollection;

            try
            {
                myProcessModuleCollection = m_Process.Modules;
            }
            catch { return IntPtr.Zero; }

            for (int i = 0; i < myProcessModuleCollection.Count; i++)
            {
                myProcessModule = myProcessModuleCollection[i];
                if (myProcessModule.ModuleName.Contains(moduleName))
                {
                    baseAddress = myProcessModule.BaseAddress;
                    break;
                }
            }

            return baseAddress;
        }

        public T Read<T>(IntPtr address, int structSize = 0)
        {
            if (structSize == 0)
                structSize = Marshal.SizeOf(typeof(T));
            byte[] bytes = new byte[structSize];
            IntPtr numRead = IntPtr.Zero;
            MemoryAPI.ReadProcessMemory(m_hProcess, address, bytes, (uint)bytes.Length, out numRead);
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            T structure = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();
            return structure;
        }

        public T Read<T>(int[] offsets, string module, int structSize = 0)
        {
            return Read<T>(GetModuleBaseAddress(module), structSize);
        }

        public T Read<T>(int[] offsets, IntPtr _address, int structSize = 0)
        {
            IntPtr address = _address + offsets[0];

            for (int i = 1; i < offsets.Length; i++)
                address = Read<IntPtr>(address) + offsets[i];

            return Read<T>(address, structSize);
        }

        public string ReadString(IntPtr address, int length = 0)
        {
            if (length == 0)
            {
                for (int i = 0; i < 100; i++)
                {
                    if (Read<byte>(address + i) == 0)
                    {
                        length = i;
                        break;
                    }
                }
            }

            byte[] buffer = new byte[length];
            IntPtr ptrBytesRead;

            MemoryAPI.ReadProcessMemory(m_hProcess, address, buffer, (uint)length, out ptrBytesRead);

            Encoding ascii = Encoding.GetEncoding("us-ascii");

            return ascii.GetString(buffer);
        }

        public string ReadString(int[] offsets, string module, int length)
        {
            return ReadString(GetModuleBaseAddress(module), length);
        }

        public string ReadString(int[] offsets, IntPtr _address, int length)
        {
            IntPtr address = _address + offsets[0];

            for (int i = 1; i < offsets.Length; i++)
                address = Read<IntPtr>(address) + offsets[i];

            return ReadString(address, length);
        }

        public string ReadUniString(IntPtr address, int length = 0)
        {
            if (length == 0)
            {
                for (int i = 0; i < 100; i++)
                {
                    if (Read<ushort>(address + i * 2) == 0)
                    {
                        if (i < 1)
                            throw new Exception("Invalid unicode string length");
                        else
                            length = i;
                        break;
                    }
                }
            }


            byte[] buffer = new byte[length * 2];
            IntPtr ptrBytesRead;

            MemoryAPI.ReadProcessMemory(m_hProcess, address, buffer, (uint)(length * 2), out ptrBytesRead);

            return Encoding.Unicode.GetString(buffer);
        }


        public string ReadUniString(int[] offsets, IntPtr _address, int length = 0)
        {
            IntPtr address = _address + offsets[0];

            for (int i = 1; i < offsets.Length; i++)
                address = Read<IntPtr>(address) + offsets[i];

            return ReadUniString(address, length);
        }
    
        public bool WriteString(IntPtr address, string value)
        {
            Encoding ascii = Encoding.GetEncoding("us-ascii");
            byte[] buffer = ascii.GetBytes(value.ToCharArray());
            IntPtr ptrBytesWrote;

            return MemoryAPI.WriteProcessMemory(m_hProcess, address, buffer, (uint)ascii.GetByteCount(value.ToCharArray()), out ptrBytesWrote);
        }

        public bool WriteString(int[] offsets, string module, string value)
        {
            return WriteString(GetModuleBaseAddress(module), value);
        }

        public byte[] ReadByteArray(IntPtr address, uint size)
        {
            byte[] buffer = new byte[size];
            IntPtr ptrBytesRead;

            MemoryAPI.ReadProcessMemory(m_hProcess, address, buffer, size, out ptrBytesRead);

            return buffer;
        }
    }
}