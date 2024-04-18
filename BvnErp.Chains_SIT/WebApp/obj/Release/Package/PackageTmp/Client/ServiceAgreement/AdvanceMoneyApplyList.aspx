<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdvanceMoneyApplyList.aspx.cs" Inherits="WebApp.Client.ServiceAgreement.AdvanceMoneyApplyList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js"></script>
    <script type="text/javascript">
        var IsSAEleUploadOption = eval('(<%=this.Model.IsSAEleUploadOption%>)');
        var IsSAPaperUploadOption = eval('(<%=this.Model.IsSAPaperUploadOption%>)');
        var AdvanceMoneyApplyStatus = eval('(<%=this.Model.AdvanceMoneyApplyStatus%>)');

        $(function () {
            //垫资申请(电子)下拉框初始化
            $('#IsSAEleUploadOption').combobox({
                data: IsSAEleUploadOption,
                editable: false,
                valueField: 'TypeValue',
                textField: 'TypeText'
            });

            //垫资申请(纸质)下拉框初始化
            $('#IsSAPaperUploadOption').combobox({
                data: IsSAPaperUploadOption,
                editable: false,
                valueField: 'TypeValue',
                textField: 'TypeText'
            });

            $("#AdvanceMoneyApplyStatus").combobox({
                data: AdvanceMoneyApplyStatus,
            });


            loadList();
        });

        //申请列表初始化
        function loadList() {
            $('#AdvanceMoneyApplyList').myDatagrid({
                actionName: 'data',
                fitColumns: true,
                fit: true,
                loadFilter: function (data) {
                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];
                        for (var name in row.item) {
                            row[name] = row.item[name];
                        }
                        delete row.item;
                    }
                    return data;
                },
                onLoadSuccess: function (data) {
                    for (var index = 0; index < data.rows.length; index++) {
                        $('#uploadFile' + index).filebox({
                            validType: ['fileSize[10240,"KB"]'],
                            buttonText: '上传文件',
                            buttonAlign: 'right',
                            prompt: '请选择图片或PDF类型的文件',
                            accept: ['application/pdf'],
                            onChange: function (e) {
                                if ($(this).next().attr("class").indexOf("textbox-invalid") > 0) {
                                    $.messager.alert('提示', '文件大小不能超过10M！');
                                    return;
                                }

                                var formData = new FormData($('#form1')[0]);
                                var rowNum = this.id.split("uploadFile")[1];
                                var row = $('#AdvanceMoneyApplyList').datagrid('getRows')[rowNum];

                                formData.append('RowNum', rowNum);
                                formData.append('ClientID', row.ClientID);
                                formData.append('AdvanceMoneyApplyID', row.AdvanceMoneyApplyID);
                                formData.append('FileID', row.FileID);
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
                                        if (res.success) {
                                            $('#AdvanceMoneyApplyList').datagrid('reload');
                                        } else {
                                            $.messager.alert('提示', res.message);
                                        }
                                    }
                                }).done(function (res) {

                                });
                            }
                        });
                    }
                }
            });
        }

        //查询
        function Search() {

            var ClientCode = $('#ClientCode').textbox('getValue');
            var ClientName = $('#ClientName').textbox('getValue');
            var CreateDateFrom = $('#CreateDateFrom').datebox('getValue');
            var CreateDateTo = $('#CreateDateTo').datebox('getValue');
            var AdvanceMoneyApplyStatus = $('#AdvanceMoneyApplyStatus').combobox('getValue');

            var parm = {
                ClientCode: ClientCode,
                ClientName: ClientName,
                CreateDateFrom: CreateDateFrom,
                CreateDateTo: CreateDateTo,
                AdvanceMoneyApplyStatus: AdvanceMoneyApplyStatus
            };

            $('#AdvanceMoneyApplyList').myDatagrid({
                actionName: 'data',
                queryParams: parm,
                fitColumns: true,
                fit: true,
                loadFilter: function (data) {
                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];
                        for (var name in row.item) {
                            row[name] = row.item[name];
                        }
                        delete row.item;
                    }
                    return data;
                },
                onLoadSuccess: function (data) {
                    for (var index = 0; index < data.rows.length; index++) {
                        $('#uploadFile' + index).filebox({
                            validType: ['fileSize[10240,"KB"]'],
                            buttonText: '上传文件',
                            buttonAlign: 'right',
                            prompt: '请选择图片或PDF类型的文件',
                            accept: ['application/pdf'],
                            onChange: function (e) {
                                if ($(this).next().attr("class").indexOf("textbox-invalid") > 0) {
                                    $.messager.alert('提示', '文件大小不能超过10M！');
                                    return;
                                }

                                var formData = new FormData($('#form1')[0]);
                                var rowNum = this.id.split("uploadFile")[1];
                                var row = $('#AdvanceMoneyApplyList').datagrid('getRows')[rowNum];

                                formData.append('RowNum', rowNum);
                                formData.append('ClientID', row.ClientID);
                                formData.append('AdvanceMoneyApplyID', row.AdvanceMoneyApplyID);
                                formData.append('FileID', row.FileID);
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
                                        if (res.success) {
                                            $('#AdvanceMoneyApplyList').datagrid('reload');
                                        } else {
                                            $.messager.alert('提示', res.message);
                                        }
                                    }
                                }).done(function (res) {

                                });
                            }
                        });
                    }
                }
            });

        }

        //重置查询条件
        function Reset() {
            $('#ClientCode').textbox('setValue', null);
            $('#ClientName').textbox('setValue', null);
            $('#CreateDateFrom').datebox('setValue', null);
            $('#CreateDateTo').datebox('setValue', null);
            $('#AdvanceMoneyApplyStatus').combobox('setValue', null);
            Search();
        }

        function IsSAEleUploadOperation(val, row, index) {
            var str = '';
            if (row.SAUrl != null && row.SAUrl != 'undefined' && row.SAUrl != "" && row.SAUrl.indexOf("doc") == -1) {
                str += '<span style="color:green;">已上传</span>';

                str += '<input id="uploadFile' + index + '" name="uploadFile" class="easyui-filebox" style="width: 57px; height: 26px" />';

                str += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px; margin-left: 10px;" onclick="ViewSAEle(\''
                    + row.SAUrl + '\')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">查看</span>' +
                    '<span class="l-btn-icon icon-search">&nbsp;</span>' +
                    '</span>' +
                    '</a>';
            } else {
                str += '<span style="color:red;">未上传</span>';
                str += '<input id="uploadFile' + index + '" name="uploadFile" class="easyui-filebox" style="width: 57px; height: 26px" />';
            }

            return str;
        }

        //查看电子版
        function ViewSAEle(url) {
            $('#viewfileImg').css('display', 'none');
            $('#viewfilePdf').css('display', 'none');

            if (url.toLowerCase().indexOf('doc') > 0) {
                var a = document.createElement("a");
                //a.download = name + ".xls";
                a.href = url;
                $("body").append(a); // 修复firefox中无法触发click
                a.click();
                $(a).remove();
            } else if (url.toLowerCase().indexOf('pdf') > 0) {
                $('#viewfilePdf').attr('src', url);
                $('#viewfilePdf').css("display", "block");

                $('#viewFileDialog').window('open').window('center');
            } else {
                $('#viewfileImg').attr('src', url);
                $('#viewfileImg').css("display", "block");

                $('#viewFileDialog').window('open').window('center');
            }
        }

        function IsSAPaperUploadOperation(val, row, index) {
            var str = '';
            var targetIsSAPaperUpload = "";
            var targetIsSAPaperUploadString = "";

            if (row.IsSAPaperUpload == "0") {
                str += '<span style="color:red;">' + row.IsSAPaperUploadString + '</span>';
                targetIsSAPaperUpload = "1";
                targetIsSAPaperUploadString = "设为已上交";
            } else {
                str += '<span style="color:green;">' + row.IsSAPaperUploadString + '</span>';
                targetIsSAPaperUpload = "0";
                targetIsSAPaperUploadString = "设为未上交";
            }

            str += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px; margin-left: 10px;" onclick="SetIsPaperUpload(\''
                + row.AdvanceMoneyApplyID + '\',\'' + row.ClientID + '\',\'' + row.ClientName + '\',\'' + targetIsSAPaperUpload + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">' + targetIsSAPaperUploadString + '</span>' +
                '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                '</span>' +
                '</a>';

            return str;
        }

        //设置垫资申请(纸质)上交状态
        function SetIsPaperUpload(advanceMoneyApplyID, clientID, clientName, targetIsSAPaperUpload) {
            var message = '';

            if (targetIsSAPaperUpload == "1") {
                message = '确定要将客户 ' + clientName + ' 的纸质协议设置为已上交？';
            } else {
                message = '确定要将客户 ' + clientName + ' 的纸质协议设置为未上交？';
            }

            $.messager.confirm('确认', message, function (r) {
                if (r) {
                    MaskUtil.mask();
                    $.post(location.pathname + '?action=SetIsPaperUpload', {
                        AdvanceMoneyApplyID: advanceMoneyApplyID,
                        ClientID: clientID,
                        TargetIsSAPaperUpload: targetIsSAPaperUpload,
                    }, function (res) {
                        MaskUtil.unmask();
                        var result = JSON.parse(res);
                        if (result.success) {
                            $.messager.alert('提示', result.message, 'info', function () {
                                $('#AdvanceMoneyApplyList').myDatagrid('reload');
                            });
                        } else {
                            $.messager.alert('提示', result.message, 'info', function () {
                                $('#AdvanceMoneyApplyList').myDatagrid('reload');
                            });
                        }
                    });
                }
            });
        }
    </script>
</head>
<body class="easyui-layout">
    <form id="form1" runat="server" method="post">
        <div id="topBar">
            <div id="search">
                <ul style="margin-top: 5px; margin-bottom: 5px;">
                    <li>
                        <span class="lbl">客户名称：</span>
                        <input class="easyui-textbox" id="ClientName" data-options="width:180" />
                        <span class="lbl">客户编号：</span>
                        <input class="easyui-textbox" id="ClientCode" data-options="width:180" />
                        <span class="lbl">状态：</span>
                        <input class="easyui-combobox" id="AdvanceMoneyApplyStatus" data-options="valueField:'Key',textField:'Value',editable:false, width: 180" />
                    </li>
                </ul>
                <ul style="margin-top: 5px; margin-bottom: 5px;">
                    <li>
                        <span class="lbl">申请日期: </span>
                        <input class="easyui-datebox" id="CreateDateFrom" data-options="editable:false" />
                        <span class="lbl">至 </span>
                        <input class="easyui-datebox" id="CreateDateTo" data-options="editable:false" />

                        <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" style="margin-left: 10px;" onclick="Search()">查询</a>
                        <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                    </li>
                </ul>
            </div>
        </div>
        <div id="data" data-options="region:'center',border:false">
            <table id="AdvanceMoneyApplyList" title="垫资申请" data-options="toolbar:'#topBar',">
                <thead>
                    <tr>
                        <%--<th data-options="field:'AdvanceMoneyApplyID',align:'left'" style="width: 8%;">垫资申请编号</th>--%>
                        <th data-options="field:'ClientName',align:'left'" style="width: 20%;">客户名称</th>
                        <th data-options="field:'ClientCode',align:'left'" style="width: 8%;">客户编号</th>
                        <th data-options="field:'ServiceManager',align:'left'" style="width: 8%;">业务员</th>
                        <th data-options="field:'Amount',align:'center'" style="width: 8%;">金额</th>
                        <th data-options="field:'LimitDays',align:'center'" style="width: 6%;">垫资期限（天）</th>
                        <th data-options="field:'InterestRate',align:'center'" style="width: 6%;">月利率</th>
                        <th data-options="field:'OverdueInterestRate',align:'center'" style="width: 6%;">逾期日利率</th>
                        <th data-options="field:'AdvanceMoneyApplyStatus',align:'left'" style="width: 8%;">状态</th>
                        <th data-options="field:'IsSAEleUpload',align:'left',formatter:IsSAEleUploadOperation" style="width: 10%;">垫款保证协议(电子)</th>
                        <th data-options="field:'IsSAPaperUpload',align:'left',formatter:IsSAPaperUploadOperation" style="width: 15%;">垫款保证协议(纸质)</th>
                    </tr>
                </thead>
            </table>
        </div>

        <div id="viewFileDialog" class="easyui-window" title="查看附件" data-options="iconCls:'pag-list',modal:true,collapsible:false,minimizable:false,maximizable:true,resizable:true,closed:true" style="width: 800px; height: 400px;">
            <img id="viewfileImg" src="" style="width: auto; height: auto; max-width: 100%; max-height: 100%;" />
            <iframe id="viewfilePdf" src="" width="100%" height="100%" frameborder="0" scroll="no"></iframe>
        </div>
    </form>
</body>
</html>
