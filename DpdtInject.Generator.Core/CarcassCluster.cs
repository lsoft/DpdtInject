#pragma warning disable CS0105
// ReSharper disable SuspiciousTypeConversion.Global

//GENERATOR: place for an additional usings

using System.Threading.Tasks;

namespace DpdtInject.Generator.Core
{
#nullable disable
    public partial class CarcassCluster : global::DpdtInject.Injector.Src.ICluster
        //GENERATOR: place for a resolution interfaces
    {
        private readonly global::DpdtInject.Injector.Src.ICluster _parentCluster;
        private global::System.Int64 _disposed = 0L;
        private global::System.Int64 _asyncDisposed = 0L;

        private readonly global::DpdtInject.Injector.Src.Reinvented.FixedSizeFactoryContainer _typeContainerGet;
        private readonly global::DpdtInject.Injector.Src.Reinvented.FixedSizeFactoryContainer _typeContainerGetAll;

        public CarcassCluster(
            global::DpdtInject.Injector.Src.ICluster parentCluster
            )
        {
            //parentCluster allowed to be null

            _parentCluster = parentCluster;

            _typeContainerGet = new global::DpdtInject.Injector.Src.Reinvented.FixedSizeFactoryContainer(
                //GENERATOR: add nongeneric GET binding
                );
            _typeContainerGetAll = new global::DpdtInject.Injector.Src.Reinvented.FixedSizeFactoryContainer(
                //GENERATOR: add nongeneric GET ALL binding
                );

            //GENERATOR: place for an unknown type resolutions
        }

        #region dispose

        public void Dispose()
        {
            DoDispose();

            global::System.GC.SuppressFinalize(this);
        }

        public async global::System.Threading.Tasks.ValueTask DisposeAsync()
        {
            //do regular dispose at first, if the user only async-dispose this cluster
            //if this cluster is sync-disposed already, DoDispose method is no op
            DoDispose();

            await DoDisposeAsync().ConfigureAwait(false);

            global::System.GC.SuppressFinalize(this);
        }

        private void DoDispose()
        {
            var exceptions = TrySafelyDispose();
            if (exceptions.Count > 0)
            {
                throw new global::System.AggregateException(exceptions);
            }
        }

        public async global::System.Threading.Tasks.ValueTask DoDisposeAsync()
        {
            var exceptions = await TrySafelyDisposeAsync().ConfigureAwait(false);
            if (exceptions.Count > 0)
            {
                throw new global::System.AggregateException(exceptions);
            }

        }

        private global::System.Collections.Generic.List<global::System.Exception> TrySafelyDispose(
            )
        {
            var result = new global::System.Collections.Generic.List<global::System.Exception>();

            if (global::System.Threading.Interlocked.Exchange(ref _disposed, 1L) != 0L)
            {
                return result;
            }

            //GENERATOR: place for a dispose clauses

            return result;
        }

        private async global::System.Threading.Tasks.ValueTask<global::System.Collections.Generic.List<global::System.Exception>> TrySafelyDisposeAsync(
            )
        {
            var result = new global::System.Collections.Generic.List<global::System.Exception>();

            if (global::System.Threading.Interlocked.Exchange(ref _asyncDisposed, 1L) != 0L)
            {
                return result;
            }

            //GENERATOR: place for an async dispose clauses

            return result;
        }

        private void TryToSafeDispose(
            global::System.Action disposeAction,
            ref global::System.Collections.Generic.List<global::System.Exception> exceptions
            )
        {
            try
            {
                disposeAction();
            }
            catch (global::System.Exception excp)
            {
                exceptions.Add(excp);
            }
        }

        private async global::System.Threading.Tasks.ValueTask<global::System.Exception> TryToSafeDisposeAsync(
            global::System.Func<global::System.Threading.Tasks.ValueTask> disposeAction
            )
        {
            try
            {
                await disposeAction().ConfigureAwait(false);

                return null;
            }
            catch (global::System.Exception excp)
            {
                return excp;
            }
        }

        #endregion

        /// <inheritdoc />
        public global::DpdtInject.Injector.Src.CustomScope.CustomScopeObject CreateCustomScope(
            )
        {
            return new(
                1 //GENERATOR: place for conditional binding count
                );
        }

        #region IsRegisteredFrom<> and IsRegisteredFrom(Type)

        /// <inheritdoc />
        //GENERATOR: aggressive inline and optimize
        public global::System.Boolean IsRegisteredFrom<T>()
        {
            return this is global::DpdtInject.Injector.Src.IResolution<T>;
        }

        /// <inheritdoc />
        //GENERATOR: aggressive inline and optimize
        public global::System.Boolean IsRegisteredFrom(System.Type requestedType)
        {
            return _typeContainerGet.IsRegisteredFrom(requestedType);
        }

        /// <inheritdoc />
        //GENERATOR: aggressive inline and optimize
        public global::System.Boolean IsRegisteredFromRecursive<T>()
        {
            if(this is global::DpdtInject.Injector.Src.IResolution<T>)
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

        /// <inheritdoc />
        //GENERATOR: aggressive inline and optimize
        public T Get<T>()
        {
            if (this is global::DpdtInject.Injector.Src.IResolution<T> r)
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

        /// <inheritdoc />
        //GENERATOR: aggressive inline and optimize
        public T Get<T>(global::DpdtInject.Injector.Src.CustomScope.CustomScopeObject customScope)
        {
            if (this is global::DpdtInject.Injector.Src.IResolution<T> r)
            {
                return r.Get(
                    new global::DpdtInject.Injector.Src.RContext.ResolutionRequest<CarcassCluster, T>(
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

        /// <inheritdoc />
        //GENERATOR: aggressive inline and optimize
        public global::System.Collections.Generic.List<T> GetAll<T>()
        {
            if (this is global::DpdtInject.Injector.Src.IResolution<T> r)
            {
                return r.GetAll(
                    null
                    );
            }
            else
            {
                throw new global::DpdtInject.Injector.Src.Excp.DpdtException(
                    global::DpdtInject.Injector.Src.Excp.DpdtExceptionTypeEnum.NoBindingAvailable,
                    $"No bindings available for {typeof(T).FullName}",
                    typeof(T).FullName
                    );
            }
        }

        //GENERATOR: aggressive inline and optimize
        public global::System.Collections.Generic.List<T> GetAll<T>(global::DpdtInject.Injector.Src.CustomScope.CustomScopeObject customScope)
        {
            if (this is global::DpdtInject.Injector.Src.IResolution<T> r)
            {
                return r.GetAll(
                    new global::DpdtInject.Injector.Src.RContext.ResolutionRequest<CarcassCluster, T>(
                        true,
                        customScope
                        )
                    );
            }
            else
            {
                throw new global::DpdtInject.Injector.Src.Excp.DpdtException(
                    global::DpdtInject.Injector.Src.Excp.DpdtExceptionTypeEnum.NoBindingAvailable,
                    $"No bindings available for {typeof(T).FullName}",
                    typeof(T).FullName
                    );
            }
        }

        #endregion

        #region Get(Type)

        /// <inheritdoc />
        public global::System.Object Get(global::System.Type requestedType)
        {
            var result = _typeContainerGet.GetGetObject(
                requestedType,
                null
                );
            return result;
        }
        /// <inheritdoc />
        public global::System.Object Get(global::System.Type requestedType, global::DpdtInject.Injector.Src.CustomScope.CustomScopeObject scope)
        {
            var result = _typeContainerGet.GetGetObject(
                requestedType,
                new global::DpdtInject.Injector.Src.RContext.ResolutionRequest<CarcassCluster>(
                    false,
                    requestedType,
                    scope
                    )
                );
            return result;
        }

        #endregion

        #region GetAll(Type)

        /// <inheritdoc />
        public global::System.Collections.Generic.IEnumerable<global::System.Object> GetAll(global::System.Type requestedType)
        {
            var result = _typeContainerGetAll.GetGetObject(
                requestedType,
                null
                );
            return (global::System.Collections.Generic.IEnumerable<global::System.Object>)result;
        }

        /// <inheritdoc />
        public global::System.Collections.Generic.IEnumerable<global::System.Object> GetAll(global::System.Type requestedType, global::DpdtInject.Injector.Src.CustomScope.CustomScopeObject scope)
        {
            var result = _typeContainerGetAll.GetGetObject(
                requestedType,
                new global::DpdtInject.Injector.Src.RContext.ResolutionRequest<CarcassCluster>(
                    true,
                    requestedType,
                    scope
                    )
                );
            return (global::System.Collections.Generic.IEnumerable<global::System.Object>)result;
        }

        #endregion

        #region GetFast

        /// <summary>
        /// Get a registered service faster. Cannot be invoked via <see cref="DpdtInject.Injector.Src.ICluster"/>
        /// <see cref="DpdtInject.Injector.Src.Excp.DpdtException"/> can be raised if any errors occurs.
        /// </summary>
        public TR GetFast<TR>(TR unused)
        {
            return RaiseNoBindingAvailable<TR>();
        }

#endregion

        #region cross cluster methods

        [global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public T GetToChild<T>(global::DpdtInject.Injector.Src.RContext.IResolutionRequest resolutionRequest)
        {
            if (!(this is global::DpdtInject.Injector.Src.IResolution<T> r))
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

        [global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private T GetFromParent<T>(
            global::DpdtInject.Injector.Src.RContext.IResolutionTarget resolutionTarget,
            global::System.String constructorArgument
            )
        {
            if (_parentCluster is null)
            {
                RaiseNoBindingAvailable<T>();
            }

            return _parentCluster.GetToChild<T>(
                new global::DpdtInject.Injector.Src.RContext.ResolutionRequest<CarcassCluster, T>(
                    resolutionTarget,
                    constructorArgument
                    )
                );
        }

        #endregion

        #region private cluster methods

        [global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private T GetFromLocalUnsafely<T>(
            )
        {
            return ((global::DpdtInject.Injector.Src.IResolution<T>)this).Get(
                null
                );
        }

        [global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private T GetFromLocalUnsafely<T>(
            global::DpdtInject.Injector.Src.RContext.IResolutionTarget resolutionTarget,
            global::System.String constructorArgument
            )
        {
            return ((global::DpdtInject.Injector.Src.IResolution<T>)this).Get(
                new global::DpdtInject.Injector.Src.RContext.ResolutionRequest<CarcassCluster, T>(
                    resolutionTarget,
                    constructorArgument
                    )
                );
        }


        #endregion

        #region raise exceptions

        [global::System.Diagnostics.CodeAnalysis.DoesNotReturn]
        private static T RaiseNoCustomScopeObject<T>()
        {
            throw new global::DpdtInject.Injector.Src.Excp.DpdtException(
global::DpdtInject.Injector.Src.Excp.DpdtExceptionTypeEnum.CustomScopeObjectDoesNotFound,
                $"No custom scope object available for {typeof(T).FullName}",
                typeof(T).FullName
                );
        }

        [global::System.Diagnostics.CodeAnalysis.DoesNotReturn]
        private static T RaiseNoBindingAvailable<T>()
        {
            throw new global::DpdtInject.Injector.Src.Excp.DpdtException(
global::DpdtInject.Injector.Src.Excp.DpdtExceptionTypeEnum.NoBindingAvailable,
                $"No bindings available for {typeof(T).FullName}",
                typeof(T).FullName
                );
        }

        [global::System.Diagnostics.CodeAnalysis.DoesNotReturn]
        private static T RaiseTooManyBindingException<T>()
        {
            throw new global::DpdtInject.Injector.Src.Excp.DpdtException(
global::DpdtInject.Injector.Src.Excp.DpdtExceptionTypeEnum.DuplicateBinding,
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
