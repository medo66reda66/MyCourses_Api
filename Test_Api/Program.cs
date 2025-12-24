using Ecommers.Api.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

using Test_Api.Datebase;
using Test_Api.Models;
using Test_Api.Rpository;
using Test_Api.Rpository.Irepository;
using Test_Api.Servers;
using Test_Api.Utility;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Test_Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
           builder.Services.AddDbContext<ApplicationDBcontext>(option =>
            {
                //option.UseSqlServer(builder.Configuration.GetSection("ConnectionStrings")["DefaultConnection"]);
                //option.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"]);
                option.UseSqlServer(builder.Configuration.GetConnectionString("default"));
            });
            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(option =>
            {
                option.Password.RequireDigit = false;
                option.Password.RequiredLength = 6;
                option.Password.RequireNonAlphanumeric = false;
                option.Password.RequireUppercase = false;
                option.Password.RequireLowercase = false;
                option.User.RequireUniqueEmail = true;
                option.SignIn.RequireConfirmedEmail = true;
            })
                .AddEntityFrameworkStores<ApplicationDBcontext>()
                .AddDefaultTokenProviders();

            builder.Services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "http://localhost:5000",
                    ValidAudience = "http://localhost:5000",
                    IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("Nf7$Pq19!sD@84LmZ#xT2wQvR%k9Hp36Nf7$Pq19!sD@84LmZ#xT2wQvR%k9Hp36"))
                };
            });
            builder.Services.AddScoped<IEmailSender, EmailSender>();
            builder.Services.AddScoped<Datebase.ApplicationDBcontext>();
            builder.Services.AddScoped<IRepository<Category> , Repository<Category>>();
            builder.Services.AddScoped<IRepository<Instructor> , Repository<Instructor>>();
            builder.Services.AddScoped<IRepository<Course> , Repository<Course>>();
            builder.Services.AddScoped<IRepository<Carts> , Repository<Carts>>();
            builder.Services.AddScoped<IRepository<ApplicationuserOTP> , Repository<ApplicationuserOTP>>();
            builder.Services.AddScoped<IRepository<CourseVideos> , Repository<CourseVideos>>();
            builder.Services.AddScoped<IRepository<Promotion> , Repository<Promotion>>();
            builder.Services.AddScoped<IRepository<Order> , Repository<Order>>();
            builder.Services.AddScoped<IRepository<Mycourse> , Repository<Mycourse>>();
            builder.Services.AddScoped<IrepositoruSupImg , RepositorySupImg>();
            builder.Services.AddScoped<Irepositoruorderitem , Repositoryorderitem>();
            builder.Services.AddScoped<IToken, Token>();
            builder.Services.AddScoped<IDBinitializer, DBinitializer>();

            Stripe.StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

            var app = builder.Build();
            var scope = app.Services.CreateScope();
            var server = scope.ServiceProvider.GetRequiredService<IDBinitializer>();
            server.Initialize();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
