using System;
using System.Text;
using System.Runtime.InteropServices;

namespace CairoDesktop.Interop
{
    /// <summary>
    /// Class containing methods to retrieve specific file system paths.
    /// </summary>
    public static class KnownFolders
    {
        private static string[] _knownFolderGuids = new string[]
        {
        "{56784854-C6CB-462B-8169-88E350ACB882}", // Contacts
        "{B4BFCC3A-DB2C-424C-B029-7FE99A87C641}", // Desktop
        "{FDD39AD0-238F-46AF-ADB4-6C85480369C7}", // Documents
        "{374DE290-123F-4565-9164-39C4925E467B}", // Downloads
        "{1777F761-68AD-4D8A-87BD-30B759FA33DD}", // Favorites
        "{BFB9D5E0-C6A9-404C-B2B2-AE6DB6AF4968}", // Links
        "{4BD8D571-6D19-48D3-BE97-422220080E43}", // Music
        "{33E28130-4E1E-4676-835A-98395C3BC3BB}", // Pictures
        "{4C5C32FF-BB9D-43B0-B5B4-2D72E54EAAA4}", // SavedGames
        "{7D1D3A04-DEBB-4115-95CF-2F29DA2920DA}", // SavedSearches
        "{18989B1D-99B5-455B-841C-AB7C74E4DDFC}" // Videos
        };

        /// <summary>
        /// Gets the current path to the specified known folder as currently configured. This does
        /// not require the folder to be existent.
        /// </summary>
        /// <param name="knownFolder">The known folder which current path will be returned.</param>
        /// <returns>The default path of the known folder.</returns>
        /// <exception cref="System.Runtime.InteropServices.ExternalException">Thrown if the path
        ///     could not be retrieved.</exception>
        public static string GetPath(KnownFolder knownFolder)
        {
            return GetPath(knownFolder, false);
        }

        /// <summary>
        /// Gets the current path to the specified known folder as currently configured. This does
        /// not require the folder to be existent.
        /// </summary>
        /// <param name="knownFolder">The known folder which current path will be returned.</param>
        /// <param name="defaultUser">Specifies if the paths of the default user (user profile
        ///     template) will be used. This requires administrative rights.</param>
        /// <returns>The default path of the known folder.</returns>
        /// <exception cref="System.Runtime.InteropServices.ExternalException">Thrown if the path
        ///     could not be retrieved.</exception>
        public static string GetPath(KnownFolder knownFolder, bool defaultUser)
        {
            return GetPath(knownFolder, KnownFolderFlags.DontVerify, defaultUser);
        }

        private static string GetPath(KnownFolder knownFolder, KnownFolderFlags flags,
            bool defaultUser)
        {
            IntPtr outPath;
            //int result = SHGetFolderPath(IntPtr.Zero, SpecialFolderCSIDL, new IntPtr(defaultUser ? -1 : 0), (uint)flags, out outPath);
            /*if (result >= 0)
            {
                return Marshal.PtrToStringUni(outPath);
            }
            else
            {
                throw new ExternalException("Unable to retrieve the known folder path. It may not "
                    + "be available on this system.", result);
            } */
            return "Documents";
        }

        [DllImport("shell32.dll")]
        static extern int SHGetFolderPath(IntPtr hwndOwner, int nFolder, IntPtr hToken, uint dwFlags, [Out] StringBuilder pszPath);

        [Flags]
        private enum KnownFolderFlags : uint
        {
            SimpleIDList = 0x00000100,
            NotParentRelative = 0x00000200,
            DefaultPath = 0x00000400,
            Init = 0x00000800,
            NoAlias = 0x00001000,
            DontUnexpand = 0x00002000,
            DontVerify = 0x00004000,
            Create = 0x00008000,
            NoAppcontainerRedirection = 0x00010000,
            AliasOnly = 0x80000000
        }
    }

    /// <summary>
    /// Standard folders registered with the system. These folders are installed with Windows Vista
    /// and later operating systems, and a computer will have only folders appropriate to it
    /// installed.
    /// </summary>
    public enum KnownFolder
    {
        Contacts,
        Desktop,
        Documents,
        Downloads,
        Favorites,
        Links,
        Music,
        Pictures,
        SavedGames,
        SavedSearches,
        Videos
    }
    public enum SpecialFolderCSIDL : int
    {
        CSIDL_DESKTOP = 0x0000,    // <desktop>
        CSIDL_INTERNET = 0x0001,    // Internet Explorer (icon on desktop)
        CSIDL_PROGRAMS = 0x0002,    // Start Menu\Programs
        CSIDL_CONTROLS = 0x0003,    // My Computer\Control Panel
        CSIDL_PRINTERS = 0x0004,    // My Computer\Printers
        CSIDL_PERSONAL = 0x0005,    // My Documents
        CSIDL_FAVORITES = 0x0006,    // <user name>\Favorites
        CSIDL_STARTUP = 0x0007,    // Start Menu\Programs\Startup
        CSIDL_RECENT = 0x0008,    // <user name>\Recent
        CSIDL_SENDTO = 0x0009,    // <user name>\SendTo
        CSIDL_BITBUCKET = 0x000a,    // <desktop>\Recycle Bin
        CSIDL_STARTMENU = 0x000b,    // <user name>\Start Menu
        CSIDL_DESKTOPDIRECTORY = 0x0010,    // <user name>\Desktop
        CSIDL_DRIVES = 0x0011,    // My Computer
        CSIDL_NETWORK = 0x0012,    // Network Neighborhood
        CSIDL_NETHOOD = 0x0013,    // <user name>\nethood
        CSIDL_FONTS = 0x0014,    // windows\fonts
        CSIDL_TEMPLATES = 0x0015,
        CSIDL_COMMON_STARTMENU = 0x0016,    // All Users\Start Menu
        CSIDL_COMMON_PROGRAMS = 0x0017,    // All Users\Programs
        CSIDL_COMMON_STARTUP = 0x0018,    // All Users\Startup
        CSIDL_COMMON_DESKTOPDIRECTORY = 0x0019,    // All Users\Desktop
        CSIDL_APPDATA = 0x001a,    // <user name>\Application Data
        CSIDL_PRINTHOOD = 0x001b,    // <user name>\PrintHood
        CSIDL_LOCAL_APPDATA = 0x001c,    // <user name>\Local Settings\Applicaiton Data (non roaming)
        CSIDL_ALTSTARTUP = 0x001d,    // non localized startup
        CSIDL_COMMON_ALTSTARTUP = 0x001e,    // non localized common startup
        CSIDL_COMMON_FAVORITES = 0x001f,
        CSIDL_INTERNET_CACHE = 0x0020,
        CSIDL_COOKIES = 0x0021,
        CSIDL_HISTORY = 0x0022,
        CSIDL_COMMON_APPDATA = 0x0023,    // All Users\Application Data
        CSIDL_WINDOWS = 0x0024,    // GetWindowsDirectory()
        CSIDL_SYSTEM = 0x0025,    // GetSystemDirectory()
        CSIDL_PROGRAM_FILES = 0x0026,    // C:\Program Files
        CSIDL_MYPICTURES = 0x0027,    // C:\Program Files\My Pictures
        CSIDL_PROFILE = 0x0028,    // USERPROFILE
        CSIDL_SYSTEMX86 = 0x0029,    // x86 system directory on RISC
        CSIDL_PROGRAM_FILESX86 = 0x002a,    // x86 C:\Program Files on RISC
        CSIDL_PROGRAM_FILES_COMMON = 0x002b,    // C:\Program Files\Common
        CSIDL_PROGRAM_FILES_COMMONX86 = 0x002c,    // x86 Program Files\Common on RISC
        CSIDL_COMMON_TEMPLATES = 0x002d,    // All Users\Templates
        CSIDL_COMMON_DOCUMENTS = 0x002e,    // All Users\Documents
        CSIDL_COMMON_ADMINTOOLS = 0x002f,    // All Users\Start Menu\Programs\Administrative Tools
        CSIDL_ADMINTOOLS = 0x0030,    // <user name>\Start Menu\Programs\Administrative Tools
        CSIDL_CONNECTIONS = 0x0031,    // Network and Dial-up Connections
        CSIDL_CDBURN_AREA = 0x003B    // Data for burning with interface ICDBurn
    }
}
