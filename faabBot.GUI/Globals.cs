using System.Collections.Generic;

namespace faabBot.GUI
{
    internal class Globals
    {
        public static double Version = 1.0;

        public static bool DevelopersMode = false;

        public static int MaxUrlDisplayLength = 70;

        public static int ExplicitWaitInSeconds = 2;

        public static int ImplicitWaitInMilliseconds = 100;

        public static string AssemblyName = "faabBot.GUI.dll";

        public static string MainImageDirectoryName = "PRODUCTS";

        public static List<string> Sizes = new()
        {
            "",
            "XS",
            "S",
            "M",
            "L",
            "XL",
            "XXL",
            "FREE"
        };
    }
}
