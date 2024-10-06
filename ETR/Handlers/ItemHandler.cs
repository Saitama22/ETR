using ETR.Handlers.Interface;
using ETR.Models.DBModels;
using ETR.Repos;
using Microsoft.AspNetCore.Mvc;

namespace ETR.Handlers
{
    public class ItemHandler : IItemHandler {
		private readonly BaseItemRepo _repo;
		public ItemHandler(BaseItemRepo repo) {
			_repo = repo;
		}

		public async Task AddItemAsync(BaseItem baseItem) {
		 	await _repo.AddRecord(baseItem);
		}

		public IQueryable<BaseItem> GetUserRecords(int page, string userName) {
			var result = _repo.GetRecords().Where(r => r.Creator == userName).Skip((page - 1) * 10).Take(10);
			return result;
		}

		public IQueryable<BaseItem> GetAllRecords(int page) {
			var result = _repo.GetRecords().Skip((page - 1) * 10).Take(10);
			return result;
		}
	}
}
