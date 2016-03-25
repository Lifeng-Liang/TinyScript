using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using System.Collections.Generic;

namespace TinyScript
{
    public class InterpreterVisitor : TinyScriptBaseVisitor<object>
    {
        private Dictionary<string, object> Variables = new Dictionary<string, object>();
        private Channel _channel;

        public InterpreterVisitor(Channel channel)
        {
            _channel = channel;
        }

        public override object VisitDeclareExpression([NotNull] TinyScriptParser.DeclareExpressionContext context)
        {
            var assigns = context.declarators().assign();
            foreach (var assign in assigns)
            {
                var name = assign.Identifier().GetText();
                switch (context.basicType().GetText())
                {
                    case "decimal":
                        Variables.Add(name, (decimal)0);
                        break;
                    case "string":
                        Variables.Add(name, "");
                        break;
                    case "bool":
                        Variables.Add(name, false);
                        break;
                    case "var":
                        Variables.Add(name, null);
                        break;
                }
            }
            return base.VisitDeclareExpression(context);
        }

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

        public override object VisitPrintStatement([NotNull] TinyScriptParser.PrintStatementContext context)
        {
            var r = VisitExpression(context.expression());
            _channel.Print(r);
            return null;
        }

        public override object VisitUnaryExpression([NotNull] TinyScriptParser.UnaryExpressionContext context)
        {
            if (context.ChildCount > 1)
            {
                switch (context.GetChild(0).GetText())
                {
                    case "-": return -((decimal)VisitUnaryExpression(context.unaryExpression()));
                    case "!": return !((bool)VisitUnaryExpression(context.unaryExpression()));
                }
            }
            return VisitPrimaryExpression(context.primaryExpression());
        }

        public override object VisitExpression([NotNull] TinyScriptParser.ExpressionContext context)
        {
            var a = VisitAndAndExpression(context.andAndExpression(0));
            for (int i = 1; i < context.ChildCount; i += 2)
            {
                var b = (bool)VisitAndAndExpression((TinyScriptParser.AndAndExpressionContext)context.GetChild(i + 1));
                a = (bool)a || b;
            }
            return a;
        }

        public override object VisitAndAndExpression([NotNull] TinyScriptParser.AndAndExpressionContext context)
        {
            var a = VisitCmpExpression(context.cmpExpression(0));
            for (int i = 1; i < context.ChildCount; i += 2)
            {
                var b = (bool)VisitCmpExpression((TinyScriptParser.CmpExpressionContext)context.GetChild(i + 1));
                a = (bool)a && b;
            }
            return a;
        }

        public override object VisitAddExpression([NotNull] TinyScriptParser.AddExpressionContext context)
        {
            var a = VisitMulExpression(context.mulExpression(0));
            for (int i = 1; i < context.ChildCount; i += 2)
            {
                var op = context.GetChild(i).GetText();
                var b = VisitMulExpression((TinyScriptParser.MulExpressionContext)context.GetChild(i + 1));
                switch (op)
                {
                    case "+":
                        if (a is string || b is string)
                        {
                            a = a.ToString() + b.ToString();
                        }
                        else
                        {
                            a = (decimal)a + (decimal)b;
                        }
                        break;
                    case "-":
                        a = (decimal)a - (decimal)b;
                        break;
                }
            }
            return a;
        }

        public override object VisitMulExpression([NotNull] TinyScriptParser.MulExpressionContext context)
        {
            var a = VisitUnaryExpression(context.unaryExpression(0));
            for (int i = 1; i < context.ChildCount; i += 2)
            {
                var op = context.GetChild(i).GetText();
                var b = (decimal)VisitUnaryExpression((TinyScriptParser.UnaryExpressionContext)context.GetChild(i + 1));
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

        public override object VisitPrimaryExpression([NotNull] TinyScriptParser.PrimaryExpressionContext context)
        {
            if (context.ChildCount == 1)
            {
                var c = context.GetChild(0);
                if (c is TinyScriptParser.VariableExpressionContext)
                {
                    return VisitVariableExpression((TinyScriptParser.VariableExpressionContext)c);
                }
                else
                {
                    return VisitNumericLiteral(context.numericLiteral());
                }
            }
            else
            {
                return VisitExpression(context.expression());
            }
        }

        public override object VisitQuoteExpr([NotNull] TinyScriptParser.QuoteExprContext context)
        {
            return VisitExpression(context.expression());
        }

        public override object VisitNumericLiteral([NotNull] TinyScriptParser.NumericLiteralContext context)
        {
            var text = context.Decimal().GetText().Replace("_", "");
            return decimal.Parse(text);
        }

        public override object VisitVariableExpression([NotNull] TinyScriptParser.VariableExpressionContext context)
        {
            var n = context.GetText();
            if (n == "true")
            {
                return true;
            }
            else if (n == "false")
            {
                return false;
            }
            else if (n.StartsWith("\""))
            {
                return n.Substring(1, n.Length - 2);
            }
            object obj;
            if (!Variables.TryGetValue(n, out obj))
            {
                throw context.Exception("Use of undeclared identifier [{0}]", n);
            }
            return obj;
        }

        public override object VisitCmpExpression([NotNull] TinyScriptParser.CmpExpressionContext context)
        {
            var a = VisitAddExpression(context.addExpression(0));
            if (context.ChildCount > 2)
            {
                var b = VisitAddExpression(context.addExpression(1));
                var op = context.GetChild(1).GetText();
                switch (op)
                {
                    case "==":
                        return ValueEquals(a, b, context);
                    case "!=":
                        return !ValueEquals(a, b, context);
                    case "<":
                        return (decimal)a < (decimal)b;
                    case "<=":
                        return (decimal)a <= (decimal)b;
                    case ">":
                        return (decimal)a > (decimal)b;
                    case ">=":
                        return (decimal)a >= (decimal)b;
                }
                throw context.Exception("Unsupported operation.");
            }
            return a;
        }

        private bool ValueEquals(object a, object b, ParserRuleContext ctx)
        {
            if(a is bool && b is bool)
            {
                return (bool)a == (bool)b;
            }
            else if(a is decimal && b is decimal)
            {
                return (decimal)a == (decimal)b;
            }
            else if(a is string && b is string)
            {
                return (string)a == (string)b;
            }
            throw ctx.Exception("Cannot compare [{0}] and [{1}]", a.GetType().Name, b.GetType().Name);
        }

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

        public override object VisitWhileStatement([NotNull] TinyScriptParser.WhileStatementContext context)
        {
            while (true)
            {
                var condition = (bool)VisitExpression(context.expression());
                if (!(condition)) { break; }
                VisitBlockStatement(context.blockStatement());
            }
            return null;
        }

        public override object VisitDoWhileStatement([NotNull] TinyScriptParser.DoWhileStatementContext context)
        {
            bool condition;
            do
            {
                VisitBlockStatement(context.blockStatement());
                condition = (bool)VisitExpression(context.expression());
            } while (condition);
            return null;
        }

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
    }
}
