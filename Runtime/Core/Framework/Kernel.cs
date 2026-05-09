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
        private static GameSubsystem.ShareData _rpository;
        private static ServiceLocator _serviceLocator;
        private static DependencyInjection _dependencyInjection;


        // GameModules
        private static GameSubsystem.ShareData Repo
        {
            get
            {
                if (_rpository == null)
                {
                    _rpository = new GameSubsystem.ShareData(10);
                }
                return _rpository;
            }
        }
        protected static ServiceLocator Services
        {
            get
            {
                if (_serviceLocator == null)
                {
                    _serviceLocator = new ServiceLocator();
                    ((IBootstrap)_serviceLocator).Setup(Repo);
                }
                return _serviceLocator;
            }
        }
        protected static DependencyInjection DInjection
        {
            get
            {
                if (_dependencyInjection == null)
                {
                    _dependencyInjection = new DependencyInjection();
                    ((IBootstrap)_dependencyInjection).Setup(Repo);
                }
                return _dependencyInjection;
            }
        }


        protected static void ClearKernel()
        {
            if (_serviceLocator != null)
            {
                ((IDisposable)_serviceLocator).Dispose();
                _serviceLocator = null;
            }
            if (_dependencyInjection != null)
            {
                ((IDisposable)_dependencyInjection).Dispose();
                _dependencyInjection = null;
            }
            if (_rpository != null)
            {
                ((IDisposable)_rpository).Dispose();
                _rpository = null;
            }

        }



#if UNITY_EDITOR
        public virtual IInspectorInfo[] GetAllInfo()
        {
            return new IInspectorInfo[3] { _rpository, Services, DInjection };
        }
#endif
    }
}