/*
combogridapi控件用于获取标准库api复杂数据,
继承自combogrid,只需增加属性 apiUrl 即可
示例:
<input class="easyui-combogridapi" data-options="apiUrl:'http://stadard.b1b.com/ApiProduct'">
或
<input class="easyui-combogridapi" apiUrl="http://stadard.b1b.com/ApiProduct">
或
<input id="cc">
$('#cc').combogridapi({apiUrl:'http://stadard.b1b.com/ApiManufacturer'});
其他属性及事件方法的使用见combogrid
*/
(function ($) {
    $.fn.combogridapi = function (options, param) {
        if (typeof (options) == 'string') {
            var self = $(this)
            $.fn.combogrid.methods[options](this, param);
            return self.combogrid(options, param);
        }
        var self = this;
        var dataOptions = (new Function('return {' + self.attr('data-options') + '}'))();
        var fields = (function () {
            var arry = [];
            var columns = options.columns || dataOptions.columns || [];
            for (var index = 0, length = columns.length; index < length; index++) {
                $.each(columns[index], function () {
                    arry.push(this.field);
                });
            }
            return arry;
        })();
        options = $.extend({}, options, {
            loader: function (param, success, error) {
                var q = param.q || '';
                if (q.length < 1) { return false }
                $.ajax({
                    url: options.apiUrl || self.attr('apiUrl') || dataOptions.apiUrl,
                    dataType: 'jsonp',
                    data: {
                        search: q,
                        fields: fields
                    },
                    success: function (data) {
                        success(data);
                    },
                    error: function () {
                        error.apply(this, arguments);
                    }
                });
            },
            mode: 'remote'
        });
        this.combogrid(options);
    };
    $.parser.plugins.push('combogridapi');
    $.fn.form.defaults.fieldTypes && $.fn.form.defaults.fieldTypes.push('combogridapi');
})(jQuery);