<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ApprovedList.aspx.cs" Inherits="WebApp.Finance.Swap.ApprovedList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>待审批</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <script type="text/javascript">

        var BankData = eval('(<%=this.Model.BankData%>)');

        $(function () {
            //订单列表初始化
            $('#datagrid').myDatagrid({
                fitColumns: true,
                fit: true,
                rownumbers: true,
                singleSelect: true,
                toolbar: '#topBar'
            });

            $("#Bank").combobox({
                data: BankData,
                valueField: 'value',
                textField: 'text',
            })
        });

        //查询
        function Search() {
            var BankName = $('#Bank').combobox('getText');
            var StartDate = $('#StartDate').datebox('getValue');
            var EndDate = $('#EndDate').datebox('getValue');

            var parm = {
                BankName: BankName,
                StartDate: StartDate,
                EndDate: EndDate,
            };
            $('#datagrid').myDatagrid('search', parm);
        }

        //重置查询条件
        function Reset() {
            $('#Bank').textbox('setValue', null);
            $('#StartDate').datebox('setValue', null);
            $('#EndDate').datebox('setValue', null);
            Search();
        }
        debugger;
        //查看付汇审批
        function ViewEdit(ID) {
            var url = location.pathname.replace(/ApprovedList.aspx/ig, 'ViewEdit.aspx') + '?ID=' + ID;
            window.location = url;
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="ViewEdit(\''
                + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">审批</span>' +
                '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                '</span>' +
                '</a>';
            return buttons;
        }

    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <table style="line-height: 30px">
                <tr>
                    <td class="lbl">换汇银行: </td>
                    <td>
                        <input class="easyui-combobox" id="Bank" data-options="height:26,width:150" />
                    </td>
                    <td class="lbl">申请日期: </td>
                    <td>
                        <input class="easyui-datebox" id="StartDate" data-options="height:26,width:150,editable:false" />
                    </td>
                    <td class="lbl">至</td>
                    <td>
                        <input class="easyui-datebox" id="EndDate" data-options="height:26,width:150,editable:false" />
                    </td>
                    <td>
                        <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                        <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" title="换汇通知-待审批" data-options="
            fitColumns:true,
            fit:true,
            rownumbers:true,
            singleSelect:true,
            toolbar:'#topBar'">
            <thead>
                <tr>
                    <th data-options="field:'Creator',align:'center'" style="width: 60px">申请人</th>
                    <th data-options="field:'Currency',align:'center'" style="width: 60px">币种</th>
                    <th data-options="field:'TotalAmount',align:'center'" style="width: 60px">换汇金额</th>
                    <th data-options="field:'BankName',align:'center'" style="width: 60px">换汇银行</th>
                    <th data-options="field:'ConsignorCode',align:'center'" style="width: 80px">境外发货人</th>
                    <th data-options="field:'CreateDate',align:'center'" style="width: 60px">申请日期</th>
                    <th data-options="field:'SwapStatus',align:'center'" style="width: 60px">换汇状态</th>
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 160px">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>

