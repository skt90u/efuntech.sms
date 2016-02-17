using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Http.Controllers;

namespace EFunTech.Sms.Portal.Filters
{
    /// <summary>
    /// http://remy.supertext.ch/2012/04/basic-http-authorization-for-web-api-in-mvc-4-beta/
    /// </summary>
    public class BasicAuthorizeAttribute : System.Web.Http.AuthorizeAttribute
    {
        private bool requireAuthentication = true;

        public bool RequireAuthentication
        {
            get { return requireAuthentication; }
            set { requireAuthentication = value; }
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var result = new HttpResponseMessage()
            {
                //StatusCode = HttpStatusCode.Unauthorized,
                StatusCode = HttpStatusCode.BadRequest,
                RequestMessage = actionContext.Request,
            };

            actionContext.Response = result;
            //if (!RequireAuthentication || Authenticate(actionContext))
            //{
            //    return;
            //}
            //else
            //{
            //    HandleUnauthorizedRequest(actionContext);
            //}
        }

        protected override void HandleUnauthorizedRequest(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            //var challengeMessage = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
            //challengeMessage.Headers.Add("WWW-Authenticate", "Basic");
            //throw new HttpResponseException(challengeMessage);
        }


        private bool Authenticate(System.Web.Http.Controllers.HttpActionContext actionContext) //HttpRequestMessage input)
        {
            //if (RequireSsl && !HttpContext.Current.Request.IsSecureConnection && !HttpContext.Current.Request.IsLocal)
            //{
            //    log.Error("Failed to login: SSL:" + HttpContext.Current.Request.IsSecureConnection);
            //    return false;
            //}

            //if (!HttpContext.Current.Request.Headers.AllKeys.Contains("Authorization")) return false;

            //string authHeader = HttpContext.Current.Request.Headers["Authorization"];

            //IPrincipal principal;
            //if (TryGetPrincipal(authHeader, out principal))
            //{
            //    HttpContext.Current.User = principal;
            //    return true;
            //}
            return false;
        }


        private bool TryGetPrincipal(string authHeader, out IPrincipal principal)
        {
            var creds = ParseAuthHeader(authHeader);
            if (creds != null)
            {
                if (TryGetPrincipal(creds[0], creds[1], out principal)) return true;
            }

            principal = null;
            return false;
        }


        private string[] ParseAuthHeader(string authHeader)
        {
            // Check this is a Basic Auth header 
            if (authHeader == null || authHeader.Length == 0 || !authHeader.StartsWith("Basic", StringComparison.OrdinalIgnoreCase)) return null;

            // Pull out the Credentials with are seperated by ':' and Base64 encoded 
            string base64Credentials = authHeader.Substring(6);
            string[] credentials = Encoding.ASCII.GetString(Convert.FromBase64String(base64Credentials)).Split(new char[] { ':' });

            if (credentials.Length != 2 || string.IsNullOrEmpty(credentials[0]) || string.IsNullOrEmpty(credentials[0])) return null;

            // Okay this is the credentials 
            return credentials;
        }


        private bool TryGetPrincipal(string username, string password, out IPrincipal principal)
        {
            // this is the method that does the authentication 

            //users often add a copy/paste space at the end of the username
            username = username.Trim();
            password = password.Trim();

            principal = null;
            return true;
            //TODO
            //Replace this with your own Authentication Code
            //Person person = AccountManagement.ApiLogin(username, password);

            //if (person != null)
            //{
            //    // once the user is verified, assign it to an IPrincipal with the identity name and applicable roles
            //    principal = new GenericPrincipal(new GenericIdentity(username), System.Web.Security.Roles.GetRolesForUser(username));
            //    return true;
            //}
            //else
            //{
            //    if (!String.IsNullOrWhiteSpace(username))
            //    {
            //        log.Error("Failed to login: username=" + username + "; password=" + password);
            //    }
            //    principal = null;
            //    return false;
            //}
        }
    }
}