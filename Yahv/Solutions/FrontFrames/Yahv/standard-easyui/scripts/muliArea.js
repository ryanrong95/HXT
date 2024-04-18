// 多级联动城市
(function ($) {
    // 私有方法，由于在此闭包内所以无法访问到
    var randomID = function (model) {
        return model + '_' + parseInt(Math.random() * Math.pow(10, 10))
    }
   
    var MuliArea = (function () {
        function MuliArea(element, options) {
            this.element = element;
            var data_options = {};
            var s = $.trim(this.element.attr("data-options"));
            if (s) {
                if (s.substring(0, 1) != "{") {
                    s = "{" + s + "}";
                }
                data_options = (new Function("return " + s))();
                this.settings = $.extend(true, {}, $.fn.MuliArea.defaults, data_options, options || {});
            } else {
                this.settings = $.extend(true, {}, $.fn.MuliArea.defaults, options || {});
            }
            this.init();
        }
        MuliArea.prototype = {
            // 公共方法(初始化dom结构，布局，分页及绑定事件)
            init: function () {
                var me = this;
                //me.defaultWidth = 120;
                //me.Width = me.settings.Width;
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
                var id = randomID('muliArea');
                var one = $('<input name="' + id + '" />')
                me.one = one
                me.element.append(me.one)
                //var width = me.defaultWidth;
                //if (me.Width && me.Width.length) {
                //    width = me.Width[0];
                //}
                me.one.combobox({
                    valueField: 'n',
                    textField: 'n',
                    //width: width,
                    data: me.data,
                    limitToList: me.limitToList,
                    readonly: me.readonly,
                    required: me.required,
                    onSelect: function (rec) {
                        me.saveValue = []
                        me.saveValue.push(rec.n)
                        $(this).next('span').nextAll('input').combobox('destroy')
                        var arr2 = []
                        arr2.push(me.saveValue[0])
                        me.saveValue = arr2
                        me.inputSaveVal.val(me.saveValue);
                        if (rec.s) {
                            if (me.value && me.value.length > 1) {
                                me.count = 1
                                me.createArea(rec.s, me.value[1])
                            } else {
                                me.createArea(rec.s)
                            }
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
            },
            createArea: function (data, val) {
                var me = this;
                var id = randomID('muliArea')
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
                    onSelect: function (rec) {
                        var l = $(this).prevAll('.combobox-f').length
                        me.saveValue.splice(l)
                        me.saveValue.push(rec.n)
                        me.inputSaveVal.val(me.saveValue);
                        $(this).next('span').nextAll('input').combobox('destroy')
                        me.count += 1;
                        if (rec.s  && me.isSetValue) {
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
                    this.settings = $.extend(true, {}, $.fn.MuliArea.defaults, options || {});
                    this.init();
                }
            }

        }
        return MuliArea;
    })()
    $.fn.MuliArea = function (options, param) {
        if (typeof options == 'string') {
            var method = $.fn.MuliArea.methods[options];
            if (method) {
                return method(this, param);//如果有该方法就调用该方法
            } else {
                alert("该插件没有这个方法")
            }
        }
      

        return this.each(function () {
            var me = $(this)
            instance = me.data('MuliArea');
            if (!instance) {

                instance = new MuliArea(me, options);
                me.data('MuliArea', instance);

            }
        })
    }
    // 定义默认配置参数
    $.fn.MuliArea.defaults = $.extend({}, $.fn.MuliArea.defaults, {
        //Width: [150,150],
        data: multiAreaData,
        readonly: false,
        value: null,
        limitToList: true,
        required: true
    })
    $.fn.MuliArea.methods = {
        getValue: function (jq) {
            return $(jq).data('MuliArea').saveValue;
        },
        setValue: function (jq, param) {
            $(jq).data('MuliArea').setValue(param);
        },
        ChangeOptions: function (jq, options)
        { $(jq).data('MuliArea').ChangeOptions(options); }
    }
    $.parser.plugins.push('MuliArea');//将自定义的插件加入 easyui 的插件组  
})(jQuery)