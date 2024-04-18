<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InvoiceNoticeFile.aspx.cs" Inherits="WebApp.Finance.Invoice.InvoiceNoticeFile" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <script type="text/javascript">

        var InvoiceNoticeID = '(<%=this.Model.InvoiceNoticeID%>)';

        $(function () {
            //发票附件列表初始化
            $('#InvoiceFileTable').myDatagrid({
                //queryParams:{ InvoiceNoticeID: InvoiceNoticeID, },
                actionName: 'InvoiceFiles',
                border: false,
                showHeader: false,
                nowrap: false,
                pagination: false,
                rownumbers: false,
                fitcolumns: true,
                rowStyler: function (index, row) {
                    return 'background-color:white;';
                },
                loadFilter: function (data) {
                    $('#invoiceList').text('发票(' + data.total + ')');
                    if (data.total == 0) {
                        $('#unUpload').css('display', 'block');
                        $('#fileContainer').children(":first").css("overflow","hidden");
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

                    
                    var heightValue = $("#InvoiceFileTable").prev().find(".datagrid-body").find(".datagrid-btable").height() + 20;
                    $("#InvoiceFileTable").prev().find(".datagrid-body").height(heightValue);
                    $("#InvoiceFileTable").prev().height(heightValue);
                    $("#InvoiceFileTable").prev().parent().height(heightValue);
                    $("#InvoiceFileTable").prev().parent().parent().height(heightValue);

                    $("#InvoiceFileTable").prev().find(".datagrid-header").height(0);

                    //调整“其它信息”和“发票附件”的高度
                    var panel3Height = $('#panel3').height();
                    var fileContainerHeight = $('#fileContainer').height();

                    var targetHeight = 0;
                    if (panel3Height >= fileContainerHeight) {
                        targetHeight = panel3Height;
                    } else {
                        targetHeight = fileContainerHeight;
                    }
                    targetHeight = targetHeight + 40;

                    $('#panel3').panel('resize',{
	                    height: targetHeight,
                    });

                    $('#fileContainer').panel('resize',{
	                    height: targetHeight,
                    });

                    $("#InvoiceFileTable").prev().find(".datagrid-header").height(0);

                    $("#panel3").children(":first").height($("#panel3").children(":first").height() - 35);
                    $("#fileContainer").children(":first").height($("#fileContainer").children(":first").height() - 35);


                    $("#InvoiceFileTable").prev().find(".datagrid-body").find("tr").width(400).css("display", "inline-block").css("margin-top", "8px");

                }
            });
        });

        //查看文件
        function View(url) {
            var from = getQueryString('From');
            $('#viewfileImg').css('display', 'none');
            $('#viewfilePdf').css('display', 'none');

            if (url.toLowerCase().indexOf('pdf') > 0) {
                $('#viewfilePdf').attr('src', url);
                $('#viewfilePdf').css("display", "block");
                if (from.indexOf('Query') == -1) {
                    $('#viewFileDialog').window('open').window('center');
                } else {
                    $('#viewFileDialog').window('open').window('center').window("resize", { top: 200 });
                }
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
                if (from.indexOf('Query') == -1) {
                    $('#viewFileDialog').window('open').window('center');
                } else {
                    $('#viewFileDialog').window('open').window('center').window("resize", { top: 200 });
                }
            }

            $("#unUpload").next().find(".datagrid-wrap").find(".datagrid-view").find(".datagrid-view2").find(".datagrid-body").find(".datagrid-btable")
                .find("td[field='Btn']").css({ "color": "#000000" });
        }

        //文件图片
        function ShowImg(val, row, index) {
            return '<img src="../../App_Themes/xp/images/wenjian.png" />';
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = row.Name + '<br/>';
            buttons += '<a href="#"><span style="color: cornflowerblue;" onclick="View(\'' + row.Url + '\')">预览</span></a>';
            return buttons;
        }
    </script>
</head>
<body class="easyui-layout">
    <div>
        <form id="form1" runat="server">
            <div data-options="region:'center',border: false," style="float: left; margin-top: 0px; margin-left: 4px;">
                <div class="sec-container">
                    <div id="fileContainer" class="easyui-panel" title="发票附件" data-options="border: false, noheader: true,">
                        <div class="sub-container" style="overflow:auto; width: 824px;">
                            <div style="margin-bottom: 5px">
                                <span>
                                    <img src="../../App_Themes/xp/images/blue-fujian.png" /></span>
                                <span id="invoiceList" style="font-weight: bold">发票</span>
                            </div>
                            <p id="unUpload" style="display: none">未上传</p>
                            <table id="InvoiceFileTable">
                                <thead>
                                    <tr>
                                        <th data-options="field:'img',formatter:ShowImg">图片</th>
                                        <th style="width: auto" data-options="field:'Btn',align:'left',formatter:Operation">操作</th>
                                    </tr>
                                </thead>
                            </table>
                        </div>
                    </div>
                    <div id="viewFileDialog" class="easyui-window" title="查看附件" data-options="iconCls:'pag-list',modal:true,collapsible:false,minimizable:false,maximizable:true,resizable:true,closed:true" style="width: 800px; height: 400px;">
                        <img id="viewfileImg" src="" style="width: auto; height: auto; max-width: 100%; max-height: 100%;" />
                        <iframe id="viewfilePdf" src="" width="100%" height="100%" frameborder="0" scroll="no"></iframe>
                    </div>
                </div>
            </div>
        </form>
    </div>
</body>
</html>
