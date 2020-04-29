using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using AutoMapper;
using Cityton.Api.Handlers;
using Cityton.Api.Contracts.Requests.Authentication;
using Cityton.Api.Contracts.Requests.User;
using Cityton.Api.Contracts.Requests.Challenge;
using Cityton.Api.Handlers.Authentication;
using Cityton.Api.Data;
using Microsoft.AspNetCore.Http;
using Cityton.Api.Contracts.DTOs;
using Cityton.Api.Contracts.Validators;
using FluentValidation;

namespace Cityton.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddScoped<
                IHandler<LoginRequest, ObjectResult>,
                LoginHandler>();
            services.AddScoped<
                IHandler<SignupRequest, ObjectResult>,
                SignupHandler>();
            services.AddScoped<
                IHandler<GetConnectedUserRequest, ObjectResult>,
                GetConnectedUserHandler>();
            services.AddScoped<
                IHandler<ChangePasswordRequest, ObjectResult>,
                ChangePasswordHandler>();
            services.AddScoped<
                IHandler<GetProfileRequest, ObjectResult>,
                GetProfileHandler>();
            services.AddScoped<
                IHandler<SearchChallengeRequest, ObjectResult>,
                SearchChallengeHandler>();
            services.AddScoped<
                IHandler<CreateChallengeRequest, ObjectResult>,
                CreateChallengeHandler>();
            services.AddScoped<
                IHandler<UpdateChallengeRequest, ObjectResult>,
                UpdateChallengeHandler>();
            services.AddScoped<
                IHandler<DeleteChallengeRequest, ObjectResult>,
                DeleteChallengeHandler>();
            services.AddScoped<
                IHandler<SearchUserRequest, ObjectResult>,
                SearchUserHandler>();
            services.AddScoped<
                IHandler<DeleteUserRequest, ObjectResult>,
                DeleteUserHandler>();

            services.AddDbContext<ApplicationDBContext>(options => options.UseMySql(Configuration.GetConnectionString("DefaultConnection")));

            services
                .AddControllers()
                .AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            services.AddAutoMapper(typeof(Startup));

            services.AddCors(o => o.AddPolicy("CorsPolicy", builder =>
            {
                builder
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
            }));

            // configure strongly typed settings objects
            var secret = Configuration.GetSection("Settings:Secret").Value;

            // configure jwt authentication
            var key = Encoding.ASCII.GetBytes(secret);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };

                x.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        // If the request is for our hub...
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) &&
                            (path.StartsWithSegments("/hubs/chat")))
                        {
                            // Read the token out of the query string
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            //services.AddMemoryCache();

            services.AddSignalR();

            services
                .AddControllers()
                .ConfigureApiBehaviorOptions(options => options.SuppressInferBindingSourcesForParameters = true)
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Startup>());

            // must first setup FV
            // services
            //     .AddMvc()
            //     .AddFluentValidation(fv => { });

            // // can then manually register validators
            // services.AddTransient<IValidator<SignupDTO>, SignupDTOValidator>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(options => options
                .SetIsOriginAllowed(x => _ = true)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
