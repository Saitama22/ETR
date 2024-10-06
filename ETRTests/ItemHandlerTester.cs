using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using ETR.Handlers.Interface;
using Microsoft.Extensions.DependencyInjection;
using ETR.Models.DBModels;

namespace ETRTests {
	public class ItemHandlerTester : BaseTester {
		protected IItemHandler _itemHandler;
		[SetUp]
		public void Setup() {
			_itemHandler = _serviceProvider.GetService<IItemHandler>();
		}

		[Test]
		public async Task TestAdd() {
			await _itemHandler.AddItemAsync(new BaseItem {
				Name = "Test",	
				Creator = "q",
				Description = "desc"
			});
		}
	}
}