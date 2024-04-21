<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PcFiles.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.PcFiles" %>



<table id="pcFiles" title="附件信息"></table>
<script>

    $(function () {
        $("#pcFiles").myDatagrid({
            fitColumns: true,
            fit: false,
            rownumbers: true,
            pagination: false,
            data: eval('<%=Files.Json()%>'),
            columns: [[
                { field: 'ID', title: '附件编码', width: fixWidth(10) },
                { field: 'CustomName', title: '附件名称', width: fixWidth(20) },
                { field: 'CreateDate', title: '上传时间', width: fixWidth(10) },
                { field: 'btn', title: '操作', width: fixWidth(15), formatter: btnformatter }
            ]]
        });
    });
</script>
