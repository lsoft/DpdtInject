using DpdtInject.Injector;
using DpdtInject.Injector.CustomScope;
using DpdtInject.Injector.Excp;
using DpdtInject.Injector.Module.RContext;
using DpdtInject.Injector.Reinvented;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
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

            try
            {
                //GENERATOR: place for a dispose clauses
            }
            catch(Exception excp)
            {
                result.Add(excp);
            }

            if (!(_parentCluster is null))
            {
                try
                {
                    var parentExceptions = _parentCluster.TrySafelyDispose();
                    if (parentExceptions.Count > 0)
                    {
                        result.AddRange(parentExceptions);
                    }
                }
                catch (Exception excp)
                {
                    result.Add(excp);
                }
            }

            return result;
        }

        #endregion

        public CustomScopeObject CreateCustomScope(
            )
        {
            return new CustomScopeObject(
                1 //GENERATOR: place for conditional binding count
                );
        }

        #region IsRegisteredFrom<> and IsRegisteredFrom(Type)

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsRegisteredFrom<T>()
        {
            return this is IResolution<T>;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsRegisteredFrom(System.Type requestedType)
        {
            return _typeContainerGet.IsRegisteredFrom(requestedType);
        }

        #endregion

        #region Get<>

        public T Get<T>()
        {
            return ((IResolution<T>)this).Get(
                null
                );

            //if (this is IResolution<T> r)
            //{
            //    return r.Get(
            //        null
            //        );
            //}

            //throw new DpdtException(
            //    DpdtExceptionTypeEnum.NoBindingAvailable,
            //    $"No bindings available for {typeof(T).FullName}",
            //    typeof(T).FullName
            //    );
        }

        public T Get<T>(CustomScopeObject customScope)
        {
            return ((IResolution<T>)this).Get(
                    new ResolutionRequest<CarcassCluster, T>(
                        false,
                        customScope
                        )
                );

            //if (this is IResolution<T> r)
            //{
            //    return r.Get(
            //        new ResolutionRequest<CarcassCluster, T>(
            //            false,
            //            customScope
            //            )
            //        );
            //}

            //throw new DpdtException(
            //    DpdtExceptionTypeEnum.NoBindingAvailable,
            //    $"No bindings available for {typeof(T).FullName}",
            //    typeof(T).FullName
            //    );
        }

        #endregion

        #region GetAll<>

        public List<T> GetAll<T>()
        {
            return ((IResolution<T>)this).GetAll(
                null
                );

            //if (this is IResolution<T> r)
            //{
            //    return r.GetAll(
            //        null
            //        );
            //}

            //throw new DpdtException(
            //    DpdtExceptionTypeEnum.NoBindingAvailable,
            //    $"No bindings available for {typeof(T).FullName}",
            //    typeof(T).FullName
            //    );
        }

        public List<T> GetAll<T>(CustomScopeObject customScope)
        {
            return ((IResolution<T>)this).GetAll(
                    new ResolutionRequest<CarcassCluster, T>(
                        true,
                        customScope
                        )
                );

            //if (this is IResolution<T> r)
            //{
            //    return r.GetAll(
            //        new ResolutionRequest<CarcassCluster, T>(
            //            true,
            //            customScope
            //            )
            //        );
            //}

            //throw new DpdtException(
            //    DpdtExceptionTypeEnum.NoBindingAvailable,
            //    $"No bindings available for {typeof(T).FullName}",
            //    typeof(T).FullName
            //    );
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private T RaiseNoCustomScopeObject<T>()
        {
            throw new DpdtException(
                DpdtExceptionTypeEnum.CustomScopeObjectDoesNotFound,
                $"No custom scope object available for {typeof(T).FullName}",
                typeof(T).FullName
                );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private T RaiseNoBindingAvailable<T>()
        {
            throw new DpdtException(
                DpdtExceptionTypeEnum.NoBindingAvailable,
                $"No bindings available for {typeof(T).FullName}",
                typeof(T).FullName
                );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private T RaiseTooManyBindingException<T>()
        {
            throw new DpdtException(
                DpdtExceptionTypeEnum.DuplicateBinding,
                $"Too many bindings available for {typeof(T).FullName}",
                typeof(T).FullName
                );
        }

        #endregion

        #region intance interface providers
        //GENERATOR: place for an intance interface providers
        #endregion


        #region intance object producers
        //GENERATOR: place for an intance object producers
        #endregion

    }
}
