/*
属性说明:
url  加载远程数据的地址,如果没有设置data和url,则加载area.data.js数据
data 控件绑定的数据, 如果设置了url,则data属性无效
textField 自定义显示字段
valueField 自定义值字段
sonsField 下级数据字段
country 设置的国家值,如果设置了url或data,该属性无效
label 控件标签文字
labelAlign 控件标签文字对其center left right
labelWidth 控件标签宽度


增加属性:
20181029 detailBox,boolean,默认值:true,输入详细地址框显示
         method,值为get和post,默认post,当设置url时从远程获取数据的http方法
20181113 disabled boolean 设置启用/禁用字段。 false 
         readonly boolean 设置该字段为读写/只读模式。
20181212 newline boolean 设置详细地址是否换行 
         newlinewidth int 设置详细地址宽度 
20190408 width string 设置控件宽度,默认不设置,按照easyui控件的默认大小显示.示例:data-options="width:'80px'"或data-options="width:'80%'"
方法说明:
setValue 参数示例:a b c c,空格隔开的字符串, 设置控件值
getValue 无参,获取控件值
options 获取所有属性

示例:
    <script>
        $(function () {
            var value = $('#area1')
                .area({ label: '测试', country: '中国' })
                .area('setValue', '内蒙古 鄂尔多斯 鄂托克旗 sadfaf')
                .area('getValue');;
        });
    </script>
    <input id="area1" /><br />
    <input class="easyui-area" data-options="label: '测试2',url:'**.json',textField:'n',valueField:'v',sonsField:'s',value:'02 021 asdfadf'" /><br />
    <input class="easyui-area" data-options="label: '测试2',data:[{n: '北京', v: '01', s: [{n: '海淀', v: '011'}]}, {n: '上海', v: '02', s: [{n: '外滩', v: '021'}]}],textField:'n',valueField:'v',sonsField:'s',value:'01 011 787979'" /><br />
    <input class="easyui-area" data-options="label: '测试2',country: '美国'" value="阿拉巴马 蒙哥马利 啊手动阀" />
*/
(function ($) {
    var arry1 = ['选择国家'], arry2 = ['选择省', '选择市区', '选择县'];
    function addCombobox(index, data, prompts, op) {
        var self = this;
        var comboboxOption = {
            textField: op.textField,
            valueField: op.valueField,
            sonsField: op.sonsField,
            prompt: prompts[index],
            disabled: op.disabled,
            readonly: op.readonly,
            onSelect: function (selectValue) {
                self.children('[index]:gt(' + $(this).attr('index') + '),br,i').each(function () {
                    if (this.localName == 'br' || this.localName == 'i') {
                        $(this).remove();
                    } else {
                        $(this).textbox('destroy');
                    }
                });
                if (selectValue != undefined && selectValue[comboboxOption.sonsField]) {
                    addCombobox.call(self, index + 1, selectValue[comboboxOption.sonsField], prompts, op);
                } else if (op.detailBox) {
                    addTextBox.call(self, index + 1, { readonly: op.readonly, disabled: op.disabled, newline: op.newline, newlinewidth: op.newlinewidth, validType: op.validType, resetWidth: !!op.width });
                }
            },
            onChange: function () {
                changeValue.call(self);
            }
        };
        if (typeof data === 'string') {
            comboboxOption.url = data;
            comboboxOption.method = op.method;
            comboboxOption.onLoadSuccess = function () {
                setValue.call(self, (self.parent().prev().area('options').value || '').split(' '));
            }
        } else {
            comboboxOption.data = data;
        }
        $('<input>').attr({
            index: index
        }).appendTo(self).combobox(comboboxOption);
        op.width && resetWidth(self, op.newline);
    };
    function resetWidth(span, newline) {
        var label = span.find('label.textbox-label:eq(0)');
        var elements = !newline ? span.find('.textbox-f') : span.find('combobox-f');
        var elementWidth = (span.width() - label.outerWidth()) / elements.length;
        elements.textbox({ width: elementWidth });
        span.find('span').css({ margin: 0 });
    }
    function addTextBox(index, op) {
        var self = this;
        var brcss = '';
        if (op.newline == true) {
            self.append('<br/><i style="height:3px;display:block;">');
            brcss += !op.newlinewidth ? ' style="width:100%;" ' : ' style="width:' + op.newlinewidth + 'px;" ';
        }
        $('<input ' + brcss + '>').attr({
            index: index
        }).appendTo(self).textbox({
            onChange: function () {
                changeValue.call(self);
            },
            readonly: op.readonly,
            disabled: op.disabled,
            validType: op.validType,
        });
        op.resetWidth && resetWidth(self);
    };
    function changeValue() {
        this.next('input').val($.map(this.children('input'), function (item) {
            return $(item).textbox('getValue').replace(/\s/g, '');
        }).join(' '));
    };
    function setValue(values) {
        var self = this;
        $.each(values, function (index, value) {
            var control = self.children('[index="' + index + '"]');
            if (control.hasClass('combobox-f')) {
                control.combobox('select', value);
            }
            else {
                control.textbox('setValue', value);
            }
        });
    };

    $.fn.area = function (options, param) {
        if (typeof (options) == 'string') {
            var method = $.fn.area.methods[options];
            if (method) {
                return method(this, param);
            } else {
                throw '[' + options + '] is undefined';
            }
        };
        var self = this;
        var dataOptions = self.attr('data-options');
        options = $.extend({ detailBox: true, method: 'post', readonly: false, disabled: false }, (self.data('area') || {}).options, options, dataOptions ? (new Function('return {' + dataOptions + '};'))() : {});
        self.data('area', { options: options });
        self.next('span').remove();
        var container = options.width ? $('<span>').css({ display: 'inline-block', width: options.width }) : $('<span>');
        var inner = options.width ? $('<span>').css({ display: 'inline-block', width: '100%' }) : $('<span>');
        self.attr({
            textboxname: self.attr('name') || self.attr('textboxname'),
        }).css({
            display: 'none'
        }).after(container.append([
            inner,
            $('<input>').attr({
                id: 'hidAddress_' + self.index(),
                type: 'hidden',
                name: self.attr('name') || self.attr('textboxname'),
                value: options.value
            })
        ])).removeAttr('name');
        if (options.label) {
            var label = $("<label class=\"textbox-label\"></label>").html(options.label);
            label.css({ textAlign: options.labelAlign, width: options.labelWidth });
            label.appendTo(self.next().children('span'));
        }
        (function () {
            if (options.url || options.data) {
                addCombobox.call(self.next().children('span'), 0, options.url || options.data, [], options);
            } else {
                $.pccData = $.pccData || [];
                var prompts = !options.country ? arry1.concat(arry2) : arry2;
                var data = !!options.country ? $.grep($.pccData, function (item) { return item.n == options.country; })[0].s : $.pccData;
                addCombobox.call(self.next().children('span'), 0, data, prompts, $.extend({
                    textField: 'n',
                    valueField: 'n',
                    sonsField: 's'
                }, options));
            }
        })();
        if (self.attr('value') || options['value']) {
            setValue.call(self.next().children('span'), (self.attr('value') || options['value']).split(' '));
        }
        return self;
    };
    $.fn.area.methods = {
        setValue: function (jq, value) {
            $.each(jq, function (i, area) {
                setValue.call($(area).next().children('span'), (value || '').split(' '));
            });
            return jq;
        },
        getValue: function (jq) {
            return $(jq[0]).next().children('input:hidden').val();
        },
        options: function (jq) {
            return jq.data('area').options;
        }
    };
    $.parser.plugins.push('area');
    $.fn.form.defaults.fieldTypes.push('area');
})(jQuery);
