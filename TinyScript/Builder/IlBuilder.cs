using System;
using System.Reflection;
using System.Reflection.Emit;

namespace TinyScript.Builder
{
    public class IlBuilder
    {
        private static MethodInfo _print = typeof(Console).GetMethod("WriteLine", new Type[] { typeof(string) });
        protected readonly AssemblyBuilder _innerAssembly;
        protected readonly ModuleBuilder _innerModule;
        protected TypeBuilder _program;
        protected MethodBuilder _main;
        protected readonly ILGenerator _builder;
        protected string _exeName;

        public IlBuilder(string exeName, string className = "Program")
        {
            _exeName = exeName;
            var an = new AssemblyName("TinyScript");
            _innerAssembly = AppDomain.CurrentDomain.DefineDynamicAssembly(
                an, AssemblyBuilderAccess.RunAndSave);
            _innerModule = _innerAssembly.DefineDynamicModule("TinyScript", _exeName);
            _builder = InitMain(className);
        }

        private ILGenerator InitMain(string className)
        {
            _program = _innerModule.DefineType(className, TypeAttributes.BeforeFieldInit | TypeAttributes.AutoClass | TypeAttributes.AnsiClass | TypeAttributes.NotPublic);
            var ctor = _program.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);
            _main = BuildMain();
            _innerAssembly.SetEntryPoint(_main);
            return _main.GetILGenerator();
        }

        protected virtual MethodBuilder BuildMain()
        {
            return _program.DefineMethod("Main", MethodAttributes.Static | MethodAttributes.Private | MethodAttributes.HideBySig, null, new[] { typeof(string[]) });
        }

        public virtual void Save()
        {
            _builder.Emit(OpCodes.Ret);
            _program.CreateType();
            _innerAssembly.Save(_exeName);
        }

        public LocalBuilder DeclareLocal(Type type)
        {
            return _builder.DeclareLocal(type);
        }

        public Label DefineLabel()
        {
            return _builder.DefineLabel();
        }

        public void MarkLabel(Label loc)
        {
            _builder.MarkLabel(loc);
        }

        public virtual void EmitPrint()
        {
            _builder.Emit(OpCodes.Call, _print);
        }

        public void LoadInt(int n)
        {
            switch(n)
            {
                case 0: _builder.Emit(OpCodes.Ldc_I4_0); return;
                case 1: _builder.Emit(OpCodes.Ldc_I4_1); return;
                case 2: _builder.Emit(OpCodes.Ldc_I4_2); return;
                case 3: _builder.Emit(OpCodes.Ldc_I4_3); return;
                case 4: _builder.Emit(OpCodes.Ldc_I4_4); return;
                case 5: _builder.Emit(OpCodes.Ldc_I4_5); return;
                case 6: _builder.Emit(OpCodes.Ldc_I4_6); return;
                case 7: _builder.Emit(OpCodes.Ldc_I4_7); return;
                case 8: _builder.Emit(OpCodes.Ldc_I4_8); return;
            }
            _builder.Emit(OpCodes.Ldc_I4, n);
        }

        public void LoadString(string text)
        {
            _builder.Emit(OpCodes.Ldstr, text);
        }

        public void LoadLocal(LocalBuilder local)
        {
            _builder.Emit(OpCodes.Ldloc, local);
        }

        public void LoadLocalAddress(LocalBuilder local)
        {
            _builder.Emit(OpCodes.Ldloca, local);
        }

        public void SetLocal(LocalBuilder local)
        {
            _builder.Emit(OpCodes.Stloc, local);
        }

        public void Nop()
        {
            _builder.Emit(OpCodes.Nop);
        }

        public void BrFalse(Label loc)
        {
            _builder.Emit(OpCodes.Brfalse, loc);
        }

        public void BrTrue(Label loc)
        {
            _builder.Emit(OpCodes.Brtrue, loc);
        }

        public void Br(Label loc)
        {
            _builder.Emit(OpCodes.Br, loc);
        }

        public void Call(MethodInfo info)
        {
            _builder.Emit(OpCodes.Call, info);
        }

        public void CallVirtual(MethodInfo info)
        {
            _builder.Emit(OpCodes.Callvirt, info);
        }

        public void Ceq()
        {
            _builder.Emit(OpCodes.Ceq);
        }

        public void Or()
        {
            _builder.Emit(OpCodes.Or);
        }

        public void And()
        {
            _builder.Emit(OpCodes.And);
        }

        public void Box(Type type)
        {
            _builder.Emit(OpCodes.Box, type);
        }
    }
}
