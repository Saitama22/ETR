using System.Text;
using ETR;
using ETR.DB;
using ETR.Middleware;
using ETR.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

internal class Program {
	private static void Main(string[] args) {
		var builder = WebApplication.CreateBuilder(args);

		builder.Services.AddBaseServices();
		builder.Services.AddDevServices(builder.Configuration);

		builder.Services.AddEndpointsApiExplorer();

		builder.Services.AddSwaggerGen(c => {
			c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

			// Настройка авторизации
			c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
				Name = "Authorization",
				Type = SecuritySchemeType.Http,
				Scheme = "Bearer",
				BearerFormat = "JWT",
				In = ParameterLocation.Header,
				Description = "Введите JWT токен"
			});

			c.AddSecurityRequirement(new OpenApiSecurityRequirement { {
					new OpenApiSecurityScheme {
						Reference = new OpenApiReference {
							Type = ReferenceType.SecurityScheme,
							Id = "Bearer"
						}
					},
					new string[] {}
				}
			});
		});

		// Добавление MVC (контроллеров и представлений) в проект
		builder.Services.AddControllersWithViews();

		builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => {
			// Настройка параметров пароля
			options.Password.RequireDigit = false; // Требовать цифры
			options.Password.RequireLowercase = false; // Требовать строчные буквы
			options.Password.RequireUppercase = false; // Требовать прописные буквы
			options.Password.RequireNonAlphanumeric = false; // Требовать специальные символы
			options.Password.RequiredLength = 1; // Минимальная длина пароля
			options.Password.RequiredUniqueChars = 0; // Количество уникальных символов

			// Другие настройки, такие как блокировка учетной записи, политики безопасности и т.д.
			options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Время блокировки учетной записи
			options.Lockout.MaxFailedAccessAttempts = 100; // Максимальное количество попыток ввода неверного пароля

			options.User.RequireUniqueEmail = true; // Требовать уникальные email адреса
		})
			.AddEntityFrameworkStores<ETRMainDbContext>()
			.AddDefaultTokenProviders();


		builder.Services.AddAuthentication(options => {
			options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
		}).AddJwtBearer(options => {
			options.TokenValidationParameters = new TokenValidationParameters {
				ValidateIssuer = true,
				ValidateAudience = true,
				ValidateLifetime = true,
				ValidateIssuerSigningKey = true,
				ValidIssuer = builder.Configuration["Jwt:Issuer"],
				ValidAudience = builder.Configuration["Jwt:Audience"],
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
			};
		});
		builder.Services.AddAuthorization();

		var app = builder.Build();

		// Настройка конвейера обработки запросов
		if (app.Environment.IsDevelopment()) {
			// В неразвивающей среде отображать страницы ошибок
			app.UseExceptionHandler("/Home/Error");
		}

		if (app.Configuration.GetValue<string>("EnableSwagger") == "true") {
			app.UseSwagger();
			app.UseSwaggerUI(c => {
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
				c.RoutePrefix = string.Empty; // Открыть Swagger UI на корневом пути
			});
		}
		app.UseMiddleware<ExceptionHandlerMiddleware>();

		// Использовать HSTS (HTTP Strict Transport Security) для повышения безопасности
		app.UseHsts();
		// Перенаправление HTTP на HTTPS
		app.UseHttpsRedirection();
		// Обработка статических файлов (например, CSS, JavaScript, изображения)
		app.UseStaticFiles();

		app.UseRouting();

		app.UseAuthentication();  // Включаем аутентификацию
		app.UseAuthorization();   // Включаем авторизацию

		app.MapFallbackToFile("index.html");
		// Настройка маршрутизации для контроллеров и представлений
		app.MapControllerRoute(
			name: "default",
			pattern: "{controller=Base}/{action=Index}");

		app.Run();
	}
}