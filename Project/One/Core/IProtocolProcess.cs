namespace One.Core
{
    interface IProtocolProcess
    {
        void Pack(IProtocolBody body);

        void Unpack(byte[] buf);
    }
}
