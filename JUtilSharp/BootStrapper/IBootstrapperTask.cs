using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JUtilSharp.BootStrapper
{
    /// <summary>
    /// 繼承IBootstrapperTask的類別限制
    /// (1) 類別必須為public
    /// </summary>
    public interface IBootstrapperTask
    {
        int Order { get; } // 設定順序, 由小到大

        void Execute();
    }
}
