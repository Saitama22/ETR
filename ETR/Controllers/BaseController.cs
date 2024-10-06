using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ETR.Controllers {
	[Route("[controller]")]
	[Authorize]
	public class BaseController : Controller {
		[HttpGet("TestGet")]
		
		public async Task<IActionResult> Index() {
			return Ok();
		}

		[HttpPost("TestPost")]
		public async Task<IActionResult> Post() {
			return Ok();
		}

	}
}