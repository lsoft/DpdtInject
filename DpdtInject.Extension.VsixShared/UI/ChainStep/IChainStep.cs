using Task = System.Threading.Tasks.Task;

namespace DpdtInject.Extension.UI.ChainStep
{
    public interface IChainStep
    {
        Task CreateAsync();
    }
}
