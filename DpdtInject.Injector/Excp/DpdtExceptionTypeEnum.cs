namespace DpdtInject.Injector.Excp
{
    public enum DpdtExceptionTypeEnum
    {
        NotSpecified,
        GeneralError,
        InternalError,
        ConstructorArgumentMiss,
        InvalidTestConfiguration,
        DuplicateBinding,
        NoBindingAvailable,
        CircularDependency,
        ConstantCantHaveConstructorArguments, //TODO: implement this check
        UnknownScope,
        TargetClassMustBePartial,
        CustomScopeObjectDoesNotFound,

        IncorrectBinding_IncorrectFrom,
        IncorrectBinding_IncorrectTarget,
        IncorrectBinding_IncorrectReturnType,
        CannotBuildFactory,
    }
}