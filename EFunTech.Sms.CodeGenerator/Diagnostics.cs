
using EFunTech.Sms.Portal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFunTech.Sms.CodeGenerator
{
    public static class Diagnostics
    {
        private static ILogService logService;

        static Diagnostics(){
            logService = new SmartInspectLogService();
        }

        public static void Check()
        {
            CheckApiUrl();
            CheckFileEncoding();
        }

        private static void CheckApiUrl()
        {
            logService.Error("TODO: CheckApiUrl");
        }

        private static void CheckFileEncoding()
        {
            logService.Error("TODO: CheckFileEncoding");
        }

        private static void CheckController()
        {
            logService.Error("TODO: 檢查所有的Controller以及ApiController都繼承特定EFunTechController, EFunTechApiController");
        }
    }
}
