using System;
using System.Configuration;
using System.IO;

namespace ConnectionStringEncrypter
{
    /// <summary>
    /// Encrypts the connection string part of app.config for IremEczOtomasyonu setup builds. The application is run 
    /// as administrator during setup. This means, the connection string is first installed unencrypted. 
    /// It may seem like a security problem but this level of security is enough for the application.
    /// </summary>
    class Program
    {
        // ReSharper disable UnusedParameter.Local
        static void Main(string[] args)
        // ReSharper restore UnusedParameter.Local
        {
            try
            {
                //Opens the specified client configuration file as a Configuration object 
                System.Reflection.Assembly a = System.Reflection.Assembly.GetEntryAssembly();
                // ReSharper disable AssignNullToNotNullAttribute
                string iremEczExePath = Path.Combine(Path.GetDirectoryName(a.Location), "IremEczOtomasyonu.exe");
                // ReSharper restore AssignNullToNotNullAttribute
                Configuration config = ConfigurationManager.OpenExeConfiguration(iremEczExePath);

                // Get the connectionStrings section. 
                ConfigurationSection section = config.GetSection("connectionStrings");

                //Ensures that the section is not already protected 
                if (!section.SectionInformation.IsProtected)
                {
                    //Uses the Windows Data Protection API (DPAPI) to encrypt the 
                    //configuration section using a machine-specific secret key 
                    section.SectionInformation.ProtectSection("DataProtectionConfigurationProvider");
                    config.Save();
                }

            }
            catch (Exception e)
            {
                using (StreamWriter writer = new StreamWriter(
                    Path.Combine(Path.GetTempPath(), "IremEczOtomasyonuInstallationOutput.txt"), true))
                {
                    writer.Write(e);
                    writer.Close();
                }
                throw;
            }
        }
    }
}
