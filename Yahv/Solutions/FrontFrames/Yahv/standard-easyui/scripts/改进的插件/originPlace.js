(function ($) {
    var originPlaceData = [{
        "en": "Angola",
        "Name": "安哥拉",
        "abbreviation": "AO"
    }, {
        "en": "Afghanistan",
        "Name": "阿富汗",
        "abbreviation": "AF"
    }, {
        "en": "Albania",
        "Name": "阿尔巴尼亚",
        "abbreviation": "AL"
    }, {
        "en": "Algeria",
        "Name": "阿尔及利亚",
        "abbreviation": "DZ"
    }, {
        "en": "Andorra",
        "Name": "安道尔共和国",
        "abbreviation": "AD"
    }, {
        "en": "Anguilla",
        "Name": "安圭拉岛",
        "abbreviation": "AI"
    }, {
        "en": "Antigua and Barbuda",
        "Name": "安提瓜和巴布达",
        "abbreviation": "AG"
    }, {
        "en": "Argentina",
        "Name": "阿根廷",
        "abbreviation": "AR"
    }, {
        "en": "Armenia",
        "Name": "亚美尼亚",
        "abbreviation": "AM"
    }, {
        "en": "Ascension",
        "Name": "阿森松",
        "abbreviation": " "
    }, {
        "en": "Australia",
        "Name": "澳大利亚",
        "abbreviation": "AU"
    }, {
        "en": "Austria",
        "Name": "奥地利",
        "abbreviation": "AT"
    }, {
        "en": "Azerbaijan",
        "Name": "阿塞拜疆",
        "abbreviation": "AZ"
    }, {
        "en": "Bahamas",
        "Name": "巴哈马",
        "abbreviation": "BS"
    }, {
        "en": "Bahrain",
        "Name": "巴林",
        "abbreviation": "BH"
    }, {
        "en": "Bangladesh",
        "Name": "孟加拉国",
        "abbreviation": "BD"
    }, {
        "en": "Barbados",
        "Name": "巴巴多斯",
        "abbreviation": "BB"
    }, {
        "en": "Belarus",
        "Name": "白俄罗斯",
        "abbreviation": "BY"
    }, {
        "en": "Belgium",
        "Name": "比利时",
        "abbreviation": "BE"
    }, {
        "en": "Belize",
        "Name": "伯利兹",
        "abbreviation": "BZ"
    }, {
        "en": "Benin",
        "Name": "贝宁",
        "abbreviation": "BJ"
    }, {
        "en": "Bermuda Is.",
        "Name": "百慕大群岛",
        "abbreviation": "BM"
    }, {
        "en": "Bolivia",
        "Name": "玻利维亚",
        "abbreviation": "BO"
    }, {
        "en": "Botswana",
        "Name": "博茨瓦纳",
        "abbreviation": "BW"
    }, {
        "en": "Brazil",
        "Name": "巴西",
        "abbreviation": "BR"
    }, {
        "en": "Brunei",
        "Name": "文莱",
        "abbreviation": "BN"
    }, {
        "en": "Bulgaria",
        "Name": "保加利亚",
        "abbreviation": "BG"
    }, {
        "en": "Burkina-faso",
        "Name": "布基纳法索",
        "abbreviation": "BF"
    }, {
        "en": "Burma",
        "Name": "缅甸",
        "abbreviation": "MM"
    }, {
        "en": "Burundi",
        "Name": "布隆迪",
        "abbreviation": "BI"
    }, {
        "en": "Cameroon",
        "Name": "喀麦隆",
        "abbreviation": "CM"
    }, {
        "en": "Canada",
        "Name": "加拿大",
        "abbreviation": "CA"
    }, {
        "en": "Cayman Is.",
        "Name": "开曼群岛",
        "abbreviation": " "
    }, {
        "en": "Central African Republic",
        "Name": "中非共和国",
        "abbreviation": "CF"
    }, {
        "en": "Chad",
        "Name": "乍得",
        "abbreviation": "TD"
    }, {
        "en": "Chile",
        "Name": "智利",
        "abbreviation": "CL"
    }, {
        "en": "China",
        "Name": "中国",
        "abbreviation": "Name"
    }, {
        "en": "Colombia",
        "Name": "哥伦比亚",
        "abbreviation": "CO"
    }, {
        "en": "Congo",
        "Name": "刚果",
        "abbreviation": "CG"
    }, {
        "en": "Cook Is.",
        "Name": "库克群岛",
        "abbreviation": "CK"
    }, {
        "en": "Costa Rica",
        "Name": "哥斯达黎加",
        "abbreviation": "CR"
    }, {
        "en": "Cuba",
        "Name": "古巴",
        "abbreviation": "CU"
    }, {
        "en": "Cyprus",
        "Name": "塞浦路斯",
        "abbreviation": "CY"
    }, {
        "en": "Czech Republic ",
        "Name": "捷克",
        "abbreviation": "CZ"
    }, {
        "en": "Denmark",
        "Name": "丹麦",
        "abbreviation": "DK"
    }, {
        "en": "Djibouti",
        "Name": "吉布提",
        "abbreviation": "DJ"
    }, {
        "en": "Dominica Rep.",
        "Name": "多米尼加共和国",
        "abbreviation": "DO"
    }, {
        "en": "Ecuador",
        "Name": "厄瓜多尔",
        "abbreviation": "EC"
    }, {
        "en": "Egypt",
        "Name": "埃及",
        "abbreviation": "EG"
    }, {
        "en": "EI Salvador",
        "Name": "萨尔瓦多",
        "abbreviation": "SV"
    }, {
        "en": "Estonia",
        "Name": "爱沙尼亚",
        "abbreviation": "EE"
    }, {
        "en": "Ethiopia",
        "Name": "埃塞俄比亚",
        "abbreviation": "ET"
    }, {
        "en": "Fiji",
        "Name": "斐济",
        "abbreviation": "FJ"
    }, {
        "en": "Finland",
        "Name": "芬兰",
        "abbreviation": "FI"
    }, {
        "en": "France",
        "Name": "法国",
        "abbreviation": "FR"
    }, {
        "en": "French Guiana",
        "Name": "法属圭亚那",
        "abbreviation": "GF"
    }, {
        "en": "Gabon",
        "Name": "加蓬",
        "abbreviation": "GA"
    }, {
        "en": "Gambia",
        "Name": "冈比亚",
        "abbreviation": "GM"
    }, {
        "en": "Georgia ",
        "Name": "格鲁吉亚",
        "abbreviation": "GE"
    }, {
        "en": "Germany ",
        "Name": "德国",
        "abbreviation": "DE"
    }, {
        "en": "Ghana",
        "Name": "加纳",
        "abbreviation": "GH"
    }, {
        "en": "Gibraltar",
        "Name": "直布罗陀",
        "abbreviation": "GI"
    }, {
        "en": "Greece",
        "Name": "希腊",
        "abbreviation": "GR"
    }, {
        "en": "Grenada",
        "Name": "格林纳达",
        "abbreviation": "GD"
    }, {
        "en": "Guam",
        "Name": "关岛",
        "abbreviation": "GU"
    }, {
        "en": "Guatemala",
        "Name": "危地马拉",
        "abbreviation": "GT"
    }, {
        "en": "Guinea",
        "Name": "几内亚",
        "abbreviation": "GN"
    }, {
        "en": "Guyana",
        "Name": "圭亚那",
        "abbreviation": "GY"
    }, {
        "en": "Haiti",
        "Name": "海地",
        "abbreviation": "HT"
    }, {
        "en": "Honduras",
        "Name": "洪都拉斯",
        "abbreviation": "HN"
    }, {
        "en": "Hongkong",
        "Name": "香港",
        "abbreviation": "HK"
    }, {
        "en": "Hungary",
        "Name": "匈牙利",
        "abbreviation": "HU"
    }, {
        "en": "Iceland",
        "Name": "冰岛",
        "abbreviation": "IS"
    }, {
        "en": "India",
        "Name": "印度",
        "abbreviation": "IN"
    }, {
        "en": "Indonesia",
        "Name": "印度尼西亚",
        "abbreviation": "ID"
    }, {
        "en": "Iran",
        "Name": "伊朗",
        "abbreviation": "IR"
    }, {
        "en": "Iraq",
        "Name": "伊拉克",
        "abbreviation": "IQ"
    }, {
        "en": "Ireland",
        "Name": "爱尔兰",
        "abbreviation": "IE"
    }, {
        "en": "Israel",
        "Name": "以色列",
        "abbreviation": "IL"
    }, {
        "en": "Italy",
        "Name": "意大利",
        "abbreviation": "IT"
    }, {
        "en": "Ivory Coast",
        "Name": "科特迪瓦",
        "abbreviation": " "
    }, {
        "en": "Jamaica",
        "Name": "牙买加",
        "abbreviation": "JM"
    }, {
        "en": "Japan",
        "Name": "日本",
        "abbreviation": "JP"
    }, {
        "en": "Jordan",
        "Name": "约旦",
        "abbreviation": "JO"
    }, {
        "en": "Kampuchea (Cambodia )",
        "Name": "柬埔寨",
        "abbreviation": "KH"
    }, {
        "en": "Kazakstan",
        "Name": "哈萨克斯坦",
        "abbreviation": "KZ"
    }, {
        "en": "Kenya",
        "Name": "肯尼亚",
        "abbreviation": "KE"
    }, {
        "en": "Korea",
        "Name": "韩国",
        "abbreviation": "KR"
    }, {
        "en": "Kuwait",
        "Name": "科威特",
        "abbreviation": "KW"
    }, {
        "en": "Kyrgyzstan ",
        "Name": "吉尔吉斯坦",
        "abbreviation": "KG"
    }, {
        "en": "Laos",
        "Name": "老挝",
        "abbreviation": "LA"
    }, {
        "en": "Latvia ",
        "Name": "拉脱维亚",
        "abbreviation": "LV"
    }, {
        "en": "Lebanon",
        "Name": "黎巴嫩",
        "abbreviation": "LB"
    }, {
        "en": "Lesotho",
        "Name": "莱索托",
        "abbreviation": "LS"
    }, {
        "en": "Liberia",
        "Name": "利比里亚",
        "abbreviation": "LR"
    }, {
        "en": "Libya",
        "Name": "利比亚",
        "abbreviation": "LY"
    }, {
        "en": "Liechtenstein",
        "Name": "列支敦士登",
        "abbreviation": "LI"
    }, {
        "en": "Lithuania",
        "Name": "立陶宛",
        "abbreviation": "LT"
    }, {
        "en": "Luxembourg",
        "Name": "卢森堡",
        "abbreviation": "LU"
    }, {
        "en": "Macao",
        "Name": "澳门",
        "abbreviation": "MO"
    }, {
        "en": "Madagascar",
        "Name": "马达加斯加",
        "abbreviation": "MG"
    }, {
        "en": "Malawi",
        "Name": "马拉维",
        "abbreviation": "MW"
    }, {
        "en": "Malaysia",
        "Name": "马来西亚",
        "abbreviation": "MY"
    }, {
        "en": "Maldives",
        "Name": "马尔代夫",
        "abbreviation": "MV"
    }, {
        "en": "Mali",
        "Name": "马里",
        "abbreviation": "ML"
    }, {
        "en": "Malta",
        "Name": "马耳他",
        "abbreviation": "MT"
    }, {
        "en": "Mariana Is",
        "Name": "马里亚那群岛",
        "abbreviation": " "
    }, {
        "en": "Martinique",
        "Name": "马提尼克",
        "abbreviation": " "
    }, {
        "en": "Mauritius",
        "Name": "毛里求斯",
        "abbreviation": "MU"
    }, {
        "en": "Mexico",
        "Name": "墨西哥",
        "abbreviation": "MX"
    }, {
        "en": "Moldova, Republic of ",
        "Name": "摩尔多瓦",
        "abbreviation": "MD"
    }, {
        "en": "Monaco",
        "Name": "摩纳哥",
        "abbreviation": "MC"
    }, {
        "en": "Mongolia ",
        "Name": "蒙古",
        "abbreviation": "MN"
    }, {
        "en": "Montserrat Is",
        "Name": "蒙特塞拉特岛",
        "abbreviation": "MS"
    }, {
        "en": "Morocco",
        "Name": "摩洛哥",
        "abbreviation": "MA"
    }, {
        "en": "Mozambique",
        "Name": "莫桑比克",
        "abbreviation": "MZ"
    }, {
        "en": "Namibia ",
        "Name": "纳米比亚",
        "abbreviation": "NA"
    }, {
        "en": "Nauru",
        "Name": "瑙鲁",
        "abbreviation": "NR"
    }, {
        "en": "Nepal",
        "Name": "尼泊尔",
        "abbreviation": "NP"
    }, {
        "en": "Netheriands Antilles",
        "Name": "荷属安的列斯",
        "abbreviation": " "
    }, {
        "en": "Netherlands",
        "Name": "荷兰",
        "abbreviation": "NL"
    }, {
        "en": "New Zealand",
        "Name": "新西兰",
        "abbreviation": "NZ"
    }, {
        "en": "Nicaragua",
        "Name": "尼加拉瓜",
        "abbreviation": "NI"
    }, {
        "en": "Niger",
        "Name": "尼日尔",
        "abbreviation": "NE"
    }, {
        "en": "Nigeria",
        "Name": "尼日利亚",
        "abbreviation": "NG"
    }, {
        "en": "North Korea",
        "Name": "朝鲜",
        "abbreviation": "KP"
    }, {
        "en": "Norway",
        "Name": "挪威",
        "abbreviation": "NO"
    }, {
        "en": "Oman",
        "Name": "阿曼",
        "abbreviation": "OM"
    }, {
        "en": "Pakistan",
        "Name": "巴基斯坦",
        "abbreviation": "PK"
    }, {
        "en": "Panama",
        "Name": "巴拿马",
        "abbreviation": "PA"
    }, {
        "en": "Papua New Cuinea",
        "Name": "巴布亚新几内亚",
        "abbreviation": "PG"
    }, {
        "en": "Paraguay",
        "Name": "巴拉圭",
        "abbreviation": "PY"
    }, {
        "en": "Peru",
        "Name": "秘鲁",
        "abbreviation": "PE"
    }, {
        "en": "Philippines",
        "Name": "菲律宾",
        "abbreviation": "PH"
    }, {
        "en": "Poland",
        "Name": "波兰",
        "abbreviation": "PL"
    }, {
        "en": "French Polynesia",
        "Name": "法属玻利尼西亚",
        "abbreviation": "PF"
    }, {
        "en": "Portugal",
        "Name": "葡萄牙",
        "abbreviation": "PT"
    }, {
        "en": "Puerto Rico",
        "Name": "波多黎各",
        "abbreviation": "PR"
    }, {
        "en": "Qatar",
        "Name": "卡塔尔",
        "abbreviation": "QA"
    }, {
        "en": "Reunion",
        "Name": "留尼旺",
        "abbreviation": " "
    }, {
        "en": "Romania",
        "Name": "罗马尼亚",
        "abbreviation": "RO"
    }, {
        "en": "Russia",
        "Name": "俄罗斯",
        "abbreviation": "RU"
    }, {
        "en": "Saint Lueia",
        "Name": "圣卢西亚",
        "abbreviation": "LC"
    }, {
        "en": "Saint Vincent",
        "Name": "圣文森特岛",
        "abbreviation": "VC"
    }, {
        "en": "Samoa Eastern",
        "Name": "东萨摩亚(美)",
        "abbreviation": " "
    }, {
        "en": "Samoa Western",
        "Name": "西萨摩亚",
        "abbreviation": " "
    }, {
        "en": "San Marino",
        "Name": "圣马力诺",
        "abbreviation": "SM"
    }, {
        "en": "Sao Tome and Principe",
        "Name": "圣多美和普林西比",
        "abbreviation": "ST"
    }, {
        "en": "Saudi Arabia",
        "Name": "沙特阿拉伯",
        "abbreviation": "SA"
    }, {
        "en": "Senegal",
        "Name": "塞内加尔",
        "abbreviation": "SN"
    }, {
        "en": "Seychelles",
        "Name": "塞舌尔",
        "abbreviation": "SC"
    }, {
        "en": "Sierra Leone",
        "Name": "塞拉利昂",
        "abbreviation": "SL"
    }, {
        "en": "Singapore",
        "Name": "新加坡",
        "abbreviation": "SG"
    }, {
        "en": "Slovakia",
        "Name": "斯洛伐克",
        "abbreviation": "SK"
    }, {
        "en": "Slovenia",
        "Name": "斯洛文尼亚",
        "abbreviation": "SI"
    }, {
        "en": "Solomon Is",
        "Name": "所罗门群岛",
        "abbreviation": "SB"
    }, {
        "en": "Somali",
        "Name": "索马里",
        "abbreviation": "SO"
    }, {
        "en": "South Africa",
        "Name": "南非",
        "abbreviation": "ZA"
    }, {
        "en": "Spain",
        "Name": "西班牙",
        "abbreviation": "ES"
    }, {
        "en": "Sri Lanka",
        "Name": "斯里兰卡",
        "abbreviation": "LK"
    }, {
        "en": "St.Lucia",
        "Name": "圣卢西亚",
        "abbreviation": "LC"
    }, {
        "en": "St.Vincent",
        "Name": "圣文森特",
        "abbreviation": "VC"
    }, {
        "en": "Sudan",
        "Name": "苏丹",
        "abbreviation": "SD"
    }, {
        "en": "Suriname",
        "Name": "苏里南",
        "abbreviation": "SR"
    }, {
        "en": "Swaziland",
        "Name": "斯威士兰",
        "abbreviation": "SZ"
    }, {
        "en": "Sweden",
        "Name": "瑞典",
        "abbreviation": "SE"
    }, {
        "en": "Switzerland",
        "Name": "瑞士",
        "abbreviation": "CH"
    }, {
        "en": "Syria",
        "Name": "叙利亚",
        "abbreviation": "SY"
    }, {
        "en": "Taiwan",
        "Name": "台湾省",
        "abbreviation": "TW"
    }, {
        "en": "Tajikstan",
        "Name": "塔吉克斯坦",
        "abbreviation": "TJ"
    }, {
        "en": "Tanzania",
        "Name": "坦桑尼亚",
        "abbreviation": "TZ"
    }, {
        "en": "Thailand",
        "Name": "泰国",
        "abbreviation": "TH"
    }, {
        "en": "Togo",
        "Name": "多哥",
        "abbreviation": "TG"
    }, {
        "en": "Tonga",
        "Name": "汤加",
        "abbreviation": "TO"
    }, {
        "en": "Trinidad and Tobago",
        "Name": "特立尼达和多巴哥",
        "abbreviation": "TT"
    }, {
        "en": "Tunisia",
        "Name": "突尼斯",
        "abbreviation": "TN"
    }, {
        "en": "Turkey",
        "Name": "土耳其",
        "abbreviation": "TR"
    }, {
        "en": "Turkmenistan ",
        "Name": "土库曼斯坦",
        "abbreviation": "TM"
    }, {
        "en": "Uganda",
        "Name": "乌干达",
        "abbreviation": "UG"
    }, {
        "en": "Ukraine",
        "Name": "乌克兰",
        "abbreviation": "UA"
    }, {
        "en": "United Arab Emirates",
        "Name": "阿拉伯联合酋长国",
        "abbreviation": "AE"
    }, {
        "en": "United Kiongdom",
        "Name": "英国",
        "abbreviation": "GB"
    }, {
        "en": "United States of America",
        "Name": "美国",
        "abbreviation": "US"
    }, {
        "en": "Uruguay",
        "Name": "乌拉圭",
        "abbreviation": "UY"
    }, {
        "en": "Uzbekistan",
        "Name": "乌兹别克斯坦",
        "abbreviation": "UZ"
    }, {
        "en": "Venezuela",
        "Name": "委内瑞拉",
        "abbreviation": "VE"
    }, {
        "en": "Vietnam",
        "Name": "越南",
        "abbreviation": "VN"
    }, {
        "en": "Yemen",
        "Name": "也门",
        "abbreviation": "YE"
    }, {
        "en": "Yugoslavia",
        "Name": "南斯拉夫",
        "abbreviation": "YU"
    }, {
        "en": "Zimbabwe",
        "Name": "津巴布韦",
        "abbreviation": "ZW"
    }, {
        "en": "Zaire",
        "Name": "扎伊尔",
        "abbreviation": "ZR"
    }, {
        "en": "Zambia",
        "Name": "赞比亚",
        "abbreviation": "ZM"
    }];
    $.fn.originPlace = function (options, param) {
        function saveResult() {

        }

        function init(target) {
            var $input = $('<input />');
            var name = $(target).attr("name") + "_result";
            var valuer = $('<input type="hidden" name="' + name + '" />');//保存原厂地的input元素
            $(target).before(valuer);
            $(target).data("dom_result", valuer);//存储保存原产地的input元素
            $(target).data("data", originPlaceData);//存储保存原产地数据
            return $(target);
        }


        //如果options为string，则是方法调用，如$('#divMyPlugin').originPlace('sayoriginPlace');
        if (typeof options == 'string') {
            var method = $.fn.originPlace.methods[options];
            if (method) {
                return method(this, param);
            }
        }

        //否则是插件初始化函数，如$('#divMyPlugin').originPlace();
        options = options || {};
        return this.each(function () {
            var state = $.data(this, 'originPlace');
            if (state) {
                $.extend(state.options, options);
            } else {
                //easyui的parser会计算options
                state = $.data(this, 'originPlace', {
                    options: $.extend({}, $.fn.originPlace.defaults, $.fn.originPlace.parseOptions(this), options)
                })

                init(this);
            }
            $(this).combobox(state.options); //调用继承的combobox的构造方法
            
        })
    }

    //设置originPlace插件的一些方法的默认实现
    //注：第一个参数为当前元素对应的jQuery对象
    $.fn.originPlace.methods = {
        options: function(jq) {
            var opts = jq.combobox('options'); //获取combobox继承的options
            return $.extend($.data(jq[0], 'originPlace').options, { width: opts.width, height: opts.height, originalValue: opts.originalValue, disabled: opts.disabled, readonly: opts.readonly });
        }
    }

    //设置参数转换方法
    $.fn.originPlace.parseOptions = function (target) {
        var opts = $.extend({}, $.parser.parseOptions(target)); //这里可以指定参数类型
        return opts;
    }

    //设置originPlace插件的一些默认值
    $.fn.originPlace.defaults = {
        width: 260,
        data:originPlaceData,
        valueField: 'Name',
        textField: 'Name',
        prompt: '请选择原产地',
        panelMaxHeight: 300,
        required: true,
        missingMessage: '原产地不能为空',
        novalidate: true,
        tipPosition: 'right',
        value: null,
        validType: null,
        //onChange: null,
        //onSelect: null
    }

    //注册自定义easyui插件originPlace
    $.parser.plugins.push("originPlace");
})(jQuery)