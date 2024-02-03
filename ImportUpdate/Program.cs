﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

using Microsoft.Win32;

namespace ImportUpdate {
    class Program {
        static string GetBaseFolder(string folder) => Path.Combine(
            Directory.GetParent(
                Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory)
            ).FullName,
            folder
        );

        static string Handle64Path => $"{AppDomain.CurrentDomain.BaseDirectory}handle64.exe";

        public static readonly string UserPath = Path.Combine(Environment.GetEnvironmentVariable(
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows)? "USERPROFILE" : "HOME"
        ), ".importstudio");

        public static readonly string CrashDir = Path.Combine(Program.UserPath, "Crashes");

        static void Main(string[] args) {
            AppDomain.CurrentDomain.UnhandledException += (object sender, UnhandledExceptionEventArgs e) => {
                if (!Directory.Exists(CrashDir)) Directory.CreateDirectory(CrashDir);
                string crashName = Path.Combine(CrashDir, $"Crash-{DateTimeOffset.Now.ToUnixTimeSeconds()}");

                using (MemoryStream memoryStream = new MemoryStream()) {
                    using (ZipArchive archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                        using (Stream log = archive.CreateEntry("exception.log").Open())
                            using (StreamWriter writer = new StreamWriter(log))
                                writer.Write(
                                    $"CWD: {AppDomain.CurrentDomain.BaseDirectory}\n\n" +
                                    $"Operating System: {RuntimeInformation.OSDescription}\n\n" +
                                    e.ExceptionObject.ToString()
                                );

                    File.WriteAllBytes(crashName + ".zip", memoryStream.ToArray());
                }
            };

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                throw new InvalidOperationException("Auto-updating is not supported on Linux");
            
            string temppath = Program.GetBaseFolder("Temp");

            if (!Directory.Exists(temppath)) return;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE", true);
                if (!key.GetSubKeyNames().Contains("Sysinternals"))
                    key.CreateSubKey("Sysinternals");
                key.Flush();
                
                key = key.OpenSubKey("Sysinternals", true);
                if (!key.GetSubKeyNames().Contains("Handle"))
                    key.CreateSubKey("Handle");
                key.Flush();
                
                key = key.OpenSubKey("Handle", true);
                key.SetValue("EulaAccepted", 1);
                key.Close();
            }

            Thread.Sleep(2000);
            
            string importpath = Program.GetBaseFolder("Import");

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                Process handle64 = Process.Start(new ProcessStartInfo(Handle64Path, "-p ImportUpdate.exe -nobanner") {
                    RedirectStandardOutput = true
                });
                handle64.WaitForExit();
                
                IEnumerable<string> strings = handle64.StandardOutput.ReadToEnd().Split('\n');

                string pid = strings.FirstOrDefault(i => i.Contains("pid"));
                string handle = strings.FirstOrDefault(i => i.Contains(importpath));

                if (handle != null)
                    Process.Start(new ProcessStartInfo(Handle64Path, $"-p {pid.Trim().Split(' ')[2]} -c {handle.Trim().Split(':')[0]} -y -nobanner")).WaitForExit();
            }

            Thread.Sleep(1000);

            if (Directory.Exists(importpath))
                while (true)
                    try {
                        Directory.Delete(importpath, true);
                        break;
                    } catch {
                        Thread.Sleep(1000);
                    }
            
            Directory.Move(temppath, importpath);

            string tempm4lpath = Program.GetBaseFolder("TempM4L");

            if (Directory.Exists(tempm4lpath)) {
                string m4lpath = Program.GetBaseFolder("M4L");
                string[] m4lfiles = Directory.GetFiles(m4lpath).Select(x => Path.GetFileName(x)).ToArray();

                foreach (string newpath in Directory.GetFiles(tempm4lpath)) {
                    string newfile = Path.GetFileName(newpath);
                    
                    if (!m4lfiles.Contains(newfile))
                        File.Copy(newpath, Path.Combine(m4lpath, newfile));
                }

                Directory.Delete(tempm4lpath, true);
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                Process.Start(Path.Combine(importpath, "Import.exe"));
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                Process.Start("open", $"/Applications/Utilities/Terminal.app \"{Path.Combine(importpath, "Import")}\"");
        }
    }
}
