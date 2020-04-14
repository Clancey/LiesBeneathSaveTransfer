using AndroidSdk;
using Microsoft.SqlServer.Server;
//using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

namespace LiesBeneathSaveTransfer
{
	public class AdbDevice
	{
		public Adb.AdbDevice Device { get; set; }

		public string Name { get; set; }
		public string DisplayName => Device == null ? "No Device" : $"{Name} - ({Device.Serial})";
		public override bool Equals(object obj)
		{
			if (obj == null && Device == null)
				return true;
			return Device?.Equals(obj) ?? false;
		}
		public override int GetHashCode() => Device?.GetHashCode() ?? 0;

	}
	public class AdbManager
	{

		
		public static string appDataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "LiesBeneathTransfer");
		public static string adbDirectory = Path.Combine(appDataDirectory, "adb");
		public static string platformToolsDirectory = Path.Combine(adbDirectory, "platform-tools");
		public static string tempDirectory = Path.Combine(appDataDirectory, "temp");
		public static string BackupDirectory = Path.Combine(appDataDirectory, "backups");

		public static AdbManager Shared { get; } = new AdbManager();
		Adb adb = new Adb();
		public AdbManager()
		{
			Directory.CreateDirectory(tempDirectory);
			Directory.CreateDirectory(BackupDirectory);
		}
		public async Task AquireAdb() 
		{
			try
			{
				if (await IsAdbSetup())
					return;
				await Task.Run(Download);
			}
			finally
			{
				RefreshDevices();
			}
		}
		async Task Download()
		{
			var appData = Directory.CreateDirectory(adbDirectory);
			adb = new Adb(appData);
			//Check if exists in new Dirctory
			if (await IsAdbSetup())
				return;

			var adbZipPAth = Path.Combine(tempDirectory, "adb.zip");
			try
			{
				var client = new WebClient();
				await Task.Run(()=>client.DownloadFile(new Uri("https://adbshell.com/upload/adb.zip"), adbZipPAth));
				Directory.CreateDirectory(platformToolsDirectory);
				System.IO.Compression.ZipFile.ExtractToDirectory(adbZipPAth, platformToolsDirectory);
				//sdkManager = new Adb(appData);
				var success = await IsAdbSetup();
			}
			catch (Exception ex)
			{

			}
			finally
			{
				if (File.Exists(adbZipPAth))
					File.Delete(adbZipPAth);
			}
			//Lets download it!
		}

		public async Task<bool> IsAdbSetup()
		{
			try
			{
				await Task.Run(adb.Acquire);
				return true;
			}
			catch
			{

				return false;
			}
		}

		Task monitorTask;
		bool isWatchingDevices;
		List<AdbDevice> _devices = new List<AdbDevice>
		{
			new AdbDevice(),
		};
		public IReadOnlyList<AdbDevice> Devices => _devices;

		public Action DevicesChanged { get; set; }

		public void RefreshDevices()
		{
			var devices = adb.GetDevices().Where(x=> x.Model == "Quest");
			var changed = devices.Count() != _devices.Count || devices.Any(d => !_devices.Any(x=> x.Device == d));
			if (changed)
			{
				_devices = devices.Select(d=>
				{
					return new AdbDevice
					{
						Device = d,
						Name = adb.GetDeviceName(d.Serial),
					};
				}).ToList();
				if (_devices.Count == 0)
					_devices.Add(new AdbDevice());
				DevicesChanged?.Invoke();
			}
		}

		public async void StartMonitoringDevices()
		{
			await AquireAdb();
			if (! await IsAdbSetup())
				return;
			isWatchingDevices = true;
			if (monitorTask?.IsCompleted ?? true)
				monitorTask = Task.Run(async () =>
				{
					while(isWatchingDevices)
					{
						RefreshDevices();
						await Task.Delay(1000);
					}
				});
		}
		public void StopMonitoringDevices()
		{
			isWatchingDevices = false;
		}

		public async Task<(bool Success, string Error)> Backup(AdbDevice device,  string fileName ,CancellationToken cancelToken ,  string appId = "com.Drifter.Kodiak")
		{
			if(! await IsAdbSetup())
			{
				return (false,"Adb is not Setup");
			}
			try
			{
				if (File.Exists(fileName))
					File.Delete(fileName);
				var success = await Task.Run(async () =>
				{
					
					var serial = device?.Device?.Serial;
					var serialArg = string.IsNullOrEmpty(serial) ? "" : $"-s \"{serial}\"";
					var result = await RunAdbCommand("backup",cancelToken, "-f", fileName, "-noapk", appId, serialArg);
					var fileInfo = new FileInfo(fileName);
					if (fileInfo.Length > 0)
						return true;
					//var resp = adb.( $"backup -f {fileName} -noapk {appId}", device.Device.Serial);
					return true;
				});
				return (success, "");
			}
			catch(Exception ex)
			{

				return (false, ex.Message);
			}

		}

		public async Task<(bool Success, string Error)> Restore(AdbDevice device, string fileName, CancellationToken cancelToken)
		{
			if (!await IsAdbSetup())
			{
				return (false, "Adb is not Setup");
			}
			try
			{
				if (File.Exists(fileName))
					File.Delete(fileName);
				var success = await Task.Run(async () =>
				{

					var serial = device?.Device?.Serial;
					var serialArg = string.IsNullOrEmpty(serial) ? "" : $"-s \"{serial}\"";
					var result = await RunAdbCommand("restore", cancelToken, serialArg);
					var fileInfo = new FileInfo(fileName);
					if (fileInfo.Length > 0)
						return true;
					//var resp = adb.( $"backup -f {fileName} -noapk {appId}", device.Device.Serial);
					return true;
				});
				return (success, "");
			}
			catch (Exception ex)
			{

				return (false, ex.Message);
			}
		}

		Task RunAdbCommand(string command, params string[] parameters) => RunAdbCommand(command, CancellationToken.None, parameters);
		async Task<ProcessResult> RunAdbCommand(string command,CancellationToken cancellationToken,  params string[] parameters)
		{
			var standardOutput = new List<string>();
			var standardError = new List<string>();

			var adbPath = adb.FindToolPath(adb.AndroidSdkHome);
			var args = string.Join(" ", parameters);
			var processes = new Process
			{
				StartInfo =
				{
					FileName = adbPath.FullName,
					UseShellExecute = false,
					Arguments = $"{command} args",
					//RedirectStandardOutput = true,
					//RedirectStandardError = true,

				}
			};
			//processes.OutputDataReceived += (s, e) =>
			//{
			//	if (e.Data != null)
			//		standardOutput.Add(e.Data);
			//};
			//processes.ErrorDataReceived += (s, e) =>
			//{

			//	if (e.Data != null)
			//		standardError.Add(e.Data);
			//};
			cancellationToken.Register(() =>
			{
				try
				{
					processes.Kill();
				}
				catch { }
			});

			processes.Start();
			//processes.BeginErrorReadLine();
			await Task.Run(() =>
			{
				processes.WaitForExit();
			});
			return new ProcessResult(standardOutput, standardError, processes.ExitCode);
		}


			}



	public class ProcessResult
	{
		public readonly List<string> StandardOutput;
		public readonly List<string> StandardError;

		public readonly int ExitCode;

		public bool Success
			=> ExitCode == 0;

		internal ProcessResult(List<string> stdOut, List<string> stdErr, int exitCode)
		{
			StandardOutput = stdOut;
			StandardError = stdErr;
			ExitCode = exitCode;
		}
	}
}
