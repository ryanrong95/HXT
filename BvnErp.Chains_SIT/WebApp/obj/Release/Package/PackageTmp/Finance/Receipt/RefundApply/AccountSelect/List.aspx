<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Finance.Receipt.RefundApply.AccountSelect.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script src="../../../../Scripts/Ccs.js"></script>
    <link href="../../../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script>
        $(function () {
            //收款记录列表初始化
            $('#receiptRecords').myDatagrid({
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
            var StartDate = $('#StartDate').datebox('getValue');
            var EndDate = $('#EndDate').datebox('getValue');
            var ClientName = $('#ClientName').textbox('getValue');

            var parm = {
                ClientName: ClientName,
                StartDate: StartDate,
                EndDate: EndDate
            };
            $('#receiptRecords').myDatagrid('search', parm);

        }

        //重置查询条件
        function Reset() {
            $('#ClientName').textbox('setValue', null);
            $('#StartDate').datebox('setValue', null);
            $('#EndDate').datebox('setValue', null);
            Search();
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = "";

            buttons += '<a id="btnDetail" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Match(\'' + row.ID + '\',\''+row.FinanceReceiptID+'\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">匹配账号</span>' +
                '<span class="l-btn-icon icon-search">&nbsp;</span>' +
                '</span>' +
                '</a>';

            return buttons;
        }

        function Match(ApplyID,ReceiptID) {
            var url = location.pathname.replace(/List.aspx/ig, 'AccountMatch.aspx?ApplyID=' + ApplyID+'&ReceiptID='+ReceiptID);
            top.$.myWindow({
                iconCls: "icon-add",
                url: url,
                noheader: false,
                title: '匹配退款账号',
                width: 850,
                height: 500,
                overflow: "hidden",
                onClose: function () {
                    $('#receiptRecords').datagrid('reload');
                }
            });
        }

        
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <table>
                <tr>
                    <td class="lbl">申请日期:</td>
                    <td>
                        <input class="easyui-datebox" id="StartDate" data-options="editable:false" style="width: 180px;" />
                    </td>
                    <td class="lbl">至</td>
                    <td>
                        <input class="easyui-datebox" id="EndDate" data-options="editable:false" style="width: 180px;" />
                    </td>

                </tr>
                <tr>
                    <td class="lbl">退款人:</td>
                    <td>
                        <input class="easyui-textbox" id="ClientName" style="width: 180px;" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                        <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                    </td>
                </tr>
            </table>
        </div>
    </div>

    <div id="data" data-options="region:'center',border:false">
        <table id="receiptRecords" title="退款申请" data-options="nowrap:false,fitColumns:true,fit:true,toolbar:'#topBar'">
            <thead>
                <tr>
                    <th data-options="field:'CreateDate',align:'left'" style="width: 10%;">申请时间</th>
                    <th data-options="field:'CompanyName',align:'left'" style="width: 20%;">退款人</th>
                    <th data-options="field:'Amount',align:'left'" style="width: 10%;">退款金额</th>
                    <th data-options="field:'Currency',align:'left'" style="width: 10%;">币种</th>
                    <th data-options="field:'ApplicantName',align:'center'" style="width: 10%;">申请人</th>
                    <th data-options="field:'ApplyStatusDesc',align:'center'" style="width: 10%;">状态</th>
                    <th data-options="field:'Btn',align:'left',formatter:Operation" style="width: 15%;">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
