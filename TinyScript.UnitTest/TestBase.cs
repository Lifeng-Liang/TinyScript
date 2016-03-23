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

		protected void AssertIs(Runner r, string script,  string exp)
		{
			r.Run(script);
			AssertIs(exp);
		}
    }
}
