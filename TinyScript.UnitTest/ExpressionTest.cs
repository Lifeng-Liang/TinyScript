using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace TinyScript.UnitTest
{
    [TestFixture]
    public class ExpressionTest : TestBase
    {
        [Test]
		public void TestLiteral()
        {
            var r = new Runner(MockChannel.Instance);
            // decimal
			AssertIs(r, "print(123);", "123");
			AssertIs(r, "print(123_456);", "123456");
			AssertIs(r, "print(123_456.789_012);", "123456.789012");
            // bool
			AssertIs(r, "print(true);", "True");
			AssertIs(r, "print(false);", "False");
            // string
			AssertIs(r, @"print(""this is a string."");", "this is a string.");
        }

		[Test, ExpectedException(typeof(ParserRuleContextException), ExpectedMessage = "[1,8] Cannot assign [Boolean] type value to a variable with type [Decimal].")]
		public void TestShouldNotAssignBoolValueToDecimalVariable()
		{
			var r = new Runner(MockChannel.Instance);
			r.Run("decimal n = true;");
		}

		[Test, ExpectedException(typeof(ParserRuleContextException), ExpectedMessage = "[1,0] Variable [n] should be definded first.")]
		public void TestVariableNotDefined()
		{
			var r = new Runner(MockChannel.Instance);
			r.Run("n = true;");
		}

        [Test, ExpectedException(typeof(ParserRuleContextException), ExpectedMessage = "[1,8] Use of undeclared identifier [y]")]
        public void TestVariableNotDefinedInExpression()
        {
            var r = new Runner(MockChannel.Instance);
            r.Run("var x = y + 2;");
        }

        [Test, ExpectedException(typeof(ParserRuleContextException), ExpectedMessage = "[1,8] Cannot compare [Boolean] and [Decimal]")]
        public void TestCompareDifferentTypes()
        {
            var r = new Runner(MockChannel.Instance);
            r.Run("var x = true == 123;");
        }

        [Test]
		public void TestExpression()
		{
			var r = new Runner(MockChannel.Instance);
			// decimal
			AssertIs(r, "print( 3 > 4 );", "False");
			AssertIs(r, "print( 3 > 3 );", "False");
			AssertIs(r, "print( 3 < 4 );", "True");
			AssertIs(r, "print( 3 < 3 );", "False");
			AssertIs(r, "print( 3 >= 3 );", "True");
			AssertIs(r, "print( 3 >= 4 );", "False");
			AssertIs(r, "print( 3 <= 3 );", "True");
			AssertIs(r, "print( 3 <= 4 );", "True");
			// bool
			AssertIs(r, "print( true == true );", "True");
			AssertIs(r, "print( true != true );", "False");
			AssertIs(r, "print( true == false );", "False");
			AssertIs(r, "print( true != false );", "True");
			AssertIs(r, "print( true && true );", "True");
			AssertIs(r, "print( true && false );", "False");
			AssertIs(r, "print( true || true );", "True");
			AssertIs(r, "print( true || false );", "True");
			AssertIs(r, "print( false && false );", "False");
			AssertIs(r, "print( false || false );", "False");
			// string
			AssertIs(r, @"print( ""123"" == ""123"" );", "True");
			AssertIs(r, @"print( ""123"" == ""1230"" );", "False");
			AssertIs(r, @"print( ""123"" != ""123"" );", "False");
			AssertIs(r, @"print( ""123"" != ""1230"" );", "True");
			// caculate
			AssertIs(r, @"print( 123 - 23);", "100");
			AssertIs(r, @"print( 3 + 3 * 5 / 2 );", "10.5");
			AssertIs(r, @"print( (3 + 3) * 5 / 2 );", "15");
			AssertIs(r, @"print( ""result : "" + 123);", "result : 123");
			AssertIs(r, @"print( 123.45 - 1.45 );", "122.00");
			AssertIs(r, @"print( 123.45 - 0.45 );", "123.00");
		}
    }
}
