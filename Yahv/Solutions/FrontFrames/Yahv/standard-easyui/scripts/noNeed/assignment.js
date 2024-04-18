//指派控件wjl
(function ($) {
    function randomID(model) {
        return model + '_' + parseInt(Math.random() * Math.pow(10, 10))
    }
    //获取已选择列表和未选择列表数据的方法
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

    $.fn.assignment = function (opt, param) {
        var sender = this;
        //如果options为string，则是方法调用
        if (typeof opt == 'string') {
            var method = $.fn.assignment.methods[opt];
            if (method) {
                method(this, param);//如果有该方法就调用该方法
                return;
            } else {
                alert("该插件没有这个方法")
            }
        }
        var options = opt || {};
        var data_options = {};
        var s = $.trim(sender.attr("data-options"));
        //如果html配置了data-options
        if (s) {
            if (s.substring(0, 1) != "{") {
                s = "{" + s + "}";
            }
            data_options = (new Function("return " + s))();
            //$.extend(true, {}, $.fn.assignment.defaults, data_options, opt);第一个参数为true，则是深度合并对象
            options = $.extend(true, {}, $.fn.assignment.defaults, data_options, opt);
        } else {
            options = $.extend(true, {}, $.fn.assignment.defaults, opt);
        }

        //如果已经初始化，就不需要再进行初始化了
        if ($(sender).data("options")) {
            show();
            return;
        }
        $(sender).data("options", options);//存储options的值

        //为页面添加dom元素---开始
        var $contains = $("<div></div>");
        var $pending = $("<table></table>");
        var $selected = $("<table></table>");

        $(sender).data("pending", $pending);//存储待选择列表
        $(sender).data("selected", $selected);//存储已选择列表
        $(sender).data("contains", $contains);//存储整个dialog容器

        var toolbar_id = randomID("toolbar");
        toolbar_id = toolbar_id;
        var $toolbar = $("<div id=" + toolbar_id + " style='padding:5px;height:auto'></div>");//包裹工具栏的div

        var $input = $("<input style='width:200px;height:32px'>");//input的搜索框
        $(sender).data("input", $input);//存储input搜索框

        var $btn_search = $("<div style='margin-left:5px;margin-right:5px;'>搜索</div>");//搜索按钮
        var $btn_search_all = $("<div>搜索全部</div>");//搜索全部按钮
        var $div = $("<div style='height:36px;'></div>");//两个列表之间的间距
        
        $toolbar.append($input);
        $toolbar.append($btn_search);
        $toolbar.append($btn_search_all);
        $contains.append($selected);
        $contains.append($div);
        $contains.append($pending);
        $contains.append($toolbar);
        //为页面添加dom元素---结束

        //初始化工具栏
        var init = function () {
            $input.textbox();
            $btn_search.linkbutton({
                iconCls: 'icon-search',
                onClick: function () {
                    options.pending.Search($input.textbox('getValue'));
                }
            });
            $btn_search_all.linkbutton({
                iconCls: 'icon-search',
                onClick: function () {
                    $input.textbox("setValue", "");
                    $pending.datagrid('load');
                }
            });
        }
        init.call(this);

        //打开dialog
        function show() {
            $(sender).data("contains").dialog({
                title: options.title,
                width: options.width,
                height: options.height,
                modal: true,
                buttons: [{
                    text: '关闭',
                    handler: function () {
                        $(sender).data("contains").dialog("close");
                    }
                }],
                onOpen: function () {
                    var col_pending = $.extend(true, [], options.columns);
                    col_pending[0].push({
                        field: 'operation',
                        title: '操作',
                        width: "90px",
                        formatter: function (value, row, index) {
                            return '<span class="easyui-formatted"><button class="easyui-linkbutton op-transfor" style="margin-left:5px;">选择</button></span>';
                        }
                    });
                    $(sender).data("pending").datagrid({
                        title: "待选择列表",
                        loader: function (param, success, error) {
                            load(options.pending.url, function (data) {
                                success(data);
                            });
                        },
                        columns: col_pending,
                        fitColumns: true,
                        fit: true,
                        rownumbers: true,
                        singleSelect: true,
                        onLoadSuccess: function (data) {
                            $.parser.parse('.easyui-formatted');
                            //为列表中的button按钮通过委托去除点击事件，否则的话会重复添加点击事件
                            $(sender).data("pending").prev().off("click", ".op-transfor");
                            //为列表中的button按钮通过委托添加点击事件
                            $(sender).data("pending").prev().on("click", ".op-transfor", function () {
                                var index = $(this).parents("tr").attr("datagrid-row-index");
                                options.pending.onClick($(sender).data("pending").datagrid('getData').rows[index], index);
                            });
                        },
                        toolbar: '#' + toolbar_id,
                    });
                    var col_selected = $.extend(true, [], options.columns);
                    col_selected[0].push({
                        field: 'operation',
                        title: '操作',
                        width: "90px",
                        formatter: function (value, row, index) {
                            return '<span class="easyui-formatted"><button class="easyui-linkbutton op-transfor" style="margin-left:5px; ">取消选择</button></span>';
                        }
                    });
                    $(sender).data("selected").datagrid({
                        title: "已选择列表",
                        loader: function (param, success, error) {
                            load(options.selected.url, function (data) {
                                success(data);
                            });
                        },
                        columns: col_selected,
                        fitColumns: true,
                        rownumbers: true,
                        singleSelect: true,
                        onLoadSuccess: function (data) {
                            $.parser.parse('.easyui-formatted');
                            //为列表中的button按钮通过委托去除点击事件，否则的话会重复添加点击事件
                            $(sender).data("selected").prev().off("click", ".op-transfor");
                            //为列表中的button按钮通过委托添加点击事件
                            $(sender).data("selected").prev().on("click", ".op-transfor", function () {
                                var index = $(this).parents("tr").attr("datagrid-row-index");
                                options.selected.onClick($(sender).data("selected").datagrid('getData').rows[index], index);
                            });
                        }
                    });
                }
            });
        }
        show.call(this);
    }

    $.fn.assignment.defaults = {
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

    $.fn.assignment.methods = {
        refresh: function (jq) {
            $(jq).data("input").textbox("setValue", "");
            $(jq).data("pending").datagrid('load');
            $(jq).data("selected").datagrid('load');
        },
        searchInput: function (jq, patam) {
            $(jq).data("pending").datagrid('loadData', patam);
        }
    };
    $.parser.plugins.push('assignment');

})(jQuery)
