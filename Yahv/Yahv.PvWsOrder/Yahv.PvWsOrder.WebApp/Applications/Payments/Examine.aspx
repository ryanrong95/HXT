<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Examine.aspx.cs" Inherits="Yahv.PvOms.WebApp.Applications.Payments.Examine" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script src="../../Content/Themes/Scripts/PvWsOrder.js"></script>
    <script>
        var ID = getQueryString("ID");
        var closeFlag = false;
        $(function () {
            //页面初始化
            window.grid = $("#tab1").myDatagrid({
                fitColumns: true,
                pagination: false,
                fit: false,
                onLoadSuccess: function (data) {
                    AddSubtotalRow();
                }
            });
            $('#pi').myDatagrid({
                fitColumns: true,
                fit: false,
                pagination: false,
                actionName: 'LoadInvoice',
                rowStyler: function (index, row) {
                    return 'background-color:white;';
                },
                columns: [[
                    { field: 'ID', title: '', width: 70, align: 'center', hidden: true, },
                    { field: 'CustomName', title: '', width: 70, align: 'center', hidden: true },
                    { field: 'FileName', title: '', width: 70, align: 'center', hidden: true },
                    { field: 'FileType', title: '', width: 70, align: 'center', hidden: true },
                    { field: 'Url', title: '', width: 70, align: 'center', hidden: true },
                    { field: 'Btn', title: '', width: 100, align: 'left', formatter: OperationPI }
                ]],
                onLoadSuccess: function (data) {
                    var obj = $(".pi");
                    var wrap = obj.find('div.datagrid-wrap');
                    wrap.css({
                        'border': '0',
                    });
                    var view = obj.find('div.datagrid-view');
                    view.css({
                        'height': data.rows.length * 32,
                    });
                    var header = obj.find('div.datagrid-header');
                    header.css({
                        'display': 'none',
                    });
                    var tr = obj.find('div.datagrid-body tr');
                    tr.each(function () {
                        var td = $(this).children('td');
                        td.css({
                            'border-width': '0',
                            'padding': '0',
                        });
                    });
                },
            });
            $('#proxy').myDatagrid({
                fitColumns: true,
                fit: false,
                pagination: false,
                actionName: 'LoadProxy',
                rowStyler: function (index, row) {
                    return 'background-color:white;';
                },
                columns: [[
                    { field: 'ID', title: '', width: 70, align: 'center', hidden: true, },
                    { field: 'CustomName', title: '', width: 70, align: 'center', hidden: true },
                    { field: 'FileName', title: '', width: 70, align: 'center', hidden: true },
                    { field: 'FileType', title: '', width: 70, align: 'center', hidden: true },
                    { field: 'Url', title: '', width: 70, align: 'center', hidden: true },
                    { field: 'Btn', title: '', width: 100, align: 'left', formatter: OperationProxy }
                ]],
                onLoadSuccess: function (data) {
                    var obj = $(".proxy");
                    var wrap = obj.find('div.datagrid-wrap');
                    wrap.css({
                        'border': '0',
                    });
                    var view = obj.find('div.datagrid-view');
                    view.css({
                        'height': data.rows.length * 32,
                    });
                    var header = obj.find('div.datagrid-header');
                    header.css({
                        'display': 'none',
                    });
                    var tr = obj.find('div.datagrid-body tr');
                    tr.each(function () {
                        var td = $(this).children('td');
                        td.css({
                            'border-width': '0',
                            'padding': '0',
                        });
                    });
                },
            });
            $('#confirm').myDatagrid({
                fitColumns: true,
                fit: false,
                pagination: false,
                actionName: 'LoadConfirm',
                rowStyler: function (index, row) {
                    return 'background-color:white;';
                },
                columns: [[
                    { field: 'ID', title: '', width: 70, align: 'center', hidden: true, },
                    { field: 'CustomName', title: '', width: 70, align: 'center', hidden: true },
                    { field: 'FileName', title: '', width: 70, align: 'center', hidden: true },
                    { field: 'FileType', title: '', width: 70, align: 'center', hidden: true },
                    { field: 'Url', title: '', width: 70, align: 'center', hidden: true },
                    { field: 'Btn', title: '', width: 100, align: 'left', formatter: OperationConfirm }
                ]],
                onLoadSuccess: function (data) {
                    var obj = $(".confirm");
                    var wrap = obj.find('div.datagrid-wrap');
                    wrap.css({
                        'border': '0',
                    });
                    var view = obj.find('div.datagrid-view');
                    view.css({
                        'height': data.rows.length * 32,
                    });
                    var header = obj.find('div.datagrid-header');
                    header.css({
                        'display': 'none',
                    });
                    var tr = obj.find('div.datagrid-body tr');
                    tr.each(function () {
                        var td = $(this).children('td');
                        td.css({
                            'border-width': '0',
                            'padding': '0',
                        });
                    });
                },
            });
            $('#logs').myDatagrid({
                fitColumns: true,
                fit: false,
                pagination: false,
                actionName: 'LoadLogs',
                columns: [[
                    { field: 'CreateDate', title: '审批日期', width: 100, align: 'center', },
                    { field: 'StepName', title: '步骤名称', width: 100, align: 'center', },
                    { field: 'Status', title: '审批结果', width: 100, align: 'center', },
                    { field: 'Summary', title: '备注信息', width: 300, align: 'center', },
                ]],
            });
            //审核通过
            $("#btnPass").click(function () {
                //各种验证
                if (!ValidationOrder()) {
                    return;
                }
                $.messager.confirm('确认', '请您再次确认是否审核通过。', function (success) {
                    if (success) {
                        $.post('?action=Pass', { ID: ID }, function (result) {
                            var res = JSON.parse(result);
                            if (res.success) {
                                top.$.timeouts.alert({ position: "TC", msg: res.message, type: "success" });
                                BackToList();
                            }
                            else {
                                top.$.timeouts.alert({ position: "TC", msg: res.message, type: "error" });
                            }
                        })
                    }
                });
                return false;
            })
            //审批驳回
            $("#btnReject").click(function () {
                $.myWindow.setMyWindow("Examine", window);
                $.myWindow({
                    title: "审核驳回",
                    url: 'ExamineReject.aspx?ID=' + ID,
                    width: 550,
                    height: 300,
                    onClose: function () {
                        if (closeFlag == true) {
                            BackToList();
                        }
                    }
                });
                return false;
            })
            //返回列表
            $("#btnBack").click(function () {
                BackToList();
                return false;
            })

            Init();
        })
    </script>
    <script>
        function Init() {
            if (model.ApplicationData != null) {
                $("#CreateDate").text(model.ApplicationData.CreateDate);
                $("#ClientName").text(model.ApplicationData.ClientName);
                $("#EnterCode").text(model.ApplicationData.EnterCode);
                $("#AvailableBalance").text(Number(model.BalanceData.AvailableBalance).toFixed(4) + " 美元");

                $("#PayerName").text(model.ApplicationData.PayerName);
                //$("#PayerBankName").text(model.ApplicationData.PayerBankName);
                //$("#PayerBankAccount").text(model.ApplicationData.PayerBankAccount);
                $("#PayerMethod").text(model.ApplicationData.PayerMethod);
                $("#PayerCurrency").text(model.ApplicationData.PayerCurrency);

                $("#PayeeName").text(model.ApplicationData.PayeeName);
                $("#PayeeBankName").text(model.ApplicationData.PayeeBankName);
                $("#PayeeBankAccount").text(model.ApplicationData.PayeeBankAccount);
                $("#PayeeMethod").text(model.ApplicationData.PayeeMethod);
                $("#PayeeCurrency").text(model.ApplicationData.PayeeCurrency);

                $("#InCompanyName").text(model.ApplicationData.InCompanyName);
                $("#InBankName").text(model.ApplicationData.InBankName);
                $("#InBankAccount").text(model.ApplicationData.InBankAccount);
                $("#OutCompanyName").text(model.ApplicationData.OutCompanyName);
                $("#OutBankName").text(model.ApplicationData.OutBankName);
                $("#OutBankAccount").text(model.ApplicationData.OutBankAccount);

                
            }
        }
        //PI文件操作
        function OperationPI(val, row, index) {
            return '<img src="../../Content/Themes/Images/blue-fujian.png" />'
                + '<a href="javascript:void(0);" style="margin-left: 5px;" onclick="View(\'' + row.Url + '\')">' + row.CustomName + '</a>';
        }
        //委托文件操作
        function OperationProxy(val, row, index) {
            return '<img src="../../Content/Themes/Images/blue-fujian.png" />'
                + '<a href="javascript:void(0);" style="margin-left: 5px;" onclick="View(\'' + row.Url + '\')">' + row.CustomName + '</a>';
        }
        //收付款凭证文件操作
        function OperationConfirm(val, row, index) {
            return '<img src="../../Content/Themes/Images/blue-fujian.png" />'
                + '<a href="javascript:void(0);" style="margin-left: 5px;" onclick="View(\'' + row.Url + '\')">' + row.CustomName + '</a>';
        }
        //查看图片
        function View(url) {
            $('#viewfileImg').css('display', 'none');
            $('#viewfilePdf').css('display', 'none');
            if (url.toLowerCase().indexOf('pdf') > 0) {
                $('#viewfilePdf').attr('src', url);
                $('#viewfilePdf').css("display", "block");
                $('#viewFileDialog').window('open').window('center');
            }
            else if (url.toLowerCase().indexOf('docx') > 0 || url.toLowerCase().indexOf('doc') > 0) {
                $('#viewfilePdf').css("display", "none");
                $('#viewfileImg').css("display", "none");
                let a = document.createElement('a');
                a.href = url;
                a.download = "";
                a.click();
            }
            else {
                $('#viewfileImg').attr('src', url);
                $('#viewfileImg').css("display", "block");
                $('#viewFileDialog').window('open').window('center');
            }
        }
    </script>
    <script>
        function ValidationOrder() {
            ////验证合同发票
            //var pi = $("#pi").datagrid("getRows")
            //if (pi.length == 0) {
            //    $.messager.alert('提示', '请上传合同发票');
            //    return false;
            //}
            //验证付款委托书
            var proxy = $("#proxy").datagrid("getRows")
            if (proxy.length == 0) {
                $.messager.alert('提示', '请上传付款委托书');
                return false;
            }
            return true;
        }
        //返回列表页
        function BackToList() {
            var url = location.pathname.replace('Examine.aspx', 'ListExamine.aspx')
            window.location = url;
        }

        //添加合计行
        function AddSubtotalRow() {
            //添加合计行
            $('#tab1').datagrid('appendRow', {
                OrderID: '<span class="subtotal" style="color:red">合计：</span>',
                Currency: '<span class="subtotal">--</span>',
                TotalPrice: '<span class="subtotal">--</span>',
                AppliedPrice: '<span class="subtotal">--</span>',
                CurrentPrice: '<span class="subtotal" style="color:red">' + compute('CurrentPrice') + '</span>',
            });
        }
        //删除合计行
        function RemoveSubtotalRow() {
            var lastIndex = $('#tab1').datagrid('getRows').length - 1;
            $('#tab1').datagrid('deleteRow', lastIndex);
        }
        //计算合计值
        function compute(colName) {
            var rows = $('#tab1').datagrid('getRows');
            var total = 0;
            for (var i = 0; i < rows.length; i++) {
                if (rows[i][colName] != undefined) {
                    total += parseFloat(Number(rows[i][colName]));
                }
            }
            return total;
        }
    </script>
    <style>
        .lbl {
            width: 100px
        }

        .panel .panel-htop {
            padding-top: 5px
        }

        .panel-header {
            border-bottom: none
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-layout" style="width: 100%; height: 100%; border: none">
        <div data-options="region:'north'," style="height: 40px; border: none; padding-top: 11px; padding-left: 5px">
            <a id="btnPass" class="easyui-linkbutton" iconcls="icon-ok">审核通过</a>
            <a id="btnReject" class="easyui-linkbutton" iconcls="icon-cancel">审核驳回</a>
            <a id="btnBack" class="easyui-linkbutton" iconcls="icon-back">返回列表</a>
        </div>
        <div data-options="region:'west'," style="width: 30%; min-width: 360px; border: none; padding-left: 5px">
            <div class="easyui-panel" title="客户信息" style="border: none">
                <table class="liebiao">
                    <tr>
                        <td class="lbl">申请日期：</td>
                        <td>
                            <label id="CreateDate"></label>
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl">客户名称：</td>
                        <td>
                            <label id="ClientName"></label>
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl">客户编号：</td>
                        <td>
                            <label id="EnterCode"></label>
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl">可用授信额度：</td>
                        <td>
                            <label id="AvailableBalance"></label>
                        </td>
                    </tr>
                </table>
            </div>
            <div class="easyui-panel" title="客户付款人" style="border: none;">
                <table class="liebiao">
                    <tr>
                        <td class="lbl">账户名称：</td>
                        <td>
                            <label id="PayerName"></label>
                        </td>
                    </tr>
                    <%--<tr>
                        <td class="lbl">银行名称：</td>
                        <td>
                            <label id="PayerBankName"></label>
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl">银行账号：</td>
                        <td>
                            <label id="PayerBankAccount"></label>
                        </td>
                    </tr>--%>
                    <tr>
                        <td class="lbl">支付币种：</td>
                        <td>
                            <label id="PayerCurrency"></label>
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl">支付方式：</td>
                        <td>
                            <label id="PayerMethod"></label>
                        </td>
                    </tr>
                </table>
            </div>
            <div class="easyui-panel" title="我方收款人" style="border: none">
                <table class="liebiao">
                    <tr>
                        <td class="lbl">账户名称：</td>
                        <td>
                            <label id="InCompanyName"></label>
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl">银行名称：</td>
                        <td>
                            <label id="InBankName"></label>
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl">银行账号：</td>
                        <td>
                            <label id="InBankAccount"></label>
                        </td>
                    </tr>
                </table>
            </div>
            <div class="easyui-panel" title="我方付款人" style="border: none">
                <table class="liebiao">
                    <tr>
                        <td class="lbl">账户名称：</td>
                        <td>
                            <label id="OutCompanyName"></label>
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl">银行名称：</td>
                        <td>
                            <label id="OutBankName"></label>
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl">银行账号：</td>
                        <td>
                            <label id="OutBankAccount"></label>
                        </td>
                    </tr>
                </table>
            </div>
            <div class="easyui-panel" title="供应商收款人" style="border: none">
                <table class="liebiao">
                    <tr>
                        <td class="lbl">账户名称：</td>
                        <td>
                            <label id="PayeeName"></label>
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl">银行名称：</td>
                        <td>
                            <label id="PayeeBankName"></label>
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl">银行账号：</td>
                        <td>
                            <label id="PayeeBankAccount"></label>
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl">支付币种：</td>
                        <td>
                            <label id="PayeeCurrency"></label>
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl">支付方式：</td>
                        <td>
                            <label id="PayeeMethod"></label>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <div data-options="region:'center'," style="width: 70%; border: none; padding-left: 5px">
            <div>
                <table id="tab1" title="申请明细项">
                    <thead>
                        <tr>
                            <th data-options="field:'OrderID',align:'center'" style="width: 200px;">订单编号</th>
                            <th data-options="field:'Currency',align:'center'" style="width: 100px">币种</th>
                            <th data-options="field:'TotalPrice',align:'center'" style="width: 100px">订单总金额</th>
                            <th data-options="field:'AppliedPrice',align:'center'" style="width: 100px">已申请金额</th>
                            <th data-options="field:'CurrentPrice',align:'center'" style="width: 100px">本次申请金额</th>
                        </tr>
                    </thead>
                </table>
            </div>
            <div class="pi">
                <table id="pi" title="合同发票">
                </table>
            </div>
            <div class="proxy">
                <table id="proxy" title="代付委托书">
                </table>
            </div>
            <div class="confirm">
                <table id="confirm" title="收付款凭证">
                </table>
            </div>
            <div class="logs">
                <table id="logs" title="审批日志">
                </table>
            </div>
        </div>
        <div id="viewFileDialog" class="easyui-window" title="查看附件" data-options="iconCls:'pag-list',modal:true,collapsible:false,minimizable:false,maximizable:true,resizable:true,closed:true" style="width: 800px; height: 500px; min-width: 70%; min-height: 80%">
            <img id="viewfileImg" src="" style="width: auto; height: auto; max-width: 100%; max-height: 100%;" />
            <iframe id="viewfilePdf" src="" width="100%" height="100%" frameborder="0" scroll="no"></iframe>
        </div>
    </div>
</asp:Content>

