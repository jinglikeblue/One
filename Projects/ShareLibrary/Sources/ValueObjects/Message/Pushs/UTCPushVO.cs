namespace Share
{
    /// <summary>
    /// 推送时间
    /// </summary>
    [Msg(ES2C.UTC_PUSH)]
    class UTCPushVO
    {
        /// <summary>
        /// 服务器的UTC时间
        /// </summary>
        public int utc;
    }
}
