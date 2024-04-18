<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Complete.aspx.cs" Inherits="WebApp.Finance.Swap.Complete" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>完成换汇</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <script type="text/javascript">

        var VaultData = eval('(<%=this.Model.VaultData%>)');
        var AllData = eval('(<%=this.Model.AllData%>)');

        $(function () {
            $("#tt").height(document.documentElement.clientHeight + "px");  //修改 layout 高度

            //列表初始化
            //debugger;
            $('#datagrid').myDatagrid({
                fitColumns: true,
                fit: false,
                pagination: false,
                scrollbarSize: 0,
                rownumbers: true,
                pagination: false,
                scrollbarSize: 0,
                onLoadSuccess: function (data) {
                    var leftTrs = $(".datagrid-view1>.datagrid-body tr");
                    var rightTrs = $(".datagrid-view2>.datagrid-body tr");

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
            $('#TotalRmb').numberbox({
                onChange: function () {
                    Calculate();
                }
            })
            //初始化金库
            $('#VaultOut').combobox({
                data: VaultData,
                onSelect: function (record) {
                    $.post('?action=RmbAccountSelect', { ID: record.Value, }, function (data) {
                        data = eval(data);
                        $('#AccountOut').combobox({
                            data: eval(data)
                        });
                    })
                }
            });
            $('#VaultMid').combobox({
                data: VaultData,
                onSelect: function (record) {
                    $.post('?action=ForeignAccountSelect', { ID: record.Value, NoticeID: AllData["ID"] }, function (data) {
                        data = eval(data);
                        $('#AccountMid').combobox({
                            data: eval(data)
                        });
                    })
                }
            });
            $('#VaultIn').combobox({
                data: VaultData,
                onSelect: function (record) {
                    $.post('?action=ForeignAccountSelectIn', { ID: record.Value, NoticeID: AllData["ID"] }, function (data) {
                        data = eval(data);
                        $('#AccountIn').combobox({
                            data: eval(data)
                        });
                    })
                }
            });

            if (AllData != null && AllData != "") {
                $('#TotalAmount').numberbox('setValue', AllData.TotalAmount);
                $('#BankName').textbox('setValue', AllData.BankName);
                $('#RealTimeExchangeRate').textbox('setValue', AllData.RealTimeExchangeRate);
            }

            //换汇完成默认当天
            $('#ExchangeDate').datebox('setValue', new Date().format("yyyy-MM-dd"));

            //绑定日志信息           
            var data = new FormData($('#form1')[0]);
            data.append("ID", AllData.ID);
            $.ajax({
                url: '?action=LoadLogs',
                type: 'POST',
                data: data,
                dataType: 'JSON',
                cache: false,
                processData: false,
                contentType: false,
                success: function (data) {
                    showLogContent(data);
                },
                error: function (msg) {
                    alert("ajax连接异常：" + msg);
                }
            });

        });
        //计算
        function Calculate() {
            var TotalAmount = $('#TotalAmount').numberbox('getValue');
            var TotalRmb = $('#TotalRmb').numberbox('getValue');
            if (TotalAmount != null && TotalRmb != null) {
                TotalRmb = (TotalRmb / TotalAmount).toFixed(6);
                $('#ExchangeRate').numberbox('setValue', TotalRmb);
            } else {
                $.messager.alert('提示', '请输入信息');
            }
        }
        //关闭弹出页面
        function Close() {
            var url = location.pathname.replace(/Complete.aspx/ig, 'UnSwapList.aspx');
            window.location = url;
        }
        //保存校验
        function Save() {
            //验证表单数据
            if (!Valid('form1')) {
                return;
            }
            var data = new FormData($('#form1')[0]);
            if (AllData != null) {
                data.append('ID', AllData["ID"])
                data.append('BankName', $('#BankName').textbox('getValue'))
                data.append('ExchangeRate', $('#ExchangeRate').numberbox('getValue'))
                data.append('TotalRmb', $('#TotalRmb').numberbox('getValue'))
                data.append('AccountOut', $('#AccountOut').combobox('getValue'))
                data.append('AccountMid', $('#AccountMid').combobox('getValue'))
                data.append('AccountIn', $('#AccountIn').combobox('getValue'))
                data.append('VaultOut', $('#VaultOut').combobox('getValue'))
                data.append('VaultIn', $('#VaultIn').combobox('getValue'))
                data.append('VaultMid', $('#VaultMid').combobox('getValue'))
                data.append('SeqNoOut', $('#SeqNoOut').textbox('getValue'))
                data.append('SeqNoIn', $('#SeqNoIn').textbox('getValue'))
                data.append('SeqNoMidR', $('#SeqNoMidR').textbox('getValue'))
                data.append('SeqNoMidP', $('#SeqNoMidP').textbox('getValue'))
                data.append('SeqNoPoundage', $('#SeqNoPoundage').textbox('getValue'))
                data.append('Poundage', $('#Poundage').numberbox('getValue'))
                data.append('ExchangeDate', $('#ExchangeDate').datebox('getValue'))
                data.append('RealTimeExchangeRate', $('#RealTimeExchangeRate').textbox('getValue'))
            }
            $.messager.confirm('确认', '请您再次确认是否换汇完成！', function (success) {
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

         //显示日志数据
        function showLogContent(data) {
            var str = "";//定义用于拼接的字符串
            $.each(data.rows, function (index, row) {
                if (row.Summary != null) {
                    str = "<p>" + row.CreateDate + "&nbsp;&nbsp;" + row.Summary + "</p>"
                }
                //追加到table中
                $("#LogContent").append(str);
            });
        }
    </script>
    <style>
        .lab {
            padding-left: 5px;
        }
    </style>
</head>
<body class="easyui-layout">
    <div id="tt" class="easyui-tabs" style="width: auto; overflow: auto;" data-options="border: false,">
        <div title="完成换汇" style="display: none; padding: 5px;">
            <div data-options="region:'center',border:false,">
                <form id="form1" runat="server" method="post">
                    <div style="margin: 0 5px">
                        <div style="margin: 5px 0">
                            <a class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="Save()">确认完成</a>
                            <a class="easyui-linkbutton" data-options="iconCls:'icon-back'" onclick="Close()">返回</a>
                        </div>
                        <div>
                            <table id="datagrid" title="换汇明细" >
                            
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
                        <div style="margin-top: 5px; margin-left: 2px;">
                        <div class="easyui-panel" title="日志记录" style="width: 100%;">
                            <div class="sub-container">
                                <div class="text-container" id="LogContent">
                                </div>
                            </div>
                        </div>
                    </div>
                        <div style="margin: 5px 0">
                            <label style="font-size: 15px; font-weight: 600; color: orangered">换汇汇率</label>
                        </div>
                        <div id="SwapInf">
                            <table style="line-height: 30px">
                                <tr>
                                    <td class="lbl">换汇银行：</td>
                                    <td>
                                        <input class="easyui-textbox" id="BankName" data-options="height:26,width:200,editable:false" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="lbl">外币总金额：</td>
                                    <td>
                                        <input class="easyui-numberbox" id="TotalAmount" data-options="min:0,precision:2,height:26,width:200,editable:false" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="lbl">RMB总金额：</td>
                                    <td>
                                        <input class="easyui-numberbox" id="TotalRmb" data-options="min:0,precision:2,required:true,height:26,width:200,validType:'length[1,50]'" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="lbl">换汇汇率：</td>
                                    <td>
                                        <input class="easyui-numberbox" id="ExchangeRate"
                                            data-options="min:0,precision:6,required:true,height:26,width:200,editable:false,tipPosition:'right'" />
                                    </td>
                                     <td class="lbl">换汇日期：</td>
                                    <td>
                                        <input class="easyui-datebox" id="ExchangeDate" data-options="required:true,height:26,width:150,editable:false" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="lbl">实时汇率：</td>
                                    <td>
                                        <input class="easyui-numberbox" id="RealTimeExchangeRate" data-options="min:0,precision:4,required:true,height:26,width:200,validType:'length[1,50]'" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        <div style="margin: 5px 0">
                            <label style="font-size: 15px; font-weight: 600; color: orangered">换汇账户</label>
                        </div>
                        <div id="SwapAcc">
                            <table>
                                <tr>
                                    <td class="lbl" colspan="4" style="font-weight: 600;">人民币账户：</td>
                                </tr>
                                <tr>
                                    <td class="lbl">金库：</td>
                                    <td>
                                        <input class="easyui-combobox" id="VaultOut" data-options="required:true,height:26,width:150,valueField:'Value',textField:'Text',tipPosition:'right'" />
                                    </td>
                                    <td class="lbl lab">账户：</td>
                                    <td>
                                        <input class="easyui-combobox" id="AccountOut" data-options="required:true,height:26,width:150,valueField:'Value',textField:'Text',tipPosition:'right'" />
                                    </td>
                                    <td class="lbl lab">流水号(付)：</td>
                                    <td>
                                        <input class="easyui-textbox" id="SeqNoOut" data-options="required:true,height:26,width:150,validType:'length[1,50]',tipPosition:'right'" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="lbl" colspan="4" style="padding-top: 10px; font-weight: 600">外币账户：</td>
                                </tr>
                                <tr>
                                    <td class="lbl">金库：</td>
                                    <td>
                                        <input class="easyui-combobox" id="VaultMid" data-options="height:26,width:150,valueField:'Value',textField:'Text'" />
                                    </td>
                                    <td class="lbl lab">账户：</td>
                                    <td>
                                        <input class="easyui-combobox" id="AccountMid" data-options="height:26,width:150,valueField:'Value',textField:'Text'" />
                                    </td>
                                    <td class="lbl lab">流水号(收)：</td>
                                    <td>
                                        <input class="easyui-textbox" id="SeqNoMidR" data-options="height:26,width:150,validType:'length[1,50]'" />
                                    </td>
                                    <td class="lbl lab">流水号(付)：</td>
                                    <td>
                                        <input class="easyui-textbox" id="SeqNoMidP" data-options="height:26,width:150,validType:'length[1,50]'" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="lbl" colspan="4" style="padding-top: 10px; font-weight: 600">供应商账户：</td>
                                </tr>
                                <tr>
                                    <td class="lbl">金库：</td>
                                    <td>
                                        <input class="easyui-combobox" id="VaultIn" data-options="required:true,height:26,width:150,valueField:'Value',textField:'Text',tipPosition:'right'" />
                                    </td>
                                    <td class="lbl lab">账户：</td>
                                    <td>
                                        <input class="easyui-combobox" id="AccountIn" data-options="required:true,height:26,width:150,valueField:'Value',textField:'Text',tipPosition:'right'" />
                                    </td>
                                    <td class="lbl lab">流水号(收)：</td>
                                    <td>
                                        <input class="easyui-textbox" id="SeqNoIn" data-options="required:true,height:26,width:150,validType:'length[1,50]',tipPosition:'right'" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="lbl" colspan="4" style="padding-top: 10px; font-weight: 600">换汇手续费：</td>
                                </tr>
                                <tr>
                                    <td class="lbl">金额：</td>
                                    <td>
                                        <input class="easyui-numberbox" id="Poundage" data-options="min:0,precision:2,height:26,width:150,validType:'length[1,18]',tipPosition:'right'" />
                                    </td>
                                    <td class="lbl lab">流水号：</td>
                                    <td>
                                        <input class="easyui-textbox" id="SeqNoPoundage" data-options="height:26,width:150,validType:'length[1,50]',tipPosition:'right'" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </div>
                </form>
            </div>
        </div>
    </div>
</body>
</html>
