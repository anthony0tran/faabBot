using System.Collections.Generic;

namespace faabBot.GUI
{
    internal class Globals
    {
        public static double Version = 1.0;

        public static int MaxUrlDisplayLength = 40;

        public static int ExplicitWaitInSeconds = 4;

        public static int ImplicitWaitSeconds = 2;

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
