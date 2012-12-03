using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ConnectionStringEncrypter
{
    /// <summary>
    /// Encrypts the connection string part of app.config for IremEczOtomasyonu setup builds.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            // Copy app.config file located at the IremEczOtomasyonu project as web.config file
            System.Reflection.Assembly a = System.Reflection.Assembly.GetEntryAssembly();
            string baseDir = Path.GetDirectoryName(a.Location);
            if (string.IsNullOrEmpty(baseDir))
            {
                return;
            }
            
            string appConfigPath = Path.GetFullPath(
                Path.Combine(baseDir, "..\\..\\..\\IremEczOtomasyonu\\app.config"));
            string webConfigPath = Path.Combine(baseDir, "web.config");
            File.Copy(appConfigPath, webConfigPath, true);

            // Get the path of aspnet_iis executable
            string dotNetInstallationFolder = GetDotNetInstallationFolder();
            string aspNetRegIisPath = Path.Combine(dotNetInstallationFolder, "aspnet_regiis.exe");

            // start aspnet_regiis to encrypt the connection string
            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.FileName = aspNetRegIisPath;
            p.StartInfo.Arguments = "-pef connectionStrings \"" + baseDir + 
                "\" -prov DataProtectionConfigurationProvider";
            p.Start();
            p.WaitForExit();

            // rename the web.config
            string finalConfigPath = Path.Combine(baseDir, "IremEczOtomasyonu.exe.config");
            if (File.Exists(finalConfigPath))
            {
                File.Delete(finalConfigPath);
            }
            File.Move(webConfigPath, finalConfigPath);
        }

        // ReSharper disable InconsistentNaming
        // ReSharper disable UnusedMember.Local
        [Flags]
        enum RuntimeInfo
        {
            UPGRADE_VERSION = 0x01,
            REQUEST_IA64 = 0x02,
            REQUEST_AMD64 = 0x04,
            REQUEST_X86 = 0x08,
            DONT_RETURN_DIRECTORY = 0x10,
            DONT_RETURN_VERSION = 0x20,
            DONT_SHOW_ERROR_DIALOG = 0x40
        }
        // ReSharper restore InconsistentNaming
        // ReSharper restore UnusedMember.Local

        [DllImport("mscoree.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        static extern int GetRequestedRuntimeInfo(string pExe, string pwszVersion, string pConfigurationFile, 
            uint startupFlags, RuntimeInfo runtimeInfoFlags, StringBuilder pDirectory, uint dwDirectory, 
            out uint dwDirectoryLength, StringBuilder pVersion, uint cchBuffer, out uint dwLength);

        /// <summary>
        /// Gets the .NET installation folder
        /// </summary>
        /// <returns></returns>
        private static string GetDotNetInstallationFolder()
        {
            Version envVersion = Environment.Version;
            StringBuilder directory = new StringBuilder(0x200);
            StringBuilder version = new StringBuilder(0x20);
            uint directoryLength;
            uint versionLength;
            int hr = GetRequestedRuntimeInfo(null, "v" + envVersion.ToString(3), null, 0,
                RuntimeInfo.DONT_SHOW_ERROR_DIALOG | RuntimeInfo.UPGRADE_VERSION, directory, (uint)directory.Capacity, 
                out directoryLength, version, (uint)version.Capacity, out versionLength);

            Marshal.ThrowExceptionForHR(hr);
            return Path.Combine(directory.ToString(), version.ToString());
        }
    }
}
