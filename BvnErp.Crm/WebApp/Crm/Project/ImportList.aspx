<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ImportList.aspx.cs" Inherits="WebApp.Crm.Project.ImportList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script>
        /* 每个需要颗粒化的页面都需要指定 menu ，否则不会写入菜单和颗粒化*/
        gvSettings.fatherMenu = 'CRM客户管理';
        gvSettings.menu = '销售机会导入';
        gvSettings.summary = '';
    </script>
    <script type="text/javascript">
        //导入校验
        function Import() {
            var a = document.getElementById("exportfile");
            if (a.files.length == 0) {
                $.messager.alert('提示', '请上传导入文件！');
                return;
            };
            if (a.files[0].size > 4096 * 1024) {
                alert("上传的文件不得大于4M!");
                return;
            }
            var filename = a.files[0].name;
            if (filename.indexOf(".xls") < 0) {
                $.messager.alert('提示', "只能上传Excel格式文件");
                return;
            };
            $('#importFileForm').form('submit', {
                url: window.location.pathname + '?action=Import',
                success: function (filename) {
                    var url = location.pathname.replace(/importlist.aspx/ig, 'checklist.aspx') + "?filename=" + filename;
                    top.$.myWindow({
                        iconCls: "",
                        url: url,
                        title: '导入校验页面',
                        width: '90%',
                        height: '90%',
                        noheader: false,
                        onClose: function () {
                            window.location.href = window.location.href;
                        },
                    }).open();
                }
            });
        }
    </script>
</head>
<body>
    <div id="import" title="销售机会信息导入" class="easyui-panel" data-options="border:false,fit:true">
        <form id="importFileForm" method="post" enctype="multipart/form-data" runat="server">
            <table id="table3" style="width: 100%;margin-left:30px;">
                <tr style="height: 30px">
                </tr>
                <tr >
                    <td colspan="5" style="margin-left:20px">
                        <input type="file" id="exportfile" name="exportfile" runat="server" accept="application/vnd.openxmlformats-officedocument.spreadsheetml.sheet,application/vnd.ms-excel" />
                    </td>
                </tr>
                <tr style="height: 20px">
                </tr>
                <tr>
                    <td colspan="5">
                        <h1>说明:</h1>
                        <p style="color:red;">1. 销售机会导入模板中，除送样信息列外，批注为必填的列，必须有值，才能正常导入。</p>
                        <p style="color:red;">2. 送样信息列中如果有一项内容不为空，则送样信息的所有列均不能为空。</p>
                        <p style="color:red;">3. 送样信息列如果没有需要填写的内容，请保持所有的送样信息列均为空即可。</p>
                    </td>
                </tr>
                <tr style="height: 20px">
                </tr>
                <tr>
                    <td colspan="5">
                        <a id="btnImport" href="javascript:void(0)" class="easyui-linkbutton" onclick="Import()">Excel导入</a>
                        <a id="btnFileDownload" name="btnFileDownload" href="../UploadTemplate/销售机会导入模板.xlsx" class="easyui-linkbutton" data-options="iconCls:'icon-excel'">下载导入模板</a>
                    </td>
                </tr>
            </table>
        </form>
    </div>
</body>
</html>
