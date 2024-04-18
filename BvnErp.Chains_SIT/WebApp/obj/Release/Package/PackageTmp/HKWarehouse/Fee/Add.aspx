<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Add.aspx.cs" Inherits="WebApp.HKWarehouse.Fee.Add" %>

<!DOCTYPE html>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <script src="../../Scripts/chainsupload.js"></script>
    <script type="text/javascript">

        var PaymentType = eval('(<%=this.Model.PaymentType%>)');
        var WarehousePremiumType = eval('(<%=this.Model.WarehousePremiumType%>)');

        var FeeID = getQueryString('FeeID');
        var OrderID = getQueryString('OrderID');
        var btnSaveEnable = true;

        $(function () {

            var price = $('#UnitPrice').combobox('getValue');
            var Count = $('#Count').textbox('getValue');
            $('#TotalPrice').textbox('setValue', price * Count);
            //注册上传原始单据filebox的onChange事件
            $('#uploadFile').filebox({
                multiple: true,
                //validType: ['fileSize[500,"KB"]'],
                buttonText: '选择文件',
                buttonAlign: 'right',
                prompt: '请选择图片或PDF类型的文件',
                accept: ['image/jpg', 'image/bmp', 'image/jpeg', 'image/gif', 'image/png', 'application/pdf'],
                onClickButton: function () {
                    $('#uploadFile').filebox('setValue', '');
                },
                onChange: function (e) {
                    if ($('#uploadFile').filebox('getValue') == '') {
                        return;
                    }
                    var formData = new FormData($('#form1')[0]);
                    var files = $("input[name='uploadFile']").get(0).files;
                    for (var i = 0; i < files.length; i++) {
                        //文件信息
                        var file = files[i];
                        var fileType = file.type;
                        var fileSize = file.size / 1024;
                        var imgArr = ["image/jpg", "image/bmp", "image/jpeg", "image/gif", "image/png"];

                        if (imgArr.indexOf(fileType) > -1 && fileSize > 500) { //大于500kb的图片压缩
                            photoCompress(file, { quality: 1 }, function (base64Codes, fileName) {
                                var bl = convertBase64UrlToBlob(base64Codes);
                                formData.set('uploadFile', bl, fileName); // 文件对象
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
                                            var row = $('#pi').datagrid('getRows');
                                            var data = res.data;
                                            for (var i = 0; i < data.length; i++) {
                                                $('#pi').datagrid('insertRow', {
                                                    index: row.length + i,
                                                    row: {
                                                        Name: data[i].Name,
                                                        FileType: data[i].FileType,
                                                        FileFormat: data[i].FileFormat,
                                                        WebUrl: data[i].WebUrl,
                                                        Url: data[i].Url
                                                    }
                                                });
                                            }
                                        } else {
                                            $.messager.alert('提示', res.message);
                                        }
                                    }
                                }).done(function (res) {

                                });
                            });
                        } else if (imgArr.indexOf(file.type) <= -1 && fileSize > 3072) { //非图片文件限制3M
                            $.messager.alert('提示', '上传的pdf文件大小不能超过3M!');
                            continue;
                        } else {
                            formData.set('uploadFile', file);
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
                                        var row = $('#pi').datagrid('getRows');
                                        var data = res.data;
                                        for (var i = 0; i < data.length; i++) {
                                            $('#pi').datagrid('insertRow', {
                                                index: row.length + i,
                                                row: {
                                                    Name: data[i].Name,
                                                    FileType: data[i].FileType,
                                                    FileFormat: data[i].FileFormat,
                                                    WebUrl: data[i].WebUrl,
                                                    Url: data[i].Url
                                                }
                                            });
                                        }
                                    } else {
                                        $.messager.alert('提示', res.message);
                                    }
                                }
                            }).done(function (res) {

                            });
                        }
                    }

                }
            });

            //原始PI列表初始化
            $('#pi').myDatagrid({
                fitColumns: true,
                fit: false,
                pagination: false,
                actionName: 'filedata',
                rowStyler: function (index, row) {
                    return 'background-color:white;';
                },
                columns: [[
                    { field: 'ID', title: '', width: 70, align: 'center', hidden: true, },
                    { field: 'Name', title: '', width: 70, align: 'center', hidden: true },
                    { field: 'FileFormat', title: '', width: 70, align: 'center', hidden: true },
                    { field: 'WebUrl', title: '', width: 70, align: 'center', hidden: true },
                    { field: 'Url', title: '', width: 70, align: 'center', hidden: true },
                    { field: 'Btn', title: '', width: 100, align: 'left', formatter: ShowInfo }
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

            $('#Count').textbox({
                onChange: function SetPrice() {
                    var price = $('#UnitPrice').combobox('getValue');
                    var Count = $('#Count').textbox('getValue');
                    $('#TotalPrice').textbox('setValue', price * Count);
                }
            });

            $('#UnitPrice').combobox({
                onChange: function SetPrice() {
                    var price = $('#UnitPrice').combobox('getValue');
                    var Count = $('#Count').textbox('getValue');
                    $('#TotalPrice').textbox('setValue', price * Count);
                }
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
        });

        var isRemovePIRow = false;
        function Delete(index) {
            $('#pi').datagrid('deleteRow', index);
            var data = $('#pi').datagrid('getData');
            $('#pi').datagrid('loadData', data);
        }

        function onClickPIRow(index) {
            if (isRemovePIRow) {
                $('#pi').datagrid('deleteRow', index);
                isRemovePIRow = false;
            }
        }
        //保存
        function Save() {
            if (btnSaveEnable == false) {
                $.messager.alert('提示', "不能重复点击");
                return;
            }
            var isValid = $("#form1").form("enableValidation").form("validate");
            if (!isValid) {
                return;
            }
            else {
                $('#uploadFile').filebox('setValue', '');
                var FileData = $("#pi").datagrid("getRows");

                //验证成功
                var PaymentType = $("#PaymentType").combobox('getValue');
                var Currency = $("#Currency").combobox('getValue');
                var FeeType = $("#FeeType").combobox('getValue');
                var Count = $("#Count").numberbox('getValue');
                var UnitPrice = $("#UnitPrice").combobox('getValue');
                var UnitName = $("#UnitName").combobox('getValue');
                var Summary = $("#Summary").textbox('getValue');
                btnSaveEnable = false;
                $.post('?action=Save', {
                    FeeID: FeeID,
                    OrderID: OrderID,
                    PaymentType: PaymentType,
                    Currency: Currency,
                    FeeType: FeeType,
                    Count: Count,
                    UnitPrice: UnitPrice,
                    UnitName: UnitName,
                    Summary: Summary,
                    FileData: JSON.stringify(FileData),
                }, function (result) {
                    btnSaveEnable = true;
                    var rel = JSON.parse(result);
                    $.messager.alert('消息', rel.message, 'info', function () {
                        if (rel.success) {
                            Close();
                        }
                    });
                })
            }
        }

        //关闭
        function Close() {
            $.myWindow.close();
        }

        function View(url) {
            $('#viewfileImg').css('display', 'none');
            $('#viewfilePdf').css('display', 'none');
            if (url.toLowerCase().indexOf('pdf') > 0) {
                $('#viewfilePdf').attr('src', url);
                $('#viewfilePdf').css("display", "block");
            }
            else {
                $('#viewfileImg').attr('src', url);
                $('#viewfileImg').css("display", "block");
            }
            $('#viewFileDialog').window('open').window('center');
        }

        //显示附件图片
        function ShowInfo(val, row, index) {
            return '<img src="../../App_Themes/xp/images/blue-fujian.png" />'
                + '<a href="javascript:void(0);" style="margin-left: 5px;" onclick="View(\'' + row.WebUrl + '\')">' + row.Name + '</a>'
                + '<a href="javascript:void(0);" style="margin-left: 25px; color: cornflowerblue;" onclick="Delete(' + index + ')">删除</a>';
        }


    </script>
    <style>
        #pi-fujian-table td {
            border-width: 0;
            padding: 0;
        }

            #pi-fujian-table td div {
                padding: 0;
            }
    </style>
</head>
<body class="easyui-layout">
    <div data-options="region:'center'">
        <form id="form1" runat="server" method="post">
            <div title="" class="easyui-panel" data-options="height:'auto',border:false">
                 <table id="editTable" style="margin-left: 20px">
                    <tr>
                        <td class="lbl">收款方式：</td>
                        <td>
                            <input class="easyui-combobox" id="PaymentType" name="PaymentType" style="width: 250px; height: 28px"
                                data-options="required:true,valueField:'Value',textField:'Text',limitToList:true,editable:false,tipPosition:'right',missingMessage:'请选择收款方式'" />
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl">币种：</td>
                        <td>
                            <select class="easyui-combobox" id="Currency" name="Currency" style="width: 250px; height: 26px"
                                data-options="required:true,editable:false,disabled:true">
                                <option value="HKD">港币</option>
                                <option value="CNY">人民币</option>
                            </select>
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl">费用类型：</td>
                        <td>
                            <input class="easyui-combobox" id="FeeType" name="FeeType" style="width: 250px; height: 28px"
                                data-options="required:true,valueField:'Value',textField:'Text',limitToList:true,editable:false,tipPosition:'right',missingMessage:'请选择费用类型'" />
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl">数量：</td>
                        <td>
                            <input class="easyui-numberbox" id="Count" name="Count" value="1" style="width: 250px; height: 26px"
                                data-options="validType:'length[1,9]',required:true,tipPosition:'right',missingMessage:'请输入数量'" />
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl">单价：</td>
                        <td>
                            <select class="easyui-combobox" id="UnitPrice" name="UnitPrice" style="width: 150px; height: 26px"
                                data-options="required:true,tipPosition:'right',missingMessage:'请输入单价'">
                                <option>5</option>
                                <option>20</option>
                                <option>30</option>
                                <option>40</option>
                                <option>50</option>
                                <option>100</option>
                                <option>150</option>
                                <option>200</option>
                            </select>
                            <select class="easyui-combobox" id="UnitName" name="UnitName" style="width: 95px; height: 26px"
                                data-options="required:true,editable:false">
                                <option>笔</option>
                                <option>个</option>
                                <option>单/次</option>
                                <option>立方米/周</option>
                            </select>
                        </td>
                    </tr>

                     <tr>
                        <td class="lbl">总价：</td>
                        <td>
                            <input class="easyui-numberbox" id="TotalPrice" name="TotalPrice" data-options="min:0,precision:'4',required:true,width:250" readonly="true" />
                        </td>
                    </tr>
                  
                     <tr>
                                <td class="lbl">费用附件：</td>
                                <td>
                                    <input id="uploadFile" name="uploadFile" class="easyui-filebox" style="width: 57px; height: 24px" />
                                </td>
                            </tr>
                     <tr>
                                <td></td>
                                <td>
                                    <label>仅限图片、pdf格式的文件，且pdf文件不超过3M。</label>
                                </td>
                            </tr>
                     <tr>
                                <td></td>
                                <td>
                                    <div id="viewFileDialog" class="easyui-window" title="查看附件" data-options="iconCls:'pag-list',modal:true,collapsible:false,minimizable:false,maximizable:true,resizable:true,closed:true" style="width: 600px; height: 450px;">
                                        <img id="viewfileImg" src="" style="width: auto; height: auto; max-width: 100%; max-height: 100%;" />
                                        <iframe id="viewfilePdf" src="" width="100%" height="100%" frameborder="0" scroll="no"></iframe>
                                    </div>
                                    <div id="pi-fujian-table" class="ProductTable pi">
                                        <table id="pi" data-options="pageSize:50,fitColumns:true,fit:false,pagination:false,queryParams:{ action: 'filedata' }">
                                           <%-- <thead>
                                                <tr>
                                                    <th data-options="field:'info',width: 100,align:'left',formatter:ShowInfo"">附件信息</th>
                                                </tr>
                                            </thead>--%>
                                        </table>
                                    </div>
                                </td>
                            </tr>
                      <tr>
                        <td class="lbl" style="width: 80px; font-size: 14px">备注：</td>
                        <td>
                            <input class="easyui-textbox" id="Summary" name="Summary"
                                data-options="validType:'length[1,300]',multiline:true" style="width: 250px; height: 50px" />
                        </td>
                    </tr>

                </table>
            </div>
        </form>
    </div>
    <div data-options="region:'south',border:false,collapsible:false,split:true">
        <div class="easyui-layout" data-options="fit:true">
            <div data-options="region:'center',border:false,collapsible:false,split:true" style="min-height: 100px" title="日志记录">
                <div class="sub-container">
                    <div class="text-container" id="LogContent">
                    </div>
                </div>
            </div>
            <div id="dlg-buttons" data-options="region:'south',border:false">
                <a id="btnSave" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="Save()">保存</a>
                <a id="btnBack" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="Close()">取消</a>
            </div>
        </div>
    </div>
</body>
</html>
