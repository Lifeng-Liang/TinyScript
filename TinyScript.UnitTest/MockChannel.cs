using System.Text;

namespace TinyScript.UnitTest
{
    public class MockChannel : Channel
    {
        public static readonly MockChannel Instance = new MockChannel();

        public readonly StringBuilder Cache = new StringBuilder();

        private MockChannel()
        {
        }

        public override void Print(string message, params object[] args)
        {
            Cache.AppendFormat(message, args);
            Cache.AppendLine();
        }
    }
}
