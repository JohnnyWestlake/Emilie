using System.Threading.Tasks;

namespace Emilie.Core.Common
{
    public interface IIncrementalLoading
    {
        //
        // Summary:
        //     Initializes incremental loading from the view.
        //
        // Parameters:
        //   count:
        //     The number of items to load.
        //
        // Returns:
        //     The wrapped results of the load operation.

        Task<uint> LoadMoreAsync(uint count);

        //
        // Summary:
        //     Gets a sentinel value that supports incremental loading implementations.
        //
        // Returns:
        //     **true** if additional unloaded items remain in the view; otherwise, **false**.
        bool HasMoreItems { get; }
    }
}
