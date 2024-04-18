<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ModifyDeclareCreator.aspx.cs" Inherits="WebApp.Declaration.DeclarantCandidates.ModifyDeclareCreator" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>调整制单员</title>
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

            $('#MyDecNotice').change(function () {
                Search();
            });
        });

        //查询
        function Search() {
            var OrderID = $('#OrderID').textbox('getValue');
            var ClientName = $('#ClientName').textbox('getValue');
            var VoyageID = $('#VoyageID').textbox('getValue');
            var MyDecNotice = $('#MyDecNotice').prop("checked");

            var parm = {
                OrderID: OrderID,
                ClientName: ClientName,
                VoyageID: VoyageID,
                MyDecNotice: MyDecNotice,
            };
            $('#datagrid').myDatagrid('search', parm);
        }

        //重置查询条件
        function Reset() {           
            $('#OrderID').textbox('setValue', null);
            $('#ClientName').textbox('setValue', null);
            $('#VoyageID').textbox('setValue', null);
            $('#MyDecNotice').prop('checked', false);

            Search();
        }

        function Operation(val, row, index) {
            var buttons = '';

            buttons = buttons + '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="ModifyDeclareCreator(\''
                + row.DecNoticeID + '\',\'' + row.OrderID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">修改制单员</span>' +
                '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                '</span>' +
                '</a>';

            return buttons;
        }

        //单个修改制单人
        function ModifyDeclareCreator(DecNoticeID, OrderID) {
            var params = DecNoticeID + "|" + OrderID;

            var url = location.pathname.replace(/ModifyDeclareCreator.aspx/ig, 'ModifyDeclareCreatorWindow.aspx') + "?Params=" + params;

            self.$.myWindow({
                iconCls: "",
                noheader: false,
                title: '修改制单员',
                width: '620',
                height: '350',
                url: url,
                onClose: function () {
                    $('#datagrid').datagrid('reload');
                }
            });
        }

        //批量修改制单人
        function BatchModify() {
            var checkedItems = $('#datagrid').datagrid('getChecked');
            if (checkedItems == null || checkedItems.length <= 0) {
                $.messager.alert('提示','未选择报关通知！','info');
                return;
            }

            var params = "";
            for (var i = 0; i < checkedItems.length; i++) {
                params = params + checkedItems[i].DecNoticeID + "|" + checkedItems[i].OrderID + ",";
            }

            params = params.substring(0, params.length - 1);

            var url = location.pathname.replace(/ModifyDeclareCreator.aspx/ig, 'ModifyDeclareCreatorWindow.aspx') + "?Params=" + params;

            self.$.myWindow({
                iconCls: "",
                noheader: false,
                title: '修改制单员',
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
            <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-edit'" onclick="BatchModify()">修改制单员</a>
        </div>
        <div style="margin-left: 15px;">
            <ul style="list-style-type: none;">
                <li style="margin-top: 5px;">
                    <span class="lbl" style="margin-left: 14px;">订单编号: </span>
                    <input class="easyui-textbox" id="OrderID" style="width: 250px;" />
                    <span class="lbl" style="margin-left: 10px;">客户名称: </span>
                    <input class="easyui-textbox" id="ClientName" style="width: 250px;" />
                    <%--<span style="margin-left: 10px;">
                        <input type="checkbox" name="MyDecNotice" id="MyDecNotice" style="display: none;"/>
                        <label for="MyDecNotice">我的报关通知</label>
                    </span>--%>
                    
                </li>
                <li style="margin-top: 5px;">
                    <span class="lbl">运输批次号: </span>
                    <input class="easyui-textbox" id="VoyageID" style="width: 250px;" />
                    <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()" style="margin-left: 10px;">查询</a>
                    <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                </li>
            </ul>
        </div>
    </div>
    <table id="datagrid" title="调整制单员" data-options="nowrap:false,fitColumns:true,fit:true,border:false,singleSelect:false," toolbar="#topBar">
        <thead>
            <tr>
                <th data-options="field:'CheckBox',align:'center',checkbox:true," style="width: 10px;"></th>
                <th data-options="field:'OrderID',align:'left'" style="width: 10%">订单编号</th>
                <th data-options="field:'ClientName',align:'left'" style="width: 14%">客户名称</th>
                <th data-options="field:'VoyageID',align:'left'" style="width: 8%">运输批次号</th>
                <th data-options="field:'VoyageTypeName',align:'left'" style="width: 6%">运输类型</th>
                <th data-options="field:'TotalDeclarePriceDisplay',align:'left'" style="width: 8%">报关总价</th>
                <th data-options="field:'DeclarantName',align:'left'" style="width: 6%">跟单员</th>
                <th data-options="field:'CreateDate',align:'left'" style="width: 6%">创建日期</th>
                <th data-options="field:'IcgooOrder',align:'left'" style="width: 8%">主订单号</th>
                <th data-options="field:'CreateDeclareAdminName',align:'left'" style="width: 6%">制单员</th>
                <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 8%">操作</th>
            </tr>
        </thead>
    </table>
</body>
</html>
