using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFunTech.Sms.Schema
{
    [TableDescription("角色")]
    public enum Role
    {
        [ColumnDescription("尚未定義")]
        Unknown = 0,

        [ColumnDescription("員工")]
        Employee = 1,

        [ColumnDescription("部門主管")]
        DepartmentHead = 10,

        [ColumnDescription("督導者")]
        Supervisor = 100,

        [ColumnDescription("系統管理者")]
        Administrator = 1000,
    }
}
