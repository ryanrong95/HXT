<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CustomsFiles.aspx.cs" Inherits="WebApp.Finance.Declare.CustomsFiles" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
     <uc:EasyUI runat="server" />
    <script>
        $(function () {
            var ID = getQueryString("DeclarationID");
            $('#datagraid').myDatagrid({
                pagination: false,
                queryParams: { action: 'data', DecHeadID: ID },
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
            })
        });

        function Operation(val, row, index) {
            var buttons = "";
            var buttons = '<a id="btnView" name="btnView" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" data-fileurl="' + row.FileUrl + '" style="margin:3px" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">查看</span>' +
                '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                '</span>' +
                '</a>';
            return buttons;
        }
    </script>
</head>
<body class="easyui-layout">
   <div id="data" data-options="region:'center',border:false">
        <table id="datagraid"  data-options="fitColumns:true,border:false,fit:true,nowrap:false">
            <thead data-options="frozen:true">
                <tr>                                      
                    <th data-options="field:'FileName',align:'left'" style="width: 50%">附件名称</th>
                    <th data-options="field:'FileType',align:'left'" style="width: 25%">附件类型</th>                                                  
                    <th data-options="field:'Btn',align:'left',formatter:Operation" style="width: 20%">操作</th>   
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
