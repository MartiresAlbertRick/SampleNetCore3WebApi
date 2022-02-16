using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AD.CAAPS.Services
{
    public interface IReadOnlyService<T>
    {
        IQueryable<T> GetMany();

        Task<T> GetOne(int id);
    }

    public interface IWriteService<T>
    {
        Task<IList<T>> Import(IList<T> dataset, string[] uniqueIdentifierMapping, bool truncateTable = false, bool generateSequence = false, string generateSequenceStoredProcedureName = null);
    }

    public interface IService<T> : IReadOnlyService<T>, IWriteService<T>
    {
    }
}