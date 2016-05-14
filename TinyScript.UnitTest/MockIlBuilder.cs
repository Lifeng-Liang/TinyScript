using System.Reflection;
using System.Reflection.Emit;

namespace TinyScript.UnitTest
{
    public class MockIlBuilder : JitBuilder
    {
        private static readonly MethodInfo _print = typeof(MockIlBuilder).GetMethod("Print");

        public override void EmitPrint()
        {
            Builder.Emit(OpCodes.Call, _print);
        }

        public static void Print(string msg)
        {
            MockChannel.Instance.Print(msg);
        }
    }
}
