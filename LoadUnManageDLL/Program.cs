using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using PInvoke;
using Win32Exception = System.ComponentModel.Win32Exception;

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
            var dllPath = Path.Combine(folderPath, $"{nameof(Resource.pdfium)}.dll");//解压输出的路径
            if (!File.Exists(dllPath))
                File.WriteAllBytes(dllPath, Resource.pdfium);
            LoadDll(dllPath);//应该每次都加载非托管
        }

        /// <summary>
        /// 加载非托管DLL
        /// </summary>
        /// <param name="dllName"></param>
        public static void LoadDll(string dllName)
        {
            var h =Kernel32.LoadLibrary(dllName);
            if (h.IsInvalid)//是否是无效的
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
