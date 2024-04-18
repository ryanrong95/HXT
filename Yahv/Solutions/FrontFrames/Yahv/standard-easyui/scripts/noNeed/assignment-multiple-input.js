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

    function changeString(data) {
        var m = [];
        for (var i = 0; i <data.length; i++) {
            m.push(data[i].RealName)
        }
        m = m.join(",");
        return m;
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
        var $selected = $("<div></div>");
        var $selected_input = $("<input />");

        $(sender).data("pending", $pending);//存储待选择列表
        $(sender).data("selected", $selected);//存储已选择input
        $(sender).data("contains", $contains);//存储整个dialog容器
        $(sender).data("selected_input", $selected_input);//存储整个dialog容器

        var pendtoolbar_id = randomID("pendtoolbar");
        $(sender).data("pendtoolbar_id", pendtoolbar_id);
        var $pendtoolbar = $("<div id=" + pendtoolbar_id + " style='padding:5px;height:auto'></div>");//待选择列表工具栏

        var $input = $("<input style='width:200px;height:32px'>");//input的搜索框
        $(sender).data("input", $input);//存储input搜索框

        var $btn_search = $("<div style='margin-left:5px;margin-right:5px;'>搜索</div>");//搜索按钮
        var $btn_search_all = $("<div style='margin-right:5px;'>搜索全部</div>");//搜索全部按钮
        var $btn_select_items = $("<div>确认选择</div>");//确认选择

        $pendtoolbar.append($input);
        $pendtoolbar.append($btn_search);
        $pendtoolbar.append($btn_search_all);
        $pendtoolbar.append($btn_select_items);


        $contains.append($selected);
        $contains.append($pending);
        $contains.append($pendtoolbar);

        $selected.append($selected_input);

        //为页面添加dom元素---结束

        $(sender).data("selectpendingData", []);//存储待选择列表里面选择的数据
        $(sender).data("selectselectedData", []);//存储已选择列表里面选择的数据

        //初始化工具栏
        var init = function () {
            $input.textbox();
            $selected.panel({
                title: "已选择数据",
                height: 80,
                bodyCls: 'pd'
            })
            $selected_input.textbox({
                width:300     
            });
            $selected_input.textbox('textbox').bind('keyup', function (e) {
                if (e.keyCode == 8) {
                    var val = $(this).val();
                    var mm = val.split(",");
                    console.log(mm)
                    mm.pop();
                    console.log(mm)
                    var mm2 = mm.join(",")
                    console.log(mm2);
                    $selected_input.textbox('setValue', mm2);
                    console.log($selected_input.textbox('getValue'));
                    console.log($selected_input.textbox('getValue'));
                }
            });
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
            $btn_select_items.linkbutton({
                iconCls: 'icon-ok',
                onClick: function () {
                    if ($(sender).data("selectpendingData").length == 0) {
                        $.messager.alert('警告', '请选择待选择列表中的数据');
                    } else {
                        if (options.selectitems) {
                            options.selectitems($(sender).data("selectpendingData"));
                        }
                    }
                }
            });
        }
        init.call(this);

        //打开dialog
        function show() {
            $(sender).data("selectpendingData", []);
            $(sender).data("selectselectedData", []);
            $(sender).data("input").textbox("setValue", "");

            $(sender).data("contains").mydialog({
                title: options.title,
                width: options.width,
                height: options.height,
                modal: true,
                buttons: [{
                    text: '关闭',
                    handler: function () {
                        $(sender).data("contains").mydialog("close");
                    }
                }],
                onOpen: function () {
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
                            selectedVal($(sender).data("selected_input"), $(sender).data("selectpendingData"))
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
                            selectedVal($(sender).data("selected_input"), $(sender).data("selectpendingData"))
                        },
                        onSelectAll: function (rows) {
                            $(sender).data("selectpendingData", rows);
                            selectedVal($(sender).data("selected_input"), $(sender).data("selectpendingData"))
                        },
                        onUnselectAll: function (rows) {
                            $(sender).data("selectpendingData", []);
                            selectedVal($(sender).data("selected_input"), $(sender).data("selectpendingData"))
                        }
                    });
                    var col_selected = $.extend(true, [], options.columns);
                }
            });
        }
        show.call(this);

        function selectedVal($element,data) {
            $element.textbox('setValue', changeString(data))
        }
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
            $(jq).data("pending").datagrid('load');
            $(jq).data("selected").datagrid('load');
            $(jq).data("selectpendingData", []);
            $(jq).data("selectselectedData", []);
        },
        searchInput: function (jq, patam) {
            $(jq).data("pending").datagrid('loadData', patam);
        },
        selectitems: null,
        unSelectitems: null
    };
    $.parser.plugins.push('assignment');

})(jQuery)
