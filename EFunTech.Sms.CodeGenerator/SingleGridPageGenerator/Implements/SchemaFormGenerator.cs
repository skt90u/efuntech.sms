
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFunTech.Sms.CodeGenerator
{
    public class SchemaFormGenerator : IJob
    {
        public string PortalDir { get; set; }
        public bool Overwrite { get; set; }
        public SchemaInfo SchemaInfo { get; set; }

        #region WriteSchemaProperty
        private void WriteSchemaProperty(List<string> lines, SchemaPropertyInfo property)
        {
            string key = property.SchemaForm_SchemaKey;
            string type = property.SchemaForm_SchemaType;
            string title = property.DisplayName;

            lines.Add(string.Format("	                {0}: {{", key));
            lines.Add(string.Format("	                    type: \"{0}\",", type));
            lines.Add(string.Format("	                    title: \"{0}\",", title));

            WriteSchemaPropertyEnum(lines, property);
            WriteSchemaPropertyValidation(lines, property);

            lines.Add(string.Format("    				}},"));
        }

        private void WriteSchemaPropertyEnum(List<string> lines, SchemaPropertyInfo property)
        {
            if (!property.IsEnum) return;

            List<string> result = new List<string>();

            foreach (var value in Enum.GetValues(property.PropertyType))
            {
                result.Add(((int)value).ToString());
            }

            string enumString = string.Join(",", result.Select(p => string.Format("'{0}'", p)));

            if (!string.IsNullOrEmpty(enumString))
            {
                lines.Add(string.Format("                        enum: [{0}],", enumString));
            }
        }

        private void WriteSchemaPropertyValidation(List<string> lines, SchemaPropertyInfo property)
        {
            RequiredAttribute RequiredAttribute = property.GetCustomAttribute<RequiredAttribute>();
            RegularExpressionAttribute RegularExpressionAttribute = property.GetCustomAttribute<RegularExpressionAttribute>();
            MaxLengthAttribute MaxLengthAttribute = property.GetCustomAttribute<MaxLengthAttribute>();
            MinLengthAttribute MinLengthAttribute = property.GetCustomAttribute<MinLengthAttribute>();

            if (RequiredAttribute != null)
            {
                lines.Add(string.Format("                        required: true,"));
            }
            if (RegularExpressionAttribute != null)
            {
                lines.Add(string.Format("                        pattern: \"{0}\",", RegularExpressionAttribute.Pattern));
            }
            if (MaxLengthAttribute != null)
            {
                lines.Add(string.Format("                        maxLength: \"{0}\",", MaxLengthAttribute.Length));
            }
            if (MinLengthAttribute != null)
            {
                lines.Add(string.Format("                        minLength: \"{0}\",", MinLengthAttribute.Length));
            }

            List<ValidationAttribute> ValidationAttributes = new List<ValidationAttribute>();
            if (RequiredAttribute != null) ValidationAttributes.Add(RequiredAttribute);
            if (RegularExpressionAttribute != null) ValidationAttributes.Add(RegularExpressionAttribute);
            if (MaxLengthAttribute != null) ValidationAttributes.Add(MaxLengthAttribute);
            if (MinLengthAttribute != null) ValidationAttributes.Add(MinLengthAttribute);

            if (ValidationAttributes.Any(p => !string.IsNullOrEmpty(p.ErrorMessage)))
            {
                lines.Add(string.Format("                        validationMessage: {{"));

                if (RequiredAttribute != null)
                {
                    // 302: 'Required',
                    if (!string.IsNullOrEmpty(RequiredAttribute.ErrorMessage))
                        lines.Add(string.Format("                            302: '{0}',", RequiredAttribute.ErrorMessage));
                }
                if (RegularExpressionAttribute != null)
                {
                    // 202: 'String does not match pattern: {{schema.pattern}}',
                    if (!string.IsNullOrEmpty(RegularExpressionAttribute.ErrorMessage))
                        lines.Add(string.Format("                            202: '{0}',", RegularExpressionAttribute.ErrorMessage));
                }

                if (MaxLengthAttribute != null)
                {
                    // 201: 'String is too long ({{viewValue.length}} chars), maximum {{schema.maxLength}}',
                    if (!string.IsNullOrEmpty(MaxLengthAttribute.ErrorMessage))
                        lines.Add(string.Format("                            201: '{0}',", MaxLengthAttribute.ErrorMessage));
                }
                if (MinLengthAttribute != null)
                {
                    // 200: 'String is too short ({{viewValue.length}} chars), minimum {{schema.minLength}}',
                    if (!string.IsNullOrEmpty(MinLengthAttribute.ErrorMessage))
                        lines.Add(string.Format("                            200: '{0}',", MinLengthAttribute.ErrorMessage));
                }
                lines.Add(string.Format("                        }},")); // validationMessage
            }
        }

        #endregion

        #region WriteFormProperties

        private void WriteFormProperties(List<string> lines, List<SchemaPropertyInfo> properties)
        {
            int columnWidth = SchemaInfo.ColumnWidth;

            int rowCount = (int)Math.Ceiling((decimal)properties.Count / (decimal)columnWidth);

            for (int i = 0; i < rowCount; i++)
            {
                lines.Add(string.Format("                {{"));
                lines.Add(string.Format("                    type: \"section\","));
                lines.Add(string.Format("                    htmlClass: \"row\","));
                lines.Add(string.Format("                    items: ["));


                for (int j = 0; j < columnWidth; j++)
                {
                    int index = i * columnWidth + j;

                    if (index >= properties.Count) break;

                    SchemaPropertyInfo property = properties[index];

                    WriteFormProperty(lines, property);
                }

                lines.Add(string.Format("                    ]"));
                lines.Add(string.Format("                }},"));
            }
        }

        private void WriteFormPropertyFeedback(List<string> lines, SchemaPropertyInfo property)
        {
            RequiredAttribute RequiredAttribute = property.GetCustomAttribute<RequiredAttribute>();
            if (RequiredAttribute != null)
            {
                lines.Add(string.Format("                                    feedback: \"{{'glyphicon': true, 'glyphicon-ok': hasSuccess(), 'glyphicon-star': !hasSuccess() }}\", "));
            }
            else
            {
                lines.Add(string.Format("                                    feedback: \"{{'glyphicon': true, 'glyphicon-ok': hasSuccess() }}\", "));
            }

            lines.Add(string.Format("                                    htmlClass: 'has-feedback',"));
        }

        private void WriteFormPropertyTitleMap(List<string> lines, SchemaPropertyInfo property)
        {
            if (!property.IsEnum) return;

            lines.Add(string.Format("                                      titleMap: ["));
            
            foreach (var enumVal in Enum.GetValues(property.PropertyType))
            {
                lines.Add(string.Format("                                          {{ value: '{0}', name: '{1}' }},", (int)enumVal, enumVal.ToString()));
            }

            lines.Add(string.Format("                                      ],"));
        }

        private void WriteFormProperty(List<string> lines, SchemaPropertyInfo property)
        {
            int columnWidth = SchemaInfo.ColumnWidth;

            string htmlClass = string.Format("col-xs-{0}", columnWidth);

            string key = property.SchemaForm_FormKey;
            string type = property.SchemaForm_FormType;
            string title = property.DisplayName;

            lines.Add(string.Format("                        {{"));
            lines.Add(string.Format("                            type: \"section\","));
            lines.Add(string.Format("                            htmlClass: \"{0}\",", htmlClass));
            lines.Add(string.Format("                            items: ["));
            lines.Add(string.Format("                                {{"));
            lines.Add(string.Format("                                    key: \"{0}\",", key));
            lines.Add(string.Format("                                    type: \"{0}\",", type));

            WriteFormPropertyFeedback(lines, property);
            WriteFormPropertyTitleMap(lines, property);

            lines.Add(string.Format("                                }},"));
            lines.Add(string.Format("                            ]"));
            lines.Add(string.Format("                        }},"));
        }
        #endregion

        private List<string> CreateFileContent()
        {
            var lines = new List<string>();

            var modelName = SchemaInfo.ModelName;
            var properties = SchemaInfo.SchemaFormProperties;

            lines.Add(string.Format("(function (window, document) {{"));
            lines.Add(string.Format("    'use strict';"));
            lines.Add(string.Format(""));
            lines.Add(string.Format("    angular.module('app').run(['SchemaFormCache', 'GlobalSettings', function (SchemaFormCache, GlobalSettings) {{"));
            lines.Add(string.Format(""));
            lines.Add(string.Format("        SchemaFormCache.put('{0}', function (options) {{", modelName));
            lines.Add(string.Format(""));
            lines.Add(string.Format("            var schema = {{"));
            lines.Add(string.Format("                type: \"object\","));
            lines.Add(string.Format("                properties: {{"));

            foreach (var property in properties)
                WriteSchemaProperty(lines, property);

            lines.Add(string.Format("	            }}"));
            lines.Add(string.Format("	        }};"));
            lines.Add(string.Format(""));
            lines.Add(string.Format("            var form = ["));

            WriteFormProperties(lines, properties);

            lines.Add(string.Format("            ];"));
            lines.Add(string.Format(""));
            lines.Add(string.Format("            return {{ schema: schema, form: form, }};"));
            lines.Add(string.Format(""));
            lines.Add(string.Format("        }}); // $cacheFactory('SchemaFormFactory').put('{0}', function (options) {{", modelName));
            lines.Add(string.Format(""));
            lines.Add(string.Format("    }}]); //  angular.module('app').run(['$cacheFactory', function ($cacheFactory) {{"));
            lines.Add(string.Format(""));
            lines.Add(string.Format("}})(window, document);"));
            lines.Add(string.Format(""));

            return lines;
        }

        public void Execute()
        {
            var lines = CreateFileContent();
            var filePath = SchemaInfo.GetSchemaFormFilePath(PortalDir);
            Utils.WriteToJavascriptFile(filePath, lines, Overwrite);
        }
    }
}
