<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Uc/Works.Master" CodeBehind="PrintInvoice.aspx.cs" Inherits="Yahv.PvWsOrder.WebApp.Invoice.PrintInvoice" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script src="../Content/Themes/Scripts/jquery-barcode.js"></script>
    <script src="../Content/Themes/Scripts/jquery-migrate-1.2.1.min.js"></script>
    <script src="../Content/Themes/Scripts/jquery.jqprint-0.3.js"></script>
    <link href="../Content/Themes/Styles/jquery.jqprint.css" rel="stylesheet" />
    <script src="../Content/Themes/Scripts/Ccs.js"></script>
    <script>
        $(function () {
            var invoiceNoticeIDs = getQueryString('IDs');
            //初始化快递信息
            $('#ExpressName').combobox({
                data: model.ExpressCompanyData,
                onLoadSuccess: function (data) {
                    //默认选择顺丰
                    for (var i = 0; i < data.length; i++) {
                        if (data[i].Text == "顺丰") {
                            $('#ExpressName').combobox('select', data[i].Value);
                        }
                    }
                },
                onSelect: function (record) {
                    $.post('?action=ExpressSelect', { ID: record.Value }, function (data) {
                        //更新快递方式
                        data = eval(data);
                        $('#ExpressType').combobox({
                            data: data,
                        });
                        //默认选择第一行
                        $('#ExpressType').combobox('setValue', data[0].Value);
                    })
                },
            });
            //
            $('#WaybillCode').textbox({
                readonly: true,
            });
            $('#InvoiceNotice').textbox({
                readonly: true,
                multiline: true,
            });
            $('#InvoiceNotice').textbox("setValue", (invoiceNoticeIDs + ''));
        });

        //生成快递面单
        function Print() {
            //验证表单数据
            
            if (!Valid('form1')) {
                return;
            }
            var InvoiceNotice = $('#InvoiceNotice').textbox('getValue');
            var ExpressName = $('#ExpressName').combobox('getValue');
            var ExpressType = $('#ExpressType').combobox('getValue');
            $.ajax({
                type: "GET",
                url: "?action=ExpressSelf",
                data: { InvoiceNotice: InvoiceNotice, ExpressName: ExpressName, ExpressType: ExpressType },
                dataType: "json",
                success: function (data) {
                    if (data.success) {
                        $('#WaybillCode').textbox('setValue', data.LogisticCode);
                        if (data.ShipperCode == "SF") {
                            //添加母模板；
                            //var div = document.getElementById("expresskdd");
                            //var divChild = document.createElement("div");
                            //divChild.innerHTML = data.PrintTemplate;
                            //div.appendChild(divChild);
                            //var br = document.createElement("br");
                            //div.appendChild(br);


                            var $div1 = $("#expresskdd");
                            //var $divf = $("<div class='kddiv'></div>").append(data.PrintTemplate);
                            var $divf = $("<img src='" + data.PrintTemplate + "' alt='SF' height='680' width='350' />");
                            $div1.append($divf);


                            //设置代收货款高度为40px;小心轻放的行高为23；寄方地址高度设置为35
                            // $(".print_paper:eq(6) tr:first ").css({ "height": 29 }); //适应搜狗打印
                            //$(".print_paper:eq(3) tr:first ").find("td").eq(1).css({ "height": 40 });
                            //$(".print_paper:eq(8) .f10").css({ "max-height": 23 });
                            //$(".xx10").find("div").eq(0).css({ "height": 35 });
                        }                       
                    } else {
                        $.messager.alert('消息', "生成快递面单:" + data.message, 'info');
                    }
                },


            });
        }

        //打印快递面单
        function Printkdd() {
            var waybillCode = $('#WaybillCode').textbox('getValue');
            if (waybillCode == "" || waybillCode == null) {
                $.messager.alert('消息', "请先生成发票运单");
                return;
            }
            event.preventDefault();
            $("#expresskdd").jqprint();
        }

        //保存运单数据
        function Save() {
            //验证表单数据
            if (!Valid('form1')) {
                return;
            }
            //验证运单号
            if ($('#WaybillCode').textbox('getValue') == "") {
                $.messager.alert('消息', "运单号为空，保存失败");
                return;
            }
            var data = new FormData($('#form1')[0]);
            $.ajax({
                url: '?action=Save',
                type: 'POST',
                data: data,
                dataType: 'JSON',
                cache: false,
                processData: false,
                contentType: false,
                success: function (res) {
                    if (res.success) {
                        $.messager.alert('消息', res.message, 'info', function () {
                        });
                    } else {
                        $.messager.alert('提示', res.message);
                    }
                }
            });
        }
    </script>
    <style>
        .kddiv {
            position: relative;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="content" style="margin: 0 auto">
        <div style="float: left; padding-left: 10px">
            <form id="form1" method="post">
                <div>
                    <table style="margin: 0 auto; line-height: 30px">
                        <tr>
                            <td class="lbl">开票通知：</td>
                            <td>
                                <input class="easyui-textbox" id="InvoiceNotice" name="InvoiceNotice" data-options="required:true,height:26,width:210" />
                            </td>
                        </tr>
                        <tr>
                            <td class="lbl">快递公司：</td>
                            <td>
                                <input class="easyui-combobox" id="ExpressName" name="ExpressName" data-options="required:true,height:26,width:210,valueField:'Value',textField:'Text'" />
                            </td>
                        </tr>
                        <tr>
                            <td class="lbl">快递方式：</td>
                            <td>
                                <input class="easyui-combobox" id="ExpressType" name="ExpressType" data-options="required:true,height:26,width:210,valueField:'Value',textField:'Text'" />
                            </td>
                        </tr>
                        <tr>
                            <td class="lbl">运单号：</td>
                            <td>
                                <input class="easyui-textbox" id="WaybillCode" name="WaybillCode" data-options="height:26,width:210,validType:'length[1,50]'" />
                            </td>
                        </tr>
                        <tr>
                            <td></td>
                            <td style="text-align: right">
                                <a class="easyui-linkbutton" data-options="height:26,iconCls:'icon-ok'" onclick="Print()">生成发票运单</a>
                                <a class="easyui-linkbutton" data-options="height:26,iconCls:'icon-print'" onclick="Printkdd()">打印发票运单</a>
                            </td>
                        </tr>
                    </table>
                </div>
            </form>
        </div>
        <div style="padding-top: 10px; padding-right: 10px; float: right">
            <div id="expresskdd">
                <style>
                    #expresskdd * {
                        font-family: '微软雅黑' !important;
                    }
                </style>
            </div>
        </div>
    </div>
</asp:Content>
