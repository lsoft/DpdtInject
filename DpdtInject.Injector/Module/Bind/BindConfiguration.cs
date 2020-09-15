using DpdtInject.Injector.Excp;
using DpdtInject.Injector.Helper;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace DpdtInject.Injector.Module.Bind
{
    //public class BindConfiguration : IBindConfiguration
    //{
    //    private readonly bool _conditionalBinding;
    //    private readonly IReadOnlyDictionary<string, ConstructorArgument> _constructorArguments;
    //    private readonly object? _constant;

    //    public BindNode BindNode
    //    {
    //        get;
    //    }

    //    //public DpdtIdempotentStatusEnum IdempotentStatus
    //    //{
    //    //    get;
    //    //}

    //    //public Func<IEmptyContext, bool>? Predicate
    //    //{
    //    //    get;
    //    //}

    //    public BindConfiguration(
    //        BindNode bindNode,
    //        bool conditionalBinding,
    //        //Func<IEmptyContext, bool>? predicate,
    //        IReadOnlyDictionary<string, ConstructorArgument> constructorArguments
    //        //DpdtIdempotentStatusEnum idempotentStatus
    //        )
    //    {
    //        if (bindNode is null)
    //        {
    //            throw new ArgumentNullException(nameof(bindNode));
    //        }

    //        if (constructorArguments is null)
    //        {
    //            throw new ArgumentNullException(nameof(constructorArguments));
    //        }

    //        BindNode = bindNode;
    //        _conditionalBinding = conditionalBinding;
    //        //Predicate = predicate;
    //        _constructorArguments = constructorArguments;
    //        //IdempotentStatus = idempotentStatus;
    //    }

    //    //public BindConfiguration(
    //    //    BindNode bindNode,
    //    //    Func<IEmptyContext, bool>? predicate,
    //    //    object? constant,
    //    //    DpdtIdempotentStatusEnum idempotentStatus
    //    //    )
    //    //{
    //    //    if (bindNode is null)
    //    //    {
    //    //        throw new ArgumentNullException(nameof(bindNode));
    //    //    }


    //    //    BindNode = bindNode;
    //    //    Predicate = predicate;
    //    //    _constant = constant;
    //    //    _constructorArguments = null!;
    //    //    IdempotentStatus = idempotentStatus;
    //    //}


    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public object? GetConstant()
    //    {
    //        if (BindNode.BindScope != BindScopeEnum.Constant)
    //        {
    //            throw new DpdtException(DpdtExceptionTypeEnum.IncorrectScope, $"Incorrect scope: {BindNode.BindScope}");
    //        }

    //        return _constant;
    //    }

    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public IReadOnlyDictionary<string, ConstructorArgument> GetConstructorArguments()
    //    {
    //        if(BindNode.BindScope.NotIn(BindScopeEnum.Singleton, BindScopeEnum.Transient))
    //        {
    //            throw new DpdtException(DpdtExceptionTypeEnum.IncorrectScope, $"Incorrect scope: {BindNode.BindScope}");
    //        }

    //        return _constructorArguments!;
    //    }

    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    internal bool TryGetConstructorArguments(
    //        string constructorArgumentName,
    //        [NotNullWhen(true)] out ConstructorArgument? constructorArgument
    //        )
    //    {
    //        if (BindNode.BindScope.NotIn(BindScopeEnum.Singleton, BindScopeEnum.Transient))
    //        {
    //            throw new DpdtException(DpdtExceptionTypeEnum.IncorrectScope, $"Incorrect scope: {BindNode.BindScope}");
    //        }

    //        return _constructorArguments.TryGetValue(constructorArgumentName, out constructorArgument);
    //    }
    //}
}
