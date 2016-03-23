using Antlr4.Runtime;
using System;
using System.IO;
using System.Text;

namespace TinyScript
{
    public enum RunnerType
    {
        Interpreter,
        Compiler
    }

    public class Runner
    {
        private ITinyScriptVisitor<object> _visitor;

        public Runner(Channel channel, RunnerType type = RunnerType.Interpreter)
        {
            switch (type)
            {
                case RunnerType.Interpreter:
                    _visitor = new InterpreterVisitor(channel);
                    break;
                default:
                    throw new Exception("Unsupported Runner Type.");
            }
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
        }
    }
}
