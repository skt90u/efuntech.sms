(function (window, document) {
    'use strict';

    
    angular.module('app').factory('MobileUtil', ['$log', function ($log) {
        
        var regionInfos = [
            { value: "TW", text: "Taiwan" }, // 把它放在第一位
            { value: "HK", text: "Hong Kong" },
            { value: "CN", text: "China" },
            /*
            { value: "AF", text: "Afghanistan"},
            { value: "AL", text: "Albania"},
            { value: "DZ", text: "Algeria"},
            { value: "AS", text: "American Samoa"},
            { value: "AD", text: "Andorra"},
            { value: "AO", text: "Angola"},
            { value: "AI", text: "Anguilla"},
            { value: "AQ", text: "Antarctica"},
            { value: "AG", text: "Antigua And Barbuda"},
            { value: "AR", text: "Argentina"},
            { value: "AM", text: "Armenia"},
            { value: "AW", text: "Aruba"},
            { value: "AU", text: "Australia"},
            { value: "AT", text: "Austria"},
            { value: "AZ", text: "Azerbaijan"},
            { value: "BS", text: "Bahamas"},
            { value: "BH", text: "Bahrain"},
            { value: "BD", text: "Bangladesh"},
            { value: "BB", text: "Barbados"},
            { value: "BY", text: "Belarus"},
            { value: "BE", text: "Belgium"},
            { value: "BZ", text: "Belize"},
            { value: "BJ", text: "Benin"},
            { value: "BM", text: "Bermuda"},
            { value: "BT", text: "Bhutan"},
            { value: "BO", text: "Bolivia"},
            { value: "BA", text: "Bosnia And Herzegovina"},
            { value: "BW", text: "Botswana"},
            { value: "BV", text: "Bouvet Island"},
            { value: "BR", text: "Brazil"},
            { value: "IO", text: "British Indian Ocean Territory"},
            { value: "BN", text: "Brunei"},
            { value: "BG", text: "Bulgaria"},
            { value: "BF", text: "Burkina Faso"},
            { value: "BI", text: "Burundi"},
            { value: "KH", text: "Cambodia"},
            { value: "CM", text: "Cameroon"},
            { value: "CA", text: "Canada"},
            { value: "CV", text: "Cape Verde"},
            { value: "KY", text: "Cayman Islands"},
            { value: "CF", text: "Central African Republic"},
            { value: "TD", text: "Chad"},
            { value: "CL", text: "Chile"},
            { value: "CN", text: "China"},
            { value: "CX", text: "Christmas Island"},
            { value: "CC", text: "Cocos (Keeling) Islands"},
            { value: "CO", text: "Columbia"},
            { value: "KM", text: "Comoros"},
            { value: "CG", text: "Congo"},
            { value: "CK", text: "Cook Islands"},
            { value: "CR", text: "Costa Rica"},
            { value: "CI", text: "Cote D'Ivorie (Ivory Coast)"},
            { value: "HR", text: "Croatia (Hrvatska)"},
            { value: "CU", text: "Cuba"},
            { value: "CY", text: "Cyprus"},
            { value: "CZ", text: "Czech Republic"},
            { value: "CD", text: "Democratic Republic Of Congo (Zaire)"},
            { value: "DK", text: "Denmark"},
            { value: "DJ", text: "Djibouti"},
            { value: "DM", text: "Dominica"},
            { value: "DO", text: "Dominican Republic"},
            { value: "TP", text: "East Timor"},
            { value: "EC", text: "Ecuador"},
            { value: "EG", text: "Egypt"},
            { value: "SV", text: "El Salvador"},
            { value: "GQ", text: "Equatorial Guinea"},
            { value: "ER", text: "Eritrea"},
            { value: "EE", text: "Estonia"},
            { value: "ET", text: "Ethiopia"},
            { value: "FK", text: "Falkland Islands (Malvinas)"},
            { value: "FO", text: "Faroe Islands"},
            { value: "FJ", text: "Fiji"},
            { value: "FI", text: "Finland"},
            { value: "FR", text: "France"},
            { value: "FX", text: "France, Metropolitan"},
            { value: "GF", text: "French Guinea"},
            { value: "PF", text: "French Polynesia"},
            { value: "TF", text: "French Southern Territories"},
            { value: "GA", text: "Gabon"},
            { value: "GM", text: "Gambia"},
            { value: "GE", text: "Georgia"},
            { value: "DE", text: "Germany"},
            { value: "GH", text: "Ghana"},
            { value: "GI", text: "Gibraltar"},
            { value: "GR", text: "Greece"},
            { value: "GL", text: "Greenland"},
            { value: "GD", text: "Grenada"},
            { value: "GP", text: "Guadeloupe"},
            { value: "GU", text: "Guam"},
            { value: "GT", text: "Guatemala"},
            { value: "GN", text: "Guinea"},
            { value: "GW", text: "Guinea-Bissau"},
            { value: "GY", text: "Guyana"},
            { value: "HT", text: "Haiti"},
            { value: "HM", text: "Heard And McDonald Islands"},
            { value: "HN", text: "Honduras"},
            { value: "HK", text: "Hong Kong"},
            { value: "HU", text: "Hungary"},
            { value: "IS", text: "Iceland"},
            { value: "IN", text: "India"},
            { value: "ID", text: "Indonesia"},
            { value: "IR", text: "Iran"},
            { value: "IQ", text: "Iraq"},
            { value: "IE", text: "Ireland"},
            { value: "IM", text: "Isle of Man"},
            { value: "IL", text: "Israel"},
            { value: "IT", text: "Italy"},
            { value: "JM", text: "Jamaica"},
            { value: "JP", text: "Japan"},
            { value: "JO", text: "Jordan"},
            { value: "KZ", text: "Kazakhstan"},
            { value: "KE", text: "Kenya"},
            { value: "KI", text: "Kiribati"},
            { value: "KW", text: "Kuwait"},
            { value: "KG", text: "Kyrgyzstan"},
            { value: "LA", text: "Laos"},
            { value: "LV", text: "Latvia"},
            { value: "LB", text: "Lebanon"},
            { value: "LS", text: "Lesotho"},
            { value: "LR", text: "Liberia"},
            { value: "LY", text: "Libya"},
            { value: "LI", text: "Liechtenstein"},
            { value: "LT", text: "Lithuania"},
            { value: "LU", text: "Luxembourg"},
            { value: "MO", text: "Macau"},
            { value: "MK", text: "Macedonia"},
            { value: "MG", text: "Madagascar"},
            { value: "MW", text: "Malawi"},
            { value: "MY", text: "Malaysia"},
            { value: "MV", text: "Maldives"},
            { value: "ML", text: "Mali"},
            { value: "MT", text: "Malta"},
            { value: "MH", text: "Marshall Islands"},
            { value: "MQ", text: "Martinique"},
            { value: "MR", text: "Mauritania"},
            { value: "MU", text: "Mauritius"},
            { value: "YT", text: "Mayotte"},
            { value: "MX", text: "Mexico"},
            { value: "FM", text: "Micronesia"},
            { value: "MD", text: "Moldova"},
            { value: "MC", text: "Monaco"},
            { value: "MN", text: "Mongolia"},
            { value: "MS", text: "Montserrat"},
            { value: "MA", text: "Morocco"},
            { value: "MZ", text: "Mozambique"},
            { value: "MM", text: "Myanmar (Burma)"},
            { value: "NA", text: "Namibia"},
            { value: "NR", text: "Nauru"},
            { value: "NP", text: "Nepal"},
            { value: "NL", text: "Netherlands"},
            { value: "AN", text: "Netherlands Antilles"},
            { value: "NC", text: "New Caledonia"},
            { value: "NZ", text: "New Zealand"},
            { value: "NI", text: "Nicaragua"},
            { value: "NE", text: "Niger"},
            { value: "NG", text: "Nigeria"},
            { value: "NU", text: "Niue"},
            { value: "NF", text: "Norfolk Island"},
            { value: "KP", text: "North Korea"},
            { value: "MP", text: "Northern Mariana Islands"},
            { value: "NO", text: "Norway"},
            { value: "OM", text: "Oman"},
            { value: "PK", text: "Pakistan"},
            { value: "PW", text: "Palau"},
            { value: "PA", text: "Panama"},
            { value: "PG", text: "Papua New Guinea"},
            { value: "PY", text: "Paraguay"},
            { value: "PE", text: "Peru"},
            { value: "PH", text: "Philippines"},
            { value: "PN", text: "Pitcairn"},
            { value: "PL", text: "Poland"},
            { value: "PT", text: "Portugal"},
            { value: "PR", text: "Puerto Rico"},
            { value: "QA", text: "Qatar"},
            { value: "RE", text: "Reunion"},
            { value: "RO", text: "Romania"},
            { value: "RU", text: "Russia"},
            { value: "RW", text: "Rwanda"},
            { value: "SH", text: "Saint Helena"},
            { value: "KN", text: "Saint Kitts And Nevis"},
            { value: "LC", text: "Saint Lucia"},
            { value: "PM", text: "Saint Pierre And Miquelon"},
            { value: "VC", text: "Saint Vincent And The Grenadines"},
            { value: "SM", text: "San Marino"},
            { value: "ST", text: "Sao Tome And Principe"},
            { value: "SA", text: "Saudi Arabia"},
            { value: "SN", text: "Senegal"},
            { value: "SC", text: "Seychelles"},
            { value: "SL", text: "Sierra Leone"},
            { value: "SG", text: "Singapore"},
            { value: "SK", text: "Slovak Republic"},
            { value: "SI", text: "Slovenia"},
            { value: "SB", text: "Solomon Islands"},
            { value: "SO", text: "Somalia"},
            { value: "ZA", text: "South Africa"},
            { value: "GS", text: "South Georgia And South Sandwich Islands"},
            { value: "KR", text: "South Korea"},
            { value: "ES", text: "Spain"},
            { value: "LK", text: "Sri Lanka"},
            { value: "SD", text: "Sudan"},
            { value: "SR", text: "Suriname"},
            { value: "SJ", text: "Svalbard And Jan Mayen"},
            { value: "SZ", text: "Swaziland"},
            { value: "SE", text: "Sweden"},
            { value: "CH", text: "Switzerland"},
            { value: "SY", text: "Syria"},
            //{ value: "TW", text: "Taiwan"}, // 把它放在第一位
            { value: "TJ", text: "Tajikistan"},
            { value: "TZ", text: "Tanzania"},
            { value: "TH", text: "Thailand"},
            { value: "TG", text: "Togo"},
            { value: "TK", text: "Tokelau"},
            { value: "TO", text: "Tonga"},
            { value: "TT", text: "Trinidad And Tobago"},
            { value: "TN", text: "Tunisia"},
            { value: "TR", text: "Turkey"},
            { value: "TM", text: "Turkmenistan"},
            { value: "TC", text: "Turks And Caicos Islands"},
            { value: "TV", text: "Tuvalu"},
            { value: "UG", text: "Uganda"},
            { value: "UA", text: "Ukraine"},
            { value: "AE", text: "United Arab Emirates"},
            { value: "GB", text: "United Kingdom"},
            { value: "US", text: "United States"},
            { value: "UM", text: "United States Minor Outlying Islands"},
            { value: "UY", text: "Uruguay"},
            { value: "UZ", text: "Uzbekistan"},
            { value: "VU", text: "Vanuatu"},
            { value: "VA", text: "Vatican City (Holy See)"},
            { value: "VE", text: "Venezuela"},
            { value: "VN", text: "Vietnam"},
            { value: "VG", text: "Virgin Islands (British)"},
            { value: "VI", text: "Virgin Islands (US)"},
            { value: "WF", text: "Wallis And Futuna Islands"},
            { value: "EH", text: "Western Sahara"},
            { value: "WS", text: "Western Samoa"},
            { value: "YE", text: "Yemen"},
            { value: "YU", text: "Yugoslavia"},
            { value: "ZM", text: "Zambia"},
            { value: "ZW", text: "Zimbabwe"},
            */
        ];

        var iso3166s = _.pluck(regionInfos, 'value');

        function processPhoneNumber(phone, regionCode) {

            phone = phone || '';
            regionCode = regionCode || 'TW';

            // 回傳 PhoneNumber 資訊，請參考
            // http://www.phoneformat.com/
            // http://www.phoneformat.com/js/source.js

            var e164 = formatE164(regionCode, phone);
            if (e164.substring(0, 1) !== "+") {
                e164 = null;
            }

            var international = formatInternational(regionCode, phone);
            var national = formatLocal(regionCode, phone);

            var country = null;
            var countryCode = countryForE164Number(formatE164(regionCode, phone));
            if (countryCode.length !== 0) {
                country = countryCode + " - " + countryCodeToName(countryCode);
            }

            var mobileDial = formatNumberForMobileDialing(regionCode, phone);
            var valid = isValidNumber(phone, regionCode);

            var result = {
                e164: e164,
                international: international,
                national: national,
                country: country,
                countryCode: countryCode,
                mobileDial: mobileDial,
                valid: valid,
            };

            if (result.valid)
            {
                $log.log('phone: ' + phone);
                $log.log('regionCode: ' + regionCode);
                $log.log(result);
            }

            return result;
        }

        function parse(phone) {

            for (var i = 0; i < iso3166s.length; i++) {

                var regionCode = iso3166s[i];

                var phoneNumber = processPhoneNumber(phone, regionCode);

                if (phoneNumber.valid)
                    return phoneNumber;
            }

            return null;
        }

        function isPossibleNumber(phone) {
            //var reg = RegularExpressionPatterns.PhoneNumber.pattern;
            //return reg.test(input);
            var phoneNumber = parse(phone);
            return phoneNumber != null;
        }

        function getE164PhoneNumber(phone) {
            var phoneNumber = parse(phone);
            return phoneNumber != null ? phoneNumber.e164 : null;
        }

        return {
            parse: parse,
            isPossibleNumber: isPossibleNumber,
            getE164PhoneNumber: getE164PhoneNumber,
        };
        
    }]);

})(window, document);