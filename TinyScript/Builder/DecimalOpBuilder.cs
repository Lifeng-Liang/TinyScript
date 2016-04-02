using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace TinyScript.Builder
{
    public class DecimalOpBuilder : OpBuilder
    {
        private static Type _handleType = typeof(decimal);
        private static MethodInfo _gt = _handleType.GetMethod("op_GreaterThan");
        private static MethodInfo _lt = _handleType.GetMethod("op_LessThan");
        private static MethodInfo _gte = _handleType.GetMethod("op_GreaterThanOrEqual");
        private static MethodInfo _lte = _handleType.GetMethod("op_LessThanOrEqual");
        private static MethodInfo _eq = _handleType.GetMethod("op_Equality");
        private static MethodInfo _neq = _handleType.GetMethod("op_Inequality");
        private static MethodInfo _add = _handleType.GetMethod("op_Addition");
        private static MethodInfo _sub = _handleType.GetMethod("op_Subtraction");
        private static MethodInfo _mul = _handleType.GetMethod("op_Multiply");
        private static MethodInfo _div = _handleType.GetMethod("op_Division");
        private static MethodInfo _neg = _handleType.GetMethod("op_UnaryNegation");
        private static MethodInfo _parse = _handleType.GetMethod("Parse", new Type[] { typeof(string) });
        private static MethodInfo _tostr = _handleType.GetMethod("ToString", new Type[0]);

        public DecimalOpBuilder(ParserRuleContext ctx, IlBuilder builder) : base(ctx, builder)
        {
        }

        protected override void CheckEq()
        {
            Il.Call(_eq);
        }

        protected override void CheckNeq()
        {
            Il.Call(_neq);
        }

        protected override void CheckGt()
        {
            Il.Call(_gt);
        }

        protected override void CheckGte()
        {
            Il.Call(_gte);
        }

        protected override void CheckLt()
        {
            Il.Call(_lt);
        }

        protected override void CheckLte()
        {
            Il.Call(_lte);
        }

        protected override void DoAdd()
        {
            Il.Call(_add);
        }

        protected override void DoSub()
        {
            Il.Call(_sub);
        }

        protected override void DoMul()
        {
            Il.Call(_mul);
        }

        protected override void DoDiv()
        {
            Il.Call(_div);
        }

        protected override void DoNeg()
        {
            Il.Call(_neg);
        }

        public override void LoadNum(string num)
        {
            Il.LoadString(num);
            Il.Call(_parse);
        }

        public override void CallToString()
        {
            var variable = Il.DeclareLocal(_handleType);
            Il.SetLocal(variable);
            Il.LoadLocalAddress(variable);
            Il.Call(_tostr);
        }
    }
}
