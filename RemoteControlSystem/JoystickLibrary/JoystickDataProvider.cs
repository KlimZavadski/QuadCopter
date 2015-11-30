using System;
using System.Linq;
using HidLibrary;
using JoystickLibrary.Parsers;

namespace JoystickLibrary
{
    public class JoystickDataProvider
    {
        // xBox - HID\VID_054C&PID_0268
        // Playstation - HID\VID_0810&PID_0003
//        private const int _vendorId = 0x054C;

        private JoystickType _type;
        private int _vendorId;

        private HidDevice _device;
        private readonly IJoystickDataParser _joystickDataParser;

        public JoystickDataProvider(JoystickType type, int vendorId)
        {
            _type = type;
            _vendorId = vendorId;

            _joystickDataParser = JoystickDataParserFactory.GetJoystickDataParser(type);

            var devices = HidDevices.Enumerate(_vendorId);

                if (devices != null && devices.Any())
                {
                    _device = devices.First();

                    if (_device != null)
                    {
                        _device.OpenDevice();
                        _device.Inserted += () =>
                        {
                            DeviceAttached();
                            _device.ReadReport(OnReportReceived);
                        };
                        _device.Removed += DeviceRemoved;
                        _device.MonitorDeviceEvents = true;
                    }
                }
        }

        private void OnReportReceived(HidReport report)
        {
            if (_device.IsConnected == false)
            {
                return;
            }

            if (report.Data.Length == _joystickDataParser.DataSize)
            {
                ReportReceived(_joystickDataParser.Parse(report.Data));
            }

            _device.ReadReport(OnReportReceived);
        }

        public virtual void DeviceAttached()
        {
            Console.WriteLine("Joystick attached.");
        }

        public virtual void DeviceRemoved()
        {
            Console.WriteLine("Joystick removed.");
        }

        public virtual void ReportReceived(JoystickData data)
        {
            Console.WriteLine("Data received.");
        }
    }
}
