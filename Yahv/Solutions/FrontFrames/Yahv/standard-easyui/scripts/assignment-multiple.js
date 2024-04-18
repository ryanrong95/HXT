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
                return method(this, param);//如果有该方法就调用该方法
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
        if ($(sender).data("pending")) {
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

        var pendtoolbar_id = randomID("pendtoolbar");
        $(sender).data("pendtoolbar_id", pendtoolbar_id);
        var $pendtoolbar = $("<div id=" + pendtoolbar_id + " style='padding:5px;height:auto'></div>");//待选择列表工具栏

        var $input = $("<input>");//input的搜索框
        $(sender).data("input", $input);//存储input搜索框

        var $btn_search = $("<button style='margin-left:5px;margin-right:5px;'>筛选</button>");//搜索按钮
        var $btn_search_all = $("<button style='margin-right:5px;'>取消筛选</button>");//搜索全部按钮
        var $btn_select_items = $("<button>确认选择</button>");//确认选择


        var selectedtoolbar_id = randomID("selectedtoolbar");
        $(sender).data("selectedtoolbar_id", selectedtoolbar_id);
        var $selectedtoolbar = $("<div id=" + selectedtoolbar_id + " style='padding:5px;height:auto'></div>");//已选择列表工具栏
        var $btn_cancelselect_items = $("<button style='display:none;'>取消已选择项</button>");//确认取消选择

        var $div = $("<div style='height:36px;display:none;'></div>");//两个列表之间的间距
        $(sender).data("div", $div);

        $pendtoolbar.append($input);
        $pendtoolbar.append($btn_search);
        $pendtoolbar.append($btn_search_all);
        $pendtoolbar.append($btn_select_items);

        $selectedtoolbar.append($btn_cancelselect_items);
        $(sender).data("btn_cancelselect_items", $btn_cancelselect_items);

        $contains.append($selected);
        $contains.append($div);
        $contains.append($pending);
        $contains.append($selectedtoolbar);
        $contains.append($pendtoolbar);
        $('body').append($contains);

        //为页面添加dom元素---结束

        $(sender).data("selectpendingData", []);//存储待选择列表里面选择的数据
        $(sender).data("selectselectedData", []);//存储已选择列表里面选择的数据
        $(sender).data("selectedData", []);//存储已选择列表的数据

        //初始化工具栏
        var init = function () {
            $input.textbox({ prompt: "请输入用户名或者真实姓名", width: 180 });
            $btn_search.linkbutton({
                iconCls: 'icon-yg-search',
                onClick: function () {
                    options.pending.Search($input.textbox('getValue'));
                }
            });
            $btn_search_all.linkbutton({
                onClick: function () {
                    $input.textbox("setValue", "");
                    $pending.datagrid('load');
                }
            });
            $btn_select_items.linkbutton({
                onClick: function () {
                    if ($(sender).data("selectpendingData").length == 0) {
                        $.messager.alert('警告', '请选择待选择列表中的数据');
                    } else {
                        if (options.type == "holistic") {
                            if ($(sender).data("div").css("display") == "none") {
                                $(sender).data("selectedData", $(sender).data("selectpendingData"));
                            } else {
                                var selectedData = $(sender).data("selected").datagrid("getData").rows;
                                var selectpendingData = $(sender).data("selectpendingData");
                                if (selectedData.length >= selectpendingData.length) {
                                    selectedData.push.apply(selectedData, selectpendingData);
                                    $(sender).data("selectedData", selectedData);
                                } else {
                                    selectpendingData.push.apply(selectpendingData, selectedData);
                                    $(sender).data("selectedData", selectpendingData);
                                }
                            }
                            showSelected($(sender).data("selectedData"));
                        }
                        if (options.selectitems) {
                            options.selectitems($(sender).data("selectpendingData"));
                        }
                    }
                }
            });
            $btn_cancelselect_items.linkbutton({
                onClick: function () {
                    if ($(sender).data("selectselectedData").length == 0) {
                        $.messager.alert('警告', '请选择已选择列表中的数据');
                    } else {
                        if (options.type == "holistic") {
                            var a = $(sender).data("selectedData");
                            var b = $(sender).data("selectselectedData");
                            var c = [];
                            for (var i = 0; i < a.length; i++) {
                                var m = 0;
                                for (var j = 0; j < b.length; j++) {
                                    if (a[i].ID == b[j].ID) {
                                        break;
                                    } else {
                                        m++;
                                    }
                                }
                                if (m == b.length) {
                                    c.push(a[i])
                                }
                            }
                            $(sender).data("selectedData", c);
                            $(sender).data("selected").datagrid("loadData", $(sender).data("selectedData"));
                        }
                        if (options.unSelectitems) {
                            options.unSelectitems($(sender).data("selectselectedData"));
                        }
                    }
                }
            });
        }
        init.call(this);

        //已选列表的参数
        var selectedOptions = {
            title: "已选择列表",
            fitColumns: true,
            columns: null,
            rownumbers: true,
            singleSelect: false,
            toolbar: '#' + $(sender).data("selectedtoolbar_id"),
            onSelect: function (index, row) {
                $(sender).data("selectselectedData").push(row);
            },
            onUnselect: function (index, row) {
                var selectselectedData = $(sender).data("selectselectedData");
                var selectselectedData2 = [];
                for (var i = 0; i < selectselectedData.length; i++) {
                    if (selectselectedData[i].ID != row.ID) {
                        selectselectedData2.push(selectselectedData[i]);
                    }
                }
                $(sender).data("selectselectedData", selectselectedData2);
            },
            onSelectAll: function (rows) {
                $(sender).data("selectselectedData", rows);
            },
            onUnselectAll: function (rows) {
                $(sender).data("selectselectedData", []);
            }
        }

        function showSelected(data) {
            $(sender).data("div").show();
            var columns = options.columns;
            if ($(sender).data("options").type == null) {
                $(sender).data("btn_cancelselect_items").show();
                selectedOptions.loader = function (param, success, error) {
                    load(options.selected.url, function (data) {
                        success(data);
                    });
                }
            } else if ($(sender).data("options").type == "holistic") {
                if (data) {
                    selectedOptions.data = data;
                }
                var columns2 = [];
                for (var i = 0; i < columns[0].length; i++) {
                    if (columns[0][i].field != 'ck') {
                        columns2.push(columns[0][i]);
                    }
                }
                columns[0] = columns2;
            }
            var col_selected = $.extend(true, [], columns);
            selectedOptions.columns = col_selected;
            $(sender).data("selected").datagrid(selectedOptions);
        }

        //打开dialog
        function show() {
            $(sender).data("selectpendingData", []);
            $(sender).data("selectselectedData", []);
            $(sender).data("input").textbox("setValue", "");
            var col_pending = $.extend(true, [], options.columns);
            $(sender).data("pending").datagrid({
                title: "待选择列表",
                fitColumns: true,
                loader: function (param, success, error) {
                    load(options.pending.url, function (data) {
                        success(data);
                    });
                },
                columns: col_pending,
                rownumbers: true,
                singleSelect: false,
                toolbar: '#' + $(sender).data("pendtoolbar_id"),
                onSelect: function (index, row) {
                    $(sender).data("selectpendingData").push(row);
                },
                onUnselect: function (index, row) {
                    var selectpendingData = $(sender).data("selectpendingData");
                    var selectpendingData2 = [];
                    for (var i = 0; i < selectpendingData.length; i++) {
                        if (selectpendingData[i].ID != row.ID) {
                            selectpendingData2.push(selectpendingData[i]);
                        }
                    }
                    $(sender).data("selectpendingData", selectpendingData2);
                },
                onSelectAll: function (rows) {
                    $(sender).data("selectpendingData", rows);
                },
                onUnselectAll: function (rows) {
                    $(sender).data("selectpendingData", []);
                }
            });
            if ($(sender).data("options").type == null) {
                showSelected();
            }
        }
        show.call(this);
    }

    $.fn.assignment.defaults = {
        type: null, //"holistic"代表批量选择
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
        options: function (jq) {
            return $(jq).data("options");//返回options
        },
        twoselectData: function (jq) {
            return {//返回待选择列表和已选择列表中选中的值
                selectpendingData: $(jq).data("selectpendingData"),
                selectselectedData: $(jq).data("selectselectedData")
            }
        },
        refresh: function (jq) {
            $(jq).data("input").textbox("setValue", "");
            $(jq).data("selectpendingData", []);

            if ($(jq).data("options").type == "holistic") {
                var selectedData = $(jq).data("selectedData");
                var ids = [];
                for (var i = 0; i < selectedData.length; i++) {
                    ids.push(selectedData[i].ID);
                }
                ids = ids.join(",");
                var url = $(jq).data("options").pending.url + "?ids=" + ids;
                load(url, function (data) {
                    $(jq).data("pending").datagrid("loadData", data);
                })
            } else if ($(jq).data("options").type == null) {
                $(jq).data("pending").datagrid('load');
                $(jq).data("selected").datagrid('load');
                $(jq).data("selectselectedData", []);
            }
        },
        searchInput: function (jq, param) {
            $(jq).data("pending").datagrid('loadData', param);
        },
        selectitems: null,
        unSelectitems: null
    };
    $.parser.plugins.push('assignment');

})(jQuery)
