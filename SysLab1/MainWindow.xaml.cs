using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace SysLab1
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		ObservableCollection<ProcessView> processes = new ObservableCollection<ProcessView>();
		public MainWindow()
		{
			InitializeComponent();
			Load();
			
		}

		private void Load()
		{
			update.IsEnabled = false;
			GetProcesses();
			GetAllOwners();
		}

		private void GetProcesses()
		{
			if (processes.Count != 0) processes.Clear();
			foreach (Process process in Process.GetProcesses())
			{
				processes.Add(new ProcessView(process));
			}
		}	

		public async void GetAllOwners()
		{
			ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Process");

			var requestedProcesses = searcher.Get();

			if (requestedProcesses.Count == 0)
				return;

			await Task.Run(() =>
			{
				int i = 0;
				foreach (ManagementObject Process in requestedProcesses)
				{
					if (Process["ExecutablePath"] != null)
					{
						string ExecutablePath = Process["ExecutablePath"].ToString();

						ProcessView pbuff = new ProcessView();
						string[] OwnerInfo = new string[2];
						try
						{
							Process.InvokeMethod("GetOwner", (object[])OwnerInfo);
							pbuff = LinkProccesAndOwner(OwnerInfo[0], Convert.ToInt32(Process["Handle"]));
						}
						catch
						{
							pbuff = LinkProccesAndOwner("NO_ACCESS", Convert.ToInt32(Process["Handle"]));
						}
						finally
						{
							ProgressBarChanging(i, requestedProcesses.Count, pbuff);
						}
					}
					i++;
				}
			}).ContinueWith(SearchEnding);
		}

		private void ProgressBarChanging(int i, int processesCount, ProcessView pbuff)
		{
			this.Dispatcher.Invoke(new Action(() =>
			{
				pbStatus.Value = (double)i / processesCount * 100;
				if(pbuff != null) processView.Items.Add(pbuff);
				count.Text = "Count: " + processView.Items.Count;
			}));
		}

		private void SearchEnding(Task task)
		{
			this.Dispatcher.Invoke(new Action(() =>
			{
				pbStatus.Visibility = Visibility.Collapsed;
				processView.Items.Add(processes.Select(p => p.Owner == "NO_ACCESS"));
				update.IsEnabled = true;
			}));
		}

		private ProcessView? LinkProccesAndOwner(string owner, int PID)
		{
			var pbuff = processes.FirstOrDefault(p => p.Id == PID);
			if(pbuff != null) pbuff.Owner = owner;
			return pbuff;
		}

		// Функция для получения владельца процесса по ID процесса
		//private string GetProcessOwner(int PID)
		//{
		//	ObjectQuery sq = new ObjectQuery
		//	("Select * from Win32_Process Where ProcessID = '" + PID + "'");
		//	ManagementObjectSearcher Processes = new ManagementObjectSearcher(sq);

		//	string User = string.Empty;
		//	string processname = String.Empty;
		//	foreach (ManagementObject Process in Processes.Get())
		//	{
		//		if (Process["ExecutablePath"] != null)
		//		{
		//			string ExecutablePath = Process["ExecutablePath"].ToString();
		//			string[] o = new String[2];

		//			Process.InvokeMethod("GetOwner", (object[])o);

		//			processname = (string)Process["Name"];

		//			User = o[0];
		//			if (User == null)
		//				User = String.Empty;
		//			return User;
		//		}
		//	}
		//	return User;

		//}

		private ListView CreateThreadsListView(ProcessThreadCollection threads)
		{
			ListView threadsView = new ListView();

			GridView threadsGridView = new GridView();
			threadsGridView.AllowsColumnReorder = true;
			threadsGridView.ColumnHeaderToolTip = "Threads";

			GridViewColumn gvc1 = new GridViewColumn();
			gvc1.DisplayMemberBinding = new Binding("Id");
			gvc1.Header = "id";
			gvc1.Width = 50;
			threadsGridView.Columns.Add(gvc1);

			GridViewColumn gvc2 = new GridViewColumn();
			gvc2.DisplayMemberBinding = new Binding("BasePriority");
			gvc2.Header = "priority";
			gvc2.Width = 50;
			threadsGridView.Columns.Add(gvc2);

			threadsView.ItemsSource = threads;
			threadsView.View = threadsGridView;
			return threadsView;
		}

		private void button1_Click(object sender, RoutedEventArgs e)
		{
			Button button = sender as Button;
			ProcessView process = button.DataContext as ProcessView;
			if(processInfo.Items.Count > 1) processInfo.Items.RemoveAt(1);
			ListView threadsListView = CreateThreadsListView(process.Threads);
			processInfo.Items.Add(new TabItem
			{
				Header = string.Format("{0} threads", process.Name),
				Content = threadsListView
			});
			processInfo.SelectedIndex = 1;
		}

		private void update_Click(object sender, RoutedEventArgs e)
		{
			pbStatus.Value = 0;
			pbStatus.Visibility = Visibility.Visible;
			processView.Items.Clear();
			Load();
		}
	}
}
