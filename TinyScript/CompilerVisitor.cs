using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime;
using TinyScript.Builder;

namespace TinyScript
{
    public class CompilerVisitor :  TinyScriptBaseVisitor<object>, ISaveable
    {
        public class Variable
        {
            public Type Type;
            public LocalBuilder Value;
        }

        private readonly IlBuilder _builder;
        private readonly Dictionary<string, Variable> _variables = new Dictionary<string, Variable>();

        public CompilerVisitor(IlBuilder builder)
        {
            _builder = builder;
        }

        public void Save()
        {
            _builder.Save();
        }

        public override object VisitDeclareExpression([NotNull] TinyScriptParser.DeclareExpressionContext context)
        {
            var type = GetDeclType(context, context.basicType().GetText());
            foreach (var assign in context.declarators().assign())
            {
                var name = assign.Identifier().GetText();
                Variable obj;
                if(_variables.TryGetValue(name, out obj))
                {
                    throw context.Exception("Variable [{0}] already defined.", name);
                }
                _variables.Add(name, new Variable { Type = type });
            }
            return VisitDeclarators(context.declarators());
        }

        private Type GetDeclType(ParserRuleContext context, string typeName)
        {
            switch(typeName)
            {
                case "decimal": return typeof(decimal);
                case "bool": return typeof(bool);
                case "string": return typeof(string);
                case "var": return null;
            }
            throw context.Exception("Unknown type declare : [{0}]", typeName);
        }

        public override object VisitDeclarators([NotNull] TinyScriptParser.DeclaratorsContext context)
        {
            base.VisitDeclarators(context);
            return null;
        }

        public override object VisitAssign([NotNull] TinyScriptParser.AssignContext context)
        {
            var type = (Type)VisitExpression(context.expression());
            var name = context.Identifier().GetText();
            Variable value;
            if(_variables.TryGetValue(name, out value))
            {
                if (value.Value == null)
                {
                    LocalBuilder variable;
                    if(value.Type == null)
                    {
                        variable = _builder.DeclareLocal(type);
                        value.Type = type;
                    }
                    else if(value.Type != type)
                    {
                        throw context.Exception("Cannot convert type [{0}] to [{1}].", type.Name, value.Type.Name);
                    }
                    else
                    {
                        variable = _builder.DeclareLocal(type);
                    }
                    value.Value = variable;
                }
                _builder.SetLocal(value.Value);
                return null;
            }
            throw context.Exception("Variable [{0}] not defined.", name);
        }

        public override object VisitExpression([NotNull] TinyScriptParser.ExpressionContext context)
        {
            var ret = VisitAndAndExpression(context.andAndExpression(0));
            if(context.ChildCount > 1)
            {
                for (int i = 2; i < context.ChildCount; i+=2)
                {
                    VisitAndAndExpression((TinyScriptParser.AndAndExpressionContext)context.GetChild(i));
                    _builder.Or();
                }
                return typeof(bool);
            }
            return ret;
        }

        public override object VisitAndAndExpression([NotNull] TinyScriptParser.AndAndExpressionContext context)
        {
            var ret = VisitCmpExpression(context.cmpExpression(0));
            if (context.ChildCount > 1)
            {
                for (int i = 2; i < context.ChildCount; i += 2)
                {
                    VisitCmpExpression((TinyScriptParser.CmpExpressionContext)context.GetChild(i));
                    _builder.And();
                }
                return typeof(bool);
            }
            return ret;
        }

        public override object VisitCmpExpression([NotNull] TinyScriptParser.CmpExpressionContext context)
        {
            var ret = (Type)VisitAddExpression(context.addExpression(0));
            if (context.ChildCount > 1)
            {
                var t2 = (Type)VisitAddExpression(context.addExpression(1));
                var op = context.GetChild(1).GetText();
                var cmpType = GetOpType(context, ret, t2);
                var b = OpBuilder.GetOpBuilder(cmpType, context, _builder);
                b.ProcessCmp(op);
                return typeof(bool);
            }
            return ret;
        }

        private Type GetOpType(ParserRuleContext ctx, Type t1, Type t2)
        {
            if(t1 == t2)
            {
                return t1;
            }
            throw ctx.Exception("Cannot do operation between [{0}] and [{1}].", t1.Name, t2.Name);
        }

        public override object VisitAddExpression([NotNull] TinyScriptParser.AddExpressionContext context)
        {
            var ret = (Type)VisitMulExpression(context.mulExpression(0));
            if(context.ChildCount > 1)
            {
                Type typeAdd = null;
                for (int i = 1; i < context.ChildCount; i+=2)
                {
                    var op = context.GetChild(i).GetText();
                    var t2 = (Type)VisitMulExpression((TinyScriptParser.MulExpressionContext)context.GetChild(i + 1));
                    typeAdd = ret == typeof(string) ? ret : GetOpType(context, ret, t2);
                    if(ret == typeof(string) && t2.IsValueType)
                    {
                        _builder.Box(t2);
                    }
                    var b = OpBuilder.GetOpBuilder(typeAdd, context, _builder);
                    b.ProcessAdd(op);
                }
                return typeAdd;
            }
            return ret;
        }

        public override object VisitMulExpression([NotNull] TinyScriptParser.MulExpressionContext context)
        {
            var ret = (Type)VisitUnaryExpression(context.unaryExpression(0));
            if (context.ChildCount > 1)
            {
                Type typeAdd = null;
                for (int i = 1; i < context.ChildCount; i+=2)
                {
                    var op = context.GetChild(i).GetText();
                    var t2 = (Type)VisitUnaryExpression((TinyScriptParser.UnaryExpressionContext)context.GetChild(i + 1));
                    typeAdd = GetOpType(context, ret, t2);
                    var b = OpBuilder.GetOpBuilder(typeAdd, context, _builder);
                    b.ProcessMul(op);
                }
                return typeAdd;
            }
            return ret;
        }

        public override object VisitUnaryExpression([NotNull] TinyScriptParser.UnaryExpressionContext context)
        {
            if(context.ChildCount == 1)
            {
                return VisitPrimaryExpression(context.primaryExpression());
            }
            var ret = (Type)VisitUnaryExpression(context.unaryExpression());
            var op = context.GetChild(0).GetText();
            var b = OpBuilder.GetOpBuilder(ret, context, _builder);
            b.ProcessUnary(op);
            return ret;
        }

        public override object VisitPrimaryExpression([NotNull] TinyScriptParser.PrimaryExpressionContext context)
        {
            if(context.ChildCount == 1)
            {
                var c = context.GetChild(0);
                if(c is TinyScriptParser.VariableExpressionContext)
                {
                    return VisitVariableExpression(context.variableExpression());
                }
                var num = context.numericLiteral().GetText().Replace("_", "");
                var b = OpBuilder.GetOpBuilder(typeof(decimal), context, _builder);
                b.LoadNum(num);
                return typeof(decimal);
            }
            return VisitExpression(context.expression());
        }

        public override object VisitVariableExpression([NotNull] TinyScriptParser.VariableExpressionContext context)
        {
            var c = context.GetChild(0);
            var text = c.GetText();
            switch(text)
            {
                case "true": _builder.LoadInt(1); return typeof(bool);
                case "false": _builder.LoadInt(0); return typeof(bool);
            }
            if(text.StartsWith("\""))
            {
                var str = text.Substring(1, text.Length - 2);
                _builder.LoadString(str);
                return typeof(string);
            }
            Variable variable;
            if(_variables.TryGetValue(text, out variable))
            {
                _builder.LoadLocal(variable.Value);
                return variable.Type;
            }
            throw context.Exception("Use of undeclared identifier [{0}].", text);
        }

        public override object VisitPrintStatement([NotNull] TinyScriptParser.PrintStatementContext context)
        {
            var ret = (Type)VisitExpression(context.expression());
            var b = OpBuilder.GetOpBuilder(ret, context, _builder);
            b.CallToString();
            _builder.EmitPrint();
            return null;
        }

        public override object VisitIfStatement([NotNull] TinyScriptParser.IfStatementContext context)
        {
            VisitQuoteExpr(context.quoteExpr());
            var l1 = _builder.DefineLabel();
            _builder.BrFalse(l1);
            VisitBlockStatement(context.blockStatement(0));
            if(context.ChildCount == 5)
            {
                var l2 = _builder.DefineLabel();
                _builder.Br(l2);
                _builder.Nop();
                _builder.MarkLabel(l1);
                VisitBlockStatement(context.blockStatement(1));
                _builder.Nop();
                _builder.MarkLabel(l2);
            }
            else
            {
                _builder.Nop();
                _builder.MarkLabel(l1);
            }
            return null;
        }

        public override object VisitQuoteExpr([NotNull] TinyScriptParser.QuoteExprContext context)
        {
            AssertIsBool(context, VisitExpression(context.expression()));
            return typeof(bool);
        }

        private void AssertIsBool(ParserRuleContext ctx, object result)
        {
            if ((Type)result != typeof(bool))
            {
                throw ctx.Exception("Confition result of IF statement must be boolean.");
            }
        }

        public override object VisitWhileStatement([NotNull] TinyScriptParser.WhileStatementContext context)
        {
            var l1 = _builder.DefineLabel();
            _builder.MarkLabel(l1);
            AssertIsBool(context, VisitExpression(context.expression()));
            var l2 = _builder.DefineLabel();
            _builder.BrFalse(l2);
            VisitBlockStatement(context.blockStatement());
            _builder.Br(l1);
            _builder.MarkLabel(l2);
            _builder.Nop();
            return null;
        }

        public override object VisitDoWhileStatement([NotNull] TinyScriptParser.DoWhileStatementContext context)
        {
            var l1 = _builder.DefineLabel();
            _builder.MarkLabel(l1);
            VisitBlockStatement(context.blockStatement());
            AssertIsBool(context, VisitExpression(context.expression()));
            _builder.BrTrue(l1);
            return null;
        }

        public override object VisitForStatement([NotNull] TinyScriptParser.ForStatementContext context)
        {
            VisitCommonExpression(context.commonExpression());
            var l1 = _builder.DefineLabel();
            _builder.MarkLabel(l1);
            AssertIsBool(context, VisitExpression(context.expression()));
            var l2 = _builder.DefineLabel();
            _builder.BrFalse(l2);
            VisitBlockStatement(context.blockStatement());
            VisitAssignAbleStatement(context.assignAbleStatement());
            _builder.Br(l1);
            _builder.MarkLabel(l2);
            _builder.Nop();
            return null;
        }
    }
}
