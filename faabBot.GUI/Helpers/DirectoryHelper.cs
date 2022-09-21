using faabBot.GUI.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
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
            try
            {
                var mainImageDirectory = GetMainImageDirectory(o);
                if (Directory.Exists(mainImageDirectory))
                {
                    return;
                }

                var directoryInfo = Directory.CreateDirectory(mainImageDirectory);
                log.NewLogCreatedEvent(string.Format("Maindirectory added at: {0}", directoryInfo.FullName), DateTime.Now);
            }
            catch
            {
                log.NewLogCreatedEvent("Error: Could not create maindirectory", DateTime.Now);
            }
        }

        public static void CreateSubImageDirectory(string subDirectoryName, object o, LogController log)
        {
            try
            {
                var mainImageDirectory = GetMainImageDirectory(o);
                if (Directory.Exists(mainImageDirectory))
                {
                    var subDirectoryLocation = string.Format("{0}\\{1}", mainImageDirectory, subDirectoryName);
                    var directoryInfo = Directory.CreateDirectory(subDirectoryLocation);
                    log.NewLogCreatedEvent(string.Format("{0} added at: {1}", subDirectoryName, directoryInfo.FullName), DateTime.Now);
                }
                else
                {
                    CreateMainImageDirectory(o, log);
                }

            }
            catch
            {
                log.NewLogCreatedEvent("Error: Could not create sub image directory", DateTime.Now);
            }
        }
    }
}
