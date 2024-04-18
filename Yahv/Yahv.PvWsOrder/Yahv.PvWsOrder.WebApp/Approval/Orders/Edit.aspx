<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.PvOms.WebApp.Approval.Orders.Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        var OrderID = getQueryString("ID");

        $(function () {
            //页面初始化
             $("#tab1").myDatagrid({
                toolbar: '#topper',
                pagination: false,
                singleSelect: false,
                fitColumns: true,
                rownumbers: true,
                scrollbarSize: 0,
            });
            //审批通过
            $("#btnSubmit").click(function () {
                $.messager.confirm('确认', '请您再次确认是否审批通过.', function (success) {
                    if (success) {
                        ajaxLoading();
                        $.post('?action=ApproveOrder', { orderID: OrderID }, function (result) {
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
            })
            //审批驳回
            $("#btnRejected").click(function () {
                $.messager.confirm('确认', '请您再次确认是否驳回.', function (success) {
                    if (success) {
                        ajaxLoading();
                        $.post('?action=UnApproveOrder', { orderID: OrderID }, function (result) {
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
            })
            //取消
            $("#btnClose").click(function () {
                $.myWindow.close();
            })
        });
    </script>
    <script>
          
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-layout" style="width: 100%; height: 100%">
        <div data-options="region:'center'" style="border:none">
            <table id="tab1">
                <thead>
                    <tr>
                        <th data-options="field:'PartNumber',align:'left'" style="width: 150px">型号</th>
                        <th data-options="field:'Manufacturer',align:'left'" style="width: 150px">品牌</th>
                        <th data-options="field:'Origin',align:'center'" style="width: 80px">产地</th>
                        <th data-options="field:'Quantity',align:'center'" style="width: 50px">数量</th>
                        <%--<th data-options="field:'Unit',align:'center'" style="width: 7%">单位</th>--%>
                        <th data-options="field:'UnitPrice',align:'center'" style="width: 50px">单价</th>
                        <th data-options="field:'Currency',align:'center'" style="width: 50px">币种</th>
                        <th data-options="field:'TotalPrice',align:'center'" style="width: 50px">总价值</th>
                        <th data-options="field:'GrossWeight',align:'center'" style="width: 50px">毛重(kg)</th>
                        <%--<th data-options="field:'Volume',align:'center'" style="width: 6%">体积(m³)</th>--%>
                        <th data-options="field:'Condition',align:'center'" style="width: 150px">归类信息</th>
                    </tr>
                </thead>
            </table>
        </div>
        <div data-options="region:'south',height:40" style="background-color: #f5f5f5">
            <div style="float: right; margin-right: 5px; margin-top: 8px;">
                <a id="btnSubmit" class="easyui-linkbutton" iconcls="icon-yg-confirm">通过</a>
                <a id="btnRejected" class="easyui-linkbutton" iconcls="icon-yg-cancel">驳回</a>
                <a id="btnClose" class="easyui-linkbutton" iconcls="icon-yg-cancel">取消</a>
            </div>
        </div>
    </div>
</asp:Content>

