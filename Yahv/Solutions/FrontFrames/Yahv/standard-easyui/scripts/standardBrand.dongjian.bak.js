
(function ($) {
    $.extend($.fn.combobox.defaults.rules, {
        isNull: {
            validator: function (value) {
                return $.trim(value) != ''
            },
            message: '品牌不能为空'
        },
    });
    //接口地址
    var AjaxUrl = {
        getStandardBrand: '/csrmapi/Manufacturers?key=',
        isStandardBrand: '/csrmapi/Manufacturers/Validate?key='
    }
    //ajax获取品牌数据
    function GetStandardBrand(Name, cb) {
        $.ajax({
            type: "get",
            url: AjaxUrl.getStandardBrand + Name,
            dataType: "jsonp",
            success: function (data) {
                if (data.Code == "200") {
                    cb(data.Data);
                } else if (data.Code == "300") {
                    console.log("接口异常")
                }
            },
            error: function (err) {
                console.log(err);
            }
        });
    }
    //ajax标准品牌验证
    function IsStandardBrand(Name, cb) {
        $.ajax({
            type: "get",
            url: AjaxUrl.isStandardBrand + Name,
            dataType: "jsonp",
            success: function (data) {
                cb(data);
            },
            error: function (err) {
                console.log(err);
            }
        });
    }
    //设置边框颜色
    function setBorder(sender, color) {
        $(sender).next().css("border-color", color);
    }
    //设置背景提示图片
    function setBgImg(sender, className, title) {
        $em = $("<em title=" + title + ">")
        $em.addClass(className);
        $em.css({ "margin-right": "25px", "right": "0px", "position": "absolute" });
        $(sender).next().append($em);
    }
    //标准品牌验证结果处理
    function verifyHandle(sender, Name) {
        IsStandardBrand(Name,
            function (data) {
                if (data.Standard == "bug") {
                    setBorder(sender, "#ffc107")//品牌不是标准可补齐（黄色）
                } else if (data.Standard == "no") {
                    setBorder(sender, "#FF9800")//品牌不存在（橙色）
                } else if (data.Standard == "yes") {
                    setBorder(sender, "#D3D3D3")//品牌存在
                }
                //代理品牌（有水印）
                if (data.IsAgent == "True") {
                    setBgImg(sender, "icon-IsAgent", "代理品牌");
                }
                else {
                    $(sender).next().find("em").remove();
                }
            })
    }
    //编写standardBrand插件
    $.fn.standardBrand = function (opt, param) {
        //如果options为string，则是方法调用
        if (typeof opt == 'string') {
            var method = $.fn.standardBrand.methods[opt];
            if (method) {
                return method(this, param);//如果有该方法就调用该方法
            } else {
                alert("该插件没有这个方法:" + opt)
            }
        }
        var options = opt || {};
        options = $.extend(true, {}, $.fn.standardBrand.defaults, options);

        return this.each(function () {
            var sender = $(this);
            //保存设置类型
            sender.data('options', options);
            //设置控件id
            var sender_id = sender.prop('id');
            if (!sender_id) {
                sender_id = 'standardBrand_sender_' + Math.random().toString().substring(2);
                sender.prop('id', sender_id);
            }
            //设置form返回值
            sender.data('brandName', options.value);

            //首次默认值查询
            var firstQuery = options.value;
            var value = sender.val();
            if (value) {
                firstQuery = options.value = value;
            }
            var initdata = [];
            if (firstQuery) {
                q = firstQuery;
                firstQuery = null;
                //数据查询
                $.ajax({
                    type: "get",
                    url: AjaxUrl.getStandardBrand + q,
                    dataType: "jsonp",
                    success: function (data) {
                        if (data.Code == "200") {
                            var items = $.map(data.Data, function (item, index) {
                                return {
                                    value: item.Name,
                                    text: item.Name,
                                    Agent: item.Agent,
                                };
                            });
                            initdata = items;
                        }
                    },
                    error: function (err) {
                        alert('error:' + JSON.stringify(err));
                    }
                });
                //初始value品牌验证
                verifyHandle(sender, q);
            }

            var setTimeoutAjax = 0;
            sender.combobox({
                loader: function (param, success, error) {
                    var q = $.trim(sender.combobox('getText'));
                    q = $.trim(q);
                    if (q.length <= 1) { return false; }
                    clearTimeout(setTimeoutAjax);
                    setTimeoutAjax = setTimeout(function () {
                        $.ajax({
                            type: "get",
                            url: AjaxUrl.getStandardBrand + q,
                            dataType: "jsonp",
                            success: function (data) {
                                if (data.Code == "200") {
                                    var items = $.map(data.Data, function (item, index) {
                                        return {
                                            value: item.Name,
                                            text: item.Name,
                                            Agent: item.Agent,
                                        };
                                    });
                                    sender.data('items', items);
                                    success(items);
                                } else if (data.Code == "300") {
                                    console.log("接口异常:300");
                                }
                            },
                            error: function (err) {
                                alert('error:' + JSON.stringify(err));
                            }
                        });
                    }, 100);
                },
                value: options.value,
                valueField: 'value',
                textField: 'text',
                required: options.required,
                mode: 'remote',
                width: options.width,
                prompt: options.prompt,
                icons: options.icons,
                onSelect: function (record) {
                    var oldValue = sender.data('brandName');
                    sender.data('brandName', record.text);
                    ////标准品牌验证结果
                    //verifySelectHandle($(this), record.text)
                },
                onChange: function (data) {
                    var oldValue = sender.data('brandName');
                    sender.data('brandName', data);
                    //标准品牌验证结果
                    verifyHandle(sender, data);
                },
                data: initdata,
            });
        });
    };
    //标准品牌名称补齐插件的默认配置
    $.fn.standardBrand.defaults = $.extend({}, $.fn.combobox.defaults, {
        value: null,
        width: 200,
        required: true,
        prompt: '请输入三位以上搜索品牌',
        missingMessage: '品牌不能为空',
        panelMaxHeight: 300,
        loader: null,
        mode: 'remote',
        novalidate: true,
        tipPosition: 'right',
        onChange: null,
        onSelect: null,
        onDoChange: null,
        validType: 'isNull',
        icons: null,
    });
    //标准品牌名称补齐插件对外的方法
    $.fn.standardBrand.methods = {
        //获取input框的值
        getVal: function (jq) {
            var sender = $(jq);
            return sender.data('brandName');
        },
        //设置input框的值
        setVal: function (jq, param) {
            var sender = $(jq);
            sender.data('brandName', param);
        },
        //获取标准品牌名称补齐options
        options: function (jq) {
            var sender = $(jq);
            return sender.data('options');
        },
    };
    //将自定义的插件加入 easyui 的插件组
    $.parser.plugins.push('standardBrand');
})(jQuery)