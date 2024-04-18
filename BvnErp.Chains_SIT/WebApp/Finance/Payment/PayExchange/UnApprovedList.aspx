<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UnApprovedList.aspx.cs" Inherits="WebApp.Finance.Payment.PayExchange.UnApprovedList" %>

<!DOCTYPE html>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>付汇审批</title>
    <uc:EasyUI runat="server" />
    <link href="../../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../../Scripts/Ccs.js"></script>
    <%--   <script>
        gvSettings.fatherMenu = '付汇审批(XDT)';
        gvSettings.menu = '待审批';
        gvSettings.summary = '';
    </script>--%>
    <script>
        //页面加载时
        $(function () {
            $('#datagrid').myDatagrid({
                fitColumns: true,
                fit: true,
                border: false,
                scrollbarSize: 0,
                toolbar: '#topBar',
                nowrap: false,
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
            var ClientCode = $("#ClientCode").textbox('getValue');
            var StartDate = $("#StartDate").datebox('getValue');
            var EndDate = $("#EndDate").datebox('getValue');
            var ApplyID = $("#ApplyID").textbox("getValue");
            var OrderID = $("#OrderID").textbox("getValue");
            $('#datagrid').myDatagrid('search', { ClientCode: ClientCode, StartDate: StartDate, EndDate: EndDate, ApplyID: ApplyID, OrderID: OrderID });
        }

        function Reset() {
            $("#ApplyID").textbox('setValue', null);
            $("#ClientCode").textbox('setValue', "");
            $('#StartDate').datebox('setValue', null);
            $('#EndDate').datebox('setValue', null);
            $("#OrderID").textbox('setValue', null);
            Search();
        }

        //审批
        function Approval(id) {
            var url = location.pathname.replace(/UnApprovedList.aspx/ig, 'Approval.aspx') + "?ApplyID=" + id;
            MaskUtil.mask();
            window.location = url;
        }

        //操作
        function Operation(val, row, index) {
            var buttons = '<a id="btnDetails" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px;" onclick="Approval(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">审批</span>' +
                '<span class="l-btn-icon icon-ok">&nbsp;</span>' +
                '</span>' +
                '</a>';

            return buttons;
        }

    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <table id="table1" style="margin: 5px 0 5px 0">
                <tr>
                    <td class="lbl">客户编号：</td>
                    <td>
                        <input class="easyui-textbox" data-options="height:26,width:150" id="ClientCode" />
                    </td>
                    <td class="lbl">申请编号：</td>
                    <td>
                        <input class="easyui-textbox" data-options="height:26,width:150" id="ApplyID" />
                    </td>
                    <td class="lbl">订单编号：</td>
                    <td>
                        <input class="easyui-textbox" data-options="height:26,width:150" id="OrderID" />
                    </td>
                    <td class="lbl">申请时间：</td>
                    <td>
                        <input class="easyui-datebox" data-options="height:26,width:150" id="StartDate" />
                    </td>
                    <td class="lbl">至</td>
                    <td>
                        <input class="easyui-datebox" data-options="height:26,width:150" id="EndDate" />
                    </td>
                    <td style="padding-left: 5px">
                        <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                        <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" class="mygrid" title="付汇申请记录">

            <thead>
                <tr>
                    <th field="ID" data-options="align:'center'" style="width: 10%">申请编号</th>
                    <th field="CreateDate" data-options="align:'center'" style="width: 6%">申请日期</th>
                    <th field="ClientCode" data-options="align:'center'" style="width: 6%">客户编号</th>
                    <th field="TotalAmount" data-options="align:'center'" style="width: 6%">金额</th>
                    <th field="Currency" data-options="align:'center'" style="width: 6%">币种</th>
                    <th field="SupplierName" data-options="align:'left'" style="width: 15%">供应商名称</th>
                    <%--<th field="SupplierEnglishName" data-options="align:'left'" style="width: 50px">英文名称</th>--%>
                    <th field="BankName" data-options="align:'left'" style="width: 15%">银行名称</th>
                    <th field="BankAccount" data-options="align:'left'" style="width: 10%">银行账号</th>
                    <th field="Status" data-options="align:'center'" style="width: 6%">状态</th>
                    <th field="IsAdvanceMoney" data-options="align:'center'" style="width: 6%">是否垫款</th>
                    <th field="FatherID" data-options="align:'center'" style="width: 5%">付汇类型</th>
                    <th data-options="field:'btn',width:'8%',formatter:Operation,align:'center'">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
