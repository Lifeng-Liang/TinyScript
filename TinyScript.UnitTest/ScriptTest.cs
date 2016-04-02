using NUnit.Framework;

namespace TinyScript.UnitTest
{
    [TestFixture]
    public class TestScript : TestBase
    {
        [Test]
        public void TestAssianDeclareAndIf()
        {
            var script = @"var n = 1.5 + (2 + 3) * 2;
print(n);
print(3+4);
print(true);
print(1+2*n);
var x = true;
print(x);
print(x == false);
print(n > 5);
print(n > 105);

if( n > 18 ) {
	print("" n > 18"");
} else {
    print("" n <= 18"");
}

if( n > 5 )
{
    print(""n is greater than 5 !!!"");
}

print(true);

print(""test ok aaaa!"");

print(""The value of n is : "" + n);
print(2 + 3 - 1 - 5 * 3 / 2 - -8);
print(!(3>4));
print((2+3)*2);
print(true && true);
print(true && false);
print(true || true);
bool newValue = true || false;
print(newValue);
print( 3 != 2 );
print( 3 != 3 );
print( true == true );
print( true == false );
print( ""abc"" == ""abc"" );
print( ""abc"" == ""abc1"" );
print( 3 < 4 );
print( 3 >= 3 );
";
            AssertIs(script, @"11.5
7
True
24.0
True
False
True
False
 n <= 18
n is greater than 5 !!!
True
test ok aaaa!
The value of n is : 11.5
4.5
True
10
True
False
True
True
True
False
True
False
True
False
True
True");
        }

        [Test]
        public void TestSum1to100()
        {
            var script = @"
var sum = 0, i = 1;
while(i<=100)
{
	sum = sum + i;
	i = i + 1;
}

print(""sum 1 to 100 is : "" + sum);";
            AssertIs(script, "sum 1 to 100 is : 5050");
        }

        [Test]
        public void TestSum1to100ByDoWhile()
        {
            var script = @"
decimal sum = 0, i = 1;
do
{
	sum = sum + i;
	i = i + 1;
} while (i<=100);

string msg = ""sum 1 to 100 is : "" + sum;
print(msg);";
            AssertIs(script, "sum 1 to 100 is : 5050");
        }

        [Test]
        public void TestFib()
        {
            var script = @"var fib0 = 1;
var fib1 = 1;
var temp = 0;
for (var i = 1; i <= 10; i=i+1 ) {
	temp = fib1;
	fib1 = fib0 + fib1;
	fib0 = temp;
	print(fib1);
}
";
            AssertIs(script, @"2
3
5
8
13
21
34
55
89
144");
        }
    }
}
