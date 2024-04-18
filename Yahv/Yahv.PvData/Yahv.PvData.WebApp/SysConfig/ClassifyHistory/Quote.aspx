<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Quote.aspx.cs" Inherits="Yahv.PvData.WebApp.SysConfig.ClassifyHistory.Quote" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            $('#partNumber').text(model.PartNumber);

            window.grid = $('#dg').myDatagrid({
                pagination: false,
                nowrap: false,
            });
        });
    </script>

    <style>
        
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div data-options="region:'center',border:false" style="text-align: center; margin: 2px">
        <table class="liebiao">
            <tr>
                <td class="liebiao-label" style="width: 100px">型号：</td>
                <td>
                    <label id="partNumber" />
                </td>
            </tr>
        </table>
    </div>

    <table id="dg">
        <thead>
            <tr>
                <th data-options="field:'Manufacturer',align:'left',width:100">品牌</th>
                <th data-options="field:'UnitPrice',align:'center',width:80">单价</th>
                <th data-options="field:'Currency',align:'center',width:80">币种</th>
                <th data-options="field:'Quantity',align:'center',width:80">数量</th>
                <th data-options="field:'CreateDate',align:'center',width:160">创建时间</th>
            </tr>
        </thead>
    </table>
</asp:Content>
