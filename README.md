# AutoUpdater

## English 🇺🇸
**AutoUpdater** is a library (.dll) and a console application (.exe) designed to simplify updating your .NET applications from GitHub Releases.
- 📚 Library (UpdaterLib.dll) – provides functionality to check GitHub releases and determine if a newer version of your application is available.
- 🖥️ Console application (UpdaterApp.exe) – downloads the latest release, extracts it, and replaces old files with updated ones using two parameters:
  `UpdaterApp.exe <zip_url> <target_directory>`

### Features
- Check for the latest release on GitHub
- Download and extract .zip releases
- Safely replace files in the target directory
- Automatically restart your main application after update

### ⚙️ Requirements
- .NET Framework 4.7.2
- Internet connection to access GitHub releases

### 📝 Usage
1. Add the library to your project and reference UpdaterLib.dll.
2. Check for updates:
```charp
var updater = new AppUpdater("owner_name", "repository_name");
var update = await updater.UpdateAsync();

if (update != null)
{
    Console.WriteLine($"New version available: {update.Latest}");
    // Optionally launch UpdaterApp.exe to apply update
}
```
4. Launch the console updater from your application to apply the update:
```charp
Process.Start(new ProcessStartInfo
{
    FileName = "UpdaterApp.exe",
    Arguments = $"\"{update.DownloadUrl}\" \"{AppContext.BaseDirectory}\"",
    CreateNoWindow = true,
    UseShellExecute = false
});
```

### Notes
- The updater downloads the .zip release from GitHub and extracts it relative to the folder where your application resides.
- Ensure that no files in the target directory are locked by other processes while updating.
___

## Русский 🇷🇺
**AutoUpdater** это библиотека (.dll) и консольное приложение (.exe) создана для простого обновления приложений на .NET Framework через Github Releases.
- 📚 Библиотека (UpdaterLib.dll) – предоставляет функционал для проверки релизов на GitHub и определения наличия более новой версии приложения.
- 🖥️ Консольное приложение (UpdaterApp.exe) – скачивает последний релиз, распаковывает его и заменяет старые файлы обновлёнными, используя два параметра:
  `UpdaterApp.exe <zip_url> <target_directory>`

### Функции
- Проверка последнего релиза на Github
- Скачивание и распаковка .zip релиза
- Безопасная замена файлов в целевом каталоге
- Автоматический перезапуск основного приложения после обновления

### ⚙️ Требования
- .NET Framework 4.7.2
- Интернет соединение для досутпа к релизам на Github

### 📝 Использование
1. Добавить библиотеку в ваш проект и ссылку на неё UpdaterLib.dll.
2. Проверить обновления основной программы:
```charp
var updater = new AppUpdater("owner_name", "repository_name");
var update = await updater.UpdateAsync();

if (update != null)
{
    Console.WriteLine($"New version available: {update.Latest}");
    // Optionally launch UpdaterApp.exe to apply update
}
```
4. Запустить консольное приложение для обновления основной программы:
```charp
Process.Start(new ProcessStartInfo
{
    FileName = "UpdaterApp.exe",
    Arguments = $"\"{update.DownloadUrl}\" \"{AppContext.BaseDirectory}\"",
    CreateNoWindow = true,
    UseShellExecute = false
});
```

### Примечание
- Программа обновления загружает релиз в формате .zip с GitHub и распаковывает его относительно папки, где находится ваше приложение.
- Убедитесь, что во время обновления никакие файлы в целевом каталоге не заблокированы другими процессами.
