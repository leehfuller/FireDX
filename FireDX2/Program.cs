using System;

namespace FireDX2
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            string firstArg = "/c";
            if (args.Length > 1 && args[1].Trim() != "") firstArg = args[1].ToLower();
            firstArg = firstArg.Replace("-", "/");
            firstArg = firstArg.Substring(0, 2);

            switch (firstArg)
            {
                case ("/c"):
                    // screen saver configuration
                    // TODO
                    break;
                case ("/p"):
                    // screen saver preview, parent hwnd in args
                    if (args.Length > 2)
                    {
                        // TODO
                    }
                    break;
                case ("/s"):
                    // screen saver show
                    // TODO
                    break;
                case ("/d"):
                // show diagnostics info
                // TODO
                default:
                    // TODO - error?
                    break;
            }

            using (var game = new FireDX2())
                game.Run();
        }
    }
#endif
}
