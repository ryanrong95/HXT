/// <reference path="../../jquery-easyui-1.7.6/jquery.min.js" />
/// <reference path="../../jquery.cookie-1.4.1.min.js" />
/*
属性:
支持easyui-datagrid所有属性
方法:
flush 刷新 json参数
reset 从新加载初始化数据 无参
search 搜索,json参数
json参数举例说明:{name:'张三',age:15},将以name和age为请求参数请求到服务器
除以上三个方法,vagrid继承了datagrid所有方法
*/
/*
如果想保持页码，请务必引用jquery.cookie-1.4.1.min.js
*/
(function ($) {

    $.fn.myDatagrid = function (options, param) {
        options = options || {};
        if (typeof (options) == 'string') {
            var method = $.fn.myDatagrid.methods[options];
            if (method) {
                return method(this, param);
            } else {
                return this.datagrid(options, param);
            }
        }


        if (typeof (options.singleSelect) == 'undefined') {
            options.singleSelect = true;
        }

        if (typeof (options.pagination) == 'undefined') {
            options.pagination = true;
        }

        if (typeof (options.showFooter) == 'undefined') {
            options.showFooter = false;
        }

        if (typeof (options.checkOnSelect) == 'undefined') {
            options.checkOnSelect = true;
        }

        if (typeof (options.selectOnCheck) == 'undefined') {
            options.selectOnCheck = true;
        }

        //alert([options.checkOnSelect, options.selectOnCheck]);

        if (typeof (options.nowrap) == 'undefined') {
            options.nowrap = true;
        }

        if (typeof (options.fitColumns) == 'undefined') {
            options.fitColumns = false;
        }

        if (typeof (options.actionName) == 'undefined') {
            options.actionName = 'data';
        }

        if (typeof (options.border) == 'undefined') {
            options.border = true;
        }
        if (typeof (options.autoRowHeight) == 'undefined') {
            options.autoRowHeight = false;
        }
        if (typeof (options.fit) == 'undefined') {
            options.fit = true;
        }
        // 第一次加载是否显示列表
        if (typeof (options.loadEmpty) == 'undefined') {
            options.loadEmpty = false;
        }

        if (typeof (options.width) == 'undefined') {
            options.width = 'auto';
        }

        if (typeof (options.height) == 'undefined') {
            options.height = 'auto';
        }

        if (typeof (options.pageSize) == 'undefined') {
            options.pageSize = 20;
        }

        //this

        $(this).each(function (index, item) {
            var id = $(this).prop('id');
            //按照要求，需要记录页面
            var ckey = 'cookie-pageNumber-' + index + id + '-' + encodeURIComponent(document.URL);
            if ($.cookie) {
                var pageIndex = parseInt($.cookie(ckey));
                //alert(pageIndex);
                if (pageIndex) {
                    options.pageNumber = pageIndex;
                } else {
                    options.pageNumber = 1;
                }
            }

            var url = options.url || document.URL;
            var method = options.url ? null : 'get';

            if (options.data) {
                url = undefined;
                method = undefined;
            }

            var defaults = {
                refreshChannelReq: options.refreshChannelReq || false,//单行刷新是否刷新渠道列
                cellstyle: options.cellstyle || {
                    //单元格提示信息 client客户提示信息，supplier供应商提示信息
                    client: 'all',//'noContact','hideNameAndnoContact' 'all'表示全部显示,'noContact'表示不显示联系人,'hideNameAndnoContact'表示不显示联系人,隐藏客户公司
                    supplier: 'all'//'hideName'//隐藏供应商
                },

                height: options.height,
                width: options.width,

                actionName: options.actionName,
                columns: options.columns,
                frozenColumns: options.frozenColumns,
                fitColumns: options.fitColumns,
                resizeHandle: options.resizeHandle,
                autoRowHeight: options.autoRowHeight,
                border: options.border,

                data: options.data,

                fit: options.fit,
                url: url,
                method: method,
                queryParams: $.extend(options.queryParams, { action: options.actionName || "data" }),
                singleSelect: options.singleSelect,
                striped: true,
                rownumbers: options.rownumbers,

                pagePosition: 'bottom',
                pagination: options.pagination,
                pageNumber: options.pageNumber,
                //pageSize: 20,
                pageSize: options.pageSize,
                pageList: [5, 10, 15, 20,30,40, 50, 100, 200, 300],

                nowrap: options.nowrap, //	boolean	设置为 true，则把数据显示在一行里。设置为 true 可提高加载性能。

                idField: options.idField, //|| 'ID', //	string	指示哪个字段是标识字段。	
                loadMsg: 'loading',//string	当从远程站点加载数据时，显示的提示消息。

                checkOnSelect: options.checkOnSelect,
                selectOnCheck: options.selectOnCheck,
                toolbar: options.toolbar,

                multiSort: false,
                remoteSort: false,

                scrollbarSize: 12,

                showHeader: true,
                showFooter: options.showFooter,
                rowStyler: options.rowStyler,

                loader: undefined,

                editors: options.editors,
                view: options.view,
                loadEmpty: options.loadEmpty,

                loadFilter: options.loadFilter,// function (data) {return data;},
                onBeforeLoad: function (param) {
                    if (options.loadEmpty) {
                        options.loadEmpty = false;
                        return false;
                    }
                    else {
                        return true;
                    }
                },
                onLoadError: function () {
                    $.messager.alert('数据加载错误', arguments.length > 2 ? arguments[2] : '', 'error');
                },

                onLoadSuccess: function (data) {
                    if (options.onLoadSuccess != 'undefined' && options.onLoadSuccess) {
                        options.onLoadSuccess(data);
                    }
                    $.parser.parse('.easyui-formatted');
                    $.parser.parse('.easyui-tooltip');

                    if (options.pagination) {
                        var pagination = $(this).datagrid("getPager").data("pagination").options;
                        var pageIndex = pagination.pageNumber;

                        if ($.cookie) {
                            $.cookie(ckey, pageIndex);
                        }
                    }
                },

                //允许触发的其他事件
                onClickRow: options.onClickRow,
                onDblClickRow: options.onDblClickRow,
                onClickCell: options.onClickCell,
                //onClickCell: function (rowIndex, field, value) {
                //    if (field.indexOf('btn') > -1) {
                //        return false;
                //    }

                //    if (options.onClickCell) {
                //        options.onClickCell(rowIndex, field, value);
                //    }
                //}, 
                onDblClickCell: options.onDblClickCell,
                onSelect: options.onSelect,
                onUnselect: options.onUnselect,
                onSelectAll: options.onSelectAll,
                onUnselectAll: options.onUnselectAll,
                onCheck: options.onCheck,
                onUncheck: options.onUncheck,
                onCheckAll: options.onCheckAll,
                onUncheckAll: options.onUncheckAll,
                onBeforeEdit: options.onBeforeEdit,
                onAfterEdit: options.onAfterEdit,
                onCancelEdit: options.onCancelEdit,
                onBeforeSelect: !!options.onBeforeSelect ? options.onBeforeSelect : function (index, row) {
                    //alert(JSON.stringify(row));
                    //return false;
                }
            };
            $(item).datagrid(defaults);
        });
        return this;
    };

    $.fn.myDatagrid.methods = {
        reload:/*刷新*/function (jq) {
            return jq.each(function () {
                $(this).datagrid('reload');
            });
        },
        flush:/*刷新*/function (jq) {
            return jq.each(function () {
                $(this).datagrid('reload');
                $(this).datagrid('clearChecked');
            });
        },
        reset:/*重新加载*/function (jq) {
            return jq.each(function () {
                $(this).datagrid('load', {});
            });
        },
        clear:/*重新加载*/function (jq) {
            return jq.each(function () {
                $(this).datagrid('loadData', []);
            });
        },
        search:/*搜索*/function (jq, searchs) {
            if ($.isEmptyObject(searchs)) {
                $.messager.alert('操作错误', '未输入任何搜索条件', 'warning');
                return false;
            }

            searchs = searchs || {};
            if (!searchs.action) {
                searchs.action = 'data';
            }
            return jq.each(function () {
                $(this).datagrid('load', $.extend({ type: 'search' }, searchs));
            });
        },
        // 行扩展，展开显示详细。比如询价展开显示报价。
        rowExpand: function (jq, index) {
            return jq.each(function () {
                var rowData = $(this).datagrid('getRows')[index];
                var that = this;
                if (rowData.opened) { // 关闭
                    var changesRow = $(this).datagrid('getChanges', 'inserted');
                    rows = $.map(changesRow, function (value, index) {
                        if (value.InquiryID == rowData.ID) {
                            return value;
                        }
                    });

                    var dl = parseInt(rows.length) + parseInt(index);
                    for (var i = dl; i > index; i--) {
                        console.log(i);
                        $(this).datagrid('deleteRow', i);
                    }

                    rowData.opened = false;
                }
                else { // 展开
                    $.post('?action=getQuotes', { inquiryid: rowData.ID }, function (data) {
                        if (data.data.length > 0) {
                            var idx = index;
                            for (var i = data.data.length - 1; i >= 0; i--) {
                                idx = idx + 1;
                                var insertrow = data.data[i];
                                insertrow.topIndex = index,
                                insertrow.index = idx;
                                insertrow.isOpen = true; // 是报价行标识
                                insertrow.inquiryStatus = rowData.Status;
                                insertrow.inquiryQuoteStatus = rowData.QuoteStatus;
                                insertrow.ClientName = rowData.Client.Name;
                                insertrow.InquiryInvoiceType = rowData.InvoiceType;
                                $(that).datagrid('insertRow', {
                                    index: insertrow.index,
                                    row: insertrow
                                });
                                createSupplierTip(insertrow, $(that).myDatagrid('options').cellstyle.supplier, insertrow.index);
                            }
                            $.parser.parse('.easyui-formatted');
                            rowData.opened = true;
                        }
                    });
                }
            });
        },
        // 单行刷新
        refreshSingle: function (jq, index) {
            return jq.each(function () {
                var that = this;
                var rowData = $(this).datagrid('getRows')[index];
                if (rowData.topIndex || rowData.topIndex >= 0) {
                    index = rowData.topIndex;
                }
                var topData = $(this).datagrid('getRows')[index];// 实际行数据
                $.post('?action=single', {
                    id: topData.ID
                }, function (row) {
                    row.opened = topData.opened;

                    // 更新实际行
                    $(that).datagrid('updateRow', {
                        index: index,
                        row: row
                    });
                    $(that).datagrid('refreshRow', index);
                    createClientTip(row, $(that).myDatagrid('options').cellstyle.client, index);
                    if ($(that).myDatagrid('options').refreshChannelReq) {
                        createChannelTip(row, $(that).myDatagrid('options').cellstyle.supplier, index);
                    }
                    if (topData.opened || rowData.topIndex) {
                        $(that).myDatagrid('rowExpand', index);
                    }
                    $.parser.parse('.easyui-formatted');
                });
            });
        },
        deleteRow: function (jq, index) {
            $(jq).datagrid('deleteRow', index);
            var rows = $(jq).datagrid("getRows");    //重新获取数据生成行号
            $(jq).datagrid("loadData", rows);
        }
    };

})(jQuery);

