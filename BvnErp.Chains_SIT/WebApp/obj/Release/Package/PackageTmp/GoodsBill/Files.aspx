<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Files.aspx.cs" Inherits="WebApp.GoodsBill.Files" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js"></script>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">        
        $(function () {
            InitClientPage();
            $('#edocs').myDatagrid({
                toolbar:'#topBar',
                singleSelect: false,
                autoRowHeight: false, //自动行高
                autoRowWidth: true,
                pagination: false, //启用分页
                rownumbers: true, //显示行号
                multiSort: true, //启用排序
                fitcolumns: true,
                fit: true,
                nowrap: false,
                onLoadSuccess: function () {
                    $("a[name=btnView]").on('click', function () {
                        var $this = $(this);
                        var fileUrl = $this.data("fileurl");
                        top.$.myWindow({
                            iconCls: "",
                            url: fileUrl,
                            noheader: false,
                            title: '查看电子单据',
                            width: '1024px',
                            height: '600px'
                        });
                    });
                }
            });
        });

        //列表框按钮加载
        function Operation(val, row, index) {
            if (row.FileUrl != "") {
                var buttons = '<a id="btnView" name="btnView" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" data-fileurl="' + row.FileUrl + '" style="margin:3px" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">查看</span>' +
                '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                '</span>' +
                '</a>';
            }
            

            //buttons += '<a id="btnDelete" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="DownLoad(\'' + row.FileUrl + '\')" group >' +
            //    '<span class =\'l-btn-left l-btn-icon-left\'>' +
            //    '<span class="l-btn-text">下载</span>' +
            //    '<span class="l-btn-icon icon-yg-excelExport">&nbsp;</span>' +
            //    '</span>' +
            //    '</a>';

           
            return buttons;
        }

      

     

        function DownLoad(url) {
            try {
                let a = document.createElement('a');
                a.href = url;
                a.download = "";
                a.click();
            } catch (e) {
                console.log(e);
            }
        }

    </script>
</head>
<body class="easyui-layout">
    <div id="data" data-options="region:'center',border:false" style="margin:5px;">
        <table id="edocs" title="文件">
            <thead>
                <tr>
                   <%-- <th data-options="field:'EdocID',align:'left'" style="width: 25%">文件名</th>--%>
                    <th data-options="field:'EdocName',align:'left'" style="width: 25%">单证类别</th>
                    <th data-options="field:'EdocCopId',align:'left'" style="width:25%">原文件名</th>
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width:15%">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>

