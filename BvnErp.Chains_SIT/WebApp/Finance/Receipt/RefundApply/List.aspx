<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Finance.Receipt.RefundApply.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script src="../../../Scripts/Ccs.js"></script>
    <link href="../../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script>
        $(function () {
            var refundApplyStatus = eval('(<%=this.Model.RefundApplyStatus%>)');
            $('#ApplyStatus').combobox({
                data: refundApplyStatus
            });

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
            var ApplyStatus = $('#ApplyStatus').combobox('getValue');

            var parm = {
                ClientName: ClientName,
                StartDate: StartDate,
                EndDate: EndDate,
                ApplyStatus: ApplyStatus
            };
            $('#receiptRecords').myDatagrid('search', parm);

        }

        //重置查询条件
        function Reset() {
            $('#ClientName').textbox('setValue', null);
            $('#StartDate').datebox('setValue', null);
            $('#EndDate').datebox('setValue', null);
            $('#ApplyStatus').combobox('setValue', null);
            Search();
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = "";

            if (row.ApplyStatus == 1) {
                buttons += '<a id="btnDetail" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Approve(\'' + row.ID + '\')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">审批</span>' +
                    '<span class="l-btn-icon icon-search">&nbsp;</span>' +
                    '</span>' +
                    '</a>';             
            } else {
                buttons += '<a id="btnDetail" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="View(\'' + row.ID + '\')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">查看</span>' +
                    '<span class="l-btn-icon icon-search">&nbsp;</span>' +
                    '</span>' +
                    '</a>';
            }
            return buttons;
        }

        function View(ApplyID) {
            var url = location.pathname.replace(/List.aspx/ig, 'Apply.aspx?ApplyID=' + ApplyID + '&PageSource=View');
            top.$.myWindow({
                iconCls: "icon-add",
                url: url,
                noheader: false,
                title: '查看退款申请',
                width: 850,
                height: 700,
                overflow: "hidden",
                onClose: function () {
                   
                }
            });
        }

        function Approve(ApplyID) {
            var url = location.pathname.replace(/List.aspx/ig, 'Apply.aspx?ApplyID=' + ApplyID + '&PageSource=Approve');
            top.$.myWindow({
                iconCls: "icon-add",
                url: url,
                noheader: false,
                title: '审批退款申请',
                width: 850,
                height: 750,
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
                    <td class="lbl">状态:</td>
                    <td>
                        <input class="easyui-combobox" id="ApplyStatus" data-options="valueField:'Key',textField:'Value',editable:false" style="width: 180px;" />
                    </td>
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
