using System;
using System.Linq;
using HidLibrary;
using JoystickLibrary.Parsers;

namespace JoystickLibrary.DataProviders
{
    public abstract class JoystickDataProvider
    {
        protected JoystickType Type;
        protected int VendorId;

        protected HidDevice Device;
        protected readonly IJoystickDataParser JoystickDataParser;

        protected JoystickDataProvider(JoystickType type, int vendorId)
        {
            Type = type;
            VendorId = vendorId;

            JoystickDataParser = JoystickDataParserFactory.GetJoystickDataParser(type);
        }

        public bool OpenDevice()
        {
            var devices = HidDevices.Enumerate(VendorId);

            if (devices != null && devices.Any())
            {
                Device = devices.First();

                if (Device != null)
                {
                    Device.OpenDevice();

                    Device.Inserted += () =>
                    {
                        DeviceAttached();
                        Device.ReadReport(OnReportReceived);
                    };
                    Device.Removed += DeviceRemoved;
                    Device.MonitorDeviceEvents = true;

                    return true;
                }
            }

            return false;
        }

        private void OnReportReceived(HidReport report)
        {
            if (Device.IsConnected == false)
            {
                return;
            }

            if (report.Data.Length == JoystickDataParser.DataSize)
            {
                ReportReceived(JoystickDataParser.Parse(report.Data));
            }

            Device.ReadReport(OnReportReceived);
        }

        public virtual void DeviceAttached()
        {
            Console.WriteLine("Joystick attached.");
        }

        public virtual void DeviceRemoved()
        {
            Console.WriteLine("Joystick removed.");
        }

        public virtual void ReportReceived(JoystickData report)
        {
            Console.WriteLine("Data received.");
        }
    }
}
