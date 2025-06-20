
using BusinessLogicLayer;
using DataAccessLayer;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OData.ModelBuilder;
using NewsAPI.Services;
using System;
using System.Text;

namespace NewsAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<FunewsManagementContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddScoped<AccountRepository>();
            builder.Services.AddScoped<ArticleRepository>();
            builder.Services.AddScoped<CategoryRepository>();
            builder.Services.AddScoped<AccountService>();
            builder.Services.AddScoped<ArticleService>();
            builder.Services.AddScoped<CategoryService>();

            var modelBuilder = new ODataConventionModelBuilder();
            modelBuilder.EntitySet<SystemAccount>("Accounts").EntityType.HasKey(a => a.AccountId);
            modelBuilder.EntitySet<Category>("Categories").EntityType.HasKey(c => c.CategoryId);
            modelBuilder.EntitySet<NewsArticle>("NewsArticles").EntityType.HasKey(a => a.NewsArticleId);

            builder.Services.AddControllers().AddOData(opt =>
            {
                opt.Select().Filter().OrderBy().Expand().Count().SetMaxTop(100)
                    .AddRouteComponents("odata", modelBuilder.GetEdmModel());
            });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            var jwtKey = builder.Configuration["Jwt:Key"];
            var keyBytes = Encoding.UTF8.GetBytes(jwtKey!);

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(keyBytes)
                    };
                });

            builder.Services.AddScoped<JwtService>();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
