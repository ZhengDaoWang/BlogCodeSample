using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace LoadUnManageDLL
{
    class Program
    {
        static void Main(string[] args)
        {
            UnzipAndLoad();
        }

        /// <summary>
        /// 解压资源并且加载非托管DLL
        /// </summary>
        static void UnzipAndLoad()
        {
            var folderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var dllPath = Path.Combine(folderPath, $"{nameof(Resource.pdfium)}.dll");
            File.WriteAllBytes(dllPath,Resource.pdfium);
            LoadDll(dllPath);
        }

        ///// <summary>.
        ///// managed wrapper around LoadLibrary
        ///// </summary>
        ///// <param name="dllName"></param>
        public static void LoadDll(string dllName)
        {
            IntPtr h = LoadLibrary(dllName);
            if (h == IntPtr.Zero)
            {
                Exception e = new Win32Exception();
                throw new DllNotFoundException($"Unable to load library: {dllName}", e);
            }

            Console.WriteLine("Load library successful");
        }

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern IntPtr LoadLibrary(string lpFileName);
    }
}
