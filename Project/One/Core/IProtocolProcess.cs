namespace One.Core
{
    interface IProtocolProcess
    {
        byte[] Pack(IProtocolBody body);

        int Unpack(byte[] buf);
    }
}
