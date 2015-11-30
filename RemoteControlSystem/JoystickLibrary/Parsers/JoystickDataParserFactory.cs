namespace JoystickLibrary.Parsers
{
    public static class JoystickDataParserFactory
    {
        public static IJoystickDataParser GetJoystickDataParser(JoystickType type)
        {
            switch (type)
            {
                case JoystickType.XBox:
                    return new XBoxJoystickDataParser();
                case JoystickType.Playstation:
                    return new PlaystationJoystickDataParser();
            }

            return null;
        }
    }
}
