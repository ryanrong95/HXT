<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PayeeLeftsList.aspx.cs" Inherits="WebApp.Finance.Receipt.SzStore.PayeeLeftsList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>代仓储收款弹框</title>
    <uc:EasyUI runat="server" />
    <script src="../../../Scripts/Ccs.js"></script>
    <link href="../../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">
        var CutDateIndex = getQueryString("CutDateIndex");
        var SeqNo = getQueryString("SeqNo");
        var FinanceReceiptId = getQueryString("FinanceReceiptId");
        var ClientName = getQueryString("ClientName");

        var NoticeData = eval('(<%=this.Model.NoticeData%>)');
        $(function () {
            $('#datagrid').myDatagrid({
                nowrap: false,
                border: false,
                fitColumns: true,
                scrollbarSize: 0,
                fit: true,
                singleSelect: false,
                loadFilter: function (data) {
                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];
                        for (var name in row.item) {
                            row[name] = row.item[name];
                        }
                        delete row.item;
                    }
                    return data;
                },
            });

            Init();
        });
    </script>
    <script>
        function Init() {
            $("#CutDateIndex").text(CutDateIndex);
            if (NoticeData != null) {
                $("#ClienName").text(NoticeData.ClienName);
                $("#Amount").text(NoticeData.Amount);
                $("#ClearAmount").text(NoticeData.ClearAmount);
                $("#RemainAmount").text(NoticeData.RemainAmount);
                $("#ReceiptDate").text(NoticeData.ReceiptDate);
            }
        }
        //列表框按钮
        function Operation(val, row, index) {
            var buttons = '';
            //剩余应收大于0
            if (Number(row.RemianAmount) > 0) {
                buttons += '<a id="btnDetail" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="ConfimSubmit(' + index + ')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">确认收款</span>' +
                    '<span class="l-btn-icon icon-ok">&nbsp;</span>' +
                    '</span>' +
                    '</a>';
            }
            //实收大于0
            if (Number(row.PayeeRightAmount) > 0) {
                buttons += '<a id="btnDetail" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Detail(' + index + ')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">收款记录</span>' +
                    '<span class="l-btn-icon icon-search">&nbsp;</span>' +
                    '</span>' +
                    '</a>';
            }
            return buttons;
        }
        //确认收款
        function ConfimSubmit(index) {
            //可用余额验证
            var RemainAmount = Number($("#RemainAmount").text())
            if (RemainAmount == 0) {
                $.messager.alert('提示', "改流水已没有可用余额");
            }
            var row = $("#datagrid").myDatagrid('getRows')[index];
            //基本信息
            var data = new FormData();
            data.append('ID', row.ID);
            data.append('RemainAmount', RemainAmount);
            data.append('FinanceReceiptId', FinanceReceiptId);
            data.append('SeqNo', SeqNo);
            MaskUtil.mask();
            $.ajax({
                url: '?action=ConfimSubmit',
                type: 'POST',
                data: data,
                dataType: 'JSON',
                cache: false,
                processData: false,
                contentType: false,
                success: function (res) {
                    MaskUtil.unmask();
                    if (res.success == "true") {
                        $.messager.alert('提示', '收款成功', 'info', function () {
                            window.location.reload();
                        });
                    } else {
                        $.messager.alert('提示', res.message);
                    }
                }
            })
        }
        //收款记录
        function Detail(index) {
            var data = $("#datagrid").myDatagrid('getRows')[index];
            var url = location.pathname.replace(/PayeeLeftsList.aspx/ig, 'PayeeLeftsDetails.aspx') + '?ID=' + data.ID;
            top.$.myWindow({
                iconCls: "icon-edit",
                url: url,
                title: '收款记录',
                width: '1000px',
                height: '500px',
            });
        }
    </script>
    <style>
        .lbl {
            width: 70px;
            font-weight: bold;
        }
    </style>
</head>
<body class="easyui-layout">
    <div data-options="region:'north',border:false," style="height: 50px;">
        <table>
            <tr>
                <td class="lbl">
                    <label>账单期号:</label></td>
                <td style="width: 100px">
                    <label id="CutDateIndex"></label>
                </td>
                <td class="lbl">
                    <label>客户名称:</label></td>
                <td colspan="5">
                    <label id="ClienName"></label>
                </td>
            </tr>
            <tr>
                <td class="lbl">
                    <label>付款时间:</label></td>
                <td style="width: 100px">
                    <label id="ReceiptDate"></label>
                </td>
                <td class="lbl">
                    <label>付款金额:</label></td>
                <td style="width: 100px">
                    <label id="Amount"></label>
                </td>
                <td class="lbl">
                    <label>已用金额:</label></td>
                <td style="width: 100px">
                    <label id="ClearAmount"></label>
                </td>
                <td class="lbl">
                    <label>可用金额:</label></td>
                <td>
                    <label id="RemainAmount" style="color: orangered"></label>
                </td>
            </tr>
        </table>
    </div>
    <div data-options="region:'center',border:false," title="费用列表">
        <table id="datagrid">
            <thead>
                <tr>
                    <th data-options="field:'CreateDate',align:'center'" style="width: 100px;">发生日期</th>
                    <th data-options="field:'FormID',align:'center'" style="width: 100px;">订单编号</th>
                    <th data-options="field:'Subject',align:'center'" style="width: 100px;">费用科目</th>
                    <th data-options="field:'PayeeLeftAmount',align:'center'" style="width: 100px;">应收金额</th>
                    <th data-options="field:'PayeeRightAmount',align:'center'" style="width: 100px;">实收金额</th>
                    <th data-options="field:'RemianAmount',align:'center'" style="width: 100px;">剩余应收</th>
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 200px;">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
