<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Detail.aspx.cs" Inherits="WebApp.HKWarehouse.Fee.Approved.Detail" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <link href="../../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../../Scripts/Ccs.js"></script>
    <script type="text/javascript">

        var PaymentType = eval('(<%=this.Model.PaymentType%>)');
        var WarehousePremiumType = eval('(<%=this.Model.WarehousePremiumType%>)');
        var FeeData = eval('(<%=this.Model.FeeData%>)');

        var FeeID = getQueryString('FeeID');
        $(function () {
            //文件列表初始化
            $('#files').myDatagrid({
                nowrap: false,
                fitColumns: true,
                fit: false,
                pagination: false,
                actionName: 'filedata',
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
                columns: [[
                    { field: 'ID', title: '', width: 70, align: 'center', hidden: true, },
                    { field: 'Name', title: '', width: 70, align: 'center', hidden: true },
                    //{ field: 'FileType', title: '', width: 70, align: 'center', hidden: true },
                    { field: 'FileFormat', title: '', width: 70, align: 'center', hidden: true },
                    { field: 'WebUrl', title: '', width: 70, align: 'center', hidden: true },
                    { field: 'Url', title: '', width: 70, align: 'center', hidden: true },
                    { field: 'Btn', title: '', width: 100, align: 'left', formatter: Operation }
                ]],
                onLoadSuccess: function (data) {
                    var obj = $(".pi");
                    var wrap = obj.find('div.datagrid-wrap');
                    wrap.css({
                        'border': '0',
                    });
                    var view = obj.find('div.datagrid-view');
                    view.css({
                        'height': data.rows.length * 36,
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

            $("#PaymentType").combobox({
                data: PaymentType,
                onSelect: function (record) {
                    if (record.Value == '<%=Needs.Ccs.Services.Enums.WhsePaymentType.Cash.GetHashCode()%>') {
                        $("#Currency").combobox('setValue', "HKD");
                    }
                    else {
                        $("#Currency").combobox('setValue', "CNY");
                    }
                }
            })
            $("#FeeType").combobox({
                data: WarehousePremiumType,
            })

            //绑定日志信息
            var data = new FormData($('#form2')[0]);
            data.append("FeeID", FeeID);
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

            Init();
        });

        function Init() {
            if (FeeData != null && FeeData != "") {
                $("#OrderID").textbox('setValue', FeeData.OrderID);
                $("#ClientName").textbox('setValue', FeeData.ClientName);
                $("#Type").textbox('setValue', FeeData.Type);
                $("#Creater").textbox('setValue', FeeData.Creater);
                $("#Price").textbox('setValue', FeeData.Price);
                $("#CreateDate").textbox('setValue', FeeData.CreateDate);
                $("#ExchangeRate").textbox('setValue', FeeData.ExchangeRate);
                $("#ApprovalPrice").textbox('setValue', FeeData.ApprovalPrice);
                $("#Summary").textbox('setValue', FeeData.Summary);
            }
        }

        //关闭
        function Close() {
            $.myWindow.close();
        }

        //预览文件
        function View(url) {
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

        //文件图片
        function ShowImg(val, row, index) {
            return '<img src="../../../App_Themes/xp/images/wenjian.png" />';
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

        //操作
        function Operation(val, row, index) {
            var buttons = row.Name + '<br/>';
            buttons += '<a href="#"><span style="color: cornflowerblue;" onclick="View(\'' + row.WebUrl + '\')">预览</span></a>';
            return buttons;
        }
    </script>
</head>
<body class="easyui-layout">
    <div data-options="region:'west',border:false,collapsible:false,split:true" style="width: 50%; min-width: 350px;">
        <form id="form1" runat="server" method="post">
            <div title="" class="easyui-panel" data-options="height:'auto',border:false">
                <div class="sub-container">
                    <table id="table1" class="row-info" cellspacing="0" cellpadding="0">
                        <tr>
                            <td class="lbl">订单编号：</td>
                            <td>
                                <input class="easyui-textbox" id="OrderID" data-options="height:26,width:250" disabled="disabled" />
                            </td>
                        </tr>
                        <tr>
                            <td class="lbl">客户名称：</td>
                            <td>
                                <input class="easyui-textbox" id="ClientName" data-options="height:26,width:250" disabled="disabled" />
                            </td>
                        </tr>
                        <tr>
                            <td class="lbl">费用名称：</td>
                            <td>
                                <input class="easyui-textbox" id="Type" data-options="height:26,width:250" disabled="disabled" />
                            </td>
                        </tr>
                        <tr>
                            <td class="lbl">录入人：</td>
                            <td>
                                <input class="easyui-textbox" id="Creater" data-options="height:26,width:250" disabled="disabled" />
                            </td>
                        </tr>
                        <tr>
                            <td class="lbl">金额：</td>
                            <td>
                                <input class="easyui-textbox" id="Price" data-options="height:26,width:250" disabled="disabled" />
                            </td>
                        </tr>
                        <tr>
                            <td class="lbl">录入时间：</td>
                            <td>
                                <input class="easyui-textbox" id="CreateDate" data-options="height:26,width:250" disabled="disabled" />
                            </td>
                        </tr>
                        <tr>
                            <td class="lbl">实时汇率：</td>
                            <td>
                                <input class="easyui-textbox" id="ExchangeRate" data-options="height:26,width:250" disabled="disabled" />
                            </td>
                        </tr>
                        <tr>
                            <td class="lbl">审批金额(RMB)：</td>
                            <td>
                                <input class="easyui-textbox" id="ApprovalPrice" data-options="required:true,height:26,width:250,validType:'length[1,18]'" disabled="disabled" />
                            </td>
                        </tr>
                        <tr>
                            <td class="lbl">备注信息：</td>
                            <td>
                                <input class="easyui-textbox" id="Summary" data-options="multiline:true,validType:'length[1,500]'" disabled="disabled" style="height: 50px; width: 100%" />
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </form>
    </div>
    <div data-options="region:'center',border:false,collapsible:false,split:true" style="width: 59%;">
        <form id="form2">
            <div id="fileContainer" title="" class="easyui-panel" data-options="iconCls:'icon-blue-fujian', height:'auto',border:false">
                <div class="sub-container">
                    <div id="unUpload" style="margin-left: 5px">
                        <p>未上传</p>
                    </div>
                    <div class="pi">
                        <table id="files">
                            <%-- <thead>
                                <tr>
                                    <th data-options="field:'img',formatter:ShowImg">图片</th>
                                    <th style="width: auto" data-options="field:'Btn',align:'left',formatter:Operation">操作</th>
                                </tr>
                            </thead>--%>
                        </table>
                    </div>
                    <div class="text-container" style="margin-top: 10px;">
                        <p>仅限图片、pdf格式的文件，且pdf文件不超过3M。</p>
                    </div>
                </div>
            </div>
        </form>
    </div>
    <div data-options="region:'south',border:false,collapsible:false,split:true">
        <div class="easyui-layout" data-options="fit:true">
            <%--  <div data-options="region:'center',border:false,collapsible:false,split:true" style="min-height: 150px" title="日志记录">
                <div class="sub-container">
                    <div class="text-container" id="LogContent">
                    </div>
                </div>
            </div>--%>
            <div id="dlg-buttons" data-options="region:'south',border:false">
                <a id="btnBack" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="Close()">取消</a>
            </div>
        </div>
    </div>
    <div id="viewFileDialog" class="easyui-window" title="查看附件" data-options="iconCls:'pag-list',modal:true,collapsible:false,minimizable:false,maximizable:true,resizable:true,closed:true" style="width: 600px; height: 450px;">
        <img id="viewfileImg" src="" style="width: auto; height: auto; max-width: 100%; max-height: 100%;" />
        <iframe id="viewfilePdf" src="" width="100%" height="100%" frameborder="0" scroll="no"></iframe>
    </div>
</body>
</html>
