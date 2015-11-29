namespace JoystickLibrary.Parsers
{
    /// <summary>
    /// Represent class for parsing data from xBox joystick;
    /// xBox joystick return 48 bytes data, but used only 6 (0-1 & 4-5).
    /// </summary>
    public class XBoxJoystickDataParser : IJoystickDataParser
    {
        public int DataSize { get { return 48; } }

        public JoystickData Parse(byte[] data)
        {
            return new JoystickData
            {
                // Up/Down 0-127-255
                UpDown = data[6],
                // Rotate Left/Right 0-128-255
                RotateLeftRight = data[5],
                // Forward/Back 0-127-255
                ForwardBack = data[8],
                // Left/Right 0-128-255
                LeftRight = data[7],
                Buttons = data[1],
                Button4PressingDegree = data[13],
                Button5PressingDegree = data[14],
                Button6PressingDegree = data[15],
                Button7PressingDegree = data[16],
                AdditionalButtons = data[2],
                AdditionalButton0PressingDegree = data[17],
                AdditionalButton1PressingDegree = data[18],
                AdditionalButton2PressingDegree = data[19],
                AdditionalButton3PressingDegree = data[20],
                AdditionalButton4PressingDegree = data[21],
                AdditionalButton5PressingDegree = data[22],
                AdditionalButton6PressingDegree = data[23],
                AdditionalButton7PressingDegree = data[24]
            };
        }
    }
}
