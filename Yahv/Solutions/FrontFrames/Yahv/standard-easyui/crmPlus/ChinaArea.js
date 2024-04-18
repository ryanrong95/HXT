// 多级联动城市
(function ($) {
    var randomID = function (model) {
        return model + '_' + parseInt(Math.random() * Math.pow(10, 10))
    }

    var ChinaArea = (function () {
        function ChinaArea(element, options) {
            this.element = element;
            var data_options = {};
            var n = $.trim(this.element.attr("data-options"));
            if (n) {
                if (n.substring(0, 1) != "{") {
                    n = "{" + n + "}";
                }
                data_options = (new Function("return " + n))();
                this.settings = $.extend(true, {}, $.fn.ChinaArea.defaults, data_options, options || {});
            } else {
                this.settings = $.extend(true, {}, $.fn.ChinaArea.defaults, options || {});
            }
            this.init();
        }
        ChinaArea.prototype = {
            // 公共方法(初始化dom结构，布局，分页及绑定事件)
            init: function () {
                var me = this;
                me.value = me.settings.value;
                me.limitToList = me.settings.limitToList;
                me.saveValue = [];
                me.isSetValue = false;
                if (me.value && me.value.length) {
                    me.isSetValue = true;
                }
                me.data = me.settings.data;
                me.readonly = me.settings.readonly;
                me.one = null;
                me.count = 0;
                me.initDom();
            },
            initDom: function () {
                var me = this;
                var $input_saveVal = $('<input type="hidden" name="' + me.element.attr('name') + '" />')
                me.inputSaveVal = $input_saveVal;
                me.element.append(me.inputSaveVal);
                var id = randomID('ChinalArea');
                var one = $('<input name="' + id + '" />')
                me.one = one
                me.element.append(me.one)

                me.one.combobox({
                    valueField: 'n',
                    textField: 'n',
                    data: me.data,
                    limitToList: me.limitToList,
                    readonly: me.readonly,
                    required: me.required,
                    onSelect: function (rec) {
                        me.saveValue = []
                        me.saveValue.push(rec.n)
                        var next=$(this).next('span').nextAll('input');
                        if (next) {
                            next.combobox('clear');
                            next.combobox('loadData', rec.s);
                        }
                        else {
                            me.createArea(rec.s, rec.s[0].n)
                        }
                        
                    },
                    onChange (n, v) {
                        if (!n) {
                            $(this).next('span').nextAll('input').combobox('destroy')
                            var arr2 = []
                            arr2.push(me.saveValue[0])
                            me.saveValue = arr2
                            me.inputSaveVal.val(me.saveValue);
                        } else {
                            if (me.saveValue.length == 0) {
                                me.saveValue = []
                                me.saveValue.push(n)
                                me.inputSaveVal.val(me.saveValue);
                            }
                        }
                    }
                })
                if (me.value && me.value.length) {
                    me.one.combobox('select', me.value[0])
                    if (me.value && me.value.length == 1) {
                        me.value = null;
                        me.count = 0;
                        me.isSetValue = false;
                    }
                }
                else {
                    me.one.combobox('select', me.data[0].n)
                }
            },
            createArea: function (data, val) {
                var me = this;
                var id = randomID('ChianArea')
                var el = $('<input id="' + id + '">');
                me.element.append(el);
                //var width2 = me.defaultWidth;
                //if (me.Width && me.Width.length && me.count > 0 && (me.Width.length - 1 >= me.count)) {
                //    width2 = me.Width[me.count];
                //}
                el.combobox({
                    valueField: 'n',
                    textField: 'n',
                    data: data,
                    //width: width2,
                    required: true,
                    readonly: me.readonly,
                    editable: false,
                    onLoadSuccess: function () {
                        var nextinput = $(this).next('span').nextAll('input');
                        if (data.length)
                        {
                            if (nextinput) {
                                me.createArea(data[0].s, data[0].n)
                            }
                            else {
                                nextinput.combobox('clear');
                                nextinput.combobox('loadData', data[0].s);
                                nextinput.combobox('setValue', data[0].s[0].n);
                            }
                        }
                        else {
                            $(this).next('span').nextAll('input').combobox('destroy')
                        }
                    },
                    onSelect: function (rec) {
                        var l = $(this).prevAll('.combobox-f').length
                        me.saveValue.splice(l)
                        me.saveValue.push(rec.n)
                        me.inputSaveVal.val(me.saveValue);
                        $(this).next('span').nextAll('input').combobox('destroy')
                        me.count += 1;
                        if (rec.s && me.isSetValue) {
                            if (me.count < me.value.length) {
                                if (me.count + 1 == me.value.length) {
                                    me.createArea(rec.s, me.value[me.count])
                                    me.value = null;
                                    me.count = 0;
                                    me.isSetValue = false;
                                } else {
                                    me.createArea(rec.s, me.value[me.count])
                                }
                            }
                        } else if (rec.s && !me.isSetValue) {
                            me.createArea(rec.s)
                        }
                    }
                })
                el.combobox('showPanel')
                if (me.value && me.value.length >= me.count) {
                    el.combobox('select', val)
                    el.combobox('hidePanel')
                }
                else
                {
                    el.combobox('select', data[0].n)
                    el.combobox('hidePanel')
                }
            },
            setValue: function (value) {
                var me = this
                me.settings.value = value;
                me.value = value
                me.count = 0
                if (me.value && me.value.length) {
                    me.isSetValue = true;
                    me.setValueFun()
                }
            },
            setValueFun: function () {
                var me = this
                me.one.next('span').nextAll('input').combobox('destroy')
                if (me.value[0] == me.one.combobox('getValue')) {
                    me.one.combobox('unselect', me.value[0])
                }
                me.one.combobox('select', me.value[0])

            },
            ChangeOptions: function (options) {
                if (options) {
                    this.element.html("");
                    this.settings = $.extend(true, {}, $.fn.ChinaArea.defaults, options || {});
                    this.init();
                }
            }

        }
        return ChinaArea;
    })()
    $.fn.ChinaArea = function (options, param) {
        if (typeof options == 'string') {
            var method = $.fn.ChinaArea.methods[options];
            if (method) {
                return method(this, param);//如果有该方法就调用该方法
            } else {
                alert("该插件没有这个方法")
            }
        }


        return this.each(function () {
            var me = $(this)
            instance = me.data('ChinaArea');
            if (!instance) {
                instance = new ChinaArea(me, options);
                me.data('ChinaArea', instance);

            }
        })
    }
    // 定义默认配置参数
    $.fn.ChinaArea.defaults = $.extend({}, $.fn.ChinaArea.defaults, {
        //Width: [150,150],
        data: ChinaAreaData,
        readonly: false,
        value: null,
        limitToList: true,
        required: true
    })
    $.fn.ChinaArea.methods = {
        getValue: function (jq) {
            return $(jq).data('ChinaArea').saveValue;
        },
        setValue: function (jq, param) {
            $(jq).data('ChinaArea').setValue(param);
        },
        ChangeOptions: function (jq, options)
        { $(jq).data('ChinaArea').ChangeOptions(options); }
    }
    $.parser.plugins.push('ChinaArea');//将自定义的插件加入 easyui 的插件组  
})(jQuery)