namespace DpdtInject.Injector.Src.Excp
{
    /// <summary>
    /// Error types Dpdt aware of.
    /// </summary>
    public enum DpdtExceptionTypeEnum
    {
        /// <summary>
        /// Unknown type.
        /// </summary>
        NotSpecified,

        /// <summary>
        /// General error. Something went really awful.
        /// </summary>
        GeneralError,

        /// <summary>
        /// Internal error of the Dpdt logic.
        /// </summary>
        InternalError,

        /// <summary>
        /// A required constructor argument is missing.
        /// </summary>
        ConstructorArgumentMiss,

        /// <summary>
        /// Duplicate binding found during resolution.
        /// </summary>
        DuplicateBinding,

        /// <summary>
        /// Such binding is unavailable.
        /// </summary>
        NoBindingAvailable,

        /// <summary>
        /// Circular dependency in the binding tree is found.
        /// </summary>
        CircularDependency,

        /// <summary>
        /// Dpdt cluster type should be partial to make Dpdt source generator succeeds.
        /// </summary>
        TargetClassMustBePartial,

        /// <summary>
        /// For scoped binding scope object must exists.
        /// </summary>
        CustomScopeObjectDoesNotFound,

        /// <summary>
        /// Incorrent binding in the from clause.
        /// For example, binding from dynamic is not allowed.
        /// Also, binding target type should implement binding from type
        /// and allow to cast itself to binding from type.
        /// </summary>
        IncorrectBinding_IncorrectFrom,

        /// <summary>
        /// Incorrect binding in the to clause.
        /// </summary>
        IncorrectBinding_IncorrectTarget,

        /// <summary>
        /// Incorrect return type of the method of a factory type.
        /// </summary>
        IncorrectBinding_IncorrectReturnType,

        /// <summary>
        /// Incorrent binding statement. Possibly, incomplete.
        /// </summary>
        IncorrectBinding_IncorrectConfiguration,

        /// <summary>
        /// Factory type cannot be built.
        /// </summary>
        CannotBuildFactory,

        /// <summary>
        /// Bind method of the Dpdt cluster type must be parameter-less.
        /// </summary>
        BindMethodHasArguments,

        /// <summary>
        /// Incorrent binding statement. Possibly, incomplete.
        /// </summary>
        IncorrectBinding_IncorrectClause,

        /// <summary>
        /// A problem found in the settings scope of the binding.
        /// </summary>
        IncorrectBinding_IncorrectSetting,

        /// <summary>
        /// Local binding has been found, but it's forbidden by specific setting.
        /// </summary>
        LocalBindingFound,

        /// <summary>
        /// Dpdt cluster type cannot be used as a target for a binding statement.
        /// </summary>
        BindToClusterType
    }
}
