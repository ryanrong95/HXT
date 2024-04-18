/// <reference path="../jquery-easyui-1.5.3/jquery.min.js" />
/*
属性:
formatters:示例:{createdate:function(val,rowdata,index){return '['+val+']'}},createdate为列绑定字段
支持easyui-datagrid所有属性
方法:
flush 刷新, 无参
reset 从新加载初始化数据 无参
search 搜索,参数为object类型,如{name:'张三',age:15},将以name和age为请求参数请求到服务器
除以上三个方法,vagrid继承了datagrid所有方法
*/
(function ($) {
    $.fn.bvgrid = function (options, param) {
        options = options || {};
        //判断一下如果$（this） 是否有 table 结构？
        if (typeof (options) == 'string') {
            var method = $.fn.bvgrid.methods[options];
            if (method) {
                return method(this, param);
            } else {
                return this.datagrid(options, param);
            }
        }
        var outLoadFilter = options.loadFilter;
        delete options.loadFilter;
        var defaults = {
            url: this.context.URL,
            method: 'get',
            queryParams: { action: 'data' },
            fitColumns: true,
            singleSelect: true,
            striped: true,
            rownumbers: true,
            pagination: true,
            pageNumber: 1,
            pageSize: 10,
            pageList: [1, 5, 10, 15, 20, 30, 40, 50],
            loadEmpty:options.loadEmpty,
            loadFilter: function (data) {
                if (!!outLoadFilter) {
                    data = outLoadFilter(data);
                }
                var rows = !!data.rows ? data.rows : data;
                for (var index in rows) {
                    var row = rows[index];
                    if (!!row.CreateDate) {
                        if (isNaN(Date(row.CreateDate))) {
                            row.CreateDate = row.CreateDate.replace(/T.+$/, '');
                        } else {
                            row.CreateDate = new Date(row.CreateDate).toDateStr();
                        }
                    }
                    if (!!row.UpdateDate) {
                        if (isNaN(Date(row.UpdateDate))) {
                            row.UpdateDate = row.UpdateDate.replace(/T/, ' ').replace(/\.\d+$/, '');
                        } else {
                            row.UpdateDate = new Date(row.UpdateDate).toDateTimeStr();
                        }
                    }
                    //if (!!row.Status && !!window.replacer.SelfStatus) {
                    //    row.Status = (isNaN(row.Status) ? parseInt(row.Status) : row.Status).getName(replacer.SelfStatus);
                    //}
                }
                return data;
            },
            onBeforeLoad: function (param) {
                if (options.loadEmpty) {
                    options.loadEmpty = false;
                    return false;
                }
                if (!!param.type) {
                    delete param.type;
                    for (var name in settings.queryParams) {
                        if (typeof (param[name]) == 'undefined') {
                            param[name] = settings.queryParams[name];
                        }
                    }
                };
                param._ = Math.random();
            },
            onLoadError: function () {
                $.messager.alert('数据加载错误', arguments.length > 2 ? arguments[2] : '', 'error');
            }
        };
        var settings = $.extend({}, defaults, (new Function('return {' + this.attr('data-options') + '}'))(), options);
        this.datagrid(settings);
        if (!!settings.formatters) {
            var currentOpitions = this.datagrid('options');
            var columns = currentOpitions.columns;
            $.each(columns, function (index, arry) {
                $.each(arry, function (cindex, column) {
                    if (!!settings.formatters[column.field]) {
                        column.formatter = settings.formatters[column.field];
                    }
                });
            });
            delete settings.formatters;
            delete currentOpitions.formatters;
        }
    };

    $.fn.bvgrid.methods = {
        flush:/*刷新*/function (jq, searchs) {
            if (!searchs || typeof (searchs) != 'object') {
                return jq.each(function () {
                    $(this).datagrid('reload', { type: 'reload' });
                });
            }
            if ($.isEmptyObject(searchs)) {
                return jq.each(function () {
                    $(this).datagrid('reload', { type: 'reload' });
                });
            }
            return jq.each(function () {
                $(this).datagrid('reload', $.extend({ type: 'reload' }, searchs));
            });
        },
        reset:/*重新加载*/function (jq) {
            return jq.each(function () {
                $(this).datagrid('load', { type: 'load' });
            });
        },
        search:/*搜索*/function (jq, searchs) {
            if (!searchs || typeof (searchs) != 'object') {
                throw '无输入搜索条件或参数类型错误!';
            }
            if ($.isEmptyObject(searchs)) {
                $.messager.alert('操作错误', '未输入任何搜索条件', 'warning');
                return false;
            }
            return jq.each(function () {
                $(this).datagrid('load', $.extend({ type: 'search' }, searchs));
            });
        }
    };
})(jQuery);

