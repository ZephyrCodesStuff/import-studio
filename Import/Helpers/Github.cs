using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Octokit;

using Import.Core;

namespace Import.Helpers {
    public static class Github {
        static GitHubClient _client = null;
        static GitHubClient Client => _client
            ?? (_client = new GitHubClient(new ProductHeaderValue("mat1jaczyyy-import-studio")));

        static RepositoryContent blogpost = null;
        static Release release = null;
        static ReleaseAsset download = null;
        static string avalonia = "";

        public static bool UpdateChecked = false;

        public static async Task<RepositoryContent> LatestBlogpost() {
            if (blogpost == null) {
                blogpost = (
                    await Client.Repository.Content.GetAllContentsByRef(
                        "mat1jaczyyy", "import-studio-blog", 
                        (await Client.Repository.Content.GetAllContents("mat1jaczyyy", "import-studio-blog")).Last().Name,
                        "master"
                    )
                ).Last();
            }

            return blogpost;
        }

        public static async Task<Release> LatestRelease() {
            if (release == null) {
                release = (
                    await Client.Repository.Release.GetAll("mat1jaczyyy", "import-studio")
                ).First(i => i.Prerelease == false);
                
                download = RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                    ? null
                    : release.Assets.FirstOrDefault(i => i.Name.Contains(
                        RuntimeInformation.IsOSPlatform(OSPlatform.Windows)? "Win.zip" : "Mac.zip"
                    ));
            }

            return release;
        }

        public static async Task<ReleaseAsset> LatestDownload() {
            if (release == null)
                try {
                    await LatestRelease();
                } catch {
                    return null;
                }

            return download;
        }

        public static async Task<bool> ShouldUpdate() {
            if (release == null)
                try {
                    await LatestRelease();
                } catch {
                    return false;
                }

            return Preferences.CheckForUpdates && release.Name != Program.Version && download != null;
        }

        static readonly string DepsPath = $"{AppDomain.CurrentDomain.BaseDirectory}Import.deps.json";

        public static string AvaloniaVersion() {
            if (avalonia == "" && File.Exists(DepsPath)) {
                try {
                    using (StreamReader file = File.OpenText(DepsPath))
                        using (JsonTextReader reader = new JsonTextReader(file))
                            while (reader.Read())
                                if (reader.TokenType == JsonToken.String &&
                                    reader.Path.StartsWith("targets['.NETCoreApp,Version=v5.0") &&
                                    reader.Path.EndsWith("']['Import/1.0.0'].dependencies.Avalonia")) {
                                        
                                    avalonia = (string)reader.Value;
                                    break;
                                }
                } catch {
                    avalonia = "";
                }
            }

            return avalonia;
        }
    }
}