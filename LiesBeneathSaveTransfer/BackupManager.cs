using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using SharpCompress.Common;
using SharpCompress.Writers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiesBeneathSaveTransfer
{
	public class BackupManager
	{
		public static readonly byte[] TarHeader = { 0x1F, 0x8B, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00 };
		public static BackupManager Shared { get; set; } = new BackupManager();


		public Task<bool> Extract(string backup, string outputDirecory) => Task.Run(() => _Extract(backup, outputDirecory));
		bool _Extract(string backup, string outputDirecory)
		{

			try
			{
				var tarPAth = $"{backup}.tar.gz";
				if (File.Exists(tarPAth))
					File.Delete(tarPAth);

				var outStream = File.Open(tarPAth, FileMode.OpenOrCreate);
				foreach (var b in BackupManager.TarHeader)
					outStream.WriteByte(b);
				var fileStream = File.OpenRead(backup);
				fileStream.Position = 24;
				fileStream.CopyTo(outStream);
				fileStream.Close();
				outStream.Position = 0;


				//TarArchive tarArchive = TarArchive.CreateInputTarArchive(outStream);
				//tarArchive.ExtractContents(AdbManager.tempDirectory);
				//tarArchive.Close();
				var targetDir = AdbManager.tempDirectory;

				using (var fsIn = new GZipInputStream(outStream))

				{
					TarInputStream tarIn = new TarInputStream(fsIn);
					TarEntry tarEntry;
					try
					{
						while ((tarEntry = tarIn.GetNextEntry()) != null)
						{
							if (tarEntry.IsDirectory)
								continue;


							// Converts the unix forward slashes in the filenames to windows backslashes
							string name = tarEntry.Name.Replace('/', Path.DirectorySeparatorChar);
							if (name.Contains(":"))
								continue;


							// Remove any root e.g. '\' because a PathRooted filename defeats Path.Combine
							if (Path.IsPathRooted(name))
								name = name.Substring(Path.GetPathRoot(name).Length);

							// Apply further name transformations here as necessary
							string outName = Path.Combine(targetDir, name);

							string directoryName = Path.GetDirectoryName(outName);
							//if (!directoryName.Contains("SaveGames"))
							//	continue;

							// Does nothing if directory exists
							Directory.CreateDirectory(directoryName);

							FileStream outStr = new FileStream(outName, FileMode.Create);

							//if (asciiTranslate)
							//	CopyWithAsciiTranslate(tarIn, outStr);
							//else
							tarIn.CopyEntryContents(outStr);

							outStr.Close();

							// Set the modification date/time. This approach seems to solve timezone issues.
							DateTime myDt = DateTime.SpecifyKind(tarEntry.ModTime, DateTimeKind.Utc);
							File.SetLastWriteTime(outName, myDt);
						}
					}
					catch (EndOfStreamException)
					{

					}

					tarIn.Close();
				}

				outStream.Close();
				return true;
			}
			catch
			{

			}
			return false;
		}

		public static readonly byte[] backupHeader = new byte[] { 0x41, 0x4E, 0x44, 0x52, 0x4F, 0x49, 0x44, 0x20, 0x42, 0x41, 0x43, 0x4B, 0x55, 0x50, 0x0A, 0x34, 0x0A,
			//0x30 //Means No Compression
			0x31, //Means Compressed
			
			0x0A, 0x6E, 0x6F, 0x6E, 0x65, 0x0A };
		public bool CreateBackupFromFolder(string source, string output)
		{

			Stream outStream = File.Create(output);
			//outStream.Write(backupHeader, 0, backupHeader.Length);

			var root = Path.GetDirectoryName(source);

			Func<string, string> cleanesPath = (string inPath) => inPath.Replace(root, "");

			using (var writer = WriterFactory.Open(outStream, ArchiveType.Tar, CompressionType.GZip))
			{


				Action<IWriter, string> writeAllFiles = (w, directory) =>
				 {
					 var files = Directory.GetFiles(directory, "*", SearchOption.AllDirectories);
					 writer.Write(cleanesPath(directory), directory);
					 foreach(var f in files)
					 {
						 writer.Write(cleanesPath(f), new FileInfo(f));
					 }
				 };

				//Find the app manifest first!
				var manifest = Directory.GetFiles(source, "_manifest", SearchOption.AllDirectories).FirstOrDefault();

				writer.Write(cleanesPath(manifest), new FileInfo(manifest));

				var fFolder = Path.Combine(Path.GetDirectoryName(manifest), "f");

				if (Directory.Exists(fFolder))
				{

					writeAllFiles(writer, fFolder);
					//AddDirectoryFilesToTar(tarArchive, fFolder, true);
					//writer.WriteAll(directory: fFolder, "*", option: SearchOption.AllDirectories);
				}

				fFolder = Path.Combine(Path.GetDirectoryName(manifest), "sp");
				if (Directory.Exists(fFolder))
				{
					writeAllFiles(writer, fFolder);
					//writer.WriteAll(directory: fFolder, "*", option: SearchOption.AllDirectories);
				}

				//writer.WriteAll(filesPath, "*", SearchOption.AllDirectories);
			}
			outStream.Dispose();
			//Zlib
			return true;
		}
	}
}
