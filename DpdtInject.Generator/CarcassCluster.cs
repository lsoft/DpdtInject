#pragma warning disable CS0105
using DpdtInject.Injector;
using DpdtInject.Injector.CustomScope;
using DpdtInject.Injector.Excp;
using DpdtInject.Injector.Reinvented;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
using DpdtInject.Injector.RContext;
// ReSharper disable SuspiciousTypeConversion.Global

//GENERATOR: place for an additional usings

namespace DpdtInject.Generator
{
#nullable disable
    public partial class CarcassCluster : ICluster
        //GENERATOR: place for a resolution interfaces
    {
        private readonly ICluster _parentCluster;
        private long _disposed = 0L;

        private readonly FixedSizeFactoryContainer _typeContainerGet;
        private readonly FixedSizeFactoryContainer _typeContainerGetAll;

        public CarcassCluster(
            ICluster parentCluster
            )
        {
            //parentCluster allowed to be null

            _parentCluster = parentCluster;

            _typeContainerGet = new FixedSizeFactoryContainer(
                //GENERATOR: add nongeneric GET binding
                );
            _typeContainerGetAll = new FixedSizeFactoryContainer(
                //GENERATOR: add nongeneric GET ALL binding
                );

            //GENERATOR: place for an unknown type resolutions
        }

        #region dispose

        public void Dispose()
        {
            var exceptions = TrySafelyDispose();

            if(exceptions.Count > 0)
            {
                throw new AggregateException(exceptions);
            }
        }

        public List<Exception> TrySafelyDispose(
            )
        {
            var result = new List<Exception>();

            if (Interlocked.Exchange(ref _disposed, 1L) != 0L)
            {
                return result;
            }

            //GENERATOR: place for a dispose clauses

            //why do we need to dispose a parent cluster??
            //if (!(_parentCluster is null))
            //{
            //    try
            //    {
            //        var parentExceptions = _parentCluster.TrySafelyDispose();
            //        if (parentExceptions.Count > 0)
            //        {
            //            result.AddRange(parentExceptions);
            //        }
            //    }
            //    catch (Exception excp)
            //    {
            //        result.Add(excp);
            //    }
            //}

            return result;
        }

        private void TryToSafeDispose(
            Action disposeAction,
            ref List<Exception> exceptions
            )
        {
            try
            {
                disposeAction();
            }
            catch (Exception excp)
            {
                exceptions.Add(excp);
            }
        }

        #endregion

        public CustomScopeObject CreateCustomScope(
            )
        {
            return new(
                1 //GENERATOR: place for conditional binding count
                );
        }

#region IsRegisteredFrom<> and IsRegisteredFrom(Type)

        //GENERATOR: aggressive inline and optimize
        public bool IsRegisteredFrom<T>()
        {
            return this is IResolution<T>;
        }

        //GENERATOR: aggressive inline and optimize
        public bool IsRegisteredFrom(System.Type requestedType)
        {
            return _typeContainerGet.IsRegisteredFrom(requestedType);
        }

        //GENERATOR: aggressive inline and optimize
        public bool IsRegisteredFromRecursive<T>()
        {
            if(this is IResolution<T>)
            {
                return true;
            }

            if (_parentCluster is null)
            {
                return false;
            }

            return _parentCluster.IsRegisteredFromRecursive<T>();
        }

        #endregion

        #region Get<>

        //GENERATOR: aggressive inline and optimize
        public T Get<T>()
        {
            if (this is IResolution<T> r)
            {
                return r.Get(
                    null
                    );
            }
            else
            {
                return RaiseNoBindingAvailable<T>();
            }
        }

        //GENERATOR: aggressive inline and optimize
        public T Get<T>(CustomScopeObject customScope)
        {
            if (this is IResolution<T> r)
            {
                return r.Get(
                    new ResolutionRequest<CarcassCluster, T>(
                        false,
                        customScope
                        )
                    );
            }
            else
            {
                return RaiseNoBindingAvailable<T>();
            }
        }

        #endregion

        #region GetAll<>

        //GENERATOR: aggressive inline and optimize
        public List<T> GetAll<T>()
        {
            if (this is IResolution<T> r)
            {
                return r.GetAll(
                    null
                    );
            }
            else
            {
                throw new DpdtException(
                    DpdtExceptionTypeEnum.NoBindingAvailable,
                    $"No bindings available for {typeof(T).FullName}",
                    typeof(T).FullName
                    );
            }
        }

        //GENERATOR: aggressive inline and optimize
        public List<T> GetAll<T>(CustomScopeObject customScope)
        {
            if (this is IResolution<T> r)
            {
                return r.GetAll(
                    new ResolutionRequest<CarcassCluster, T>(
                        true,
                        customScope
                        )
                    );
            }
            else
            {
                throw new DpdtException(
                    DpdtExceptionTypeEnum.NoBindingAvailable,
                    $"No bindings available for {typeof(T).FullName}",
                    typeof(T).FullName
                    );
            }
        }

        #endregion

        #region Get(Type)

        public object Get(System.Type requestedType)
        {
            var result = _typeContainerGet.GetGetObject(
                requestedType,
                null
                );
            return result;
        }
        public object Get(System.Type requestedType, CustomScopeObject scope)
        {
            var result = _typeContainerGet.GetGetObject(
                requestedType,
                new ResolutionRequest<CarcassCluster>(
                    false,
                    requestedType,
                    scope
                    )
                );
            return result;
        }

#endregion

#region GetAll(Type)


        public IEnumerable<object> GetAll(Type requestedType)
        {
            var result = _typeContainerGetAll.GetGetObject(
                requestedType,
                null
                );
            return (IEnumerable<object>)result;
        }

        public IEnumerable<object> GetAll(Type requestedType, CustomScopeObject scope)
        {
            var result = _typeContainerGetAll.GetGetObject(
                requestedType,
                new ResolutionRequest<CarcassCluster>(
                    true,
                    requestedType,
                    scope
                    )
                );
            return (IEnumerable<object>)result;
        }

#endregion

#region GetFast

        public TR GetFast<TR>(TR unused)
        {
            return RaiseNoBindingAvailable<TR>();
        }

#endregion


#region cross cluster methods

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetToChild<T>(IResolutionRequest resolutionRequest)
        {
            if (!(this is IResolution<T> r))
            {
                if (_parentCluster is null)
                {
                    RaiseNoBindingAvailable<T>();
                }

                return _parentCluster.GetToChild<T>(
                    resolutionRequest
                    );
            }

            return r.Get(
                resolutionRequest
                );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private T GetFromParent<T>(
            IResolutionTarget resolutionTarget,
            string constructorArgument
            )
        {
            if (_parentCluster is null)
            {
                RaiseNoBindingAvailable<T>();
            }

            return _parentCluster.GetToChild<T>(
                new ResolutionRequest<CarcassCluster, T>(resolutionTarget, constructorArgument)
                );
        }

#endregion

#region private cluster methods

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private T GetFromLocalUnsafely<T>(
            )
        {
            return ((IResolution<T>)this).Get(
                null
                );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private T GetFromLocalUnsafely<T>(
            IResolutionTarget resolutionTarget,
            string constructorArgument
            )
        {
            return ((IResolution<T>)this).Get(
                new ResolutionRequest<CarcassCluster, T>(resolutionTarget, constructorArgument)
                );
        }


        #endregion

        #region raise exceptions

        [DoesNotReturn]
        private T RaiseNoCustomScopeObject<T>()
        {
            throw new DpdtException(
                DpdtExceptionTypeEnum.CustomScopeObjectDoesNotFound,
                $"No custom scope object available for {typeof(T).FullName}",
                typeof(T).FullName
                );
        }

        [DoesNotReturn]
        private T RaiseNoBindingAvailable<T>()
        {
            throw new DpdtException(
                DpdtExceptionTypeEnum.NoBindingAvailable,
                $"No bindings available for {typeof(T).FullName}",
                typeof(T).FullName
                );
        }

        [DoesNotReturn]
        private T RaiseTooManyBindingException<T>()
        {
            throw new DpdtException(
                DpdtExceptionTypeEnum.DuplicateBinding,
                $"Too many bindings available for {typeof(T).FullName}",
                typeof(T).FullName
                );
        }

#endregion

#region instance interface providers
        //GENERATOR: place for an instance interface providers
#endregion


#region instance object producers
        //GENERATOR: place for an instance object producers
#endregion

    }
}
