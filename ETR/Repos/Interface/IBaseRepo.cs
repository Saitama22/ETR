

namespace ETR.Repos.Interface
{
	public interface IBaseRepo<T> where T : class {
		Task AddRecord(T record);
		Task DeleteRecord(int id);
		Task DeleteRecord(T record);
		Task<T> GetRecord(int id);
		IQueryable<T> GetRecords();
		Task UpdateRecord(T record);
	}
}
