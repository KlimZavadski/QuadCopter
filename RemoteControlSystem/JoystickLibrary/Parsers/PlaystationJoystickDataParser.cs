namespace JoystickLibrary.Parsers
{
    /// <summary>
    /// Represent class for parsing data from Playstation joystick;
    /// Playstation joystick return 8 bytes data, but used only 6 (0-6).
    /// </summary>
    public class PlaystationJoystickDataParser : IJoystickDataParser
    {
        public int DataSize { get { return 8; } }

        public JoystickData Parse(byte[] data)
        {
            var joystickData = new JoystickData
            {
                ForwardBack = data[1],
                LeftRight = data[2],
                UpDown = data[4],
                RotateLeftRight = data[3]
            };

            byte b5 = (byte)(data[5] - 0xF);
            byte b6 = data[6];

            // b[5](0:2) , b[6](0:3)
            int ub = b6 & 0xF;
            int ob = GetButtonNumber(b5) << 4;
            joystickData.Buttons = (byte)(ob + ub);

            // b[6](4:5) , b[5](4:7)
            ub = (b5 & 0xF0) >> 4;
            ob = b6 & 0x30;
            joystickData.AdditionalButtons = (byte)(ob + ub);

            return joystickData;
        }

        private byte GetButtonNumber(byte b)
        {
            switch ((b - 1) & 0x7) // Get last 3 bits.
            {
                // 000b - 1000b
                case 0x0:
                    return 0x8;
                // 001b - 1010b
                case 0x1:
                    return 0xA;
                // 010b - 0010b
                case 0x2:
                    return 0x2;
                // 011b - 0110b
                case 0x3:
                    return 0x6;
                // 100b - 0100b
                case 0x4:
                    return 0x4;
                // 101b - 0101b
                case 0x5:
                    return 0x5;
                // 110b - 0001b
                case 0x6:
                    return 0x1;
                // 111b - 1001b
                case 0x7:
                    return 0x9;
            }
            return 0;
        }
    }
}
