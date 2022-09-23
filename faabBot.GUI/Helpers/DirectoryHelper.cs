using faabBot.GUI.Controllers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace faabBot.GUI.Helpers
{
    public class DirectoryHelper
    {
        public static string GetImageDirectoryLocation(object o)
        {
            var assemblyLocation = o.GetType().Assembly.Location;
            var assemblyNameIndex = assemblyLocation.IndexOf(Globals.AssemblyName);
            return assemblyLocation[..assemblyNameIndex];
        }

        public static string GetMainImageDirectory(object o)
        {
            return string.Format("{0}{1}", GetImageDirectoryLocation(o), Globals.MainImageDirectoryName);
        }

        public static void CreateMainImageDirectory(object o, LogController log)
        {

            System.Security.Principal.SecurityIdentifier sid = new System.Security.Principal.SecurityIdentifier(System.Security.Principal.WellKnownSidType.WorldSid, null);
            System.Security.Principal.NTAccount acct = sid.Translate(typeof(System.Security.Principal.NTAccount)) as System.Security.Principal.NTAccount;
            string strEveryoneAccount = acct.ToString();

            try
            {
                var mainImageDirectory = GetMainImageDirectory(o);
                if (Directory.Exists(mainImageDirectory))
                {
                    return;
                }

                DirectorySecurity securityRules = new();
                securityRules.AddAccessRule(new FileSystemAccessRule(strEveryoneAccount, FileSystemRights.FullControl, AccessControlType.Allow));

                var directoryInfo = Directory.CreateDirectory(mainImageDirectory);
                directoryInfo.SetAccessControl(securityRules);
                
                log.NewLogCreatedEvent(string.Format("Maindirectory added at: {0}", directoryInfo.FullName), DateTime.Now);
            }
            catch
            {
                log.NewLogCreatedEvent("Error: Could not create maindirectory", DateTime.Now);
            }
        }

        public static string? CreateSubImageDirectory(MainWindow mainWindow, LogController log)
        {
            var ci = CultureInfo.InvariantCulture;
            string? subDirectoryName;

            System.Security.Principal.SecurityIdentifier sid = new System.Security.Principal.SecurityIdentifier(System.Security.Principal.WellKnownSidType.WorldSid, null);
            System.Security.Principal.NTAccount acct = sid.Translate(typeof(System.Security.Principal.NTAccount)) as System.Security.Principal.NTAccount;
            string strEveryoneAccount = acct.ToString();

            switch (mainWindow.SizesInstance.Sizes.Any(), !string.IsNullOrWhiteSpace(mainWindow.ClientName))
            {
                case (false, false):
                    subDirectoryName = string.Format("{0} ({1})", DateTime.Now.ToString("dd-MM-yyyy HH.mm", ci), "ALL AVAILABLE SIZES");
                    break;
                case (false, true):
                    subDirectoryName = string.Format("{0} {1} ({2})", mainWindow.ClientName, DateTime.Now.ToString("dd-MM-yyyy HH.mm", ci), "ALL AVAILABLE SIZES");
                    break;
                case (true, false):
                    subDirectoryName = string.Format("{0} ({1})", DateTime.Now.ToString("dd-MM-yyyy HH.mm", ci), string.Join(", ", mainWindow.SizesInstance.Sizes));
                    break;
                case (true, true):
                    subDirectoryName = string.Format("{0} {1} ({2})", mainWindow.ClientName, DateTime.Now.ToString("dd-MM-yyyy HH.mm", ci), string.Join(", ", mainWindow.SizesInstance.Sizes));
                    break;
            }

            try
            {
                var mainImageDirectory = GetMainImageDirectory(mainWindow);
                if (!Directory.Exists(mainImageDirectory))
                {
                    CreateMainImageDirectory(mainWindow, log);
                }

                var subDirectoryLocation = string.Format("{0}\\{1}", mainImageDirectory, subDirectoryName);

                if (Directory.Exists(subDirectoryLocation))
                {
                    return subDirectoryLocation;
                }
                DirectorySecurity securityRules = new();
                securityRules.AddAccessRule(new FileSystemAccessRule(strEveryoneAccount, FileSystemRights.FullControl, AccessControlType.Allow));

                var directoryInfo = Directory.CreateDirectory(subDirectoryLocation);
                directoryInfo.SetAccessControl(securityRules);
                log.NewLogCreatedEvent(string.Format("{0} added at: {1}", subDirectoryName, directoryInfo.FullName), DateTime.Now);
                return subDirectoryLocation;
            }
            catch (Exception e)
            {
                log.NewLogCreatedEvent(string.Format("{0} {1}", e, "Could not create subDirectory"), DateTime.Now);
                return null;
            }
        }
    }
}
