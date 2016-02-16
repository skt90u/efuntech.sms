using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Runtime.CompilerServices;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Objects;

namespace EFunTech.Sms.Schema
{
    // 您可以在 ApplicationUser 類別新增更多屬性，為使用者新增設定檔資料，請造訪 http://go.microsoft.com/fwlink/?LinkID=317594 以深入了解。

    // 0000,{'Member':'356610','Name':'Dino','Sex':'暫無指定','MobileCountry':'886','Mobile':'','ContactPhone':'','ContactPhoneExt':'','Email':'','AddressZip':'','AddressCity':'','AddressStreet':'','SmsBalance':'36.00','SmsBalanceExpireDate':'2114/02/06 23:59:59'}
    [TableDescription("使用者")]
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // 注意 authenticationType 必須符合 CookieAuthenticationOptions.AuthenticationType 中定義的項目
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // 在這裡新增自訂使用者宣告
            return userIdentity;
        }

        public int Level { get; set; }

        [MaxLength(128), ForeignKey("Parent")]
        public string ParentId { get; set; }

        public virtual ApplicationUser Parent { get; set; }

        [Required]
        [ColumnDescription("姓名")]
        public string FullName { get; set; }

        [ColumnDescription("性別")]
        public Gender Gender { get; set; }

        [ColumnDescription("聯絡電話")]
        public string ContactPhone { get; set; }

        [ColumnDescription("聯絡電話(分機)")]
        public string ContactPhoneExt { get; set; }

        [ColumnDescription("通訊地址(縣市)")]
        public string AddressCountry { get; set; }

        [ColumnDescription("通訊地址(鄉鎮市區)")]
        public string AddressArea { get; set; }

        [ColumnDescription("通訊地址(郵遞區號)")]
        public string AddressZip { get; set; }

        [ColumnDescription("通訊地址")]
        public string AddressStreet { get; set; }

        //[ForeignKey("Department")]
        //[ColumnDescription("部門編號")]
        //public int? DepartmentId { get; set; }

        [ColumnDescription("部門")]
        public virtual Department Department { get; set; }

        [ColumnDescription("員工編號")]
        public string EmployeeNo { get; set; }

        [ColumnDescription("點數預警設定")]
        public virtual CreditWarning CreditWarning { get; set; }

        [ColumnDescription("開啟回覆轉寄")]
        public virtual ReplyCc ReplyCc { get; set; }

        [ColumnDescription("開啟國際簡訊發送")]
        public bool ForeignSmsEnabled { get; set; }

        [DecimalPrecision(15, 2)] // 1000000000000.00
        [Required]
        [ColumnDescription("可用點數")]
        public decimal SmsBalance { get; set; }

        [Required]
        [ColumnDescription("可用點數到期日")]
        [DateTimeKind(DateTimeKind.Utc)]
        public DateTime SmsBalanceExpireDate { get; set; }

        [ColumnDescription("啟用/關閉")]
        public bool Enabled { get; set; }

        public virtual ICollection<Blacklist> Blacklists { get; set; }
        public virtual ICollection<CommonMessage> CommonMessages { get; set; }
        public virtual ICollection<UploadedFile> UploadedFiles { get; set; }
        public virtual ICollection<Signature> Signatures { get; set; }
        public virtual ICollection<SendMessageRule> SendMessageRules { get; set; }
        public virtual ICollection<Contact> Contacts { get; set; }
        public virtual ICollection<Group> Groups { get; set; }

        //public virtual ICollection<TradeDetail> TradeDetails { get; set; }

        public virtual AllotSetting AllotSetting { get; set; }

        [ColumnDescription("簡訊供應商類型，用以指定首要簡訊提供商")]
        public SmsProviderType SmsProviderType { get; set; }
        

    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("ApplicationDbContext", throwIfV1Schema: false)
        {
            // http://www.farreachinc.com/blog/far-reach/2013/09/26/entity-framework-query-optimizations
            this.Configuration.LazyLoadingEnabled = true; // 手動指定(不確定預設值是?) 
            //this.Configuration.LazyLoadingEnabled = false; // 20151202 Norman 測試效能，暫時關閉

            // 當遇上無法理解的 EntityFramework 錯誤
            // (1) 請在發生錯誤的地方前後下中斷點
            // (2) 打開輸入視窗(顯示輸出來源選擇[偵錯])
            // (3) 觀察輸入訊息，應該就可以找到問題發生原因
            this.Database.Log = (message) => { System.Diagnostics.Debug.WriteLine(message); }; // 20151122 Norman 測試效能，暫時關閉

            // http://stackoverflow.com/questions/4648540/entity-framework-datetime-and-utc
            ((IObjectContextAdapter)this).ObjectContext.ObjectMaterialized +=
                (sender, e) => DateTimeKindAttribute.Apply(e.Entity);

            //ObjectQuery<Blacklist> query = ((IObjectContextAdapter)this).ObjectContext.CreateQuery<Blacklist>("ccc");
            //query.MergeOption = MergeOption.NoTracking;

            // this.Blacklists.MergeOption
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<Department> Departments { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<CommonMessage> CommonMessages { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Blacklist> Blacklists { get; set; }
        public DbSet<GroupContact> GroupContacts { get; set; }
        public DbSet<Signature> Signatures { get; set; }
        public DbSet<UploadedFile> UploadedFiles { get; set; }
        public DbSet<SharedGroupContact> SharedGroupContacts { get; set; }
        public DbSet<SendMessageRule> SendMessageRules { get; set; }
        public DbSet<ReplyCc> ReplyCcs { get; set; }
        public DbSet<CreditWarning> CreditWarnings { get; set; }

        public DbSet<LogItem> LogItems { get; set; }

        // sms
        public DbSet<DeliveryReportQueue> DeliveryReportQueues { get; set; }

        // sms: infobip
        public DbSet<Infobip_SendMessageResultItem> Infobip_SendMessageResultItems { get; set; }
        public DbSet<Infobip_SendMessageResult> Infobip_SendMessageResults { get; set; }
        public DbSet<Infobip_ResourceReference> Infobip_ResourceReferences { get; set; }
        public DbSet<Infobip_DeliveryReport> Infobip_DeliveryReports { get; set; }
        
        // sms: every8d
        public DbSet<Every8d_DeliveryReport> Every8d_DeliveryReports { get; set; }
        public DbSet<Every8d_SendMessageResult> Every8d_SendMessageResults { get; set; }
        
        public DbSet<SendMessageHistory> SendMessageHistorys { get; set; }
        public DbSet<SendMessageStatistic> SendMessageStatistics { get; set; }
        
        public DbSet<TradeDetail> TradeDetails { get; set; }
        public DbSet<WebAuthorization> WebAuthorizations { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        
        public DbSet<UploadedMessageReceiver> UploadedMessageReceivers { get; set; }
        public DbSet<MessageReceiver> MessageReceivers { get; set; }

        public DbSet<UniqueJob> UniqueJobs { get; set; }

        public DbSet<SystemAnnouncement> SystemAnnouncements { get; set; }
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //EFunTech.Sms.Schema.IdentityUserLogin: : EntityType 'IdentityUserLogin' 未定義索引鍵。請定義此 EntityType 的索引鍵。
            //EFunTech.Sms.Schema.IdentityUserRole: : EntityType 'IdentityUserRole' 未定義索引鍵。請定義此 EntityType 的索引鍵。
            //IdentityUserLogins: EntityType: EntitySet 'IdentityUserLogins' 是以未定義索引鍵的類型 'IdentityUserLogin' 為基礎。
            //IdentityUserRoles: EntityType: EntitySet 'IdentityUserRoles' 是以未定義索引鍵的類型 'IdentityUserRole' 為基礎。
            base.OnModelCreating(modelBuilder); // <--- IdentityDbContext 會用到

            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }

        #region Override Examples
        /*
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            if (modelBuilder == null)
            {
                throw new ArgumentNullException("modelBuilder");
            }

            // Needed to ensure subclasses share the same table
            var user = modelBuilder.Entity<TUser>()
                .ToTable("AspNetUsers");
            user.HasMany(u => u.Roles).WithRequired().HasForeignKey(ur => ur.UserId);
            user.HasMany(u => u.Claims).WithRequired().HasForeignKey(uc => uc.UserId);
            user.HasMany(u => u.Logins).WithRequired().HasForeignKey(ul => ul.UserId);
            user.Property(u => u.UserName)
                .IsRequired()
                .HasMaxLength(256)
                .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("UserNameIndex") { IsUnique = true }));

            // CONSIDER: u.Email is Required if set on options?
            user.Property(u => u.Email).HasMaxLength(256);

            modelBuilder.Entity<TUserRole>()
                .HasKey(r => new { r.UserId, r.RoleId })
                .ToTable("AspNetUserRoles");

            modelBuilder.Entity<TUserLogin>()
                .HasKey(l => new { l.LoginProvider, l.ProviderKey, l.UserId })
                .ToTable("AspNetUserLogins");

            modelBuilder.Entity<TUserClaim>()
                .ToTable("AspNetUserClaims");

            var role = modelBuilder.Entity<TRole>()
                .ToTable("AspNetRoles");
            role.Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(256)
                .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("RoleNameIndex") { IsUnique = true }));
            role.HasMany(r => r.Users).WithRequired().HasForeignKey(ur => ur.RoleId);
        }

        /// <summary>
        ///     Validates that UserNames are unique and case insenstive
        /// </summary>
        /// <param name="entityEntry"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        protected override DbEntityValidationResult ValidateEntity(DbEntityEntry entityEntry,
            IDictionary<object, object> items)
        {
            if (entityEntry != null && entityEntry.State == EntityState.Added)
            {
                var errors = new List<DbValidationError>();
                var user = entityEntry.Entity as TUser;
                //check for uniqueness of user name and email
                if (user != null)
                {
                    if (Users.Any(u => String.Equals(u.UserName, user.UserName)))
                    {
                        errors.Add(new DbValidationError("User",
                            String.Format(CultureInfo.CurrentCulture, IdentityResources.DuplicateUserName, user.UserName)));
                    }
                    if (RequireUniqueEmail && Users.Any(u => String.Equals(u.Email, user.Email)))
                    {
                        errors.Add(new DbValidationError("User",
                            String.Format(CultureInfo.CurrentCulture, IdentityResources.DuplicateEmail, user.Email)));
                    }
                }
                else
                {
                    var role = entityEntry.Entity as TRole;
                    //check for uniqueness of role name
                    if (role != null && Roles.Any(r => String.Equals(r.Name, role.Name)))
                    {
                        errors.Add(new DbValidationError("Role",
                            String.Format(CultureInfo.CurrentCulture, IdentityResources.RoleAlreadyExists, role.Name)));
                    }
                }
                if (errors.Any())
                {
                    return new DbEntityValidationResult(entityEntry, errors);
                }
            }
            return base.ValidateEntity(entityEntry, items);
        }
        */
        #endregion
    }
}