using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;


namespace Container
{
    public class ObjectCreator
    {
        private readonly IDependencyFinder _finder;
        private readonly ModuleBuilder _builder;

        public ObjectCreator(IDependencyFinder finder)
        {
            _finder = finder;
            var aName = new AssemblyName("Container");
            var ab =
                AppDomain.CurrentDomain.DefineDynamicAssembly(
                    aName,
                    AssemblyBuilderAccess.RunAndSave);

            // For a single-module assembly, the module name is usually
            // the assembly name plus an extension.
            _builder =
                ab.DefineDynamicModule(aName.Name, aName.Name + ".dll");



        }
 

        public object CreateObject(Registration reg)
        {
            ConstructorInfo targetConstructor = null;
            var targetParameters = new ParameterInfo[] {};
            var typeBuilder = _builder.DefineType(
                reg.ImplementationType.Name,
                 TypeAttributes.Public);
            var constructors = typeBuilder.GetConstructors();
            foreach(var constructor in constructors)
            {
                targetConstructor = constructor;
                var args = new Dictionary<string, object>();
                var parms = constructor.GetParameters();
                targetParameters = parms;
                foreach(var parm in parms)
                {
                    var obj = _finder.FindDependency(parm.Attributes);
                    if(obj != null)
                        args.Add(parm.Name, obj);
                        
                }
            }
            return targetConstructor.Invoke(targetParameters);
        }
    }

    public interface IDependencyFinder
    {
        object FindDependency(ParameterAttributes attributes);
    }
}