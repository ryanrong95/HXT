/*
comboapi控件用于获取标准库api简单数据,
继承自combobox,只需增加属性 apiUrl 即可,并且不需要设置 valueField 和 textField
示例:
<input class="easyui-comboapi" data-options="apiUrl:'http://stadard.b1b.com/ApiManufacturer'">
或
<input class="easyui-comboapi" apiUrl="http://stadard.b1b.com/ApiManufacturer">
或
<input id="cc">
$('#cc').comboapi({apiUrl:'http://stadard.b1b.com/ApiManufacturer'});
其他属性及事件方法见combobox
*/
(function ($) {
    $.fn.comboapi = function (options, param) {
        if (typeof (options) == 'string') {
            return $(this).combobox(options, param);
        }
        var self = this;
        options = $.extend({}, options, {
            loader: function (param, success, error) {
                var q = param.q || '';
                if (q.length < 1) { return false }
                $.ajax({
                    url: options.apiUrl || self.attr('apiUrl') || ((new Function('return {' + self.attr('data-options') + '}'))() || {}).apiUrl,
                    dataType: 'jsonp',
                    data: {
                        search: q
                    },
                    success: function (data) {
                        var items = $.map(data, function (item) {
                            return {
                                name: item
                            };
                        });
                        success(items);
                    },
                    error: function () {
                        error.apply(this, arguments);
                    }
                });
            },
            mode: 'remote',
            valueField: 'name',
            textField: 'name'
        });
        this.combobox(options);
    };
    $.parser.plugins.push('comboapi');
    $.fn.form.defaults.fieldTypes && $.fn.form.defaults.fieldTypes.push('comboapi');
})(jQuery);