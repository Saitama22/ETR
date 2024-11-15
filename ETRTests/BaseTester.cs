using System.ComponentModel.Design;
using System.Reflection.Metadata;
using Microsoft.Extensions.DependencyInjection;
using ETR;
using ETR.Models;
using Microsoft.Extensions.Configuration;

namespace ETRTests {
	public class BaseTester {
		protected IServiceProvider _serviceProvider;
		public BaseTester() {
			IServiceCollection services = new ServiceCollection();
			services.AddBaseServices();
			services.AddDbContexts("Host=localhost:5432;Database=ETRTestDb;Username=postgres;Password=qwerty");

			_serviceProvider = services.BuildServiceProvider();
		}
	}
}