
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EFunTech.Sms.CodeGenerator
{
    public class SyncValidationApi : IJob
    {
        public string PortalDir { get; set; }
        public bool Overwrite { get; set; }

        private void WriteFunctions(List<string> lines)
        {
            Type type = typeof(EFunTech.Sms.Portal.Controllers.ValidationApiController);

            MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

            foreach (MethodInfo method in methods)
                WriteFunction(lines, method);
        }

        private void WriteFunction(List<string> lines, MethodInfo method)
        {
            var functionName = method.Name;

            ParameterInfo[] parameters = method.GetParameters();

            string functionDeclaration = string.Format("{0} ({1})",
                method.Name,
                //string.Join(", ", parameters.Select(p => string.Format("{0} {1}", p.ParameterType.ToString(), p.Name)))
                string.Join(", ", parameters.Select(p => p.Name))
                );

            lines.Add(string.Format("            '{0}', // {1}", functionName, functionDeclaration));
        }

        private List<string> CreateFileContent()
        {
            var lines = new List<string>();

            lines.Add(string.Format("// 請勿修改這個檔案，因為這個檔案是由 EFunTech.Sms.CodeGenerator 自動產生的，任何修改都將被覆蓋掉"));
            lines.Add(string.Format("(function (window, document) {{"));
            lines.Add(string.Format("    'use strict';"));
            lines.Add(string.Format(""));
            lines.Add(string.Format("    angular.module('app').factory('ValidationApi', ['$http', '$log', 'toaster', function ($http, $log, toaster) {{"));
            lines.Add(string.Format("        "));
            lines.Add(string.Format("        var functions = {{}};"));
            lines.Add(string.Format(""));

            lines.Add(string.Format("        var functionNames = ["));
            WriteFunctions(lines);
            lines.Add(string.Format("        ];"));

            lines.Add(string.Format("        "));
            lines.Add(string.Format("        angular.forEach(functionNames, function (functionName) {{"));
            lines.Add(string.Format(""));
            lines.Add(string.Format("            functions[functionName] = function (params, successFn, errorFn) {{"));
            lines.Add(string.Format(""));
            lines.Add(string.Format("                var url = '/api/ValidationApi/' + functionName;"));
            lines.Add(string.Format(""));
            lines.Add(string.Format("                var res = $http.get(url, {{ params: angular.copy(params) }});"));
            lines.Add(string.Format(""));
            lines.Add(string.Format("                res.success(function (data, status, headers, config, statusText) {{"));
            lines.Add(string.Format("                    $log.debug(arguments);"));
            lines.Add(string.Format("                    (successFn || angular.noop).apply(null, arguments);"));
            lines.Add(string.Format("                }});"));
            lines.Add(string.Format(""));
            lines.Add(string.Format("                res.error(function (data, status, headers, config, statusText) {{"));
            lines.Add(string.Format("                    $log.error(arguments);"));
            lines.Add(string.Format("                    (errorFn || angular.noop).apply(null, arguments);"));
            lines.Add(string.Format("                }});"));
            lines.Add(string.Format(""));
            lines.Add(string.Format("                return res;"));
            lines.Add(string.Format("            }};"));
            lines.Add(string.Format("        }});"));
            lines.Add(string.Format(""));
            lines.Add(string.Format("        return functions; // ValidationApi"));
            lines.Add(string.Format(""));
            lines.Add(string.Format("    }}]);"));
            lines.Add(string.Format(""));
            lines.Add(string.Format("}})(window, document);"));


            return lines;
        }

        public void Execute()
        {
            
            var lines = CreateFileContent();
            var filePath = SchemaInfo.GetValidationApiFilePath(PortalDir);
            Utils.WriteToJavascriptFile(filePath, lines, Overwrite, false, false);
        }
    }
}
