using System;
using System.Reflection;
using System.Reflection.Emit;
using TinyScript.Builder;

namespace TinyScript
{
    public class JitBuilder : IlBuilder
    {
        private static int _index;
        public Type GenType;

        public JitBuilder() : base("Jit.exe", "p" + _index)
        {
            _index++;
        }

        protected override MethodBuilder BuildMain()
        {
            return Program.DefineMethod("Main", MethodAttributes.Static | MethodAttributes.Public, null, new Type[0]);
        }

        public override void Save()
        {
            Builder.Emit(OpCodes.Ret);
            GenType = Program.CreateType();
        }
    }
}
