//1.currency:币种1
//2.currency1:币种2
//3.Price:币种1的价格值
//4.Price1:币种2的价格值（未税，一般指CNY）
//5.TaxedPrice:币种2的价格值（含税，一般指CNY）
//初始化的时候：同时设置属性price,Price1,TaxedPrice,则Price生效;同时设置Price1,TaxedPrice,则Price1生效;单个设置单个生效。
//使用方法时：使用setPrice,setPrice1,不使用TaxedPrice,不管是哪种顺序setPrice生效;其他按顺序生效;单个设置单个生效。
//设置属性Price,使用setPrice方法，键盘输入Price，都会触发onEnter1事件
//设置属性Price1,使用setPrice1方法，键盘输入Price1，都会触发onEnter2事件
//设置属性TaxedPrice,使用setTaxedPrice方法，键盘输入TaxedPrice，都会触发onEnter3事件
//切换币种会触发onChangeCurrency事件
//onEnter1: function (data, currency, currency1,ERate1,VATRate, Price, Price1, TaxedPrice);//data:币种数据，ERate1汇率,Price报价,Price1本位币报价,VATRate增值税率,TaxedPrice已税价

(function ($) {
    //var currencyData = [{ "ID": 1, "Name": "人民币", "ShortName": "CNY", "Symbol": "CNY¥" }, { "ID": 2, "Name": "美元", "ShortName": "USD", "Symbol": "US$" }, { "ID": 3, "Name": "港元", "ShortName": "HKD", "Symbol": "HK$" }];
    var currencyData = null;//存储币种数据

    //接口url
    var AjaxUrl = {
        Currency: "/PvDataApi/Currencies",//获取币种数据的接口地址
        getRate: "/PvDataApi/ExchangeRates"//获取汇率(以及换算好的价格,传参不一样)
    };

    //获取币种数据
    function getCurrencies(cb) {
        $.ajax({
            type: "get",
            url: AjaxUrl.Currency,
            //dataType: "jsonp",
            success: function (data) {
                if (cb) {
                    //修改，不知道为什么接口中返回数据的问题
                    cb(data.data)
                    //cb(JSON.parse(data.data))
                }
            },
            error: function (err) {
                alert(JSON.stringify(err));
            }
        });
    }
    //根据发票类型获得增值税率
    function getVATRateByInvoiceType(data, Type) {
        var VATRate;
        for (var i = 0; i < data.length; i++) {
            if (data[i].Type == Type) {
                VATRate = data[i].VATRate;
                break;
            }
        }
        return VATRate;
    }
    //生成随机ID
    function randomID(model) {
        return model + '_' + parseInt(Math.random() * Math.pow(10, 10))
    }
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

    //将值转换为小数点后precision位
    function tofixedPrice(price, precision) {
        return parseFloat(price).toFixed(precision);
    }

    //设置两个变量为完成
    var isCompleted_PriceResult = true;
    var isCompleted_Rate = true;


    //获取换算结果以及汇率
    function getPriceResult(from, to, price, type, cb) {
        isCompleted_PriceResult = false;
        $.ajax({
            type: "get",
            url: AjaxUrl.getRate,
            //dataType: "jsonp",
            data: {
                from: from,
                to: to,
                price: price,
                type: type
            },
            success: function (data) {
                //isCompleted_PriceResult = true;
                cb(data.data);
            },
            error: function (err) {
                alert(JSON.stringify(err));
            },
            complete: function () {
                isCompleted_PriceResult = true;
            }
        });
    }

    //从接口中获取汇率
    function getRate(from, to, type, cb) {
        isCompleted_Rate = false;
        $.ajax({
            type: "get",
            url: AjaxUrl.getRate,
            //dataType: "jsonp",
            data: {
                from: from,
                to: to,
                type: type
            },
            success: function (data) {
                cb(data.data);
            },
            error: function (err) {
                alert(JSON.stringify(err));
            },
            complete: function () {
                isCompleted_Rate = true;
            }
        });
    }
    //获取汇率
    function doGetRate(sender) {
        var currency = $(sender).data("options").currency;
        var currency1 = $(sender).data("options").currency1;
        var rateType = $(sender).data("options").rateType;
        getRate(currency, currency1, rateType, function (data) {
            setAllRate(sender, data);
        })
    }
    //保存汇率值
    function setAllRate(sender, data) {
        saveAttr(sender, 'ERate1', data);
        setInputHiddenVal(sender, "dom_ERate1", data);
    }
    //保存增值税率
    function setVATRate(sender, data) {
        saveAttr(sender, 'VATRate', data);
        setInputHiddenVal(sender, "dom_VATRate", data);
    }
    //存储某个数据的值（sender:this即调用该插件的元素,AttrName:存储的某个数据的data名称，AttrValue:值）
    function saveAttr(sender, AttrName, AttrValue) {
        $(sender).data(AttrName, AttrValue);
    }
    //为隐藏的input的赋值（sender:this即调用该插件的元素,inputDom:隐藏的某个input，AttrValue:值）
    function setInputHiddenVal(sender, inputDom, AttrValue) {
        $(sender).data(inputDom).val(AttrValue);
    }
    //设置价格
    function setPriceVal(sender, priceAttr, value) {
        $(sender).data(priceAttr).numberbox("setValue", value);
    }

    function saveUntaxedPrice(sender, price) {
        saveAttr(sender, 'UntaxedPrice', price);
        setInputHiddenVal(sender, "dom_UntaxedPrice", price);
    }

    //设置options的价格值
    function setOptionprice(sender, p1, p2, p3) {
        $(sender).data("options").Price = p1;
        $(sender).data("options").Price1 = p2;
        $(sender).data("options").TaxedPrice = p3;
    }
    //保存价格值
    function savePriceVal(sender, p1, p2, p3) {
        saveAttr(sender, 'Price', p1);
        saveAttr(sender, 'Price1', p2);
        saveAttr(sender, 'TaxedPrice', p3);
    }
    //初始化传入的币种值
    function initCurrencyVal(data, sender, currencyAttr, domCurrencyAttr, currency) {
        if (currency && currency != "") {
            var j = 0;
            for (var i = 0; i < data.length; i++) {
                if (currency == data[i].ShortName) {
                    break;
                } else {
                    j++;
                }
            }
            if (j == data.length) {
                currency = data[0].ShortName;
            }
        } else {
            currency = data[0].ShortName;
        }
        saveAttr(sender, currencyAttr, currency);//存储币种值
        setInputHiddenVal(sender, domCurrencyAttr, currency);//存储币种值
        return currency;
    }

    //初始化价格
    function initPrice(sender, priceAttr, price) {
        if (!price || price == "") {
            price = "0.00000";
        }
        saveAttr(sender, priceAttr, price);
        return price;
    }

    //计算币种换算值
    function dosomething(sender, price, flag) {
        var VATRate = $(sender).data("VATRate");
        var options = $(sender).data("options");
        var currency = $(sender).data("options").currency;
        var currency1 = $(sender).data("options").currency1;
        var rateType = $(sender).data("options").rateType;
        getPriceResult(currency, currency1, digitalConversion(price), rateType, function (data) {
            setAllRate(sender, data.rate);
            //在keyup事件中暴露onEnter事件
            if (flag == 1 || flag == 3) {
                var PriceVal = parseFloat(price).toFixed(options.precision);
                setInputHiddenVal(sender, "dom_Price", PriceVal);
                setPriceVal(sender, 'dom_Price1', data.price);
                var TaxedPrice = ConvertToTaxPrice(options.invoiceType, data.price);
                setPriceVal(sender, 'dom_TaxedPrice', TaxedPrice);
                var TaxedPriceVal = $(sender).data("dom_TaxedPrice").numberbox('getValue');
                savePriceVal(sender, PriceVal, data.price, TaxedPriceVal);
                setOptionprice(sender, PriceVal, data.price, TaxedPriceVal);
                saveUntaxedPrice(sender, data.price);
                if (flag == 1 && options.onEnter1) {
                    options.onEnter1(currencyData, currency, options.currency1, data.rate, VATRate, PriceVal, data.price, TaxedPriceVal);
                } else if (flag == 3 && options.onChangeCurrency) {
                    options.onChangeCurrency(currencyData, currency, options.currency1, data.rate, VATRate, PriceVal, data.price, TaxedPriceVal);
                }
            }
            if (flag == 2) {
                var Price1Val = parseFloat(price).toFixed(options.precision);
                $(sender).numberbox("setValue", data.price);
                setInputHiddenVal(sender, "dom_Price", data.price);
                var TaxedPrice = ConvertToTaxPrice(options.invoiceType, digitalConversion(price));
                setPriceVal(sender, 'dom_TaxedPrice', TaxedPrice);
                var TaxedPriceVal = $(sender).data("dom_TaxedPrice").numberbox('getValue');

                savePriceVal(sender, data.price, Price1Val, TaxedPriceVal);
                setOptionprice(sender, data.price, Price1Val, TaxedPriceVal);
                saveUntaxedPrice(sender, Price1Val);

                if (options.onEnter2) {
                    options.onEnter2(currencyData, currency, options.currency1, data.rate, VATRate, data.price, Price1Val, TaxedPriceVal);
                }
            }
        })
    };

    //初始化计算价格
    function initComputPrice(sender, Price, Price1, TaxedPrice, currency) {
        if (currency == "CNY") {
            $(sender).data("dom_Price1").numberbox("readonly", false);
            $(sender).data("dom_TaxedPrice").numberbox("readonly", false);
        } else {
            $(sender).data("dom_Price1").numberbox("readonly", true);
            $(sender).data("dom_TaxedPrice").numberbox("readonly", true);
        }

        if (Price && Price != "" && Price != "0.00000") {
            dosomething(sender, Price, 1);
        } else if ((Price == "" || Price == "0.00000") && (Price1 && Price1 != "" && Price1 != "0.00000")) {
            dosomething(sender, Price1, 2);
        } else if ((Price == "" || Price == "0.00000") && (Price1 == "" || Price1 == "0.00000") && (TaxedPrice && TaxedPrice != "" && TaxedPrice != "0.00000")) {
            CurrencyConversion(sender, TaxedPrice);
        } else if ((Price == "" || Price == "0.00000") && (Price1 == "" || Price1 == "0.00000") && (TaxedPrice == "" && TaxedPrice == "0.00000")) {
            if (currency == "CNY") {
                saveAttr(sender, "ERate1", 1);
                setInputHiddenVal(sender, "dom_ERate1", 1);
            } else {
                doGetRate(sender);
            }
            setPriceVal(sender, 'dom_Price1', 0);
            setPriceVal(sender, 'dom_TaxedPrice', 0);
            setInputHiddenVal(sender, "dom_Price", "0.00000");
            savePriceVal(sender, "0.00000", "0.00000", "0.00000");
            setOptionprice(sender, "0.00000", "0.00000", "0.00000");
            saveUntaxedPrice(sender, "0.00000");
        }
    }

    //根据Price或Price1计算价格
    function computPriceByP1orP2(sender, price, currency, flag) {
        if (flag == 1 || flag == 3) {
            $(sender).data("options").Price = price;
        } else if (flag == 2) {
            $(sender).data("options").Price1 = price;
        }

        if (currency == "CNY") {
            $(sender).data("dom_Price1").numberbox("readonly", false);
            $(sender).data("dom_TaxedPrice").numberbox("readonly", false);
        } else {
            $(sender).data("dom_Price1").numberbox("readonly", true);
            $(sender).data("dom_TaxedPrice").numberbox("readonly", true);
        }
        if (price == "" || price == "0.00000") {
            if (currency == "CNY") {
                saveAttr(sender, "ERate1", 1);
                setInputHiddenVal(sender, "dom_ERate1", 1);
            } else {
                doGetRate(sender);
            }
            setPriceVal(sender, 'dom_Price1', 0);
            setPriceVal(sender, 'dom_TaxedPrice', 0);

            savePriceVal(sender, "0.00000", "0.00000", "0.00000");
            setOptionprice(sender, "0.00000", "0.00000", "0.00000");
            saveUntaxedPrice(sender, "0.00000");
            setInputHiddenVal(sender, "dom_Price", "0.00000");
        } else {
            dosomething(sender, price, flag);
        }
    }

    //初始化左边的下拉控件
    function initLeft(data, sender) {
        var options = $(sender).data("options");
        var taxPriceInput = $(sender).data("taxPriceInput");
        var Price = sender.val();
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

        //初始化币种
        options.currency = initCurrencyVal(data, sender, "currency", "dom_currency", options.currency);
        options.currency1 = initCurrencyVal(data, sender, "currency1", "dom_currency1", options.currency1);

        $(sender).data("options", options);
        var sender_handle = topper.menubutton({
            text: options.currency,
            CurrencyType: options.currency,
            menu: '#' + id
        });
        //切换选择不同的币种
        $(sender_handle.menubutton('options').menu).menu({
            onClick: function (item) {
                topper.menubutton({
                    text: item.CurrencyType,
                    CurrencyType: item.CurrencyType,
                    menu: '#' + id
                });
                saveAttr(sender, "currency", item.CurrencyType);
                setInputHiddenVal(sender, "dom_currency", item.CurrencyType);//存储币种值
                $(sender).data("options").currency = item.CurrencyType;
                var Price = sender.val();
                computPriceByP1orP2(sender, Price, item.CurrencyType, 3);
            }
        });
        sender.before(topper);
        var Price1 = $(sender).data("options").Price1;
        var TaxedPrice = $(sender).data("options").TaxedPrice;
        initComputPrice(sender, Price, Price1, TaxedPrice, options.currency, 1);
    };

    //设置未税和含税价格以及其他
    function setP2P3(sender, rate, Price, Price1, TaxedPrice, TaxedPriceVal, options) {
        var VATRate = $(sender).data("VATRate");
        $(sender).numberbox('setValue', Price);
        setPriceVal(sender, "dom_Price1", Price1);
        var PriceVal = $(sender).numberbox("getValue");
        var Price1Val = $(sender).data("dom_Price1").numberbox("getValue");
        savePriceVal(sender, PriceVal, Price1Val, TaxedPriceVal);
        setOptionprice(sender, PriceVal, Price1Val, TaxedPriceVal);
        saveUntaxedPrice(sender, Price1Val);
        setInputHiddenVal(sender, "dom_Price", PriceVal);
        if (options.onEnter3 && TaxedPrice != "" && TaxedPrice != "0.00000") {
            options.onEnter3(currencyData, options.currency, options.currency1, rate, VATRate, PriceVal, Price1Val, TaxedPriceVal);
        }
    }

    //含税换未税
    function CurrencyConversion(sender, TaxedPrice) {
        var Price;
        var Price1;
        var options = $(sender).data("options");
        var TaxedPriceVal = parseFloat(TaxedPrice).toFixed(options.precision);
        if (TaxedPrice == "" || TaxedPrice == "0.00000") {
            Price = "0.00000";
            Price1 = "0.00000";
            doGetRate(sender);
            setP2P3(sender, rate, Price, Price1, TaxedPrice, TaxedPriceVal, options);
        } else {
            Price1 = ConvertToPrice(options.invoiceType, TaxedPrice);
            if (options.currency == 'CNY') {
                setAllRate(sender, 1);
                setP2P3(sender, 1, Price1, Price1, TaxedPrice, TaxedPriceVal, options);
            } else {
                getPriceResult(options.currency, options.currency1, digitalConversion(Price1), oprions.rateType, function (data) {
                    setAllRate(sender, data.rate);
                    setP2P3(sender, data.rate, data.price, Price1, TaxedPrice, TaxedPriceVal, options);
                })
            }
        }
    }





    $(function () {
        //提交验证
        $('form').submit(function () {
            //alert(0000);
            var result = isCompleted_PriceResult && isCompleted_Rate;
            //alert(result);
            if (!result) {
                var $top = window.top.$ || $;

                //alert(1111);
                $top.timeouts.alert({
                    position: "TC",
                    timeout: 1500,
                    msg: '请稍作操作,正在请求汇率..',
                    type: 'warning'//success,error,info,warning
                });

                //alert(2222);
            }


            //alert(result);

            return result;
        });
    });

    //目标input
    $.fn.currency = function (opt, param) {

        //如果options为string，则是方法调用
        if (typeof opt == 'string') {
            var method = $.fn.currency.methods[opt];
            if (method) {
                return method(this, param);//如果有该方法就调用该方法
            } else {
                alert("该插件没有这个方法:" + opt)
            }
        }

        //情况一，在html中直接配置data-options
        var options = opt || {};
        var sender = this;
        sender.css({ 'text-align': 'right' });

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
        if ($(this).data('options')) {
            return
        }
        //初始化价格
        options.Price = initPrice(sender, "Price", options.Price);
        options.Price1 = initPrice(sender, "Price1", options.Price1);
        options.TaxedPrice = initPrice(sender, "TaxedPrice", options.TaxedPrice);

        //$(sender).data("options", options);

        var groupName = $(this).attr("name");//获取this的name值

        var dom_currency = $('<input type="hidden" name="' + groupName + '_currency" />');
        $(sender).before(dom_currency);
        $(sender).data('dom_currency', dom_currency);

        var dom_currency1 = $('<input type="hidden" name="' + groupName + '_currency1" />');
        $(sender).before(dom_currency1);
        $(sender).data('dom_currency1', dom_currency1);

        var dom_ERate1 = $('<input type="hidden" name="' + groupName + '_ERate1" />');
        $(sender).before(dom_ERate1);
        $(sender).data('dom_ERate1', dom_ERate1);

        var dom_TaxedPrice_span = $('<span style="margin:0 5px;">' + options.currency1 + '(含税)' + '</span>');
        var dom_TaxedPrice = $('<input name="' + groupName + '_TaxedPrice" />');
        $(sender).after(dom_TaxedPrice);
        $(sender).data('dom_TaxedPrice', dom_TaxedPrice);
        $(sender).data('dom_TaxedPrice').before(dom_TaxedPrice_span);

        var dom_Price1_span = $('<span style="margin:0 5px;">' + options.currency1 + '(未税)' + '</span>');
        var dom_Price1 = $('<input name="' + groupName + '_Price1" />');
        $(sender).after(dom_Price1);
        $(sender).data('dom_Price1', dom_Price1);
        $(sender).data('dom_Price1').before(dom_Price1_span);

        //未税价(等同于Price)
        var UntaxedPrice = options.Price1;
        var dom_UntaxedPrice = $('<input type="hidden" name="' + groupName + '_UntaxedPrice" />');
        dom_UntaxedPrice.val(UntaxedPrice);
        $(sender).before(dom_UntaxedPrice);
        $(sender).data('dom_UntaxedPrice', dom_UntaxedPrice);
        saveAttr(sender, 'UntaxedPrice', UntaxedPrice);

        //输入的价格(price)
        var PriceSave = options.Price;
        var dom_Price = $('<input type="hidden" name="' + groupName + '_Price" />');
        dom_Price.val(PriceSave);
        $(sender).before(dom_Price);
        $(sender).data('dom_Price', dom_Price);

        //初始化增值税率
        var VATRate = getVATRateByInvoiceType(options.invoiceDatas, options.invoiceType);
        var dom_VATRate = $('<input type="hidden" name="' + groupName + '_VATRate" />');
        dom_VATRate.val(VATRate);
        $(sender).before(dom_VATRate);
        $(sender).data('dom_VATRate', dom_VATRate);
        saveAttr(sender, 'VATRate', VATRate);

        //初始化价格插件
        var number_options = {
            currency: options.currency || "CNY",
            currency1: options.currency1 || "CNY",
            Price: options.Price || null,
            Price1: options.Price1 || null,
            TaxedPrice: options.TaxedPrice || null,
            precision: options.precision || 5,
            groupSeparator: options.groupSeparator || ',',
            width: options.width || 100,
            required: options.required || true,
            missingMessage: options.missingMessage || '币种值不能为空',
            novalidate: options.novalidate || true,
            tipPosition: options.tipPosition || 'bottom',
            prompt: options.prompt || "请输入价格",
            readonly: false
        }

        var number_options1 = number_options;
        var number_options2 = number_options;
        var number_options3 = number_options;

        number_options1.value = options.Price;
        sender.numberbox(number_options1);

        number_options2.value = options.Price1;
        number_options2.prompt = "请输入未税价格";
        if (options.currency != "CNY") {
            number_options2.readonly = true;
        }
        $(sender).data("dom_Price1").numberbox(number_options2);

        number_options3.value = options.TaxedPrice;
        number_options3.prompt = "请输入含税价格";
        if (options.currency != "CNY") {
            number_options3.readonly = true;
        }
        $(sender).data("dom_TaxedPrice").numberbox(number_options3);

        //判断如果有币种数据，则进行实例化左侧币种数据，如果没有币种数据，将调用改组件的dom元素push到initsItem

        if (currencyData) {
            if (currencyData && currencyData.length > 0) {
                initLeft(currencyData, sender);
            }
        } else {
            getCurrencies(function (data) {
                currencyData = data;
                if (currencyData.length > 0) {
                    initLeft(currencyData, sender);
                }
            })
        }
        function keyupFun1() {
            var price = $(this).val();
            computPriceByP1orP2(sender, price, $(sender).data("currency"), 1);
        }

        function keyupFun2() {
            var price = $(this).val();
            computPriceByP1orP2(sender, price, $(sender).data("currency"), 2);
        }
        function keyupFun3() {
            var TaxedPrice = $(this).val();
            CurrencyConversion(sender, TaxedPrice);

        }
        $(sender).numberbox('textbox').on("keyup", debounce(keyupFun1, 200));

        $(sender).data("dom_Price1").numberbox('textbox').on("keyup", debounce(keyupFun2, 200));

        $(sender).data("dom_TaxedPrice").numberbox('textbox').on("keyup", debounce(keyupFun3, 200));

        $(this).data('options', options);
    }

    //币种插件的默认配置
    $.fn.currency.defaults = {
        invoiceDatas: [        //税率数据
        {
            Type: 1,
            Name: "普票税率",
            VATRate: 1.03
        },
        {
            Type: 2,
            Name: "专票税率",
            VATRate: 1.13
        },
        {
            Type: 4,
            Name: "不开票税率",
            VATRate: 1
        }
        ],
        data: null,             //数据
        source_width: '150px',  //默认下拉里列表的宽度
        currency: "CNY",        //币种1
        currency1: "CNY",       //币种2
        invoiceType: 1,         //发票类型
        Price: null,           //币种1的价格值
        Price1: null,           //币种2的价格值（未税，一般指CNY）
        TaxedPrice: null,       //币种2的价格值（含税，一般指CNY）
        precision: 5,           //小数点后保存5位
        groupSeparator: ',',    //价格用,格式化
        width: 100,             //价格input框默认100px
        required: true,         //是否为必填
        missingMessage: '币种值不能为空',//input框为空的提示信息
        novalidate: true,       //为true时关闭验证功能。
        tipPosition: 'bottom',  //提示信息的位置
        onEnter1: null,         //币种1的价格输入时的事件
        onEnter2: null,         //币种2的价格输入时的事件
        onEnter3: null,         //币种3的价格输入时的事件
        onChangeCurrency: null, //切换币种事件
        rateType: 30 //汇率类型（默认固定汇率） 海关汇率：10，实时汇率/浮动汇率 20，固定汇率 30
    };

    //币种插件对外的方法
    $.fn.currency.methods = {
        //获取币种的所有值
        getCurrencyData: function () {
            return currencyData;
        },
        //获取币种的options
        options: function (jq) {
            return $(jq).data("options");
        },
        //获取原币种类型
        getCurrency: function (jq) {
            return $(jq).data("currency");
        },
        //获取现有币种
        getCurrency1: function (jq) {
            return $(jq).data("currency1")
        },
        //获取原价格
        getPrice: function (jq) {
            return $(jq).data("Price");
        },
        //设置原价格
        setPrice: function (jq, param) {
            $(jq).numberbox("setValue", param);
            computPriceByP1orP2(jq, param, $(jq).data("options").currency, 1);
        },
        //获取计算后的价格
        getPrice1: function (jq) {
            return $(jq).data("Price1");
        },
        //设置未税价格
        setPrice1: function (jq, param) {
            $(jq).data("dom_Price1").numberbox("setValue", param);
            computPriceByP1orP2(jq, param, $(jq).data("options").currency, 2);
        },
        //获取含税价格
        getTaxedPrice: function (jq) {
            return $(jq).data("TaxedPrice");
        },
        //设置含税价格
        setTaxedPrice: function (jq, param) {
            $(jq).data("dom_TaxedPrice").numberbox("setValue", param);
            CurrencyConversion(jq, param);
        },
        //获取含税价格
        getUntaxedPrice: function (jq) {
            return $(jq).data("UntaxedPrice");
        },
        //获取增值税率
        getVATRate: function (jq) {
            return $(jq).data("VATRate");
        },
        //获取汇率
        getERate1: function (jq) {
            return $(jq).data("ERate1");
        },
        //更改发票类型
        setInvoiceType: function (jq, param) {
            $(jq).data("options").invoiceType = param;
            var VATRate = getVATRateByInvoiceType($(jq).data("options").invoiceDatas, param);
            setVATRate(jq, VATRate);
            computPriceByP1orP2(jq, $(jq).data("Price"), $(jq).data("currency"), 1);
        },
        //获取所有的值
        getAllData: function (jq) {
            return {
                currencyData: currencyData,
                options: $(jq).data("options"),
                currency: $(jq).data("currency"),
                currency1: $(jq).data("currency1"),
                Price: $(jq).data("Price"),
                Price1: $(jq).data("Price1"),
                UntaxedPrice: $(jq).data("UntaxedPrice"),
                TaxedPrice: $(jq).data("TaxedPrice"),
                ERate1: $(jq).data("ERate1"),
                VATRate: $(jq).data("VATRate")
            }
        }
    };
    $.parser.plugins.push('currency'); //将自定义的插件加入 easyui 的插件组    
})(jQuery)