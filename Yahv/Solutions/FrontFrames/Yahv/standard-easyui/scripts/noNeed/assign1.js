//指派插件wjl

(function ($) {
    //公共方法
    //接口地址
    var AjaxUrl = {
        PendingUrl: "/rfqapi/offers/List?id=",//待选择数据
        SelectedUrl: "/rfqapi/offers/offered?id=",//已选择数据
        OfferUrl: "/rfqapi/offers/Offer",//选择数据操作接口
        CancelOfferUrl: "/rfqapi/offers/CancelOffer"//取消选择接口
    }
    //获取数据
    function getData(url, id, cb) {
        $.ajax({
            type: "get",
            url: url + id,
            dataType: "jsonp",
            success: function (data) {
                cb(data);
            },
            error: function () {
                console.log('接口异常，请求数据失败');
            }
        });
    }
    //根据key值获取待选择数据
    function getPendingDataByKey(id, key, cb) {
        $.ajax({
            type: "get",
            url: AjaxUrl.PendingUrl + id + "&key=" + key,
            dataType: "jsonp",
            success: function (data) {
                cb(data);
            },
            error: function () {
                console.log('接口异常，请求数据失败');
            }
        });
    }
    //生成随机ID
    function randomID(model) {
        return model + '_' + parseInt(Math.random() * Math.pow(10, 10))
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

        /*公共方法开始*/

        //获取待选择数据的loader方法
        function pendingLoader1(param, success, error) {
            getData(AjaxUrl.PendingUrl, options.id, function (data) { success(data) });
        }
        //获取已选择数据的loader方法
        function selectedLoader1(param, success, error) {
            getData(AjaxUrl.SelectedUrl, options.id, function (data) { success(data) });
        }

        /*公共方法结束*/


        /*生成dom节点元素开始*/
        var pending_id = randomID("pending");
        var $Pending = $("<table id=" + pending_id + "></table>");//待选择列表
        var $Selected = $("<table id=" + randomID("selected") + "></table>");//已选择列表

        var toolbar_id = randomID(pending_id + "toolbar");
        var $Toolbar = $("<div id=" + toolbar_id + " style='padding:5px;height:auto'></div>");//待选择列表工具

        var $Input = $("<div id=" + randomID("search_input") + "></div>");//搜索input框


        var $SearchPending = $("<div style='margin-left:5px;margin-right:5px;'>" + options.PendingList.toolbar.searchDataText + "</div>");//搜索按钮

        var $ShowAllPending = $("<div>" + options.PendingList.toolbar.showAllDataText + "</div>");//展示所有待选择数据列表
        var $div = $("<div style='height:36px;'></div>");//已选择列表和待选择列表之间的间距

        //将工具集的dom元素添加到$Toolbar里
        $Toolbar.append($Input);
        $Toolbar.append($SearchPending);
        $Toolbar.append($ShowAllPending);

        //将所有的元素添加到this里面
        $(sender).append($Selected);
        $(sender).append($div);
        $(sender).append($Pending);
        $(sender).append($Toolbar);
        /*生成dom节点元素结束*/


        /*初始化所有的dom节点开始*/
        //初始化搜索待选择数据的input框
        $Input.textbox({
            width: options.PendingList.toolbar.searchDataWidth,
            height: options.PendingList.toolbar.searchDataHeight,
        });

        //初始化搜索按钮
        var searchDataFun = options.PendingList.toolbar.searchDataFun != null ? options.PendingList.toolbar.searchDataFun : searchPendingData;
        $SearchPending.linkbutton({
            iconCls: 'icon-yg-search',
            onClick: function () {
                searchDataFun($Input)
            }
        });

        //初始化展示所有待选择的全部数据的那妞
        var showAllDataFun = options.PendingList.toolbar.showAllDataFun != null ? options.PendingList.toolbar.showAllDataFun : showAllPendingData;
        $ShowAllPending.linkbutton({
            onClick: function () {
                showAllDataFun($Input)
            }
        });


        //初始化已选择列表开始
        var SelectedFormatter = options.SelectedList.OperationButton.generatingButton != null ? options.SelectedList.OperationButton.generatingButton : SelectedFormatter1;
        var SelectedColumns1 = [[
            { field: 'ID', title: 'ID', width: "100px" },
            { field: 'UserName', title: '用户名', width: "80px" },
            { field: 'RealName', title: '真实姓名', width: "90px" },
            { field: 'btn', title: '操作', width: "90px", formatter: SelectedFormatter },
        ]];
        var SelectedColumns = options.SelectedList.columns != null ? options.SelectedList.columns : SelectedColumns1;
        var selectedLoader = options.SelectedList.loader != null ? options.SelectedList.loader : selectedLoader1;

        $Selected.datagrid({
            data: options.SelectedList.data,
            loader: selectedLoader,
            fitColumns: true,
            columns: SelectedColumns,
            rownumbers: true,
            singleSelect: true,
            onLoadSuccess: function (data) {
                $.parser.parse('.easyui-formatted');
            }
        });
        //初始化已选择列表结束


        //初始化待选择列表开始
        var PendingFormatter = options.PendingList.OperationButton.generatingButton != null ? options.PendingList.OperationButton.generatingButton : PendingFormatter1;
        var PendingColumns1 = [[
            { field: 'ID', title: 'ID', width: "100px" },
            { field: 'UserName', title: '用户名', width: "80px" },
            { field: 'RealName', title: '真实姓名', width: "90px" },
            { field: 'btn', title: '操作', width: "90px", formatter: PendingFormatter },
        ]];
        var PendingColumns = options.PendingList.columns != null ? options.PendingList.columns : PendingColumns1;
        var PendingLoader = options.PendingList.loader != null ? options.PendingList.loader : pendingLoader1;

        //选择函数
        function assgin(ID) {
            $.ajax({
                type: "get",
                url: AjaxUrl.OfferUrl + "?id=" + options.id + "&items=" + ID,
                dataType: "jsonp",
                success: function (data) {
                    if (data.success) {
                        getData(AjaxUrl.PendingUrl, options.id, function (PendingData) {
                            $Pending.datagrid("loadData", PendingData);
                        })
                        getData(AjaxUrl.SelectedUrl, options.id, function (SelectedData) {
                            $Selected.datagrid("loadData", SelectedData);
                        })
                    }
                },
                error: function () {
                    console.log('接口异常，请求数据失败');
                }
            });
        }
        //取消选择函数
        function cancelassgin(ID) {
            $.ajax({
                type: "get",
                url: AjaxUrl.CancelOfferUrl + "?id=" + options.id + "&items=" + ID,
                dataType: "jsonp",
                success: function (data) {
                    console.log(data);
                    if (data.success) {
                        getData(AjaxUrl.PendingUrl, options.id, function (SelectedData) {
                            $Pending.datagrid("loadData", SelectedData);
                        })
                        getData(AjaxUrl.SelectedUrl, options.id, function (PendingData) {
                            $Selected.datagrid("loadData", PendingData);
                        })
                    }
                },
                error: function () {
                    console.log('接口异常，请求数据失败');
                }
            });
        }

        //生成待选择数据的方法
        function PendingFormatter1(value, row, index) {
            var PendingClick = options.PendingList.OperationButton.onClick != null ? options.PendingList.OperationButton.onClick : assgin;
            var btn = '<span class="easyui-formatted"><button class="easyui-linkbutton" style="margin-left:5px; "' + options.PendingList.OperationButton.text + '>';

            $(btn).attr('onclick', function () {
                alert(111);
            });
            return btn;


            //  return ['<span class="easyui-formatted"></button></span>',
            //, '<button class="easyui-linkbutton" style="margin-left:5px; onclick="PendingClick(\'' + row.ID + '\');">' + options.PendingList.OperationButton.text + '</button>'
            //, '</span>'].join('');
            //  return arry.join('');
        }
        //生成已选择数据的方法
        function SelectedFormatter1(value, row, index) {
            SelectedClick = options.SelectedList.OperationButton.onClick != null ? options.SelectedList.OperationButton.onClick : cancelassgin;
            return ['<span class="easyui-formatted">',
          , '<button class="easyui-linkbutton" style="margin-left:5px;"  onclick="SelectedClick(\'' + row.ID + '\');">' + options.SelectedList.OperationButton.text + '</button>'
          , '</span>'].join('');
            return arry.join('');
        }

        $Pending.datagrid({
            data: options.PendingList.data,
            loader: PendingLoader,
            columns: PendingColumns,
            fitColumns: true,
            rownumbers: true,
            singleSelect: true,
            onLoadSuccess: function (data) {
                $.parser.parse('.easyui-formatted');
                //$Pending.find(".easyui-formatted").find(".easyui-linkbutton").linkbutton({
                //    onClick: function () {
                //        var w = $Pending.find(".easyui-formatted").index();
                //        console.log(w);
                //        console.log($Pending.datagrid("getRows"));
                //        //PendingClick(row.ID);
                //    }
                //})
            },
            toolbar: '#' + toolbar_id,
        });

        //根据input框搜索待选择数据
        function searchPendingData($ele) {
            var key = $ele.textbox("getValue");
            if (key && key != "") {
                getPendingDataByKey(options.id, key, function (data) {
                    $Pending.datagrid("loadData", data);
                })
            }
        }
        //搜索所有的待选择数据
        function showAllPendingData($ele) {
            $ele.textbox("setValue", "");
            getData(AjaxUrl.PendingUrl, options.id, function (data) {
                $Pending.datagrid("loadData", data);
            });
        }



        //初始化待选择列表结束

        //初始化弹出框
        var $dialog = $(sender).dialog({
            title: options.dialog.title,
            width: options.dialog.width,
            height: options.dialog.height,
            closed: options.dialog.closed,
            modal: options.dialog.modal,
            buttons: [{
                text: options.dialog.closeText,
                handler: function () {
                    $(sender).dialog("close");
                }
            }]
        });
        /*初始化所有的dom节点结束*/
    }

    //指派控件名称补齐插件的默认配置
    $.fn.assgin.defaults = {
        id: "",//某个指派任务的id,必写参数
        //弹出框配置
        dialog: {
            title: "操作列表",//弹出框的标题
            width: 700,
            height: 500,
            //closed: true,
            modal: true,
            buttons: [{
                text: '关闭',
                handler: function () { }
            }],
            closeText: '关闭'
        },
        //待选择列表
        PendingList: {
            title: '待选择列表',
            fitColumns: true,
            rownumbers: true,
            singleSelect: true,
            data: null,
            url: null,
            loader: null,
            columns: null,
            onLoadSuccess: function (data) {
                $.parser.parse('.easyui-formatted');
            },
            //操作按钮
            OperationButton: {
                generatingButton: null,//生成按钮的方法
                text: "选择",//按钮的文字
                onClick: null,//点击按钮执行的方法
            },
            //工具
            toolbar: {
                searchDataText: '搜索',//搜索数据的文字
                searchDataFun: null,//搜索数据
                searchDataWidth: 200,
                searchDataHeight: 30,
                showAllDataText: '展示待选择列表',//展示全部数据的文字
                showAllDataFun: null//展示全部数据
            }
        },
        //已选择列表
        SelectedList: {
            title: '已选择列表',
            fitColumns: true,
            rownumbers: true,
            singleSelect: true,
            data: null,
            url: null,
            columns: null,
            loader: null,
            onLoadSuccess: function (data) {
                $.parser.parse('.easyui-formatted');
            },
            //操作按钮
            OperationButton: {
                generatingButton: null,//生成按钮的方法
                text: "取消选择",//按钮的文字
                onClick: null,//点击按钮执行的方法
            }
        }
    };

    //指派控件名称补齐插件对外的方法
    $.fn.assgin.methods = {
        //获取指派控件options
        options: function (jq) {
            return 0;
        }
    };

    //将自定义的插件加入 easyui 的插件组
    $.parser.plugins.push('assgin');
})(jQuery)