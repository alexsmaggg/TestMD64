using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TestMD64
{
    [StructLayout(LayoutKind.Sequential)]
    public class SP_DEVINFO_DATA
    {
        public uint cbSize;
        public Guid classGuid;
        public uint devInst;
        public IntPtr reserved;
    };

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public class SP_DEVICE_INTERFACE_DETAIL_DATA
    {
        public uint cbSize;
        public byte[] devicePath;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public class SP_DEVICE_INTERFACE_DATA
    {
        public uint cbSize;
        public Guid InterfaceClassGuid;
        public uint Flags;
        public IntPtr Reserved;
    }

    public class Win32
    {
        public static uint ANYSIZE_ARRAY = 1000;

        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern IntPtr SetupDiGetClassDevs(ref Guid ClassGuid, IntPtr Enumerator, IntPtr hwndParent, uint Flags);

        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern Boolean SetupDiEnumDeviceInfo(IntPtr lpInfoSet, UInt32 dwIndex, SP_DEVINFO_DATA devInfoData);

        [DllImport(@"setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern Boolean SetupDiEnumDeviceInterfaces(IntPtr hDevInfo, IntPtr devInfo, ref Guid interfaceClassGuid, uint memberIndex, SP_DEVICE_INTERFACE_DATA deviceInterfaceData);

        [DllImport(@"setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern Boolean SetupDiGetDeviceInterfaceDetail(IntPtr hDevInfo, SP_DEVICE_INTERFACE_DATA deviceInterfaceData, IntPtr deviceInterfaceDetailData, uint deviceInterfaceDetailDataSize, out uint requiredSize, SP_DEVINFO_DATA deviceInfoData);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr CreateFileW(
            [MarshalAs(UnmanagedType.LPTStr)] string filename,
            [MarshalAs(UnmanagedType.U4)] FileAccess access,
            [MarshalAs(UnmanagedType.U4)] FileShare share,
            IntPtr securityAttributes, // optional SECURITY_ATTRIBUTES struct or IntPtr.Zero
            [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
            [MarshalAs(UnmanagedType.U4)] FileAttributes flagsAndAttributes,
            IntPtr templateFile);


        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool DeviceIoControl(
            IntPtr hDevice, 
            uint dwIoControlCode,
            ref ushort inBuffer,
            uint nInBufferSize,
            IntPtr lpOutBuffer, 
            uint nOutBufferSize,
            out uint lpBytesReturned, 
            IntPtr lpOverlapped);


        public const int DIGCF_PRESENT           = 0x02;
        public const int DIGCF_DEVICEINTERFACE   = 0x10;
        public const int SPDRP_DEVICEDESC        = (0x00000000);
        public const long ERROR_NO_MORE_ITEMS    = 259L;
        public const uint FILE_SHARE_READ        = 0x1;
        public const uint FILE_SHARE_WRITE       = 0x2;
        public const uint FILE_ATTRIBUTE_NORMAL  = 0x80;
        public const uint OPEN_EXISTING          = 3;
        public const UInt32 INVALID_HANDLE_VALUE = 0xffffffff;

        public enum DesiredAccess : uint
        {
            GENERIC_READ  = 0x80000000,
            GENERIC_WRITE = 0x40000000
        }
        [Flags]
        public enum ShareMode : uint
        {
            FILE_SHARE_NONE   = 0x0,
            FILE_SHARE_READ   = 0x1,
            FILE_SHARE_WRITE  = 0x2,
            FILE_SHARE_DELETE = 0x4,

        }
        public enum MoveMethod : uint
        {
            FILE_BEGIN   = 0,
            FILE_CURRENT = 1,
            FILE_END     = 2
        }
        public enum CreationDisposition : uint
        {
            CREATE_NEW       = 1,
            CREATE_ALWAYS    = 2,
            OPEN_EXISTING    = 3,
            OPEN_ALWAYS      = 4,
            TRUNCATE_EXSTING = 5
        }
        [Flags]
        public enum FlagsAndAttributes : uint
        {
            FILE_ATTRIBUTES_ARCHIVE      = 0x20,
            FILE_ATTRIBUTE_HIDDEN        = 0x2,
            FILE_ATTRIBUTE_NORMAL        = 0x80,
            FILE_ATTRIBUTE_OFFLINE       = 0x1000,
            FILE_ATTRIBUTE_READONLY      = 0x1,
            FILE_ATTRIBUTE_SYSTEM        = 0x4,
            FILE_ATTRIBUTE_TEMPORARY     = 0x100,
            FILE_FLAG_WRITE_THROUGH      = 0x80000000,
            FILE_FLAG_OVERLAPPED         = 0x40000000,
            FILE_FLAG_NO_BUFFERING       = 0x20000000,
            FILE_FLAG_RANDOM_ACCESS      = 0x10000000,
            FILE_FLAG_SEQUENTIAL_SCAN    = 0x8000000,
            FILE_FLAG_DELETE_ON          = 0x4000000,
            FILE_FLAG_POSIX_SEMANTICS    = 0x1000000,
            FILE_FLAG_OPEN_REPARSE_POINT = 0x200000,
            FILE_FLAG_OPEN_NO_CALL       = 0x100000
        }
    }
}

