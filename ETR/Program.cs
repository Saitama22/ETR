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

			// ��������� �����������
			c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
				Name = "Authorization",
				Type = SecuritySchemeType.Http,
				Scheme = "Bearer",
				BearerFormat = "JWT",
				In = ParameterLocation.Header,
				Description = "������� JWT �����"
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

		// ���������� MVC (������������ � �������������) � ������
		builder.Services.AddControllersWithViews();

		builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => {
			// ��������� ���������� ������
			options.Password.RequireDigit = false; // ��������� �����
			options.Password.RequireLowercase = false; // ��������� �������� �����
			options.Password.RequireUppercase = false; // ��������� ��������� �����
			options.Password.RequireNonAlphanumeric = false; // ��������� ����������� �������
			options.Password.RequiredLength = 1; // ����������� ����� ������
			options.Password.RequiredUniqueChars = 0; // ���������� ���������� ��������

			// ������ ���������, ����� ��� ���������� ������� ������, �������� ������������ � �.�.
			options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // ����� ���������� ������� ������
			options.Lockout.MaxFailedAccessAttempts = 100; // ������������ ���������� ������� ����� ��������� ������

			options.User.RequireUniqueEmail = true; // ��������� ���������� email ������
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

		// ��������� ��������� ��������� ��������
		if (app.Environment.IsDevelopment()) {
			// � ������������� ����� ���������� �������� ������
			app.UseExceptionHandler("/Home/Error");
		}

		if (app.Configuration.GetValue<string>("EnableSwagger") == "true") {
			app.UseSwagger();
			app.UseSwaggerUI(c => {
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
				c.RoutePrefix = string.Empty; // ������� Swagger UI �� �������� ����
			});
		}
		app.UseMiddleware<ExceptionHandlerMiddleware>();

		// ������������ HSTS (HTTP Strict Transport Security) ��� ��������� ������������
		app.UseHsts();
		// ��������������� HTTP �� HTTPS
		app.UseHttpsRedirection();
		// ��������� ����������� ������ (��������, CSS, JavaScript, �����������)
		app.UseStaticFiles();

		app.UseRouting();

		app.UseAuthentication();  // �������� ��������������
		app.UseAuthorization();   // �������� �����������

		app.MapFallbackToFile("index.html");
		// ��������� ������������� ��� ������������ � �������������
		app.MapControllerRoute(
			name: "default",
			pattern: "{controller=Base}/{action=Index}");

		app.Run();
	}
}