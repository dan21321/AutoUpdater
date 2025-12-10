using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading;

namespace UpdaterApp
{
    internal class UpdaterApp
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: updater.exe <zip_url> <target_directory>");
                return;
            }

            var zipUrl = args[0];
            var targetDir = args[1].TrimEnd('\\');

            var zipPath = Path.Combine(AppContext.BaseDirectory, "update.zip");
            var tempDir = Path.Combine(AppContext.BaseDirectory, "update_extracted");

            try
            {
                var wc = new WebClient();
                wc.DownloadFile(zipUrl, zipPath);

                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, true);

                ZipFile.ExtractToDirectory(zipPath, tempDir);

                foreach (var file in Directory.GetFiles(tempDir, "*", SearchOption.AllDirectories))
                {
                    var relative = file.Substring(tempDir.Length + 1);
                    var destPath = Path.Combine(targetDir, relative);

                    Directory.CreateDirectory(Path.GetDirectoryName(destPath));
                    File.Copy(file, destPath, true);
                }

                Console.WriteLine("Update complete.");

                // ищем .exe для запуска
                var exe = Directory.GetFiles(targetDir, "*.exe")
                    .FirstOrDefault(f =>
                    {
                        var name = Path.GetFileNameWithoutExtension(f).ToLowerInvariant();
                        return name != "update" && name != "updater" && !name.Contains("update");
                    });

                if (exe != null)
                {
                    Console.WriteLine("Restarting app...");
                    Thread.Sleep(1000);
                    System.Diagnostics.Process.Start(exe);
                }
                else
                {
                    Console.WriteLine("No main exe found to restart");
                }

                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, true);
                if (File.Exists(zipPath))
                    File.Delete(zipPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Updater failed: " + ex.Message);
            }
        }
    }
}
