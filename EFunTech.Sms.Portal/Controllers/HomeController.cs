using System;
using System.Linq;
using System.Web.Mvc;
using JUtilSharp.Database;
using EFunTech.Sms.Portal.Controllers.Common;
using EFunTech.Sms.Portal.Models;
using WebMarkupMin.Mvc.ActionFilters;

namespace EFunTech.Sms.Portal.Controllers
{
    public class HomeController : MvcControllerBase
    {
        public HomeController(IUnitOfWork unitOfWork, ILogService logService)
			: base(unitOfWork, logService)
		{
		}

        public ActionResult Index()
        {
            try
            {
                MenuItemModel menuItem = GetMenuItems().FirstOrDefault();
                if (menuItem != null)
                {
                    return View();

                    //return RedirectToAction(
                    //    menuItem.WebAuthorization.ActionName, 
                    //    menuItem.WebAuthorization.ControllerName);

                    //return RedirectToAction(
                    //    menuItem.ActionName,
                    //    menuItem.ControllerName);
                }
                else
                {
                    // 沒有任何MenuItem
                    return RedirectToAction("Login", "Account");
                }
                //return View();
            }
            catch(Exception ex)
            {
                this.logService.Error(ex);
                return RedirectToAction("Login", "Account");
            }
            
        }

        public ActionResult SendMessage() 
        { 
            if(GetMenuItems().Count == 0)
                return RedirectToAction("Login", "Account", new { ReturnUrl = "SendMessage" });
            
            return View(); 
        }

        public ActionResult SendParamMessage()
        {
            if (GetMenuItems().Count == 0)
                return RedirectToAction("Login", "Account", new { ReturnUrl = "SendParamMessage" });

            return View();
        }

        
        public ActionResult SearchMemberSend()
        {
            if (GetMenuItems().Count == 0)
                return RedirectToAction("Login", "Account", new { ReturnUrl = "SearchMemberSend" });

            return View();
        }

        
        public ActionResult ContactManager()
        {
            if (GetMenuItems().Count == 0)
                return RedirectToAction("Login", "Account", new { ReturnUrl = "ContactManager" });

            return View();
        }

        
        public ActionResult SMS_Setting()
        {
            if (GetMenuItems().Count == 0)
                return RedirectToAction("Login", "Account", new { ReturnUrl = "SMS_Setting" });

            return View();
        }

        
        public ActionResult RecurringSMS()
        {
            if (GetMenuItems().Count == 0)
                return RedirectToAction("Login", "Account", new { ReturnUrl = "RecurringSMS" });

            return View();
        }

        
        public ActionResult DepartmentManager()
        {
            if (GetMenuItems().Count == 0)
                return RedirectToAction("Login", "Account", new { ReturnUrl = "DepartmentManager" });

            return View();
        }

        
        public ActionResult DepartmentPointManager()
        {
            if (GetMenuItems().Count == 0)
                return RedirectToAction("Login", "Account", new { ReturnUrl = "DepartmentPointManager" });

            return View();
        }

        
        public ActionResult SectorStatistics()
        {
            if (GetMenuItems().Count == 0)
                return RedirectToAction("Login", "Account", new { ReturnUrl = "SectorStatistics" });

            return View();
        }

        public ActionResult Others()
        {
            if (GetMenuItems().Count == 0)
                return RedirectToAction("Login", "Account", new { ReturnUrl = "Others" });

            return View();
        }
        
        
    }
}