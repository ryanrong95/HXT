/// <reference path="../../jquery-easyui-1.7.6/jquery.min.js" />
/// <reference path="../../jquery-easyui-1.7.6/jquery.easyui.min.js" />
//大赢家品牌名称补齐插件（可以输入可以选择）

//1.把失去焦点的bug给除去
//2.要完成初始化数据判断，并完成验证（红色边框）
//3.所有空间的标准数据的引用一定要统一（当前我们的控件开发就是当前页面全局的）
//4.批量加载的时候，一定要完成全局统一验证（例如：加载400行数据，要做一次验证）

//$.fn.form.defaults

(function ($) {
    //接口地址
    var AjaxUrl = {
        getdyjBrand: '/PvDataApi/DyjData/Manufacturers' //大赢家品牌数据 ，陈翰提供。高效性我来保障！
    }

    $.extend($.fn.combobox.defaults.rules, {
        isEmpty: {
            validator: function (value) {
                return $.trim(value) != ''
            },
            message: '品牌不能为空'
        },
        isDyj: {
            // 就在获取数据的同时，保存展示过的数据并进行验证
            validator: function (value) {

                return $.inArray(value, showedItems) > -1;
            },
            message: '不是大赢家标准数据'
        }
    });
    //数组去重
    function uniqueArr(arr) {
        var hash = [];
        for (var i = 0; i < arr.length; i++) {
            if (hash.indexOf(arr[i]) == -1) {
                hash.push(arr[i]);
            }
        }
        return hash;
    }



    var showedItems = [];
    var defaultValues = [];

    //编写dyjBrand插件
    $.fn.dyjBrand = function (opt, param) {
        //如果options为string，则是方法调用
        if (typeof opt == 'string') {
            var method = $.fn.dyjBrand.methods[opt];
            if (method) {
                return method(this, param);//如果有该方法就调用该方法
            } else {
                alert("该插件没有这个方法")
            }
        }

        var options = opt || {};
        var sender = this;
        options = $.extend(true, {}, $.fn.dyjBrand.defaults, options);

        //如果已经初始化，就不需要再进行初始化了
        $(sender).data("options", options);
        var inputw;//组件初始化后input的宽度

        //从远程获取数据
        var myloader = function (param, success, error) {
            var q = param.q || '';
            if (q.length < 1) { return false }
            $.ajax({
                url: AjaxUrl.getdyjBrand + '?key=' + encodeURI(q),
                dataType: 'jsonp',
                jsonp: "callback",
                success: function (data) {
                    //console.log(data)
                    var str = ""
                    var items = $.map(data, function (item) {


                        //$[sender.prop('id') + 'showedItems']
                        //全局唯一
                        str += item.Name + ","
                        return {
                            IsAgent: item.IsAgent,
                            Name: item.Name
                        };
                    });
                    str = str.substring(0, str.length - 1)
                    showedItems = uniqueArr(str.split(","))
                    //showedItems=str.split(",")
                    //showedItems.push(item.Name);
                    //console.log(str)
                    //console.log(showedItems)
                    success(items);
                },
                error: function () {
                    error.apply(this, arguments);
                }
            });
        }
        //options.value = '';
        //defaultValues.push(options.value);

        //输入框失去焦点
        sender.blur(function () {
            var blurExist = $.inArray(sender.val(), showedItems);
            if (event.keyCode == 13) {
                if (blurExist == -1) {
                    sender.val() == "";
                    sender.css("border-color", "red");
                }
            }
        })

        //如果原有控件有val ，而且 option.vale ==null 需要把原有 val 做复制
        //options.value ,,   sender.val();
        if (sender.val().length > 0 && options.value == null) {
            options.value = sender.val();
            $(sender).dyjBrand("setVal", options.value);
            

        }

        options.loader = myloader;

        options.onLoadSuccess = function (data) {
            $(sender).combobox("enableValidation");
        }

        $(sender).combobox(options);
        $(sender).each(function () {

        });
        //统一获取，并验证
    }

    //标准品牌名称补齐插件的默认配置
    $.fn.dyjBrand.defaults = $.extend({}, $.fn.combobox.defaults, {
        value: null,
        width: 200,
        valueField: 'Name',
        textField: 'Name',
        required: true,
        prompt: '品牌',
        missingMessage: '品牌不能为空',
        panelMaxHeight: 300,
        loader: null,
        mode: 'remote',
        novalidate: false,
        tipPosition: 'right',
        onChange: undefined,
        onSelect: undefined,
        onDoChange: undefined,
        validType: ['isEmpty', 'isDyj']
    });
    //标准品牌名称补齐插件对外的方法
    $.fn.dyjBrand.methods = {
        //获取input框的值
        getVal: function (jq) {
            //var maps = showedItems.map(data, function (item) {
            //    return item.Name
            //});
            //var index = $.inArray($(jq).combobox("getValue"), maps);
            //return showedItems[index]; // { IsAgent: false,  Name: 'aaa' }
            return $(jq).combobox("getValue");
        },
        //设置input框的值
        setVal: function (jq, param) {
            if (param) {
                $.ajax({
                    url: AjaxUrl.getdyjBrand + '?key=' + encodeURI(param),
                    dataType: 'jsonp',
                    jsonp: "callback",
                    success: function (data) {
                        //console.log(data)
                        var str = ""
                        var items = $.map(data, function (item) {


                            //$[sender.prop('id') + 'showedItems']
                            //全局唯一
                            str += item.Name + ","
                            return {
                                IsAgent: item.IsAgent,
                                Name: item.Name
                            };
                        });
                        showedItems = uniqueArr(str.split(","));
                        if ($.inArray(param, showedItems) > -1) {
                            $(jq).combobox("loadData", showedItems);
                            $(jq).combobox("select", param);
                        }
                        else {
                            $(jq).combobox("setValue", param);
                        }
                    }
                });
            }


        },
        //获取标准品牌名称补齐options
        options: function (jq) {
            return $(jq).data('options');
        },
    };
    //将自定义的插件加入 easyui 的插件组
    $.parser.plugins.push('dyjBrand');

})(jQuery)