using NUnit.Framework;

namespace TinyScript.UnitTest
{
    [TestFixture]
    public class ExpressionJitCompilerTest : ExpressionTest
    {
        protected override void AssertIs(string script, string exp)
        {
            var builder = new MockIlBuilder();
            var r = new Runner(new CompilerVisitor(builder));
            r.Run(script);
            builder.GenType.RunMain();
            AssertIs(exp);
        }
    }
}
