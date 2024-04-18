<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReceivedList.aspx.cs" Inherits="WebApp.GeneralManage.Receipt.Admin.ReceivedList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>已收款记录查询</title>
    <link href="../../../App_Themes/xp/Style.css" rel="stylesheet" />
    <uc:EasyUI runat="server" />
    <%--<script>
        gvSettings.fatherMenu = '综合管理(XDT)';
        gvSettings.menu = '已收款';
        gvSettings.summary = '';
    </script>--%>
    <script type="text/javascript">
        $(function () {
            //业务员下拉框初始化

            var SalesmanData = eval('(<%=this.Model.Salesman%>)');

            $('#Salesman').combobox({
                data: SalesmanData,
            });

            //列表初始化
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

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = '<a id="btnDetail" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="View(\'' + row.OrderId + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">查看</span>' +
                '<span class="l-btn-icon icon-search">&nbsp;</span>' +
                '</span>' +
                '</a>';
            return buttons;
        }

        //查看已收款状态 弹窗
        function View(ID) {
            if (ID) {
                var status = '<%=Needs.Ccs.Services.Enums.OrderReceivedStatus.Received.GetHashCode()%>';
                var url = location.pathname.replace(/ReceivedList.aspx/ig, '../NewDetail.aspx?ID=' + ID + '&Status=' + status);
                top.$.myWindow({
                    iconCls: "icon-search",
                    url: url,
                    noheader: false,
                    title: '查看订单收款',
                    width:1100,
                    height: 600,
                    onClose: function () {
                        $('#datagrid').datagrid('reload');
                    }
                });
            }
        }

        //查询
        function Search() {
            var clientCode = $('#ClientCode').textbox('getValue');
            var orderId = $('#OrderId').textbox('getValue');
            var salesman = $('#Salesman').combobox('getValue');
            var startDate = $('#StartDate').datebox('getValue');
            var endDate = $('#EndDate').datebox('getValue');
            var parm = {
                ClientCode: clientCode,
                OrderId: orderId,
                Salesman: salesman,
                StartDate: startDate,
                EndDate: endDate
            };
            $('#datagrid').myDatagrid('search', parm);
        }
        //重置查询条件
        function Reset() {
            $('#ClientCode').textbox('setValue', null);
            $('#OrderId').textbox('setValue', null);
            $('#Salesman').combobox('setValue', null);
            $('#StartDate').datebox('setValue', null);
            $('#EndDate').datebox('setValue', null);
            Search();
        }

    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <table style="line-height: 30px">
                <tr>
                    <td class="lbl">客户编号:</td>
                    <td>
                        <input class="easyui-textbox" id="ClientCode" data-options="height:26,width:200,validType:'length[1,50]'" />
                    </td>
                    <td class="lbl">订单编号:</td>
                    <td>
                        <input class="easyui-textbox" id="OrderId" data-options="height:26,width:200" />
                    </td>
                    <td class="lbl">业务员:</td>
                    <td>
                        <input class="easyui-combobox" id="Salesman" data-options="valueField:'Value',textField:'Text',editable:false,height:26,width:200" />
                    </td>
                  </tr>
                    <tr>
                        <td class="lbl">报关日期:</td>
                        <td>
                            <input class="easyui-datebox" id="StartDate" data-options="height:26,width:200," />
                        </td>
                        <td class="lbl">至</td>
                        <td>
                            <input class="easyui-datebox" id="EndDate" data-options="height:26,width:200," />
                        </td>
                        <td></td>
                        <td>
                            <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                            <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                        </td>
                    </tr>
            </table>
        </div>
    </div>

       <div id="data" data-options="region:'center',border:false">
           <table id="datagrid" title="已收款" data-options="
             nowrap:false,
            fitColumns:true,
            fit:true,
            scrollbarSize:0,
            singleSelect:false,
            toolbar:'#topBar',
            rownumbers:true">
            <thead>
                <tr>
                    <th data-options="field:'ClientCode',align:'left'" style="width: 10%;">客户编号</th>
                    <th data-options="field:'CompanyName',align:'left'" style="width: 20%;">客户名称</th>
                    <th data-options="field:'OrderId',align:'left'" style="width: 15%;">订单编号</th>
                    <th data-options="field:'OrderStatus',align:'left'" style="width: 8%;">订单状态</th>
                    <th data-options="field:'Salesman',align:'center'" style="width: 7%;">业务员</th>
                    <th data-options="field:'DeclareDate',align:'center'" style="width: 10%;">报关日期</th>
                    <th data-options="field:'DeclarePrice',align:'center'" style="width: 10%;">报关货值</th>
                    <th data-options="field:'Received',align:'center'" style="width:7%;">已收款</th>
                    <%--<th data-options="field:'Profit',align:'center'" style="width: 7%;">利润</th>--%>
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 10%;">操作</th>
                </tr>
            </thead>
        </table>
    </div>

</body>
</html>

