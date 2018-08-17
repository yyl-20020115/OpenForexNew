using System;
using System.Text;
using System.Reflection;
using System.IO;
using System.Runtime.InteropServices;

namespace ForexPlatformClient
{
    /// <summary>
    /// Since this class uses unsafe importing, it has not been added to the CommonSupport assembly.
    /// Usage example:
    /// GACHelper.AddAssemblyToCache(Assembly.GetExecutingAssembly().Location);
    /// GACHelper.RemoveAssemblyFromCache(assembly.GetName().Name);
    /// </summary>
    public static class GACHelper
    {
        [DllImport("Fusion.dll", CharSet = CharSet.Auto)]
        internal static extern int CreateAssemblyCache(out IAssemblyCache ppAsmCache, uint dwReserved);

        static public int AddAssemblyToCache(string assemblyExecutionName)
        {
            IAssemblyCache ac = null;
            int hr = CreateAssemblyCache(out ac, 0);
            if (hr != 0)
            {
                return hr;
            }

            return ac.InstallAssembly(0, assemblyExecutionName, (IntPtr)0);
        }

        static public int RemoveAssemblyFromCache(string assemblyName)
        {
            IAssemblyCache ac = null;

            int hr = CreateAssemblyCache(out ac, 0);

            if (hr != 0)
            {
                return hr;
            }

            uint n;
            return ac.UninstallAssembly(0, assemblyName, (IntPtr)0, out n);
        }

    }

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("e707dcde-d1cd-11d2-bab9-00c04f8eceae")]
    internal interface IAssemblyCache
    {
        [PreserveSig()]
        int UninstallAssembly(uint dwFlags, [MarshalAs(UnmanagedType.LPWStr)] string pszAssemblyName, IntPtr pvReserved, out uint pulDisposition);

        [PreserveSig()]
        int QueryAssemblyInfo(uint dwFlags, [MarshalAs(UnmanagedType.LPWStr)] string pszAssemblyName, IntPtr pAsmInfo);

        [PreserveSig()]
        int CreateAssemblyCacheItem(uint dwFlags, IntPtr pvReserved, out /*IAssemblyCacheItem*/IntPtr ppAsmItem, [MarshalAs(UnmanagedType.LPWStr)] String pszAssemblyName);

        [PreserveSig()]
        int CreateAssemblyScavenger(out object ppAsmScavenger);

        [PreserveSig()]
        int InstallAssembly(uint dwFlags, [MarshalAs(UnmanagedType.LPWStr)] string pszManifestFilePath, IntPtr pvReserved);
    }

}
