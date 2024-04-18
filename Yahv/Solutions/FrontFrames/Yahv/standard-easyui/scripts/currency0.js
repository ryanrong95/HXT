//币种换算控件
//name              表示调用控件的dom的name属性
//有4个<input type="hidden">的input，以下它们的name值，这些值携带了响应的数据
//name_currency1    表示左侧原则的币种
//name_currency2    表示右侧原则的币种
//name              表示用户输入的价格(在币种为CNY的情况下表示未税价格)
//name_price2       表示币种兑换后的价格
//name_taxPrice     表示含税人民币价格
//name_rate         表示币种的汇率


//1.onEnter在不是人民币的情况下，第一个输入框按下或者失去焦点触发onEnter
//onEnter: function (data, rate, price1, price2, currency1, currency2) {
//    console.log(data);//所有币种列表
//    console.log(rate);//汇率
//    console.log(price1);//输入的价格
//    console.log(price2);//换算后的价格
//    console.log(currency1);//原币种
//    console.log(currency2);//目标币种
//},
//2.onEnter1在是人民币的情况下，第一个输入框按下或者失去焦点触发onEnter1
//onEnter1: function (currencyData, price1, currency1) {
//    console.log(currencyData)//所有币种列表
//    console.log(price1)//不含税的价格
//    console.log(currency1)//原币种（即CNY）
//这里可拿着price1去计算含税的值
//    return 1;返回含税的值
//},
//3.onEnter2在是人民币的情况下，第二个输入框按下或者失去焦点触发onEnter2
//onEnter2: function (currencyData, price3, currency1) {
//    console.log(currencyData)//所有币种列表
//    console.log(price3)//含税的价格
//    console.log(currency1//原币种（即CNY）
//这里可拿着price3去计算未税的值
//    return 2;返回未税的值
//}
//在切换到人民币的情况下，触发onChangeCurrency事件
//4.onChangeCurrency: function (price1, currency1) {
//    console.log(price1)//不含税的价格
//    console.log(currency1)//原币种（即CNY）
//这里可拿着price1去计算含税的值
//    return 3;返回含税的值
//}

(function ($) {
    //接口url
    var AjaxUrl = {
        Currency: "/csrmapi/Currency",//获取币种数据的接口地址
        computedprice: ajaxPrexUrl + "/api/data/exchangerate",//获取汇率以及换算好的价格
        getRate: ajaxPrexUrl + "/api/data/exchangerate"//获取汇率
    };

    //生成随机ID
    function randomID(model) {
        return model + '_' + parseInt(Math.random() * Math.pow(10, 10))
    }

    var currencyData = null;//存储币种数据
    var initsItem = [];//存储调用改组件的dom元素

    //去掉价格的","
    function digitalConversion(num) {
        var num = num + '';
        if (num.indexOf(',') > 0) {
            num = num.replace(/,/g, '');
            return num;
        }
        else {
            return num;
        }

    }

    //获取币种数据
    (function () {
        $.ajax({
            type: "get",
            url: AjaxUrl.Currency,
            dataType: "jsonp",
            success: function (data) {
                currencyData = data;
                for (var index = 0; index < initsItem.length; index++) {
                    initLeft(currencyData, initsItem[index])
                };
            },
            error: function (err) {
                console.log(err);
            }
        });
    })();

    //获取换算结果以及汇率
    function getPriceResult(from, to, price, cb, options_target) {
        $.ajax({
            type: "get",
            url: AjaxUrl.computedprice,
            dataType: "jsonp",
            data: {
                from: from,
                to: to,
                price: price
            },
            success: function (data) {
                $(options_target).text(data.Price);
                cb(data);
            },
            error: function () {
                alert('fail');
            }
        });
    }

    //获取汇率
    function getRate(from, to, cb) {
        $.ajax({
            type: "get",
            url: AjaxUrl.getRate,
            dataType: "jsonp",
            data: {
                from: from,
                to: to
            },
            success: function (data) {
                cb(data);
            },
            error: function () {
                alert('获取汇率失败');
            }
        });
    }

    //计算币种换算值
    function dosomething(sender, price) {
        savePrice1(sender, price);
        var currency = $(sender).data("currency1");
        var options = $(sender).data("options");
        getPriceResult(currency, options.currency2, digitalConversion(price), function (data) {
            saveRate(sender, data.Rate);
            savePrice2(sender, data.Price);
            //在keyup事件中暴露onEnter事件
            if (options.onEnter) {
                options.onEnter(currencyData, data.Rate, digitalConversion(price), data.Price, currency, options.currency2)
            }
        }, options.target)
    };

    //获取汇率
    function doGetRate(sender) {
        var currency = $(sender).data("currency1");
        var options = $(sender).data("options");
        getRate(currency, options.currency2, function (data) {
            saveRate(sender, data);
        })
    }

    //存储原币种类型
    function saveCurrency1(sender, currency1) {
        $(sender).data('currency1', currency1);
        var currency1Name = sender.next().find('[name]').prop('name') + '_currency1';
        $("input[name='" + currency1Name + "']").val(currency1);
    }

    //存储目的币种类型
    function saveCurrency2(sender, currency2) {
        $(sender).data('currency2', currency2);
        var currency2Name = sender.next().find('[name]').prop('name') + '_currency2';
        $("input[name='" + currency2Name + "']").val(currency2);
    }

    //存储原价格
    function savePrice1(sender, price1) {
        $(sender).data('price1', price1);
    }

    //存储计算好的价格
    function savePrice2(sender, price2) {
        $(sender).data('price2', price2);
        var price2Name = sender.next().find('[name]').prop('name') + '_price2';
        $("input[name='" + price2Name + "']").val(price2);
    }

    //存储含税价格
    function savePrice3(sender, price3) {
        $(sender).data('price3', price3);
    }

    //设置未税价格
    function setFormatPrice1(sender, price1) {
        $(sender).numberbox('setValue', price1);
    }
    //设置换算后的价格
    function setFormatPrice2(sender, price2) {
        var options = $(sender).data('options');
        if (options.target) {
            $(options.target).text(price2);
        }
    }

    //设置含税价格
    function setFormatPrice3(sender, price3) {
        $(sender).data("taxPriceInput").numberbox('setValue', price3);
    }
    //存储汇率
    function saveRate(sender, rate) {
        $(sender).data('rate', rate);
        var rateName = sender.next().find('[name]').prop('name') + '_rate';
        $("input[name='" + rateName + "']").val(rate);
    }

    //初始化左边的下拉控件
    function initLeft(data, caller) {
        var options = $(caller).data("options");
        var taxPriceInput = $(caller).data("taxPriceInput");

        var topper = $('<a href="javascript:void(0)" style="width:58px;"></>');
        var id = randomID('topper');
        var content = $('<div id="' + id + '" style="width:150px;"></div>');

        $.each(data, function (index, source) {
            var item = $('<div></div>')
            item.attr('data-options', "CurrencyType:'" + source.ShortName + "',textIcon:'" + source.ShortName + "'")
            item.html(source.Name + "(" + source.ShortName + ")");
            content.append(item);
        });
        content.addClass("currency-noIcon")
        $(document.body).append(content);

        var currencyName;
        var j = 0;
        if (options.currency1 && options.currency1 != "") {
            currencyName = options.currency1;
            if (data.length > 0) {
                for (var i = 0; i < data.length; i++) {
                    if (currencyName == data[i].ShortName) {
                        break;
                    } else {
                        j++;
                    }
                }
                if (j == data.length) {
                    currencyName = data[0].ShortName;
                }
            }
        } else {
            currencyName = data[0].ShortName;
        }
        var sender_handle = topper.menubutton({
            text: currencyName,
            CurrencyType: currencyName,
            menu: '#' + id
        });

        var groupName = caller.next().find('[name]').prop('name');

        var name1 = groupName + '_currency1';
        var valuer1 = $('<input type="hidden" name="' + name1 + '" />');
        caller.before(valuer1);

        var name2 = groupName + '_currency2';
        var valuer2 = $('<input type="hidden" name="' + name2 + '" />');
        caller.before(valuer2);

        var name3 = groupName + '_price2';
        var valuer3 = $('<input type="hidden" name="' + name3 + '" />');
        caller.before(valuer3);

        var name4 = groupName + '_rate';
        var valuer4 = $('<input type="hidden" name="' + name4 + '" />');
        caller.before(valuer4);

        saveCurrency1(caller, currencyName);
        saveCurrency2(caller, options.currency2);
        savePrice1(caller, caller.val());

        //切换选择不同的币种
        $(sender_handle.menubutton('options').menu).menu({
            onClick: function (item) {
                topper.menubutton({
                    text: item.CurrencyType,
                    CurrencyType: item.CurrencyType,
                    menu: '#' + id
                });

                var price1 = caller.val();
                saveCurrency1(caller, item.CurrencyType);
                showInputAndTarget(caller);
                if (price1 == "" || price1 == "0.00000") {
                    if (item.CurrencyType == "CNY") {
                        saveRate(caller, 1);
                    } else {
                        if (options.target) {
                            doGetRate(caller);
                            $(options.target).text("0.00000");
                        }
                    }
                    savePrice2(caller, 0);
                    savePrice3(caller, 0);
                    setFormatPrice3(caller, 0);
                } else {
                    if (item.CurrencyType != "CNY") {
                        dosomething(caller, price1);
                    } else {
                        saveRate(caller, 1);
                        savePrice2(caller, price1);
                        if (options.onChangeCurrency) {
                            var price3 = options.onChangeCurrency(digitalConversion(price1), item.CurrencyType);
                            savePrice3(caller, price3);
                            setFormatPrice3(caller, price3);
                        }
                    }
                }
            }
        });
        caller.before(topper);
        //初始化结束后获取汇率
        doGetRate(caller);
    };

    function CNYshow(sender) {
        var price1 = $(sender).numberbox("getValue");
        var options = $(sender).data("options");
        savePrice1(sender, price1);
        savePrice2(sender, price1);
        if (options.onEnter1) {
            var price3 = options.onEnter1(currencyData, digitalConversion(price1), $(sender).data("currency1"));
            savePrice3(sender, price3);
            setFormatPrice3(sender, price3);
        }
    }
    
    //根据币种展示不同的ui
    function showInputAndTarget(sender) {
        var options = $(sender).data("options");
        var currency = $(sender).data("currency1");
        var taxPriceInput = $(sender).data("taxPriceInput");
        if (currency != "CNY") {
            savePrice3(sender, 0);
            setFormatPrice3(sender, 0);
            $(sender).data("taxPriceInput").next().css("display", "none");
            $($(sender).data("options").target).parent().show();
            sender.numberbox({ prompt: "请输入价格" });
            function keyupFun3() {
                var price1 = $(this).val();
                if (price1 != '') {
                    dosomething(sender, digitalConversion(price1));
                    savePrice1(sender, price1);
                } else {
                    savePrice2(sender, 0);
                    savePrice1(sender, 0);
                    setFormatPrice2(sender, "0.00000");
                }
            }
            sender.numberbox('textbox').on("keyup", debounce(keyupFun3, 200));
        } else {
            $(sender).data("taxPriceInput").next().css("display", "inline-block");
            $($(sender).data("options").target).parent().hide();
            sender.numberbox({ prompt: "请输入未税价格" });
            function keyupFun4() {
                var price1 = $(this).val();
                if (price1 != '') {
                    savePrice1(sender, price1);
                    savePrice2(sender, price1);
                    if (options.onEnter1) {
                        var price3 = options.onEnter1(currencyData, digitalConversion(price1), $(sender).data("currency1"));
                        savePrice3(sender, price3);
                        setFormatPrice3(sender, $(sender).data("price3"));
                    }
                } else {
                    savePrice2(sender, 0);
                    savePrice1(sender, 0);
                    savePrice3(sender, 0);
                    setFormatPrice2(sender, "0.00000");
                    setFormatPrice3(sender, 0);
                }
            }
            sender.numberbox('textbox').on("keyup", debounce(keyupFun4, 200));
        }
        sender.numberbox('textbox').on("blur", function () {
            var price1 = $(this).val();
            if (price1 == "请输入未税价格" || price1 == "请输入价格") {
                setFormatPrice1(sender, 0);
            }
        });
    }

    //目标input
    $.fn.currency = function (opt, param) {
        //如果options为string，则是方法调用
        if (typeof opt == 'string') {
            var method = $.fn.currency.methods[opt];
            if (method) {
                return method(this, param);//如果有该方法就调用该方法
            } else {
                alert("该插件没有这个方法")
            }
        }

        //情况一，在html中直接配置data-options
        var options = opt || {};
        var sender = this;
        sender.css({ 'text-align': 'right' });

        //html配置
        (function () {
            var data_options = {};
            var s = $.trim(sender.attr("data-options"));
            if (s) {
                if (s.substring(0, 1) != "{") {
                    s = "{" + s + "}";
                }
                data_options = (new Function("return " + s))();
                options = $.extend(true, {}, $.fn.currency.defaults, data_options, options);
            } else {
                options = $.extend(true, {}, $.fn.currency.defaults, options);
            }
        })();

        var OriginalVal = $(this).val();

        //存储options
        $(this).data('options', options);
        //存储目标币种ID
        $(this).data('currency1', options.currency1);
        $(this).data('currency2', options.currency2);

        var groupName = $(this).attr("name");
        var number_options = {
            precision: options.precision || 5,
            groupSeparator: options.groupSeparator || ',',
            width: options.width || 150,
            required: true,
            missingMessage: '币种值不能为空',
            novalidate: true,
            tipPosition: 'bottom',
            prompt: "请输入价格"
        }

        //实例化插件
        sender.numberbox(number_options);

        //判断如果有币种数据，则进行实例化左侧币种数据，如果没有币种数据，将调用改组件的dom元素push到initsItem
        if (currencyData) {
            initLeft(currencyData, sender);
        } else {
            initsItem.push(sender);
        }

        //含税的输入框
        function keyupFun2() {
            var price3 = $(this).val();
            if (price3 != "") {
                savePrice3(sender, price3);
                if (options.onEnter2) {
                    var price1 = options.onEnter2(currencyData, digitalConversion(price3), $(sender).data("currency1"));
                    savePrice1(sender, price1);
                    savePrice2(sender, price1);
                    setFormatPrice1(sender, $(sender).data("price1"));
                }
            } else {
                savePrice2(sender, 0);
                savePrice1(sender, 0);
                savePrice3(sender, 0);
                setFormatPrice2(sender, "0.00000");
                setFormatPrice1(sender, 0);
            }
        }
        var taxPriceName = groupName + '_taxPrice';
        var taxPriceInput = $("<input name='" + taxPriceName + "'/>");
        $(sender).next().after(taxPriceInput);
        $(sender).data("taxPriceInput", taxPriceInput);
        var number_options2 = number_options;
        number_options2.prompt = "请输入含税价格";
        taxPriceInput.numberbox(number_options2);
        $(sender).data("taxPriceInput").next().css("margin-left", "5px");

        $(taxPriceInput).numberbox('textbox').on("keyup", debounce(keyupFun2, 200));
        $(taxPriceInput).numberbox('textbox').on("blur", function () {
            var price1 = $(this).val();
            if (price1 == "请输入含税价格") {
                setFormatPrice3(sender, 0);
            }
        });

        showInputAndTarget(sender);

        if (options.price1 && options.price1 != "") {
            $(sender).currency("setPrice", options.price1);
        }

        if (options.price3 && options.price3 != "") {
            $(sender).currency("setPrice3", options.price3);
        }
    }

    //币种插件的默认配置
    $.fn.currency.defaults = {
        data: null,//数据
        source_width: '150px',//默认下拉里列表的宽度
        url: null, //后续jsonp
        target: null,//插件对应的目标，（jquery select）jquery选择器
        currency1: "CNY",
        currency2: null,
        price1: null,
        price3: null,
        precision: 5,
        groupSeparator: ',',
        width: 150,
        required: true,
        missingMessage: '币种值不能为空',
        novalidate: true,
        tipPosition: 'bottom',
        onEnter: null,
        onEnter1: null,
        onEnter2: null,
        onChangeCurrency: null
    };

    //币种插件对外的方法
    $.fn.currency.methods = {
        getCurrencyData: function () {
            return currencyData;
        },
        //获取币种的options
        options: function (jq) {
            return $(jq).data("options");
        },
        //获取原币种类型
        getCurrency1: function (jq) {
            return $(jq).data("currency1");
        },
        //获取现有币种
        getCurrency2: function (jq) {
            return $(jq).data("currency2")
        },
        //获取原价格
        getPrice1: function (jq) {
            return $(jq).data("price1");
        },
        //设置原价格
        setPrice: function (jq, param) {
            savePrice1(jq, param);
            setFormatPrice1(jq, param);
            savePrice3(jq, param);
            setFormatPrice3(jq, param);
            if ($(jq).data("currency1") == "CNY") {
                CNYshow(jq);
            } else {
                dosomething(jq, digitalConversion(param));
            }
        },
        //获取计算后的价格
        getPrice2: function (jq) {
            return $(jq).data("price2");
        },
        //获取含税价格
        getPrice3: function (jq) {
            return $(jq).data("price3");
        },
        //设置含税价格
        setPrice3: function (jq, param) {
            savePrice3(jq, param);
            setFormatPrice3(jq, param);
        },
        //获取汇率
        getRate: function (jq) {
            return $(jq).data("rate");
        },
        //获取所有的值
        getAllData: function (jq) {
            return {
                currencyData: currencyData,
                options: $(jq).data("options"),
                currency1: $(jq).data("currency1"),
                currency2: $(jq).data("currency2"),
                price1: $(jq).data("price1"),
                price2: $(jq).data("price2"),
                price3: $(jq).data("price3"),
                rate: $(jq).data("rate")
            }
        }
    };

    $.parser.plugins.push('currency'); //将自定义的插件加入 easyui 的插件组    
})(jQuery)