namespace Share
{
    /// <summary>
    /// 登录返回数据
    /// </summary>
    [Msg(ES2C.LOGIN_RESPONSE)]
    public class LoginResponseVO
    {
        /// <summary>
        /// 昵称
        /// </summary>
        public string nickname;
    }
}
