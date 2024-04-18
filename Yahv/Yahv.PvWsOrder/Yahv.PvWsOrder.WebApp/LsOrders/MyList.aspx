<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="MyList.aspx.cs" Inherits="Yahv.PvOms.WebApp.LsOrders.MyList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            $("#Status").combobox({
                editable: false,
                valueField: 'Value',
                textField: 'Text',
                data: model.StatusData,
            });
            $("#InvoiceStatus").combobox({
                editable: false,
                valueField: 'Value',
                textField: 'Text',
                data: model.InvoiceData,
            });
            // 搜索按钮
            $('#btnSearch').click(function () {
                grid.myDatagrid('search', getQuery());
                return false;
            });
            // 清空按钮
            $('#btnClear').click(function () {
                $('#OrderID').textbox("setText", "");
                $('#StartDate').datebox("setText", "");
                $('#EndDate').datebox("setText", "");
                $('#Status').combobox("setValue", "");
                $('#InvoiceStatus').combobox("setValue", "");
                grid.myDatagrid('search', getQuery());
                return false;
            });
            //页面初始化
            window.grid = $("#tab1").myDatagrid({
                toolbar: '#topper',
                pagination: true,
                singleSelect: true,
                fitColumns: true,
                scrollbarSize: 0,
                onCheck: function (rowIndex, rowData) {
                    //上传合同逻辑
                    if (rowData.Status == '待支付') {
                        $('#uploadContract').filebox('enable');
                        $('#btnExportExcel').linkbutton('disable');
                        $('#btnExportXml').linkbutton('disable');
                    }
                    else {
                        var id = rowData.ID;
                        ajaxLoading();
                        $.post('?action=IsContracted', { ID: id }, function (result) {
                            ajaxLoadEnd();
                            var res = JSON.parse(result);
                            if (res) {
                                $('#uploadContract').filebox('enable');
                            }
                            else {
                                $('#uploadContract').textbox('disable');
                            }
                        })
                        //申请开票逻辑  已收款订单 + isInvoiced为false
                        if (!rowData.IsInvoiced) {
                            $('#btnExportExcel').linkbutton('enable');
                            $('#btnExportXml').linkbutton('enable');
                        }
                    }
                },
            });
            //上传合同  
            $('#uploadContract').filebox({
                multiple: false,
                validType: ['fileSize[10,"MB"]'],
                buttonText: '上传合同',
                buttonIcon: 'icon-yg-add',
                width: 82,
                height: 22,
                accept: ['image/jpg', 'image/bmp', 'image/jpeg', 'image/gif', 'image/png', 'application/pdf'],
                onClickButton: function () {
                    //防止重复上传相同名称的文件时不读取数据
                    $('#uploadContract').textbox('setValue', '');
                },
                onChange: function (e) {
                    if ($('#uploadContract').filebox('getValue') == '') {
                        return;
                    }
                    var row = $("#tab1").datagrid("getChecked");
                    if (row.length == 0) {
                        $.messager.alert('提示', "请勾选订单项。")
                        return;
                    }
                    var formData = new FormData($('#form1')[0]);
                    //订单ID
                    formData.append("LsOrderID", row[0].ID);
                    var files = $("input[name='uploadContract']").get(0).files;
                    for (var i = 0; i < files.length; i++) {
                        //文件信息
                        var file = files[i];
                        var fileType = file.type;
                        var fileSize = file.size / 1024;
                        var imgArr = ["image/jpg", "image/bmp", "image/jpeg", "image/gif", "image/png"];
                        if (imgArr.indexOf(fileType) > -1 && fileSize > 500) { //大于500kb的图片压缩
                            photoCompress(file, { quality: 1 }, function (base64Codes, fileName) {
                                var bl = convertBase64UrlToBlob(base64Codes);
                                //文件对象
                                formData.set('uploadContract', bl, fileName);
                                //上传文件
                                uploadContract(formData);
                            });
                        } else if (imgArr.indexOf(file.type) <= -1 && fileSize > 3072) { //非图片文件限制3M
                            $.messager.alert('提示', '上传的pdf文件大小不能超过3M!');
                            continue;
                        } else {
                            formData.set('uploadContract', file);
                            //上传文件
                            uploadContract(formData);
                        }
                    }
                    $('#tab1').datagrid('reload');
                }
            })
            //上传发票  
            $('#btnUploadInvoice').filebox({
                multiple: false,
                validType: ['fileSize[10,"MB"]'],
                buttonText: '上传发票',
                buttonIcon: 'icon-yg-add',
                width: 82,
                height: 22,
                accept: ['image/jpg', 'image/bmp', 'image/jpeg', 'image/gif', 'image/png', 'application/pdf'],
                onClickButton: function () {
                    //防止重复上传相同名称的文件时不读取数据
                    $('#btnUploadInvoice ').textbox('setValue', '');
                },
                onChange: function (e) {
                    if ($('#btnUploadInvoice ').filebox('getValue') == '') {
                        return;
                    }
                    var row = $("#tab1").datagrid("getChecked");
                    if (row.length == 0) {
                        $.messager.alert('提示', "请勾选订单项。")
                        return;
                    }
                    var formData = new FormData($('#form1')[0]);
                    //订单ID
                    formData.append("LsOrderID", row[0].ID);
                    var files = $("input[name='uploadInvoice']").get(0).files;
                    for (var i = 0; i < files.length; i++) {
                        //文件信息
                        var file = files[i];
                        var fileType = file.type;
                        var fileSize = file.size / 1024;
                        var imgArr = ["image/jpg", "image/bmp", "image/jpeg", "image/gif", "image/png"];
                        if (imgArr.indexOf(fileType) > -1 && fileSize > 500) { //大于500kb的图片压缩
                            photoCompress(file, { quality: 1 }, function (base64Codes, fileName) {
                                var bl = convertBase64UrlToBlob(base64Codes);
                                //文件对象
                                formData.set('uploadInvoice ', bl, fileName);
                                //上传文件
                                uploadInvoice(formData);
                            });
                        } else if (imgArr.indexOf(file.type) <= -1 && fileSize > 3072) { //非图片文件限制3M
                            $.messager.alert('提示', '上传的pdf文件大小不能超过3M!');
                            continue;
                        } else {
                            formData.set('uploadInvoice ', file);
                            //上传文件
                            uploadInvoice(formData);

                        }
                    }

                }
            })
        });
    </script>
    <script>
        var getQuery = function () {
            var params = {
                action: 'data',
                OrderID: $.trim($('#OrderID').textbox("getText")),
                StartDate: $.trim($('#StartDate').datebox("getText")),
                EndDate: $.trim($('#EndDate').datebox("getText")),
                Status: $.trim($('#Status').combobox("getValue")),
                InvoiceStatus: $.trim($('#InvoiceStatus').combobox("getValue")),
            };
            return params;
        };
        //导出合同
        function ExportContract() {
            var row = $("#tab1").datagrid("getChecked");
            if (row.length == 0) {
                $.messager.alert('提示', "请勾选订单项。")
                return;
            }
            var formData = new FormData($('#form1')[0]);
            //订单ID
            formData.append("LsOrderID", row[0].ID);
            $.ajax({
                url: '?action=ExportContract',
                type: 'POST',
                data: formData,
                dataType: 'JSON',
                cache: false,
                processData: false,
                contentType: false,
                success: function (res) {
                    $.messager.alert('消息', res.message, 'info', function () {
                        if (res.success) {
                            //下载文件
                            try {
                                let a = document.createElement('a');
                                a.href = res.fileurl;
                                a.download = "";
                                a.click();
                            } catch (e) {
                                console.log(e);
                            }
                        }
                    });
                }
            })
        }
        //上传合同
        function uploadContract(formData) {
            $.ajax({
                url: '?action=UploadContract',
                type: 'POST',
                data: formData,
                dataType: 'JSON',
                cache: false,
                processData: false,
                contentType: false,
                success: function (res) {
                    $.messager.alert('提示', res.message);
                }
            })
        }
        //申请开票(Excel)
        function ExportExcel() {
            var row = $("#tab1").datagrid("getChecked");
            if (row.length == 0) {
                $.messager.alert('提示', "请勾选订单项。")
                return;
            }
            var formData = new FormData($('#form1')[0]);
            //订单ID
            formData.append("LsOrderID", row[0].ID);
            $.ajax({
                url: '?action=ExportExcel',
                type: 'POST',
                data: formData,
                dataType: 'JSON',
                cache: false,
                processData: false,
                contentType: false,
                success: function (res) {
                    $.messager.alert('消息', res.message, 'info', function () {
                        if (res.success) {
                            //下载文件
                            try {
                                let a = document.createElement('a');
                                a.href = res.fileUrl;
                                a.download = "";
                                a.click();
                            } catch (e) {
                                console.log(e);
                            }
                        }
                    });
                }
            })
        }

        //申请开票（xml）
        function ExportXml() {
            var row = $("#tab1").datagrid("getChecked");
            if (row.length == 0) {
                $.messager.alert('提示', "请勾选订单项。")
                return;
            }
            //验证成功
            $.post('?action=ExportXml', {
                LsOrderID: row[0].ID,
            }, function (result) {
                var rel = JSON.parse(result);
                $.messager.alert('消息', rel.message, 'info', function () {
                    if (rel.success) {
                        //下载文件
                        try {
                            let a = document.createElement('a');
                            a.href = rel.fileUrl;
                            a.download = "";
                            a.click();
                        } catch (e) {
                            console.log(e);
                        }
                        $('#tab1').bvgrid('reload');
                    }
                });
            })
        }

        //上传发票
        function uploadInvoice(formData) {
            $.ajax({
                url: '?action=UploadInvoice',
                type: 'POST',
                data: formData,
                dataType: 'JSON',
                cache: false,
                processData: false,
                contentType: false,
                success: function (res) {
                    $.messager.alert('提示', res.message);
                }
            }).done(function (res) {

            });
        }
    </script>
    <script>
        function Edit(id) {
            $.myWindow({
                title: "编辑",
                url: location.pathname.replace('MyList.aspx', 'Edit.aspx?ID=' + id),
                minWidth: 1200,
                minHeight: 600,
                onClose: function () {
                    window.grid.myDatagrid('flush');
                },
            });
            return false;
        }
        function Delete(id) {
            $.messager.confirm('确认', '请您确认是否删除所选项。', function (success) {
                if (success) {
                    $.post('?action=Delete', { id: id }, function (result) {
                        var res = JSON.parse(result);
                        if (res.success) {
                            top.$.timeouts.alert({ position: "TC", msg: res.message, type: "success" });
                        }
                        else {
                            top.$.timeouts.alert({ position: "TC", msg: res.message, type: "error" });
                        }
                        $('#tab1').datagrid('reload');
                    })
                }
            });
        }
        function ReNew(id, clientId) {
            var clientId = getQueryString('ID');
            $.myWindow({
                title: "续租申请",
                url: location.pathname.replace('MyList.aspx', 'ReNew.aspx?ID=' + id + '&ClientID=' + clientId),
                minWidth: 1200,
                minHeight: 600,
                onClose: function () {
                    window.grid.myDatagrid('flush');
                },
            });
            return false;
        }
        function Detail(id) {
            $.myWindow({
                title: "租赁申请详情",
                url: location.pathname.replace('MyList.aspx', 'Details.aspx?ID=' + id),
                minWidth: 1200,
                minHeight: 600,
            });
            return false;
        }
        //收款确认
        function Received(index) {
            var data = $("#tab1").myDatagrid('getRows')[index];
            $.myWindow({
                title: '收款确认',
                url: '/Pays/pays/ReceiptNotice/Tabs.aspx?id=' + data.ID,
                minWidth: 1200,
                minHeight: 600,
                onClose: function () {
                    window.grid.myDatagrid('flush');
                },
            });
            return false;
        }
        function Operation(val, row, index) {
            var buttons = [];
            if (row.Status == "待支付") {
                buttons.push('<span class="easyui-formatted">');
                buttons.push('<a class="easyui-linkbutton" data-options="iconCls:\'icon-yg-quote\'" onclick="Received(' + index + ');return false;">收款确认</a> ')
                buttons.push('</span>')
                buttons.push('<span class="easyui-formatted">');
                buttons.push('<a class="easyui-linkbutton" data-options="iconCls:\'icon-yg-edit\'" onclick="Edit(\'' + row.ID + '\');return false;">编辑</a> ')
                buttons.push('</span>')
                buttons.push('<span class="easyui-formatted">');
                buttons.push('<a class="easyui-linkbutton" data-options="iconCls:\'icon-yg-delete\'" onclick="Delete(\'' + row.ID + '\');return false;">删除</a> ')
                buttons.push('</span>')
            }
            else {
                if (!row.InheritStatus) {
                    buttons.push('<span class="easyui-formatted">');
                    buttons.push('<a class="easyui-linkbutton" data-options="iconCls:\'icon-yg-add\'" onclick="ReNew(\'' + row.ID + '\');return false;">续租</a> ')
                    buttons.push('</span>')
                }
                buttons.push('<span class="easyui-formatted">');
                buttons.push('<a class="easyui-linkbutton" data-options="iconCls:\'icon-yg-details\'" onclick="Detail(\'' + row.ID + '\');return false;">详情</a> ')
                buttons.push('</span>')
            }
            return buttons.join('');
        };
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div id="topper">
        <!--搜索按钮-->
        <table class="liebiao">
            <tr>
                <td style="width: 90px;">申请日期</td>
                <td>
                    <input id="StartDate" data-options="prompt:''" class="easyui-datebox" style="width: 108px" />
                    &nbsp&nbsp<span>至</span>&nbsp&nbsp
                    <input id="EndDate" data-options="prompt:''" class="easyui-datebox" style="width: 108px" />
                </td>
                <td style="width: 90px;">订单编号:</td>
                <td>
                    <input id="OrderID" class="easyui-textbox" style="width: 250px" />
                </td>
            </tr>
            <tr>
                <td style="width: 90px;">订单状态:</td>
                <td>
                    <input id="Status" class="easyui-combobox" style="width: 250px" />
                </td>
                <td style="width: 90px;">开票状态:</td>
                <td>
                    <input id="InvoiceStatus" class="easyui-combobox" style="width: 250px" />
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <a id="btnSearch" class="easyui-linkbutton" iconcls="icon-yg-search">搜索</a>
                    <a id="btnClear" class="easyui-linkbutton" iconcls="icon-yg-clear">清空</a>
                    <a id="btnExportContract" class="easyui-linkbutton" iconcls="icon-yg-excelImport" onclick="ExportContract()">导出合同</a>
                    <a id="uploadContract" name="uploadContract" class="easyui-filebox">上传合同</a>
                    <a id="btnExportExcel" class="easyui-linkbutton" iconcls="icon-man" onclick="ExportExcel()">申请开票(Excel)</a>
                    <a id="btnExportXml" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-man'" onclick="ExportXml()">申请开票(Xml)</a>
                    <a id="btnUploadInvoice" name="uploadInvoice" class="easyui-filebox">上传发票</a>
                </td>
            </tr>
        </table>
    </div>
    <table id="tab1">
        <thead>
            <tr>
                <th data-options="field:'ck',checkbox:true"></th>
                <th data-options="field:'CreateDate',align:'center'" style="width: 60px;">申请日期</th>
                <th data-options="field:'ID',align:'center'" style="width: 80px">订单编号</th>
                <th data-options="field:'CompanyName',align:'left'" style="width: 120px;">客户名称</th>
                <th data-options="field:'EnterCode',align:'center'" style="width: 50px">客户入仓号</th>
                <th data-options="field:'Status',align:'center'" style="width: 50px">状态</th>
                <th data-options="field:'InvoiceStatus',align:'center'" style="width: 50px;">开票状态</th>
                <th data-options="field:'InheritStatus',align:'center'" style="width: 50px;">是否续租</th>
                <th data-options="field:'Creator',align:'center'" style="width: 50px;">申请人</th>
                <th data-options="field:'Btn',align:'left',formatter:Operation" style="width: 120px;">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>
