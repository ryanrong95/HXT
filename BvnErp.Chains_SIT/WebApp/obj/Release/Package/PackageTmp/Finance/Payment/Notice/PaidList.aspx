<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PaidList.aspx.cs" Inherits="WebApp.Finance.Payment.Notice.PaidList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script src="../../../Scripts/Ccs.js"></script>
    <link href="../../../App_Themes/xp/Style.css" rel="stylesheet" />
    <%--<script>
        gvSettings.fatherMenu = '付款通知(XDT)';
        gvSettings.menu = '已付款';
        gvSettings.summary = '已付款付款通知';
    </script>--%>
    <script type="text/javascript">

        var FeeType = eval('(<%=this.Model.FeeType%>)');

        $(function () {
            //下拉框数据初始化
            //$('#FeeType').combobox({
            //    data: FeeType
            //});
            //代理订单列表初始化
            $('#notices').myDatagrid({
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

        //查询
        function Search() {
            //var FeeType = $('#FeeType').combobox('getValue');
            var StartDate = $('#StartDate').datebox('getValue');
            var EndDate = $('#EndDate').datebox('getValue');
            $('#notices').myDatagrid('search', {  StartDate: StartDate, EndDate: EndDate });
        }

        //重置查询条件
        function Reset() {
            //$('#FeeType').combobox('setValue', null);
            $('#StartDate').datebox('setValue', null);
            $('#EndDate').datebox('setValue', null);
            Search();
        }

        //查看订单
        function Look(Index) {
            $('#notices').datagrid('selectRow', Index);
            var rowdata = $('#notices').datagrid('getSelected');
            var ID = rowdata.ID;
            var ApplyID = rowdata.ApplyID;
            var CostApplyID = rowdata.CostApplyID;     
            var url = location.pathname.replace(/PaidList.aspx/ig, 'Edit.aspx?ID=' + ID + '&ApplyID=' + ApplyID+'&CostApplyID='+CostApplyID);
            window.location = url;
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Look(' + index+ ')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">查看</span>' +
                '<span class="l-btn-icon icon-search">&nbsp;</span>' +
                '</span>' +
                '</a>';
            return buttons;
        }

        //单据号
        function FormatID(val, row, index) {
            var str = (row.CostApplyID == null ? '' : row.CostApplyID) + (row.ApplyID == null ? '' : row.ApplyID);
            return str;
        }

    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <table style="line-height: 26px">
                <tr>
                   <%-- <td class="lbl">付款类型：</td>
                    <td>
                        <input class="easyui-combobox" id="FeeType" name="FeeType" style="height: 26px; width: 200px"
                            data-options="valueField:'Key',textField:'Value'" />
                    </td>--%>
                    <td class="lbl">付款日期：</td>
                    <td>
                        <input class="easyui-datebox" id="StartDate" style="height: 26px; width: 200px" />
                    </td>
                    <td class="lbl">至</td>
                    <td>
                        <input class="easyui-datebox" id="EndDate" style="height: 26px; width: 200px" />
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
        <table id="notices" title="付款通知" data-options="
            fitColumns:true,
            fit:true,
            nowrap:false,
            toolbar:'#topBar'">
            <thead>
                <tr>
                    <th data-options="field:'SeqNo',align:'left'" style="width: 12%">流水号</th>
                     <th data-options="field:'ID',align:'left',formatter:FormatID" style="width: 10%">单据号</th>
                    <th data-options="field:'PayeeName',align:'left'" style="width: 18%">收款方</th>
                   <%-- <th data-options="field:'FeeType',align:'center'" style="width: 7%">费用类型</th>
                    <th data-options="field:'FeeDesc',align:'center'" style="width: 11%">费用名称</th>--%>
                    <th data-options="field:'Amount',align:'center'" style="width: 7%">金额</th>
                    <th data-options="field:'Currency',align:'center'" style="width: 6%">币种</th>
                    <th data-options="field:'PayDate',align:'center'" style="width: 8%">期望付款日期</th>
                    <th data-options="field:'Summary',align:'left'" style="width: 20%">备注</th>
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 8%">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
