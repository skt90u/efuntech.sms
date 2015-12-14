using System.Collections.Generic;
using System.Web;
using System.Web.Hosting;
using System.Web.Optimization;
using System.IO;

namespace EFunTech.Sms.Portal
{
    public class BundleConfig
    {
        // 如需「搭配」的詳細資訊，請瀏覽 http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //bundles.Clear();
            //bundles.ResetAll();
            
            //bundles.UseCdn = true;

            List<string> allCss = new List<string> { 
                "~/Scripts/bower_components/bootstrap/dist/css/bootstrap.css",
                "~/Scripts/bower_components/bootstrap/dist/css/bootstrap-theme.css",
                "~/Scripts/bower_components/font-awesome/css/font-awesome.css",
                "~/Scripts/bower_components/AngularJS-Toaster/toaster.css",
                "~/Scripts/bower_components/angular-material/angular-material.css",
                // angular-block-ui
                "~/Scripts/bower_components/angular-block-ui/dist/angular-block-ui.css",
                // angular-dialog-service
                "~/Scripts/bower_components/angular-dialog-service/dist/dialogs.css",
                //"~/Content/custom-dialogs",
                // angular-motion
                // http://mgcrea.github.io/angular-motion/#
                //"~/Scripts/bower_components/angular-motion/dist/angular-motion.css",
                "~/Content/spinkit.css",
                "~/Content/animate.css",

                // 目前只用在顯示系統公告
                // https://jamesflorentino.github.io/nanoScrollerJS/
                "~/Scripts/bower_components/nanoScrollerJS/css/nanoscroller.css",
                
                // angular-schema-form-datepicker (01)
                "~/Scripts/bower_components/pickadate/lib/themes/classic.css",
                "~/Scripts/bower_components/pickadate/lib/themes/classic.date.css",
                "~/Scripts/bower_components/pickadate/lib/themes/classic.time.css",

                // ui-grid.min.css 進行壓所會出問題，因此直接使用
                //"~/Scripts/bower_components/angular-ui-grid/ui-grid.css",

                // 一定放在所有CSS之後
                //"~/Content/in.css",
                //"~/Content/Site.css",
            };

            bundles.Add(new StyleBundle("~/Content/css").Include(allCss.ToArray()));

            List<string> allJs = new List<string>();

            List<string> jquery = new List<string>
            {
                //"~/Scripts/jquery-{version}.js", // v1.10.2
                // 只支援IE9+ (https://jquery.com/browser-support/)
                "~/Scripts/bower_components/jquery-legacy/dist/jquery.js", // jquery v1.11.3 (如果測試有問題，改回  v1.10.2)
                // "~/Scripts/bower_components/jquery-modern/dist/jquery.js", // jquery 最新版(20150816為止為 v2.1.4)

                // 目前只用在顯示系統公告
                // https://jamesflorentino.github.io/nanoScrollerJS/
                "~/Scripts/bower_components/nanoScrollerJS/javascripts/overthrow.min.js",
                "~/Scripts/bower_components/nanoScrollerJS/javascripts/jquery.nanoscroller.js",
                
                // http://underscorejs.org
                // https://github.com/floydsoft/angular-underscore
                "~/Scripts/bower_components/underscore/underscore.js",

                "~/Scripts/bower_components/moment/moment.js",
                //"~/Scripts/bower_components/moment-timezone/moment-timezone.js",
                "~/Scripts/bower_components/moment-timezone/builds/moment-timezone-with-data.js",

                // angular-schema-form-datepicker (02)
                "~/Scripts/bower_components/pickadate/lib/picker.js",
                "~/Scripts/bower_components/pickadate/lib/picker.date.js",
                "~/Scripts/bower_components/pickadate/lib/picker.time.js",
                "~/Scripts/bower_components/pickadate/lib/translations/zh_TW.js",

                // libphonenumber (不知道怎麼用，不知道要用哪一個)
                //"~/Scripts/bower_components/libphonenumber/libphonenumber.js",
                //"~/Scripts/bower_components/libphonenumber/dist/libphonenumber.js",

                // phoneformat
                // 使用範例：http://www.phoneformat.com/js/source.js
                // http://www.phoneformat.com/
                "~/Scripts/bower_components/phoneformat.js/dist/phone-format.js",
            };
            allJs.AddRange(jquery); //jquery 與 angular 放在一起 找不到 $ 符號

            List<string> angular = new List<string> { 
                "~/Scripts/bower_components/angular/angular.js",
                "~/Scripts/bower_components/angular-i18n/angular-locale_zh-tw.js",
                "~/Scripts/bower_components/angular-animate/angular-animate.js",
                "~/Scripts/bower_components/angular-cookies/angular-cookies.js",
                "~/Scripts/bower_components/angular-bootstrap/ui-bootstrap-tpls.js",
                "~/Scripts/bower_components/angular-ui-grid/ui-grid.js",
                // underscore
                "~/Scripts/bower_components/angular-underscore/angular-underscore.js",
                // angular-schema-form
                "~/Scripts/bower_components/tv4/tv4.js",
                "~/Scripts/bower_components/angular-sanitize/angular-sanitize.js",
                "~/Scripts/bower_components/objectpath/lib/ObjectPath.js",
                "~/Scripts/bower_components/angular-schema-form/dist/schema-form.js",
                "~/Scripts/bower_components/angular-schema-form/dist/bootstrap-decorator.js",
                // AngularJS-Toaster
                "~/Scripts/bower_components/AngularJS-Toaster/toaster.js",
                // Angular Material
                //"~/Scripts/bower_components/angular-aria/angular-aria.js",
                //"~/Scripts/bower_components/angular-material/angular-material.js",
                // angular-block-ui
                "~/Scripts/bower_components/angular-block-ui/dist/angular-block-ui.js",
                // angular-dialog-service
                // https://github.com/m-e-conroy/angular-dialog-service
                "~/Scripts/bower_components/angular-translate/angular-translate.js",
                "~/Scripts/bower_components/angular-dialog-service/dist/dialogs-default-translations.js",
                "~/Scripts/bower_components/angular-dialog-service/dist/dialogs.js",
                // angular-strap
                // http://mgcrea.github.io/angular-strap/
                // https://github.com/mgcrea/angular-strap
                //"~/Scripts/bower_components/angular-strap/dist/angular-strap.js",
                //"~/Scripts/bower_components/angular-strap/dist/angular-strap.tpl.js",
                // FlieSaver
                //"~/Scripts/bower_components/file-saver.js/FileSaver.js",
                //"~/Scripts/bower_components/blob-polyfill/Blob.js",
                //"~/Scripts/bower_components/angular-file-saver/dist/angular-file-saver.js",

                // angular-schema-form-datepicker (03)
                "~/Scripts/bower_components/angular-schema-form-datepicker/bootstrap-datepicker.js",

            };
            allJs.AddRange(angular);
            
            allJs.AddRange(GetAppScripts());

            bundles.Add(new ScriptBundle("~/bundles/js").Include(allJs.ToArray()));

            //----------------------------------------
            // jqueryval
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include("~/Scripts/bower_components/jquery.validate/dist/jquery.validate.js"));

            //----------------------------------------
            // 各個頁面相關
            BundleControllers(bundles);

            //----------------------------------------
            // 手動進行最佳化
            // http://www.dotblogs.com.tw/yowko/archive/2013/01/10/87120.aspx
            // 
            // 對於壓縮後，有問題的項目，直接引用他的min版本，不使用bundle進行管理

            //BundleTable.EnableOptimizations = true;//用來強制啟用打包及最佳化
            /*
            Note: Unless EnableOptimizations is true or the debug attribute in the compilation Element in the 
                  Web.config file is set to false, files will not be bundled or minified. Additionally, the 
                  .min version of files will not be used,  the full debug versions will be selected. EnableOptimizations  
                  overrides the debug attribute in the compilation Element in the Web.config file.
            */
        }

        private static void BundleControllers(BundleCollection bundles)
        {
            string applicationPhysicalDir = HostingEnvironment.MapPath("~/");

            string controllersDir = Path.Combine(applicationPhysicalDir, @"Scripts\Application\Controllers");

            foreach (string dir in Directory.GetDirectories(controllersDir))
            {
                string folderName = Path.GetFileName(dir); // 這樣就只會取到資料夾名稱，而非全路徑

                bundles.Add(new ScriptBundle("~/bundles/" + folderName).Include(GetControllerScripts(folderName).ToArray()));
            }
        }

        private static List<string> GetControllerScripts(string controllerName)
        {
            List<string> scripts = new List<string>();

            scripts.AddRange(GetScripts(@"Scripts\Application\Controllers\" + controllerName));

            return scripts;
        }

        private static List<string> GetAppScripts()
        {
            List<string> scripts = new List<string>();

            // Modules (要放在 app 之前，定義的 module 都是被 app 所用的)
            scripts.AddRange(GetScripts(@"Scripts\Application\Modules"));
            // app
            scripts.AddRange(GetScripts(@"Scripts\Application"));
            // Constants
            scripts.AddRange(GetScripts(@"Scripts\Application\Constants"));
            // Configs
            scripts.AddRange(GetScripts(@"Scripts\Application\Configs"));
            // Decorators
            //scripts.AddRange(GetScripts(@"Scripts\Application\Decorators"));
            // Filters
            scripts.AddRange(GetScripts(@"Scripts\Application\Filters"));
            // Directives
            scripts.AddRange(GetScripts(@"Scripts\Application\Directives"));
            // Services
            scripts.AddRange(GetScripts(@"Scripts\Application\Services"));
            // Factories
            scripts.AddRange(GetScripts(@"Scripts\Application\Factories"));
            // Runs - SchemaForm
            scripts.AddRange(GetScripts(@"Scripts\Application\Runs\SchemaForm"));
            // Runs - Template
            scripts.AddRange(GetScripts(@"Scripts\Application\Runs\Template"));
            // Runs - Configs
            scripts.AddRange(GetScripts(@"Scripts\Application\Runs\Configs"));
            // Conttrollers - Common
            scripts.AddRange(GetScripts(@"Scripts\Application\Controllers\Common"));

            return scripts;
        }

        private static string ToRelativePath(string physicalPath)
        {
            string applicationPhysicalDir = HostingEnvironment.MapPath("~/");

            return physicalPath.Replace(applicationPhysicalDir, "~/").Replace(@"\", "/");
        }

        private static List<string> GetScripts(string relativeDir)
        {
            List<string> scripts = new List<string>();

            string applicationPhysicalDir = HostingEnvironment.MapPath("~/");

            string dir = Path.Combine(applicationPhysicalDir, relativeDir);

            if (!Directory.Exists(dir)) return scripts;

            var files = Directory.GetFiles(dir, "*.js");

            foreach (var file in files)
            {
                scripts.Add(ToRelativePath(file));
            }

            return scripts;
        }
    }
}
