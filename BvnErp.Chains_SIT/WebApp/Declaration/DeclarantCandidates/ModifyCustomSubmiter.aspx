<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ModifyCustomSubmiter.aspx.cs" Inherits="WebApp.Declaration.DeclarantCandidates.ModifyCustomSubmiter" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>调整录入及申报员</title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js"></script>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">
        $(function () {
            //订单列表初始化
            $('#datagrid').myDatagrid({
                nowrap: false,
                fitColumns: true,
                fit: true,
                border: false,
                singleSelect: false,
            });

            $('#MyDecHead').change(function () {
                Search();
            });
        });

        //查询
        function Search() {
            var ContrNO = $('#ContrNO').textbox('getValue');
            var OrderID = $('#OrderID').textbox('getValue');

            var parm = {
                ContrNO: ContrNO,
                OrderID: OrderID,
            };
            $('#datagrid').myDatagrid('search', parm);
        }

        //重置查询条件
        function Reset() {
            $('#ContrNO').textbox('setValue', null);
            $('#OrderID').textbox('setValue', null);

            Search();
        }

        function Operation(val, row, index) {
            var buttons = '';

            buttons = buttons + '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="ModifyCustomSubmiter(\''
                + row.DecHeadID + '\',\'' + row.OrderID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">修改录入及申报员</span>' +
                '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                '</span>' +
                '</a>';

            buttons += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="ModifyCusDecStatus(\''
                + row.DecHeadID + '\',\'' + row.OrderID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">修改报关单状态</span>' +
                '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                '</span>' +
                '</a>';

            return buttons;
        }

        //单个修改发单人
        function ModifyCustomSubmiter(DecHeadID, OrderID) {
            var params = DecHeadID + "|" + OrderID;

            var url = location.pathname.replace(/ModifyCustomSubmiter.aspx/ig, 'ModifyCustomSubmiterWindow.aspx') + "?Params=" + params;

            self.$.myWindow({
                iconCls: "",
                noheader: false,
                title: '修改录入及申报员',
                width: '620',
                height: '350',
                url: url,
                onClose: function () {
                    $('#datagrid').datagrid('reload');
                }
            });
        }

        //修改报关单状态
        function ModifyCusDecStatus(DecHeadID, OrderID) {
            var url = location.pathname.replace(/ModifyCustomSubmiter.aspx/ig, 'ModifyCusDecStatusWindow.aspx') + "?ID=" + DecHeadID;

            self.$.myWindow({
                iconCls: "",
                noheader: false,
                title: '修改报关单状态',
                width: '620',
                height: '350',
                url: url,
                onClose: function () {
                    $('#datagrid').datagrid('reload');
                }
            });
        }

        //批量修改发单人
        function BatchModify() {
            var checkedItems = $('#datagrid').datagrid('getChecked');
            if (checkedItems == null || checkedItems.length <= 0) {
                $.messager.alert('提示','未选择报关单！','info');
                return;
            }

            var params = "";
            for (var i = 0; i < checkedItems.length; i++) {
                params = params + checkedItems[i].DecHeadID + "|" + checkedItems[i].OrderID + ",";
            }

            params = params.substring(0, params.length - 1);

            var url = location.pathname.replace(/ModifyCustomSubmiter.aspx/ig, 'ModifyCustomSubmiterWindow.aspx') + "?Params=" + params;

            self.$.myWindow({
                iconCls: "",
                noheader: false,
                title: '修改录入及申报员',
                width: '620',
                height: '350',
                url: url,
                onClose: function () {
                    $('#datagrid').datagrid('reload');
                }
            });
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div style="margin: 5px 0 0px 15px;">
            <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-edit'" onclick="BatchModify()">修改录入及申报员</a>
        </div>
        <div style="margin-left: 15px;">
            <ul style="list-style-type: none;">
                <li style="margin-top: 5px;">
                    <span class="lbl" style="margin-left: 10px;">合同号: </span>
                    <input class="easyui-textbox" id="ContrNO" style="width: 250px;" />
                    <span class="lbl" style="margin-left: 14px;">订单编号: </span>
                    <input class="easyui-textbox" id="OrderID" style="width: 250px;" />
                </li>
                <li style="margin-top: 5px;">
                    <%--<span class="lbl">运输批次号: </span>
                    <input class="easyui-textbox" id="VoyageID" style="width: 250px;" />--%>
                    <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()" style="margin-left: 10px;">查询</a>
                    <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                </li>
            </ul>
        </div>
    </div>
    <table id="datagrid" title="调整录入及申报员" data-options="nowrap:false,fitColumns:true,fit:true,border:false,singleSelect:false," toolbar="#topBar">
        <thead>
            <tr>
                <th data-options="field:'CheckBox',align:'center',checkbox:true," style="width: 10px;"></th>
                <th data-options="field:'ContrNo',align:'left'" style="width: 8%">合同号</th>
                <th data-options="field:'BillNo',align:'left'" style="width: 8%">提(运)单号</th>
                <th data-options="field:'OrderID',align:'left'" style="width: 10%">订单编号</th>
                <th data-options="field:'ClientCode',align:'left'" style="width: 6%">客户编号</th>
                <th data-options="field:'ClientName',align:'left'" style="width: 12%">客户名称</th>
                <th data-options="field:'StatusName',align:'left'" style="width: 8%">单据状态</th>
                <th data-options="field:'DDate',align:'left'" style="width: 8%">申报日期</th>
                <th data-options="field:'CreateDeclareAdminName',align:'left'" style="width: 8%">制单员</th>
                <th data-options="field:'CustomSubmiterName',align:'left'" style="width: 8%">录入及申报员</th>
                <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 20%">操作</th>
            </tr>
        </thead>
    </table>
</body>
</html>
