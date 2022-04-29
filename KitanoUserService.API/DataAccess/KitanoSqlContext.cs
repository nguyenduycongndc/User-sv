using KitanoUserService.API.Models.MigrationsModels;
using KitanoUserService.API.Models.MigrationsModels.Category;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace KitanoUserService.API.DataAccess
{
    public class KitanoSqlContext : DbContext
    {
        public KitanoSqlContext()
        {
        }
        public KitanoSqlContext(DbContextOptions<KitanoSqlContext> options) : base(options)
        {
        }

        // TABLE
        //===================================================================
        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<UsersGroup> UsersGroup { get; set; }
        public virtual DbSet<UsersGroupMapping> UsersGroupMapping { get; set; }
        public virtual DbSet<Department> Department { get; set; }
        public virtual DbSet<UnitType> UnitType { get; set; }
        public virtual DbSet<CatAuditRequest> CatAuditRequest { get; set; }
        public virtual DbSet<CatDetectType> CatDetectType { get; set; }
        public virtual DbSet<CatRiskLevel> CatRiskLevel { get; set; }
        public virtual DbSet<UsersWorkHistory> UsersWorkHistory { get; set; }
        public virtual DbSet<Roles> Roles { get; set; }
        public virtual DbSet<UsersRoles> UsersRoles { get; set; }
        public virtual DbSet<UsersGroupRoles> UsersGroupRoles { get; set; }
        public virtual DbSet<SystemParameter> SystemParameter { get; set; }

        public virtual DbSet<Menu> Menu { get; set; }
        public virtual DbSet<Permission> Permission { get; set; }
        public virtual DbSet<RolePermissionMenu> RolePermissionMenu { get; set; }
        public virtual DbSet<Document> Document { get; set; }
        public virtual DbSet<DocumentFile> DocumentFile { get; set; }


        public virtual DbSet<ApprovalConfig> ApprovalConfig { get; set; }
        public virtual DbSet<ApprovalFunction> ApprovalFunction { get; set; }
        public virtual DbSet<ApprovalFunctionFile> ApprovalFunctionFile { get; set; }
        public virtual DbSet<ControlDocument> ControlDocument { get; set; }

        public virtual DbSet<AuditPlan> AuditPlan { get; set; } // bảng lấy từ BE khác
        public virtual DbSet<AuditDetect> AuditDetect { get; set; } // bảng lấy từ BE khác
        public virtual DbSet<AuditRequestMonitor> AuditRequestMonitor { get; set; } // bảng lấy từ BE khác
        public virtual DbSet<AuditStrategyRisk> AuditStrategyRisk { get; set; } // bảng lấy từ BE khác
        public virtual DbSet<AuditCycle> AuditCycle { get; set; } // bảng lấy từ BE khác

        public virtual DbSet<FacilityRequestMonitorMapping> FacilityRequestMonitorMapping { get; set; } // bảng lấy từ BE khác

        public virtual DbSet<RatingScale> RatingScale { get; set; }// Bảng lấy từ RiskAssessment
        public virtual DbSet<SystemLog> SystemLog { get; set; }
        public virtual DbSet<PermissionMenu> PermissionMenu { get; set; }
        //===================================================================
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionBuilder)
        {
            optionBuilder.UseLoggerFactory(GetLoggerFactory());       // bật logger
        }
        public override int SaveChanges()
        {
            ChangeTracker.DetectChanges();
            return base.SaveChanges();
        }
        private ILoggerFactory GetLoggerFactory()
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(builder =>
                    builder.AddConsole()
                           .AddFilter(DbLoggerCategory.Database.Command.Name,
                                    LogLevel.Information));
            return serviceCollection.BuildServiceProvider()
                    .GetService<ILoggerFactory>();
        }
    }
}
