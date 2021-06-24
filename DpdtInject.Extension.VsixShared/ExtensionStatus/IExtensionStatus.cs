
namespace DpdtInject.Extension.ExtensionStatus
{
    public interface IExtensionStatus
    {
        bool IsSolutionExists
        {
            get;
        }

        bool IsEnabled
        {
            get;
        }
    }

}
