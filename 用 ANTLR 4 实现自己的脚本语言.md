用 ANTLR 4 实现自己的脚本语言
==========

ANTLR 是一个 Java 实现的词法/语法分析生成程序，目前最新版本为 4.5.2，支持 Java，C#，JavaScript 等语言，这里我们用 ANTLR 4.5.2 来实现一个自己的脚本语言。

因为某些未知原因，ANTLR 官方的文档似乎有些地方和 4.5.2 版的实际情况不太吻合，所以，有些部分，我们必须多方查找和自己实践得到，所幸 ANTLR 的文档比较丰富，其在 Github 上例子程序也很多，足够我们探索的了。

如果你没有编译原理的基础，只要写过正则表达式，应该也能很快理解其规则，进而编写自己的规则文件，事实上，因为结构更清晰， ANTLR 的规则文件，比正则表达式要简单得多。

我使用 C# 版本，所以下载了 antlr-4.5.2-complete.jar 和 C# 的支持库 Antlr4.Runtime.dll。 

* ANTLR 官方网址 http://www.antlr.org/
* ANTLR 官方 Github https://github.com/antlr/antlr4
* 大量语法文件例子 https://github.com/antlr/grammars-v4

因为文章中不适合贴全部的代码，建议下载了 TinyScript 的代码后，和此文章对照阅读和实践。

* 本文程序的 Github https://github.com/Lifeng-Liang/TinyScript

好了，进入正题，我们要定义一个解释型的脚本语言，就起个名叫 TinyScript 好了，规则文件名 TinyScript.g4 ，简单起见，暂不实现函数，具体实现的功能如下：

1. 变量，支持的数据类型为 decimal，bool，string，不支持 null
2. 变量赋值支持自动类型推断，用 var 标识
3. 四则运算，支持字符串通过 + 进行连接
4. 支持比较运算符，支持与或非运算符
5. if 语句，语句块必须用大括号包裹
6. while，do/while，for 循环，同样语句块必须用大括号包裹
7. 一个内置的输出函数 print，可以输出表达式的值到控制台

先说四则运算。四则运算里，除了括号外，需要先乘除，后加减，这个规则在 ANTLR 里怎么实现呢？

在 ANTLR 里，我们写的规则，会生成解析器的代码，这个解析器，会把目标脚本，解析成一个抽象语法树。这颗抽象语法树上，越是靠近叶子节点的地方，结合优先级越高，越是靠近根的地方，结合优先级越低，根据这个特点，我们就可以让 ANTLR 帮我们完成以上的规则：

````
addExpression
	: mulExpression (('+' | '-') mulExpression)*
	;
mulExpression
	: primaryExpression (('*' | '/') primaryExpression)*
	;
primaryExpression
	: Decimal
	| '(' addExpression ')'
	;
````

上面展示的 ANTLR 规则，在 primaryExpression 中，包括两个可选项，要么是数字，要么是括号表达式，是最高优先级，然后是 mulExpression，优先级最低的是 addExpression 。括号表达式内，是一个 addExpression ，所以，这是一个循环结构，可以处理无限长的四则运算式，比如 1+2*3-(4+5)/6+7+8，会被解析为如下的语法树：

addExpression				: 1 + child1_1 - child1_2 + 7 + 8
child1_1 mulExpression		: 2 * 3
child1_2 mulExpression		: child1_2_1 / 6
child1_2_1 addExpression	: 4 + 5

以上的语法树，其实是我简化了的，比如，其中的数字 1 其实应该是 mulExpression，而这个 mulExpression 只有一项 primaryExpression，而这个 primaryExpression，是 Decimal，其值为 1 。

PS: 在 ANTLR 中，大写字母开头的标识符，如上面的 Decimal，是词法分析器解析的，而小写字母开头的标识符，如 addExpression，是语法分析器解析的，它可以通过 override Visitor 的相应函数，改成我们自己的处理。因为缺省情况下，ANTLR 4 生成的是 listener，而我想要使用 visitor，所以命令行输入为： java -jar C:\Projects\ScriptParser\ts\antlr-4.5.2-complete.jar -visitor -no-listener TinyScript.g4

用上面的命令生成代码后，我们需要知道怎么才能启动它，可惜这里，至少对于 C#，文档写的要么不全，要么不正确，最后，我找到了正确的打开方式：

````
using (var ais = new AntlrInputStream(new FileStream(fileName, FileMode.Open)))
{
	var lexer = new TinyScriptLexer(ais);
	var tokens = new CommonTokenStream(lexer);
	var parser = new TinyScriptParser(tokens);
	parser.BuildParseTree = true;
	var tree = parser.program();
	var visitor = new MyVisitor();
	visitor.Visit(tree);
}
````

上面的 MyVisitor，是我们需要实现的，它从生成的 TinyScriptBaseVisitor 继承， TinyScriptBaseVisitor 是个泛型类，研究后，它的泛型参数是设计用来传递返回值的，因为要支持多种数据类型，所以我把它定义为 object 。

在实现 MyVisitor 时，只要每个节点都做好自己的工作就可以了。下面我们以 VisitMulExpression 函数来简单介绍一下如何实现乘除运算：

````
public override object VisitMulExpression([NotNull] TinyScriptParser.MulExpressionContext context)
{
	var a = VisitPrimaryExpression(context.primaryExpression(0));
	for (int i = 1; i < context.ChildCount; i += 2)
	{
		var op = context.GetChild(i).GetText();
		var b = (decimal)VisitPrimaryExpression((TinyScriptParser.PrimaryExpressionContext)context.GetChild(i + 1));
		switch (op)
		{
			case "*":
				a = (decimal)a * b;
				break;
			case "/":
				a = (decimal)a / b;
				break;
		}
	}
	return a;
}
````

因为 mulExpression 的定义中，至少有一个 primaryExpression，然后，可以有任意多乘除运算符及相应的 primaryExpression ，对应在 VisitMulExpression 函数中，就是第一个子节点是 primaryExpression ，（如果有的话）第二个子节点是运算符，第三个子节点是 primaryExpression，第四个子节点是运算符……所以，上面的代码，先通过 VisitPrimaryExpression 取出第一个节点值，保存在变量 a 中，然后，通过循环获取运算符和另一个值，并进行相应的运算，并把结果保存在 a 中，最后把运算结果 a 返回。因为在 VisitMulExpression 中，只会处理乘除运算，它们是同等的优先级，我们也就不用考虑这个问题，直接运算下去就可以了。

要注意的是，如果 mulExpression 只有一个 primaryExpression 节点，它就不一定是 decimal ，所以 a 的类型是 object ，而在进行运算时，才会把它强制类型转换成 decimal，因为这时我们已经确定它是 decimal 类型了。

PS：在这里，我们有两种方式取得子节点的值，如果定义中用了标识符，就可以直接使用这个标识符名作为函数调用，如上面的 context.primaryExpression(0) ，表示取第一个 primaryExpression ；另一种方法是调用 GetChild 函数，GetChild 函数因为是通用函数，所以经常需要强制类型转换为我们需要的类型。

下面，我们来说说变量定义及自动类型推断。

为了实现变量，我们在我们的 Visitor 中定义一个 Dictionary 类型的变量 Variables ，用来保存变量和它的值，在 VisitDeclareExpression 函数中，根据变量类型，在 Variables 中插入相应的键值对，然后，在赋值时，检查要被赋值的表达式的值的类型，是否和 Variables 中的一致，如果不一致，则抛出异常。

````
public override object VisitAssign([NotNull] TinyScriptParser.AssignContext context)
{
	var name = context.Identifier().GetText();
	object obj;
	if (!Variables.TryGetValue(name, out obj))
	{
		throw context.Exception("Variable [{0}] should be definded first.", name);
	}
	var r = base.VisitAssign(context);
	if (obj != null)
	{
		if (obj.GetType() != r.GetType())
		{
			throw context.Exception("Cannot assign [{1}] type value to a variable with type [{0}].", obj.GetType().Name, r.GetType().Name);
		}
	}
	Variables[name] = r;
	return null;
}
````

当然，我们也可以选择不在乎赋值语句两边是否类型相同，这样，它的行为方式就和很多脚本语言如 JavaScript 比较类似，变量在使用中可以改变类型。

不知道你是否注意到了，在上面的描述中，我们说到，我们其实知道表达式的结果的类型，并能在类型不匹配的时候抛出异常，那么，如果我们选择在定义类型时，如果变量类型是 var 的话，我们就不处理类型不匹配的问题，就是实现了自动类型推断！有点小颠覆吧？似乎很高级的这个语言特性，其实是顺理成章就可以得到的，不需要什么高大上的技术。在我们的脚本里，要做到这一点，只要在 VisitDeclareExpression 函数中，遇到 var 时，在插入变量时，变量值是 null 就可以了。

下面，我们再来看看 if 语句的处理，我们顶一个一个必须用大括号包裹的语句组类型 blockStatement ， if 语句定义如下：

````
ifStatement
	: 'if' quoteExpr blockStatement
	| 'if' quoteExpr blockStatement 'else' blockStatement
	;
````

当然，其实，上面的定义和下面这种写法是等价的：

````
ifStatement
	: 'if' quoteExpr blockStatement ('else' blockStatement)?
	;
````

然后，我们在 VisitIfStatement 函数中，真的写一个 if 语句，用来执行不同的 blockStatement 就可以了：

````
public override object VisitIfStatement([NotNull] TinyScriptParser.IfStatementContext context)
{
	var condition = (bool)VisitQuoteExpr(context.quoteExpr());
	if (condition)
	{
		VisitBlockStatement(context.blockStatement(0));
	}
	else if (context.ChildCount == 5)
	{
		VisitBlockStatement(context.blockStatement(1));
	}
	return null;
}
````

最后那个 return null 是表明，我们的 if 语句不产生任何值。加上对 Visitor 内取值遍历等的理解，这个 if 语句的处理是否看起来非常清晰明了？

最后，来看看循环语句，我们以 for 循环为例，先看定义：

````
forStatement
	: 'for' '(' commonExpression ';' expression ';' assignAbleStatement ')' blockStatement
	;
````

再看实现：

````
public override object VisitForStatement([NotNull] TinyScriptParser.ForStatementContext context)
{
	for (VisitCommonExpression(context.commonExpression());
		(bool)VisitExpression(context.expression());
		VisitAssignAbleStatement(context.assignAbleStatement()))
	{
		VisitBlockStatement(context.blockStatement());
	}
	return null;
}
````

嗯，你没看错，我们真的用了一个 for 循环来实现 for 循环 :)

好了，如果你下载了整个程序，并编译成功，我们现在可以编写一些脚本来做测试了，比如下面这个计算 1 到 100 的和的程序 sum.ts ：

````
var sum = 0;
for(var i=1; i<=100; i=i+1) {
	sum = sum + i;
}
print("sum 1 to 100 is : " + sum);
````

运行 ts sum.ts ，控制台输出：

````
sum 1 to 100 is : 5050
````

当然，这个脚本语言功能还比较弱，比如不支持函数，比如字符串不支持转义符等；也有一些实现的不太严格地方，比如强制类型转换如果出错，出错信息不准确等。不过，它是一个好的开始，可以让我们在此基础上，设计更完善、易用的语言。
