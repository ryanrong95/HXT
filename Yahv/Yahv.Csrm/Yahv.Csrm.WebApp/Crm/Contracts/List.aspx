<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.Csrm.WebApp.Crm.Contracts.List" %>

<%@ Import Namespace="Yahv.Underly" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <link href="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/styles/plugin.css" rel="stylesheet" />
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/ajaxPrexUrl.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/easyui.jl.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/jquery-easyui-extension/jqueryform.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/timeouts.js"></script>
    <script>
        $(function () {
            $('#aaa').click(function () {
                $('input[type="submit"]').click();
            });
            $("#radio_PayExchange").radio({
                name: "PayExchange",//input统一的name值
                data: model.ExchageDate,//数据
                valueField: 'value',//value值
                labelField: 'text',//
                checked: null //选中的值为某一条的valueField
            });
            $("#cboInvoiceTaxRate").combobox({
                data: model.InvoiceRate,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应 
                multiple: false,
                onLoadSuccess: function (data) {
                    $('#cboInvoiceTaxRate').combobox("select", data[0].value);
                }
            });
            $("#InvoiceType").combobox({
                data: model.BillingType,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: false,
                onLoadSuccess: function (data) {
                    var selectvalue = model.Contract == null ? data[0].value : model.Contract.InvoiceType;
                    $(this).combobox('select', selectvalue);

                    if (selectvalue == '<%=(int)BillingType.Full%>') {
                        $('#SPInvoiceAdd').css("display", "block");
                        $('#SPInvoiceRate').css("display", "none");
                    }
                    else {
                        $('#cboInvoiceTaxRate').combobox("select", model.Contract.InvoiceTaxRate * 100 + "%");
                    }
                },
                onSelect: function (record) {
                    if (record.value == '<%=(int)BillingType.Full%>') {
                        $('#SPInvoiceAdd').css("display", "block");
                        $('#SPInvoiceRate').css("display", "none");
                    }
                    else if (record.value == '<%=(int)BillingType.Service%>') {
                        $('#cboInvoiceTaxRate').combobox("select", '<%= InvoiceRate.ThreePercent.GetDescription()%>');
                        $('#SPInvoiceAdd').css("display", "none");
                        $('#SPInvoiceRate').css("display", "block");
                    }
                }
            });
            if (model.NotShowBtnSave || model.Nonstandard) {
                $("#btnSave").hide();
            }

            if (!jQuery.isEmptyObject(model.Contract)) {
                $('#form1').form('load',
                    {
                        AgencyRate: model.Contract.AgencyRate,//代理费率
                        MinAgencyFee: model.Contract.MinAgencyFee,//最低代理费
                        Summary: model.Contract.Summary//备注
                    });
                $("#radio_PayExchange").radio('setCheck', model.Contract.ExchangeMode);
                $('#StartDate').datebox('setValue', model.Contract.StartDate);
                $('#EndDate').datebox('setValue', model.Contract.EndDate);
                //if (!jQuery.isEmptyObject(model.Contract.ServiceAgreement)) {
                //    $('#PdfUpload').fileUpload('setFile', { src: model.Contract.ServiceAgreement.Url, name: model.Contract.ServiceAgreement.CustomName });
                //}
                $('#ExportDiv').css("display", "block");
                //if (!jQuery.isEmptyObject(model.Contract.ServiceAgreement)) {
                //    $("#afile").html("<a href='#' onclick=Pdf('" + model.Contract.ServiceAgreement.Url + "')> " + model.Contract.ServiceAgreement.CustomName + "</a>")
                //}
            }
            //预览
            $("#btnPreview").on('click', function () {
                $.post('?action=PreviewAgreement', { clientid: model.Contract.Enterprise.ID, CompanyID: model.CompanyID }, function (result) {
                    if (result.success) {
                        window.open(result.url);
                    }
                })
            })
            //导出协议
            $('#btnExport').on('click', function () {
                $.post('?action=ExportAgreement', { clientid: model.Contract.Enterprise.ID, CompanyID: model.CompanyID }, function (result) {
                    $.messager.alert('消息', result.message, 'info', function () {
                        if (result.success) {
                            //下载文件
                            try {
                                let a = document.createElement('a');
                                a.href = result.url;
                                a.download = "";
                                a.click();
                            } catch (e) {
                                console.log(e);
                            }
                        }
                    });
                })
            });


            $.extend($.fn.validatebox.defaults.rules, {
                equaldDate: {
                    validator: function (value, param) {
                        var start = $("#StartDate").datetimebox('getValue');  //获取开始时间    
                        return value > start;                             //有效范围为当前时间大于开始时间    
                    },
                    message: '结束日期应大于开始日期!'                     //匹配失败消息  
                }
            });
        })
    function fungeturl() {
        var url = $('#PdfUpload').fileUpload('getFilesUploadAfterPath');
        var file = $('#PdfUpload').fileUpload('files')
        if (file != null) {
            $("#hidurl").val(url)
            $("#hidFormat").val(file[0].type)
            $("#hidName").val(file[0].name)
        }
    }
    function Pdf(url) {
        window.open(url, '', '', false)
    }
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <%-- <div style="margin: 8px; display: none;" id="ExportDiv">
        <a id="btnPreview" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-yg-search'">预览协议</a>
        <a id="btnExport" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-yg-exportFile'">导出协议</a>
    </div>--%>
    <div class="easyui-panel" id="tt" data-options="fit:true" style="padding: 10px 10px 0px 10px;">
        <%-- <div style="width: 900px">--%>
        <table class="liebiao">
            <tr>
                <td style="width: 100px">开始日期</td>
                <td>
                    <input class="easyui-datebox" id="StartDate" name="StartDate" data-options="editable:false,required:true" style="width: 200px" />
                </td>
                <td style="width: 100px">结束日期 </td>
                <td>
                    <input class="easyui-datebox" id="EndDate" name="EndDate" data-options="editable:false,required:true,validType:'equaldDate[\'#StartDate\']'" style="width: 200px" />
                </td>
            </tr>
            <tr>
                <td style="width: 100px">代理费率：</td>
                <td>
                    <input class="easyui-numberbox" id="AgencyRate" name="AgencyRate"
                        data-options="min:0,max:1,precision:4,required:true" style="width: 150px" />
                </td>
                <td style="width: 100px">最低代理费：</td>
                <td>
                    <input class="easyui-numberbox" id="MinAgencyFee" name="MinAgencyFee"
                        data-options="min:0,precision:0,required:true" style="width: 200px" />
                </td>
            </tr>
            <tr>
                <td style="width: 100px">换汇方式</td>
                <td colspan="3">
                    <span id="radio_PayExchange" style="width: 16px"></span>
                </td>
            </tr>
            <tr class="tr_gradeORvip">
                <td style="width: 100px">开票类型</td>
                <td>
                    <input class="easyui-combobox" id="InvoiceType" name="InvoiceType"
                        data-options="valueField:'Key',textField:'Value',limitToList:true,required:true,editable:false" style="width: 200px" />
                </td>
                <td style="width: 100px">对应税点</td>
                <td>
                    <span id="SPInvoiceAdd">
                        <input class="easyui-textbox" id="InvoiceRateAdd" name="fullTaxRate" value="13%" readonly="readonly" style="width: 200px" />
                    </span>
                    <span id="SPInvoiceRate">
                        <input id="cboInvoiceTaxRate" name="InvoiceTaxRate" class="easyui-combobox" style="width: 200px;" data-options="valueField:'Key',textField:'Value',limitToList:true,required:true,editable:false">
                    </span>
                </td>
            </tr>
            <tr>
                <td style="width: 100px">备注</td>
                <td colspan="3">
                    <input class="easyui-textbox" id="Summary" name="Summary" data-options="validType:'length[1,200]',tipPosition:'bottom',multiline:true" style="width: 550px; height: 60px;" />
                </td>
            </tr>
            <%--<tr>
                <td style="width: 100px">服务协议</td>
                <td colspan="3">
                    
                    <div id="afile"> </div>
                    <<input type="hidden" id="hidurl" runat="server">
                    <input type="hidden" id="hidFormat" runat="server">
                    <input type="hidden" id="hidName" runat="server">
                    <a id="PdfUpload" href="#" title="请选择PDF类型的文件" class="easyui-fileUpload" data-options="{accept:'application/pdf',limit:1,required:false}"></a>
                </td>
            </tr>--%>
        </table>
        <%--  <div style="text-align: center; padding: 5px">
            <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClientClick="fungeturl()" OnClick="btnSubmit_Click" />
            <a id="btnSave" class="easyui-linkbutton" particle="Name:'保存',jField:'btnSave'" data-options="iconCls:'icon-yg-save'" onclick="$('#<%=btnSubmit.ClientID%>').click();">保存</a>
        </div>--%>

        <%-- </div>--%>
    </div>

</asp:Content>
