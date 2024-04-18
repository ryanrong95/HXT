<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PayeeList.aspx.cs" Inherits="WebApp.Finance.CenterCostApply.PayeeList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>费用申请-收款账户列表</title>
    <uc:EasyUI runat="server" />
    <script src="../../../Scripts/Ccs.js"></script>
    <link href="../../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">
        var From = '<%=this.Model.From%>';

        $(function () {
            if (From == 'select') {
                $("#btn-select").show();
            }

            $('#PayeeList').myDatagrid({
                loadFilter: function (data) {
                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];
                        for (var name in row.item) {
                            row[name] = row.item[name];
                        }
                        delete row.item;
                    }
                    return data;
                },
                onLoadSuccess: function (data) {
                    $(".datagrid-header-check").html("");
                },
            });

        });

        function Search()
        {
            var AccountName = $('#AccountName').textbox('getValue');
            var BankAccount = $('#BankAccount').textbox('getValue');
            var parm = {
                AccountName: AccountName,
                BankAccount: BankAccount,
            };
            $('#PayeeList').myDatagrid('search', parm);
        }

        //重置查询条件
        function Reset() {
            $('#AccountName').textbox('setValue', null);
            $('#BankAccount').textbox('setValue', null);
            Search();
        }

        function Operation(val, row, index) {
            var buttons = '';

            buttons += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="EditPayee(\''
                + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">编辑</span>' +
                '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                '</span>' +
                '</a>';

            //buttons += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="DeletePayee(\''
            //    + index + '\')" group >' +
            //    '<span class =\'l-btn-left l-btn-icon-left\'>' +
            //    '<span class="l-btn-text">删除</span>' +
            //    '<span class="l-btn-icon icon-cancel">&nbsp;</span>' +
            //    '</span>' +
            //    '</a>';

            return buttons;
        }

        function AddPayee() {
            var url = location.pathname.replace(/PayeeList.aspx/ig, './EditPayee.aspx') + '?From=Add';

            top.$.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '新增收款方',
                width: 400,
                height: 200,
                onClose: function () {
                    $('#PayeeList').datagrid('reload');
                }
            });
        }

        function EditPayee(CostApplyPayeeID) {
            var url = location.pathname.replace(/PayeeList.aspx/ig, './EditPayee.aspx') + '?From=Edit&ID=' + CostApplyPayeeID;

            top.$.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '编辑收款方',
                width: 400,
                height: 200,
                onClose: function () {
                    $('#PayeeList').datagrid('reload');
                }
            });
        }

        function DeletePayee(index) {
            var rows = $("#PayeeList").datagrid('getRows');
            var row = rows[index];

            var tip = '';
            tip += '<label>确定要删除该收款方信息吗？</label><br>';
            tip += '<label>收款方名称：' + row.AccountName + '</label><br>';
            tip += '<label>收款方账号：' + row.BankAccount + '</label><br>';
            tip += '<label>收款方银行：' + row.BankName + '</label>';

            $("#comfirm-dialog-content").html(tip);
            $('#comfirm-dialog').dialog({
                title: '提示',
                width: 350,
                height: 180,
                closed: false,
                //cache: false,
                modal: true,
                buttons: [{
                    id: 'btn-cancel-comfirm-ok',
                    text: '确定',
                    width: 70,
                    handler: function () {
                        MaskUtil.mask();
                        $.post(location.pathname + '?action=Delete', {
                            CostApplyPayeeID: row.ID,
                        }, function (res) {
                            MaskUtil.unmask();
                            var result = JSON.parse(res);
                            if (result.success) {
                                var alert1 = $.messager.alert('提示', result.message, 'info', function () {
                                    $('#comfirm-dialog').dialog('close');
                                    $('#PayeeList').datagrid('reload');

                                });
                                alert1.window({
                                    modal: true, onBeforeClose: function () {
                                        $('#comfirm-dialog').dialog('close');
                                        $('#PayeeList').datagrid('reload');
                                    }
                                });
                            } else {
                                $.messager.alert('提示', result.message, 'info', function () {

                                });
                            }
                        });
                    }
                }, {
                    id: 'btn-cancel-comfirm-cancel',
                    text: '取消',
                    width: 70,
                    handler: function () {
                        $('#comfirm-dialog').dialog('close');
                    }
                }],
            });

            $('#comfirm-dialog').window('center');
        }

        function Select() {
            var selectPayee = $('#PayeeList').datagrid('getChecked');
            if (selectPayee == null || selectPayee.length <= 0) {
                $.messager.alert('提示', '请选择收款方！');
                return;
            }

            var ewindow = $.myWindow.getMyWindow("ViewEdit2PayeeList");

            ewindow.SelectPayee.IsSelected = true;
            ewindow.SelectPayee.PayeeName = selectPayee[0].AccountName;
            ewindow.SelectPayee.PayeeAccount = selectPayee[0].BankAccount;
            ewindow.SelectPayee.PayeeBank = selectPayee[0].BankName;
            ewindow.SelectPayee.PayeeAccountID = selectPayee[0].ID;

            ewindow.$("#PayeeAccountID").val(selectPayee[0].ID);
            ewindow.$("#PayeeName").textbox('setValue', selectPayee[0].AccountName);
            ewindow.$("#PayeeAccount").textbox('setValue', selectPayee[0].BankAccount);
            ewindow.$("#PayeeBank").textbox('setValue', selectPayee[0].BankName);

            //$.myWindow.close();
            //self.parent.$('iframe').parent().window('close');

            var $iframes = self.parent.$('iframe');
            for (var i = 0; i < $iframes.length; i++) {
                if ($iframes[i].src.indexOf("PayeeList") != -1) {
                    //console.log('123');
                    //console.log($($iframes[i]));
                    //console.log('456');
                    //console.log($($iframes[i]).parent());
                    //console.log('789');
                    //console.log($($iframes[i]).parent().parent());

                    $($iframes[i]).parent().parent().next().next().remove();
                    $($iframes[i]).parent().parent().next().remove();
                    $($iframes[i]).parent().parent().remove();



                    break;
                }
            }

        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <ul>
            <li>
                <span class="lbl">收款方名称:</span>
                <input class="easyui-textbox search" id="AccountName" />
                <span class="lbl">收款方账号:</span>
                <input class="easyui-textbox search" id="BankAccount" />
                <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
            </li>
            <li>
                <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-add'" style="margin-left: 5px;" onclick="AddPayee()">新增</a>
                <a id="btn-select" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-ok'" style="margin-left: 5px; display: none;" onclick="Select()">选择</a>
            </li>
        </ul>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="PayeeList" data-options="fitColumns:true,fit:true,border:false,toolbar:'#topBar',">
            <thead>
                <tr>
                    <% if (this.Model.From == "select") %>
                    <% { %>
                    <th data-options="field:'CheckBox',align:'center',checkbox:true" style="width: 3%;">全选</th>
                    <th data-options="field:'AccountName',align:'left'" style="width: 37%;">收款方名称</th>
                    <% } %>
                    <% else %>
                    <% { %>
                    <th data-options="field:'AccountName',align:'left'" style="width: 40%;">收款方名称</th>
                    <% } %>

                    <th data-options="field:'BankAccount',align:'left'" style="width: 26%;">收款方账号</th>
                    <th data-options="field:'BankName',align:'left'" style="width: 16%;">收款方银行</th>
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 16%;">操作</th>
                </tr>
            </thead>
        </table>
    </div>

    <div id="comfirm-dialog" class="easyui-dialog" data-options="resizable:false, modal:true, closed: true, closable: false,">
        <div id="comfirm-dialog-content" style="margin: 15px 15px 15px 15px;"></div>
    </div>

</body>
</html>
