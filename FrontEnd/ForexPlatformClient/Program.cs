using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Reflection;
using System.Threading;
using System.IO;
using System.Diagnostics;

namespace ForexPlatformClient
{
    static class Program
    {
        static string[] RegistrationToGACAssemblies = new string[]
        {
            "Arbiter",
            "CommonSupport",
            "CommonFinancial",
            "ForexPlatform",
            "ForexPlatformPersistence",
            "MT4Adapter",
            "TA-Lib-Core"
        };

        static string SQLiteX64FileName = "System.Data.SQLite.x64.DLL";
        static string SQLiteX86FileName = "System.Data.SQLite.x86.DLL";
        static string SQLiteWorkingFileName = "System.Data.SQLite.DLL";


        /// <summary>
        /// Unregister any leftovers from previous namings.
        /// </summary>
        static string[] AdditionalUnRegistrationToGACAssemblies = new string[]
        {
            "ForexPlatformIntegration",
        };

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                if (ForexPlatformClient.Properties.Settings.Default.AutoRegisterReferencedAssembliesToGAC)
                {
                    UnregisterReferencedAssembliesFromGAC();

                    if (RegisterReferencedAssembliesToGAC() == false)
                    {
                        if (MessageBox.Show("Registration of used assemblies to GAC has failed. \r\nWould you like to continue? \r\n(This may cause problems with some integrations)", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                        {
                            return;
                        }
                    }
                }

                StartApplication();
            }
            else
            {
                if (args[0].ToLower() == "NoStart".ToLower())
                {
                    return;
                }
                else if (args[0].ToLower() == "Unregister".ToLower())
                {
                    UnregisterReferencedAssembliesFromGAC();
                    MessageBox.Show("Assemblies unregistered.", "Forex Platform Client", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (args[0].ToLower() == "SilentUnregister".ToLower())
                {
                    UnregisterReferencedAssembliesFromGAC();
                    //Console.WriteLine("Assemblies unregisted silently.");
                }
                else if (args[0].ToLower() == "Register".ToLower())
                {
                    UnregisterReferencedAssembliesFromGAC();
                    Thread.Sleep(500);
                    if (RegisterReferencedAssembliesToGAC())
                    {
                        MessageBox.Show("Assemblies registered successfully.", "Forex Platform Client", MessageBoxButtons.OK);
                    }
                    else
                    {
                        MessageBox.Show("Error: Assemblies registration failed.", "Forex Platform Client", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else if (args[0].ToLower() == "SilentRegister".ToLower())
                {
                    UnregisterReferencedAssembliesFromGAC();
                    Thread.Sleep(500);
                    if (RegisterReferencedAssembliesToGAC())
                    {
                        //Console.WriteLine("Assemblies unregisted and registed silently.");
                    }
                    else
                    {
                        //Console.WriteLine("Assemblies unregisted but registration had errors.");
                    }
                }
            }
        }

        /// <summary>
        /// Launch the main application.
        /// </summary>
        static void StartApplication()
        {
            try
            {
                if (IsRunningOn64bOS())
                {// 64b OS.
                    File.Copy(SQLiteX64FileName, SQLiteWorkingFileName, true);
                }
                else
                {// 32b OS.
                    File.Copy(SQLiteX86FileName, SQLiteWorkingFileName, true);
                }
            }
            catch (Exception ex)
            {
            }
            
            ProcessStartInfo startInfo = new ProcessStartInfo("ForexPlatformFrontEnd.exe", "ManagedLaunch");
            //startInfo.UseShellExecute = true;
            //startInfo.Verb = "runas";
            //startInfo.Arguments = "/env /user: Administrator cmd";

            System.Diagnostics.Process.Start(startInfo);
        }

        static bool RegisterReferencedAssembliesToGAC()
        {
            List<string> assemblies = new List<string>(RegistrationToGACAssemblies);

            bool result = true;
            foreach (string assemblyName in assemblies)
            {
                string name = assemblyName;

                // This is a signed assembly, so unregister it to GAC.
                if (GACHelper.AddAssemblyToCache(assemblyName + ".dll") != 0)
                {// Error in registration.
                    UnregisterReferencedAssembliesFromGAC();

                    Console.WriteLine("ERROR - registration to GAC failed [" + assemblyName + "].");

                    result = false;
                    continue;
                }
                else
                {
                    Console.WriteLine("Registered to GAC [" + assemblyName + "].");
                }
            }

            return result;
        }

        static void UnregisterReferencedAssembliesFromGAC()
        {
            List<string> assemblies = new List<string>(RegistrationToGACAssemblies);
            assemblies.AddRange(AdditionalUnRegistrationToGACAssemblies);

            foreach (string assemblyName in assemblies)
            {
                string name = assemblyName;

                // This is a signed assembly, so unregister it to GAC.
                int intResult = GACHelper.RemoveAssemblyFromCache(assemblyName);

                Console.WriteLine("Unregistered from GAC [" + assemblyName + "], result [" + intResult + "].");
            }
        }

        /// <summary>
        /// Helper, Is the current OS Windows.
        /// </summary>
        /// <returns></returns>
        static bool IsRunningOnWindows()
        {
            OperatingSystem osInfo = Environment.OSVersion;
            return osInfo.Platform == PlatformID.Win32NT || osInfo.Platform == PlatformID.Win32S
                || osInfo.Platform == PlatformID.Win32Windows;
        }

        /// <summary>
        /// Helper, check to see if you are running on a 32b or a 64b OS.
        /// </summary>
        /// <returns></returns>
        static bool IsRunningOn64bOS()
        {
            return IntPtr.Size == 8;
        }

    }
}
