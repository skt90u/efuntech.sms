using AutoPoco.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFunTech.Sms.Simulation.DataSources
{
    public class SubjectDataSource : DatasourceBase<string>
    {
        private readonly string[] data = { 
            "中職大物年 林哲瑄",
            "唱rap秀扣籃 豪粉大驚喜",
            "二刀流沒行情 陽耀勳落榜",
            "金鶯4連勝 衝上美東第1",
            "豪場下唱rap 場上秀扣籃",
            "好野手難選 桃猿挑走王柏融",
            "好野手難選 桃猿挑走王柏融",
            "陽耀勳未獲青睞 會繼續努力",
            "溫網終身會員證 詹皓晴到手",
            "富邦長春賽 由卜派對決鷹王",
            "將成自由身 豪盼跟狄安東尼",
            "中職季中選秀 陽耀勳成遺珠"
        };

        public override string Next(IGenerationContext context)
        {
            var builder = new StringBuilder();
            int index = RandomNumberGenerator.Current.Next(data.Length);
            return data[index];
        }
    }
}
