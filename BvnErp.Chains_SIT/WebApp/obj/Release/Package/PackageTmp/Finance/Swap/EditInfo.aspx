<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditInfo.aspx.cs" Inherits="WebApp.Finance.Swap.EditInfo" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>编辑换汇信息</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <script type="text/javascript">
        var AllData = eval('(<%=this.Model.AllData%>)');

        var changeBankOK = false;


        var closeFromAddDecHeadInEditInfo = false;
        var decHeadIDFromAddDecHeadInEditInfo = '';


        $(function () {
            //列表初始化
            $('#datagrid').myDatagrid({
                fitColumns:true,
                fit:false,
                pagination:false,
                scrollbarSize:0,
                rownumbers:true,
            });

            if (AllData != null && AllData != "") {
                $('#TotalAmount').html(AllData.TotalAmount);
                $('#BankName').html(AllData.BankName);
            }

        });

        //操作
        function Operation(val, row, index) {
            var buttons = '';

            //buttons += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="EditInfo(\'' + row.ID + '\')" group >' +
            //    '<span class =\'l-btn-left l-btn-icon-left\'>' +
            //    '<span class="l-btn-text">编辑</span>' +
            //    '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
            //    '</span>' +
            //    '</a>';

            buttons += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px; margin-left: 5px;" onclick="Delete(\'' + row.DecHeadID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">删除</span>' +
                '<span class="l-btn-icon icon-remove">&nbsp;</span>' +
                '</span>' +
                '</a>';

            return buttons;
        }

        //修改银行
        function EditBank() {
            var url = location.pathname.replace(/EditInfo.aspx/ig, 'EditBank.aspx')
                + '?CurrentBankName=' + AllData.BankName
                + '&SwapNoticeID=' + AllData.SwapNoticeID;

            $.myWindow.setMyWindow("EditInfo2EditBank", window);

            $.myWindow({
                iconCls: "",
                noheader: false,
                title: '修改银行',
                width: '550',
                height: '350',
                url: url,
                onClose: function () {
                    if (changeBankOK == true) {
                        $.messager.alert('消息', '修改银行成功！', 'info', function () {
                            location.reload();
                        });
                    }
                }
            });
        }

        //返回
        function Back() {
            var url = location.pathname.replace(/EditInfo.aspx/ig, 'UnSwapList.aspx');
            window.location = url;
        }

        //删除
        function Delete(decHeadID) {
            var rows = $('#datagrid').datagrid('getRows');
            if (rows == null || rows.length <= 1) {
                $.messager.alert('消息', '当前报关通知中报关单数量只剩下一个，无法删除。您可以取消当前换汇通知。', 'info', function () {
                    location.reload();
                });
                return;
            }

            $.messager.confirm('确认', '确定要在该换汇中移除该报关单吗！', function (success) {
                if (success) {
                    MaskUtil.mask();
                    $.post('?action=DeleteDecHeadFromSwapNotice', {
                        SwapNoticeID: AllData.SwapNoticeID,
                        DeleteDecHeadID: decHeadID,
                    }, function () {
                        MaskUtil.unmask();
                        $.messager.alert('消息', '移除成功！', 'info', function () {
                            location.reload();
                        });
                    })
                }
            });
        }

        //在换汇信息中增加报关单
        function AddDecHead() {
            //1600   600

            var currentDecHeadIDs = '';

            var rows = $('#datagrid').datagrid('getRows');
            for (var i = 0; i < rows.length; i++) {
                currentDecHeadIDs += rows[i].DecHeadID + ',';
            }

            currentDecHeadIDs = (currentDecHeadIDs.substring(currentDecHeadIDs.length - 1) == ',')
                ? currentDecHeadIDs.substring(0, currentDecHeadIDs.length - 1) : currentDecHeadIDs;

            var url = location.pathname.replace(/EditInfo.aspx/ig, '/AddDecHeadInEditInfo.aspx')
                + '?CurrentDecHeadIDs=' + currentDecHeadIDs
                + '&BankName=' + AllData.BankName
                + '&Currency=' + AllData.Currency;

            $.myWindow.setMyWindow("EditInfo2AddDecHeadInEditInfo", window);

            $.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '添加报关单',
                width: 1700,
                height: 600,
                onClose: function () {
                    if (closeFromAddDecHeadInEditInfo == true) {
                        closeFromAddDecHeadInEditInfo = false;


                        var url2 = location.pathname.replace(/EditInfo.aspx/ig, '/EditAddDecHead.aspx')
                            + '?DecHeadIDFromAddDecHeadInEditInfo=' + decHeadIDFromAddDecHeadInEditInfo
                            + '&BankName=' + AllData.BankName
                            + '&Currency=' + AllData.Currency
                            + '&SwapNoticeID=' + AllData.SwapNoticeID;

                        //$.myWindow.setMyWindow("EditInfo2EditAddDecHead", window);

                        //下面这个窗口打开两次，其中一个是给它关的

                        self.$.myWindow({
                            iconCls: "",
                            url: url2,
                            noheader: false,
                            title: '编辑换汇金额',
                            width: 1700,
                            height: 600,
                            onClose: function () {
                                location.reload();
                            }
                        });

                        self.$.myWindow({
                            iconCls: "",
                            url: url2,
                            noheader: false,
                            title: '编辑换汇金额',
                            width: 1700,
                            height: 600,
                            onClose: function () {
                                location.reload();
                            }
                        });

                    }
                }
            });
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="tt" class="easyui-tabs" style="width: auto;overflow:auto;" data-options="border: false,">
        <div title="编辑换汇信息" style="display: none; padding: 5px;">
            <div data-options="region:'center',border:false,">
                <form id="form1">
                    <div style="margin: 0 5px">
                        <div style="margin: 5px 0">
                            <%--<a class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="Save()">确定</a>--%>
                            <a class="easyui-linkbutton" data-options="iconCls:'icon-back'" onclick="Back()">返回</a>
                        </div>
                        <div>
                            <table style="line-height: 30px">
                                <tr>
                                    <td class="lbl">换汇银行：</td>
                                    <td>
                                        <label id="BankName"></label>
                                    </td>
                                    <td>
                                        <a class="easyui-linkbutton" data-options="iconCls:'icon-edit'" onclick="EditBank()">修改</a>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="lbl">外币总金额：</td>
                                    <td>
                                        <label id="TotalAmount"></label>
                                    </td>
                                </tr>
                            </table>
                        </div>
                        <div>
                            <a class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="AddDecHead()">增加</a>
                        </div>
                        <div style="margin-top: 10px;">
                            <table id="datagrid" title="换汇明细" data-options="
                                fitColumns:true,
                                fit:false,
                                pagination:false,
                                scrollbarSize:0,
                                rownumbers:true">
                                <thead>
                                    <tr>
                                        <th data-options="field:'ContrNo',align:'center'" style="width: 100px;">合同协议号</th>
                                        <th data-options="field:'OrderID',align:'center'" style="width: 100px;">订单编号</th>
                                        <th data-options="field:'Currency',align:'center'" style="width: 100px;">币种</th>
                                        <th data-options="field:'SwapAmount',align:'center'" style="width: 100px;">换汇金额</th>
                                        <th data-options="field:'DDate',align:'center'" style="width: 100px;">报关日期</th>
                                        <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 100px;">操作</th>
                                    </tr>
                                </thead>
                            </table>
                        </div>
                    </div>
                </form>
            </div>
        </div>
    </div>
</body>
</html>
