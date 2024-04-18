<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="UnReceivedStorageCharges.aspx.cs" Inherits="Yahv.PvOms.WebApp.Orders.Common.UnReceivedStorageCharges" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        var id = getQueryString("ID");
        $(function () {
            //页面初始化
            window.grid = $("#tab1").myDatagrid({
                toolbar: '#topper',
                singleSelect: true,
                fitColumns: true,
                pagination: false,
                fit: true,
                nowrap: false,
                onLoadSuccess: function (data) {
                    if (data.rows.length > 0) {
                        $('#payee').textbox('setValue', data.rows[0].PayeeName);
                        $('#payer').textbox('setValue', data.rows[0].PayerName);
                    }
                }
            });
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-layout" style="width: 100%; height: 100%;">
        <div id="topper">
            <table class="liebiao">
                <tr>
                    <td style="width:100px">收款公司</td>
                    <td>
                        <input id="payee" class="easyui-textbox" style="width: 250px;" />
                    </td>
                </tr>
                <tr>
                    <td style="width:100px">付款公司</td>
                    <td>
                        <input id="payer" class="easyui-textbox" style="width: 250px;" />
                    </td>
                </tr>
            </table>
        </div>
        <table id="tab1" title="">
            <thead>
                <tr>
                    <th data-options="field:'LeftDate',align:'center'" style="width: 50px;">创建日期</th>
                    <th data-options="field:'Catalog',align:'center'" style="width: 50px;">分类</th>
                    <th data-options="field:'Subject',align:'center'" style="width: 50px;">科目</th>
                    <th data-options="field:'LeftPrice',align:'center'" style="width: 50px;">应收</th>
                    <th data-options="field:'RightPrice',align:'center'" style="width: 50px;">实收</th>
                     <th data-options="field:'ReducePrice',align:'center'" style="width: 50px;">减免</th>
                    <th data-options="field:'Currency',align:'center'" style="width: 50px;">币种</th>
                    <th data-options="field:'AdminName',align:'center'" style="width: 50px;">创建人</th>
                </tr>
            </thead>
        </table>
    </div>
</asp:Content>
