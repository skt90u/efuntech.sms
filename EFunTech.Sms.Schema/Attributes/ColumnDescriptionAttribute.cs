using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ComponentModel
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field /* Enum Field */)]
    public class ColumnDescriptionAttribute : Attribute
    {
        static ColumnDescriptionAttribute()
        {
            Default = new ColumnDescriptionAttribute();
        }

        // 摘要: 
        //     指定 System.ComponentModel.DescriptionAttribute 的預設值，也就是空字串 ("")。 這個 static
        //     欄位是唯讀的。
        public static readonly ColumnDescriptionAttribute Default;

        // 摘要: 
        //     使用無參數的方式，初始化 System.ComponentModel.DescriptionAttribute 類別的新執行個體。
        public ColumnDescriptionAttribute()
        {
            this.Description = string.Empty;
        }

        //
        // 摘要: 
        //     使用描述來初始化 System.ComponentModel.DescriptionAttribute 類別的新執行個體。
        //
        // 參數: 
        //   description:
        //     描述文字。
        public ColumnDescriptionAttribute(string description)
        {
            this.Description = description;
        }

        // 摘要: 
        //     取得儲存在這個屬性中的描述。
        //
        // 傳回: 
        //     儲存在這個屬性中的描述。
        public virtual string Description { get; private set; }
    }
}
