using ETR.DB;
using ETR.Models.DBModels;
using Microsoft.EntityFrameworkCore;

namespace ETR.Repos
{
    public class BaseItemRepo : BaseRepo<BaseItem> {
		public BaseItemRepo(ETRMainDbContext db) : base(db) {

		}
	}
}
