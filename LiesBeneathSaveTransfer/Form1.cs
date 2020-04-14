using AndroidSdk;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LiesBeneathSaveTransfer
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		protected override void OnCreateControl()
		{
			base.OnCreateControl();
			Setup();

		}

		async void Setup()
		{
			try
			{
				AdbManager.Shared.DevicesChanged = SetupComboBoxItems;
				await AdbManager.Shared.AquireAdb();
				loadingPanel.Visible = false;
			}
			catch (Exception ex)
			{
				loadingLabel.Text = ex.Message;
			}
		}

		void SetupComboBoxItems()
		{
			var item = deviceComboBox.SelectedItem;
			deviceComboBox.DisplayMember = "DisplayName";
			deviceComboBox.Items.Clear();
			var items = AdbManager.Shared.Devices.ToArray();
			deviceComboBox.Items.AddRange(items);
			if (item == null)
				deviceComboBox.SelectedIndex = 0;
			else
			{
				var newItem = items.FirstOrDefault(x => x.Equals(item));
				if (newItem == null)
					deviceComboBox.SelectedIndex = 0;
				else
					deviceComboBox.SelectedItem = newItem;
			}

		}

		private void button1_Click(object sender, EventArgs e)
		{
			AdbManager.Shared.RefreshDevices();
		}

		async void SetState()
		{
			var isDeviceReady = await IsDeviceReady();
		}

		async Task<bool> IsDeviceReady()
		{
			bool hasAdb = await AdbManager.Shared.IsAdbSetup();
			var device = deviceComboBox.SelectedItem as AdbDevice;
			return device?.Device != null;
		}
		bool isRunningBackup;
		CancellationTokenSource backupCancelationTokenSource;
		private async void backupButton_Click(object sender, EventArgs e)
		{
			if (isRunningBackup)
				return;
			try
			{
				backupCancelationTokenSource = new CancellationTokenSource();
				isRunningBackup = true;
				var isDeviceReady = await IsDeviceReady();
				if (!isDeviceReady)
				{
					//TODO: Alert
					return;
				}
				var device = deviceComboBox.SelectedItem as AdbDevice;
				var backupPath = Path.Combine(AdbManager.BackupDirectory, "kodiak.ab");
				var s = await AdbManager.Shared.Backup(device, backupPath, backupCancelationTokenSource.Token);
			}
			finally
			{
				isRunningBackup = false;
			}
		}

		private void toDesktopButton_Click(object sender, EventArgs e)
		{
			ExtractAndroidBackup();

			//Copy Files out. 
			var gameSavePath = Path.Combine(AdbManager.tempDirectory, "apps\\com.Drifter.Kodiak\\f\\UE4Game\\Kodiak\\Kodiak\\Saved\\SaveGames");


			if (Directory.Exists(appDataDirectory))
				Directory.Delete(appDataDirectory, true);
			Directory.Move(gameSavePath, appDataDirectory);

		}

		void ExtractAndroidBackup()
		{
			var backupPath = Path.Combine(AdbManager.BackupDirectory, "kodiak.ab");
			var tarPAth = Path.Combine(AdbManager.BackupDirectory, "kodiak.tar");


		}


		public static string appDataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Kodiak", "Saved", "SaveGames");



		private void syncToQuestButton_Click(object sender, EventArgs e)
		{

			var gameSavePath = Path.Combine(AdbManager.tempDirectory, "apps\\com.Drifter.Kodiak\\f\\UE4Game\\Kodiak\\Kodiak\\Saved\\SaveGames");
			Directory.CreateDirectory(gameSavePath);
			foreach (var file in Directory.GetFiles(appDataDirectory))
				File.Copy(file, Path.Combine(gameSavePath, Path.GetFileName(file)), true);
			//Now lets zip it up!
			var source = Path.Combine(AdbManager.tempDirectory, "apps");
			var output = Path.Combine(AdbManager.tempDirectory, "appBackup.tar.gz");

			BackupManager.Shared.CreateBackupFromFolder(source, output);

			//CreateTarGZ(output, source);

		}
		static readonly byte[] backupHeader = new byte[]{ 0x41, 0x4E, 0x44 , 0x52 , 0x4F , 0x49 , 0x44 , 0x20 , 0x42 , 0x41 , 0x43 , 0x4B , 0x55 , 0x50 , 0x0A , 0x34 , 0x0A , 0x30 , 0x0A , 0x6E , 0x6F , 0x6E , 0x65 , 0x0A};
		private void CreateTarGZ(string tgzFilename, string sourceDirectory)
		{
			Stream outStream = File.Create(tgzFilename);
			outStream.Write(backupHeader, 0, backupHeader.Length);

			//var deflater = new Deflater()
			//var gzoStream = new DeflaterOutputStream(outStream);
			TarArchive tarArchive = TarArchive.CreateOutputTarArchive(outStream);
			

			// Note that the RootPath is currently case sensitive and must be forward slashes e.g. "c:/temp"
			// and must not end with a slash, otherwise cuts off first char of filename
			// This is scheduled for fix in next release
			tarArchive.RootPath = Path.GetDirectoryName(sourceDirectory).Replace('\\', '/');
			//TarEntry tarEntry = TarEntry.CreateTarEntry("apps/");
			//tarArchive.WriteEntry(tarEntry, false);



			//Find the app manifest first!
			var manifest = Directory.GetFiles(sourceDirectory, "_manifest", SearchOption.AllDirectories).FirstOrDefault();


			var manifestEntry =  TarEntry.CreateEntryFromFile(manifest);
			tarArchive.WriteEntry(manifestEntry, true);

			var fFolder = Path.Combine(Path.GetDirectoryName(manifest),"f");
			if(Directory.Exists(fFolder))
			{
				AddDirectoryFilesToTar(tarArchive, fFolder, true);
			}

			fFolder = Path.Combine(Path.GetDirectoryName(manifest), "sp");
			if (Directory.Exists(fFolder))
			{
				AddDirectoryFilesToTar(tarArchive, fFolder, true);
			}
			//Then we upload /f

			tarArchive.Close();
		}

		private void AddDirectoryFilesToTar(TarArchive tarArchive, string sourceDirectory, bool recurse)
		{
			// Optionally, write an entry for the directory itself.
			// Specify false for recursion here if we will add the directory's files individually.
			TarEntry tarEntry = TarEntry.CreateEntryFromFile(sourceDirectory);
			tarArchive.WriteEntry(tarEntry, false);

			// Write each file to the tar.
			string[] filenames = Directory.GetFiles(sourceDirectory);
			foreach (string filename in filenames)
			{
				tarEntry = TarEntry.CreateEntryFromFile(filename);
				if(tarEntry.TarHeader.TypeFlag == 76)
				{
					Console.WriteLine("IDK...");
				}
				else
					tarArchive.WriteEntry(tarEntry, true);
			}

			if (recurse)
			{
				string[] directories = Directory.GetDirectories(sourceDirectory);
				foreach (string directory in directories)
					AddDirectoryFilesToTar(tarArchive, directory, recurse);
			}
		}

		const string ammoStartWord = "Health";
		const string ammoEndWord = "LoadedLevelNames";
		const string buildoutStartWord = "HolsteredActors";
		const string buildoutEndWord = "MasterMapProgression";

		private void button3_Click(object sender, EventArgs e)
		{
			//This will copy out all the stuff you are carying and save it.
			const int gameSaveIndex = 2;
			//Save out the important stuff from gameSave
			var gameSave = Path.Combine(appDataDirectory, $"Save_{gameSaveIndex}.sav");

			var text = File.ReadAllText(gameSave);
			var ammoStart = text.IndexOf(ammoStartWord);
			var ammoEnd = text.IndexOf(ammoEndWord, ammoStart);
			var ammoLength = ammoEnd - ammoStart;


			var buildoutStart = text.IndexOf(buildoutStartWord, ammoEnd);
			var buildoutEnd = text.IndexOf(buildoutEndWord, ammoStart);
			var buildLength = buildoutEnd - buildoutStart;


			var ammoutDataPath = Path.Combine(AdbManager.tempDirectory, "ammoData");
			var buildOutDataPath = Path.Combine(AdbManager.tempDirectory, "buildOutData");
			using (var file = File.OpenRead(gameSave))
			{
				using (var ammoFile = File.Create(ammoutDataPath))
				{
					var data = new byte[ammoLength];
					file.Position = ammoStart;
					var read = file.Read(data, 0, ammoLength);
					ammoFile.Write(data, 0, read);
				}
				using (var buildFile = File.Create(buildOutDataPath))
				{
					var data = new byte[buildLength];
					file.Position = buildoutStart;
					var read = file.Read(data, 0, buildLength);
					buildFile.Write(data, 0, read);
				}
			}
			
			
		}

		private void button2_Click(object sender, EventArgs e)
		{
			const int gameSaveIndex = 1;
			//Save out the important stuff from gameSave
			var gameSave = Path.Combine(appDataDirectory, $"Save_{gameSaveIndex}.sav");

			var text = File.ReadAllText(gameSave);
			var ammoStart = text.IndexOf(ammoStartWord);
			var ammoEnd = text.IndexOf(ammoEndWord, ammoStart);
			var ammoLength = ammoEnd - ammoStart;


			var buildoutStart = text.IndexOf(buildoutStartWord, ammoEnd);
			var buildoutEnd = text.IndexOf(buildoutEndWord, ammoStart);
			var buildLength = buildoutEnd - buildoutStart;


			var ammoutDataPath = Path.Combine(AdbManager.tempDirectory, "ammoData");
			var buildOutDataPath = Path.Combine(AdbManager.tempDirectory, "buildOutData");

			var ammoData = File.ReadAllBytes(ammoutDataPath);
			var buildOutData = File.ReadAllBytes(buildOutDataPath);
			var saveData = File.ReadAllBytes(gameSave);

			using (var file = File.Create(gameSave))
			{
				file.Write(saveData, 0, ammoStart);

				file.Write(ammoData, 0, ammoData.Length);

				file.Write(saveData, ammoEnd, buildoutStart - ammoEnd);

				file.Write(buildOutData, 0, buildOutData.Length);
				file.Write(saveData, buildoutEnd, saveData.Length - buildoutEnd);
			}
		}
	}
}
