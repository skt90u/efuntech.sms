using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;
using System.Web.Http.Results;

namespace BasicAuthentication.Filters
{                                                                   
    public abstract class BasicAuthenticationAttribute : Attribute, IAuthenticationFilter
    {
        public string Realm { get; set; }

        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            HttpRequestMessage request = context.Request;
            AuthenticationHeaderValue authorization = request.Headers.Authorization;

            if (authorization == null)
            {
                // No authentication was attempted (for this authentication method).
                // Do not set either Principal (which would indicate success) or ErrorResult (indicating an error).
                //return;

                // 20151015 Norman, 若指定使用 BasicAuthenticationAttribute，就必須遵照 Basic Auth 定義規則
                context.ErrorResult = new AuthenticationFailureResult("Can not find authentication be defined in request header", request);
                return;
            }

            if (authorization.Scheme != "Basic")
            {
                // No authentication was attempted (for this authentication method).
                // Do not set either Principal (which would indicate success) or ErrorResult (indicating an error).
                //return;

                // 20151015 Norman, 若指定使用 BasicAuthenticationAttribute，就必須遵照 Basic Auth 定義規則
                context.ErrorResult = new AuthenticationFailureResult("Authentication defined in request header is not correct format about Basic Access Authentication ", request);
                return;
            }

            if (String.IsNullOrEmpty(authorization.Parameter))
            {
                // Authentication was attempted but failed. Set ErrorResult to indicate an error.
                //context.ErrorResult = new AuthenticationFailureResult("Missing credentials", request);
                //return;

                // 20151015 Norman, 若指定使用 BasicAuthenticationAttribute，就必須遵照 Basic Auth 定義規則
                context.ErrorResult = new AuthenticationFailureResult("Authentication defined in request header can not get parameter", request);
                return;
            }

            Tuple<string, string> userNameAndPasword = ExtractUserNameAndPassword(authorization.Parameter);

            if (userNameAndPasword == null)
            {
                // Authentication was attempted but failed. Set ErrorResult to indicate an error.
                context.ErrorResult = new AuthenticationFailureResult("Invalid credentials", request);
                return;
            }

            string userName = userNameAndPasword.Item1;
            string password = userNameAndPasword.Item2;

            IPrincipal principal = await AuthenticateAsync(userName, password, cancellationToken);

            if (principal == null)
            {
                // Authentication was attempted but failed. Set ErrorResult to indicate an error.
                context.ErrorResult = new AuthenticationFailureResult("Invalid username or password", request);
            }
            else
            {
                // Authentication was attempted and succeeded. Set Principal to the authenticated user.
                context.Principal = principal;
                context.Request.GetRequestContext().Principal = principal; // 讓 DbLogService 能夠存取的到
            }
        }

        protected abstract Task<IPrincipal> AuthenticateAsync(string userName, string password,
            CancellationToken cancellationToken);

        private static Tuple<string, string> ExtractUserNameAndPassword(string authorizationParameter)
        {
            byte[] credentialBytes;

            try
            {
                credentialBytes = Convert.FromBase64String(authorizationParameter);
            }
            catch (FormatException)
            {
                return null;
            }

            // The currently approved HTTP 1.1 specification says characters here are ISO-8859-1.
            // However, the current draft updated specification for HTTP 1.1 indicates this encoding is infrequently
            // used in practice and defines behavior only for ASCII.
            Encoding encoding = Encoding.ASCII;
            // Make a writable copy of the encoding to enable setting a decoder fallback.
            encoding = (Encoding)encoding.Clone();
            // Fail on invalid bytes rather than silently replacing and continuing.
            encoding.DecoderFallback = DecoderFallback.ExceptionFallback;
            string decodedCredentials;

            try
            {
                decodedCredentials = encoding.GetString(credentialBytes);
            }
            catch (DecoderFallbackException)
            {
                return null;
            }

            if (String.IsNullOrEmpty(decodedCredentials))
            {
                return null;
            }

            int colonIndex = decodedCredentials.IndexOf(':');

            if (colonIndex == -1)
            {
                return null;
            }

            string userName = decodedCredentials.Substring(0, colonIndex);
            string password = decodedCredentials.Substring(colonIndex + 1);
            return new Tuple<string, string>(userName, password);
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            Challenge(context);
            return Task.FromResult(0);
        }

        private void Challenge(HttpAuthenticationChallengeContext context)
        {
            string parameter;

            if (String.IsNullOrEmpty(Realm))
            {
                parameter = null;
            }
            else
            {
                // A correct implementation should verify that Realm does not contain a quote character unless properly
                // escaped (precededed by a backslash that is not itself escaped).
                parameter = "realm=\"" + Realm + "\"";
            }

            context.ChallengeWith("Basic", parameter);
        }

        public virtual bool AllowMultiple
        {
            get { return false; }
        }
    }

    public class AuthenticationFailureResult : IHttpActionResult
    {
        public AuthenticationFailureResult(string reasonPhrase, HttpRequestMessage request)
        {
            ReasonPhrase = reasonPhrase;
            Request = request;
        }

        public string ReasonPhrase { get; private set; }

        public HttpRequestMessage Request { get; private set; }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(Execute());
        }

        private HttpResponseMessage Execute()
        {
            //HttpResponseMessage response = new HttpResponseMessage(IsAjaxRequest 
            //    ? HttpStatusCode.NotAcceptable 
            //    : HttpStatusCode.Unauthorized);
            //response.RequestMessage = Request;
            //response.ReasonPhrase = ReasonPhrase;

            ////////////////////////////////////////
            // 想要回傳 Json 內容
            ////////////////////////////////////////

            // CookieAuthenticationHandler
            // 由於我們使用 Startup.Auth.cs 中的 UseCookieAuthentication -> 針對對應的程式碼為 CookieAuthenticationHandler.cs 的 ApplyResponseChallengeAsync ，
            // 他針對 Unauthorized (401) 會自動會 Redirect 到登入頁，因此針對 Ajax 我想到的方式是用非 Unauthorized (NotAcceptable) 取代他
            HttpStatusCode statusCode = IsAjaxRequest ? HttpStatusCode.NotAcceptable : HttpStatusCode.Unauthorized;
            HttpResponseMessage response = this.Request.CreateResponse(statusCode, new
            {
                ErrorMessage = ReasonPhrase
            });
            // response.ReasonPhrase = ReasonPhrase;

            return response;
        }

        /// <summary>
        /// 這個方法還不錯，判斷連結的頁面。
        /// http://stackoverflow.com/questions/20149750/owin-unauthorised-webapi-call-returning-login-page-rather-than-401
        /// </summary>
        private bool IsAjaxRequest
        {
            get
            {
                //HttpRequestHeaders headers = this.Request.Headers;
                //if (headers["X-Requested-With"] != null && headers["X-Requested-With"] == "XMLHttpRequest")
                //    return true;
            
                string apiPath = VirtualPathUtility.ToAbsolute("~/api/");
                return this.Request.RequestUri.LocalPath.StartsWith(apiPath, StringComparison.OrdinalIgnoreCase);
            }
        } 
    }
}