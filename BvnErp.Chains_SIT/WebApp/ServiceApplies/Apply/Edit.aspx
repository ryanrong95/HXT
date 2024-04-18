<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.Ccs.ServiceApplies.Apply.Edit" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <style type="text/css">
        .lbl {
            text-align: center;
        }
    </style>
    <script type="text/javascript">
        $(function () {
            var ID = '<%=this.Model.ID%>';
            var ApplyData = eval('(<%=this.Model.ApplyData != null ? this.Model.ApplyData:""%>)');

            $("#CompanyName").textbox("setValue", ApplyData.CompanyName);
            $("#Address").textbox("setValue", ApplyData.Address);
            $("#Contact").textbox("setValue", ApplyData.Contact);
            $("#Mobile").textbox("setValue", ApplyData.Mobile);
            $("#Tel").textbox("setValue", ApplyData.Tel);
            $("#Email").textbox("setValue", ApplyData.Email);
            $("#Status").textbox("setValue", ApplyData.Status == '<%=Needs.Ccs.Services.Enums.HandleStatus.Pending.GetHashCode()%>' ? '待处理' : '已处理');
            $("#CreateDate").textbox("setValue", ApplyData.CreateDate);

            if (ApplyData.Status == '<%=Needs.Ccs.Services.Enums.HandleStatus.Processed.GetHashCode()%>') {
                $('#btnSave').css('display', 'none');
            }

            //
            $('#btnSave').on('click', function () {
                //提交后台
                $.post('?action=SaveApplyHandle', { ID: ID }, function (res) {
                    var result = JSON.parse(res);
                    $.messager.alert('消息', result.message, 'info', function () {
                        if (result.success) {
                            closeWin();
                        }
                    });
                });
            });

            $('#btnReturn').on('click', function () {
                closeWin();
            });
        });

        function closeWin() {
            $.myWindow.close();
        }
    </script>
</head>
<body>
    <div id="Edit" class="easyui-panel" data-options="border:false,fit:true">
        <form id="form1" runat="server">
            <div style="margin: 15px;">
                <table id="table1" class="irtbwrite">
                    <tr>
                        <th style="width: 20%"></th>
                        <th style="width: 40%"></th>
                    </tr>

                    <tr>
                        <td class="lbl">公司名称:</td>
                        <td>
                            <input class="easyui-textbox" id="CompanyName"
                                data-options="validType:'length[1,150]',tipPosition:'bottom',disabled:true" style="width: 100%" />
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl">地址:</td>
                        <td>
                            <input class="easyui-textbox" id="Address"
                                data-options="validType:'length[1,150]',tipPosition:'bottom',disabled:true" style="width: 100%" />
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl">联系人:</td>
                        <td>
                            <input class="easyui-textbox" id="Contact"
                                data-options="validType:'length[1,150]',tipPosition:'bottom',disabled:true" style="width: 100%" />
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl">手机:</td>
                        <td>
                            <input class="easyui-textbox" id="Mobile"
                                data-options="validType:'length[1,150]',tipPosition:'bottom',disabled:true" style="width: 100%" />
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl">电话:</td>
                        <td>
                            <input class="easyui-textbox" id="Tel"
                                data-options="validType:'length[1,150]',tipPosition:'bottom',disabled:true" style="width: 100%" />
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl">邮箱:</td>
                        <td>
                            <input class="easyui-textbox" id="Email"
                                data-options="validType:'length[1,150]',tipPosition:'bottom',disabled:true" style="width: 100%" />
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl">状态:</td>
                        <td>
                            <input class="easyui-textbox" id="Status"
                                data-options="validType:'length[1,150]',tipPosition:'bottom',disabled:true" style="width: 100%" />
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl">申请日期:</td>
                        <td>
                            <input class="easyui-textbox" id="CreateDate"
                                data-options="validType:'length[1,150]',tipPosition:'bottom',disabled:true" style="width: 100%" />
                        </td>
                    </tr>
                </table>
                <div id="divSave" style="text-align: center; padding-top: 15px;">
                    <a id="btnSave" href="javascript:void(0);" class="easyui-linkbutton ir-save" data-options="iconCls:'icon-save'">已处理</a>
                    <a id="btnReturn" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-undo'">返回</a>
                </div>
            </div>
        </form>
    </div>
</body>
</html>
