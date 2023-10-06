using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;

namespace SysLab1
{
    class ProcessView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        private long _memory;
		public long Memory
		{
            get => _memory; /*/ (int)(1024);*/
            set => _memory = value;
		}
        private string _priority = "NO_ACCESS";
        public string Priority
        {
            get => _priority;
            set => _priority = value;
        }
        public ProcessThreadCollection Threads { get; set; }
        public int ThreadsCount { get; set; }
        private string _owner = string.Empty;
        public string Owner
        {
            get => _owner;
            set => _owner = value;
        }

        public ProcessView() { }

        public ProcessView(Process process)
        {
            Id = process.Id;
            Name = process.ProcessName;
            Memory = process.PrivateMemorySize64;
            //var ram = new PerformanceCounter("Process", "Working Set - Private", Name, true);
            //var ramUsage = Convert.ToInt32(ram.NextValue()) / (int)(1024);
            //Memory = ramUsage;
            try
            {
                Priority = process.PriorityClass.ToString();
            }
            catch
            {
                Debug.WriteLine("Error occured while tried to get priority.");
            }
            Threads = process.Threads;
            ThreadsCount = Threads.Count;
		}
    }
}
