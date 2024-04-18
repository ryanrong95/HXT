<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UnExitedList.aspx.cs" Inherits="WebApp.HKWarehouse.Exit.UnExitedList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>出库通知—待出库</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <script>
        //gvSettings.fatherMenu = '出库通知(HK)';
        //gvSettings.menu = '待出库';
        //gvSettings.summary = '出库操作';
    </script>
    <script>
        //页面加载时
        $(function () {
            $('#datagrid').myDatagrid({
                loadFilter: function (data) {
                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];
                        for (var name in row.item) {
                            row[name] = row.item[name];
                        }
                        delete row.item;
                    }
                    return data;
                }
            });
        });

        function Search() {
            var orderID = $('#OrderID').textbox('getValue');
            var VoyNo = $('#VoyNo').textbox('getValue');
            $('#datagrid').myDatagrid('search', { OrderID: orderID, VoyNo: VoyNo });
        }

        function Reset() {
            $("#OrderID").textbox('setValue', "");
            $("#VoyNo").textbox('setValue', "");
            $("#AdminName").textbox('setValue', "");
            Search();
        }

        function Exit(ID) {
            var url = location.pathname.replace(/UnExitedList.aspx/ig, 'OutStock.aspx') + "?ExitNoticeID=" + ID + "&Status=" + 1;
            window.location = url;
        }

        function Operation(val, row, index) {
            var buttons = '<a id="btnPrint" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px;" onclick="Exit(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">出库</span>' +
                '<span class="l-btn-icon icon-ok">&nbsp;</span>' +
                '</span>' +
                '</a>';
            return buttons;
        }

        // 一键出库
        function OutStock() {
            var selectRows = $("#datagrid").datagrid("getSelections");
            //如果没有选中行的话，提示信息
            if (selectRows.length < 1) {
                $.messager.alert("提示消息", "请勾选需要出库的项！", 'info');
                return;
            }
            var strIds = "";
            //拼接字符串，这里也可以使用数组，作用一样
            //循环切割
            for (var i = 0; i < selectRows.length; i++) {
                strIds += selectRows[i].ID + ",";
            }
            strIds = strIds.substr(0, strIds.length - 1);
            if (strIds.length > 0) {
                MaskUtil.mask();//遮挡层
                $.post('?action=OutStock', {
                    IDs: strIds,
                }, function (result) {
                    MaskUtil.unmask();//关闭遮挡层
                    var rel = JSON.parse(result);
                    $.messager.alert('消息', rel.message, 'info', function () {
                        if (rel.success) {
                            $('#datagrid').myDatagrid('reload');
                        }
                    });
                })
            }
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <table id="table1">
                <tr>
                    <td colspan="2">
                        <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-ok'" onclick="OutStock()">一键出库</a>
                    </td>
                </tr>
                <tr>
                    <td class="lbl">订单编号：</td>
                    <td>
                        <input class="easyui-textbox" data-options="height:26,width:180" id="OrderID" />
                    </td>
                    <td class="lbl" style="padding-left: 5px">运输批次号：</td>
                    <td>
                        <input class="easyui-textbox" data-options="height:26,width:180" id="VoyNo" />
                    </td>
                    <td style="padding-left: 5px">
                        <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                    </td>
                    <td style="padding-left: 5px">
                        <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" title="待出库" data-options="
            fitColumns:true,
            fit:true,
            border:false,
            scrollbarSize:0,
            singleSelect:false,
            toolbar:'#topBar'">
            <thead>
                <tr>
                    <th field="ck" checkbox="true"></th>
                    <th field="VoyNo" data-options="align:'center'" style="width: 50px">运输批次号</th>
                    <th field="OrderID" data-options="align:'center'" style="width: 50px">订单编号</th>
                    <th field="ClientName" data-options="align:'left'" style="width: 100px">客户名称</th>
                    <th field="PackNo" data-options="align:'center'" style="width: 30px">件数</th>
                    <th field="CreateDate" data-options="align:'center'" style="width: 30px">创建日期</th>
                    <th field="NoticeStatus" data-options="align:'center'" style="width: 30px">状态</th>
                    <th data-options="field:'btnPacking',width:30,formatter:Operation,align:'center'">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
