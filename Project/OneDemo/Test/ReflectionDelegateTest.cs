namespace One.Test
{
    class ReflectionDelegateTest
    {
        public static void Main()
        {
            //首先将方法绑定到委托上
            //action += Run;
            
            //首先获取委托所在类的Type
            System.Type type = typeof(ReflectionDelegateTest);
            //***因为委托实际上是字段，所以这里要用GetField来查找到它的信息***
            System.Reflection.FieldInfo actionField = type.GetField("action", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            //***通过字段信息获取到字段对应的值***
            object actionObject = actionField.GetValue(null);
            //***通过字段值获取到Invoke方法的元数据***
            System.Reflection.MethodInfo handlerMethod = actionObject.GetType().GetMethod("Invoke");
            //现在可以反射调用委托了，记得传入字段对应的值对象
            handlerMethod.Invoke(actionObject, new object[] { "Call By Reflection!" });

            System.Console.ReadKey();
        }

        /// <summary>
        /// 泛型委托
        /// </summary>
        public static System.Action<string> action;

        public static void Run(string msg)
        {
            System.Console.WriteLine(msg);
        }
    }
}
