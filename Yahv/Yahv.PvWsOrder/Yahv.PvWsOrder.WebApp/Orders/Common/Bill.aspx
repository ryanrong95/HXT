<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Bill.aspx.cs" Inherits="Yahv.PvOms.WebApp.Orders.Common.Bill" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        var id = getQueryString("ID");
        $(function () {
            //应收账单
            $("#tab1").myDatagrid({
                singleSelect: true,
                fitColumns: true,
                fit: true,
                pagination: false,
            });

            //应付账单
            $("#tab2").myDatagrid({
                singleSelect: true,
                fitColumns: true,
                fit: true,
                pagination: false,
                actionName: 'PaymentData',
            });
        });
    </script>

    <script>
        function Operation(val, row, index) {
            if (row.RightPrice != 0) {
                return ['<span class="easyui-formatted">',
                    , '<a class="easyui-linkbutton"  data-options="iconCls:\'icon-yg-details\'" onclick="View(\'' + row.ReceivableID + '\');return false;">实收明细</a> '
                    , '</span>'].join('');
            }
            else {
                return ['<span class="easyui-formatted">', '</span>'].join('');
            }

        }
        function PayOperation(val, row, index) {
            if (row.RightPrice != 0) {
                return ['<span class="easyui-formatted">',
                    , '<a class="easyui-linkbutton"  data-options="iconCls:\'icon-yg-details\'" onclick="ViewPay(\'' + row.PayableID + '\');return false;">实付明细</a> '
                    , '</span>'].join('');
            }
            else {
                return ['<span class="easyui-formatted">', '</span>'].join('');
            }
        }
        function View(id) {
            $.myWindow({
                title: "实收明细",
                url: location.pathname.replace('Bill.aspx', 'Received.aspx?id=' + id),
                onClose: function () {
                    window.grid.myDatagrid('flush');
                },
            });
            return false;
        }
        function ViewPay(id) {
            $.myWindow({
                title: "实付明细",
                url: location.pathname.replace('Bill.aspx', 'Payment.aspx?id=' + id),
                onClose: function () {
                    window.grid.myDatagrid('flush');
                },
            });
            return false;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-layout" style="width: 100%; height: 100%; border: none">
        <div data-options="region:'center'" style="border: none">
            <table id="tab1" title="应收账款列表">
                <thead>
                    <tr>
                        <th data-options="field:'PayeeName',align:'left'" style="width: 200px;">收款公司</th>
                        <th data-options="field:'PayerName',align:'left'" style="width: 200px">付款公司</th>
                        <th data-options="field:'Catalog',align:'center'" style="width: 60px;">分类</th>
                        <th data-options="field:'Subject',align:'center'" style="width: 60px;">科目</th>
                        <th data-options="field:'OriginCurrency',align:'center'" style="width: 60px;">发生币种</th>
                        <th data-options="field:'Currency',align:'center'" style="width: 60px;">本位币种</th>
                        <th data-options="field:'LeftPrice',align:'center'" style="width: 80px;">应收金额</th>
                        <th data-options="field:'RightPrice',align:'center'" style="width: 80px;">实收金额</th>
                        <th data-options="field:'Remains',align:'center'" style="width: 80px;">剩余差额</th>
                        <th data-options="field:'OriginDate',align:'center'" style="width: 100px;">创建日期</th>
                        <th data-options="field:'AdminName',align:'center'" style="width: 80px;">创建人</th>
                        <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 100px;">操作</th>
                    </tr>
                </thead>
            </table>
        </div>
        <div data-options="region:'south',height:250" style="border: none">
            <table id="tab2" title="应付账款列表">
                <thead>
                    <tr>
                        <th data-options="field:'PayerName',align:'left'" style="width: 200px">付款公司</th>
                        <th data-options="field:'PayeeName',align:'left'" style="width: 200px;">收款公司</th>
                        <th data-options="field:'Catalog',align:'center'" style="width: 60px;">分类</th>
                        <th data-options="field:'Subject',align:'center'" style="width: 60px;">科目</th>
                        <th data-options="field:'Currency',align:'center'" style="width: 60px;">发生币种</th>
                        <th data-options="field:'LeftPrice',align:'center'" style="width: 80px;">应付金额</th>
                        <th data-options="field:'RightPrice',align:'center'" style="width: 80px;">实付金额</th>
                        <th data-options="field:'Remains',align:'center'" style="width: 80px;">剩余差额</th>
                        <th data-options="field:'OriginDate',align:'center'" style="width: 100px;">创建日期</th>
                        <th data-options="field:'AdminName',align:'center'" style="width: 80px;">创建人</th>
                        <th data-options="field:'Btn',align:'center',formatter:PayOperation" style="width: 100px;">操作</th>
                    </tr>
                </thead>
            </table>
        </div>
    </div>
</asp:Content>
