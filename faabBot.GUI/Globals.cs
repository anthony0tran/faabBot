using System.Collections.Generic;

namespace faabBot.GUI
{
    internal class Globals
    {
        public static double Version = 1.0;

        public static int MaxUrlDisplayLength = 70;

        public static int ExplicitWaitInSeconds = 2;

        public static int ImplicitWaitInSeconds = 1;

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
            "XXL"
        };
    }
}
