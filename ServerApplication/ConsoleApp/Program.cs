using System;
using System.Linq;
using System.Threading;
using HidLibrary;

namespace ConsoleApp
{
    class Program
    {
        public static void Main(String[] args)
        {
            new Program().Worker();
        }

        //HID\VID_0810&PID_0003
        private const int vendorId = 0x0810;
        private static HidDevice device;

        private void Worker()
        {
            var r = HidDevices.Enumerate(vendorId).ToList();
            device = HidDevices.Enumerate(vendorId).ElementAt(0);

            if (device == null)
            {
                Console.WriteLine("Could not find a joystick.");
                Console.ReadKey();
                return;
            }

            device.OpenDevice();
            device.Inserted += DeviceAttachedHandler;
            device.Removed += DeviceRemovedHandler;
            device.MonitorDeviceEvents = true;
            device.ReadReport(OnReport);

            Console.WriteLine("Joystick found and opened.");
            Thread.CurrentThread.Join();
            device.CloseDevice();
        }

        private static void OnReport(HidReport report)
        {
            if (device.IsConnected == false) return;

            if (report.Data.Length >= 8)
            {
                var d = report.Data;
                //Console.WriteLine("{0,3} {1,3} {2,3} {3,3} {4,3} {5,3} {6,3} {7,3}", d[0], d[1], d[2], d[3], d[4], d[5], d[6], d[7]);
                Console.WriteLine("{0,3} {1,3} {2,3} {3,3} {4,3} {5,3}", d[1], d[2], d[3], d[4], d[5], d[6]);
            }
            device.ReadReport(OnReport);
        }

        private static void DeviceAttachedHandler()
        {
            Console.WriteLine("Joystick attached.");
            device.ReadReport(OnReport);
        }

        private static void DeviceRemovedHandler()
        {
            Console.WriteLine("Joystick removed.");
        }
    }
}
