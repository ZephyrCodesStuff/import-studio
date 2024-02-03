using System;

using DiscordRPC;

using Import.Structures;

namespace Import.Core {
    public static class Discord {
        static bool Initialized = false;
        static DiscordRpcClient Presence;
        static Timestamps Time;
        
        static Courier courier;
        static object locker = new object();

        public static void Set(bool state) {
            if (state) Init();
            else Dispose();
        }

        static void Init() {
            if (Initialized) return;

            Presence = new DiscordRpcClient("1203476277894324264");
            Presence.Initialize();
            Time = new Timestamps(DateTime.UtcNow);
            Initialized = true;

            Refresh();

            courier = new Courier(1000, _ => Refresh(), repeat: true);
        }

        static void Refresh() {
            lock (locker) {
                if (!Initialized) return;

                RichPresence Info = new RichPresence() {
                    Details = Program.Version,
                    Timestamps = Time,
                    Assets = new Assets() {
                        LargeImageKey = "logo"
                    }
                };

                if (Preferences.DiscordFilename && Program.Project != null) {
                    string s = "Working on " + ((Program.Project.FilePath == "")? "a new Project" : Program.Project.FileName);
                    Info.State = (s.Length > 128)? s.Substring(0, 125) + "..." : s;
                }

                Presence.SetPresence(Info);
            }
        }

        static void Dispose() {
            if (!Initialized) return;

            lock (locker) {
                courier.Dispose();

                Initialized = false;
                Presence.ClearPresence();
                Presence.Dispose();
            }
        }
    }
}