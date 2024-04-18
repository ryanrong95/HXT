<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Client.ServiceAgreement.List" %>

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
        var Stonum = 0;
        $(function () {
            //电子协议下拉框初始化
            $('#IsSAEleUploadOption').combobox({
                data: IsSAEleUploadOption,
                editable: false,
                valueField: 'TypeValue',
                textField: 'TypeText'
            });

            //纸质协议下拉框初始化
            $('#IsSAPaperUploadOption').combobox({
                data: IsSAPaperUploadOption,
                editable: false,
                valueField: 'TypeValue',
                textField: 'TypeText'
            });

            //费用申请-申请列表初始化
            $('#ServiceAgreementList').myDatagrid({
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
                        //上传服务协议
                        $('#uploadFile' + index).filebox({
                            validType: ['fileSize[10240,"KB"]'],
                            buttonText: '上传文件',
                            buttonAlign: 'right',
                            prompt: '请选择图片或PDF类型的文件',
                            accept: ['image/jpg', 'image/bmp', 'image/jpeg', 'image/gif', 'image/png', 'application/pdf'],
                            onChange: function (e) {
                                if ($(this).next().attr("class").indexOf("textbox-invalid") > 0) {
                                    $.messager.alert('提示', '文件大小不能超过10M！');
                                    return;
                                }

                                var formData = new FormData($('#form1')[0]);
                                var rowNum = this.id.split("uploadFile")[1];
                                var row = $('#ServiceAgreementList').datagrid('getRows')[rowNum];

                                formData.append('RowNum', rowNum);
                                formData.append('ClientID', row.ClientID);
                                formData.append('FileID', row.FileID);
                                formData.append('FileType', '<%=Needs.Ccs.Services.Enums.FileType.ServiceAgreement.GetHashCode() %>');
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
                                            $('#ServiceAgreementList').datagrid('reload');
                                        } else {
                                            $.messager.alert('提示', res.message);
                                        }
                                    }
                                }).done(function (res) {

                                });
                            }
                        });
                    }
                        //上传代仓储协议
                        $('input[name=uploadStoFile]').filebox({
                            validType: ['fileSize[10240,"KB"]'],
                            buttonText: '上传文件',
                            buttonAlign: 'right',
                            prompt: '请选择图片或PDF类型的文件',
                            accept: ['image/jpg', 'image/bmp', 'image/jpeg', 'image/gif', 'image/png', 'application/pdf'],
                            onChange: function (e) {
                                if ($(this).next().attr("class").indexOf("textbox-invalid") > 0) {
                                    $.messager.alert('提示', '文件大小不能超过10M！');
                                    return;
                                }

                                var formData = new FormData($('#form1')[0]);
                                var rowNum = this.id.split("uploadStoFile")[1];
                                var fileRownum = this.id.split("uploadStoFile")[2];
                                var row = $('#ServiceAgreementList').datagrid('getRows')[rowNum];

                                formData.append('RowNum', fileRownum);
                                formData.append('ClientID', row.ClientID);
                                formData.append('FileID', row.FileID);
                                formData.append('FileType', '<%=Needs.Ccs.Services.Enums.FileType.StorageAgreement.GetHashCode() %>');
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
                                            $('#ServiceAgreementList').datagrid('reload');
                                        } else {
                                            $.messager.alert('提示', res.message);
                                        }
                                    }
                                }).done(function (res) {

                                });
                            }
                        });
                   
                }
            });
            Stonum = 0;

        });

        //查询
        function Search() {
            var ClientName = $('#ClientName').textbox('getValue');
            var IsSAEleUpload = $('#IsSAEleUploadOption').combobox('getValue');
            var IsSAPaperUpload = $('#IsSAPaperUploadOption').combobox('getValue');

            var parm = {
                ClientName: ClientName,
                IsSAEleUpload: IsSAEleUpload,
                IsSAPaperUpload: IsSAPaperUpload,
            };

            $('#ServiceAgreementList').myDatagrid('search', parm);
            Stonum = 0;
        }

        //重置查询条件
        function Reset() {
            $('#ClientName').textbox('setValue', null);
            $('#IsSAEleUploadOption').combobox('setValue', null);
            $('#IsSAPaperUploadOption').combobox('setValue', null);

            Search();
        }

        function IsSAEleUploadOperation(val, row, index) {
            var str = '';

            if (row.ServiceType == 2) {
                str += '<span style="color:black;">-</span>';
                return str;
            }

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

        //查看电子版服务协议
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

            if (row.ServiceType == 2) {
                str += '<span style="color:black;">-</span>';
                return str;
            }

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
                + row.ClientID + '\',\'' + row.ClientName + '\',\'' + targetIsSAPaperUpload + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">' + targetIsSAPaperUploadString + '</span>' +
                '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                '</span>' +
                '</a>';

            return str;
        }

        //设置纸质服务协议上交状态
        function SetIsPaperUpload(clientID, clientName, targetIsSAPaperUpload) {
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
                        ClientID: clientID,
                        TargetIsSAPaperUpload: targetIsSAPaperUpload,
                    }, function (res) {
                        MaskUtil.unmask();
                        var result = JSON.parse(res);
                        if (result.success) {
                            $.messager.alert('提示', result.message, 'info', function () {
                                $('#ServiceAgreementList').myDatagrid('reload');
                                Stonum = 0;
                            });
                        } else {
                            $.messager.alert('提示', result.message, 'info', function () {
                                $('#ServiceAgreementList').myDatagrid('reload');
                                Stonum = 0;
                            });
                        }
                    });
                }
            });
        }

        //仓储协议上传情况
        function IsStorageUploadOperation(val, row, index) {
            var str = '';

            if (val == 0 || val == 1) {
                str += '<span style="color:black;">-</span>';
            }
            else {
                if (row.IsStoEleUpload) {
                    str += '<span style="color:green;">已上传</span>';
                    str += '<input id="uploadStoFile' + index + 'uploadStoFile' + Stonum + '" name="uploadStoFile" class="easyui-filebox" style="width: 57px; height: 26px" />';
                    str += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px; margin-left: 10px;" onclick="ViewSAEle(\''
                        + row.StoUrl + '\')" group >' +
                        '<span class =\'l-btn-left l-btn-icon-left\'>' +
                        '<span class="l-btn-text">查看</span>' +
                        '<span class="l-btn-icon icon-search">&nbsp;</span>' +
                        '</span>' +
                        '</a>';
                }
                else {
                    str += '<span style="color:red;">未上传</span>';
                    str += '<input id="uploadStoFile' + index + 'uploadStoFile' + Stonum + '" name="uploadStoFile" class="easyui-filebox" style="width: 57px; height: 26px" />';
                }

                Stonum++;
            }
            return str;
        }

    </script>
</head>
<body class="easyui-layout">
    <form id="form1" runat="server" method="post">
        <div id="topBar">
            <div id="search">
                <ul style="margin-top: 5px; margin-bottom: 5px;">
                    <li>
                        <span class="lbl" style="margin-left: 22px;">客户名称：</span>
                        <input class="easyui-textbox" id="ClientName" data-options="width:280,validType:'length[1,50]'" />
                        <span class="lbl" style="margin-left: 10px;">电子协议：</span>
                        <input class="easyui-combobox" id="IsSAEleUploadOption" data-options="width:160,valueField:'Key',textField:'Value',editable:false," />
                        <span class="lbl" style="margin-left: 10px;">纸质协议：</span>
                        <input class="easyui-combobox" id="IsSAPaperUploadOption" data-options="width:160,valueField:'Key',textField:'Value',editable:false," />

                        <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" style="margin-left: 10px;" onclick="Search()">查询</a>
                        <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                    </li>
                </ul>
            </div>
        </div>
        <div id="data" data-options="region:'center',border:false">
            <table id="ServiceAgreementList" title="客户合同管理" data-options="toolbar:'#topBar',">
                <thead>
                    <tr>
                        <th data-options="field:'ClientCode',align:'center'" style="width: 7%;">客户编号</th>
                        <th data-options="field:'ClientName',align:'left'" style="width: 18%;">客户名称</th>
                        <th data-options="field:'ServiceTypeString',align:'center'" style="width: 7%;">业务类型</th>
                        <th data-options="field:'ClientStatusString',align:'center'" style="width: 7%;">客户状态</th>
                        <th data-options="field:'CreateDate',align:'left'" style="width: 8%;">注册时间</th>
                        <th data-options="field:'ServiceManagerName',align:'center'" style="width: 7%;">业务员</th>
                        <th data-options="field:'IsSAEleUpload',align:'left',formatter:IsSAEleUploadOperation" style="width: 10%;">报关电子协议</th>
                        <th data-options="field:'IsSAPaperUpload',align:'left',formatter:IsSAPaperUploadOperation" style="width: 10%;">报关纸质协议</th>
                        <th data-options="field:'ServiceType',align:'left',formatter:IsStorageUploadOperation" style="width: 10%;">仓储协议</th>
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
