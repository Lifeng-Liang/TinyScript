using System;
using System.IO;
using TinyScript.Builder;

namespace TinyScript
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length == 0)
            {
                ShowHelp();
                return;
            }
            try
            {
                using (var s = new FileStream(args[0], FileMode.Open))
                {
                    if(args.Length == 2)
                    {
                        if(args[1].StartsWith("-out:"))
                        {
                            var exeName = args[1].Substring(5);
                            if(!string.IsNullOrEmpty(exeName))
                            {
                                var builder = new IlBuilder(exeName);
                                var v = new CompilerVisitor(builder);
                                Run(v, s);
                                return;
                            }
                        }
                        ShowHelp();
                    }
                    else
                    {
                        Run(new InterpreterVisitor(new Channel()), s);
                    }
                }
            }
            catch (ParserRuleContextException ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static void Run(TinyScriptBaseVisitor<object> visitor, Stream s)
        {
            var r = new Runner(visitor);
            r.Run(s);
        }

        private static void ShowHelp()
        {
            Console.WriteLine(@"Usage :
    ts ScriptFileName [-out:output.exe]
Example:
    ts test.ts
    ts test.ts -out:test.exe");
        }
    }
}
