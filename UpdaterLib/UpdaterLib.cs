using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace UpdaterLib
{
    public class UpdateInfo
    {
        public Version Latest { get; }
        public string DownloadUrl { get; }

        public UpdateInfo(Version Latest, string DownloadUrl)
        {
            this.Latest = Latest;
            this.DownloadUrl = DownloadUrl;
        }
    }
    public class AppUpdater
    {
        private readonly string _owner;
        private readonly string _repo;
        public AppUpdater(string owner, string repo)
        {
            _owner = owner;
            _repo = repo;
        }

        public async Task<UpdateInfo> UpdateAsync()
        {
            Version current = Assembly.GetEntryAssembly().GetName().Version;

            using var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Updater");

            string json;
            try
            {
                json = await client.GetStringAsync($"https://api.github.com/repos/{_owner}/{_repo}/releases/latest");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return null;
            }

            var obj = JObject.Parse(json);

            var latestTag = (string)obj["tag_name"];
            if (string.IsNullOrWhiteSpace(latestTag)) return null;

            var latestVersion = new Version(latestTag.TrimStart('v'));
            if (latestVersion <= current) return null;

            var assets = obj["assets"] as JArray;
            if (assets == null || assets.Count == 0) return null;

            var zipUrl = (string)assets[0]["browser_download_url"];

            return new UpdateInfo(latestVersion, zipUrl);
        }
    }
}
