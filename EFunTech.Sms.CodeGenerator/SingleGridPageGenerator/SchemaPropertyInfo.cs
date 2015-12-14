using EFunTech.Sms.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EFunTech.Sms.CodeGenerator
{
    public class SchemaPropertyInfo
    {
        private PropertyInfo property;

        public SchemaPropertyInfo(PropertyInfo property)
        {
            this.property = property;
        }

        public bool IsModelProperty
        {
            get
            {
                // 當發生 StackOverflowException ， 請在此試試看，查看對應類別中屬型的型別新增到這個函式中

                if (typeof(ApplicationUser) == property.PropertyType) return false;
                if (typeof(SendMessageRule) == property.PropertyType) return false;
                if (typeof(UploadedFile) == property.PropertyType) return false;
                //if (typeof(Infobip_SendSMSResult) == property.PropertyType) return false;
                
                if (property.PropertyType.FullName.Contains("ICollection")) return false;
                return true;

                // http://stackoverflow.com/questions/12305945/how-to-check-if-a-property-is-virtual-with-reflection
                //return !(property.GetAccessors()[0].IsVirtual);
            }
        }

        public bool IsSchemaType
        {
            get
            {
                Type propertyType = property.PropertyType;
                return propertyType.FullName.Contains("EFunTech.Sms.Schema") && propertyType.IsClass;
            }
        }

        public string Name
        {
            get
            {
                return property.Name;
            }
        }

        public string DisplayName
        {
            get
            {
                var attrs = property.GetCustomAttributes(typeof(ColumnDescriptionAttribute), true);

                if (attrs.Length != 0)
                    return (attrs[0] as ColumnDescriptionAttribute).Description;
                else
                    return Name;
            }
        }

        public T GetCustomAttribute<T>()
        {
            var attrs = property.GetCustomAttributes(typeof(T), true);

            if (attrs.Length != 0)
                return (T)(attrs[0]);
            else
                return default(T);
        }

        
        
        private string GetMappingPropertyTypeString()
        {
            Type propertyType = property.PropertyType;
            if (typeof(int) == propertyType) return "int";
            if (typeof(string) == propertyType) return "string";
            if (typeof(bool) == propertyType) return "bool";
            if (typeof(DateTime?) == propertyType) return "DateTime?";
            return null;
        }

        public string PropertyTypeString
        {
            get
            {
                Type propertyType = property.PropertyType;

                if (propertyType.IsEnum) return propertyType.Name;

                string result = GetMappingPropertyTypeString();
                if (result != null) return result;

                if (IsSchemaType) return (new SchemaInfo { SchemaType = propertyType }).ModelName;
                
                return propertyType.Name;
            }
        }

        public bool IsEnum
        {
            get
            {
                Type propertyType = property.PropertyType;
                return propertyType.IsEnum;
            }
        }

        public Type PropertyType
        {
            get
            {
                return property.PropertyType;
            }
        }

        public static List<SchemaPropertyInfo> GetProperties(Type schemaType, BindingFlags bindingAttr)
        {
            var properties = schemaType.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            return properties.Select(p => new SchemaPropertyInfo(p)).ToList();
        }

        public static List<SchemaPropertyInfo> GetProperties(Type schemaType)
        {
            return GetProperties(schemaType, BindingFlags.Instance | BindingFlags.Public);
        }

        public string SchemaForm_SchemaKey
        {
            get
            {
                return Name;
            }
        }

        public string SchemaForm_FormKey
        {
            get
            {
                return Name;
            }
        }

        public string SchemaForm_SchemaType
        {
            get
            {
                if (typeof(string) == property.PropertyType) return "string";
                if (typeof(int) == property.PropertyType) return "integer";
                if (typeof(float) == property.PropertyType) return "number";
                if (typeof(double) == property.PropertyType) return "number";
                if (typeof(bool) == property.PropertyType) return "boolean";
                if (property.PropertyType.IsArray) return "array";
                if (property.PropertyType.FullName.Contains("System.Collections.Generic.List")) return "array";
                if (property.PropertyType.IsClass) return "object";

                return "string";
            }
        }

        public string SchemaForm_FormType
        {
            get
            {
                switch (SchemaForm_SchemaType)
                {
                    case "string":
                        {
                            return IsEnum ? "radios-inline" : "text";
                        }
                    case "number": return "number";
                    case "integer": return "number";
                    case "boolean": return "checkbox";
                    case "object": return "fieldset";
                    case "array":
                        {
                            return IsEnum ? "checkboxes" : "fieldset";
                        }
                }

                return "text";
            }
        }
        
    }
}
