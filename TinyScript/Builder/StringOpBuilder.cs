using Antlr4.Runtime;
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace TinyScript.Builder
{
    public class StringOpBuilder : OpBuilder
    {
        private static Type _handleType = typeof(string);
        private static MethodInfo _eq = _handleType.GetMethod("op_Equality");
        private static MethodInfo _neq = _handleType.GetMethod("op_Inequality");
        private static MethodInfo _concat = _handleType.GetMethod("Concat", new Type[] { typeof(object), typeof(object) });

        public StringOpBuilder(ParserRuleContext ctx, IlBuilder builder) : base(ctx, builder)
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

        protected override void DoAdd()
        {
            Il.Call(_concat);
        }

        public override void CallToString()
        {
        }
    }
}
