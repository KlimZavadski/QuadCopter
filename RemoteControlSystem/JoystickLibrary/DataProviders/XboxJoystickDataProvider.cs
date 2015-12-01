using System;
using System.Linq;

namespace JoystickLibrary.DataProviders
{
    class XboxJoystickDataProvider : JoystickDataProvider
    {
        private int _count;
        private const int _reportsInPackage = 10;
        private readonly JoystickData[] _reportArray;

        public delegate void PackageDelegate(byte[] data);

        public event PackageDelegate OnPackageAvailableEvent;

        // HID\VID_054C&PID_0268
        public XboxJoystickDataProvider() : base(JoystickType.XBox, 0x054C)
        {
            _reportArray = new JoystickData[_reportsInPackage];
        }

        public override void ReportReceived(JoystickData report)
        {
            // Event appears 130 times per sec. Reduce it to 13 times (about 72ms solving package).
            _reportArray[_count++] = report;

            // Send one package with averaged data.
            if (_count == _reportsInPackage)
            {
                _count = 0;

                var averageReport = new JoystickData
                {
                    UpDown = Convert.ToByte(_reportArray.Average(x => x.UpDown)),
                    RotateLeftRight = Convert.ToByte(_reportArray.Average(x => x.RotateLeftRight)),
                    ForwardBack = Convert.ToByte(_reportArray.Average(x => x.ForwardBack)),
                    LeftRight = Convert.ToByte(_reportArray.Average(x => x.LeftRight)),
                    Buttons = _reportArray.Last().Buttons,
                    Button4PressingDegree = Convert.ToByte(_reportArray.Average(x => x.Button4PressingDegree)),
                    Button5PressingDegree = Convert.ToByte(_reportArray.Average(x => x.Button5PressingDegree)),
                    Button6PressingDegree = Convert.ToByte(_reportArray.Average(x => x.Button6PressingDegree)),
                    Button7PressingDegree = Convert.ToByte(_reportArray.Average(x => x.Button7PressingDegree)),
                    AdditionalButtons = _reportArray.Last().AdditionalButtons,
                    AdditionalButton0PressingDegree = Convert.ToByte(_reportArray.Average(x => x.AdditionalButton0PressingDegree)),
                    AdditionalButton1PressingDegree = Convert.ToByte(_reportArray.Average(x => x.AdditionalButton1PressingDegree)),
                    AdditionalButton2PressingDegree = Convert.ToByte(_reportArray.Average(x => x.AdditionalButton2PressingDegree)),
                    AdditionalButton3PressingDegree = Convert.ToByte(_reportArray.Average(x => x.AdditionalButton3PressingDegree)),
                    AdditionalButton4PressingDegree = Convert.ToByte(_reportArray.Average(x => x.AdditionalButton4PressingDegree)),
                    AdditionalButton5PressingDegree = Convert.ToByte(_reportArray.Average(x => x.AdditionalButton5PressingDegree)),
                    AdditionalButton6PressingDegree = Convert.ToByte(_reportArray.Average(x => x.AdditionalButton6PressingDegree)),
                    AdditionalButton7PressingDegree = Convert.ToByte(_reportArray.Average(x => x.AdditionalButton7PressingDegree))
                };

                if (OnPackageAvailableEvent != null)
                {
                    OnPackageAvailableEvent(averageReport.ToRadioCarByteArray());
                }
            }
        }
    }
}
