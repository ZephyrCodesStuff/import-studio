using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

using Import.Core;
using Import.Windows;

namespace Import.Helpers {
    public static class DriverChecker {
        static readonly string[] Drivers = new [] { "novationusbmidi.inf", "nvnusbaudio.inf" };
        static readonly int[] BuildPositions = new [] { 2, 3 };

        class DriverVersion: IComparable {
            public int Minor { get; private set; }
            public int Build { get; private set; }

            public DriverVersion(int[] version) {
                Minor = version[0];
                Build = version[1];
            }

            public int CompareTo(object obj) {
                DriverVersion that = (DriverVersion)obj;

                if (this.Minor == that.Minor) {
                    if (this.Build == that.Build) return 0;
                    return (this.Build < that.Build)? -1 : 1;
                }
                
                return (this.Minor < that.Minor)? -1 : 1;
            }

            public static bool operator <(DriverVersion a, DriverVersion b)
                => a.CompareTo(b) == -1;

            public static bool operator >(DriverVersion a, DriverVersion b) 
                => a.CompareTo(b) == 1;
        }

        static MessageWindow CreateDriverError(bool IsOldVersion) {
            MessageWindow ret = new MessageWindow(
                (IsOldVersion
                    ? "Import Studio requires a newer version of the Novation USB Driver than is\n" +
                      "installed on your computer.\n\n"
                    : "Import Studio requires the Novation USB Driver which isn't installed on your\n" +
                      "computer.\n\n"
                ) +
                "Please install at least version 2.17.10 of the driver before launching Import\n" +
                "Studio.",
                new string[] {"Download Driver", "OK"}
            );

            ret.Completed.Task.ContinueWith(result => {
                if (result.Result == "Download Driver")
                    App.URL("https://fael-downloads-prod.focusrite.com/customer/prod/s3fs-public/downloads/NovationUsbMidi_2.17.10.284_Installer.exe");
            });

            return ret;
        }

        static IEnumerable<DriverVersion> GetDrivers() {
            string[] directories = Directory.GetDirectories(Environment.ExpandEnvironmentVariables(@"%SystemRoot%\System32\DriverStore\FileRepository\"));
            IEnumerable<DriverVersion> ret = Enumerable.Empty<DriverVersion>();
            
            foreach (var (driver, build) in Drivers.Zip(BuildPositions))
                ret = ret.Concat(
                    directories.Where(i => Path.GetFileName(i).StartsWith(driver))
                        .Select(j => new DriverVersion(
                            File.ReadAllLines(Path.Combine(j, driver))
                                .Where(i => i.StartsWith("DriverVer="))
                                .First().Substring(10).Split(',')[1].Split('.')
                                .Where((x, i) => i == 1 || i == build)
                                .Select(x => Convert.ToInt32(x))
                                .ToArray()
                        ))
                );

            return ret;
        }

        public static bool Run(out MessageWindow error) {
            error = null;

            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return true;

            IEnumerable<DriverVersion> drivers = GetDrivers();

            if (drivers.Count() == 0) {
                error = CreateDriverError(false);
                return false;
            }

            if (drivers.Max() < new DriverVersion(new int[] {17, 10})) { // 2.17.10 required
                error = CreateDriverError(true);
                return false;
            }

            return true;
        }
    }
}