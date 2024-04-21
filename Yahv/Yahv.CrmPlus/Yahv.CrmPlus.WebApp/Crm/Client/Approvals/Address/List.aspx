<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.Client.Approvals.Address.List" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {

            $("#addressType").combobox({
                data: model.AddressType,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: false,
                limitToList: true,
                collapsible: true,
            });

            var getQuery = function () {
                var params = {
                    action: 'data',
                    Name: $.trim($('#txtname').textbox("getText")),
                    AddressType: $("#addressType").combobox("getValue"),
                    StartDate: $('#StartDate').datebox("getValue"),
                    EndDate: $('#EndDate').datebox("getValue"),
                };
                return params;
            };
            window.grid = $("#dg").myDatagrid({
                toolbar: '#tb',
                pagination: true,
                singleSelect: false,
                method: 'get',
                queryParams: getQuery(),
                fit: true,
                rownumbers: true,
                nowrap: false,
            });
            // 搜索按钮
            $('#btnSearch').click(function () {
                $("#dg").myDatagrid('search', getQuery());
                return false;
            });

            $("#btnClear").click(function () {
                location.reload();
                return false;
            });
        });

        //操作
        function btnformatter(value, rowData) {
            var arry = ['<span class="easyui-formatted">'];
            arry.push('<a id="btnApproval" href="#" particle="Name:\'审批\',jField:\'btnEdit\'"  class="easyui-linkbutton" data-options="iconCls:\'icon-yg-approval\'" onclick="Approval(\'' + rowData.ID + '\')">审批通过</a> ');
            arry.push('<a id="btnApprovalNopass" href="#" particle="Name:\'审批\',jField:\'btnEdit\'"  class="easyui-linkbutton" data-options="iconCls:\'icon-yg-approvalNopass\'" onclick="ApprovalNopass(\'' + rowData.ID + '\')">审批不通过</a> ');
            arry.push('</span>');
            return arry.join('');
        }

        function ApprovalNopass(id) {
            $("#refuse-tip").show();
            $('#approve-dialog').dialog({
                title: '提示',
                width: 350,
                height: 200,
                closed: false,
                modal: true,
                closable: true,
                buttons: [{
                    text: '确定',
                    width: 70,
                    handler: function () {
                        $("#reason").textbox({ required: true });
                      var reason = $("#ApproveSummary").textbox('getValue');
                        if (reason == "") {
                            $.messager.alert("消息", "请填写审批原因");
                            return;
                        }
                        reason = reason.trim();
                        $.post(location.pathname + '?action=ApproveRefuse', {
                            ID: id,
                            Summary: reason
                        }, function (res) {
                            var result = JSON.parse(res);
                            if (result.success) {
                                 top.$.timeouts.alert({
                                    position: "TC",
                                    msg: "操作成功！",
                                    type: "success"
                                });
                                  NormalClose();
                            } else {
                                $.messager.alert('提示', result.message, 'info', function () {
                                });
                            }
                        });

                    }
                }, {
                    text: '取消',
                    width: 70,
                    handler: function () {
                        $('#approve-dialog').window('close');
                    }
                }],
            });

            $('#approve-dialog').window('center');
        }

           //整行关闭一系列弹框
    function NormalClose() {
        $('#approve-dialog').window('close');
        $.myWindow.close();
    }
        // 通过
        function Approval(id) {
            $.messager.confirm('确认', '您确定要审批通过吗？', function (r) {
                if (r) {
                    $.post('?action=Approval',
                        {
                            id: id,
                        }, function (data) {
                             var result = JSON.parse(data);
                            if (result.success) {
                                top.$.timeouts.alert({
                                    position: "TC",
                                    msg: "操作成功！",
                                    type: "success"
                                });
                                $.myWindow.close();
                            }
                            else {
                                $.messager.alert('操作提示', '操作失败!', result.message);
                            }
                        });
                }
            })
        }


    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div id="tb">
        <div>
            <table class="liebiao-compact">
                <tr>
                    <td style="width: 90px">客户名称：</td>
                    <td>
                        <input id="txtname" class="easyui-textbox" style="width: 200px;" /></td>
                    <td>地址类型</td>
                    <td>
                        <input id="addressType" class="easyui-combobox" style="width: 200px;" data-options="prompt:'地址类型',validType:'length[1,75]',isKeydown:true"></td>
                    <td>创建日期</td>
                    <td>
                        <input class="easyui-datebox" id="StartDate" data-options="prompt:'创建日期', editable:false" style="width: 200px;" />
                        <input class="easyui-datebox" id="EndDate" data-options=" prompt:'至', editable:false" style="width: 200px;" />
                    </td>
                    <td>
                        <a id="btnSearch" class="easyui-linkbutton" data-options="iconCls:'icon-yg-search'">搜索</a>
                        <a id="btnClear" class="easyui-linkbutton" data-options="iconCls:'icon-yg-clear'">清空</a>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <table id="dg" data-options="rownumbers:true">
        <thead>
            <tr>

                <th data-options="field:'ClientName',width:120">客户名称</th>
                <th data-options="field:'AddressType',width:120">地址类型</th>
                <th data-options="field:'Contact',width:120">联系人</th>
                <th data-options="field:'Phone',width:120">联系电话</th>
                <th data-options="field:'District',width:120">国家地区</th>
                <th data-options="field:'Context',width:120">详细地址</th>
                <th data-options="field:'PostZip',width:50">邮政编码</th>
                <th data-options="field:'creator',width:120">创建人</th>
                <th data-options="field:'CreteDate',width:200">创建时间</th>
                <th data-options="field:'StatusDes',width:80">状态</th>
                <th data-options="field:'Btn',formatter:btnformatter,width:200">操作</th>
            </tr>
        </thead>
    </table>

    <div id="approve-dialog" class="easyui-dialog" data-options="resizable:false, modal:true, closed: true, closable: false,">
        <div id="refuse-tip" style="margin-left: 15px; margin-top: 15px; display: none;">
            <div>
                <label>拒绝原因：</label>
            </div>
            <div style="margin-top: 3px;">
                <input id="ApproveSummary" class="easyui-textbox" data-options="required:true, multiline:true, validType:'length[0,150]'," style="width: 300px; height: 62px;" />
            </div>
        </div>
    </div>
</asp:Content>
