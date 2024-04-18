<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReceivedList.aspx.cs" Inherits="WebApp.Finance.Receipt.Notice.ReceivedList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>收款查询</title>
    <link href="../../../App_Themes/xp/Style.css" rel="stylesheet" />
    <uc:EasyUI runat="server" />
   <%-- <script>
        gvSettings.fatherMenu = '收款通知(XDT)';
        gvSettings.menu = '已收款';
        gvSettings.summary = '';
    </script>--%>
    <script type="text/javascript">
        $(function () {
            //订单列表初始化
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

        //查询
        function Search() {
            var ClientCode = $('#ClientCode').textbox('getValue');
            var ClientName = $('#ClientName').textbox('getValue');
            var SeqNo = $('#SeqNo').textbox('getValue');
            var StartDate = $('#StartDate').datebox('getValue');
            var EndDate = $('#EndDate').datebox('getValue');
            var parm = {
                ClientCode: ClientCode,
                ClientName: ClientName,
                SeqNo: SeqNo,
                StartDate: StartDate,
                EndDate: EndDate
            };
            $('#datagrid').myDatagrid('search', parm);
        }

        //重置查询条件
        function Reset() {
            $('#ClientCode').textbox('setValue', null);
            $('#ClientName').textbox('setValue', null);
            $('#SeqNo').textbox('setValue', null);
            $('#StartDate').datebox('setValue', null);
            $('#EndDate').datebox('setValue', null);
            Search();
        }

        //查看
        function ViewReceiptDetail(id) {
            if (id) {
                var url = location.pathname.replace(/ReceivedList.aspx/ig, 'Detail.aspx?ID=' + id);
                top.$.myWindow({
                    iconCls: "icon-search",
                    url: url,
                    noheader: false,
                    title: '查看订单已收款',
                    width: 980,
                    height: 700,
                    onClose: function () {
                        //$('#datagrid').datagrid('reload');
                    }
                });
            }
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="ViewReceiptDetail(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">查看</span>' +
                '<span class="l-btn-icon icon-add">&nbsp;</span>' +
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
                    <td class="lbl">客户编号:</td>
                    <td>
                        <input class="easyui-textbox" id="ClientCode" data-options="height:26,width:200,validType:'length[1,50]'" />
                    </td>
                     <td class="lbl">客户名称:</td>
                    <td>
                        <input class="easyui-textbox" id="ClientName" data-options="height:26,width:200,validType:'length[1,50]'" />
                    </td>
                    <td class="lbl">流水号:</td>
                    <td>
                        <input class="easyui-textbox" id="SeqNo" data-options="height:26,width:200,validType:'length[1,50]'" />
                    </td>
                    <td class="lbl">收款日期:</td>
                    <td>
                        <input class="easyui-datebox" id="StartDate" data-options="height:26,width:200," />
                    </td>
                    <td class="lbl">至</td>
                    <td>
                        <input class="easyui-datebox" id="EndDate" data-options="height:26,width:200," />
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
        <table id="datagrid" title="已收款通知" data-options="
            fitColumns:true,
            fit:true,
            scrollbarSize:0,
            singleSelect:false,
            toolbar:'#topBar',
            rownumbers:true">
            <thead>
                <tr>
                    <th data-options="field:'SeqNo',align:'left'" style="width: 11%;">流水号</th>
                    <th data-options="field:'ClientCode',align:'left'" style="width: 6%;">客户编号</th>
                    <th data-options="field:'ClientName',align:'left'" style="width: 14%;">客户名称</th>
                    <th data-options="field:'Amount',align:'center'" style="width: 7%;">金额</th>
                    <th data-options="field:'ClearAmount',align:'center',sortable:true" style="width: 7%;">已明确金额</th>
                    <%--<th data-options="field:'Rate',align:'center'" style="width: 6%;">汇率</th>--%>
                    <th data-options="field:'ReceiptDate',align:'center'" style="width: 8%;">收款日期</th>
                    <th data-options="field:'Vault',align:'center'" style="width:8%;">金库</th>
                    <th data-options="field:'AccountName',align:'left'" style="width: 12%;">账户名称</th>
                    <th data-options="field:'ReceiptType',align:'center'" style="width: 6%;">收款类型</th>
                    <%--<th data-options="field:'Currency',align:'center'" style="width: 6%;">币种</th>--%>
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width:7%;">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
