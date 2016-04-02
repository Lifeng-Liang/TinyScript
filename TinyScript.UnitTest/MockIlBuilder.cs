using System;
using System.Reflection;
using System.Reflection.Emit;
using TinyScript.Builder;

namespace TinyScript.UnitTest
{
    public class MockIlBuilder : IlBuilder
    {
        private static MethodInfo _print = typeof(MockIlBuilder).GetMethod("Print");
        private static int _index = 0;

        public Type GenType;

        public MockIlBuilder() : base("UnitTestAssm.dll", "p" + _index)
        {
            _index++;
        }

        protected override MethodBuilder BuildMain()
        {
            return _program.DefineMethod("Main", MethodAttributes.Static | MethodAttributes.Public, null, new Type[0]);
        }

        public override void EmitPrint()
        {
            _builder.Emit(OpCodes.Call, _print);
        }

        public override void Save()
        {
            _builder.Emit(OpCodes.Ret);
            GenType = _program.CreateType();
        }

        public static void Print(string msg)
        {
            MockChannel.Instance.Print(msg);
        }
    }
}
