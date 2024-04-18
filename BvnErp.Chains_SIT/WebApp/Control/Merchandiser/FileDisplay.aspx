<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FileDisplay.aspx.cs" Inherits="WebApp.Control.Merchandiser.FileDisplay" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>文件上传</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <script type="text/javascript">
        var control = eval('(<%=this.Model.ControlData%>)');

        $(function () {
            //初始化管控基本信息
            document.getElementById('OrderID').innerText = control['OrderID'];
            document.getElementById('ClientName').innerText = control['ClientName'];
            document.getElementById('ClientRank').innerText = control['ClientRank'];
            document.getElementById('DeclarePrice').innerText = control['DeclarePrice'] + '(' + control['Currency'] + ')';
            document.getElementById('Merchandiser').innerText = control['Merchandiser'];

            document.title = '上传' + control['ControlType'];
            document.getElementById('note').innerText = '*订单中含有' + control['ControlType'] + '产品，客户需提供证明文件。';
            document.getElementById('products').title = control['ControlType'] + '产品信息';

            //产品列表初始化
            $('#products').myDatagrid({
                nowrap:false,pageSize:50,pagination:false,fitcolumns:true,fit:false,toolbar:'#topBar',
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
                            accept: ['image/jpg', 'image/bmp', 'image/jpeg', 'image/gif', 'image/png', 'application/pdf'],
                            onChange: function (e) {
                                if ($(this).next().attr("class").indexOf("textbox-invalid") > 0) {
                                    $.messager.alert('提示', '文件大小不能超过10M！');
                                    return;
                                }

                                var formData = new FormData($('#form1')[0]);
                                var rowNum = this.id.split("uploadFile")[1];
                                var row = $('#products').datagrid('getRows')[rowNum];

                                formData.append('RowNum', rowNum);
                                formData.append('OrderID', row.OrderID);
                                formData.append('OrderItemID', row.OrderItemID);
                                formData.append('FileID', row.FileID);
                                if (row.ControlTypeValue == '<%=Needs.Ccs.Services.Enums.OrderControlType.CCC.GetHashCode()%>') {
                                    formData.append('FileType', '<%=Needs.Ccs.Services.Enums.FileType.CCC.GetHashCode()%>');
                                } else if (row.ControlTypeValue == '<%=Needs.Ccs.Services.Enums.OrderControlType.OriginCertificate.GetHashCode()%>') {
                                    formData.append('FileType', '<%=Needs.Ccs.Services.Enums.FileType.OriginCertificate.GetHashCode()%>');
                                }
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
                                            //$.messager.alert('提示', res.message);
                                            $('#products').datagrid('reload');
                                        } else {
                                            $.messager.alert('提示', res.message);
                                        }
                                    }
                                }).done(function (res) {

                                });
                            }
                        });
                    }
                    // var irow = data.total;
                    //$("#products").find(".datagrid-wrap").height(32 * (irow + 1));
                    //$("#products").find(".datagrid-view").height(32 * (irow + 1));
                    //$("#products").find(".datagrid-body").height(32 * irow);

                }
            });
        });

    //确认文件上传完成，审批通过时发生
    function Confirm() {
        var rows = $('#products').datagrid('getRows');
        var isUploaded = true;
        for (var i = 0; i < rows.length; i++) {
            if (rows[i].FileID == null || rows[i].FileID == '') {
                $.messager.alert('提示', '请为所有产品上传证明文件！');
                isUploaded = false;
                break;
            }
        }

        if (isUploaded) {
            $.messager.confirm('确认', '请再次确认证明材料无误，审批通过？', function (success) {
                if (success) {
                    $.post('?action=Confirm', { ID: control['ID'] }, function (res) {
                        var result = JSON.parse(res);
                        if (result.success) {
                            $.messager.alert('消息', result.message, 'info', function () {
                                Return();
                            });
                        } else {
                            $.messager.alert('审批', result.message);
                        }
                    })
                }
            });
        }
    }

    //返回
    function Return() {
        var url = location.pathname.replace(/FileDisplay.aspx/ig, 'List.aspx');
        window.location = url;
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

    //列表框按钮加载
    function Operation(val, row, index) {
        var buttons = '<input id="uploadFile' + index + '" name="uploadFile" class="easyui-filebox" style="width: 57px; height: 26px" />';

        if (row.FileID != null && row.FileID != '') {
            buttons += '<a href="javascript:void(0);" name="btnView" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="View(\'' + row.Url + '\')" group >' +
            '<span class =\'l-btn-left l-btn-icon-left\'>' +
            '<span class="l-btn-text">查看</span>' +
            '<span class="l-btn-icon icon-search">&nbsp;</span>' +
            '</span>' +
            '</a>';
        }

        return buttons;
    }
    </script>
    <style>
        .span {
            font-size: 14px;
        }

        .label {
            font-size: 14px;
            font-weight: 500;
            color: dodgerblue;
            margin-right: 20px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server" method="post">
        <div id="topBar">
            <div id="tool">
                <a id="btnApprove" href="#" class="easyui-linkbutton" data-options="iconCls:'icon-ok'" onclick="Confirm()">确认</a>
                <a id="btnReturn" href="#" class="easyui-linkbutton" data-options="iconCls:'icon-back'" onclick="Return()">返回</a>
            </div>
            <div id="search">
                <ul>
                    <li>
                        <span class="span">订单编号: </span>
                        <label id="OrderID" class="label"></label>
                        <span class="span">客户名称: </span>
                        <label id="ClientName" class="label"></label>
                        <span class="span">信用等级: </span>
                        <label id="ClientRank" class="label"></label>
                        <span class="span">报关货值: </span>
                        <label id="DeclarePrice" class="label"></label>
                        <span class="span">跟单员: </span>
                        <label id="Merchandiser" class="label"></label>
                    </li>
                    <li>
                        <span id="note" style="font-style: italic; color: orangered; font-size: 13px">*订单中含有认证产品，需客户提供证明文件。</span>
                    </li>
                </ul>
            </div>
        </div>
        <div id="data" data-options="region:'center',border:false">
            <table id="products" title="产品信息" data-options="nowrap:false,pageSize:50,pagination:false,fitcolumns:true,fit:false,toolbar:'#topBar'">
                <thead>
                    <tr>
                        <th data-options="field:'Name',align:'left'" style="width:15%">报关品名</th>
                        <th data-options="field:'Model',align:'center'" style="width:10%">型号</th>
                        <th data-options="field:'Manufacturer',align:'center'" style="width:9%">品牌</th>
                        <th data-options="field:'HSCode',align:'center'" style="width:10%">商品编码</th>
                        <th data-options="field:'Quantity',align:'center'" style="width:5%">数量</th>
                        <th data-options="field:'UnitPrice',align:'center'" style="width:10%">单价</th>
                        <th data-options="field:'TotalPrice',align:'center'" style="width:10%">报关总价</th>
                        <th data-options="field:'Origin',align:'center'" style="width:5%">产地</th>
                        <th data-options="field:'Declarant',align:'center'" style="width:10%">报关员</th>
                        <th data-options="field:'Btn',align:'left',formatter:Operation" style="width: 15%;">操作</th>
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
