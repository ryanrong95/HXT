<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.Csrm.WebApp.Crm.WsContracts.List" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <link href="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/styles/plugin.css" rel="stylesheet" />
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/ajaxPrexUrl.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/easyui.jl.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/jquery-easyui-extension/jqueryform.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/timeouts.js"></script>
    <script>
        $(function () {
            $("#WsClient").textbox('setValue', model.WsClient.Name);
            $("#Trustee").textbox('setValue', model.Trustee);
            if (model.NotShowBtnSave && model.WsContract != null) {
                $("#btnSave").hide();
            }
            if (!jQuery.isEmptyObject(model.WsContract)) {
                $('#form1').form('load',
                    {
                        ContainerNum: model.WsContract.ContainerNum,
                        Charges: model.WsContract.Charges,
                        Summary: model.WsContract.Summary
                    });
                $('#StartDate').datebox('setValue', model.WsContract.StartDate);
                $('#EndDate').datebox('setValue', model.WsContract.EndDate);
                //if (!jQuery.isEmptyObject(model.WsContract.Agreement)) {
                //    $('#PdfUpload').fileUpload('setFile', { src: model.WsContract.Agreement.Url, name: model.WsContract.Agreement.CustomName });
                //}
                $('#ExportDiv').css("display", "block");
            }
            if (model.Nonstandard) {
                $("#btnSave").hide();
            }
            //预览
            $("#btnPreview").on('click', function () {
                // $.messager.alert('提示', '正在开发中！');
                $.post('?action=PreviewAgreement', { clientid: model.WsClient.ID }, function (result) {
                    if (result.success) {
                        window.open(result.url);
                        // location.href = result.url;
                    }
                })
            })
            //导出协议
            $('#btnExport').on('click', function () {
                //$.messager.alert('提示', '正在开发中！');
                $.post('?action=ExportAgreement', { clientid: model.WsClient.ID }, function (result) {
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
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <%--<div style="margin: 8px; display: none;" id="ExportDiv">
        <a id="btnPreview" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-yg-search'">预览协议</a>
        <a id="btnExport" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-yg-exportFile'">导出协议</a>
    </div>--%>
    <div class="easyui-panel" id="tt" data-options="fit:true" style="padding: 10px 10px 0px 10px;">
        <%--<div style="width: 900px">--%>
        <table class="liebiao">
            <tr>
                <td style="width: 100px">委托方：</td>
                <td colspan="3">
                    <input class="easyui-textbox readonly_style" id="WsClient" name="WsClient"
                        data-options="required:true,readonly:true" style="width: 550px" />
                </td>

            </tr>
            <tr>
                <td style="width: 100px">受委托方：</td>
                <td colspan="3">
                    <input class="easyui-textbox readonly_style" id="Trustee" name="Trustee"
                        data-options="required:true,readonly:true" style="width: 550px" />
                </td>
            </tr>
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
                <td style="width: 100px">货柜数/个</td>
                <td>
                    <input class="easyui-numberbox" id="ContainerNum" name="ContainerNum"
                        data-options="min:0,max:99999,precision:0,required:true" style="width: 200px" />
                </td>
                <td style="width: 100px">仓储费/港元</td>
                <td>
                    <input id="Charges" name="Charges" class="easyui-numberbox" style="width: 200px;" data-options="vmin:0,precision:4,required:true">
                </td>
            </tr>
            <tr>
                <td style="width: 100px">备注</td>
                <td colspan="3">
                    <input class="easyui-textbox" id="Summary" name="Summary" data-options="validType:'length[1,200]',tipPosition:'bottom',multiline:true" style="width: 550px; height: 60px;" />
                </td>
            </tr>
            <tr>
                <td style="width: 100px">服务协议</td>
                <td colspan="3">
                    <%
                        Yahv.Services.Models.CenterFileDescription agrement = this.Model.File as Yahv.Services.Models.CenterFileDescription;
                         %>
                    <a href="<%=agrement?.Url %>"><%=agrement?.CustomName %></a>
                   <%-- <input type="hidden" id="hidurl" runat="server">
                    <input type="hidden" id="hidFormat" runat="server">
                    <input type="hidden" id="hidName" runat="server">
                    <a id="PdfUpload" href="#" title="请选择PDF类型的文件" class="easyui-fileUpload" data-options="{accept:'application/pdf',limit:1,required:false}"></a>--%>
                </td>
            </tr>
        </table>
       <%-- <div style="text-align: center; padding: 5px">
            <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClientClick="fungeturl()" OnClick="btnSubmit_Click" />
            <a id="btnSave" class="easyui-linkbutton" particle="Name:'保存',jField:'btnSave'" data-options="iconCls:'icon-yg-save'" onclick="$('#<%=btnSubmit.ClientID%>').click();">保存</a>
        </div>--%>

        <%-- </div>--%>
    </div>
</asp:Content>
