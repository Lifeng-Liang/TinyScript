using System;
using System.IO;

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
                    var r = new Runner(new Channel());
                    r.Run(s);
                }
            }
            catch (ParserRuleContextException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void ShowHelp()
        {
            Console.WriteLine(@"Usage :
    ts ScriptFileName
Example:
    ts test.ts");
        }
    }
}
