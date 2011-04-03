using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Container
{
    public interface IResolveObjects

    {
        bool CanProcess(ResolutionType rType);
        object Resolve(Registration reg);
        IList<object> ResolveAll(Type interfaceType);
         
    }

    public class InternalObjectResolver : IResolveObjects
    {
        private readonly Dictionary<Registration, object> _persistence = new Dictionary<Registration, object>();
        private readonly IList<IResolveObjects> _resolvers = new List<IResolveObjects>();
        private readonly ObjectCreator _creator;
        private readonly IRegistry _registry;
        private readonly IDependencyFinder _dependencyFinder;

        public InternalObjectResolver(IRegistry registry)
        {
            _registry = registry;
            _dependencyFinder = new DependencyFinder(_registry);
            _creator = new ObjectCreator(_dependencyFinder);
            _resolvers.Add(new TransientObjectResolver(_creator));
            _resolvers.Add(new ThreadObjectResolver(_creator));
            _resolvers.Add(new WebObjectResolver(_creator));
            _resolvers.Add(new SingletonObjectResolver(_creator, _persistence));
        }
        public bool CanProcess(ResolutionType rType)
        {
            throw new System.NotImplementedException();
        }

        public object Resolve(Registration reg)
        {
            return _registry;
        }

        public IList<object> ResolveAll(Type interfaceType)
        {
            return (IList<object>) _resolvers;
        }
    }
    public class TransientObjectResolver : IResolveObjects
    {
        private readonly ObjectCreator _creator;

        public TransientObjectResolver(ObjectCreator creator)
        {
            this._creator = creator;
        }

        public bool CanProcess(ResolutionType rType)
        {
            return rType == ResolutionType.Transient;
        }

        public object Resolve(Registration reg)
        {
            return _creator.CreateObject(reg);
        }

        public IList<object> ResolveAll(Type interfaceType)
        {
            throw new System.NotImplementedException();
        }
    }

    public class SingletonObjectResolver : IResolveObjects
    {
        private readonly Dictionary<Registration, object> _dictionary;
        private readonly ObjectCreator _creator;

        public SingletonObjectResolver (ObjectCreator creator,
            Dictionary<Registration, object> dictionary)
        {
            _dictionary = dictionary;
            _creator = creator;
        }

        public bool CanProcess(ResolutionType rType)
        {
            return rType == ResolutionType.Singleton;
        }

        public object Resolve(Registration reg)
        {
            return _dictionary.ContainsKey(reg) ? _dictionary[reg] : _creator.CreateObject(reg);
        }

        public IList<object> ResolveAll(Type interfaceType)
        {
            throw new System.NotImplementedException();
        }
    }

    public class ThreadObjectResolver: IResolveObjects
    {
        [ThreadStatic] private Dictionary<Registration, object> 
            _dictionary = new Dictionary<Registration, object>();
        private readonly ObjectCreator _creator;

        public ThreadObjectResolver(ObjectCreator creator)
        {
            _creator = creator;
        }

        public bool CanProcess(ResolutionType rType)
        {
            return rType == ResolutionType.Thread;
        }

        public object Resolve(Registration reg)
        {
            return _dictionary.ContainsKey(reg) ? _dictionary[reg] : _creator.CreateObject(reg);
        }

        public IList<object> ResolveAll(Type interfaceType)
        {
            throw new System.NotImplementedException();
        }
    }

    public class WebObjectResolver : IResolveObjects
    {
        private ObjectCreator _creator;

        public WebObjectResolver(ObjectCreator creator)
        {
            this._creator = creator;
        }

        public bool CanProcess(ResolutionType rType)
        {
            return rType == ResolutionType.Web;
        }

        public object Resolve(Registration reg)
        {
            return System.Web.HttpContext.Current.Items.Contains(reg) 
                ? System.Web.HttpContext.Current.Items[reg] 
                : _creator.CreateObject(reg); 
        }

        public IList<object> ResolveAll(Type interfaceType)
        {
            throw new System.NotImplementedException();
        }
    }
    public enum ResolutionType
    {
        Singleton,
        Transient,
        Web,
        Thread
    } ;
    public class Registration
    {
        public string Key
        {
            get; set;
        }
        public ResolutionType ResolutionType
        {
            get;
            set;
        }
        public Type InterfaceType
        {
            get; set;
        }

        public Type ImplementationType
        {
            get; set;
        }
 

        public object AnonymousDependencies
        {
            get; set;
        }
    }
    public class CoreContainer
    {
        private readonly IRegistry _registry;
        public CoreContainer()
        {
            _registry = new Registry();
            InternalResolver = new InternalObjectResolver(_registry); 
        }
        public static IResolveObjects InternalResolver{ get; private set;}


        public void Register(Registration reg)
        {
            
            _registry.Add(reg.Key, reg);
        }

        public object Resolve(string key)
        {
            if (!_registry.Contains(key))
                    throw new InvalidOperationException(
                        string.Format(@"Component cannot be 
                            resolved from key {0} because nothing was registered under that name.", key));
            var reg = _registry.Get(key);
            var resolverList = (IEnumerable<IResolveObjects>)InternalResolver
                .ResolveAll(typeof (IResolveObjects));
            var resolver = resolverList.ToList().Find(r => r.CanProcess(reg.ResolutionType));
            return resolver.Resolve(reg);
        }
 
    }

    internal class DependencyFinder : IDependencyFinder
    {
        public DependencyFinder(IRegistry registry)
        {
            throw new NotImplementedException();
        }

        public object FindDependency(ParameterAttributes attributes)
        {
            throw new System.NotImplementedException();
        }
    }
}
