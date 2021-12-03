using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading.Tasks;

namespace TestMD64
{
    class Program
    {
        static Guid GUID_DEVINTERFACE_KEYBOARD = new Guid(0x884b96c3, 0x56ef, 0x11d1, 0xbC, 0x8c, 0x00, 0xa0, 0xc9, 0x14, 0x05, 0xdd);       
        static Guid GUID_DEVINTERFACE_MONITOR  = new Guid(0xe6f07b5f, 0xee97, 0x4a90, 0xb0, 0x76, 0x33, 0xf5, 0x7b, 0xf4, 0xea, 0xa7);
        static Guid GUID_DEVINTERFACE_MD64WIN  = new Guid(0x8930eb28, 0x6a2d, 0x4e30, 0xa5, 0x7d, 0x3f, 0xd2, 0x14, 0x8d, 0x3d, 0xdd);

        public static uint CTL_CODE(uint DeviceType, uint Function, uint Method, uint Access)
        {
            return (DeviceType << 16) | (Access << 14) | (Function << 2) | Method;
        }

        static void Main(string[] args)
        {
            uint needed;
            string devicePathName;
            IntPtr hDev;
            SP_DEVICE_INTERFACE_DATA ifData;
            Guid deviceGuid;

            uint DRVNT_MD64_IOCTL_WRITE_RDOT   = CTL_CODE(0x8000, 0x805, 0, 0);
            uint DRVNT_MD64_IOCTL_clr_bit_RDOT = CTL_CODE(0x8000, 0x80f, 0, 0);
            uint DRVNT_MD64_IOCTL_set_bit_RDOT = CTL_CODE(0x8000, 0x80d, 0, 0);

            Console.WriteLine($"Версия:  \"4\" - x86  \"8\" - x64 : {IntPtr.Size}" );

            Console.WriteLine($"Для тестирования \"MD64WIN\" нажмите 1.");

            int deviceNumber = Convert.ToInt32(Console.ReadLine());

            if(deviceNumber == 1)
            {
                deviceGuid = GUID_DEVINTERFACE_MD64WIN;
            }
            else
            {
                deviceGuid = GUID_DEVINTERFACE_MONITOR;
            }

            IntPtr hDevInfo = Win32.SetupDiGetClassDevs(ref deviceGuid, IntPtr.Zero, IntPtr.Zero, Win32.DIGCF_DEVICEINTERFACE | Win32.DIGCF_PRESENT);

            Console.WriteLine($"Recieve device information elements.Handle \"hDevInfo\": {hDevInfo}");

            ifData = new SP_DEVICE_INTERFACE_DATA();
            ifData.cbSize = (uint)Marshal.SizeOf(ifData);           
            Console.WriteLine($"ifData.cbSize : {ifData.cbSize}");
            ifData.Flags = 1;           
            ifData.Reserved = IntPtr.Zero;
            bool result2 = Win32.SetupDiEnumDeviceInterfaces(hDevInfo, IntPtr.Zero, ref deviceGuid, 0, ifData);
            if (result2 == false)
            {
                int error = Marshal.GetLastWin32Error();                
                Console.WriteLine($"Error in SetupDiEnumDeviceInterfaces. Error code: {error} ");
                Console.ReadLine();
            }

            bool result3 = Win32.SetupDiGetDeviceInterfaceDetail(hDevInfo, ifData, IntPtr.Zero, 0, out needed, null);
            if (!result3)
            {
                int error = Marshal.GetLastWin32Error(); //Expected Error 
            }

            IntPtr DeviceInterfaceDetailData = Marshal.AllocHGlobal((int)needed);
            try
            {
                uint size = needed;
                Console.WriteLine($"needed size: {needed} ");
                Marshal.WriteInt32(DeviceInterfaceDetailData, (IntPtr.Size == 4) ? (4 + Marshal.SystemDefaultCharSize) : 8);
                bool result4 = Win32.SetupDiGetDeviceInterfaceDetail(hDevInfo, ifData, DeviceInterfaceDetailData, size, out needed, null);
                if (result4 == false)
                {
                    int error = Marshal.GetLastWin32Error();
                    Console.WriteLine(error);
                    Console.ReadLine();
                }              
                IntPtr pDevicePathName = new IntPtr(DeviceInterfaceDetailData.ToInt64() + 4);
                devicePathName = Marshal.PtrToStringAuto(pDevicePathName);        // Путь выглядит  "\\\\?\\display#phlc05d#4&2697a614&0&uid16843008#{e6f07b5f-ee97-4a90-b076-33f57bf4eaa7}"               
                hDev = Win32.CreateFileW(devicePathName, FileAccess.ReadWrite, FileShare.ReadWrite, IntPtr.Zero, FileMode.Open, FileAttributes.Normal, IntPtr.Zero);
                Console.WriteLine($"hDev: {hDev} ");
                                         
                int error1 = Marshal.GetLastWin32Error();               
                Console.WriteLine($"error code: {error1}. Expected 0");
            }
            finally
            {
                Marshal.FreeHGlobal(DeviceInterfaceDetailData);
            }

            // Коды лампочем пульта ОУ  5, 7, 9, 11, 2, 4, 1, 3
            ushort buff;  
            ushort[] codeArray = {5, 7, 9, 11, 2, 4, 1, 3 }; 
            uint nOutBufferSize;
            bool result;


            foreach (ushort i in codeArray)
            {
                buff = i;
                result = Win32.DeviceIoControl(hDev, DRVNT_MD64_IOCTL_clr_bit_RDOT, ref buff, 2, IntPtr.Zero, 0, out nOutBufferSize, IntPtr.Zero);
                if (result == false)
                {
                    int error = Marshal.GetLastWin32Error();
                    Console.WriteLine($"Error in DeviceIoControl. CTL DRVNT_MD64_IOCTL_clr_bit_RDOT. Lump: {i}. Error code {error}");
                    Console.ReadLine();
                    break;
                }

                result = Win32.DeviceIoControl(hDev, DRVNT_MD64_IOCTL_set_bit_RDOT, ref buff, 2, IntPtr.Zero, 0, out nOutBufferSize, IntPtr.Zero);
                if (result == false)
                {
                    int error = Marshal.GetLastWin32Error();
                    Console.WriteLine($"Error in DeviceIoControl. CTL DRVNT_MD64_IOCTL_set_bit_RDOT. Lump: {i}. Error code {error}");
                    Console.ReadLine();
                    break;
                }
            }


            /* 
             buff = 3;

             result5 = Win32.DeviceIoControl(hDev, DRVNT_MD64_IOCTL_clr_bit_RDOT, ref buff, 2, IntPtr.Zero, 0, out nOutBufferSize, IntPtr.Zero);
             if (result5 == false)
             {
                 int error = Marshal.GetLastWin32Error();
                 Console.WriteLine($"Error in DeviceIoControl. Error code {error}");
                 Console.ReadLine();
             }
             result6 = Win32.DeviceIoControl(hDev, DRVNT_MD64_IOCTL_set_bit_RDOT, ref buff, 2, IntPtr.Zero, 0, out nOutBufferSize, IntPtr.Zero);
             if (result5 == false)
             {
                 int error = Marshal.GetLastWin32Error();
                 Console.WriteLine($"Error in DeviceIoControl. Error code {error}");
                 Console.ReadLine();
             }*/

            Console.WriteLine("Лампы включены. Нажмите любую клавишу чтобы  погасить!");
            Console.ReadLine();

            foreach (ushort i in codeArray)
            {
                buff = i;
                result = Win32.DeviceIoControl(hDev, DRVNT_MD64_IOCTL_clr_bit_RDOT, ref buff, 2, IntPtr.Zero, 0, out nOutBufferSize, IntPtr.Zero);
                if (result == false)
                {
                    int error = Marshal.GetLastWin32Error();
                    Console.WriteLine($"Error in DeviceIoControl. CTL DRVNT_MD64_IOCTL_clr_bit_RDOT. Lump: {i}. Error code {error}");
                    Console.ReadLine();
                    break;
                }
            
            }

            Console.WriteLine("Лампы выключены. Нажмите любую клавишу для выхода!");
            Console.ReadLine();

        }
    }
}
