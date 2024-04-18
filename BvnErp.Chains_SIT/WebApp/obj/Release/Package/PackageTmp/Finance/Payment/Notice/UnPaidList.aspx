<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UnPaidList.aspx.cs" Inherits="WebApp.Finance.Payment.Notice.UnPaidList" %>

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
        gvSettings.menu = '待付款';
        gvSettings.summary = '待付款付款通知';
    </script>--%>
    <script type="text/javascript">
        var FeeType = eval('(<%=this.Model.FeeType%>)');
        $(function () {
            //下拉框数据初始化
            $('#FeeType').combobox({
                data: FeeType
            });
            //代理订单列表初始化
            $('#notices').myDatagrid({
                fitColumns: true,
                fit: true,
                nowrap: false,
                toolbar: '#topBar',
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
            $('#notices').myDatagrid('search', { StartDate: StartDate, EndDate: EndDate });
        }

        //重置查询条件
        function Reset() {
           // $('#FeeType').combobox('setValue', null);
            $('#StartDate').datebox('setValue', null);
            $('#EndDate').datebox('setValue', null);
            Search();
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Pay(' + index + ')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">付款</span>' +
                '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                '</span>' +
                '</a>';
            return buttons;
        }

        //单据号
        function FormatID(val, row, index) {
            var str = (row.CostApplyID == null ? '' : row.CostApplyID) + (row.ApplyID == null ? '' : row.ApplyID) + (row.RefundApplyID == null ? '' : row.RefundApplyID);
            return str;
        }

        //付款
        function Pay(Index) {
            $('#notices').datagrid('selectRow', Index);
            var rowdata = $('#notices').datagrid('getSelected');
            var ID = rowdata.ID;
            var ApplyID = rowdata.ApplyID;
            var CostApplyID = rowdata.CostApplyID;
            var RefundApplyID = rowdata.RefundApplyID;
            var url = location.pathname.replace(/UnPaidList.aspx/ig, 'Edit.aspx?ID=' + ID + '&ApplyID=' + ApplyID+'&CostApplyID='+CostApplyID+'&RefundApplyID='+RefundApplyID);
            window.location = url;
        }

    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <ul>
                <li>
                    <%--<span class="lbl">付款类型：</span>
                    <input class="easyui-combobox" id="FeeType" name="FeeType" data-options="valueField:'Key',textField:'Value'" />--%>
                    <span class="lbl">付款日期：</span>
                    <input class="easyui-datebox" id="StartDate" />
                    <span class="lbl">至</span>
                    <input class="easyui-datebox" id="EndDate" />
                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                </li>
            </ul>
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
                    <th data-options="field:'ID',align:'left',formatter:FormatID" style="width: 10%">单据号</th>
                    <th data-options="field:'PayeeName',align:'left'" style="width: 20%">收款方</th>
                   <%-- <th data-options="field:'FeeType',align:'center'" style="width: 10%">费用类型</th>
                    <th data-options="field:'FeeDesc',align:'center'" style="width: 15%">费用名称</th>--%>
                    <th data-options="field:'Amount',align:'center'" style="width: 8%">金额</th>
                    <th data-options="field:'Currency',align:'center'" style="width: 8%">币种</th>
                    <th data-options="field:'PayDate',align:'center'" style="width: 8%">期望付款日期</th>
                    <th data-options="field:'Summary',align:'left'" style="width: 21%">备注</th>
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 8%">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
