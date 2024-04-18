<%@ Page Language="C#" MasterPageFile="~/Uc/Works_hidden.Master" AutoEventWireup="true" CodeBehind="Adopt.aspx.cs" Inherits="Yahv.PvWsOrder.WebApp.Stocks.Adopt.Adopt" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script src="../../Content/Themes/Scripts/PvWsOrder.js"></script>
    <script>
        var ID = getQueryString("ID");        
        $(function () {                       
            $("#EnterCode").combobox({
                required: true,
                valueField: 'Value',
                textField: 'Text',
                data: model.Client,
            });

             //提交
            $("#btnSubmit").click(function () {
                var ClientCode = $("#EnterCode").combobox("getValue");
                if (ClientCode == "") {
                    top.$.timeouts.alert({ position: "TC", msg: "请选择客户", type: "error" });
                    return;
                }

                var data = new FormData();
                //基本信息
                data.append('ID', ID);
                data.append('EnetrCode', ClientCode);
                ajaxLoading();
                $.ajax({
                    url: '?action=Submit',
                    type: 'POST',
                    data: data,
                    dataType: 'JSON',
                    cache: false,
                    processData: false,
                    contentType: false,
                    success: function (res) {
                        ajaxLoadEnd();
                        var res = eval(res);
                        if (res.success) {
                            top.$.timeouts.alert({ position: "TC", msg: res.message, type: "success" });
                            $.myWindow.close();
                        }
                        else {
                            top.$.timeouts.alert({ position: "TC", msg: res.message, type: "error" });
                        }
                    }
                })
            });
        });
    </script>
   
    <style>
        .title {
            background-color: #F5F5F5;
            color: #575765;
            font-weight: 600;
        }

        .lbl {
            width: 100px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server" >
    <div class="easyui-layout" style="width: 100%; height: 100%">
        <div data-options="region:'center'" style="border: none">
            <table class="liebiao">               
                <tr>
                    <td class="lbl">入仓号：</td>
                    <td style="width: 300px">
                        <input id="EnterCode" class="easyui-combobox"  style="width: 250px;" />
                    </td>                    
                </tr>
            </table>
        </div>
        
        <div data-options="region:'south',height:40" style="background-color: #f5f5f5">
            <div style="float: right; margin-right: 5px; margin-top: 8px">
                <a id="btnSubmit" class="easyui-linkbutton" iconcls="icon-yg-confirm">提交</a>               
            </div>
        </div>
    </div>
    
</asp:Content>