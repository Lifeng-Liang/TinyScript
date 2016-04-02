using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace TinyScript.Builder
{
    public class OpBuilder
    {
        public static OpBuilder GetOpBuilder(Type type, ParserRuleContext ctx, IlBuilder builder)
        {
            if(type == typeof(bool))
            {
                return new BooleanOpBuilder(ctx, builder);
            }
            else if(type == typeof(string))
            {
                return new StringOpBuilder(ctx, builder);
            }
            else if(type == typeof(decimal))
            {
                return new DecimalOpBuilder(ctx, builder);
            }
            throw ctx.Exception("Unspported operation.");
        }

        protected ParserRuleContext Ctx;
        protected IlBuilder Il;

        public OpBuilder(ParserRuleContext ctx, IlBuilder builder)
        {
            Ctx = ctx;
            Il = builder;
        }

        protected void ReverseOp()
        {
            Il.LoadInt(0);
            Il.Ceq();
        }

        public void ProcessCmp(string op)
        {
            switch (op)
            {
                case "==": CheckEq(); break;
                case "!=": CheckNeq(); break;
                case "<": CheckLt(); break;
                case "<=": CheckLte(); break;
                case ">": CheckGt(); break;
                case ">=": CheckGte(); break;
            }
        }

        protected virtual void CheckEq()
        {
            throw Ctx.Exception("Sytex error.");
        }

        protected virtual void CheckNeq()
        {
            throw Ctx.Exception("Sytex error.");
        }

        protected virtual void CheckGt()
        {
            throw Ctx.Exception("Sytex error.");
        }

        protected virtual void CheckGte()
        {
            throw Ctx.Exception("Sytex error.");
        }

        protected virtual void CheckLt()
        {
            throw Ctx.Exception("Sytex error.");
        }

        protected virtual void CheckLte()
        {
            throw Ctx.Exception("Sytex error.");
        }

        public void ProcessAdd(string op)
        {
            switch(op)
            {
                case "+": DoAdd(); break;
                case "-": DoSub(); break;
            }
        }

        protected virtual void DoAdd()
        {
            throw Ctx.Exception("Sytex error.");
        }

        protected virtual void DoSub()
        {
            throw Ctx.Exception("Sytex error.");
        }

        public void ProcessMul(string op)
        {
            switch(op)
            {
                case "*": DoMul(); break;
                case "/": DoDiv(); break;
            }
        }

        protected virtual void DoMul()
        {
            throw Ctx.Exception("Sytex error.");
        }

        protected virtual void DoDiv()
        {
            throw Ctx.Exception("Sytex error.");
        }

        public void ProcessUnary(string op)
        {
            switch(op)
            {
                case "-": DoNeg(); break;
                case "!": DoNot(); break;
            }
        }

        protected virtual void DoNeg()
        {
            throw Ctx.Exception("Sytex error.");
        }

        protected virtual void DoNot()
        {
            throw Ctx.Exception("Sytex error.");
        }

        public virtual void LoadNum(string num)
        {
            throw Ctx.Exception("Sytex error.");
        }

        public virtual void CallToString()
        {
            throw Ctx.Exception("Sytex error.");
        }
    }
}
