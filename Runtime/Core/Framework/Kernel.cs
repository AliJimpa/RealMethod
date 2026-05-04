using System;
using UnityEngine;

namespace RealMethod
{
    /// <summary>
    /// The foundational runtime core of the RealMethod framework.
    /// Provides the minimal infrastructure required for all higher-level systems,
    /// including lightweight dependency injection and the base module/manager pipeline.
    /// 
    /// This class does not implement gameplay logic and is not intended to be used
    /// directly by typical developers. Instead, it serves as the internal root that
    /// powers <see cref="Scope"/>, <see cref="Game"/>Scope, and <see cref="World"/>Scope.
    /// 
    /// Responsibilities:
    /// • Acts as the DI root for registering and resolving services.
    /// • Provides the shared base for all game modules and managers.
    /// • Defines internal lifecycle hooks used by all scopes.
    /// 
    /// Regular users normally interact only with GameScope or WorldScope,
    /// while RealKernel remains an internal framework component.
    /// </summary>
    public abstract class Kernel : MonoBehaviour
    {
        private static Lazy<ServiceLocator> _serviceLocator = new Lazy<ServiceLocator>(() => new ServiceLocator());
        private static Lazy<DependencyInjection> _dependencyInjection = new Lazy<DependencyInjection>(() => new DependencyInjection());


        // GameModules
        protected static ServiceLocator Services => _serviceLocator.Value;
        protected static DependencyInjection DInjection => _dependencyInjection.Value;




#if UNITY_EDITOR
        public virtual IInspectorInfo[] GetAllInfo()
        {
            return new IInspectorInfo[2] { Services, DInjection };
        }
#endif
    }
}