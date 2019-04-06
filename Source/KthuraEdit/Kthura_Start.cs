using System;

namespace KthuraEdit
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Kthura_Start
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (var game = new Kthura_EditCore())
                game.Run();
        }
    }
#endif
}
