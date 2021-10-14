using System;
using CoMeta.Data;
using CoMeta.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MySecurity.Authentication;
using MySecurity.Authorization;
using MySecurity.Data;
using UserRepository = MySecurity.Data.UserRepository;

namespace CoMeta
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Environment = env;
        }

        private IConfiguration Configuration { get; }
        private IWebHostEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Byte[] secretBytes = new byte[40];
            // Create a byte array with random values. This byte array is used
            // to generate a key for signing JWT tokens.
            using (var rngCsp = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                rngCsp.GetBytes(secretBytes);
            }

            //Add JWT authentication
            //The settings below match the settings when we create our TOKEN:
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateAudience = false,
                    //ValidAudience = "CoMetaApiClient",
                    ValidateIssuer = false,
                    //ValidIssuer = "CoMetaApi",
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(secretBytes),
                    ValidateLifetime = true, //validate the expiration and not before values in the token
                    ClockSkew = TimeSpan.FromMinutes(5) //5 minute tolerance for the expiration date
                };
            });

            //Startup the "Entity" DBContext (PetShop, or Movies or whatever the Core is:
            services.AddDbContext<CoMetaContext>(
                opt => opt.UseInMemoryDatabase("CoMetaList")
            );
            services.AddTransient<IDbInitializer, InMemoryInitializer>();

            //I create a separate context for my security plugin: 
            services.AddDbContext<SecurityContext>(opt => opt.UseInMemoryDatabase("Security"));
            services.AddTransient<ISecurityContextInitializer, SecurityMemoryInitializer>();

            services.AddScoped<UserRepository>();
            services.AddScoped<IRepository<Message>, MessageRepository>();

            //I activate my special policies:
            services.AddAuthorization(opt =>
            {
                opt.AddPolicy("OwnerPolicy", policy =>
                {
                    policy.Requirements.Add(new ResourceOwnerRequirement());
                });
            });
            services.AddSingleton<IAuthorizationHandler, UserResourceOwnerAuthorizationService>(); //Adding the handler for the "OwnerPolicy"
            
            //I add the Authentication helper as a SINGLETON that uses the SECRET symmetric key:
            //The key is used for digitally signing the JWT tokens - keeping them secure from tampering
            //The SINGLETON is to ensure that we are using the same authenticator, with the same SECRET:
            services.AddSingleton<IAuthenticationHelper>(new AuthenticationHelper(secretBytes));

            services.AddScoped<IUserAuthenticator, UserAuthenticator>();
            
            //Below will setup CORS for the application. 
            //BEWARE that the current setup allows any origin, method and header. AKA an "Open door" policy... 
            services.AddCors(options =>
                options.AddDefaultPolicy(builder =>
                    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

            services.AddControllers();
            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo { Title = "CoMeta", Version = "v1" }); });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Initialize the database.
            using (var scope = app.ApplicationServices.CreateScope())
            {
                // Initialize the database
                var services = scope.ServiceProvider;
                var dbContext = services.GetService<CoMetaContext>();
                var dbInitializer = services.GetService<IDbInitializer>();
                dbInitializer.Initialize(dbContext);
                var secContext = services.GetService<SecurityContext>();
                var dbSecInit = services.GetService<ISecurityContextInitializer>();
                dbSecInit.Initialize(secContext);
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CoMeta v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            //UseAuthentication must be called AFTER UseRouting, and BEFORE UseEndpoints:
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}