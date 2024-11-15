using ETR.DB;
using ETR.Models;
using ETR.Repos.Interface;
using Microsoft.EntityFrameworkCore;

namespace ETR.Repos {
	public abstract class BaseRepo<T> : IBaseRepo<T> where T : class {
		protected readonly ETRMainDbContext _context;
		protected readonly DbSet<T> _dbSet;

		public BaseRepo(ETRMainDbContext dbContext) {
			_context = dbContext;
			_dbSet = _context.Set<T>();
		}

		public async Task AddRecord(T record) {
			await _dbSet.AddAsync(record);
			await _context.SaveChangesAsync();
		}

		public async Task DeleteRecord(int id) {
			var record = await GetRecord(id);
			_dbSet.Remove(record);
			await _context.SaveChangesAsync();
		}

		public async Task DeleteRecord(T record) {
			_dbSet.Remove(record);
			await _context.SaveChangesAsync();
		}

		public async Task<T> GetRecord(int id) {
			var record = await _dbSet.FindAsync(id);
			return record;
		}

		public async Task UpdateRecord(T record) {
			_dbSet.Update(record);
			await _context.SaveChangesAsync();
		}

		public IQueryable<T> GetRecords() {
			return _dbSet.AsQueryable();
		}
	}
}
