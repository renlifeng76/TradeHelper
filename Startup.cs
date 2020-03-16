using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using TradeHelper.Dto;
using TradeHelper.IService;
using TradeHelper.Service;

namespace TradeHelper
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddControllers();

            //.Net core3.0������ͨ����װ Microsoft.AspNetCore.Mvc.NewtonsoftJson  =>����AddNewtonsoftJson() ֧�ֻ��� Newtonsoft.Json �ĸ�ʽ������͹���
            //֧�ֲ���JObject
            services.AddControllers().AddNewtonsoftJson().AddNewtonsoftJson(opt => {
                opt.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
            }); 

            //services.AddControllers().AddJsonOptions(options =>
            //{

            //    options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);

            //    // Use the default property (Pascal) casing.
            //    //options.JsonSerializerOptions.PropertyNamingPolicy = null;

            //    // Configure a custom converter.
            //    //options.JsonSerializerOptions.Converters.Add(new MyCustomJsonConverter());
            //});

            #region Jwt

            services.Configure<TokenManagement>(Configuration.GetSection("tokenManagement"));
            var token = Configuration.GetSection("tokenManagement").Get<TokenManagement>();

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(token.Secret)),
                    ValidIssuer = token.Issuer,
                    ValidAudience = token.Audience,
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            #endregion

            #region Swagger
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1.1.0",
                    Title = "TradeHelper WebAPI",
                    Description = "��̨���",
                    Contact = new OpenApiContact() { Name = "TradeHelper WebAPI", Email = "rlf@sina.com", Url = new Uri("https://www.cnblogs.com/AprilBlank/") }
                });


                #region ���س��򼯵�xml�����ĵ�

                // Ϊ Swagger JSON and UI����xml�ĵ�ע��·��
                var basePath = Path.GetDirectoryName(AppContext.BaseDirectory);//��ȡӦ�ó�������Ŀ¼�����ԣ����ܹ���Ŀ¼Ӱ�죬������ô˷�����ȡ·����
                var xmlPath = Path.Combine(basePath, "TradeHelper.Api.xml");
                //options.IncludeXmlComments(xmlPath);
                options.IncludeXmlComments(xmlPath,true);// Ĭ�ϵĵڶ���������false�������controller��ע�ͣ��ǵ��޸�

                #endregion

                #region jwt

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Description = "���¿�����������ͷ����Ҫ���Jwt��ȨToken��Bearer Token",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });

                #endregion



            });
            #endregion

            #region ��������
            //services.AddCors(options =>
            //{
            //    options.AddPolicy("any", builder =>
            //    {
            //        builder.AllowAnyOrigin() //�����κ���Դ����������
            //        .AllowAnyMethod()
            //        .AllowAnyHeader()
            //        .AllowCredentials();//ָ������cookie
            //    });
            //});

            #endregion

            #region ע��service jwt��¼��֤����

            services.AddScoped<IAuthenticateService, TokenAuthenticationService>();
            services.AddScoped<IUserService, UserService>();

            #endregion

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            #region Swagger
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "ApiHelp V1");
                //������ø�Ŀ¼Ϊswagger,����ֵ�ÿ�
                //options.RoutePrefix = string.Empty;
            });
            #endregion

            #region Jwt

            app.UseAuthentication();

            #endregion

            #region ��̬�ļ�����

            DefaultFilesOptions defaultFilesOptions = new DefaultFilesOptions();
            defaultFilesOptions.DefaultFileNames.Clear();
            defaultFilesOptions.DefaultFileNames.Add("index.html");
            app.UseDefaultFiles(defaultFilesOptions);
            app.UseStaticFiles();

            #endregion

            #region ��������Cors

            app.UseCors(builder =>
            {
                builder.AllowAnyHeader();
                builder.AllowAnyMethod();
                //builder.WithOrigins("http://localhost:8080");
                builder.AllowAnyOrigin();
            });

            #endregion

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
