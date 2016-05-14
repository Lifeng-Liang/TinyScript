using Antlr4.Runtime;
using System;

namespace TinyScript
{
    public static class CoreHelper
    {
        public static ParserRuleContextException Exception(this ParserRuleContext ctx, string template, params object[] args)
        {
            return new ParserRuleContextException(ctx, template, args);
        }

        public static void Log(this ParserRuleContext ctx)
        {
            Console.WriteLine(">>>[{0},{1}][{2}] ChildCount : {3}, Text : {4}",
                ctx.Start.Line, ctx.Start.Column, ctx.GetType().Name, ctx.ChildCount, ctx.GetText());
        }

        public static void RunMain(this Type type)
        {
            var main = type.GetMethod("Main");
            main.Invoke(null, new object[0]);
        }
    }
}
