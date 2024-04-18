//zenguohau
(function ($) {

    function randomID(model) {
        return model + '_' + parseInt(Math.random() * Math.pow(10, 10))
    }

    var closure = null;

    function load(url, cb) {
        $.ajax({
            type: "get",
            url: url,
            dataType: "jsonp",
            success: function (data) {
                cb(data);
            },
            error: function (err) {
                console.log(err);
            }
        });
    }
    
    $.fn.assgin = function (opt, param) {
        if (typeof opt == 'string') {
            if (opt == 'refresh') {
                refresh.call(this);
            }
            if (opt == 'searchInput' && param != null) {
                searchInput.call(this,param);
            }
        }
        var options = opt || {};
        var sender = this;
        var data_options = {};
        var s = $.trim(sender.attr("data-options"));
        if (s) {
            if (s.substring(0, 1) != "{") {
                s = "{" + s + "}";
            }
            data_options = (new Function("return " + s))();
            options = $.extend(true, {}, $.fn.assgin.defaults, data_options, opt);
        } else {
            options = $.extend(true, {}, $.fn.assgin.defaults, opt);
        }

        if (closure) {
            if (opt == 'searchInput' && param != null) {

            } else {
                show();
            }
            return;
        }
        closure = {};

        closure.options = options;
        var $contains = $("<div></div>");
        var $pending = $("<table></table>");
        var $selected = $("<table></table>");
        closure.$pending = $pending;
        closure.$selected = $selected;
        closure.$contains = $contains;
        var toolbar_id = randomID("toolbar");
        closure.toolbar_id = toolbar_id;
        var $toolbar = $("<div id=" + toolbar_id + " style='padding:5px;height:auto'></div>");
        var $input = $("<input style='width:200px;height:32px'>");
        var $btn_search = $("<div style='margin-left:5px;margin-right:5px;'>搜索</div>");
        var $btn_search_all = $("<div>搜索全部</div>");
        var $div = $("<div style='height:36px;'></div>");
        closure.$input = $input;
        $toolbar.append($input);
        $toolbar.append($btn_search);
        $toolbar.append($btn_search_all);
        $contains.append($selected);
        $contains.append($div);
        $contains.append($pending);
        $contains.append($toolbar);
        var init = function () {
            closure.$input.textbox();
            $btn_search.linkbutton({
                iconCls: 'icon-search',
                onClick: function () {
                    closure.options.pending.Search(closure.$input.textbox('getValue'));
                }
            });
            $btn_search_all.linkbutton({
                iconCls: 'icon-search',
                onClick: function () {
                    closure.$input.textbox("setValue", "");
                    closure.$pending.datagrid('load');
                }
            });
        }
        init.call(this);
        function refresh() {
            closure.$input.textbox("setValue", "");
            closure.$pending.datagrid('load');
            closure.$selected.datagrid('load');
        }
        function searchInput(data) {
            closure.$pending.datagrid('loadData',data);
        }
        function show() {
            closure.$contains.dialog({
                title: closure.options.title,
                width: closure.options.width,
                height: closure.options.height,
                modal: true,
                buttons: [{
                    text: '关闭',
                    handler: function () {
                        closure.$contains.dialog("close");
                    }
                }],
                onOpen: function () {
                    if (closure.render) {
                        return;
                    }
                    var col_pending = $.extend(true, [], closure.options.columns);
                    col_pending[0].push({
                        field: 'operation',
                        title: '操作',
                        width: "90px",
                        formatter: function (value, row, index) {
                            return '<span class="easyui-formatted"><button class="easyui-linkbutton op-transfor" style="margin-left:5px;">选择</button></span>';
                        }
                    });
                    closure.$pending.datagrid({
                        title:"待选择列表",
                        loader: function (param, success, error) {
                            load(closure.options.pending.url, function (data) {
                                success(data);
                            });
                        },
                        columns: col_pending,
                        fitColumns: true,
                        rownumbers: true,
                        singleSelect: true,
                        onLoadSuccess: function (data) {
                            $.parser.parse('.easyui-formatted');
                            closure.$pending.prev().off("click", ".op-transfor");
                            closure.$pending.prev().on("click", ".op-transfor", function () {
                                var index = $(this).parents("tr").attr("datagrid-row-index");
                                closure.options.pending.onClick(closure.$pending.datagrid('getData').rows[index], index);
                            });
                        },
                        toolbar: '#' + closure.toolbar_id,
                    });
                    var col_selected = $.extend(true, [], closure.options.columns);
                    col_selected[0].push({
                        field: 'operation',
                        title: '操作',
                        width: "90px",
                        formatter: function (value, row, index) {
                            return '<span class="easyui-formatted"><button class="easyui-linkbutton op-transfor" style="margin-left:5px; ">取消选择</button></span>';
                        }
                    });
                    closure.$selected.datagrid({
                        title: "已选择列表",
                        loader: function (param, success, error) {
                            load(closure.options.selected.url, function (data) {
                                success(data);
                            });
                        },
                        columns: col_selected,
                        fitColumns: true,
                        rownumbers: true,
                        singleSelect: true,
                        onLoadSuccess: function (data) {
                            $.parser.parse('.easyui-formatted');
                            closure.$selected.prev().off("click", ".op-transfor");
                            closure.$selected.prev().on("click", ".op-transfor", function () {
                                var index = $(this).parents("tr").attr("datagrid-row-index");
                                closure.options.selected.onClick(closure.$selected.datagrid('getData').rows[index], index);
                            });
                        }
                    });
                }
            });
        }
        show.call(this);
    }

    $.fn.assgin.defaults = {
        title: "操作列表",//弹出框的标题
        width: 700,
        height: 500,
        columns: null,
        //待选择列表
        pending: {
            title: '待选择列表',
            data: null,
            onClick: null, // 选择按钮函数
            Search: function (key) { } // 搜索函数
        },
        //已选择列表
        selected: {
            title: '已选择列表',
            data: null,
            onClick: function () { }, // 选择按钮函数
        },
        isSearch: true, // 是否搜索
    };

    $.fn.assgin.methods = {

    };

    $.parser.plugins.push('assgin');

})(jQuery)
