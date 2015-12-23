﻿using AutoMapper;
using EFunTech.Sms.Portal.Controllers.Common;
using EFunTech.Sms.Portal.Models;
using EFunTech.Sms.Schema;
using JUtilSharp.Database;
using LinqKit;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using EntityFramework.Extensions;
using EntityFramework.Caching;

namespace EFunTech.Sms.Portal.Controllers
{
    public class LookupApiController : ApiControllerBase
    {
        public LookupApiController(IUnitOfWork unitOfWork, ILogService logService) : base(unitOfWork, logService) { }

        /// <summary>
        /// 取得目前所有帳號，用來驗證輸入使用者是否已經建立，避免每次輸入一個字元都會透過AJAX去伺服器驗證
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/LookupApi/GetExistentUserNames")]
        public IEnumerable<string> GetExistentUserNames()
        {
            return this.unitOfWork.Repository<ApplicationUser>()
                .GetAll()
                .Select(p => p.UserName)
                //.FromCache(CachePolicy.WithDurationExpiration(TimeSpan.FromSeconds(30)), tags: new[] { "GetExistentUserNames"})
                ;
        }

        [HttpGet]
        [Route("api/LookupApi/GetCurrentUser")]
        public ApplicationUserModel GetCurrentUser()
        {
            ApplicationUserModel model = Mapper.Map<ApplicationUser, ApplicationUserModel>(CurrentUser);

            switch (CurrentUserRole)
            {
                case Role.Administrator:
                case Role.Supervisor:
                    model.CanEditDepartment = true;
                    break;
                case Role.DepartmentHead:
                case Role.Employee:
                    model.CanEditDepartment = false;
                    break;
            }
            
            return model;
        }
        
        [HttpGet]
        [Route("api/LookupApi/GetAvailableRoles")]
        public IEnumerable<TitleMapModel<string, string>> GetAvailableRoles()
        {
            // 根據目前使用者角色，只能取比小於等於自己權限的角色
            //var availableRoleNames = Enum.GetValues(typeof(Role))
            //                            .Cast<Role>()
            //                            .OrderByDescending(p => (int)p)
            //                            .AsQueryable()
            //                            .Where(role => (int)role <= (int)CurrentUserRole && role != Role.Unknown)
            //                            .Select(role => role.ToString());

            // 20151030 Norman, 改成以下邏輯，避免Admin可以建立Employee，理論上是不合理的
            
            var role = CurrentUserRole;

            var availableRoleNames = new List<string>();

            switch (role)
            {
                case Role.Administrator:
                    {
                        availableRoleNames.Add(Role.Administrator.ToString());
                        availableRoleNames.Add(Role.Supervisor.ToString());
                    } break;

                case Role.Supervisor:
                    {
                        availableRoleNames.Add(Role.Supervisor.ToString());
                        availableRoleNames.Add(Role.DepartmentHead.ToString());
                    } break;

                case Role.DepartmentHead:
                    {
                        availableRoleNames.Add(Role.DepartmentHead.ToString());
                        availableRoleNames.Add(Role.Employee.ToString());
                    } break;

                case Role.Employee:
                    {
                        availableRoleNames.Add(Role.Employee.ToString());
                    } break;
            }

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(this.unitOfWork.DbContext));

            var result = roleManager.Roles
                .Where(p => availableRoleNames.Contains(p.Name))
                //.FromCache(CachePolicy.WithDurationExpiration(TimeSpan.FromSeconds(30)), tags: new[] { "GetAvailableRoles", role.ToString() })
                .Select(p => new TitleMapModel<string, string>
                {
                    name = AttributeHelper.GetColumnDescription((Role)(Enum.Parse(typeof(Role), p.Name))),
                    value = p.Id
                });

            return result;
        }

        /// <summary>
        /// 分享聯絡人功能頁，取得可以使用的所有部門
        /// </summary>
        [HttpGet]
        [Route("api/LookupApi/GetAvailableDepartments_DepartmentManager")]
        public IEnumerable<TitleMapModel<string, int>> GetAvailableDepartments_DepartmentManager()
        {
            var role = CurrentUserRole;

            switch (role)
            {
                case Role.Administrator:
                    {
                        // Administrator 可選取所有部門
                        var result = this.unitOfWork.Repository<Department>()
                                         .GetAll()
                                         //.FromCache(CachePolicy.WithDurationExpiration(TimeSpan.FromSeconds(30)), tags: new[] { "GetAvailableDepartments_DepartmentManager", role.ToString() })
                                         .Select(p => new TitleMapModel<string, int>
                                         {
                                             name = p.Name,
                                             value = p.Id,
                                         }).ToList();

                        result.Insert(0, new TitleMapModel<string, int>
                            {
                                name = "不指定",
                                value = 0,
                            });

                        return result;
                    }

                case Role.Supervisor:
                    {
                        // Supervisor 建立的所有部門
                        var result = this.unitOfWork.Repository<Department>()
                                         .GetMany(p => p.CreatedUser.Id == CurrentUserId)
                                         //.FromCache(CachePolicy.WithDurationExpiration(TimeSpan.FromSeconds(30)), tags: new[] { "GetAvailableDepartments_DepartmentManager", role.ToString() })
                                         .Select(p => new TitleMapModel<string, int>
                                         {
                                             name = p.Name,
                                             value = p.Id,
                                         }).ToList();
                        result.Insert(0, new TitleMapModel<string, int>
                        {
                            name = "不指定",
                            value = 0,
                        });

                        return result;
                    }

                case Role.DepartmentHead:
                    {
                        // Supervisor 本身所在部門
                        var result = new List<TitleMapModel<string, int>>();
                        result.Add(new TitleMapModel<string, int>
                        {
                            name = CurrentUser.Department.Name,
                            value = CurrentUser.Department.Id,
                        });
                        return result;
                    }

                case Role.Employee:
                default:
                    {
                        // Employee 無任何部門(Employee 不能建立使用者，避免 Employee 具備建立帳號功能)
                        return Enumerable.Empty<TitleMapModel<string, int>>();
                    }
            }
        }

        /// <summary>
        /// 分享聯絡人功能頁，取得可以使用的所有部門
        /// </summary>
        [HttpGet]
        [Route("api/LookupApi/GetAvailableDepartments_ShareContact")]
        public IEnumerable<TitleMapModel<string, int>> GetAvailableDepartments_ShareContact()
        {
            var role = CurrentUserRole;

            var predicate = PredicateBuilder.True<Department>();

            switch (role)
            {
                case Role.Administrator:
                    {
                        // Administrator 可選取所有部門
                    }break;

                case Role.Supervisor:
                case Role.DepartmentHead:
                    {
                        // 可顯示內容：本身所在部門 + 建立的所有部門
                        var innerPredicate = PredicateBuilder.False<Department>();
                        
                        if(CurrentUser.Department != null)
                            innerPredicate = innerPredicate.Or(p => p.Id == CurrentUser.Department.Id);

                        innerPredicate = innerPredicate.Or(p => p.CreatedUser.Id == CurrentUserId);

                        predicate = predicate.And(innerPredicate);
                    }break;

                case Role.Employee:
                    {
                        // 可顯示內容：本身所在部門
                        var innerPredicate = PredicateBuilder.False<Department>();

                        if (CurrentUser.Department != null)
                            innerPredicate = innerPredicate.Or(p => p.Id == CurrentUser.Department.Id);

                        predicate = predicate.And(innerPredicate);
                    }break;
                default:
                    {
                        var innerPredicate = PredicateBuilder.False<Department>();

                        predicate = predicate.And(innerPredicate);
                    }break;
            }

            var result = this.unitOfWork.Repository<Department>().DbSet
                            .AsExpandable()
                            .Where(predicate)
                            //.FromCache(CachePolicy.WithDurationExpiration(TimeSpan.FromSeconds(30)), tags: new[] { "GetAvailableDepartments_ShareContact", role.ToString() })
                            .Select(p => new TitleMapModel<string, int>
                            {
                                name = p.Name,
                                value = p.Id,
                            });

            return result;
        }

    }
}
