namespace DpdtInject.Injector.Src.Bind.Settings.Constructor
{
    public enum ConstructorSettingsEnum
    {
        None,

        /// <summary>
        /// All constructor arguments must exists, be in the same order (NO other arguments can lie between these), no additional arguments allowed to exists.
        /// </summary>
        AllAndOrder,

        /// <summary>
        /// All constructor arguments must exists, be in the same order (other arguments can lie between these), additional arguments may exists.
        /// </summary>
        SubsetAndOrder,

        /// <summary>
        /// All constructor arguments must exists, order does not matters, additional arguments may exists.
        /// </summary>
        SubsetNoOrder
    }
}
