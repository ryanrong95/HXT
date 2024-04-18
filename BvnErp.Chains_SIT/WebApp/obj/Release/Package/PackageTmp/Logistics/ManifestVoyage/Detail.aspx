<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Detail.aspx.cs" Inherits="WebApp.Logistics.ManifestVoyage.Detail" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>运输批次详情</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <script>
        var voyage = eval('(<%=this.Model.Voyage%>)');

        $(function () {
            //初始化内单客户下拉框
            var clients = eval('(<%=this.Model.Clients%>)');
            $('#Client').combobox({
                valueField: 'ID',
                textField: 'Name',
                data: clients,
                onSelect: function () {
                    var type = $('input:radio[name="Order"]:checked').val();
                    if (type == '<%=Needs.Ccs.Services.Enums.ClientType.Internal.GetHashCode() %>') {
                        var params = {
                            action: 'data',
                            ClientType: type,
                            ClientID: $(this).combobox('getValue'),
                        };
                        $('#voyageDetails').myDatagrid('search', params);

                        params['ID'] = voyage['ID'];
                        $.post('?action=dataVoyageSubtotal', params, function (res) {
                            var result = JSON.parse(res);
                            document.getElementById('TotalPackNo').innerHTML = result.TotalPackNo;
                            document.getElementById('TotalQuantity').innerHTML = result.TotalQuantity;
                            document.getElementById('TotalGrossWt').innerHTML = result.TotalGrossWt;
                            document.getElementById('TotalAmount').innerHTML = result.TotalAmount;
                            document.getElementById('TotalItems').innerHTML = result.TotalItems;
                            document.getElementById('IcgooOrders').innerHTML = result.IcgooOrders;
                        });
                    }
                }
            });

            //注册订单类型onClick事件
            $("input[name=Order]").change(function () {
                var type = $(this).val();
                var params;
                if (type == '<%=Needs.Ccs.Services.Enums.ClientType.Internal.GetHashCode() %>') {
                    params = {

                        ClientType: type,
                        ClientID: $('#Client').combobox('getValue'),
                        action: 'data',
                    };
                } else {
                    params = {
                        action: 'data',
                        ClientType: type,
                    };
                }

                $('#voyageDetails').myDatagrid('search', params);

                params['ID'] = voyage['ID'];
                $.post('?action=dataVoyageSubtotal', params, function (res) {
                    var result = JSON.parse(res);
                    document.getElementById('TotalPackNo').innerHTML = result.TotalPackNo;
                    document.getElementById('TotalQuantity').innerHTML = result.TotalQuantity;
                    document.getElementById('TotalGrossWt').innerHTML = result.TotalGrossWt;
                    document.getElementById('TotalAmount').innerHTML = result.TotalAmount;
                    document.getElementById('TotalItems').innerHTML = result.TotalItems;
                    document.getElementById('IcgooOrders').innerHTML = result.IcgooOrders;
                });
            });

            //根据页面来源(物流管理/香港库房待出库)隐藏相应的按钮
            var from = getQueryString('From');
            switch (from) {
                case 'Logistics':
                    $('#OutStock').hide();
                    break;
                case 'HKWarehouse':
                    $('#ExportDeliveryAgentFile').hide();
                    $('#DeliveryAgentFile').next().hide();
                    $('#LiuLianDan').next().hide();
                    $('#filesContainer').hide();
                    break;
            }

            //注册上传提货委托书的onChange事件
            $('#DeliveryAgentFile').filebox({
                buttonText: '上传提货委托书',
                buttonAlign: 'right',
                buttonIcon: 'icon-add',
                prompt: '请选择图片或PDF类型的文件',
                accept: ['image/jpg', 'image/bmp', 'image/jpeg', 'image/gif', 'image/png', 'application/pdf'],
                onChange: function (e) {
                    if ($('#DeliveryAgentFile').filebox('getValue') == '') {
                        return;
                    }

                    //文件信息
                    var file = $("input[name='DeliveryAgentFile']").get(0).files[0];
                    var fileType = file.type;
                    var fileSize = file.size / 1024;
                    var imgArr = ["image/jpg", "image/bmp", "image/jpeg", "image/gif", "image/png"];
                    var typeArr = ["image/jpg", "image/bmp", "image/jpeg", "image/gif", "image/png", "application/pdf"];

                    if (typeArr.indexOf(fileType) <= -1) {
                        $.messager.alert('提示', '请选择jpg、bmp、jpeg、gif、png、pdf格式的文件！');
                        $('#DeliveryAgentFile').filebox('setValue', null);
                        return;
                    }
                    var formData = new FormData();
                    formData.append('VoyageID', voyage['ID']);
                    formData.append('FileType', '<%=Needs.Ccs.Services.Enums.FileType.DeliveryAgentFile.GetHashCode()%>');

                    if (imgArr.indexOf(fileType) > -1 && fileSize > 500) { //大于500kb的图片压缩
                        photoCompress(file, { quality: 1 }, function (base64Codes, fileName) {
                            var bl = convertBase64UrlToBlob(base64Codes);
                            formData.append('DeliveryAgentFile', bl, fileName); // 文件对象
                            ajaxSubmit(formData);
                        });
                    } else if (imgArr.indexOf(file.type) <= -1 && fileSize > 3072) { //非图片文件限制3M
                        $.messager.alert('提示', '上传的pdf文件大小不能超过3M!');
                    } else {
                        formData.append('DeliveryAgentFile', file);
                        ajaxSubmit(formData);
                    }
                }
            });

            //注册上传六联单的onChange事件
            $('#LiuLianDan').filebox({
                buttonText: '上传六联单',
                buttonAlign: 'right',
                buttonIcon: 'icon-add',
                prompt: '请选择图片或PDF类型的文件',
                accept: ['image/jpg', 'image/bmp', 'image/jpeg', 'image/gif', 'image/png', 'application/pdf'],
                onChange: function (e) {
                    if ($('#LiuLianDan').filebox('getValue') == '') {
                        return;
                    }

                    //文件信息
                    var file = $("input[name='LiuLianDan']").get(0).files[0];
                    var fileType = file.type;
                    var fileSize = file.size / 1024;
                    var imgArr = ["image/jpg", "image/bmp", "image/jpeg", "image/gif", "image/png"];
                    var typeArr = ["image/jpg", "image/bmp", "image/jpeg", "image/gif", "image/png", "application/pdf"];

                    if (typeArr.indexOf(fileType) <= -1) {
                        $.messager.alert('提示', '请选择jpg、bmp、jpeg、gif、png、pdf格式的文件！');
                        $('#LiuLianDan').filebox('setValue', null);
                        return;
                    }

                    var formData = new FormData();
                    formData.append('VoyageID', voyage['ID']);
                    formData.append('FileType', '<%=Needs.Ccs.Services.Enums.FileType.LiuLianDan.GetHashCode()%>');

                    if (imgArr.indexOf(fileType) > -1 && fileSize > 500) { //大于500kb的图片压缩
                        photoCompress(file, { quality: 1 }, function (base64Codes, fileName) {
                            var bl = convertBase64UrlToBlob(base64Codes);
                            formData.append('LiuLianDan', bl, fileName); // 文件对象
                            ajaxSubmit(formData);
                        });
                    } else if (imgArr.indexOf(file.type) <= -1 && fileSize > 3072) { //非图片文件限制3M
                        $.messager.alert('提示', '上传的pdf文件大小不能超过3M!');
                    } else {
                        formData.append('LiuLianDan', file);
                        ajaxSubmit(formData);
                    }
                }
            });

            //运输批次附件列表初始化
            $('#files').myDatagrid({
                fitColumns: true,
                fit: false,
                pagination: false,
                actionName: 'dataVoyageFiles',
                rowStyler: function (index, row) {
                    return 'background-color:white;';
                },
                columns: [[
                    { field: 'Name', title: '', width: 70, align: 'center', hidden: true },
                    { field: 'FileType', title: '', width: 70, align: 'center', hidden: true },
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
                }
            });

            //初始化运输批次基本信息
            InitVoyageInfo();

            //运输批次明细初始化
            window.grid = $('#voyageDetails').myDatagrid({
                actionName: 'data',
                onLoadSuccess: function (data) {

                    if (data.total == 0) {
                        grid.myDatagrid('appendRow', { OrderID: '<div style="text-align:center;color:#0081d5">无运输批次明细</div>' }).datagrid('mergeCells', { index: 0, field: 'OrderID', colspan: 5 });
                        //  $(this).datagrid('appendRow', { OrderID: '<div style="text-align:center;color:#0081d5">无运输批次明细</div>' }).datagrid('mergeCells', { index: 0, field: 'OrderID', colspan: 5 });
                        return;
                    }

                    var mark = 1;
                    for (var i = 0; i < data.rows.length; i++) {
                        //合并箱号
                        if (i > 0) {
                            if (data.rows[i]['OrderID'] == data.rows[i - 1]['OrderID']) {
                                mark += 1;
                                $("#voyageDetails").datagrid('mergeCells', {
                                    index: i + 1 - mark,
                                    field: 'OrderID',
                                    rowspan: mark
                                });
                                $("#voyageDetails").datagrid('mergeCells', {
                                    index: i + 1 - mark,
                                    field: 'ClientCode',
                                    rowspan: mark
                                });
                                $("#voyageDetails").datagrid('mergeCells', {
                                    index: i + 1 - mark,
                                    field: 'ClientName',
                                    rowspan: mark
                                });
                                $("#voyageDetails").datagrid('mergeCells', {
                                    index: i + 1 - mark,
                                    field: 'ItemsCount',
                                    rowspan: mark
                                });
                            }
                            else {
                                mark = 1;
                            }
                        }
                    }
                }
            });
        });

        function ajaxSubmit(formData) {
            $.ajax({
                url: '?action=UploadVoyageFile',
                type: 'POST',
                data: formData,
                dataType: 'JSON',
                cache: false,
                processData: false,
                contentType: false,
                success: function (res) {
                    if (res.success) {
                        var data = res.data;
                        var row = $('#files').datagrid('getRows');
                        $('#files').datagrid('insertRow', {
                            index: row.length,
                            row: {
                                ID: data.ID,
                                Name: data.Name,
                                FileType: data.FileType,
                                Url: data.Url
                            }
                        });
                    } else {
                        $.messager.alert('提示', res.message);
                    }
                }
            }).done(function (res) {

            });
        }

        //初始化运输批次基本信息
        function InitVoyageInfo() {
            if (voyage['VoyageCutStatus'] == '<%=Needs.Ccs.Services.Enums.CutStatus.UnCutting.GetHashCode() %>' ||
                voyage['VoyageCutStatus'] == '<%=Needs.Ccs.Services.Enums.CutStatus.Completed.GetHashCode() %>') {
                 $('#OutStock').hide();
            }
            document.getElementById('VoyageNo').innerHTML = voyage['VoyageNo'];
            document.getElementById('Carrier').innerHTML = voyage['Carrier'];
            document.getElementById('HKLicense').innerHTML = voyage['HKLicense'];
            document.getElementById('DriverName').innerHTML = voyage['DriverName'];
            document.getElementById('TransportTime').innerHTML = voyage['TransportTime'];
            document.getElementById('VoyageType').innerHTML = voyage['VoyageType'];

            var params = {
                ID: voyage['ID'],
                Type: '<%=Needs.Ccs.Services.Enums.ClientType.External.GetHashCode() %>',
            };
            $.post('?action=dataVoyageSubtotal', params, function (res) {
                var result = JSON.parse(res);
                document.getElementById('TotalPackNo').innerHTML = result.TotalPackNo;
                document.getElementById('TotalQuantity').innerHTML = result.TotalQuantity;
                document.getElementById('TotalGrossWt').innerHTML = result.TotalGrossWt;
                document.getElementById('TotalAmount').innerHTML = result.TotalAmount;
                document.getElementById('TotalItems').innerHTML = result.TotalItems;
                document.getElementById('IcgooOrders').innerHTML = result.IcgooOrders;
            });
        }

        //查看附件
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

        //删除附件
        // var isRemoveRow = false;
        function Delete(index) {
            $("#files").datagrid('deleteRow', index);
            //解决删除行后，行号错误问题
            var data = $('#pi').datagrid('getData');
            $('#files').datagrid('loadData', data);
        }
        function onClickRow(index) {
            if (isRemoveRow) {
                var row = $('#files').datagrid('getRows')[index];
                $.post('?action=DeleteVoyageFile', { FileID: row.ID }, function (res) {
                    var result = JSON.parse(res);
                    if (result.success) {
                        $('#files').datagrid('deleteRow', index);
                    } else {
                        $.messager.alert('提示', res.message);
                    }
                })

                isRemoveRow = false;
            }
        }

        //显示附件图片
        function ShowInfo(val, row, index) {
            return '<img src="../../App_Themes/xp/images/blue-fujian.png" />'
                + '<a href="javascript:void(0);" style="margin-left: 5px;" onclick="View(\'' + row.Url + '\')">' + row.Name + '</a>'
                + '<a href="javascript:void(0);" style="margin-left: 25px; color: cornflowerblue;" onclick="Delete(' + index + ')">删除</a>';
        }

        //返回
        function Return() {
            var from = getQueryString('From');
            var url;
            switch (from) {
                case 'Logistics':
                    url = location.pathname.replace(/Detail.aspx/ig, 'ListNew.aspx');
                    break;
                case 'HKWarehouse':
                    url = location.pathname.replace(/Detail.aspx/ig, '../../HKWarehouse/Exit/VoyageList.aspx');
                    break;
            }
            window.location = url;
        }

        //导出货物提货委托书
        function ExportDeliveryAgentFile() {
            MaskUtil.mask();
            $.post('?action=ExportDeliveryAgentFile', { ID: voyage['ID'] }, function (rel) {
                MaskUtil.unmask();
                var rel = JSON.parse(rel);
                $.messager.alert('消息', rel.message, 'info', function () {
                    if (rel.success) {
                        //下载文件
                        try {
                            let a = document.createElement('a');
                            document.body.appendChild(a);
                            a.href = rel.url;
                            a.download = "";
                            a.click();
                        } catch (e) {
                            console.log(e);
                        }
                    }
                });
            });
        }

        //导出出库单
        function ExportExitBill() {
            var type = $('input:radio[name="Order"]:checked').val();
            var params;
            if (type == '<%=Needs.Ccs.Services.Enums.ClientType.Internal.GetHashCode() %>') {
                params = {
                    ID: voyage['ID'],
                    ClientType: type,
                    ClientID: $('#Client').combobox('getValue'),
                };
            } else {
                params = {
                    ID: voyage['ID'],
                    ClientType: type,
                };
            }
            MaskUtil.mask();
            $.post('?action=ExportExitBill', params, function (rel) {
                MaskUtil.unmask();
                var rel = JSON.parse(rel);
                $.messager.alert('消息', rel.message, 'info', function () {
                    if (rel.success) {
                        //下载文件
                        try {
                            let a = document.createElement('a');
                            document.body.appendChild(a);
                            a.href = rel.url;
                            a.download = "";
                            a.click();
                        } catch (e) {
                            console.log(e);
                        }
                    }
                });
            });
        }

        ///确认出库
        function ConfirmExitStock() {
            MaskUtil.mask();//遮挡层
            $.post('?action=SaveOutStock', {
                ID: voyage['ID'],
            }, function (result) {
                MaskUtil.unmask();//关闭遮挡层
                var rel = JSON.parse(result);
                if (rel.success) {
                    $.messager.alert('消息', rel.message, 'info', function () {
                        Return();
                    });
                } else {
                    $.messager.alert('提示', rel.message);
                }
            });
        }
    </script>
    <style type="text/css">
        #fujian-table td {
            border-width: 0;
            padding: 0;
        }

            #fujian-table td div {
                padding: 0;
            }

        .content {
            font: 14px Arial,Verdana,'微软雅黑','宋体';
            font-weight: normal;
            width: 25%;
        }

        .border-table {
            line-height: 15px;
            border-collapse: collapse;
            border: 1px solid lightgray;
            width: 100%;
            text-align: center;
        }

            .border-table tr {
                height: 25px;
            }

                .border-table tr td {
                    font-weight: normal;
                    border: 1px solid lightgray;
                    text-align: left;
                    padding: 5px;
                }

                .border-table tr th {
                    font-weight: normal;
                    border: 1px solid lightgray;
                    text-align: left;
                    padding: 5px;
                }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div style="padding: 20px;">
            <div style="background-color: white; border: solid 1px lightgray; margin-bottom: 1px">
                <div style="padding: 5px; margin-bottom: 1px">
                    <a id="ExportDeliveryAgentFile" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="ExportDeliveryAgentFile()">导出提货委托书</a>
                    <a id="ExportExitBill" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="ExportExitBill()">导出出库单</a>
                    <input id="DeliveryAgentFile" name="DeliveryAgentFile" class="easyui-filebox" style="width: 114px; height: 26px;" />
                    <input id="LiuLianDan" name="LiuLianDan" class="easyui-filebox" style="width: 90px; height: 26px" />
                    <a id="OutStock" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-ok'" onclick="ConfirmExitStock()">确认出库</a>
                    <a id="Return" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-back'" onclick="Return()">返回</a>
                </div>
                <div style="padding: 5px;">
                    <input type="radio" name="Order" value="0" id="AllOrder" title="全部订单" class="radio" /><label for="AllOrder" style="margin-right: 20px">全部订单</label>
                    <input type="radio" name="Order" value="<%=Needs.Ccs.Services.Enums.ClientType.External.GetHashCode() %>" id="OutsideOrder" title="仅限B类" class="radio" checked="checked" /><label for="OutsideOrder" style="margin-right: 20px">仅限B类</label>
                    <input type="radio" name="Order" value="<%=Needs.Ccs.Services.Enums.ClientType.Internal.GetHashCode() %>" id="InsideOrder" title="A类" class="radio" /><label for="InsideOrder">A类</label>
                    <input id="Client" class="easyui-combobox" style="width: 280px" />
                </div>
            </div>

            <div id="filesContainer" style="padding: 5px; border: solid 1px lightgray;">
                <div style="margin-bottom: 5px">
                    <label class="content">附&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;件：仅限图片、pdf格式的文件，且pdf文件不超过3M。</label>
                </div>
                <div id="fujian-table" class="pi" style="margin-left: 65px">
                    <table id="files" data-options="pageSize:50,fitColumns:true,fit:false,pagination:false">
                        <%--  <thead>
                            <tr>
                                <th data-options="field:'info',width: 100,align:'left',formatter:ShowInfo"">附件信息</th>
                            </tr>
                        </thead>--%>
                    </table>
                </div>
            </div>

            <table class="border-table" style="width: 100%; margin-top: 5px; margin-bottom: 5px">
                <tr>
                    <td class="content" style="background-color: whitesmoke; width: 20%">货物运输批次号</td>
                    <td class="content" id="VoyageNo" style="width: 30%"></td>
                    <td class="content" style="background-color: whitesmoke; width: 20%">承运商</td>
                    <td class="content" id="Carrier" style="width: 30%"></td>
                </tr>
                <tr>
                    <td class="content" style="background-color: whitesmoke; width: 20%">车牌号</td>
                    <td class="content" id="HKLicense" style="width: 30%"></td>
                    <td class="content" style="background-color: whitesmoke; width: 20%">司机姓名</td>
                    <td class="content" id="DriverName" style="width: 30%"></td>
                </tr>
                <tr>
                    <td class="content" style="background-color: whitesmoke; width: 20%">运输时间</td>
                    <td class="content" id="TransportTime" style="width: 30%"></td>
                    <td class="content" style="background-color: whitesmoke; width: 20%">运输类型</td>
                    <td class="content" id="VoyageType" style="width: 30%"></td>
                </tr>
                <tr>
                    <td class="content" style="background-color: whitesmoke; width: 20%">总件数</td>
                    <td class="content" id="TotalPackNo" style="width: 30%"></td>
                    <td class="content" style="background-color: whitesmoke; width: 20%">总数量</td>
                    <td class="content" id="TotalQuantity" style="width: 30%"></td>
                </tr>
                <tr>
                    <td class="content" style="background-color: whitesmoke; width: 20%">总毛重(KG)</td>
                    <td class="content" id="TotalGrossWt" style="width: 30%"></td>
                    <td class="content" style="background-color: whitesmoke; width: 20%">总金额</td>
                    <td class="content" id="TotalAmount" style="width: 30%"></td>
                </tr>
                <tr>
                    <td class="content" style="background-color: whitesmoke; width: 20%">总条数</td>
                    <td class="content" id="TotalItems" style="width: 30%"></td>
                    <td class="content" style="background-color: whitesmoke; width: 20%">客户委托单号</td>
                    <td class="content" style="width: 30%"><span id="IcgooOrders" style="font-size: 14px;"></span></td>
                </tr>
            </table>

            <table id="voyageDetails" title="运输批次明细" data-options="fitColumns:true,fit:false,pagination:false">
                <thead>
                    <tr>
                        <th data-options="field:'OrderID',align:'left'" style="width: 16%;">订单编号</th>
                        <th data-options="field:'ClientCode',align:'left'" style="width: 18%;">客户编号</th>
                        <th data-options="field:'ClientName',align:'left'" style="width: 18%;">客户名称</th>
                        <th data-options="field:'PackingDate',align:'center'" style="width: 18%;">装箱日期</th>
                        <th data-options="field:'BoxIndex',align:'center'" style="width: 18%;">箱号</th>
                        <th data-options="field:'ItemsCount',align:'center'" style="width: 10%;">条数</th>
                    </tr>
                </thead>
            </table>
        </div>

        <div id="viewFileDialog" class="easyui-window" title="查看附件" data-options="iconCls:'pag-list',modal:true,collapsible:false,minimizable:false,maximizable:true,resizable:true,closed:true" style="width: 1000px; height: 600px;">
            <img id="viewfileImg" src="" style="width: auto; height: auto; max-width: 100%; max-height: 100%;" />
            <iframe id="viewfilePdf" src="" width="100%" height="100%" frameborder="0" scroll="no"></iframe>
        </div>
    </form>
</body>
</html>
