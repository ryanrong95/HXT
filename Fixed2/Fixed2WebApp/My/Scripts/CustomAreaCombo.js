/*
属性说明:
url  加载远程数据的地址,如果没有设置data和url,则加载customarea.data.js数据
data 控件绑定的数据, 如果设置了url,则data属性无效
textField 自定义显示字段
valueField 自定义值字段
sonsField 下级数据字段
country 设置的国家值,如果设置了url或data,该属性无效
label 控件标签文字
labelAlign 控件标签文字对其center left right
labelWidth 控件标签宽度
方法说明:
setValue 参数示例:a b c c,空格隔开的字符串, 设置控件值
getValue 无参,获取控件值
options 获取所有属性
示例:
    <script>
        $(function () {
            var value = $('#area1')
                .customarea({ label: '测试', country: '中国' })
                .customarea('setValue', '内蒙古 鄂尔多斯 鄂托克旗 sadfaf')
                .customarea('getValue');;
        });
    </script>
    <input id="area1" /><br />
    <input class="easyui-customarea" data-options="label: '测试2',url:'**.json',textField:'n',valueField:'v',sonsField:'s',value:'02 021 asdfadf'" /><br />
    <input class="easyui-customarea" data-options="label: '测试2',data:[{n: '北京', v: '01', s: [{n: '海淀', v: '011'}]}, {n: '上海', v: '02', s: [{n: '外滩', v: '021'}]}],textField:'n',valueField:'v',sonsField:'s',value:'01 011 787979'" /><br />
    <input class="easyui-customarea" data-options="label: '测试2',country: '美国'" value="阿拉巴马 蒙哥马利 啊手动阀" />
*/
(function ($) {
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
                self.children('[index]:gt(' + $(this).attr('index') + ')').each(function () {
                    $(this).textbox('destroy');
                });
                if (selectValue[comboboxOption.sonsField]) {
                    addCombobox.call(self, index + 1, selectValue[comboboxOption.sonsField], prompts, op);
                } 
            },
            onChange: function () {
                changeValue.call(self);
            }
        };
        if (typeof data === 'string') {
            comboboxOption.url = data;
            comboboxOption.onLoadSuccess = function () {
                setValue.call(self, (self.parent().prev().customarea('options').value || '').split(' '));
            }
        } else {
            comboboxOption.data = data;
        }
        $('<input>').attr({
            index: index
        }).appendTo(self).combobox(comboboxOption);
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

    $.fn.customarea = function (options, param) {
        if (typeof (options) == 'string') {
            var method = $.fn.customarea.methods[options];
            if (method) {
                return method(this, param);
            } else {
                throw '[' + options + '] is undefined';
            }
        };
        var self = this;
        var dataOptions = self.attr('data-options');
        options = $.extend({ }, options, dataOptions ? (new Function('return {' + dataOptions + '};'))() : {});
        self.data('customarea', { options: options });
        self.next('span').remove();
        self.attr({
            textboxname: self.attr('name') || self.attr('textboxname'),
        }).css({
            display: 'none'
        }).after($('<span>').append([
            $('<span>'),
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
            if (options.url) {
                addCombobox.call(self.next().children('span'), 0, options.url, [], options);
            } else if (options.data) {
                addCombobox.call(self.next().children('span'), 0, options.data, [], options);
            }
        })();
        if (self.attr('value') || options['value']) {
            setValue.call(self.next().children('span'), (self.attr('value') || options['value']).split(' '));
        }
        return self;
    };
    $.fn.customarea.methods = {
        setValue: function (jq, value) {
            $.each(jq, function (i, customarea) {
                setValue.call($(customarea).next().children('span'), (value || '').split(' '));
            });
            return jq;
        },
        getValue: function (jq) {
            return $(jq[0]).next().children('input:hidden').val();
        },
        options: function (jq) {
            return jq.data('customarea').options;
        }
    };
    $.parser.plugins.push('customarea');
    $.fn.form.defaults.fieldTypes.push('customarea');
})(jQuery);
