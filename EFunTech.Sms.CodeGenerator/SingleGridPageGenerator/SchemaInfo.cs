using EFunTech.Sms.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace EFunTech.Sms.CodeGenerator
{
    public class SchemaInfo
    {
        public Type SchemaType { get; set; }

        public string SchemaName
        {
            get
            {
                return SchemaType.Name;
            }
        }

        public int _ColumnWidth;
        /// <summary>
        /// 如果沒有設定就用預設值(4)
        /// </summary>
        public int ColumnWidth
        {
            get
            {
                return (_ColumnWidth != default(int)) ? _ColumnWidth : 4;
            }
            set
            {
                _ColumnWidth = value;
            }
        }

        public string _JsControllerName;
        /// <summary>
        /// 如果沒有設定就用 SchemaName
        /// </summary>
        public string JsControllerName
        {
            get
            {
                return (_JsControllerName != default(string)) ? _JsControllerName : SchemaName;
            }
            set
            {
                _JsControllerName = value;
            }
        }

        public string ModelName
        {
            get
            {
                return string.Format("{0}Model", JsControllerName);
            }
        }

        public string ProfileName
        {
            get
            {
                return string.Format("{0}Profile", JsControllerName);
            }
        }

        public string ApiControllerName
        {
            get
            {
                return string.Format("{0}Controller", JsControllerName);
            }
        }

        public string ApiUrl
        {
            get
            {
                return string.Format("/api/{0}", ApiControllerName.Replace("Controller", string.Empty));
            }
        }

        public string GetModelFilePath(string PortalDir)
        {
            return Path.Combine(PortalDir, "GenerateResult", @"Models", ModelName + ".cs");
        }

        public string GetProfileFilePath(string PortalDir)
        {
            return Path.Combine(PortalDir, "GenerateResult", @"Models\Mapper", ProfileName + ".cs");
        }

        public string GetApiControllerFilePath(string PortalDir)
        {
            return Path.Combine(PortalDir, "GenerateResult", @"Controllers", ApiControllerName + ".cs");
        }

        public string GetSchemaFormFilePath(string PortalDir)
        {
            return Path.Combine(PortalDir, "GenerateResult", @"Scripts\Application\Runs\SchemaForm", ModelName + ".js");
        }

        public string GetJsControllerFilePath(string PortalDir)
        {
            return Path.Combine(PortalDir, "GenerateResult", @"Scripts\Application\Controllers", JsControllerName + ".js");
        }

        private List<SchemaPropertyInfo> _Properties;
        public List<SchemaPropertyInfo> Properties
        {
            get
            {
                if(_Properties == null)
                {
                    var properties = SchemaType.GetProperties(BindingFlags.Instance | BindingFlags.Public);

                    _Properties = properties.Select(p => new SchemaPropertyInfo(p)).ToList();
                }
                return _Properties;
            }
        }

        public List<SchemaPropertyInfo> SearchableProperties
        {
            get
            {
                return Properties.Where(p => p.PropertyType == typeof(string)).ToList();
            }
        }

        public string IdTypeString
        {
            get
            {
                foreach (var property in Properties)
                {
                    if (property.Name == "Id")
                        return property.PropertyTypeString;
                }

                return "int";
            }
        }

        public List<SchemaPropertyInfo> SchemaFormProperties
        {
            get
            {
                return Properties.Where(p => p.Name != "Id" && p.IsModelProperty).ToList();
            }
        }

        public static string GetValidationApiFilePath(string PortalDir)
        {
            return Path.Combine(PortalDir, @"Scripts\Application\Factories\ValidationApi.js");
        }

        public static string GetEnumFilePath(string PortalDir)
        {
            return Path.Combine(PortalDir, @"Scripts\Application\Constants\Enum.js");
        }

        public static string GetLookupApiFilePath(string PortalDir)
        {
            return Path.Combine(PortalDir, @"Scripts\Application\Factories\LookupApi.js");
        }

    }
}
