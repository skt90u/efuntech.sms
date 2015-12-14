using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JUtilSharp.BootStrapper
{
    /// <summary>
    /// 繼承IConfigureContainer的類別限制
    /// (1) 建構只不能包含參數
    /// (2) 類別必須為public
    /// </summary>
    public interface IConfigureContainer
    {
        int Order { get; } // 設定順序, 由小到大

        void Execute(IUnityContainer container);
    }
}
