<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CustomsInvoice.aspx.cs" Inherits="WebApp.Finance.Declare.CustomsInvoice" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>报关单</title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js"></script>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <%--   <script>
        gvSettings.fatherMenu = '报关单';
        gvSettings.menu = '报关单';
        gvSettings.summary = '';
    </script>--%>
    <script>

        var InvoiceTypeData = eval('(<%=this.Model.InvoiceTypeData%>)');
        var DecTaxStatusData = eval('(<%=this.Model.DecTaxStatusData%>)');

        $(function () {
            //订单列表初始化
            $('#decheads').myDatagrid({
                singleSelect: false,
                checkOnSelect: false,
                selectOnCheck: false,
                onLoadSuccess: function (data) {
                    if (data.rows.length > 0) {
                        //循环判断可勾选行
                        for (var i = 0; i < data.rows.length; i++) {
                            //可选已缴税状态的报关单
                            if (data.rows[i].DecTaxStatus == "<%=Needs.Ccs.Services.Enums.DecTaxStatus.Paid.GetHashCode()%>" && data.rows[i].InvoiceTypeData == "<%=Needs.Ccs.Services.Enums.InvoiceType.Full.GetHashCode()%>") {
                                $("input[type='checkbox']")[i + 1].disabled = false;
                            }
                            else {
                                $("input[type='checkbox']")[i + 1].disabled = 'disabled';
                            }
                        }
                    }

                    $("a[name=btnView]").on('click', function () {
                        var $this = $(this);
                        var fileUrl = $this.data("fileurl");

                        $('#viewfileImg').css("display", "none");
                        $('#viewfilePdf').css("display", "none");
                        if (fileUrl.toLowerCase().indexOf('pdf') > 0) {
                            $('#viewfilePdf').attr('src', fileUrl);
                            $('#viewfilePdf').css("display", "block");

                        }
                        else {
                            $('#viewfileImg').attr('src', fileUrl);
                            $('#viewfileImg').css("display", "block");
                        }
                        $("#viewFileDialog").window('open').window('center');
                    });
                },
                onCheckAll: function (rows) {
                    for (var i = 0; i < rows.length; i++) {
                        //可选已缴税状态的报关单
                        if (rows[i].DecTaxStatus == "<%=Needs.Ccs.Services.Enums.DecTaxStatus.Paid.GetHashCode()%>" && rows[i].InvoiceTypeData == "<%=Needs.Ccs.Services.Enums.InvoiceType.Full.GetHashCode()%>") {
                            $('#decheads').myDatagrid('checkRow', i);
                        }
                        else {
                            $('#decheads').myDatagrid('uncheckRow', i);
                        }
                    }
                    $("input[type='checkbox']")[0].checked = true
                },
            });
            //注册文件上传的onChange事件
            $('#uploadFile').filebox({
                multiple: true,
                validType: ['fileSize[500,"KB"]'],
                buttonText: '上传海关发票',
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

                    var formData = new FormData($('#form1')[0]);
                    MaskUtil.mask();
                    $.ajax({
                        url: '?action=UploadFile',
                        type: 'POST',
                        data: formData,
                        dataType: 'JSON',
                        cache: false,
                        processData: false,
                        contentType: false,
                        success: function (res) {
                            MaskUtil.unmask();
                            $.messager.alert('提示', res.message);
                            $('#decheads').myDatagrid('reload');
                        }
                    });
                }
            });
            $('#uploadExcel').filebox({
                buttonText: '上传缴税流水',
                buttonAlign: 'right',
                buttonIcon: 'icon-add',
                prompt: '请选择Excel文件',
                accept: ['application/vnd.openxmlformats-officedocument.spreadsheetml.sheet, application/vnd.ms-excel'],
                onClickButton: function () {
                    $(this).filebox('setValue', '');
                },
                onChange: function (val) {
                    if ($('#uploadExcel').filebox('getValue') == '') {
                        return;
                    }
                    var formData = new FormData($('#form1')[0]);
                    MaskUtil.mask();
                    $.ajax({
                        url: '?action=UploadExcel',
                        type: 'POST',
                        data: formData,
                        dataType: 'JSON',
                        cache: false,
                        processData: false,
                        contentType: false,
                        success: function (res) {
                            MaskUtil.unmask();
                            $.messager.alert('提示', res.message);
                            $('#decheads').myDatagrid('reload');
                        }
                    });
                }
            });

            //导入抵扣日期等信息
            $('#ImportDikouDate').filebox({
                buttonText: '导入抵扣日期',
                buttonAlign: 'right',
                buttonIcon: 'icon-add',
                prompt: '请选择Excel文件',
                accept: ['application/vnd.openxmlformats-officedocument.spreadsheetml.sheet, application/vnd.ms-excel'],
                onClickButton: function () {
                    $(this).filebox('setValue', '');
                },
                onChange: function (val) {
                    if ($('#ImportDikouDate').filebox('getValue') == '') {
                        return;
                    }
                    var formData = new FormData($('#form1')[0]);
                    MaskUtil.mask();
                    $.ajax({
                        url: '?action=ImportDikouDate',
                        type: 'POST',
                        data: formData,
                        dataType: 'JSON',
                        cache: false,
                        processData: false,
                        contentType: false,
                        success: function (res) {
                            MaskUtil.unmask();
                            if (res.success) {
                                $.messager.alert('提示', res.message);
                            } else {
                                var showmessage = res.message + '<br>';
                                for (var i = 0; i < res.errs.length; i++) {
                                    showmessage += (i + 1) + '. ' + res.errs[i] + '<br>';
                                }
                                //$.messager.alert('提示', showmessage);
                                top.$.myWindow({
                                    iconCls: "",
                                    noheader: false,
                                    title: '提示',
                                    width: '550',
                                    height: '420',
                                    content: '<div style="overflow-y:scroll; height: 380px;">' + showmessage + '</div>',
                                    onClose: function () {
                                        $('#decheads').myDatagrid('reload');
                                    }
                                });
                            }
                            $('#decheads').myDatagrid('reload');
                        }
                    });
                }
            });


            $('#InvoiceType').combobox({
                data: InvoiceTypeData,
                valueField: 'Key',
                textField: 'Value',
            })
            $('#DecTaxStatus').combobox({
                data: DecTaxStatusData,
                valueField: 'Key',
                textField: 'Value',
            })
        });

        //查询
        function Search() {
            var ContrNo = $('#ContrNo').textbox('getValue');
            var OrderID = $('#OrderID').textbox('getValue');
            var EntryID = $('#EntryID').textbox('getValue');
            var InvoiceType = $('#InvoiceType').combobox('getValue');
            var DecTaxStatus = $('#DecTaxStatus').combobox('getValue');
            var DateType = $('#DateType').combobox('getText');
            var StartDate = $('#StartDate').datebox('getValue');
            var EndDate = $('#EndDate').datebox('getValue');
            var OwnerName = $('#OwnerName').textbox('getValue');
            var parm = {
                ContrNo: ContrNo,
                OrderID: OrderID,
                EntryID: EntryID,
                InvoiceType: InvoiceType,
                DecTaxStatus: DecTaxStatus,
                DateType: DateType,
                StartDate: StartDate,
                EndDate: EndDate,
                OwnerName: OwnerName,
            };
            $('#decheads').myDatagrid('search', parm);
        }
        //重置
        function Reset() {
            $('#ContrNo').textbox('setValue', null);
            $('#OrderID').textbox('setValue', null);
            $('#EntryID').textbox('setValue', null);
            $('#InvoiceType').combobox('setValue', null);
            $('#DecTaxStatus').combobox('setValue', null);
            $('#DateType').combobox('setValue', null);
            $('#StartDate').datebox('setValue', null);
            $('#EndDate').datebox('setValue', null);
            $('#OwnerName').textbox('setValue', null);
            Search();
        }
        //导出已缴税未抵扣的增值税
        function Export() {
            //var data = $('#datagrid').myDatagrid('getSelections');
            //if (data.length == 0) {
            //    $.messager.alert('提示', '请先勾选要导出的信息！');
            //    return;
            //}
            //var strIds = "";
            ////拼接字符串
            //for (var i = 0; i < data.length; i++) {
            //    strIds += data[i].ID + ",";
            //}
            //strIds = strIds.substr(0, strIds.length - 1);

            //验证成功
            MaskUtil.mask();
            $.post('?action=Export', {

            }, function (result) {
                var rel = JSON.parse(result);
                $.messager.alert('消息', rel.message, 'info', function () {
                    MaskUtil.unmask();
                    if (rel.success) {
                        //下载文件
                        try {
                            let a = document.createElement('a');
                            a.href = rel.url;
                            a.download = "";
                            a.click();
                        } catch (e) {
                            console.log(e);
                        }
                    }
                });
            })
        }
        //导出报关单缴税流水
        function ExportAll() {
            var data = $('#decheads').myDatagrid('getRows');
            if (data.length == 0) {
                $.messager.alert('提示', '报关单数据为空！');
                return;
            }
            var ContrNo = $('#ContrNo').textbox('getValue');
            var OrderID = $('#OrderID').textbox('getValue');
            var EntryID = $('#EntryID').textbox('getValue');
            var InvoiceType = $('#InvoiceType').combobox('getValue');
            var DecTaxStatus = $('#DecTaxStatus').combobox('getValue');
            var DateType = $('#DateType').combobox('getText');
            var StartDate = $('#StartDate').datebox('getValue');
            var EndDate = $('#EndDate').datebox('getValue');
            var OwnerName = $('#OwnerName').textbox('getValue');
            var param = {
                ContrNo: ContrNo,
                OrderID: OrderID,
                EntryID: EntryID,
                InvoiceType: InvoiceType,
                DecTaxStatus: DecTaxStatus,
                DateType: DateType,
                StartDate: StartDate,
                EndDate: EndDate,
                OwnerName: OwnerName,
            };

            MaskUtil.mask();
            $.post('?action=ExportAll', param, function (result) {
                var rel = JSON.parse(result);
                $.messager.alert('消息', rel.message, 'info', function () {
                    MaskUtil.unmask();
                    if (rel.success) {
                        //下载文件
                        try {
                            let a = document.createElement('a');
                            a.href = rel.url;
                            a.download = "";
                            a.click();
                        } catch (e) {
                            console.log(e);
                        }
                    }
                });
            })
        }

        //导出报关数据
        function ExportDecData() {
            var ContrNo = $('#ContrNo').textbox('getValue');
            var OrderID = $('#OrderID').textbox('getValue');
            var EntryID = $('#EntryID').textbox('getValue');
            var InvoiceType = $('#InvoiceType').combobox('getValue');
            var DecTaxStatus = $('#DecTaxStatus').combobox('getValue');
            var DateType = $('#DateType').combobox('getText');
            var StartDate = $('#StartDate').datebox('getValue');
            var EndDate = $('#EndDate').datebox('getValue');
            var OwnerName = $('#OwnerName').textbox('getValue');
            var param = {
                ContrNo: ContrNo,
                OrderID: OrderID,
                EntryID: EntryID,
                InvoiceType: InvoiceType,
                DecTaxStatus: DecTaxStatus,
                DateType: DateType,
                StartDate: StartDate,
                EndDate: EndDate,
                OwnerName: OwnerName,
            };

            MaskUtil.mask();
            $.post('?action=ExportDecData', param, function (result) {
                var rel = JSON.parse(result);
                $.messager.alert('消息', rel.message, 'info', function () {
                    MaskUtil.unmask();
                    if (rel.success) {
                        //下载文件
                        try {
                            let a = document.createElement('a');
                            a.href = rel.url;
                            a.download = "";
                            a.click();
                        } catch (e) {
                            console.log(e);
                        }
                    }
                });
            })
        }

        //抵扣
        function Deduction() {
            var data = $('#decheads').myDatagrid('getChecked');
            if (data.length == 0) {
                $.messager.alert('提示', '请勾选需要抵扣增值税的报关单');
                return;
            }
            //拼接IDs字符串
            var IDs = "";
            for (var i = 0; i < data.length; i++) {
                IDs += data[i].ID + ",";
            }
            IDs = IDs.substr(0, IDs.length - 1);

            //拼接OrderIDs字符串
            var OrderIDs = "";
            for (var i = 0; i < data.length; i++) {
                OrderIDs += data[i].OrderID + ",";
            }
            OrderIDs = OrderIDs.substr(0, OrderIDs.length - 1);

            var url = location.pathname.replace(/CustomsInvoice.aspx/ig, 'Deduction.aspx') + "?IDs=" + IDs + "&OrderIDs=" + OrderIDs;
            top.$.myWindow({
                iconCls: "",
                noheader: false,
                title: '抵扣增值税',
                width: '500',
                height: '420',
                url: url,
                onClose: function () {
                    $('#decheads').datagrid('reload');
                }
            });
        }

        //是否勾选全选框
        function IsCheckAll() {
            var isCheckAll = true;
            var rows = $('#decheads').datagrid('getRows');
            for (var i = 0; i < rows.length; i++) {
                if (rows[i].Quantity != rows[i].DeliveriedQuantity) {
                    if (!$('#decheads').find("input[type='checkbox']")[i].checked) {
                        isCheckAll = false;
                        break;
                    };
                }
            }
            return isCheckAll;
        }
        //操作
        function Operation(val, row, index) {
            var buttons = "";
            buttons += '<a  href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small"  style="margin:3px" onClick=ViewFile(\'' + row.ID + '\') group>' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">查看附件</span>' +
                '<span class="l-btn-icon icon-search">&nbsp;</span>' +
                '</span>' +
                '</a>';
            return buttons;
        }

        function ViewFile(ID) {
            var url = location.pathname.replace(/CustomsInvoice.aspx/ig, 'CustomsFiles.aspx?DeclarationID=' + ID);
            $.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '附件',
                width: '900px',
                height: '400px'
            });
        }

        function ExportInvoice(InstanceType, FileType, FileTypeName) {
            var ContrNo = $('#ContrNo').textbox('getValue');
            var OrderID = $('#OrderID').textbox('getValue');
            var EntryID = $('#EntryID').textbox('getValue');
            var InvoiceType = $('#InvoiceType').combobox('getValue');
            var DecTaxStatus = $('#DecTaxStatus').combobox('getValue');
            var DateType = $('#DateType').combobox('getText');
            var StartDate = $('#StartDate').datebox('getValue');
            var EndDate = $('#EndDate').datebox('getValue');
            var OwnerName = $('#OwnerName').textbox('getValue');


            if (ContrNo == '' && OrderID == '' && EntryID == '' && StartDate == '' && EndDate == '') {
                $.messager.alert('提示', "请至少选择一种查询条件");
            }

            var parm = {
                ContrNo: ContrNo,
                OrderID: OrderID,
                EntryID: EntryID,
                InvoiceType: InvoiceType,
                DecTaxStatus: DecTaxStatus,
                DateType: DateType,
                StartDate: StartDate,
                EndDate: EndDate,
                OwnerName: OwnerName,
                InstanceType: InstanceType,
                FileType: FileType,
                FileTypeName: FileTypeName
            };

            MaskUtil.mask();
            $.post('?action=ExportSingleTypeFiles', parm, function (result) {
                var rel = JSON.parse(result);
                $.messager.alert('消息', rel.message, 'info', function () {
                    MaskUtil.unmask();
                    if (rel.success) {
                        //下载文件
                        try {
                            let a = document.createElement('a');
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
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <form id="form1">
                <table style="margin: 5px 0;">
                    <tr>
                        <td colspan="6" style="padding-bottom: 8px">
                            <input id="uploadFile" name="uploadFile" class="easyui-filebox" style="width: 100px; height: 26px" />
                            <input id="uploadExcel" name="uploadExcel" class="easyui-filebox" style="width: 100px; height: 26px" />
                            <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelExport'" onclick="Export()">导出抵扣增值税</a>
                            <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelExport'" onclick="ExportAll()">导出缴税流水</a>
                            <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-go'" onclick="Deduction()">抵扣增值税</a>
                            <input id="ImportDikouDate" name="ImportDikouDate" class="easyui-filebox" style="width: 100px; height: 26px" />
                            <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelExport'" onclick="ExportDecData()">导出报关数据</a>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="6" style="padding-bottom: 8px">
                            <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelExport'" onclick="ExportInvoice('1','00000001','商业发票')">导出商业发票</a>
                            <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelExport'" onclick="ExportInvoice('1','00000004','合同')">导出合同</a>
                            <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelExport'" onclick="ExportInvoice('1','00000002','箱单')">导出箱单</a>
                            <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelExport'" onclick="ExportInvoice('2','16','报关单')">导出报关单</a>
                            <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelExport'" onclick="ExportInvoice('2','18','增值税')">导出增值税发票</a>
                            <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelExport'" onclick="ExportInvoice('2','17','关税')">导出关税发票</a>
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl">订单编号：</td>
                        <td>
                            <input class="easyui-textbox" id="OrderID" data-options="height:26,width:150,validType:'length[1,50]'" />
                        </td>
                        <td class="lbl" style="padding-left: 5px">合同号：</td>
                        <td>
                            <input class="easyui-textbox" id="ContrNo" data-options="height:26,width:150,validType:'length[1,50]'" />
                        </td>
                        <td class="lbl" style="padding-left: 5px">客户名称：</td>
                        <td>
                            <input class="easyui-textbox" id="OwnerName" data-options="height:26,width:150,validType:'length[1,50]'" />
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl">海关编号：</td>
                        <td>
                            <input class="easyui-textbox" id="EntryID" data-options="height:26,width:150,validType:'length[1,50]'" />
                        </td>
                        <td class="lbl" style="padding-left: 5px">开票类型：</td>
                        <td>
                            <input class="easyui-combobox" id="InvoiceType" name="InvoiceType"
                                data-options="height:26,width:150,editable:false" />
                        </td>
                        <td class="lbl" style="padding-left: 5px">缴税状态：</td>
                        <td>
                            <input class="easyui-combobox" id="DecTaxStatus" name="DecTaxStatus"
                                data-options="height:26,width:150,editable:false" />
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl">日期类型: </td>
                        <td>
                            <select id="DateType" class="easyui-combobox" data-options="height:26,width:150,editable:false">
                                <option>报关日期</option>
                                <option>缴税日期</option>
                                <option>抵扣日期</option>
                            </select>
                        </td>
                        <td class="lbl" style="padding-left: 5px">开始日期: </td>
                        <td>
                            <input class="easyui-datebox" id="StartDate" data-options="height:26,width:150,editable:false" />
                        </td>
                        <td class="lbl" style="padding-left: 5px">结束日期</td>
                        <td>
                            <input class="easyui-datebox" id="EndDate" data-options="height:26,width:150,editable:false" />
                        </td>
                        <td colspan="2">
                            <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                            <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                        </td>
                    </tr>
                </table>
            </form>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="decheads" title="报关单" data-options="
            fitColumns:true,
            fit:true,
            scrollbarSize:0,
            nowrap:false,
            toolbar:'#topBar'">
            <thead>
                <tr>
                    <th data-options="field:'CheckBox',align:'center',checkbox:true" style="width: 20px">全选</th>
                    <th data-options="field:'OwnerName',align:'left'" style="width: 12%;">客户名称</th>
                    <th data-options="field:'ContrNo',align:'left'" style="width: 8%;">合同号</th>
                    <th data-options="field:'OrderID',align:'left'" style="width: 10%;">订单编号</th>
                    <th data-options="field:'EntryId',align:'left'" style="width: 10%;">海关编号</th>
                    <th data-options="field:'Currency',align:'center'" style="width: 5%;">币种</th>
                    <th data-options="field:'SwapAmount',align:'center'" style="width: 6%;">报关金额</th>
                    <th data-options="field:'OrderAgentAmount',align:'center'" style="width: 7%;">委托金额</th>
                    <th data-options="field:'DDate',align:'center'" style="width: 7%;">报关日期</th>
                    <th data-options="field:'PayDate',align:'center'" style="width: 7%;">缴税日期</th>
                    <th data-options="field:'DeductionTime',align:'center'" style="width: 7%;">抵扣日期</th>
                    <th data-options="field:'FillinDate',align:'center'" style="width: 7%;">填发日期</th>
                    <th data-options="field:'InvoiceType',align:'center'" style="width: 7%;">开票类型</th>
                    <th data-options="field:'Status',align:'center'" style="width: 6%;">缴税状态</th>
                    <th data-options="field:'IsDecHeadTariffFile',align:'center'" style="width: 6%;">关税发票</th>
                    <th data-options="field:'IsDecHeadExciseTaxFile',align:'center'" style="width: 6%;">消费税发票</th>
                    <th data-options="field:'IsDecHeadVatFile',align:'center'" style="width: 6%;">增值税发票</th>
                    <th data-options="field:'btn',width:120,formatter:Operation,align:'center'" style="width: 10%;">操作</th>
                </tr>
            </thead>
        </table>
    </div>
    <div id="viewFileDialog" class="easyui-window" title="查看附件" data-options="iconCls:'pag-list',modal:true,collapsible:false,minimizable:false,maximizable:true,resizable:true,closed:true" style="width: 750px; height: 500px;">
        <img id="viewfileImg" src="" style="width: auto; height: auto; max-width: 100%; max-height: 100%;" />
        <iframe id="viewfilePdf" src="" width="100%" height="99%" frameborder="0" scroll="no"></iframe>
    </div>
</body>
</html>
