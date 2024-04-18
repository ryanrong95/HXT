<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Display.aspx.cs" Inherits="WebApp.Order.File.Display" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>订单附件</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">
        $(function () {
            var from = getQueryString('From');
            switch (from) {
                case 'SalesQuery':
                case 'MerchandiserQuery':
                    break;
                case 'UnUploaded':
                case 'Auditing':
                case 'AdminQuery':
                    $('#btnUpload').hide();
                    break;
                case 'DeclareOrderQuery':
                    $('#btnUpload').hide();
                    break;
                default:
                    break;
            }

            //订单附件列表初始化
            $('#orderFiles').myDatagrid({
                actionName: 'dataOrderFiles',
                pagination: true,
                fitcolumns: true,
                fit: true,
                toolbar: '#topBar',
                loadFilter: function (data) {
                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];
                        for (var name in row.item) {
                            row[name] = row.item[name];
                        }
                        delete row.item;
                    }
                    return data;
                }
            });
        });

        //新增附件
        function Upload() {
            var url = location.pathname.replace(/Display.aspx/ig, 'Add.aspx?OrderID=' + getQueryString('ID'));
            top.$.myWindow({
                iconCls: "icon-add",
                url: url,
                noheader: false,
                title: '订单附件',
                width: 680,
                height: 340,
                onClose: function () {
                    $('#orderFiles').datagrid('reload');
                }
            });
        }

        //返回
        function Return() {
            var from = getQueryString('From');
            var url;
            switch (from) {
                case 'MerchandiserQuery':
                    url = location.pathname.replace(/Display.aspx/ig, '../Query/List.aspx');
                    break;
                case 'SalesQuery':
                    url = location.pathname.replace(/Display.aspx/ig, '../Query/SalesList.aspx');
                    break;
                case 'AdminQuery':
                    url = location.pathname.replace(/Display.aspx/ig, '../Query/AdminList.aspx');
                    break;
                case 'InsideQuery':
                    url = location.pathname.replace(/Display.aspx/ig, '../Query/InsideList.aspx');
                    break;
                case 'RiskControl':
                    url = location.pathname.replace(/Display.aspx/ig, '../RiskController/List.aspx');
                    break;
                case 'DeclareOrderQuery':
                    url = location.pathname.replace(/Display.aspx/ig, '../Query/DeclareOrderList.aspx');
                    break;
                default:
                    url = location.pathname.replace(/Display.aspx/ig, '../Query/List.aspx');
                    break;
            }
            window.parent.location = url;
        }

        //下载附件
        function Download(url) {
            let a = document.createElement('a');
            document.body.appendChild(a);
            a.href = url;
            a.download = "";
            a.click();
        }

        //查看附件
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
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Download(\'' + row.Url + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">下载</span>' +
                '<span class="l-btn-icon icon-ok">&nbsp;</span>' +
                '</span>' +
                '</a>';
            buttons += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="View(\'' + row.Url + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">查看</span>' +
                '<span class="l-btn-icon icon-search">&nbsp;</span>' +
                '</span>' +
                '</a>';
            return buttons;
        }
    </script>
    <%-- <style>
        .datagrid-row-selected {
            background: whitesmoke;
            color: #fff;
        }
    </style>--%>
</head>
<body>
    <div id="topBar">
        <div id="tool">
            <a id="btnUpload" href="#" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="Upload()">上传附件</a>
            <a id="btnReturn" href="#" class="easyui-linkbutton" data-options="iconCls:'icon-back'" onclick="Return()">返回</a>
        </div>
    </div>
    <div class="easyui-panel" data-options="region:'center',border:false,fit:true" style="padding: 5px">
        <table id="orderFiles" title="订单附件" data-options="nowrap:false,queryParams:{ action: 'dataOrderFiles' }">
            <thead>
                <tr>
                    <th data-options="field:'OrderID',align:'left'" style="width: 10%">订单编号</th>
                    <th data-options="field:'Name',align:'left'" style="width: 15%">品名</th>
                    <th data-options="field:'Model',align:'center'" style="width: 10%">型号</th>
                    <th data-options="field:'FileName',align:'left'" style="width: 13%">文件名称</th>
                    <th data-options="field:'FileType',align:'center'" style="width: 10%">文件类型</th>
                    <th data-options="field:'FileStatus',align:'center'" style="width: 5%">审核状态</th>
                    <th data-options="field:'Status',align:'center'" style="width: 5%">状态</th>
                    <th data-options="field:'CreateDate',align:'center'" style="width: 10%">创建日期</th>
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 13%;">操作</th>
                </tr>
            </thead>
        </table>
    </div>
    <div id="viewFileDialog" class="easyui-window" title="查看附件" data-options="iconCls:'pag-list',modal:true,collapsible:false,minimizable:false,maximizable:true,resizable:true,closed:true" style="width: 1000px; height: 600px;">
        <img id="viewfileImg" src="" style="width: auto; height: auto; max-width: 100%; max-height: 100%;" />
        <iframe id="viewfilePdf" src="" width="100%" height="100%" frameborder="0" scroll="no"></iframe>
    </div>
</body>
</html>
