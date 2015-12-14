using AutoPoco.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFunTech.Sms.Simulation.DataSources
{
    public class DescriptionDataSource : DatasourceBase<string>
    {
        private readonly string[] data = new string[] { 
            "本屆選秀會共93人參加，打破2010年含代訓88人的紀錄，大約半數被選中，而26位應屆高中畢業生參與，有14名入選也是史上最多，猿隊總教練洪一中認為，這代表中職蓬勃發展",
            "看林書豪投三分球沒什麼了不起，投四分球就稀奇了！林書豪昨晚出席決戰夏日籃球夜，除了跟歌手蛋堡一起唱中文RAP，更親自下場玩4對4示範怎麼投進四分球，甚至表演各式各樣扣籃，讓全場3百多名球迷驚呼連連。",
            "除了首輪選進有望成為即戰力的王柏融，桃猿2、3輪分別挑進林承飛和葉文淇2名高中畢業生，而合計選進的11名球員中，有7人畢業於位處桃園的平鎮高中，其中包括6名高中生；而第6、8輪則選中具旅外經歷的王躍霖和陳敏賜。投手共計7人、內野手2人、捕手1人、外野手1人。",
            "猿隊總教練洪一中表示：「陽耀勳的控球有點失憶了。」這應該是以投手身分報名的陽耀勳不獲青睞的原因，其實陽耀勳在國訓隊是「二刀流」，投、打皆行！王光輝表示：「讓他打擊是希望他忘掉投球的種種不快。」",
            "（中央社記者李宇政台北29日電）大滿貫等級的草地賽事溫網今天點燃戰火，台灣女將詹皓晴今天在臉書上表示，「溫布頓終身會員證到手啦！」她去年在混雙殺進決賽，今年又是新的開始。",
            "0歲的鄧去年和熱火簽訂下2年2000萬美元的合約，今年擁有球員選項，鄧在經過長時間考慮過後，決定留守熱火。鄧的經紀人魯杜（Herb Rudoy）透露，當地時間週一下午2點，鄧告知熱火他執行選項的決定。",
            "金鳥大軍在第二局的攻勢只是開胃菜，村田真正的震撼教育出現在四局下半，村田的卡特球投不到位，就會被掃出牆外，換成慢速曲球來搭配，還是被無情的轟出去，這位30歲才登上大聯盟的日籍老新人村田透，大聯盟的處女秀只投了3.1局就失掉5分，其中3分是他的自責分表現並不理想。",
            "中職選秀，今年號稱史上最大，共有93為新秀參與，創史上最多紀錄，其中更不乏多位旅外選手與業餘大物球員。Yahoo奇摩運動民調就想問問各位網友，下列這八位參與選秀的球員，誰最有機會當選狀元呢？",
            "倉本昌弘在日巡賽的戰績傲人，無奈巔峰期剛好碰到兩大日本傳奇名將中嶋常幸（Tsuneyuki Nakajima）和尾崎將司（Masashi Ozaki），像1988年單季贏得五勝，結果獎金榜還得排在尾崎兄弟和石井大衛之後，生涯最大憾事便是不曾登上「日本球王」寶座。"
        };

        public override string Next(IGenerationContext context)
        {
            var builder = new StringBuilder();
            int index = RandomNumberGenerator.Current.Next(data.Length);
            return data[index];
        }
    }
}
