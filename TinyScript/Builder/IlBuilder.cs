using System;
using System.Reflection;
using System.Reflection.Emit;

namespace TinyScript.Builder
{
    public class IlBuilder
    {
        private static readonly MethodInfo Print = typeof(Console).GetMethod("WriteLine", new[] { typeof(string) });
        protected readonly AssemblyBuilder InnerAssembly;
        protected readonly ModuleBuilder InnerModule;
        protected TypeBuilder Program;
        protected MethodBuilder Main;
        protected readonly ILGenerator Builder;
        protected string ExeName;

        public IlBuilder(string exeName, string className = "Program")
        {
            ExeName = exeName;
            var an = new AssemblyName("TinyScript");
            InnerAssembly = AppDomain.CurrentDomain.DefineDynamicAssembly(
                an, AssemblyBuilderAccess.RunAndSave);
            InnerModule = InnerAssembly.DefineDynamicModule("TinyScript", ExeName);
            Builder = InitMain(className);
        }

        private ILGenerator InitMain(string className)
        {
            Program = InnerModule.DefineType(className, TypeAttributes.BeforeFieldInit | TypeAttributes.AutoClass | TypeAttributes.AnsiClass | TypeAttributes.NotPublic);
            Program.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);
            Main = BuildMain();
            InnerAssembly.SetEntryPoint(Main);
            return Main.GetILGenerator();
        }

        protected virtual MethodBuilder BuildMain()
        {
            return Program.DefineMethod("Main", MethodAttributes.Static | MethodAttributes.Private | MethodAttributes.HideBySig, null, new[] { typeof(string[]) });
        }

        public virtual void Run()
        {
            Builder.Emit(OpCodes.Ret);
            var t = Program.CreateType();
            var main = t.GetMethod("Main");
            main.Invoke(null, new object[0]);
        }

        public virtual void Save()
        {
            Builder.Emit(OpCodes.Ret);
            Program.CreateType();
            InnerAssembly.Save(ExeName);
        }

        public LocalBuilder DeclareLocal(Type type)
        {
            return Builder.DeclareLocal(type);
        }

        public Label DefineLabel()
        {
            return Builder.DefineLabel();
        }

        public void MarkLabel(Label loc)
        {
            Builder.MarkLabel(loc);
        }

        public virtual void EmitPrint()
        {
            Builder.Emit(OpCodes.Call, Print);
        }

        public void LoadInt(int n)
        {
            switch(n)
            {
                case 0: Builder.Emit(OpCodes.Ldc_I4_0); return;
                case 1: Builder.Emit(OpCodes.Ldc_I4_1); return;
                case 2: Builder.Emit(OpCodes.Ldc_I4_2); return;
                case 3: Builder.Emit(OpCodes.Ldc_I4_3); return;
                case 4: Builder.Emit(OpCodes.Ldc_I4_4); return;
                case 5: Builder.Emit(OpCodes.Ldc_I4_5); return;
                case 6: Builder.Emit(OpCodes.Ldc_I4_6); return;
                case 7: Builder.Emit(OpCodes.Ldc_I4_7); return;
                case 8: Builder.Emit(OpCodes.Ldc_I4_8); return;
            }
            Builder.Emit(OpCodes.Ldc_I4, n);
        }

        public void LoadString(string text)
        {
            Builder.Emit(OpCodes.Ldstr, text);
        }

        public void LoadLocal(LocalBuilder local)
        {
            Builder.Emit(OpCodes.Ldloc, local);
        }

        public void LoadLocalAddress(LocalBuilder local)
        {
            Builder.Emit(OpCodes.Ldloca, local);
        }

        public void SetLocal(LocalBuilder local)
        {
            Builder.Emit(OpCodes.Stloc, local);
        }

        public void Nop()
        {
            Builder.Emit(OpCodes.Nop);
        }

        public void BrFalse(Label loc)
        {
            Builder.Emit(OpCodes.Brfalse, loc);
        }

        public void BrTrue(Label loc)
        {
            Builder.Emit(OpCodes.Brtrue, loc);
        }

        public void Br(Label loc)
        {
            Builder.Emit(OpCodes.Br, loc);
        }

        public void Call(MethodInfo info)
        {
            Builder.Emit(OpCodes.Call, info);
        }

        public void CallVirtual(MethodInfo info)
        {
            Builder.Emit(OpCodes.Callvirt, info);
        }

        public void Ceq()
        {
            Builder.Emit(OpCodes.Ceq);
        }

        public void Or()
        {
            Builder.Emit(OpCodes.Or);
        }

        public void And()
        {
            Builder.Emit(OpCodes.And);
        }

        public void Box(Type type)
        {
            Builder.Emit(OpCodes.Box, type);
        }
    }
}
