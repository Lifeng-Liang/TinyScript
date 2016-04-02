using NUnit.Framework;

namespace TinyScript.UnitTest
{
    [TestFixture]
    public class ScriptCompilerTest : TestScript
    {
        protected override void AssertIs(string script, string exp)
        {
            var builder = new MockIlBuilder();
            var r = new Runner(new CompilerVisitor(builder));
            r.Run(script);
            var main = builder.GenType.GetMethod("Main");
            main.Invoke(null, new object[0]);
            AssertIs(exp);
        }
    }
}
