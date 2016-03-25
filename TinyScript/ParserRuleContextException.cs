using Antlr4.Runtime;
using System;
using System.Text;

namespace TinyScript
{
    public class ParserRuleContextException : Exception
    {
        public ParserRuleContextException(ParserRuleContext ctx, string template, params object[] args)
            : base(Format(ctx, template, args))
        {
        }

        private static string Format(ParserRuleContext ctx, string template, params object[] args)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("[{0},{1}] ", ctx.Start.Line, ctx.Start.Column);
            sb.AppendFormat(template, args);
            return sb.ToString();
        }
    }
}
