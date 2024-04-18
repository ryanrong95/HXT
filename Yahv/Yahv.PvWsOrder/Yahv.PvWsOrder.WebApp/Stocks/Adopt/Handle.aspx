<%@ Page Language="C#" MasterPageFile="~/Uc/Works_hidden.Master" AutoEventWireup="true" CodeBehind="Handle.aspx.cs" Inherits="Yahv.PvWsOrder.WebApp.Stocks.Adopt.Handle" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script src="../../Content/Themes/Scripts/PvWsOrder.js"></script>
    <script>
        var ID = getQueryString("ID");
        var EnterCode = getQueryString("EnterCode");
        var selectedTempType = 1;
      
        $(function () { 
            rec_Delivery();
             //提交
            $("#btnSubmit").click(function () {

                var OrderID = $("#OrderNo").textbox("getValue");
                var OrderFee = $("#OrderFee").numberbox("getValue");
                var Summary = $("#Summary").textbox("getValue");

                var OrderEnterCode = OrderID.substring(0, EnterCode.length);
                if (OrderEnterCode.toUpperCase() != EnterCode.toUpperCase()) {
                    top.$.timeouts.alert({ position: "TC", msg: "请输入该客户的订单号", type: "error" });
                    return;
                }

                var data = new FormData();
                //基本信息
                data.append('ID', ID);
                data.append('TempType', selectedTempType);
                data.append('OrderID', OrderID);
                data.append('OrderFee', OrderFee);
                data.append('Summary', Summary);
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
    <script>
        //香港交货方式
        function rec_Delivery(){
            
            $("#rec_Declare").radiobutton({
                onChange: function (record) {
                    if (record) {                       
                        selectedTempType = 1;
                    }
                }
            })
            $("#rec_Immediate").radiobutton({
                onChange: function (record) {
                    if (record) {                       
                        selectedTempType = 2;
                    }
                }
            })
            $("#rec_Stock").radiobutton({
                onChange: function (record) {
                    if (record) {                       
                        selectedTempType = 3;
                    }
                }
            })
        }
       
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
                    <td class="lbl">暂存类型：</td>
                    <td style="width: 300px">
                        <input class="easyui-radiobutton" id="rec_Declare" name="rec_DeliveryType" data-options="labelPosition:'after',checked: true,label:'转报关'">
                        <%--<input class="easyui-radiobutton" id="rec_Immediate" name="rec_DeliveryType" data-options="labelPosition:'after',label:'即收即发'">--%>
                        <input class="easyui-radiobutton" id="rec_Stock" name="rec_DeliveryType" data-options="labelPosition:'after',label:'代仓储'">                     
                    </td>                    
                </tr>
                <tr>
                    <td class="lbl">订单号：</td>
                    <td style="width: 300px">
                        <input id="OrderNo" class="easyui-textbox"  style="width: 250px;" />
                    </td>                    
                </tr>
                <tr>
                    <td class="lbl">暂存费用：</td>
                    <td style="width: 300px">
                        <input id="OrderFee" class="easyui-numberbox"  style="width: 250px;" />
                    </td>                    
                </tr>
                <tr>
                    <td class="lbl">备注：</td>
                    <td style="width: 300px;height:100px">
                        <input id="Summary" class="easyui-textbox" data-options="multiline:true, validType:'length[0,200]'," style="width: 250px;height:80px" />
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