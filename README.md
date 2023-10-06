# WindowsTaskManager

This project was a part of my university laboratory work for System Programming course. The task was quite simple: build window application with info about proccess (Id, Name, MemoryUse, NumberOfThreads, Owner) and more detailed info about each thread in proccess. The main problem that i faced with was getting proccess owner from Win32API. It is quite time-consuming task, because we address to a system functions, and if this program was on C++ or another language that closer to low-level structures, this task will take less time to execute. But in my case i have knowledge only in C#, so i used built-in language methods to solve this problem.

Project was built using WPF (Windows Presentation Forms) for they responsive interface.
