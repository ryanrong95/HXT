<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Confirm.aspx.cs" Inherits="Yahv.PvWsOrder.WebApp.Invoice.Confirm" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script src="../Content/Themes/Scripts/Ccs.js"></script>
    <script>
        $(function () {
            $('#productGrid').myDatagrid({
                rownumbers: true,
                fitColumns: false,
                fit: false,
                nowrap: false,
                pagination: false,
                onClickRow: onClickRow,
                onAfterEdit: onAfterEdit,
                onCancelEdit: onCancelEdit,
                nowrap: false,
                actionName: 'ProductData',
                onLoadSuccess: function (data) {
                    $('#productGrid').datagrid('appendRow', {
                        Amount: data.totaldata.Amount,
                        Difference: data.totaldata.Difference,
                        Quantity: data.totaldata.Quantity,
                        SalesTotalPrice: data.totaldata.SalesTotalPrice,
                        InvoiceNo: "",
                    });

                    //"合计"标题
                    setHejiTitle();

                    //"其它信息"中"含税金额"显示(已经这个为准,不要使用OtherData中的,因为后来加了含税金额的计算方法)
                    $("#NoticeAmount").text(data.totaldata.Amount);

                    var heightValue = $("#productGrid").prev().find(".datagrid-body").find(".datagrid-btable").height() + 60;
                    $("#productGrid").prev().find(".datagrid-body").height(heightValue);
                    $("#productGrid").prev().height(heightValue);
                    $("#productGrid").prev().parent().height(heightValue);
                    $("#productGrid").prev().parent().parent().height(heightValue);

                },
            });
            Init();

            ////绑定日志信息
            //var id = getQueryString("InvoiceNoticeID");
            //var data = new FormData($('#form1')[0]);
            //data.append("ID", id);
            //ajaxLoading();
            //$.ajax({
            //    url: '?action=LoadInvoiceLogs',
            //    type: 'POST',
            //    data: data,
            //    dataType: 'JSON',
            //    cache: false,
            //    processData: false,
            //    contentType: false,
            //    success: function (data) {
            //        ajaxLoadEnd();
            //        showLogContent(data);
            //    },
            //    error: function (msg) {
            //        ajaxLoadEnd();
            //        alert("ajax连接异常：" + msg);
            //    }
            //});
        });

        function Init() {
            if (model.InvoiceData != null) {
                $("#InvoiceType").text(model.InvoiceData.InvoiceType);
                $("#DeliveryType").text(model.InvoiceData.DeliveryType);
                $("#CompanyName").text(model.InvoiceData.CompanyName);
                $("#TaxCode").text(model.InvoiceData.TaxCode);
                $("#BankInfo").text(model.InvoiceData.BankInfo);
                $("#AddressTel").text(model.InvoiceData.AddressTel);
            };
            if (model.MaileDate != null) {
                $("#ReceipCompany").text(model.MaileDate.ReceipCompany);
                $("#ReceiterName").text(model.MaileDate.ReceiterName);
                $("#ReceiterTel").text(model.MaileDate.ReceiterTel);
                $("#DetailAddres").text(model.MaileDate.DetailAddres);
                $("#WaybillCode").text(model.MaileDate.WaybillCode);
            }
            if (model.OtherData != null) {
                //$("#NoticeAmount").text(model.OtherData.Amount);
                $("#NoticeDifference").text(model.OtherData.Difference);
                $("#Summary").text(model.OtherData.Summary);
            }
        }

        //完成开票
        function ConfirmInvoice() {

            //结束表单编辑状态
            endEditing()

            //验证表单数据
            if (!Valid('form1')) {
                return;
            }
            //验证发票号
            if (!VerifyInvoiceNo()) {
                $.messager.alert('提示', "发票号码不能为空");
                return;
            }

            //提交表单
            var data = new FormData($('#form1')[0]);
            var products = $('#productGrid').datagrid('getRows');
            var realProducts = [];
            for (var i = 0; i < products.length - 1; i++) {
                realProducts.push(products[i]);
            }

            var InvoiceNoticeID = getQueryString("InvoiceNoticeID");
            data.append('Data', JSON.stringify(realProducts));
            data.append('InvoiceNoticeID', InvoiceNoticeID);
            //data.append("InvoiceTypeValue",InvoiceData.InvoiceTypeValue)
            ajaxLoading();
            $.ajax({
                url: '?action=ConfirmInvoice',
                type: 'POST',
                data: data,
                dataType: 'JSON',
                cache: false,
                processData: false,
                contentType: false,
                success: function (res) {
                    ajaxLoadEnd();
                    if (res.success) {
                        $.messager.alert('', res.message, 'info', function () {
                            Back();
                        });
                    } else {
                        $.messager.alert('提示', res.message);
                    }
                }
            }).done(function (res) {
                if (res.success) {
                    $.messager.alert('', res.message, 'info', function () {
                        Back();
                    });
                } else {
                    $.messager.alert('提示', res.message);
                }
            });
        }

        //打印发票运单
        function PrintWaybill() {
            //结束表单编辑状态
            endEditing()
            //验证发票号
            if (!VerifyInvoiceNo()) {
                $.messager.alert('提示', "发票号码不能为空");
                return;
            }

            var InvoiceNoticeID = getQueryString("InvoiceNoticeID");
            var url = location.pathname.replace(/Confirm.aspx/ig, 'PrintInvoice.aspx?IDs=' + InvoiceNoticeID);
            top.$.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '打印发票运单',
                width: '750px',
                height: '550px',
                onClose: function () {
                    //刷新页面
                    location.reload();
                }
            });
        }

        //打印确认单
        function Print() {
            //结束表单编辑状态
            endEditing()
            //验证发票号
            if (!VerifyInvoiceNo()) {
                $.messager.alert('提示', "发票号码不能为空");
                return;
            }

            var InvoiceNoticeID = getQueryString("InvoiceNoticeID");
            var url = location.pathname.replace(/Confirm.aspx/ig, 'PrintInvoiceConfirm.aspx?ID=' + InvoiceNoticeID);
            top.$.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '打印确认单',
                width: '750px',
                height: '550px',
                onClose: function () {
                    location.reload();
                }
            });
        }

        //返回
        function Back() {
            var url = location.pathname.replace(/Confirm.aspx/ig, 'InvoicingList.aspx');
            window.location = url;
        }

        //显示日志数据
        function showLogContent(data) {
            var str = "";//定义用于拼接的字符串
            $.each(data.rows, function (index, row) {
                if (row.Summary != null) {
                    str = "<p>" + "&nbsp;&nbsp;" + row.CreateDate + "&nbsp;," + row.Summary + "</p>"
                }
                //追加到table中
                $("#LogContent").append(str);
            });
        }

    </script>
    <script>
        var editIndex = undefined;
        function endEditing() {
            if (editIndex == undefined) { return true }
            if ($('#productGrid').datagrid('validateRow', editIndex)) {
                var rows = $('#productGrid').datagrid('getRows');
                var oldValue = rows[editIndex].InvoiceNo;
                $('#productGrid').datagrid('endEdit', editIndex);
                $('#productGrid').datagrid('acceptChanges');
                var newValue = rows[editIndex].InvoiceNo;
                if (newValue != oldValue) {
                    UpdateInvoiceNo(editIndex);
                }

                editIndex = undefined;

                return true;
            } else {
                return false;
            }
        }
        function onClickRow(index) {
            var dataLength = $("#productGrid-container .datagrid-cell-rownumber").length;
            if (index == (dataLength - 1)) {
                return;
            }

            if (editIndex != index) {
                if (endEditing()) {
                    $('#productGrid').datagrid('selectRow', index)
                    $('#productGrid').datagrid('beginEdit', index);
                    editIndex = index;
                } else {
                    $('#productGrid').datagrid('selectRow', editIndex);
                }
            }
            else {
                var rows = $('#productGrid').datagrid('getRows');
                var oldValue = rows[index].InvoiceNo;
                $('#productGrid').datagrid('acceptChanges');
                var newValue = rows[index].InvoiceNo;
                if (newValue != oldValue) {
                    UpdateInvoiceNo(index);
                }
                editIndex = undefined;
            }

            //"合计"标题
            setHejiTitle();
        }
        //验证发票号是否为空
        function VerifyInvoiceNo() {
            $('#productGrid').datagrid('acceptChanges');
            var rows = $('#productGrid').datagrid('getRows');

            for (var i = 0; i < rows.length - 1; i++) {
                var row = rows[i];
                if (row["InvoiceNo"] == "" || row["InvoiceNo"] == null) {
                    return false;
                }
            }
            return true;
        }
        //更新发票号到数据库
        function UpdateInvoiceNo(Index) {
            //提交表单
            var InvoiceNoticeID = getQueryString("InvoiceNoticeID");
            var data = new FormData($('#form1')[0]);
            var products = $('#productGrid').datagrid('getRows');
            var product = products[Index];
            var select = new Array();
            for (var i = Index; i < products.length - 1; i++) {
                products[i].InvoiceNo = product.InvoiceNo;
                select.push(products[i]);
                $('#productGrid').datagrid('refreshRow', i);
            }

            data.append('Data', JSON.stringify(select));
            data.append('InvoiceNoticeID', InvoiceNoticeID);
            $.ajax({
                url: '?action=UpdateInvoiceNo',
                type: 'POST',
                data: data,
                dataType: 'JSON',
                cache: false,
                processData: false,
                contentType: false,
                success: function (res) {
                }
            });
        }

        //"合计"标题
        function setHejiTitle() {
            var rownumberCells = $("#productGrid-container .datagrid-cell-rownumber");
            if (rownumberCells.length > 0) {
                $(rownumberCells[rownumberCells.length - 1]).html("合计");

                $("#productGrid-container tr[datagrid-row-index=" + (rownumberCells.length - 1) + "] td[field=InvoiceNo]").html("")
            }
        }

        function onAfterEdit() {
            setHejiTitle();
        }

        function onCancelEdit() {
            setHejiTitle();
        }
    </script>
    <style type="text/css">
        table.row-info tr td:first-child {
            width: 100px;
        }

        #productGrid-container .datagrid-header-rownumber, #productGrid-container .datagrid-cell-rownumber {
            width: 50px;
        }

        .container {
            margin-top: 10px;
            margin-left: 20px;
            padding-right: 40px;
        }

        .sec-container {
            margin: 1px;
            padding: 1px;
        }

        .sub-container {
            margin: 10px;
        }

        table.row-info {
            border-top: 1px dashed #cccccc;
        }

            table.row-info tr td {
                border-collapse: collapse;
                border-bottom: 1px dashed #cccccc;
                height: 22px;
                padding-left: 5px;
            }

                table.row-info tr td:first-child {
                    width: 80px;
                    background-color: #f3f3f3;
                }

        table.file-info tr td {
            padding-left: 5px;
        }

        table.file-info span {
            color: cornflowerblue;
        }

        table.file-info a:nth-child(2) span {
            margin-left: 10px;
        }

        .text-container p {
            margin: 5px;
        }

        .icon-blue-fujian {
            background: url('./images/blue-fujian.png') no-repeat center center;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div id="tt" class="easyui-tabs" style="width: auto;">
        <div title="开票确认" style="display: none; padding: 5px;">
            <div data-options="region:'north',border: false," style="overflow-y: hidden;">
                <div class="sub-container" style="height: 20px;">
                    <a class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="ConfirmInvoice()">完成开票</a>
                    <a class="easyui-linkbutton" data-options="iconCls:'icon-print'" onclick="PrintWaybill()">打印发票运单</a>
                    <a class="easyui-linkbutton" data-options="iconCls:'icon-print'" onclick="Print()">打印确认单</a>
                    <a class="easyui-linkbutton" data-options="iconCls:'icon-back'" onclick="Back()">返回</a>
                </div>
            </div>
            <div data-options="region:'center',border: false," style="width: 100%; float: left;">
                <div class="sec-container">
                    <div style="display: block; float: left; width: 48%">
                        <div id="panel1" class="easyui-panel" title="开票信息">
                            <div class="sub-container">
                                <table class="row-info" style="width: 100%;" cellspacing="0" cellpadding="0">
                                    <tr>
                                        <td>
                                            <label>开票类型：</label></td>
                                        <td>
                                            <label id="InvoiceType"></label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <label>交付方式：</label></td>
                                        <td>
                                            <label id="DeliveryType"></label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <label>公司名称：</label></td>
                                        <td>
                                            <label id="CompanyName"></label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <label>纳税人识别号：</label></td>
                                        <td>
                                            <label id="TaxCode"></label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <label>开户行及账号：</label></td>
                                        <td>
                                            <label id="BankInfo"></label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <label>地址 电话：</label></td>
                                        <td>
                                            <label id="AddressTel"></label>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </div>
                    </div>
                    <div style="display: block; float: left; width: 51%; margin-left: 5px;">
                        <div id="panel2" class="easyui-panel" title="邮寄信息">
                            <div class="sub-container">
                                <table class="row-info" style="width: 100%;" cellspacing="0" cellpadding="0">
                                    <tr>
                                        <td>收件单位：</td>
                                        <td>
                                            <label id="ReceipCompany"></label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>收件人：</td>
                                        <td>
                                            <label id="ReceiterName"></label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>手机号：</td>
                                        <td>
                                            <label id="ReceiterTel"></label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>详细邮寄地址：</td>
                                        <td>
                                            <label id="DetailAddres"></label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>发票运单：</td>
                                        <td>
                                            <label id="WaybillCode"></label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td></td>
                                        <td>
                                            <label></label>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div data-options="region:'center',border: false," style="width: 100%; float: left; margin-top: 5px;">
                <div class="sec-container">
                    <div id="panel3" class="easyui-panel" title="其它信息">
                        <div class="sub-container">
                            <table class="row-info" style="width: 100%;" cellspacing="0" cellpadding="0">
                                <tr>
                                    <td>含税金额：</td>
                                    <td>
                                        <label id="NoticeAmount"></label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>开票差额：</td>
                                    <td>
                                        <label id="NoticeDifference"></label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>备注信息：</td>
                                    <td>
                                        <label id="Summary"></label>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </div>
                </div>
            </div>
            <div data-options="region:'center',border: false," style="width: 100%; float: left; margin-top: 5px;">
                <div data-options="region:'center',border: false,">
                    <div id="productGrid-container" class="sec-container">
                        <form id="form1">
                            <table id="productGrid" class="easyui-datagrid" title="商品信息" data-options="
                                fitColumns:false,
                                fit:false,
                                nowrap:false,
                                pagination:false,
                                onClickRow:onClickRow,
                                onAfterEdit:onAfterEdit,
                                onCancelEdit:onCancelEdit">
                                <thead>
                                    <tr>
                                        <th data-options="field:'UnitName',width: 80,align:'left'">单位</th>
                                        <th data-options="field:'Quantity',width: 100,align:'center'">数量</th>
                                        <th data-options="field:'SalesUnitPrice',width: 100,align:'center'">单价</th>
                                        <th data-options="field:'SalesTotalPrice',width: 100,align:'center'">金额</th>
                                        <th data-options="field:'InvoiceTaxRate',width: 100,align:'center'">税率</th>
                                        <th data-options="field:'UnitPrice',width: 100,align:'center'">含税单价</th>
                                        <th data-options="field:'TaxName',width: 170,align:'left'">税务名称</th>
                                        <th data-options="field:'TaxCode',width: 170,align:'left'">税务编码</th>
                                    </tr>
                                </thead>
                                <thead data-options="frozen:true">
                                    <tr>
                                        <th data-options="field:'Amount',width: 100,align:'center'">含税金额</th>
                                        <th data-options="field:'Difference',width: 80,align:'center'">开票差额</th>
                                        <th data-options="field:'InvoiceNo', width: 160,align:'left',editor:{type:'textbox',options: {validType: 'length[1,50]'}}">发票号码</th>
                                        <th data-options="field:'ProductName',width: 170,align:'left'">产品名称</th>
                                        <th data-options="field:'ProductModel',width: 180,align:'left'">规格型号</th>
                                    </tr>
                                </thead>
                            </table>
                        </form>
                    </div>
                    <%--<div style="margin-top: 5px; margin-left: 2px;">
                        <div class="easyui-panel" title="日志记录" style="width: 100%;">
                            <div class="sub-container">
                                <div class="text-container">
                                    <div id="LogContent" title="日志记录">
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>--%>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
