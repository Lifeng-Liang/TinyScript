using Antlr4.Runtime;
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace TinyScript.Builder
{
    public class BooleanOpBuilder : OpBuilder
    {
        private static Type _handleType = typeof(bool);
        private static MethodInfo _tostr = _handleType.GetMethod("ToString", new Type[0]);

        public BooleanOpBuilder(ParserRuleContext ctx, IlBuilder builder) : base(ctx, builder)
        {
        }

        protected override void CheckEq()
        {
            Il.Ceq();
        }

        protected override void CheckNeq()
        {
            Il.Ceq();
            ReverseOp();
        }

        protected override void DoNot()
        {
            ReverseOp();
        }

        public override void CallToString()
        {
            var variable = Il.DeclareLocal(_handleType);
            Il.SetLocal(variable);
            Il.LoadLocalAddress(variable);
            Il.CallVirtual(_tostr);
        }
    }
}
