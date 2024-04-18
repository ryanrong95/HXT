//wangya指派插件（可以输入可以选择）

(function ($) {
    //生成随机ID
    function randomID(model) {
        return model + '_' + parseInt(Math.random() * Math.pow(10, 10))
    }
    //获取数据
    function getData(url, cb) {
        $.ajax({
            type: "get",
            url: url,
            dataType: "jsonp",
            success: function (data) {
                cb(data);
            },
            error: function () {
                alert('接口异常，请求数据失败');
            }
        });
    }
    //编写assgin插件
    $.fn.assgin = function (opt, param) {
        //如果options为string，则是方法调用
        if (typeof opt == 'string') {
            var method = $.fn.assgin.methods[opt];
            if (method) {
                return method(this, param);//如果有该方法就调用该方法
            } else {
                alert("该插件没有这个方法")
            }
        }

        var options = opt || {};
        var sender = this;

        //根据各种情况，融合options
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


        //获取待选择数据的loader方法
        function pendingLoader(param, success, error) {
            getData(options.pending.url, function (data) { success(data) });
        }
        //获取已选择数据的loader方法
        function selectedLoader(param, success, error) {
            getData(options.selected.url, function (data) { success(data) });
        }

        /*生成dom节点元素开始*/
        // 生成 dialog 容器
        var $contains = $("<div></div>");
        // 生成待选择区域
        var $pending_id = randomID("pending");
        var $pending = $("<table name=" + $pending_id + "></table>");
        //var $pending = $("<table></table>");
        // 生成已选择区域
        var $selected = $("<table></table>");
        // 搜索框工具栏
        var toolbar_id = randomID("toolbar");
        var $Toolbar = $("<div id=" + toolbar_id + " style='padding:5px;height:auto'></div>");//待选择列表工具
        var $Input = $("<input style='width:200px;height:32px'>");//搜索input框
        var $SearchPending = $("<div style='margin-left:5px;margin-right:5px;'>搜索</div>");//搜索按钮
        var $ShowAllPending = $("<div>搜索全部</div>");//展示所有待选择数据列表
        var $div = $("<div style='height:36px;'></div>");//已选择列表和待选择列表之间的间距
        //将工具集的dom元素添加到$Toolbar里
        $Toolbar.append($Input);
        $Toolbar.append($SearchPending);
        $Toolbar.append($ShowAllPending);
        // 添加到dialog容器
        $contains.append($selected);
        $contains.append($div);
        $contains.append($pending);
        $contains.append($Toolbar);
        $(sender).after($contains);
        //$('body').append($contains);

        // 待选择列表初始化
        var pendingStart = function () {
            // 搜索框初始化
            $Input.textbox();
            // 搜索按钮初始化
            $SearchPending.linkbutton({
                iconCls: 'icon-search',
                onClick: function () {
                    options.pending.Search($Input.textbox('getValue'));
                }
            });
            $ShowAllPending.linkbutton({
                iconCls: 'icon-search',
                onClick: function () {
                    $Input.textbox("setValue", "");
                    $pending.datagrid("loadData", options.pending.data); // 留意数据是否会变化，还是初始值不变 *************
                }
            });
            // 默认设置添加
            var _columns = $.extend(true, [], options.columns);
            _columns[0].push({
                field: 'btn',
                title: '操作',
                width: "90px",
                formatter: function (value, row, index) {
                    var btn = '<span class="easyui-formatted"><button id="ee" class="easyui-linkbutton" style="margin-left:5px;">选择</button></span>';
                    return btn;
                }
            });
            // 列表
            window.pendinggrid = $pending.datagrid({
                loader:pendingLoader,
                columns: _columns,
                fitColumns: true,
                rownumbers: true,
                singleSelect: true,
                onLoadSuccess: function (data) {
                    $.parser.parse('.easyui-formatted');
                    console.log(data);
                },
                toolbar: '#' + toolbar_id,
            });
        };
        // 已选择列表初始化
        var selectedStart = function () {
            // 默认设置添加
            var _columns = $.extend(true, [], options.columns);
            _columns[0].push({
                field: 'btn',
                title: '操作',
                width: "90px",
                formatter: function (value, row, index) {
                    var btn = '<span class="easyui-formatted"><button class="easyui-linkbutton" style="margin-left:5px; ">取消选择</button></span>';
                    return btn;
                }
            });
            // 列表
        window.selectedgrid = $selected.datagrid({
                loader: selectedLoader,
                columns: _columns,
                fitColumns: true,
                rownumbers: true,
                singleSelect: true,
                onLoadSuccess: function (data) {
                    $.parser.parse('.easyui-formatted');
                }
            });
        };
            // dialog弹出
        $contains.dialog({
            title: options.title,
            width: options.width,
            height: options.height,
            modal: true,
            buttons: [{
                text: '关闭',
                handler: function () {
                    $contains.dialog("close");
                }
            }],
            onOpen: function () {
                // 实例化已选择区域
                selectedStart();
                // 实例化待选择区域
                pendingStart();
                $pending.prev().on("click",".easyui-formatted",function () {
                    var index = $(this).parents("tr").attr("datagrid-row-index");
                    options.pending.onClick($pending.datagrid('getData').rows[index], index);
                })

                $selected.prev().on("click", ".easyui-formatted", function () {
                    var index = $(this).parents("tr").attr("datagrid-row-index");
                    options.selected.onClick($selected.datagrid('getData').rows[index], index);
                })
            }
        });

    }

    //指派控件名称补齐插件的默认配置
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

    //指派控件名称补齐插件对外的方法
    $.fn.assgin.methods = {
        //获取指派控件options
        refresh: function (jq) {
          
        }
    };

    //将自定义的插件加入 easyui 的插件组
    $.parser.plugins.push('assgin');

})(jQuery)
