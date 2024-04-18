<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="ApiTest.aspx.cs" Inherits="Yahv.PvData.WebApp.Test.ApiTest" %>

<%@ Import Namespace="Yahv.Underly" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>

        $(function () {

            /*
            $.ajax({
                url: 'http://hv.erp.b1b.com/PvDataApi/Order/GetProductIdByInfo?partNumber=LTC2644IMS-L8#PBF&manufacturer=LTC',
                dataType: 'jsonp',
                success: function (data) {
                    if (data.Code == "100") {
    
                    } else if (data.Code == "200") {
    
                    } else if (data.Code == "300") {
                        console.log("接口异常");
                    } 
                },
                error: function (data) {
                    
                }
            });
            */

            var elements = model.Elements;
            elements.forEach(e => {
                var key = e.Key;
                var value = e.Value;
            });

            $.ajax({
                url: 'http://hv.erp.b1b.com/PvDataApi/ClassifyInfo',
                type: 'get',
                data: {
                    cpnId: '48DEC037A51CE69537FD4EE26943A0E9'
                },
                dataType: 'json',
                success: function (res) {
                    if (res.code == "200") {
                        var data = res.data;
                        var elements = JSON.parse(data.Elements);
                    } else if (res.code == "300") {
                        console.log("接口异常");
                    }
                },
                error: function (res) {

                }
            });

        })

        function classify() {
            var taxcode = $("#taxCode").textbox('getText');
            taxcode = HtmlUtil.htmlEncode(taxcode);
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <input class="easyui-textbox" id="taxCode" name="taxCode" data-options="required:true,validType:'length[1,50]',missingMessage:'请输入税务编码'" style="width: 100px; height: 22px" />
    <a id="btnConfirm" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-yg-confirm'" onclick="classify()">确认归类</a>
</asp:Content>
