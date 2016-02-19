using PhoneNumbers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace EFunTech.Sms.Portal
{
    public class MyRegionInfo
    {
        // ISO 3166 兩個字母國家/地區代碼。
        public string Name { get; set; }

        // 取得國家/地區的完整英文名稱。
        public string EnglishName { get; set; }

        // 取得國家/地區的完整名稱。
        public string DisplayName { get; set; }
        
    }

    public static class MobileUtil
    {
        private static List<MyRegionInfo> regionInfos = null;
        public static List<MyRegionInfo> GetRegionInfos()
        {
            if (regionInfos == null)
            {
                bool useCultureInfo = false;

                if (useCultureInfo)
                {
                    const string defaultIso3166 = "TW";

                    RegionInfo defaultCountry = null;

                    var countries = new List<RegionInfo>();
                    foreach (CultureInfo culture in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
                    {
                        var country = new RegionInfo(culture.LCID);
                        if (countries.Where(p => p.Name == country.Name).Count() == 0)
                        {
                            if (country.Name == defaultIso3166)
                            {
                                defaultCountry = country;
                            }
                            else
                            {
                                countries.Add(country);
                            }
                        }
                    }

                    if (defaultCountry != null) countries.Insert(0, defaultCountry);

                    regionInfos = countries.Select(p => new MyRegionInfo
                    {
                        Name = p.Name,
                        EnglishName = p.EnglishName,
                        DisplayName = p.DisplayName,
                    }).ToList();
                }
                else
                {
                    regionInfos = new List<MyRegionInfo> { 
                        new MyRegionInfo { Name = "TW", EnglishName = "Taiwan", DisplayName = "台灣" },
                        new MyRegionInfo { Name = "HK", EnglishName = "Hong Kong", DisplayName = "香港" },
                        new MyRegionInfo { Name = "CN", EnglishName = "China", DisplayName = "中國" },
                        new MyRegionInfo { Name = "AF", EnglishName = "Afghanistan", DisplayName = "阿富汗" },
                        new MyRegionInfo { Name = "AL", EnglishName = "Albania", DisplayName = "阿爾巴尼亞" },
                        new MyRegionInfo { Name = "DZ", EnglishName = "Algeria", DisplayName = "阿爾及利亞" },
                        new MyRegionInfo { Name = "AS", EnglishName = "American Samoa", DisplayName = "美屬薩摩亞" },
                        new MyRegionInfo { Name = "AD", EnglishName = "Andorra", DisplayName = "安道​​爾" },
                        new MyRegionInfo { Name = "AO", EnglishName = "Angola", DisplayName = "安哥拉" },
                        new MyRegionInfo { Name = "AI", EnglishName = "Anguilla", DisplayName = "安圭拉" },
                        new MyRegionInfo { Name = "AQ", EnglishName = "Antarctica", DisplayName = "南極洲" },
                        new MyRegionInfo { Name = "AG", EnglishName = "Antigua And Barbuda", DisplayName = "安提瓜和巴布達" },
                        new MyRegionInfo { Name = "AR", EnglishName = "Argentina", DisplayName = "阿根廷" },
                        new MyRegionInfo { Name = "AM", EnglishName = "Armenia", DisplayName = "亞美尼亞" },
                        new MyRegionInfo { Name = "AW", EnglishName = "Aruba", DisplayName = "阿魯巴島" },
                        new MyRegionInfo { Name = "AU", EnglishName = "Australia", DisplayName = "澳大利亞" },
                        new MyRegionInfo { Name = "AT", EnglishName = "Austria", DisplayName = "奧地利" },
                        new MyRegionInfo { Name = "AZ", EnglishName = "Azerbaijan", DisplayName = "阿塞拜疆" },
                        new MyRegionInfo { Name = "BS", EnglishName = "Bahamas", DisplayName = "巴哈馬" },
                        new MyRegionInfo { Name = "BH", EnglishName = "Bahrain", DisplayName = "巴林" },
                        new MyRegionInfo { Name = "BD", EnglishName = "Bangladesh", DisplayName = "孟加拉國" },
                        new MyRegionInfo { Name = "BB", EnglishName = "Barbados", DisplayName = "巴巴多斯" },
                        new MyRegionInfo { Name = "BY", EnglishName = "Belarus", DisplayName = "白俄羅斯" },
                        new MyRegionInfo { Name = "BE", EnglishName = "Belgium", DisplayName = "比利時" },
                        new MyRegionInfo { Name = "BZ", EnglishName = "Belize", DisplayName = "伯利茲" },
                        new MyRegionInfo { Name = "BJ", EnglishName = "Benin", DisplayName = "貝寧" },
                        new MyRegionInfo { Name = "BM", EnglishName = "Bermuda", DisplayName = "百慕大" },
                        new MyRegionInfo { Name = "BT", EnglishName = "Bhutan", DisplayName = "不丹" },
                        new MyRegionInfo { Name = "BO", EnglishName = "Bolivia", DisplayName = "玻利維亞" },
                        new MyRegionInfo { Name = "BA", EnglishName = "Bosnia And Herzegovina", DisplayName = "波斯尼亞和黑塞哥維那" },
                        new MyRegionInfo { Name = "BW", EnglishName = "Botswana", DisplayName = "博茨瓦納" },
                        new MyRegionInfo { Name = "BV", EnglishName = "Bouvet Island", DisplayName = "布維島" },
                        new MyRegionInfo { Name = "BR", EnglishName = "Brazil", DisplayName = "巴西" },
                        new MyRegionInfo { Name = "IO", EnglishName = "British Indian Ocean Territory", DisplayName = "英屬印度洋領地" },
                        new MyRegionInfo { Name = "BN", EnglishName = "Brunei", DisplayName = "文萊" },
                        new MyRegionInfo { Name = "BG", EnglishName = "Bulgaria", DisplayName = "保加利亞" },
                        new MyRegionInfo { Name = "BF", EnglishName = "Burkina Faso", DisplayName = "布基納法索" },
                        new MyRegionInfo { Name = "BI", EnglishName = "Burundi", DisplayName = "布隆迪" },
                        new MyRegionInfo { Name = "KH", EnglishName = "Cambodia", DisplayName = "柬埔寨" },
                        new MyRegionInfo { Name = "CM", EnglishName = "Cameroon", DisplayName = "喀麥隆" },
                        new MyRegionInfo { Name = "CA", EnglishName = "Canada", DisplayName = "加拿大" },
                        new MyRegionInfo { Name = "CV", EnglishName = "Cape Verde", DisplayName = "佛得角" },
                        new MyRegionInfo { Name = "KY", EnglishName = "Cayman Islands", DisplayName = "開曼群島" },
                        new MyRegionInfo { Name = "CF", EnglishName = "Central African Republic", DisplayName = "中非共和國" },
                        new MyRegionInfo { Name = "TD", EnglishName = "Chad", DisplayName = "乍得" },
                        new MyRegionInfo { Name = "CL", EnglishName = "Chile", DisplayName = "智利" },
                        new MyRegionInfo { Name = "CN", EnglishName = "China", DisplayName = "中國" },
                        new MyRegionInfo { Name = "CX", EnglishName = "Christmas Island", DisplayName = "聖誕島" },
                        new MyRegionInfo { Name = "CC", EnglishName = "Cocos (Keeling) Islands", DisplayName = "科科斯群島" },
                        new MyRegionInfo { Name = "CO", EnglishName = "Columbia", DisplayName = "哥倫比亞" },
                        new MyRegionInfo { Name = "KM", EnglishName = "Comoros", DisplayName = "科摩羅" },
                        new MyRegionInfo { Name = "CG", EnglishName = "Congo", DisplayName = "剛果" },
                        new MyRegionInfo { Name = "CK", EnglishName = "Cook Islands", DisplayName = "庫克群島" },
                        new MyRegionInfo { Name = "CR", EnglishName = "Costa Rica", DisplayName = "哥斯達黎加" },
                        new MyRegionInfo { Name = "CI", EnglishName = "Cote D'Ivorie (Ivory Coast)", DisplayName = "科特迪瓦Ivorie（象牙海岸）" },
                        new MyRegionInfo { Name = "HR", EnglishName = "Croatia (Hrvatska)", DisplayName = "克羅地亞（赫爾瓦次卡）" },
                        new MyRegionInfo { Name = "CU", EnglishName = "Cuba", DisplayName = "古巴" },
                        new MyRegionInfo { Name = "CY", EnglishName = "Cyprus", DisplayName = "塞浦路斯" },
                        new MyRegionInfo { Name = "CZ", EnglishName = "Czech Republic", DisplayName = "捷克" },
                        new MyRegionInfo { Name = "CD", EnglishName = "Democratic Republic Of Congo (Zaire)", DisplayName = "剛果民主共和國（扎伊爾）" },
                        new MyRegionInfo { Name = "DK", EnglishName = "Denmark", DisplayName = "丹麥" },
                        new MyRegionInfo { Name = "DJ", EnglishName = "Djibouti", DisplayName = "吉布提" },
                        new MyRegionInfo { Name = "DM", EnglishName = "Dominica", DisplayName = "多米尼加" },
                        new MyRegionInfo { Name = "DO", EnglishName = "Dominican Republic", DisplayName = "多明尼加共和國" },
                        new MyRegionInfo { Name = "TP", EnglishName = "East Timor", DisplayName = "東帝汶" },
                        new MyRegionInfo { Name = "EC", EnglishName = "Ecuador", DisplayName = "厄瓜多爾" },
                        new MyRegionInfo { Name = "EG", EnglishName = "Egypt", DisplayName = "埃及" },
                        new MyRegionInfo { Name = "SV", EnglishName = "El Salvador", DisplayName = "薩爾瓦多" },
                        new MyRegionInfo { Name = "GQ", EnglishName = "Equatorial Guinea", DisplayName = "赤道幾內亞" },
                        new MyRegionInfo { Name = "ER", EnglishName = "Eritrea", DisplayName = "厄立特里亞" },
                        new MyRegionInfo { Name = "EE", EnglishName = "Estonia", DisplayName = "愛沙尼亞" },
                        new MyRegionInfo { Name = "ET", EnglishName = "Ethiopia", DisplayName = "埃塞俄比亞" },
                        new MyRegionInfo { Name = "FK", EnglishName = "Falkland Islands (Malvinas)", DisplayName = "福克蘭群島（馬爾維納斯群島）" },
                        new MyRegionInfo { Name = "FO", EnglishName = "Faroe Islands", DisplayName = "法羅群島" },
                        new MyRegionInfo { Name = "FJ", EnglishName = "Fiji", DisplayName = "斐" },
                        new MyRegionInfo { Name = "FI", EnglishName = "Finland", DisplayName = "芬蘭" },
                        new MyRegionInfo { Name = "FR", EnglishName = "France", DisplayName = "法國" },
                        new MyRegionInfo { Name = "FX", EnglishName = "France, Metropolitan", DisplayName = "法國本土" },
                        new MyRegionInfo { Name = "GF", EnglishName = "French Guinea", DisplayName = "法國幾內亞" },
                        new MyRegionInfo { Name = "PF", EnglishName = "French Polynesia", DisplayName = "法屬波利尼西亞" },
                        new MyRegionInfo { Name = "TF", EnglishName = "French Southern Territories", DisplayName = "法國南部領土" },
                        new MyRegionInfo { Name = "GA", EnglishName = "Gabon", DisplayName = "加蓬" },
                        new MyRegionInfo { Name = "GM", EnglishName = "Gambia", DisplayName = "岡比亞" },
                        new MyRegionInfo { Name = "GE", EnglishName = "Georgia", DisplayName = "格魯吉亞" },
                        new MyRegionInfo { Name = "DE", EnglishName = "Germany", DisplayName = "德國" },
                        new MyRegionInfo { Name = "GH", EnglishName = "Ghana", DisplayName = "加納" },
                        new MyRegionInfo { Name = "GI", EnglishName = "Gibraltar", DisplayName = "直布羅陀" },
                        new MyRegionInfo { Name = "GR", EnglishName = "Greece", DisplayName = "希臘" },
                        new MyRegionInfo { Name = "GL", EnglishName = "Greenland", DisplayName = "格陵蘭" },
                        new MyRegionInfo { Name = "GD", EnglishName = "Grenada", DisplayName = "格林納達" },
                        new MyRegionInfo { Name = "GP", EnglishName = "Guadeloupe", DisplayName = "瓜德羅普島" },
                        new MyRegionInfo { Name = "GU", EnglishName = "Guam", DisplayName = "關島" },
                        new MyRegionInfo { Name = "GT", EnglishName = "Guatemala", DisplayName = "危地馬拉" },
                        new MyRegionInfo { Name = "GN", EnglishName = "Guinea", DisplayName = "幾內亞" },
                        new MyRegionInfo { Name = "GW", EnglishName = "Guinea-Bissau", DisplayName = "幾內亞比紹" },
                        new MyRegionInfo { Name = "GY", EnglishName = "Guyana", DisplayName = "圭亞那" },
                        new MyRegionInfo { Name = "HT", EnglishName = "Haiti", DisplayName = "海地" },
                        new MyRegionInfo { Name = "HM", EnglishName = "Heard And McDonald Islands", DisplayName = "赫德和麥克唐納群島" },
                        new MyRegionInfo { Name = "HN", EnglishName = "Honduras", DisplayName = "洪都拉斯" },
                        new MyRegionInfo { Name = "HK", EnglishName = "Hong Kong", DisplayName = "香港" },
                        new MyRegionInfo { Name = "HU", EnglishName = "Hungary", DisplayName = "匈牙利" },
                        new MyRegionInfo { Name = "IS", EnglishName = "Iceland", DisplayName = "冰島" },
                        new MyRegionInfo { Name = "IN", EnglishName = "India", DisplayName = "印度" },
                        new MyRegionInfo { Name = "ID", EnglishName = "Indonesia", DisplayName = "印尼" },
                        new MyRegionInfo { Name = "IR", EnglishName = "Iran", DisplayName = "伊朗" },
                        new MyRegionInfo { Name = "IQ", EnglishName = "Iraq", DisplayName = "伊拉克" },
                        new MyRegionInfo { Name = "IE", EnglishName = "Ireland", DisplayName = "愛爾蘭" },
                        new MyRegionInfo { Name = "IM", EnglishName = "Isle of Man", DisplayName = "馬恩島" },
                        new MyRegionInfo { Name = "IL", EnglishName = "Israel", DisplayName = "以色列" },
                        new MyRegionInfo { Name = "IT", EnglishName = "Italy", DisplayName = "意大利" },
                        new MyRegionInfo { Name = "JM", EnglishName = "Jamaica", DisplayName = "牙買加" },
                        new MyRegionInfo { Name = "JP", EnglishName = "Japan", DisplayName = "日本" },
                        new MyRegionInfo { Name = "JO", EnglishName = "Jordan", DisplayName = "約旦" },
                        new MyRegionInfo { Name = "KZ", EnglishName = "Kazakhstan", DisplayName = "哈薩克斯坦" },
                        new MyRegionInfo { Name = "KE", EnglishName = "Kenya", DisplayName = "肯尼亞" },
                        new MyRegionInfo { Name = "KI", EnglishName = "Kiribati", DisplayName = "基里巴斯" },
                        new MyRegionInfo { Name = "KW", EnglishName = "Kuwait", DisplayName = "科威特" },
                        new MyRegionInfo { Name = "KG", EnglishName = "Kyrgyzstan", DisplayName = "吉" },
                        new MyRegionInfo { Name = "LA", EnglishName = "Laos", DisplayName = "老撾" },
                        new MyRegionInfo { Name = "LV", EnglishName = "Latvia", DisplayName = "拉脫維亞" },
                        new MyRegionInfo { Name = "LB", EnglishName = "Lebanon", DisplayName = "黎巴嫩" },
                        new MyRegionInfo { Name = "LS", EnglishName = "Lesotho", DisplayName = "萊索托" },
                        new MyRegionInfo { Name = "LR", EnglishName = "Liberia", DisplayName = "利比里亞" },
                        new MyRegionInfo { Name = "LY", EnglishName = "Libya", DisplayName = "利比亞" },
                        new MyRegionInfo { Name = "LI", EnglishName = "Liechtenstein", DisplayName = "列支敦士登" },
                        new MyRegionInfo { Name = "LT", EnglishName = "Lithuania", DisplayName = "立陶宛" },
                        new MyRegionInfo { Name = "LU", EnglishName = "Luxembourg", DisplayName = "盧森堡" },
                        new MyRegionInfo { Name = "MO", EnglishName = "Macau", DisplayName = "澳門" },
                        new MyRegionInfo { Name = "MK", EnglishName = "Macedonia", DisplayName = "馬其頓" },
                        new MyRegionInfo { Name = "MG", EnglishName = "Madagascar", DisplayName = "馬達加斯加" },
                        new MyRegionInfo { Name = "MW", EnglishName = "Malawi", DisplayName = "馬拉維" },
                        new MyRegionInfo { Name = "MY", EnglishName = "Malaysia", DisplayName = "馬來西亞" },
                        new MyRegionInfo { Name = "MV", EnglishName = "Maldives", DisplayName = "馬爾代夫" },
                        new MyRegionInfo { Name = "ML", EnglishName = "Mali", DisplayName = "馬里" },
                        new MyRegionInfo { Name = "MT", EnglishName = "Malta", DisplayName = "馬耳他" },
                        new MyRegionInfo { Name = "MH", EnglishName = "Marshall Islands", DisplayName = "馬紹爾群島" },
                        new MyRegionInfo { Name = "MQ", EnglishName = "Martinique", DisplayName = "馬提尼克" },
                        new MyRegionInfo { Name = "MR", EnglishName = "Mauritania", DisplayName = "毛里塔尼亞" },
                        new MyRegionInfo { Name = "MU", EnglishName = "Mauritius", DisplayName = "毛里求斯" },
                        new MyRegionInfo { Name = "YT", EnglishName = "Mayotte", DisplayName = "馬約特" },
                        new MyRegionInfo { Name = "MX", EnglishName = "Mexico", DisplayName = "墨西哥" },
                        new MyRegionInfo { Name = "FM", EnglishName = "Micronesia", DisplayName = "密克羅尼西亞" },
                        new MyRegionInfo { Name = "MD", EnglishName = "Moldova", DisplayName = "摩爾多瓦" },
                        new MyRegionInfo { Name = "MC", EnglishName = "Monaco", DisplayName = "摩納哥" },
                        new MyRegionInfo { Name = "MN", EnglishName = "Mongolia", DisplayName = "蒙古" },
                        new MyRegionInfo { Name = "MS", EnglishName = "Montserrat", DisplayName = "蒙特塞拉特" },
                        new MyRegionInfo { Name = "MA", EnglishName = "Morocco", DisplayName = "摩洛哥" },
                        new MyRegionInfo { Name = "MZ", EnglishName = "Mozambique", DisplayName = "莫桑比克" },
                        new MyRegionInfo { Name = "MM", EnglishName = "Myanmar (Burma)", DisplayName = "緬甸（緬甸）" },
                        new MyRegionInfo { Name = "NA", EnglishName = "Namibia", DisplayName = "納米比亞" },
                        new MyRegionInfo { Name = "NR", EnglishName = "Nauru", DisplayName = "瑙魯" },
                        new MyRegionInfo { Name = "NP", EnglishName = "Nepal", DisplayName = "尼泊爾" },
                        new MyRegionInfo { Name = "NL", EnglishName = "Netherlands", DisplayName = "荷蘭" },
                        new MyRegionInfo { Name = "AN", EnglishName = "Netherlands Antilles", DisplayName = "荷屬安的列斯" },
                        new MyRegionInfo { Name = "NC", EnglishName = "New Caledonia", DisplayName = "新喀裡多尼亞" },
                        new MyRegionInfo { Name = "NZ", EnglishName = "New Zealand", DisplayName = "新西蘭" },
                        new MyRegionInfo { Name = "NI", EnglishName = "Nicaragua", DisplayName = "尼加拉瓜" },
                        new MyRegionInfo { Name = "NE", EnglishName = "Niger", DisplayName = "尼日爾" },
                        new MyRegionInfo { Name = "NG", EnglishName = "Nigeria", DisplayName = "尼日利亞" },
                        new MyRegionInfo { Name = "NU", EnglishName = "Niue", DisplayName = "紐埃" },
                        new MyRegionInfo { Name = "NF", EnglishName = "Norfolk Island", DisplayName = "諾福克島" },
                        new MyRegionInfo { Name = "KP", EnglishName = "North Korea", DisplayName = "北韓" },
                        new MyRegionInfo { Name = "MP", EnglishName = "Northern Mariana Islands", DisplayName = "北馬里亞納群島" },
                        new MyRegionInfo { Name = "NO", EnglishName = "Norway", DisplayName = "挪威" },
                        new MyRegionInfo { Name = "OM", EnglishName = "Oman", DisplayName = "阿曼" },
                        new MyRegionInfo { Name = "PK", EnglishName = "Pakistan", DisplayName = "巴基斯坦" },
                        new MyRegionInfo { Name = "PW", EnglishName = "Palau", DisplayName = "帕勞" },
                        new MyRegionInfo { Name = "PA", EnglishName = "Panama", DisplayName = "巴拿馬" },
                        new MyRegionInfo { Name = "PG", EnglishName = "Papua New Guinea", DisplayName = "巴布亞新幾內亞" },
                        new MyRegionInfo { Name = "PY", EnglishName = "Paraguay", DisplayName = "巴拉圭" },
                        new MyRegionInfo { Name = "PE", EnglishName = "Peru", DisplayName = "秘魯" },
                        new MyRegionInfo { Name = "PH", EnglishName = "Philippines", DisplayName = "菲律賓" },
                        new MyRegionInfo { Name = "PN", EnglishName = "Pitcairn", DisplayName = "皮特凱恩" },
                        new MyRegionInfo { Name = "PL", EnglishName = "Poland", DisplayName = "波蘭" },
                        new MyRegionInfo { Name = "PT", EnglishName = "Portugal", DisplayName = "葡萄牙" },
                        new MyRegionInfo { Name = "PR", EnglishName = "Puerto Rico", DisplayName = "波多黎各" },
                        new MyRegionInfo { Name = "QA", EnglishName = "Qatar", DisplayName = "卡塔爾" },
                        new MyRegionInfo { Name = "RE", EnglishName = "Reunion", DisplayName = "留尼汪" },
                        new MyRegionInfo { Name = "RO", EnglishName = "Romania", DisplayName = "羅馬尼亞" },
                        new MyRegionInfo { Name = "RU", EnglishName = "Russia", DisplayName = "俄羅斯" },
                        new MyRegionInfo { Name = "RW", EnglishName = "Rwanda", DisplayName = "盧旺達" },
                        new MyRegionInfo { Name = "SH", EnglishName = "Saint Helena", DisplayName = "聖赫勒拿" },
                        new MyRegionInfo { Name = "KN", EnglishName = "Saint Kitts And Nevis", DisplayName = "聖基茨和尼維斯" },
                        new MyRegionInfo { Name = "LC", EnglishName = "Saint Lucia", DisplayName = "聖盧西亞" },
                        new MyRegionInfo { Name = "PM", EnglishName = "Saint Pierre And Miquelon", DisplayName = "聖皮埃爾和密克隆" },
                        new MyRegionInfo { Name = "VC", EnglishName = "Saint Vincent And The Grenadines", DisplayName = "聖文森特和格林納丁斯" },
                        new MyRegionInfo { Name = "SM", EnglishName = "San Marino", DisplayName = "聖馬力諾" },
                        new MyRegionInfo { Name = "ST", EnglishName = "Sao Tome And Principe", DisplayName = "聖多美和普林西比" },
                        new MyRegionInfo { Name = "SA", EnglishName = "Saudi Arabia", DisplayName = "沙特阿拉伯" },
                        new MyRegionInfo { Name = "SN", EnglishName = "Senegal", DisplayName = "塞內加爾" },
                        new MyRegionInfo { Name = "SC", EnglishName = "Seychelles", DisplayName = "塞舌爾" },
                        new MyRegionInfo { Name = "SL", EnglishName = "Sierra Leone", DisplayName = "塞拉利昂" },
                        new MyRegionInfo { Name = "SG", EnglishName = "Singapore", DisplayName = "新加坡" },
                        new MyRegionInfo { Name = "SK", EnglishName = "Slovak Republic", DisplayName = "斯洛伐克共和國" },
                        new MyRegionInfo { Name = "SI", EnglishName = "Slovenia", DisplayName = "斯洛文尼亞" },
                        new MyRegionInfo { Name = "SB", EnglishName = "Solomon Islands", DisplayName = "所羅門群島" },
                        new MyRegionInfo { Name = "SO", EnglishName = "Somalia", DisplayName = "索馬里" },
                        new MyRegionInfo { Name = "ZA", EnglishName = "South Africa", DisplayName = "南非" },
                        new MyRegionInfo { Name = "GS", EnglishName = "South Georgia And South Sandwich Islands", DisplayName = "南喬治亞島和南桑威奇群島" },
                        new MyRegionInfo { Name = "KR", EnglishName = "South Korea", DisplayName = "韓國" },
                        new MyRegionInfo { Name = "ES", EnglishName = "Spain", DisplayName = "西班牙" },
                        new MyRegionInfo { Name = "LK", EnglishName = "Sri Lanka", DisplayName = "斯里蘭卡" },
                        new MyRegionInfo { Name = "SD", EnglishName = "Sudan", DisplayName = "蘇丹" },
                        new MyRegionInfo { Name = "SR", EnglishName = "Suriname", DisplayName = "蘇里南" },
                        new MyRegionInfo { Name = "SJ", EnglishName = "Svalbard And Jan Mayen", DisplayName = "斯瓦爾巴特群島和揚馬​​延" },
                        new MyRegionInfo { Name = "SZ", EnglishName = "Swaziland", DisplayName = "斯威士蘭" },
                        new MyRegionInfo { Name = "SE", EnglishName = "Sweden", DisplayName = "瑞典" },
                        new MyRegionInfo { Name = "CH", EnglishName = "Switzerland", DisplayName = "瑞士" },
                        new MyRegionInfo { Name = "SY", EnglishName = "Syria", DisplayName = "敘利亞" },
                        new MyRegionInfo { Name = "TJ", EnglishName = "Tajikistan", DisplayName = "塔吉克斯坦" },
                        new MyRegionInfo { Name = "TZ", EnglishName = "Tanzania", DisplayName = "坦桑尼亞" },
                        new MyRegionInfo { Name = "TH", EnglishName = "Thailand", DisplayName = "泰國" },
                        new MyRegionInfo { Name = "TG", EnglishName = "Togo", DisplayName = "多哥" },
                        new MyRegionInfo { Name = "TK", EnglishName = "Tokelau", DisplayName = "托克勞" },
                        new MyRegionInfo { Name = "TO", EnglishName = "Tonga", DisplayName = "湯加" },
                        new MyRegionInfo { Name = "TT", EnglishName = "Trinidad And Tobago", DisplayName = "特立尼達和多巴哥" },
                        new MyRegionInfo { Name = "TN", EnglishName = "Tunisia", DisplayName = "突尼斯" },
                        new MyRegionInfo { Name = "TR", EnglishName = "Turkey", DisplayName = "土耳其" },
                        new MyRegionInfo { Name = "TM", EnglishName = "Turkmenistan", DisplayName = "土庫曼斯坦" },
                        new MyRegionInfo { Name = "TC", EnglishName = "Turks And Caicos Islands", DisplayName = "特克斯和凱科斯群島" },
                        new MyRegionInfo { Name = "TV", EnglishName = "Tuvalu", DisplayName = "圖瓦盧" },
                        new MyRegionInfo { Name = "UG", EnglishName = "Uganda", DisplayName = "烏干達" },
                        new MyRegionInfo { Name = "UA", EnglishName = "Ukraine", DisplayName = "烏克蘭" },
                        new MyRegionInfo { Name = "AE", EnglishName = "United Arab Emirates", DisplayName = "阿拉伯聯合酋長國" },
                        new MyRegionInfo { Name = "GB", EnglishName = "United Kingdom", DisplayName = "聯合王國" },
                        new MyRegionInfo { Name = "US", EnglishName = "United States", DisplayName = "美國" },
                        new MyRegionInfo { Name = "UM", EnglishName = "United States Minor Outlying Islands", DisplayName = "美國本土外小島嶼" },
                        new MyRegionInfo { Name = "UY", EnglishName = "Uruguay", DisplayName = "烏拉圭" },
                        new MyRegionInfo { Name = "UZ", EnglishName = "Uzbekistan", DisplayName = "烏茲別克斯坦" },
                        new MyRegionInfo { Name = "VU", EnglishName = "Vanuatu", DisplayName = "瓦努阿圖" },
                        new MyRegionInfo { Name = "VA", EnglishName = "Vatican City (Holy See)", DisplayName = "梵蒂岡（羅馬教廷）" },
                        new MyRegionInfo { Name = "VE", EnglishName = "Venezuela", DisplayName = "委內瑞拉" },
                        new MyRegionInfo { Name = "VN", EnglishName = "Vietnam", DisplayName = "越南" },
                        new MyRegionInfo { Name = "VG", EnglishName = "Virgin Islands (British)", DisplayName = "維爾京群島（英國）" },
                        new MyRegionInfo { Name = "VI", EnglishName = "Virgin Islands (US)", DisplayName = "維爾京群島（美國）" },
                        new MyRegionInfo { Name = "WF", EnglishName = "Wallis And Futuna Islands", DisplayName = "瓦利斯群島和富圖納群島" },
                        new MyRegionInfo { Name = "EH", EnglishName = "Western Sahara", DisplayName = "西撒哈拉" },
                        new MyRegionInfo { Name = "WS", EnglishName = "Western Samoa", DisplayName = "西薩摩亞" },
                        new MyRegionInfo { Name = "YE", EnglishName = "Yemen", DisplayName = "也門" },
                        new MyRegionInfo { Name = "YU", EnglishName = "Yugoslavia", DisplayName = "南斯拉夫" },
                        new MyRegionInfo { Name = "ZM", EnglishName = "Zambia", DisplayName = "贊比亞" },
                        new MyRegionInfo { Name = "ZW", EnglishName = "Zimbabwe", DisplayName = "津巴布韋" },

                    };
                }
                
            }
            return regionInfos;
        }

        public static PhoneNumber Parse(string mobile, bool throwException)
        {
            try
            {
                var iso3166s = GetRegionInfos().Select(p => p.Name).ToList();

                PhoneNumber number = null;
                foreach (var iso3166 in iso3166s)
                {
                    PhoneNumber _number = PhoneNumberUtil.GetInstance().Parse(mobile, iso3166);

                    if (PhoneNumberUtil.GetInstance().IsPossibleNumber(_number))
                    {
                        number = _number;
                        break;
                    }
                }

                return number;
            }
            catch
            {
                if (throwException)
                    throw;

                return null;
            }
        }

        public static bool IsPossibleNumber(string mobile)
        {
            //Regex rgx = new Regex(RegularExpressionPatterns.PhoneNumber);
            //return rgx.IsMatch(mobile);

            return Parse(mobile, throwException: false) != null;
        }

        private static string Format(string mobile, PhoneNumberFormat fmt, bool throwException = false)
        {
            PhoneNumber number = Parse(mobile, throwException: throwException);
            return number != null 
                ? PhoneNumberUtil.GetInstance().Format(number, fmt) 
                : null /* 目前認為傳回null，正好可以表示這個輸入的號碼不合法 */;
        }

        private static string GetRegionCode(string mobile)
        {
            PhoneNumber number = Parse(mobile, throwException: false);
            return number != null 
                ? PhoneNumberUtil.GetInstance().GetRegionCodeForNumber(number) 
                : null /* 目前認為傳回null，正好可以表示這個輸入的號碼不合法 */;
        }

        private static MyRegionInfo GetRegionInfo(string mobile)
        {
            var regionCode = GetRegionCode(mobile);

            return regionCode != null 
                ? GetRegionInfos().Where(p => p.Name == regionCode).FirstOrDefault()
                : null /* 目前認為傳回null，正好可以表示這個輸入的號碼不合法 */;
        }

        public static string GetRegionName(string mobile)
        {
            var regionInfo = GetRegionInfo(mobile);

            return regionInfo != null
            ? regionInfo.DisplayName
            : null /* 目前認為傳回null，正好可以表示這個輸入的號碼不合法 */;
        }
        
        
        /// <summary>
        /// 使用 E164 格式的電話號碼，因為 Infobip & every8d 都支援
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public static string GetE164PhoneNumber(string mobile, bool throwException = false)
        {
            return Format(mobile, PhoneNumberFormat.E164, throwException);
        }

        /// <summary>
        /// 是否為國際簡訊號碼
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public static bool IsInternationPhoneNumber(string mobile)
        {
            return !GetE164PhoneNumber(mobile).StartsWith("+886", StringComparison.OrdinalIgnoreCase);
        }
    }
}