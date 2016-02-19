using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace System.ComponentModel
{
    public static class AttributeHelper
    {
        public static T GetAttribute<T>(Type type) where T : Attribute
        {
            var attribute = (T)type.GetCustomAttributes(typeof(T), false).FirstOrDefault();

            return attribute;
        }

        public static T GetAttribute<T>(Enum enumValue) where T : Attribute
        {
            T attribute = null;

            MemberInfo memberInfo = enumValue.GetType().GetMember(enumValue.ToString()).FirstOrDefault();

            if (memberInfo != null)
            {
                attribute = (T)memberInfo.GetCustomAttributes(typeof(T), false).FirstOrDefault();
            }

            return attribute;
        }

        public static string GetColumnDescription(Enum enumValue)
        {
            var result = AttributeHelper.GetAttribute<ColumnDescriptionAttribute>(enumValue);

            return result != null ? result.Description : string.Empty;
        }

        public static string GetTableDescription(Type enumType)
        {
            var result = AttributeHelper.GetAttribute<TableDescriptionAttribute>(enumType);

            return result != null ? result.Description : string.Empty;
        }        
    }
}
