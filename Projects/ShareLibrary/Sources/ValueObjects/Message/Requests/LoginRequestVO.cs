namespace Share
{
    /// <summary>
    /// 登录请求
    /// </summary>
    [Msg(EC2S.LOGIN_REQUEST)]
    public class LoginRequestVO
    {
        /// <summary>
        /// 账号
        /// </summary>
        public string account;

        /// <summary>
        /// 密码
        /// </summary>
        public string pwd;

        /// <summary>
        /// 设备ID
        /// </summary>
        public string deviceId;
    }
}
