<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewEdit.aspx.cs" Inherits="WebApp.Finance.Swap.ViewEdit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>换汇明细</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <script type="text/javascript">

        var VaultData = eval('(<%=this.Model.VaultData%>)');
        var AllData = eval('(<%=this.Model.AllData%>)');

        $(function () {
            $("#tt").height(document.documentElement.clientHeight + "px");  //修改 layout 高度

            //列表初始化
            $('#datagrid').myDatagrid({
                fitColumns: true,
                fit: false,
                pagination: false,
                scrollbarSize: 0,
                rownumbers: true,
                pagination: false,
                scrollbarSize: 0,
                onLoadSuccess: function (data) {
                    for (var i = 0; i < leftTrs.length; i++) {
                        var useHeight = 0;

                        if ($(leftTrs[i]).height() > $(rightTrs[i]).height()) {
                            useHeight = $(leftTrs[i]).height();
                        } else {
                            useHeight = $(rightTrs[i]).height();
                        }

                        $(leftTrs[i]).height(useHeight);
                        $(rightTrs[i]).height(useHeight);
                    }

                },

            });

            if (AllData != null && AllData != "") {
                $('#TotalAmount').text(AllData.TotalAmount +"  "+ AllData.Currency);
                $('#BankName').text(AllData.BankName);
            }

        });


        //审批通过
        function Approve() {
            var data = new FormData($('#form1')[0]);
            if (AllData != null) {
                data.append('ID', AllData["ID"])
            }
            $.messager.confirm('确认', '请您再次确认是否审批通过！', function (success) {
                if (success) {
                    MaskUtil.mask();
                    $.ajax({
                        url: '?action=Save',
                        type: 'POST',
                        data: data,
                        dataType: 'JSON',
                        cache: false,
                        processData: false,
                        contentType: false,
                        success: function (res) {
                            MaskUtil.unmask();
                            if (res.success) {
                                $.messager.alert('消息', res.message, 'info', function () {
                                    Close();
                                });
                            } else {
                                $.messager.alert('提示', res.message);
                            }
                        }
                    });
                }
            });
        }

        //审批拒绝
        function Refuse() {
            //验证表单数据
            debugger;
            var Summary = $('#Summary').textbox('getValue');
            if (Summary == '' || Summary == null) {
                $.messager.alert('消息', '请填写备注', 'info', function () {

                });
                return;
            }
            var data = new FormData($('#form1')[0]);
            if (AllData != null) {
                data.append('ID', AllData["ID"]),
                    data.append('Summary', $('#Summary').textbox('getValue'))
            }
            $.messager.confirm('确认', '请您再次确认是否审批不通过！', function (success) {
                if (success) {
                    MaskUtil.mask();
                    $.ajax({
                        url: '?action=UnApprove',
                        type: 'POST',
                        data: data,
                        dataType: 'JSON',
                        cache: false,
                        processData: false,
                        contentType: false,
                        success: function (res) {
                            MaskUtil.unmask();
                            if (res.success) {
                                $.messager.alert('消息', res.message, 'info', function () {
                                    Close();
                                });
                            } else {
                                $.messager.alert('提示', res.message);
                            }
                        }
                    });
                }
            });
        }

        //返回
        function Close() {
            var url = location.pathname.replace(/ViewEdit.aspx/ig, 'ApprovedList.aspx');
            window.location = url;
        }

    </script>
</head>
<body class="easyui-layout">
    <div id="tt" class="easyui-tabs" style="width: auto; overflow: auto;" data-options="border: false,">
        <div title="付汇审批" style="display: none; padding: 5px;">
            <div data-options="region:'center',border:false,">
                <form id="form1" runat="server" method="post">
                    <div style="margin: 0 5px">
                        <div>
                            <table id="datagrid" title="换汇明细">
                                <thead>
                                    <tr>
                                        <th data-options="field:'ContrNo',align:'center'" style="width: 100px;">合同协议号</th>
                                        <th data-options="field:'OrderID',align:'center'" style="width: 100px;">订单编号</th>
                                        <th data-options="field:'Currency',align:'center'" style="width: 100px;">币种</th>
                                        <th data-options="field:'SwapAmount',align:'center'" style="width: 100px;">换汇金额</th>
                                        <th data-options="field:'DDate',align:'center'" style="width: 100px;">报关日期</th>
                                    </tr>
                                </thead>
                            </table>
                        </div>
                        <div id="SwapInf">
                            <table style="line-height: 30px">
                                <tr>
                                    <td class="lbl">换汇银行：</td>
                                    <td>
                                        <label id="BankName"></label>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="lbl">换汇总金额：</td>
                                    <td>
                                        <label id="TotalAmount"></label>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="lbl">备注：</td>
                                    <td>
                                        <input class="easyui-textbox" id="Summary" data-options="validType:'length[1,100]',width: 600,multiline:true," style="height: 50px;" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        <div style="margin: 5px 220px 0">
                            <a class="easyui-linkbutton" onclick="Approve()" data-options="iconCls:'icon-ok'">审批通过</a>
                            <a class="easyui-linkbutton" onclick="Refuse()" data-options="iconCls:'icon-cancel'">拒绝</a>
                            <a class="easyui-linkbutton" onclick="Close()" data-options="iconCls:'icon-back'">返回</a>
                        </div>
                    </div>
                </form>
            </div>
        </div>
    </div>
</body>
</html>
