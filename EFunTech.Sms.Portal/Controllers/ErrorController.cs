using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using JUtilSharp.Database;

using EFunTech.Sms.Portal.Controllers.Common;

namespace EFunTech.Sms.Portal.Controllers
{
    public class ErrorController : MvcControllerBase
    {
        public ErrorController(IUnitOfWork unitOfWork, ILogService logService)
			: base(unitOfWork, logService)
		{
		}

        private string GetExceptionMessage(Exception ex)
        {
            List<Exception> exceptions = new List<Exception>();

            Exception currentException = ex;

            while (currentException != null)
            {
                exceptions.Add(currentException);

                currentException = currentException.InnerException;
            }

            exceptions.Reverse();

            return string.Join(" | ", exceptions.Select((e, i) => e.Message));
        }

        public ActionResult Index(Exception error)
        {
            Response.StatusCode = 500;

            var message = GetExceptionMessage(error);

            ViewBag.Message = "AAAA";
            ViewBag.StackTrace = error.StackTrace.ToString();
            
            if(Request.IsAjaxRequest())
            {
                return View("AjaxRequestError");
            }
            else
            {
                return View("Error");
            }
        }

        public ActionResult HttpError404(Exception error)
        {
            Response.StatusCode = 404;
            return View("HttpError404", null);
        }

        public ActionResult HttpError505(Exception error)
        {
            Response.StatusCode = 505;
            return View();
        }
    }
}