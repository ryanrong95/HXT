<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Srm.SupplierDetails.Credits.List" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            //信用统计
            window.grid = $("#tb_Credit").myDatagrid({
                actionName: 'GetCredit',
                //toolbar: '#tb',
                pagination: false,
                fit: true,
                nowrap: false,
                singleSelect: false
            });
            //结算方式
            window.grid = $("#tb_Settlement").myDatagrid({
                actionName: 'Settlement',
                //toolbar: '#tb',
                pagination: false,
                fit: true,
                nowrap: false,
                singleSelect: false
            });
            //授信
            $("#btnCredit").click(function () {
                Credit(true);
            })
            //扣减
            $("#btnCut").click(function () {
                Credit(false);
            })
            //新增结算方式
            $("#btnCreator").click(function () {
                $.myDialog({
                    title: '新增结算方式',
                    url: '../Settlements/Add.aspx?MakerID=' + model.MakerID,
                    width: '60%',
                    height: '80%',
                    onClose: function () {
                        window.grid.myDatagrid('flush');
                    }
                });
            })
        })

        //结算方式
        function btnformatter(value, rowData) {
            var arry = ['<span class="easyui-formatted">'];
            arry.push('<a href="#" particle="Name:\'修改结算方式\',jField:\'btnUpdCredit\'" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-edit\'" onclick="edit(\'' + rowData.ID + '\')">修改</a> ');
            arry.push('<a href="#" particle="Name:\'授信按钮\',jField:\'btnCreditAdd\'" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-add\'" onclick="Credit(true,\'' + rowData.TakerID + '\')">授信</a> ');
            arry.push('<a href="#" particle="Name:\'扣减按钮\',jField:\'btnCut\'" class="easyui-linkbutton" data-options="iconCls:\'icon-cut\'" onclick="Credit(false,\'' + rowData.TakerID + '\')">扣减</a> ');
            arry.push('</span>');
            return arry.join('');
        }
        function availformatter(value) {
            return value ? "是" : "否";
        }
        ///结算方式修改
        function edit(id) {
            $.myDialogFuse({
                title: '结算方式',
                url: '../Settlements/Edit.aspx?ID=' + id,
                width: '503px',
                height: '245px',
                onClose: function () {
                    $("#tb_Settlement").myDatagrid('flush');
                }
            });
        }
        //信用
        function btnformatter1(value, rowData) {
            var arry = ['<span class="easyui-formatted">'];
            arry.push('<a id="btnFlowsCredit" href="#" particle="Name:\'查看授信流水\',jField:\'btnFlowsCredit\'" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-details\'" onclick="flow(\'' + rowData.Currency + '\',\'' + rowData.TakerID + '\',true)">授信记录</a> ');
            arry.push('<a id="btnFlows" href="#" particle="Name:\'查看信用扣减流水\',jField:\'btnFlows\'" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-details\'" onclick="flow(\'' + rowData.Currency + '\',\'' + rowData.TakerID + '\',false)">扣减流水</a> ');
            arry.push('</span>');
            return arry.join('');
        }

        function Credit(isCredit, takerid) {
            var title = isCredit ? "授信" : "信用扣减";
            $.myDialog({
                title: title,
                url: 'Add.aspx?MakerID=' + model.MakerID + '&IsCredit=' + isCredit + '&takerid=' + takerid,
                width: '60%',
                height: '362px',
                onClose: function () {
                    $("#tb_Credit").myDatagrid('flush');
                }
            });
        }

        function flow(currency, takerid, isCredit) {
            var title = isCredit ? "授信记录" : "扣减流水";
            $.myWindow({
                title: title,
                url: 'Flows.aspx?MakerID=' + model.MakerID + '&TakerID=' + takerid + '&Currency=' + currency + '&IsCredit=' + isCredit,
                width: '60%',
                height: '80%'
            });
        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-layout" style="width: 100%; height: 100%">
        <div data-options="region:'north',title:'结算方式',split:'true'" style="width: 600px; height: 200px;">
            <div id="tb">
                <div>
                    <table class="liebiao-compact">
                        <tr>
                            <td colspan="8"><a id="btnCreator" particle="Name:'新增结算方式',jField:'btnCreator'" class="easyui-linkbutton" data-options="iconCls:'icon-yg-add'">添加</a></td>
                        </tr>
                        <tr></tr>
                    </table>
                </div>
            </div>
            <table id="tb_Settlement" data-options="rownumbers:true">
                <thead>
                    <tr>
                        <th data-options="field:'MakerName',width:'20%'">供应商</th>
                        <th data-options="field:'TakerName',width:'20%'">我方公司</th>
                        <th data-options="field:'ClearType',width:'12%'">结算方式</th>
                        <th data-options="field:'ClearDate',width:'12%'">结算日期</th>
                        <th data-options="field:'IsAvailable',formatter:availformatter,width:'8%'">是否可用</th>
                        <th data-options="field:'Btn',formatter:btnformatter,width:'20%'">操作</th>
                    </tr>
                </thead>
            </table>

        </div>
        <div data-options="region:'center',title:'信用',split:'true'">

            <%--<div id="tb1">

                <table class="liebiao-compact">
                    <tr>
                        <td colspan="8">
                            <a id="btnCreditAdd" particle="Name:'授信',jField:'btnCreditAdd'" class="easyui-linkbutton" data-options="iconCls:'icon-yg-add'" onclick="Credit(true)">授信</a>
                            <a id="btnCut" particle="Name:'扣减',jField:'btnCut'" class="easyui-linkbutton" data-options="iconCls:'icon-cut'" onclick="Credit(false)">信用扣减</a>

                        </td>
                    </tr>
                    <tr></tr>
                </table>

            </div>--%>

            <table id="tb_Credit" data-options="rownumbers:true" style="width: 100%">
                <thead>
                    <tr>
                        <th data-options="field: 'MakerName1',width:'22%'">授信公司</th>
                        <th data-options="field: 'TakerName1',width:'22%'">我方公司</th>
                        <th data-options="field: 'CurrencyDes',width:'6%'">币种</th>
                        <th data-options="field: 'Total',width:'10%'">总额度</th>
                        <th data-options="field: 'Cost',width:'10%'">扣减</th>
                        <th data-options="field: 'Surplus',width:'10%'">剩余额度</th>
                        <th data-options="field: 'Btn',formatter:btnformatter1,width:'20%'">操作</th>
                    </tr>
                </thead>
            </table>
        </div>
    </div>
</asp:Content>

