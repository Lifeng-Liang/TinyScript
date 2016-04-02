using Antlr4.Runtime;
using System.IO;
using System.Text;

namespace TinyScript
{
    public class Runner
    {
        private TinyScriptBaseVisitor<object> _visitor;

        public Runner(TinyScriptBaseVisitor<object> visitor)
        {
            _visitor = visitor;
        }

        public void Run(string script)
        {
            var bs = Encoding.UTF8.GetBytes(script);
            using (var s = new MemoryStream(bs))
            {
                Run(s);
            }
        }

        public void Run(Stream stream)
        {
            var ais = new AntlrInputStream(stream);
            var lexer = new TinyScriptLexer(ais);
            var tokens = new CommonTokenStream(lexer);
            var parser = new TinyScriptParser(tokens);
            parser.BuildParseTree = true;
            var tree = parser.program();
            _visitor.Visit(tree);
            var v = _visitor as ISaveable;
            if(v != null)
            {
                v.Save();
            }
        }
    }
}
