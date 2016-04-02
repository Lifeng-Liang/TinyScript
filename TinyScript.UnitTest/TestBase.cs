using NUnit.Framework;

namespace TinyScript.UnitTest
{
    public class TestBase
    {
        [SetUp]
        public void SetUp()
        {
            MockChannel.Instance.Cache.Clear();
        }

        protected void AssertIs(string exp)
        {
			var expect = (exp + "\r\n").Replace("\r", "");
			var act = MockChannel.Instance.Cache.ToString().Replace("\r", "");
            Assert.AreEqual(expect, act);
            MockChannel.Instance.Cache.Clear();
        }

		protected virtual void AssertIs(string script,  string exp)
		{
            var r = new Runner(new InterpreterVisitor(MockChannel.Instance));
            r.Run(script);
			AssertIs(exp);
		}
    }
}
