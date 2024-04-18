<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.PvOms.WebApp.Stocks.Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>

        var id = getQueryString("ID");
        var oldtotal = getQueryString("Total");
        var oldquantity = getQueryString("Quantity");
        $(function () {
            //提交
            $("#btnSubmit").click(function () {
                var newtotal = $('#total').numberbox('getText');
                var newquantity = $('#quantity').numberbox('getText');
                if (valudateQty(newtotal, newquantity)) {
                    ajaxLoading();
                    $.post('?action=Submit', { ID: id, total: newtotal, quantity: newquantity }, function (result) {
                        ajaxLoadEnd();
                        var res = JSON.parse(result);
                        if (res.success) {
                            top.$.timeouts.alert({ position: "TC", msg: res.message, type: "success" });
                        }
                        else {
                            top.$.timeouts.alert({ position: "TC", msg: res.message, type: "error" });
                        }
                        $.myWindow.close();
                    })
                }
            });

            //取消
            $("#btnClose").click(function () {
                $.myWindow.close();
            })
        });
    </script>
    <script>
        //验证合理 返回true
        function valudateQty(newtotal, newquantity) {
            if (Number(oldtotal) == Number(oldquantity)) {
                if (Number(newtotal) == Number(newquantity)) {
                    return true;
                }
                else {
                    $.messager.alert('提示', '当可用库存和总库存相同时，修改的可用库存和总库存也必须相同。');
                    return false;
                }
            }
            else {
                if (Number(newtotal) >= Number(newquantity)) {
                    return true;
                }
                else {
                    $.messager.alert('提示', '可用库存不能大于总库存!');
                    return false;
                }
            }
        }

    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-layout" style="width: 100%; height: 100%;">
        <div data-options="region:'center'" style="border: none">
            <table id="tab1" class="liebiao">
                <tr>
                    <td class="lbl">总库存：</td>
                    <td>
                        <input id="total" name="total" class="easyui-numberbox" data-options="required:true,min:1,precision:0" style="width: 250px;" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">可用库存：</td>
                    <td>
                        <input id="quantity" name="quantity" class="easyui-numberbox" data-options="required:true,min:1,precision:0" style="width: 250px;" /></td>
                </tr>
            </table>
        </div>
        <div data-options="region:'south',height:40" style="background-color: #f5f5f5">
            <div style="float: right; margin-right: 5px; margin-top: 8px;">
                <a id="btnSubmit" class="easyui-linkbutton" iconcls="icon-yg-confirm">保存</a>
                <a id="btnClose" class="easyui-linkbutton" iconcls="icon-yg-cancel">关闭</a>
            </div>
        </div>
    </div>
</asp:Content>

