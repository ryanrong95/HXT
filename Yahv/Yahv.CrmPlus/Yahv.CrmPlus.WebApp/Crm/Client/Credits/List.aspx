<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.Client.Credits.List" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            //信用统计
            window.grid = $("#Credit").myDatagrid({
                actionName: 'GetCredit',
                //toolbar: '#tb',
                pagination: false,
                fit: true,
                nowrap: false,
                singleSelect: false
            });
            //结算方式
            window.grid = $("#Settlement").myDatagrid({
                actionName: 'Settlement',
                //toolbar: '#tb',
                pagination: false,
                fit: true,
                nowrap: false,
                singleSelect: false
            });
            //新增结算方式
            $("#btnCreator").click(function () {
                $.myDialog({
                    title: '新增结算方式',
                    url: '../Settlements/Add.aspx?TakerID=' + model.TakerID,
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
            arry.push('<a id="btnUpdCredit" href="#" particle="Name:\'修改结算方式\',jField:\'btnUpdCredit\'" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-edit\'" onclick="edit(\'' + rowData.ID + '\')">修改</a> ');
            arry.push('<a href="#" particle="Name:\'授信按钮\',jField:\'btnCreditAdd\'" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-add\'" onclick="Credit(true,\''+rowData.MakerID+'\')">授信</a> ');
            arry.push('<a href="#" particle="Name:\'扣减按钮\',jField:\'btnCut\'" class="easyui-linkbutton" data-options="iconCls:\'icon-cut\'" onclick="Credit(false,\'' + rowData.MakerID + '\')">扣减</a> ');
            arry.push('</span>');
            return arry.join('');
        }
        function availformatter(value) {
            return value ? "是" : "否";
        }
        ///结算方式修改
        function edit(id) {
            $.myDialog({
                title: '修改结算方式',
                url: '../Settlements/Edit.aspx?id=' + id,
                width: '60%',
                height: '80%',
                onClose: function () {
                    $("#Settlement").myDatagrid('flush');
                }
            });
        }
        //信用
        function btnformatter1(value, rowData) {
            var arry = ['<span class="easyui-formatted">'];
            arry.push('<a id="btnFlowsCredit" href="#" particle="Name:\'授信记录\',jField:\'btnFlowsCredit\'" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-details\'" onclick="flow(\'' + rowData.Currency + '\',\'' + rowData.MakerID + '\',true)">授信记录</a> ');
            arry.push('<a id="btnFlows" href="#" particle="Name:\'扣减流水\',jField:\'btnFlows\'" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-details\'" onclick="flow(\'' + rowData.Currency + '\',\'' + rowData.MakerID + '\',false)">扣减流水</a> ');
            arry.push('</span>');
            return arry.join('');
        }
        function Credit(isCredit,makerid) {
            var title = isCredit ? "授信" : "信用扣减";
            $.myDialog({
                title: title,
                url: 'Add.aspx?TakerID=' + model.TakerID + '&IsCredit=' + isCredit + '&makerid=' + makerid,
                width: '60%',
                height: '362px',
                onClose: function () {
                    $("#Credit").myDatagrid('flush');
                }
            });
        }
        function flow(currency, makerid, isCredit) {
            var title = isCredit ? "授信记录" : "扣减流水";
            $.myWindow({
                title: title,
                url: 'Flows.aspx?MakerID=' + makerid + '&TakerID=' + model.TakerID + '&Currency=' + currency + '&IsCredit=' + isCredit,
                width: '750px',
                height: '350px'
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
            <table id="Settlement" data-options="rownumbers:true">
                <thead>
                    <tr>
                        <th data-options="field:'TakerName',width:'20%'">客户</th>
                        <th data-options="field:'MakerName',width:'20%'">我方公司</th>
                        <th data-options="field:'ClearType',width:'12%'">结算方式</th>
                        <th data-options="field:'ClearDate',width:'12%'">结算日期</th>
                        <th data-options="field: 'IsAvailable',formatter:availformatter,width:'8%'">是否可用</th>
                        <th data-options="field:'Btn',formatter:btnformatter,width:'20%'">操作</th>
                    </tr>
                </thead>
            </table>

        </div>

        <div data-options="region:'center',title:'信用',split:'true'">
            <table id="Credit" data-options="rownumbers:true" style="width: 100%">
                <thead>
                    <tr>
                        <th data-options="field:'MakerName',width:'22%'">授信公司</th>
                        <th data-options="field:'TakerName',width:'22%'">我方公司</th>
                        <th data-options="field:'CurrencyDes',width:'6%'">币种</th>
                        <th data-options="field:'Total',width:'10%'">总额度</th>
                        <th data-options="field:'Cost',width:'10%'">扣减</th>
                        <th data-options="field:'Surplus',width:'10%'">剩余额度</th>
                        <th data-options="field:'Btn',formatter:btnformatter1,width:'20%'">操作</th>
                    </tr>
                </thead>
            </table>
        </div>
    </div>
</asp:Content>
