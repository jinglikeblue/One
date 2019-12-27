namespace OneServer
{
    /// <summary>
    /// 主循环逻辑接口，实现该接口的类，Run方法会在主循环中定期执行
    /// </summary>
    interface IMainLoopLogic
    {
        void Excute();
    }
}
