<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Detail.aspx.cs" Inherits="WebApp.Order.Fee.Detail" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>费用详情</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <script type="text/javascript">
        $(function () {
            var feeData = eval('(<%=this.Model.FeeData%>)');

            //费用信息初始化
            if (feeData != null && feeData != "") {
                $("#OrderID").textbox("setValue", feeData["OrderID"]);
                $("#ChargeStdName").textbox("setValue", feeData["FeeName"]);

                var chargeStdDetail = '';
                if (feeData.FeeItems != null && feeData.FeeItems.length > 0) {
                    for (var i = 0; i < feeData.FeeItems.length; i++) {
                        var itemTxt = '';
                        var isHasZero = false;
                        var feeItem = feeData.FeeItems[i];
                        itemTxt += '【' + feeItem.Name + '】';
                        itemTxt += '(单价：' + feeItem.UnitPrice + ' ' + feeItem.CurrencyCN + ') ';

                        if (feeItem.Units != null && feeItem.Units.length > 0) {
                            for (var j = 0; j < feeItem.Units.length; j++) {
                                itemTxt += feeItem.Units[j].Value + ' ' + feeItem.Units[j].Unit;
                                if (j != feeItem.Units.length - 1) {
                                    itemTxt += '， ';
                                }
                                if (feeItem.Units[j].Value == 0) {
                                    isHasZero = true;
                                }
                            }
                        }

                        itemTxt += '\r\n';

                        if (isHasZero == false) {
                            chargeStdDetail += itemTxt;
                        }
                    }
                }
                $("#ChargeStdDetail").textbox("setValue", chargeStdDetail);
                $("#ChargeStdPrice").textbox("setValue", feeData["StandardPrice"]);

                $("#Name").textbox("setValue", feeData["Name"]);
                $("#Count").textbox("setValue", feeData["Count"]);
                $("#UnitPrice").textbox("setValue", feeData["UnitPrice"]);
                $("#TotalPrice").textbox("setValue", feeData["UnitPrice"] * feeData["Count"]);
                $("#Currency").textbox("setValue", feeData["Currency"]);
                $("#Rate").textbox("setValue", feeData["Rate"]);
                $("#IsPaid").textbox("setValue", feeData["IsPaid"]);
                $("#PaymentDate").textbox("setValue", feeData["PaymentDate"]);
            }

            //文件面板初始化
            $('#fileContainer').panel({
                iconCls: 'icon-blue-fujian',
                closable: true,
                height: 'auto',
                border: false,
                onClose: function () {
                   // self.parent.$('iframe').parent().window('close');
                $.myWindow.close();
                }
            });

            //文件列表初始化
            $('#files').myDatagrid({
                border: false,
                showHeader: false,
                pagination: false,
                rownumbers: false,
                fitcolumns: true,
                rowStyler: function (index, row) {
                    return 'background-color:white;';
                },
                loadFilter: function (data) {
                    $('#fileContainer').panel('setTitle', '费用附件(' + data.total + ')');
                    if (data.total == 0) {
                        $('#unUpload').css('display', 'block');
                    } else {
                        $('#unUpload').css('display', 'none');
                    }
                    return data;
                },
                onLoadSuccess: function (data) {
                    var panel = $("#fileContainer");
                    var header = panel.find('div.datagrid-header');
                    header.css({
                        'visibility': 'hidden'
                    });
                    var tr = panel.find('div.datagrid-body tr');
                    tr.each(function () {
                        var td = $(this).children('td');
                        td.css({
                            'border-width': '0'
                        });
                    });

                    var heightValue = 600;
                    $("#unUpload").next().find(".datagrid-wrap").height(heightValue);
                    $("#unUpload").next().find(".datagrid-wrap").find(".datagrid-view").height(heightValue);
                    $("#unUpload").next().find(".datagrid-wrap").find(".datagrid-view").find(".datagrid-view2").height(heightValue);
                    $("#unUpload").next().find(".datagrid-wrap").find(".datagrid-view").find(".datagrid-view2").height(heightValue);
                    $("#unUpload").next().find(".datagrid-wrap").find(".datagrid-view").find(".datagrid-view2").find(".datagrid-body").height(heightValue);

                    ////因原x按钮使用不正常，遮挡后，添加一个x按钮
                    //var originContent = $("#form2").find(".panel-title").html();
                    //var $newElement = $('<span style="margin-left: 365px;"><a href="javascript:void(0);" style="text-decoration:none;" onclick="Close()">x</a></span>');
                    //$("#form2").find(".panel-title").append($newElement);


                }
            });
        });

        //关闭窗口
        function Close() {
            $.myWindow.close();
        }

        //预览文件
        function View(url) {
            var ewindow = $.myWindow.getMyWindow("Display2Detail");

            $('#viewfileImg').css('display', 'none');
            $('#viewfilePdf').css('display', 'none');
            if (url.toLowerCase().indexOf('pdf') > 0) {
                $('#viewfilePdf').attr('src', url);
                $('#viewfilePdf').css("display", "block");
                $('#viewFileDialog').window('open').window('center');
            }
            else if (url.toLowerCase().indexOf('doc') > 0 || url.toLowerCase().indexOf('docx') > 0) {
                let a = document.createElement('a');
                document.body.appendChild(a);
                a.href = url;
                a.download = "";
                a.click();
            }
            else {
                $('#viewfileImg').attr('src', url);
                $('#viewfileImg').css("display", "block");
                $('#viewFileDialog').window('open').window('center');
            }
            $("#unUpload").next().find(".datagrid-wrap").find(".datagrid-view").find(".datagrid-view2").find(".datagrid-body").find(".datagrid-btable")
                .find("td[field='Btn']").css({ "color": "#000000" });
        }

        //下载文件
        function Download(url) {
            let a = document.createElement('a');
            document.body.appendChild(a);
            a.href = url;
            a.download = "";
            a.click();
            $("#unUpload").next().find(".datagrid-wrap").find(".datagrid-view").find(".datagrid-view2").find(".datagrid-body").find(".datagrid-btable")
                .find("td[field='Btn']").css({ "color": "#000000" });
        }

        //文件图片
        function ShowImg(val, row, index) {
            return '<img src="../../App_Themes/xp/images/wenjian.png" />';
        }

        //操作
        function Operation(val, row, index) {
            var buttons = row.Name + '<br/>';
            buttons += '<a href="javascript:void(0)"><span style="color: cornflowerblue;" onclick="View(\'' + row.Url + '\')">预览</span></a>';
            buttons += '<a href="javascript:void(0)"><span style="color: cornflowerblue; margin-left: 10px;" onclick="Download(\'' + row.Url + '\')">下载</span></a>';
            return buttons;
        }
    </script>
</head>
<body class="easyui-layout">
    <div data-options="region:'west',border:false,collapsible:false,split:true" style="width: 50%; min-width: 350px;">
        <form id="form1" runat="server" method="post">
            <div title="费用信息" class="easyui-panel" data-options="height:'auto',border:false">
                <div class="sub-container">
                    <table id="table1" class="row-info" cellspacing="0" cellpadding="0">
                        <tr>
                            <td class="lbl">订单编号：</td>
                            <td>
                                <input class="easyui-textbox" id="OrderID" data-options="height:26,width:250" disabled="disabled" />
                            </td>
                        </tr>
                        <tr>
                            <td class="lbl">收费项目：</td>
                            <td>
                                <input class="easyui-textbox" id="ChargeStdName" data-options="height:78,width:250,multiline:true," disabled="disabled" />
                            </td>
                        </tr>
                        <tr>
                            <td class="lbl">应收明细：</td>
                            <td>
                                <input class="easyui-textbox" id="ChargeStdDetail" data-options="height:104,width:250,multiline:true," disabled="disabled" />
                            </td>
                        </tr>
                        <tr>
                            <td class="lbl">参考总价：</td>
                            <td>
                                <input class="easyui-textbox" id="ChargeStdPrice" data-options="height:26,width:250" disabled="disabled" />
                            </td>
                        </tr>
                        <tr>
                            <td class="lbl">备注：</td>
                            <td>
                                <input class="easyui-textbox" id="Name" data-options="height:26,width:250" disabled="disabled" />
                            </td>
                        </tr>
                        <tr>
                            <td class="lbl">数量：</td>
                            <td>
                                <input class="easyui-textbox" id="Count" data-options="height:26,width:250" disabled="disabled" />
                            </td>
                        </tr>
                        <tr>
                            <td class="lbl">单价：</td>
                            <td>
                                <input class="easyui-textbox" id="UnitPrice" data-options="height:26,width:250" disabled="disabled" />
                            </td>
                        </tr>
                        <tr>
                            <td class="lbl">总价：</td>
                            <td>
                                <input class="easyui-textbox" id="TotalPrice" data-options="height:26,width:250" disabled="disabled" />
                            </td>
                        </tr>
                        <tr>
                            <td class="lbl">币种：</td>
                            <td>
                                <input class="easyui-textbox" id="Currency" data-options="height:26,width:250" disabled="disabled" />
                            </td>
                        </tr>
                        <tr>
                            <td class="lbl">汇率：</td>
                            <td>
                                <input class="easyui-textbox" id="Rate" data-options="height:26,width:250" disabled="disabled" />
                            </td>
                        </tr>
                        <tr>
                            <td class="lbl">是否付款：</td>
                            <td>
                                <input class="easyui-textbox" id="IsPaid" data-options="height:26,width:250" disabled="disabled" />
                            </td>
                        </tr>
                        <tr>
                            <td class="lbl">付款日期：</td>
                            <td>
                                <input class="easyui-textbox" id="PaymentDate" data-options="height:26,width:250" disabled="disabled" />
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </form>
    </div>
    <div data-options="region:'center',border:false,collapsible:false,split:true" style="width: 50%;">
        <form id="form2">
            <div id="fileContainer" title="费用附件" class="easyui-panel" style="margin: 5px">
                <div class="sub-container">
                    <div id="unUpload" style="margin-left: 5px">
                        <p>未上传</p>
                    </div>
                    <div>
                        <table id="files" data-options="nowrap:false">
                            <thead>
                                <tr>
                                    <th data-options="field:'img',formatter:ShowImg">图片</th>
                                    <th style="width: auto" data-options="field:'Btn',align:'left',formatter:Operation">操作</th>
                                </tr>
                            </thead>
                        </table>
                    </div>
                </div>
            </div>
        </form>
    </div>

    <div id="viewFileDialog" class="easyui-window" title="查看附件" data-options="iconCls:'pag-list',modal:true,collapsible:false,minimizable:false,maximizable:true,resizable:true,closed:true" style="width: 950px; height: 550px;">
        <img id="viewfileImg" src="" style="width: auto; height: auto; max-width: 100%; max-height: 100%;" />
        <iframe id="viewfilePdf" src="" width="100%" height="100%" frameborder="0" scroll="no"></iframe>
    </div>

</body>
</html>
