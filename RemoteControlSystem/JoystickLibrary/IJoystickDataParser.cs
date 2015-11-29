namespace JoystickLibrary
{
    public interface IJoystickDataParser
    {
        int DataSize { get; }

        JoystickData Parse(byte[] data);
    }
}
