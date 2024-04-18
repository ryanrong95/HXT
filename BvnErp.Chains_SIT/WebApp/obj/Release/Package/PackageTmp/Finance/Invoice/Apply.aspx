<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Apply.aspx.cs" Inherits="WebApp.Finance.Invoice.Apply" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <style type="text/css">
        table.row-info tr td:first-child {
            width: 100px;
        }

        table.row-info tr td:first-child {
            width: 100px;
        }

        #productGrid-container .datagrid-header-rownumber, #productGrid-container .datagrid-cell-rownumber {
            width: 50px;
        }
    </style>
    <script type="text/javascript">
        var InvoiceData = eval('(<%=this.Model.InvoiceData%>)');
        var MaileDate = eval('(<%=this.Model.MaileDate%>)');
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
                actionName: 'ProductData',
                onLoadSuccess: function (data) {
                    $('#productGrid').datagrid('appendRow', {
                        Amount: data.totaldata.Amount,
                        TotalPrice: data.totaldata.TotalPrice,
                        Difference: '',
                    });

                    //"合计"标题
                    setHejiTitle();

                    $('#btnApply').show();
                },
            });
            Init();
        });
        //初始化发票信息
        function Init() {
            if (InvoiceData != null) {
                $("#InvoiceType").text(InvoiceData.InvoiceType);
                $("#DeliveryType").text(InvoiceData.DeliveryType);
                $("#CompanyName").text(InvoiceData.CompanyName);
                $("#TaxCode").text(InvoiceData.TaxCode);
                $("#BankInfo").text(InvoiceData.BankInfo);
                $("#AddressTel").text(InvoiceData.AddressTel);
            };
            if (MaileDate != null) {
                $("#ReceipCompany").text(MaileDate.ReceipCompany);
                $("#ReceiterName").text(MaileDate.ReceiterName);
                $("#ReceiterTel").text(MaileDate.ReceiterTel);
                $("#DetailAddres").text(MaileDate.DetailAddres);
            }
        }
        //提交申请
        function SubmitApply() {
            //结束表单编辑状态
            endEditing();
            VerifyQuantityOrigin();

            if (!$("#form1").form('validate')) {
                return;
            }
            else {
                //验证成功
                var strIds = getQueryString("IDs");
                var Summary = $("#Summary").textbox('getValue');
                var AmountLimit = $("#AmountLimit").numberbox('getValue');
                var ProductData = $("#productGrid").datagrid("getRows");
                var realProducts = [];
                for (var i = 0; i < ProductData.length - 1; i++) {
                    realProducts.push(ProductData[i]);
                }

                MaskUtil.mask();//遮挡层
                $.post('?action=SubmitApply', {
                    IDs: JSON.stringify(strIds),
                    Summary: Summary,
                    AmountLimit : AmountLimit,
                    ProductData: JSON.stringify(realProducts),
                }, function (result) {
                    MaskUtil.unmask();//关闭遮挡层
                    var rel = JSON.parse(result);
                    $.messager.alert('消息', rel.message, 'info', function () {
                        if (rel.success) {
                            Back();
                        }
                    });
                })
            }
        }
        //返回
        function Back() {
            var url = location.pathname.replace(/Apply.aspx/ig, '../../Order/UnInvoiced/List.aspx')
            window.location = url;
        }
    </script>
    <script>
        var editIndex = undefined;
        function endEditing() {
            if (editIndex == undefined) { return true }
            if ($('#productGrid').datagrid('validateRow', editIndex)) {
                $('#productGrid').datagrid('endEdit', editIndex);
                editIndex = undefined;
                return true;
            } else {
                return false;
            }
        }
        function onClickRow(index) {
            if (editIndex != index) {
                if (endEditing()) {
                    //VerifyQuantity();
                    $('#productGrid').datagrid('selectRow', index);
                    $('#productGrid').datagrid('beginEdit', index);
                    editIndex = index;
                } else {
                    $('#productGrid').datagrid('selectRow', editIndex);
                }
            }
            //else {
            //    $('#productGrid').datagrid('acceptChanges');
            //    editIndex = undefined;
            //}
            else {
                VerifyQuantity(index);
                editIndex = undefined;
            }
            //"合计"标题
            setHejiTitle();
        }
        function VerifyQuantity(Index) {
            if (Index != null) {
                $('#productGrid').datagrid('acceptChanges');
                $('#productGrid').datagrid('selectRow', Index);
                var row = $('#productGrid').datagrid('getSelected');
                if (Number(row["Difference"]) == "") {
                    row["Difference"] = 0;
                    var index = $('#productGrid').datagrid('getRowIndex', row);
                    $('#productGrid').datagrid('refreshRow', index);
                } else {
                    if (Number(row["Difference"]) <= -20 || Number(row["Difference"]) >= 20) {
                        $.messager.alert("消息", "开票差额限制在正负20以内！");
                        row["Difference"] = 0;
                        var index = $('#productGrid').datagrid('getRowIndex', row);
                        $('#productGrid').datagrid('refreshRow', index);
                    }
                }
                $('#productGrid').datagrid('acceptChanges');
            }
        }


        //
        function VerifyQuantityOrigin() {
            var rows = $('#productGrid').datagrid('getRows');
            for (var i = 0; i < rows.length - 1; i++) {
                var row = rows[i];
                if (row["Difference"] == "") {
                    row["Difference"] = 0;
                }
            }
            $('#productGrid').datagrid('acceptChanges');
        }

        //"合计"标题
        function setHejiTitle() {
            var rownumberCells = $("#productGrid-container .datagrid-cell-rownumber");
            if (rownumberCells.length > 0) {
                $(rownumberCells[rownumberCells.length - 1]).html("合计");

                $("#productGrid-container tr[datagrid-row-index=" + (rownumberCells.length - 1) + "] td[field=Difference]").html("");
            }
        }

        function onAfterEdit() {
            setHejiTitle();
        }

        function onCancelEdit() {
            setHejiTitle();
        }
    </script>
</head>
<body class="easyui-layout" style="overflow-y: scroll;">
    <div id="tt" class="easyui-tabs" style="width: auto;">
        <div title="开票申请" style="display: none; padding: 5px;">
            <div data-options="region:'north',border: false," style="overflow-y: hidden;">
                <div class="sub-container" style="height: 20px;">
                    <a id="btnApply" style="display: none;" class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="SubmitApply()">提交申请</a>
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
                                        <td></td>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td></td>
                                        <td></td>
                                    </tr>
                                </table>
                            </div>
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
                                onCancelEdit:onCancelEdit,">
                                <thead>
                                    <tr>
                                        <%--<th data-options="field:'Unit',width: 50,align:'center'">单位</th>--%>
                                        <th data-options="field:'Quantity',width: 100,align:'center'">数量</th>
                                        <th data-options="field:'Price',width: 100,align:'center'">单价</th>
                                        <th data-options="field:'TotalPrice',width: 100,align:'center'">金额</th>
                                        <%-- <th data-options="field:'Currency',width: 100,align:'center'">币种</th>--%>
                                        <th data-options="field:'InvoiceTaxRate',width: 50,align:'center'">税率</th>
                                        <th data-options="field:'UnitPrice',width: 100,align:'center'">含税单价</th>
                                        <th data-options="field:'TaxName',width: 200,align:'left'">税务名称</th>
                                        <th data-options="field:'TaxCode',width: 200,align:'left'">税务编码</th>
                                    </tr>
                                </thead>
                                <thead data-options="frozen:true">
                                    <tr>
                                        <th data-options="field:'Amount',width: 150,align:'center'">含税金额</th>
                                        <th data-options="field:'Difference',width: 100,align:'center',editor:{type:'numberbox',options:{precision:4}}">开票差额</th>
                                        <th data-options="field:'ProductName',width: 200,align:'left'">产品名称</th>
                                        <th data-options="field:'ProductModel',width: 200,align:'left'">规格型号</th>
                                    </tr>
                                </thead>
                            </table>
                            <div class="text-container" style="margin-top: 10px;">
                                <p style="font-size: 14px;">注：跟单员需填写好开票差额。</p>
                            </div>
                            <table style="line-height: 50px; margin-top: 20px">
                                <tr>
                                    <td style="font-size: 14px; color: red">备注：</td>
                                    <td>
                                        <input class="easyui-textbox" id="Summary" data-options="multiline:true,validType:'length[1,250]',height:50,width:400" />
                                    </td>
                                    <td style="font-size: 14px; color: red">开票限额：</td>
                                    <td>
                                        <input class="easyui-numberbox" id="AmountLimit"  data-options="height:28,width:250,validType:'length[1,150]',precision:5" />
                                    </td>
                                </tr>
                            </table>
                        </form>
                    </div>
                </div>
            </div>
        </div>
    </div>
</body>
</html>
