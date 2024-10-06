using ETR.Models.DBModels;
using Microsoft.AspNetCore.Mvc;

namespace ETR.Handlers.Interface
{
    public interface IItemHandler {
		Task AddItemAsync(BaseItem baseItem);
		IQueryable<BaseItem> GetAllRecords(int page);
		IQueryable<BaseItem> GetUserRecords(int page, string userName);
	}
}
