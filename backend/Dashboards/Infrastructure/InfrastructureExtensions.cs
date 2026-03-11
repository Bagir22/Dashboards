using Application.Contracts;
using Hangfire;
using Hangfire.PostgreSql;
using Infrastructure.Analysis.Services;
using Infrastructure.ETLPipeline;
using Infrastructure.ETLPipeline.ExceptionHandler;
using Infrastructure.ETLPipeline.Extract.Achivment;
using Infrastructure.ETLPipeline.Extract.AchivmentCategory;
using Infrastructure.ETLPipeline.Extract.ApiAuth;
using Infrastructure.ETLPipeline.Extract.Benifit;
using Infrastructure.ETLPipeline.Extract.Branch;
using Infrastructure.ETLPipeline.Extract.Citizenship;
using Infrastructure.ETLPipeline.Extract.Discipline;
using Infrastructure.ETLPipeline.Extract.EducationProgram;
using Infrastructure.ETLPipeline.Extract.EducationStandard;
using Infrastructure.ETLPipeline.Extract.Faculty;
using Infrastructure.ETLPipeline.Extract.Group;
using Infrastructure.ETLPipeline.Extract.Mark;
using Infrastructure.ETLPipeline.Extract.Order;
using Infrastructure.ETLPipeline.Extract.OrderCategory;
using Infrastructure.ETLPipeline.Extract.Organization;
using Infrastructure.ETLPipeline.Extract.Student;
using Infrastructure.ETLPipeline.Extract.StudentAcademicState;
using Infrastructure.ETLPipeline.Extract.StudyForm;
using Infrastructure.ETLPipeline.Extract.TrainingLevel;
using Infrastructure.ETLPipeline.Extract.Utils;
using Infrastructure.Metabase;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.Http.Headers;


namespace Infrastructure
{
    public static class InfrastructureExtensions
    {
        private static readonly string _connectionString = Environment.GetEnvironmentVariable( "DB_CONNECTION_STRING" ) ?? String.Empty;
        public static IServiceCollection AddInfrastructure( this IServiceCollection services, IConfiguration configuration )
        {
            InitDB( services );

            services.Configure<AuthBodySettings>( configuration.GetSection( "AuthBodySettings" ) );
            services.AddTransient<FileReader>();

            services.AddSingleton<HangfireExceptionFilter>();

            InitRequests( services );

            services.AddScoped<IDataSynchronizer, DataSynchronizer>();

            InitHangfire( services );

            services.AddHostedService<Worker>();

            // Регистрация Metabase
            services.AddHttpClient<IMetabaseService, MetabaseService>()
                .ConfigureHttpClient(client =>
                {
                    client.BaseAddress = new Uri("http://metabase:3000/");
                });

            AddLLMService(services);

            return services;
        }

        private static void AddLLMService(IServiceCollection services)
        {
            services.AddHttpClient<IAIService, AIService>();

            services.ConfigureHttpClientDefaults(conf => conf.ConfigureHttpClient(conf => {
                conf.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "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");
                conf.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }));
        }

        private static void InitDB( IServiceCollection services )
        {
            services.AddDbContext<UniDashDbContext>( options =>
            {
                options.UseNpgsql( _connectionString, x => x.MigrationsAssembly( "Infrastructure" ) );
            } );
            services.AddScoped<IUniDashDbContext>(sp => sp.GetRequiredService<UniDashDbContext>());
        }

        private static void InitHangfire( IServiceCollection services )
        {
            services.AddHangfire( ( provider, config ) =>
            config.UsePostgreSqlStorage( opt =>
            {
                opt.UseNpgsqlConnection( _connectionString );
            } ).UseFilter(
                provider.GetRequiredService<HangfireExceptionFilter>()
                ) );

            services.AddHangfireServer();
        }

        private static void InitRequests( IServiceCollection services )
        {
            services.AddMemoryCache();
            services.AddHttpClient<IApiAuthRequest, ApiAuthRequest>();
            services.AddHttpClient<IStudentAcademicStateRequest, StudentAcademicStateRequest>();
            services.AddHttpClient<IStudyFormRequest, StudyFormRequest>();
            services.AddHttpClient<IFacultyRequest, FacultyRequest>();
            services.AddHttpClient<IStudentRequest, StudentRequest>();
            services.AddHttpClient<ICitizenshipRequest, CitizenshipRequest>();
            services.AddHttpClient<IEducationProgramRequest, EducationProgramRequest>();
            services.AddHttpClient<IEducationStandardRequest, EducationStandardRequest>();
            services.AddHttpClient<IBenefitRequest, BenefitRequest>();
            services.AddHttpClient<IOrganizationRequest, OrganizationRequest>();
            services.AddHttpClient<IAchivmentCategoryRequest, AchivmentCategoryRequest>();
            services.AddHttpClient<IAchivmentRequest, AchivmentRequest>();
            services.AddHttpClient<IOrderCategoryRequest, OrderCategoryRequest>();
            services.AddHttpClient<IOrderRequest, OrderRequest>();
            services.AddHttpClient<IBranchRequest, BranchRequest>();
            services.AddHttpClient<ITrainingLevel, TrainingLevelRequest>();
            services.AddHttpClient<IDiscipline, DisciplineRequest>();
            services.AddHttpClient<IMark, MarkRequest>();
            services.AddHttpClient<IGroupRequest, GroupRequest>();
        }

        public static IHost MigrateInfrastructure( this IHost host )
        {
            host.Migrate<UniDashDbContext>();
            return host;
        }
    }
}
