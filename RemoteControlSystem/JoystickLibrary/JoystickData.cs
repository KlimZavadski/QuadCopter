namespace JoystickLibrary
{
    public struct JoystickData
    {
        public byte UpDown;
        public byte RotateLeftRight;

        public byte ForwardBack;
        public byte LeftRight;

        // Main group
        public byte Buttons;
        // Arrows
        public byte Button4PressingDegree;
        public byte Button5PressingDegree;
        public byte Button6PressingDegree;
        public byte Button7PressingDegree;

        // Additional group
        public byte AdditionalButtons;
        // Triggers
        public byte AdditionalButton0PressingDegree;
        public byte AdditionalButton1PressingDegree;
        // Fronters
        public byte AdditionalButton2PressingDegree;
        public byte AdditionalButton3PressingDegree;
        // Circles
        public byte AdditionalButton4PressingDegree;
        public byte AdditionalButton5PressingDegree;
        public byte AdditionalButton6PressingDegree;
        public byte AdditionalButton7PressingDegree;

        public byte[] ToRadioCarByteArray()
        {
            return new[] { Button4PressingDegree, Button6PressingDegree, Button7PressingDegree, Button5PressingDegree };
        }

        public byte[] ToQuadCopterByteArray()
        {
            return new[] { UpDown, RotateLeftRight, ForwardBack, LeftRight, Buttons, AdditionalButtons };
        }
    }
}
