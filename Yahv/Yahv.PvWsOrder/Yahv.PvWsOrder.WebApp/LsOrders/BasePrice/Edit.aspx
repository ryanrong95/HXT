<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.PvOms.WebApp.LsOrders.BasePrice.Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        var ID = getQueryString("ID");
        $(function () {
            $("#product").combobox({
                required: true,
                editable: false,
                valueField: 'Value',
                textField: 'Text',
                data: model.productData,
            })
            //提交
            $("#btnSubmit").click(function () {
                var product = $('#product').combobox('getValue');
                var month =$('#Month').textbox('getText');
                var price = $('#Price').textbox('getText');
                var summary = $('#Summary').textbox('getText');
                ajaxLoading();
                $.post('?action=Submit', {ID:ID, product: product, month: month, price: price ,summary:summary}, function (result) {
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
            });
            //取消
            $("#btnClose").click(function () {
                $.myWindow.close();
            })
            // 编辑
            Init();
        });
    </script>
    <script>
     function Init(){
            var id=getQueryString("ID");
            var specID=getQueryString("SpecID");
            var month=getQueryString("Month");
            if(id){
                 $("#product").combobox({ disabled: true, });
                 $("#Month").numberbox({ disabled: true, });
                 $("#product").combobox('setValue',specID);
                 $("#Month").numberbox('setText',month);
                 $('#Summary').textbox('setText',model.Summary);
            }
     }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-layout" style="width: 100%; height: 100%;">
        <div data-options="region:'center'" style ="border:none">
            <table id="tab1" class="liebiao">
                <tr>
                    <td class="lbl">库位类型：</td>
                    <td>
                        <input id="product" name="product" class="easyui-combobox" data-options="required:true" style="width: 250px;" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">租赁时长：</td>
                    <td>
                        <input id="Month" name="Month" class="easyui-numberbox" data-options="required:true,min:1,precision:0" style="width: 250px;" /></td>
                </tr>
                <tr>
                    <td class="lbl">租赁单价：</td>
                    <td>
                        <input id="Price" name="Price" class="easyui-numberbox" data-options="required:true,min:1,precision:0" style="width: 250px;" /></td>
                </tr>
                <tr>
                    <td class="lbl">备注：</td>
                    <td>
                        <input id="Summary" name="Summary" class="easyui-textbox" data-options="multiline:true" style="width: 250px;" /></td>
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

