using NUnit.Framework;

namespace TinyScript.UnitTest
{
    [TestFixture]
    public class ExpressionTest : TestBase
    {
        [Test]
		public void TestLiteral()
        {
            // decimal
			AssertIs("print(123);", "123");
			AssertIs("print(123_456);", "123456");
			AssertIs("print(123_456.789_012);", "123456.789012");
            // bool
			AssertIs("print(true);", "True");
			AssertIs("print(false);", "False");
            // string
			AssertIs(@"print(""this is a string."");", "this is a string.");
        }

		[Test, ExpectedException(typeof(ParserRuleContextException), ExpectedMessage = "[1,8] Cannot convert type [Boolean] to [Decimal].")]
		public void TestShouldNotAssignBoolValueToDecimalVariable()
		{
            AssertIs("decimal n = true;", "");
		}

		[Test, ExpectedException(typeof(ParserRuleContextException), ExpectedMessage = "[1,0] Variable [n] not defined.")]
		public void TestVariableNotDefined()
		{
            AssertIs("n = true;", "");
		}

        [Test, ExpectedException(typeof(ParserRuleContextException), ExpectedMessage = "[1,8] Use of undeclared identifier [y].")]
        public void TestVariableNotDefinedInExpression()
        {
            AssertIs("var x = y + 2;", "");
        }

        [Test, ExpectedException(typeof(ParserRuleContextException), ExpectedMessage = "[1,8] Cannot do operation between [Boolean] and [Decimal].")]
        public void TestCompareDifferentTypes()
        {
            AssertIs("var x = true == 123;", "");
        }

        [Test, ExpectedException(typeof(ParserRuleContextException), ExpectedMessage = "[1,8] Variable [y] already defined.")]
        public void TestCompareRedefineVariable()
        {
            AssertIs("var y=2;var y=1;", "");
        }

        [Test]
		public void TestExpression()
		{
            // decimal
            AssertIs("print( 3 > 4 );", "False");
			AssertIs("print( 3 > 3 );", "False");
			AssertIs("print( 3 < 4 );", "True");
			AssertIs("print( 3 < 3 );", "False");
			AssertIs("print( 3 >= 3 );", "True");
			AssertIs("print( 3 >= 4 );", "False");
			AssertIs("print( 3 <= 3 );", "True");
			AssertIs("print( 3 <= 4 );", "True");
			// bool
			AssertIs("print( true == true );", "True");
			AssertIs("print( true != true );", "False");
			AssertIs("print( true == false );", "False");
			AssertIs("print( true != false );", "True");
			AssertIs("print( true && true );", "True");
			AssertIs("print( true && false );", "False");
			AssertIs("print( true || true );", "True");
			AssertIs("print( true || false );", "True");
			AssertIs("print( false && false );", "False");
			AssertIs("print( false || false );", "False");
			// string
			AssertIs(@"print( ""123"" == ""123"" );", "True");
			AssertIs(@"print( ""123"" == ""1230"" );", "False");
			AssertIs(@"print( ""123"" != ""123"" );", "False");
			AssertIs(@"print( ""123"" != ""1230"" );", "True");
			// caculate
			AssertIs(@"print( 123 - 23);", "100");
			AssertIs(@"print( 3 + 3 * 5 / 2 );", "10.5");
			AssertIs(@"print( (3 + 3) * 5 / 2 );", "15");
			AssertIs(@"print( ""result : "" + 123);", "result : 123");
			AssertIs(@"print( 123.45 - 1.45 );", "122.00");
			AssertIs(@"print( 123.45 - 0.45 );", "123.00");
		}
    }
}
