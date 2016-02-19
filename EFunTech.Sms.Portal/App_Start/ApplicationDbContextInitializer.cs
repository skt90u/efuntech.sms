using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Owin;
using EFunTech.Sms.Portal.Models;
using EFunTech.Sms.Schema;
using System.Threading.Tasks;
using System.Configuration;
using Microsoft.AspNet.Identity.EntityFramework;
using EFunTech.Sms.Simulation;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using JUtilSharp.Database;

namespace EFunTech.Sms.Portal
{
    /// <summary>
    /// 在 Application_Start 設定
    /// System.Data.Entity.Database.SetInitializer(new ApplicationDbContextInitializer());
    /// 
    /// 參考資料：
    ///     http://www.codeguru.com/csharp/article.php/c19999/Understanding-Database-Initializers-in-Entity-Framework-Code-First.htm
    ///     http://www.entityframeworktutorial.net/code-first/database-initialization-strategy-in-code-first.aspx
    /// </summary>
    public class ApplicationDbContextInitializer : IDatabaseInitializer<ApplicationDbContext>
    //public class ApplicationDbContextInitializer : DropCreateDatabaseAlways<ApplicationDbContext>
    {
        public void InitializeDatabase(ApplicationDbContext context)
        {
            // Seed(context); // 測試看看關閉這個，效能是否會提升網站啟動速度
        }

        protected virtual void Seed(ApplicationDbContext context)
        //protected override void Seed(ApplicationDbContext context) // DropCreateDatabaseAlways
        {
            //context.Database.Delete(); // 似乎永遠都會失敗
            if (!context.Database.Exists())
                context.Database.Create();

            #region 建立角色

            CreateRules(context).Wait();

            #endregion

            #region 建立預設使用者
            CreateUser(context,
                role: Role.Administrator,
                username: ConfigurationManager.AppSettings["DefaultAdminUsername"],
                password: ConfigurationManager.AppSettings["DefaultAdminPassword"],
                smsBalance: Convert.ToDecimal(ConfigurationManager.AppSettings["DefaultAdminSmsBalance"]),
                phoneNumber: "+886921859698",
                parent: null)
            .Wait();
            #endregion

            #region 建立 Menu
            CreateMenus(context);
            #endregion

            #region 建立測試資料(DEBUG MODE ONLY)
#if DEBUG
            CreateTestingData(context);
#else
            CreateTestingData(context); // 未來請拿掉(正式環境，不需要建立假資料)
#endif
            #endregion

            //#region 建立預設公告內容(未來會拿掉)
            //CreateSystemAnnouncements(context);
            //#endregion
        }

        private void CreateSystemAnnouncements(ApplicationDbContext context)
        {
            var list = new List<SystemAnnouncement>();

            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var createdUser = userManager.FindByNameAsync("Admin").GetAwaiter().GetResult();
            var createdTime = new DateTime(2015, 01, 01, 0, 0, 0, DateTimeKind.Utc);
            
            list.Add(new SystemAnnouncement{
                IsVisible = true,
                PublishDate = new DateTime(2015, 03, 13, 0, 0, 0, DateTimeKind.Utc),
                Announcement = "親愛的資餘簡訊-企業簡訊客戶您好：為提供更穩定的服務，資餘簡訊預定於2015年03月19日AM:00:00~AM:08:00進行設備維護，維護期間會有數次瞬斷發生，但不影響正常使用，若遇到障礙，重試即可繼續正常運作。感謝您的了解與配合。",
                CreatedTime = createdTime,
                CreatedUser = createdUser,
            });

            list.Add(new SystemAnnouncement
            {
                IsVisible = true,
                PublishDate = new DateTime(2014, 11, 28, 0, 0, 0, DateTimeKind.Utc),
                Announcement = "親愛的客戶您好，如果您發送的簡訊內容有包含任何與選舉相關之文字內容，請務必記得將有效期限縮短設定在1小時以內(切勿超過11/28 24:00發送)，以避免接收端在收訊不佳或關機的情況下於投票當天收到簡訊而《違反選罷法》",
                CreatedTime = createdTime,
                CreatedUser = createdUser,
            });

            list.Add(new SystemAnnouncement
            {
                IsVisible = true,
                PublishDate = new DateTime(2014, 11, 28, 0, 0, 0, DateTimeKind.Utc),
                Announcement = "親愛的客戶您好，如果您發送的簡訊內容有包含任何與選舉相關之文字內容，請務必記得將有效期限縮短設定在1小時以內(切勿超過11/28 24:00發送)，以避免接收端在收訊不佳或關機的情況下於投票當天收到簡訊而《違反選罷法》",
                CreatedTime = createdTime,
                CreatedUser = createdUser,
            });

            IUnitOfWork unitOfWork = new UnitOfWork(context);
            var repository = unitOfWork.Repository<SystemAnnouncement>();

            foreach(var item in list){
                var entity = repository.Get(p => p.Announcement == item.Announcement);
         
                if(entity != null)continue;

                repository.Insert(item);
            }
        }

        private async Task CreateRules(ApplicationDbContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            foreach (var roleName in Enum.GetNames(typeof(Role)))
            {
                var result = await roleManager.RoleExistsAsync(roleName);
                if (!result)
                {
                    var role = new IdentityRole(roleName);
                    await roleManager.CreateAsync(role);
                }
            }
        }

        private async Task<ApplicationUser> CreateUser(
            ApplicationDbContext context,
            Role role,
            string username,
            string password,
            decimal smsBalance,
            string phoneNumber,
            ApplicationUser parent)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            var user = await userManager.FindByNameAsync(username);
            if (user == null)
            {
                int level = (parent != null) ? parent.Level - 1 : int.MaxValue - 1 /* 資料庫最大整數值為 int.MaxValue - 1 */;
                string parentId = (parent != null) ? parent.Id : null;

                user = new ApplicationUser { Level = level, UserName = username, FullName = username, SmsBalanceExpireDate = DateTime.MaxValue };
                user.Level = level;
                user.ParentId = parentId;
                user.UserName = username;
                user.SmsBalance = smsBalance;
                user.SmsBalanceExpireDate = DateTime.MaxValue;
                user.Department = null;
                user.EmployeeNo = null;
                user.PhoneNumber = phoneNumber;
                user.Email = null;
                user.Enabled = true;

                // 建立帳號
                await userManager.CreateAsync(user, password);

                // 加入腳色
                await userManager.AddToRoleAsync(user.Id, role.ToString());

                var creditWarning = new CreditWarning
                {
                    Enabled = CreditWarning.DefaultValue_Enabled,
                    BySmsMessage = CreditWarning.DefaultValue_BySmsMessage,
                    ByEmail = CreditWarning.DefaultValue_ByEmail,
                    LastNotifiedTime = null,
                    NotifiedInterval = CreditWarning.DefaultValue_NotifiedInterval,
                    Owner = user,
                };
                context.CreditWarnings.Add(creditWarning);
                context.SaveChanges();

                var replyCc = new ReplyCc
                {
                    Enabled = ReplyCc.DefaultValue_Enabled,
                    BySmsMessage = ReplyCc.DefaultValue_BySmsMessage,
                    ByEmail = ReplyCc.DefaultValue_ByEmail,
                    Owner = user,
                };
                context.ReplyCcs.Add(replyCc);
                context.SaveChanges();

                // 建立預設群組 [常用聯絡人]
                var group = new Group();
                group.Name = Group.CommonContactGroupName;
                group.Description = Group.CommonContactGroupName;
                group.Deletable = false;
                group.CreatedUser = user;
                context.Groups.Add(group);
                context.SaveChanges();
            }

            return user;
        }

        private void CreateMenus(ApplicationDbContext context)
        {
            var list = new[] 
            { 
                // 01
                new {
                    webAuthorization = new WebAuthorization
                    {
                        ControllerName = "Home",
                        ActionName = "SendMessage",
                        Roles = string.Join(",", new Role[] { Role.Administrator, Role.Supervisor, Role.DepartmentHead, Role.Employee }),
                        Users = string.Empty,
                        Remark = "簡訊發送",
                    },
                    menuItem = new MenuItem
                    {
                        Level = 1,
                        ParentId = null,
                        Name = "簡訊發送",
                        MapRouteUrl = "SendMessage",
                        Order = 1,
                    }
                },
                // 02
                new {
                    webAuthorization = new WebAuthorization
                    {
                        ControllerName = "Home",
                        ActionName = "SendParamMessage",
                        Roles = string.Join(",", new Role[] { Role.Administrator, Role.Supervisor, Role.DepartmentHead, Role.Employee }),
                        Users = string.Empty,
                        Remark = "參數簡訊發送",
                    },
                    menuItem = new MenuItem
                    {
                        Level = 1,
                        ParentId = null,
                        Name = "參數簡訊發送",
                        MapRouteUrl = "SendParamMessage",
                        Order = 2,
                    }
                },
                // 03
                new {
                    webAuthorization = new WebAuthorization
                    {
                        ControllerName = "Home",
                        ActionName = "SearchMemberSend",
                        Roles = string.Join(",", new Role[] { Role.Administrator, Role.Supervisor, Role.DepartmentHead, Role.Employee }),
                        Users = string.Empty,
                        Remark = "發送查詢",
                    },
                    menuItem = new MenuItem
                    {
                        Level = 1,
                        ParentId = null,
                        Name = "發送查詢",
                        MapRouteUrl = "SearchMemberSend",
                        Order = 3,
                    }
                },
                // 04
                new {
                    webAuthorization = new WebAuthorization
                    {
                        ControllerName = "Home",
                        ActionName = "ContactManager",
                        Roles = string.Join(",", new Role[] { Role.Administrator, Role.Supervisor, Role.DepartmentHead, Role.Employee }),
                        Users = string.Empty,
                        Remark = "聯絡人管理",
                    },
                    menuItem = new MenuItem
                    {
                        Level = 1,
                        ParentId = null,
                        Name = "聯絡人管理",
                        MapRouteUrl = "ContactManager",
                        Order = 4,
                    }
                },
                // 05
                new {
                    webAuthorization = new WebAuthorization
                    {
                        ControllerName = "Home",
                        ActionName = "SMS_Setting",
                        Roles = string.Join(",", new Role[] { Role.Administrator, Role.Supervisor, Role.DepartmentHead, Role.Employee }),
                        Users = string.Empty,
                        Remark = "系統設定",
                    },
                    menuItem = new MenuItem
                    {
                        Level = 1,
                        ParentId = null,
                        Name = "系統設定",
                        MapRouteUrl = "SMS_Setting",
                        Order = 5,
                    }
                },

                // 06
                new {
                    webAuthorization = new WebAuthorization
                    {
                        ControllerName = "Home",
                        ActionName = "RecurringSMS",
                        Roles = string.Join(",", new Role[] { Role.Administrator, Role.Supervisor, Role.DepartmentHead, Role.Employee }),
                        Users = string.Empty,
                        Remark = "預約/週期簡訊維護",
                    },
                    menuItem = new MenuItem
                    {
                        Level = 1,
                        ParentId = null,
                        Name = "預約/週期簡訊維護",
                        MapRouteUrl = "RecurringSMS",
                        Order = 6,
                    }
                },
                // 07
                new {
                    webAuthorization = new WebAuthorization
                    {
                        ControllerName = "Home",
                        ActionName = "DepartmentManager",
                        Roles = string.Join(",", new Role[] { Role.Administrator, Role.Supervisor, Role.DepartmentHead }),
                        Users = string.Empty,
                        Remark = "子帳號管理",
                    },
                    menuItem = new MenuItem
                    {
                        Level = 1,
                        ParentId = null,
                        Name = "子帳號管理",
                        MapRouteUrl = "DepartmentManager",
                        Order = 7,
                    }
                },
                // 08
                new {
                    webAuthorization = new WebAuthorization
                    {
                        ControllerName = "Home",
                        ActionName = "DepartmentPointManager",
                        Roles = string.Join(",", new Role[] { Role.Administrator, Role.Supervisor, Role.DepartmentHead }),
                        Users = string.Empty,
                        Remark = "子帳號點數管理",
                    },
                    menuItem = new MenuItem
                    {
                        Level = 1,
                        ParentId = null,
                        Name = "子帳號點數管理",
                        MapRouteUrl = "DepartmentPointManager",
                        Order = 8,
                    }
                },
                // 09
                new {
                    webAuthorization = new WebAuthorization
                    {
                        ControllerName = "Home",
                        ActionName = "SectorStatistics",
                        Roles = string.Join(",", new Role[] { Role.Administrator, Role.Supervisor, Role.DepartmentHead }),
                        Users = string.Empty,
                        Remark = "報表管理",
                    },
                    menuItem = new MenuItem
                    {
                        Level = 1,
                        ParentId = null,
                        Name = "報表管理",
                        MapRouteUrl = "SectorStatistics",
                        Order = 9,
                    }
                },
                // 10
                new {
                    webAuthorization = new WebAuthorization
                    {
                        ControllerName = "Home",
                        ActionName = "Others",
                        Roles = string.Join(",", new Role[] { Role.Administrator }),
                        Users = string.Empty,
                        Remark = "其他功能",
                    },
                    menuItem = new MenuItem
                    {
                        Level = 1,
                        ParentId = null,
                        Name = "其他功能",
                        MapRouteUrl = "Others",
                        Order = 10,
                    }
                },

            }.ToList();

            foreach (var data in list)
            {
                var foundWebAuthorization = context.WebAuthorizations.FirstOrDefault(p => p.Remark == data.webAuthorization.Remark);
                if (foundWebAuthorization == null)
                {
                    foundWebAuthorization = context.WebAuthorizations.Add(data.webAuthorization);
                    context.SaveChanges();
                }

                var foundMenuItem = context.MenuItems.FirstOrDefault(p => p.Name == foundWebAuthorization.Remark);
                if (foundMenuItem == null)
                {
                    data.menuItem.WebAuthorizationId = foundWebAuthorization.Id;
                    foundMenuItem = context.MenuItems.Add(data.menuItem);
                    context.SaveChanges();
                }
            }
        }

        private void CreateTestingData(ApplicationDbContext context)
        {
            ApplicationUser parentUser = null;

            var userNames = new Dictionary<Role, string>
            {
                {Role.Administrator, "Admin"},
                {Role.Supervisor, "Eric"},
                {Role.DepartmentHead, "Dino"},
                {Role.Employee, "Norman"},
            };

            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            // 由大到小排列
            var roles = Enum.GetValues(typeof(Role)).Cast<Role>().OrderByDescending(x => (int)x).ToList();
            
            foreach (Role role in roles)
            {
                if (!userNames.ContainsKey(role)) continue;

                var userName = userNames[role];

                var user = userManager.FindByNameAsync(userName).GetAwaiter().GetResult();

                if (user == null)
                {
                    user = CreateUser(context,
                        role: role,
                        username: userNames[role],
                        password: "123456",
                        smsBalance: 100M,
                        phoneNumber: "+886921859698",
                        parent: parentUser)
                    .GetAwaiter().GetResult();

                    new SampleData().Seed(context, user);
                }

                parentUser = user;

            }
        }


        
    }
}