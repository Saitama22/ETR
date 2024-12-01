using ETR.DB;
using ETR.Handlers;
using ETR.Handlers.Interface;
using ETR.Helpers;
using ETR.Models;
using ETR.Repos;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ETR {
	public static class DI {
		public static IServiceCollection AddDevServices(this IServiceCollection services, ConfigurationManager configuration) {
			services.AddDbContexts(configuration.GetConnectionString("DefaultConnection"));
			services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
			return services;
		}

		public static IServiceCollection AddBaseServices(this IServiceCollection services) {
			services.AddHelpers();
			services.AddRepos();
			services.AddHandlers();
			return services;
		}

		private static IServiceCollection AddHelpers(this IServiceCollection services) {
			services.AddScoped<IEmailSender, ETREmailSender>();
			return services;
		}
		private static IServiceCollection AddRepos(this IServiceCollection services) {
			services.AddScoped<BaseItemRepo>();
			return services;
		}
		private static IServiceCollection AddHandlers(this IServiceCollection services) {
			services.AddScoped<IItemHandler, ItemHandler>();
			services.AddScoped<IJwtAuthHandler, JwtAuthHandler>();
			services.AddScoped<IMailHandler, MailHandler>();
			
			return services;
		}


		public static IServiceCollection AddDbContexts(this IServiceCollection services, string connection) {
			// Добавление контекста базы данных в контейнер зависимостей
			// Здесь настраивается подключение к базе данных с использованием строки подключения из конфигурации
			services.AddDbContext<ETRMainDbContext>(options =>
				options.UseNpgsql(connection));
			return services;
		}
	}
}
