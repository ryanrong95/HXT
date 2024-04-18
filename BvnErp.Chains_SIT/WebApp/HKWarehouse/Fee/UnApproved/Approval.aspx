<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Approval.aspx.cs" Inherits="WebApp.HKWarehouse.Fee.UnApproved.Approval" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <link href="../../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../../Scripts/Ccs.js"></script>
    <script type="text/javascript">

        var OrderWhesPremiumData = eval('(<%=this.Model.OrderWhesPremiumData%>)');
        var FeeID = getQueryString('FeeID');

        $(function () {
            //文件列表初始化
            $('#files').myDatagrid({
               actionName:'filedata',
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


                    var heightValue = $("#files").prev().find(".datagrid-body").find(".datagrid-btable").height() + 30;
                    $("#files").prev().find(".datagrid-body").height(heightValue);
                    $("#files").prev().height(heightValue);
                    $("#files").prev().parent().height(heightValue);
                    $("#files").prev().parent().parent().height(heightValue);


                    $("#files").prev().parent().parent().height(heightValue + 35);
                }
            });
            //注册文件上传的onChange事件
            $('#uploadFile').filebox({
                multiple: true,
                validType: ['fileSize[500,"KB"]'],
                buttonText: '上传',
                buttonAlign: 'right',
                buttonIcon: 'icon-add',
                prompt: '请选择图片或PDF类型的文件',
                accept: ['image/jpg', 'image/bmp', 'image/jpeg', 'image/gif', 'image/png', 'application/pdf'],
                onClickButton: function () {
                    $(this).filebox('setValue', '');
                },
                onChange: function (val) {
                    if (val == '') {
                        return;
                    }

                    var $this = $(this);
                    //验证文件大小
                    if ($this.next().attr("class").indexOf("textbox-invalid") > 0) {
                        $.messager.alert('提示', '文件大小不能超过500kb！');
                        return;
                    }
                    //验证文件类型
                    var type = $this.filebox('options').accept.join();
                    type = type.replace(new RegExp("image/", "g"), "").replace(new RegExp("application/", "g"), "")
                    var ext = val.substr(val.lastIndexOf(".") + 1);
                    if (type.indexOf(ext.toLowerCase()) < 0) {
                        $this.filebox('setValue', '');
                        $.messager.alert('提示', "请选择" + type + "格式的文件！");
                        return;
                    }

                    var formData = new FormData($('#form2')[0]);
                    $.ajax({
                        url: '?action=UploadFile',
                        type: 'POST',
                        data: formData,
                        dataType: 'JSON',
                        cache: false,
                        processData: false,
                        contentType: false,
                        success: function (res) {
                            if (res.success) {
                                var data = res.data;
                                for (var i = 0; i < data.length; i++) {
                                    $('#files').datagrid('appendRow', {
                                        Name: data[i].Name,
                                        FileFormat: data[i].FileFormat,
                                        WebUrl: data[i].WebUrl,
                                        Url: data[i].Url,
                                    });
                                }
                                var data = $('#files').datagrid('getData');
                                $('#files').datagrid('loadData', data);


                                var heightValue = $("#files").prev().find(".datagrid-body").find(".datagrid-btable").height() + 30;
                                $("#files").prev().find(".datagrid-body").height(heightValue);
                                $("#files").prev().height(heightValue);
                                $("#files").prev().parent().height(heightValue);
                                $("#files").prev().parent().parent().height(heightValue);


                                $("#files").prev().parent().parent().height(heightValue + 35);
                            } else {
                                $.messager.alert('提示', res.message);
                            }
                        }
                    }).done(function (res) {

                    });
                }
            });

            ////绑定日志信息
            //var data = new FormData($('#form2')[0]);
            //data.append("FeeID", FeeID);
            //$.ajax({
            //    url: '?action=LoadLogs',
            //    type: 'POST',
            //    data: data,
            //    dataType: 'JSON',
            //    cache: false,
            //    processData: false,
            //    contentType: false,
            //    success: function (data) {
            //        showLogContent(data);
            //    },
            //    error: function (msg) {
            //        alert("ajax连接异常：" + msg);
            //    }
            //});

            Init();
        });

        function Init() {
            if (OrderWhesPremiumData != null && OrderWhesPremiumData != "") {
                $("#OrderID").textbox('setValue', OrderWhesPremiumData.OrderID);
                $("#ClientName").textbox('setValue', OrderWhesPremiumData.ClientName);
                $("#Type").textbox('setValue', OrderWhesPremiumData.Type);
                $("#Creater").textbox('setValue', OrderWhesPremiumData.Creater);
                $("#Price").textbox('setValue', OrderWhesPremiumData.Price);
                $("#CreateDate").textbox('setValue', OrderWhesPremiumData.CreateDate);
                $("#ExchangeRate").textbox('setValue', OrderWhesPremiumData.ExchangeRate);
                $("#ApprovalPrice").textbox('setValue', OrderWhesPremiumData.ApprovalPrice);
                $("#Summary").textbox('setValue', OrderWhesPremiumData.Summary);
            }
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

        //删除附件
        function Delete(index) {
            $('#files').datagrid('deleteRow', index);
            var data = $('#files').datagrid('getData');
            $('#files').datagrid('loadData', data);
        }

        //关闭弹出页面
        function Close() {
            $.myWindow.close();
           // self.parent.$('iframe').parent().window('close');
        }

        //保存校验
        function Approval() {
            //验证表单数据
            if (!Valid('form1')) {
                return;
            }

            var data = new FormData($('#form1')[0]);
            var ApprovalPrice = $("#ApprovalPrice").val();
            var Summary = $("#Summary").val();
            var FileData = $("#files").datagrid("getRows");

            $.messager.confirm('确认', '费用是否审批通过！', function (success) {
                if (success) {
                    $.post('?action=ApprovalFee', {
                        ID: FeeID,
                        ApprovalPrice: ApprovalPrice,
                        Summary: Summary,
                        FileData: JSON.stringify(FileData),
                    }, function (result) {
                        var rel = JSON.parse(result);
                        $.messager.alert('消息', rel.message, 'info', function () {
                            if (rel.success) {
                                Close();
                            }
                        })
                    })
                }
            })
        }

        //取消费用
        function Cancel() {
            $.messager.confirm('确认', '费用是否取消！', function (success) {
                if (success) {
                    $.post('?action=CancelFee', {
                        ID: FeeID,
                    }, function (result) {
                        var rel = JSON.parse(result);
                        $.messager.alert('消息', rel.message, 'info', function () {
                            if (rel.success) {
                                Close();
                            }
                        })
                    })
                }
            })
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
            buttons += '<a href="#"><span style="color: cornflowerblue; margin-left: 10px;" onclick="Delete(' + index + ')">删除</span></a>';
            return buttons;
        }
    </script>
</head>

<%--<table style="line-height: 40px">
                    <tr>
                        <td class="lbl">*审批通过，审批金额会写入到订单对账单，向客户收取</td>
                    </tr>
                    <tr>
                        <td class="lbl">*取消费用，不计入订单，也不向客户收取</td>
                    </tr>
                </table>--%>
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
                                <input class="easyui-textbox" id="ApprovalPrice" data-options="required:true,height:26,width:250,validType:'length[1,18]'" />
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
                    <div>
                        <table id="files" data-options="nowrap:false,queryParams:{ action: 'filedata' }">
                            <thead>
                                <tr>
                                    <th data-options="field:'img',formatter:ShowImg">图片</th>
                                    <th style="width: auto" data-options="field:'Btn',align:'left',formatter:Operation">操作</th>
                                </tr>
                            </thead>
                        </table>
                    </div>

                    <div style="margin-top: 5px; margin-left: 5px;">
                        <input id="uploadFile" name="uploadFile" class="easyui-filebox" style="width: 54px; height: 26px" />
                    </div>
                    <div class="text-container" style="margin-top: 10px;">
                        <p>仅限图片或pdf格式的文件,并且不超过500kb</p>
                    </div>
                </div>
            </div>
        </form>
    </div>
    <div data-options="region:'south',border:false,collapsible:false,split:true">
        <div class="easyui-layout" data-options="fit:true">
            <%--<div data-options="region:'center',border:false,collapsible:false,split:true" style="min-height: 150px" title="日志记录">
                <div class="sub-container">
                    <div class="text-container" id="LogContent">
                    </div>
                </div>
            </div>--%>
            <div id="dlg-buttons" data-options="region:'south',border:false">
                <a class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="Approval()">审批通过</a>
                <a class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="Cancel()">取消费用</a>
                <a class="easyui-linkbutton" data-options="iconCls:'icon-back'" onclick="Close()">返回</a>
            </div>
        </div>
    </div>
    <div id="viewFileDialog" class="easyui-window" title="查看附件" data-options="iconCls:'pag-list',modal:true,collapsible:false,minimizable:false,maximizable:true,resizable:true,closed:true" style="width: 600px; height: 450px;">
        <img id="viewfileImg" src="" style="width: auto; height: auto; max-width: 100%; max-height: 100%;" />
        <iframe id="viewfilePdf" src="" width="100%" height="100%" frameborder="0" scroll="no"></iframe>
    </div>
</body>
</html>
