using Application.Contracts;
using Hangfire;
using Hangfire.PostgreSql;
using Infrastructure.ETLPipeline;
using Infrastructure.ETLPipeline.ExceptionHandler;
using Infrastructure.ETLPipeline.Extract.ApiAuth;
using Infrastructure.ETLPipeline.Extract.Benifit;
using Infrastructure.ETLPipeline.Extract.Citizenship;
using Infrastructure.ETLPipeline.Extract.EducationProgram;
using Infrastructure.ETLPipeline.Extract.EducationStandard;
using Infrastructure.ETLPipeline.Extract.Faculty;
using Infrastructure.ETLPipeline.Extract.Organization;
using Infrastructure.ETLPipeline.Extract.Student;
using Infrastructure.ETLPipeline.Extract.StudentAcademicState;
using Infrastructure.ETLPipeline.Extract.StudyForm;
using Infrastructure.ETLPipeline.Extract.Utils;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


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

            return services;
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
        }

        public static IHost MigrateInfrastructure( this IHost host )
        {
            host.Migrate<UniDashDbContext>();
            return host;
        }
    }
}
