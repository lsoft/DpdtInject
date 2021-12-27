namespace DpdtInject.Injector.Src.Excp
{
    public enum DpdtExceptionTypeEnum
    {
        NotSpecified,
        GeneralError,
        InternalError,
        ConstructorArgumentMiss,
        DuplicateBinding,
        NoBindingAvailable,
        CircularDependency,
        TargetClassMustBePartial,
        CustomScopeObjectDoesNotFound,

        IncorrectBinding_IncorrectFrom,
        IncorrectBinding_IncorrectTarget,
        IncorrectBinding_IncorrectReturnType,
        IncorrectBinding_IncorrectConfiguration,

        CannotBuildFactory,
        BindMethodHasArguments,

        IncorrectBinding_IncorrectClause,
        IncorrectBinding_IncorrectSetting,

        LocalBindingFound,
        BindToClusterType
    }
}
