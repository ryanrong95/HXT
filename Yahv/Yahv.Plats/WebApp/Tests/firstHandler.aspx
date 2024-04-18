<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Underly.Master" AutoEventWireup="true" CodeBehind="firstHandler.aspx.cs" Inherits="WebApp.Tests.firstHandler" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">

    <h2>Accordion Tools</h2>
    <p>Click the tools on top right of panel to perform actions.</p>
    <div style="margin: 20px 0 10px 0;"></div>
    <div class="easyui-accordion" style="width: 500px; height: 300px;">
        <div title="About" data-options="iconCls:'icon-ok'" style="overflow: auto; padding: 2px;">
            <h3 style="color: #0099FF;">Accordion for jQuery</h3>
            <p>Accordion is a part of easyui framework for jQuery. It lets you define your accordion component on web page more easily.</p>
        </div>
        <div title="Help" data-options="iconCls:'icon-help'" style="padding: 2px;">
            <p>The accordion allows you to provide multiple panels and display one ore more at a time. Each panel has built-in support for expanding and collapsing. Clicking on a panel header to expand or collapse that panel body. The panel content can be loaded via ajax by specifying a 'href' property. Users can define a panel to be selected. If it is not specified, then the first panel is taken by default.</p>
        </div>
        <div title="DataGrid" style="padding: 2px" data-options="
				selected:true,
				tools:[{
					iconCls:'icon-reload',
					handler:function(){
						$('#dg').datagrid('reload');
					}
				}]">
            <table id="dg" class="easyui-datagrid"
                data-options="url:'datagrid_data1.json',method:'get',fit:true,fitColumns:true,singleSelect:true">
                <thead>
                    <tr>
                        <th data-options="field:'itemid',width:80">Item ID</th>
                        <th data-options="field:'productid',width:100">Product ID</th>
                        <th data-options="field:'listprice',width:80,align:'right'">List Price</th>
                        <th data-options="field:'unitcost',width:80,align:'right'">Unit Cost</th>
                        <th data-options="field:'attr1',width:150">Attribute</th>
                        <th data-options="field:'status',width:50,align:'center'">Status</th>
                    </tr>
                </thead>
            </table>
        </div>
    </div>

</asp:Content>
