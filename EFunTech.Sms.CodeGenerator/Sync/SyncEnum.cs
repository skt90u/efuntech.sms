
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EFunTech.Sms.CodeGenerator
{
    public class SyncEnum : IJob
    {
        public string PortalDir { get; set; }
        public bool Overwrite { get; set; }

        private List<string> CreateFileContent()
        {
            List<string> lines = new List<string>();

            lines.Add(string.Format("(function (window, document) {{"));
            lines.Add(string.Format("    'use strict';"));
            lines.Add("");
            lines.Add(string.Format("	////////////////////////////////////////"));
            lines.Add(string.Format("	// 這個檔案是經由 EFunTech.Sms.CodeGenerator.SyncEnum 自動產生，請勿手動修改"));
            lines.Add(string.Format("	//"));
            lines.Add(string.Format("	// 最後產製時間： {0}", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")));
            lines.Add(string.Format("	////////////////////////////////////////"));
            lines.Add("");

            var assembly = Assembly.GetAssembly(typeof(EFunTech.Sms.Schema.Gender));
            var types = assembly.GetTypes().Where(p => p.IsEnum).ToList();

            foreach (var enumType in types)
                CreateEnumDefine(lines, enumType);

            CreateEnumMapping(lines, types);

            lines.Add(string.Format("}})(window, document);"));

            return lines;
        }

        private void CreateEnumMapping(List<string> lines, List<Type> enumTypes)
        {
            lines.Add(string.Format("    /****************************************"));
            lines.Add(string.Format("     * 所有 ENUM 定義，用於 EnumFilter"));
            lines.Add(string.Format("     ****************************************/"));

            lines.Add(string.Format("    var EnumMapping = {{"));

            foreach (var enumType in enumTypes)
            {
                var key = string.Format("'{0}'", enumType.Name);
                var val = enumType.Name;
                var comment = AttributeHelper.GetTableDescription(enumType);

                lines.Add(string.Format("        {0}: {1}, // {2}", key, val, comment));
            }

            lines.Add(string.Format("    }};"));
            lines.Add(string.Format("    angular.module('app').constant('EnumMapping', EnumMapping);"));
            lines.Add("");
        }

        private void CreateEnumDefine(List<string> lines, Type enumType)
        {
            lines.Add(string.Format("    /**"));
            lines.Add(string.Format("     * {0}", AttributeHelper.GetTableDescription(enumType)));
            lines.Add(string.Format("     */"));

            lines.Add(string.Format("    var {0} = {{", enumType.Name));

            //var enumValues = Enum.GetValues(enumType).Cast<Enum>().OrderBy(p => p);
            var enumValues = Enum.GetValues(enumType).Cast<Enum>();

            foreach (var enumValue in enumValues)
            {
                var key = enumValue.ToString();
                var value = Convert.ToInt32(enumValue);
                var text = AttributeHelper.GetColumnDescription(enumValue as Enum);

                if (string.IsNullOrEmpty(text))
                    text = key;

                lines.Add(string.Format("        {0}: {{value: {1},  text: '{2}'}},", key, value, text));
            }

            lines.Add(string.Format("    }};"));
            lines.Add(string.Format("    angular.module('app').constant('{0}', {0});", enumType.Name));
            lines.Add("");
        }

        public void Execute()
        {
            var lines = CreateFileContent();
            var filePath = SchemaInfo.GetEnumFilePath(PortalDir);
            Utils.WriteToJavascriptFile(filePath, lines, Overwrite, false, false);
        }
    }
}
