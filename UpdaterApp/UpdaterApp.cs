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
        static StreamWriter logWriter;

        static void Log(string message)
        {
            var logMessage = $"[{DateTime.Now:HH:mm:ss}] {message}";
            Console.WriteLine(logMessage);
            try
            {
                logWriter?.WriteLine(logMessage);
                logWriter?.Flush();
            }
            catch { }
        }
        static void Main(string[] args)
        {
            try
            {
                // Создаём лог файл
                var logPath = Path.Combine(AppContext.BaseDirectory, "updater_log.txt");
                logWriter = new StreamWriter(logPath, false);

                Log("=== Updater Started ===");
                Log($"Arguments count: {args.Length}");
                for (int i = 0; i < args.Length; i++)
                {
                    Log($"Arg[{i}]: {args[i]}");
                }

                if (args.Length < 2)
                {
                    Log("ERROR: Not enough arguments");
                    Log("Usage: updater.exe <zip_url> <target_directory>");
                    Console.ReadKey();
                    return;
                }

                var zipUrl = args[0];
                var targetDir = args[1].TrimEnd('\\');

                Log($"Zip URL: {zipUrl}");
                Log($"Target Dir: {targetDir}");

                var zipPath = Path.Combine(targetDir, "update.zip");
                var tempDir = Path.Combine(targetDir, "update_extracted");

                Log($"Zip path: {zipPath}");
                Log($"Temp dir: {tempDir}");

                // Задержка для закрытия основного приложения
                Log("Waiting 2 seconds for main app to close...");
                Thread.Sleep(2000);

                Log("Downloading update...");
                var wc = new WebClient();
                wc.DownloadFile(zipUrl, zipPath);
                Log($"Downloaded to: {zipPath}");

                if (Directory.Exists(tempDir))
                {
                    Log($"Deleting old temp dir: {tempDir}");
                    Directory.Delete(tempDir, true);
                }

                Log("Extracting archive...");
                ZipFile.ExtractToDirectory(zipPath, tempDir);

                var extractedFiles = Directory.GetFiles(tempDir, "*", SearchOption.AllDirectories);
                Log($"Extracted {extractedFiles.Length} files");

                Log("Starting file copy...");
                int successCount = 0;
                int failCount = 0;

                foreach (var file in extractedFiles)
                {
                    var relative = file.Substring(tempDir.Length + 1);
                    var destPath = Path.Combine(targetDir, relative);

                    var fileName = Path.GetFileName(file).ToLowerInvariant();

                    // Пропускаем UpdaterApp.exe (он не может обновить сам себя)
                    if (fileName == "updaterapp.exe" || fileName == "updater.exe")
                    {
                        Log($"Skipping: {relative} (updater cannot update itself)");
                        continue;
                    }

                    Log($"Processing: {relative}");
                    Log($"  Source: {file}");
                    Log($"  Dest: {destPath}");

                    try
                    {
                        var destDir = Path.GetDirectoryName(destPath);
                        if (!Directory.Exists(destDir))
                        {
                            Directory.CreateDirectory(destDir);
                            Log($"  Created dir: {destDir}");
                        }

                        File.Copy(file, destPath, true);
                        Log($"  ✓ Copied successfully");
                        successCount++;
                    }
                    catch (Exception ex)
                    {
                        Log($"  ✗ FAILED: {ex.GetType().Name}: {ex.Message}");
                        failCount++;
                    }
                }

                Log($"Copy complete: {successCount} success, {failCount} failed");

                // Ищем .exe для запуска
                Log("Looking for main exe...");
                var exeFiles = Directory.GetFiles(targetDir, "*.exe");
                Log($"Found {exeFiles.Length} exe files");

                foreach (var e in exeFiles)
                {
                    Log($"  - {Path.GetFileName(e)}");
                }

                var exe = exeFiles.FirstOrDefault(f =>
                {
                    var name = Path.GetFileNameWithoutExtension(f).ToLowerInvariant();
                    return name != "update" && name != "updater" && !name.Contains("update");
                });

                if (exe != null)
                {
                    Log($"Starting: {exe}");
                    Thread.Sleep(1000);
                    System.Diagnostics.Process.Start(exe);
                    Log("App started");
                }
                else
                {
                    Log("WARNING: No main exe found to restart");
                }

                // Cleanup
                Log("Cleaning up...");
                if (Directory.Exists(tempDir))
                {
                    Directory.Delete(tempDir, true);
                    Log("Temp dir deleted");
                }

                if (File.Exists(zipPath))
                {
                    File.Delete(zipPath);
                    Log("Zip file deleted");
                }

                Log("=== Update Complete ===");
            }
            catch (Exception ex)
            {
                Log($"FATAL ERROR: {ex.GetType().Name}: {ex.Message}");
                Log($"Stack trace: {ex.StackTrace}");
                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey();
            }
            finally
            {
                logWriter?.Close();
            }
        }
    }
}
