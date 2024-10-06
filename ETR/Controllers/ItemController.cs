using System.Security.Claims;
using ETR.Handlers.Interface;
using ETR.Models;
using ETR.Models.DBModels;
using ETR.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ETR.Controllers {
	[Authorize]
	[Route("api/[controller]")]
	public class ItemController : Controller {
		private readonly IItemHandler _itemHandler;

		public ItemController(IItemHandler itemHandler) {
			_itemHandler = itemHandler;
		}

		[HttpPost]
		public async Task<IActionResult> AddItem(ItemDto item) {
			var userName = GetUserName();
			if (userName == null) {
				return BadRequest();
			}

			await _itemHandler.AddItemAsync(new BaseItem {
				Name = item.Name,
				Description = item.Description,
				Creator = userName
			});
			return Ok();
		}

		[HttpGet("my/page/{page}")]
		public IActionResult GetUserRecordsAsync(int page) {
			var userName = GetUserName();
			if (userName == null) {
				return BadRequest();
			}

			var results = _itemHandler.GetUserRecords(page, userName);
			return Ok(results);
		}

		private string GetUserName() {
			return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
		}
	}
}
