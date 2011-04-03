using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Container
{
    public class IoCWrapper
    {
        private readonly CoreContainer _container;

        public IoCWrapper(CoreContainer container)
        {
            this._container = container;
        }

        public void Register<Interface, Implementation>()
        {
            Register(typeof(Interface), typeof(Implementation));
        }

        public void Register<Interface, Implementation>(object anonymousDependencies)
        {
            Register(typeof(Interface), typeof(Implementation), anonymousDependencies);
        }

        private void Register(Type iface, Type impl, object dependencies)
        {
            Register(iface,impl, dependencies, ResolutionType.Transient);
        }

        private void Register(Type iface, Type impl, object dependencies, ResolutionType resType)
        {
            var reg = new Registration()
            {
                AnonymousDependencies = dependencies,
                ImplementationType = impl,
                InterfaceType = iface,
                ResolutionType = resType
            };
            _container.Register(reg);
        }

        private void Register(Type iface, Type impl)
        {
            Register(iface, impl, null);
        }
    }
}
