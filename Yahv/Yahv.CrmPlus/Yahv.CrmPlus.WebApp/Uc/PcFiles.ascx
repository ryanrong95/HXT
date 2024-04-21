<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PcFiles.ascx.cs" Inherits="Yahv.CrmPlus.WebApp.Uc.PcFiles" %>


<table id="pcFiles" title="附件信息"></table>
<script>
    <% string virtualPath = HttpRuntime.AppDomainAppVirtualPath;%>
    function pcFiles_btnformatter(value, rowData, index) {
        var arry = [];
        arry.push('<span class="easyui-formatted">');
        arry.push('<a class="easyui-linkbutton" data-options="iconCls:\'icon-yg-details\'" onclick="pcFiles_preview(\'' + rowData.Name + '\',\'' + rowData.Url + '\')">预览</a> ');
        arry.push('<a class="easyui-linkbutton" data-options="iconCls:\'icon-yg-edit\'" onclick="pcFiles_downLoad(\'' + rowData.Url + '\')">下载</a> ');
        arry.push('<a class="easyui-linkbutton" data-options="iconCls:\'icon-yg-delete\'" onclick="pcFiles_delfile(\'' + rowData.ID + '\',\'' + index + '\')">删除</a> ');
        arry.push('</span>');
        return arry.join('');
    }
    function pcFiles_preview(name, url) {
        var param = "?src=" + url;
        window.open('<%=virtualPath %>' + "/snapshot.html" + param);
    }
    function pcFiles_downLoad(url) {
        var $a = $("<a></a>").attr("href", url).attr("download", encodeURI(url));
        $a[0].click();
    }
    function pcFiles_delfile(id, index) {
        $.messager.confirm('确认', '确定删除吗？', function (r) {
            if (r) {
                $.post('?ucAction=Delete', { ID: id }, function () {
                    $("#pcFiles").myDatagrid('deleteRow', index);
                    top.$.timeouts.alert({
                        position: "TC",
                        msg: "删除成功!",
                        type: "success"
                    });
                });
            }
        });
    }
    function ftime_btnformatter(value, rowData)
    {
        return value
    }
    $(function () {
       
        $("#pcFiles").myDatagrid({
            fitColumns: true,
            fit: false,
            rownumbers: true,
            pagination: false,
            data: eval('<%=Files.Json() %>'),
            columns: [[
                { field: 'ID', title: '附件编码', width: fixWidth(10) },
                { field: 'CustomName', title: '附件名称', width: fixWidth(15) },
                { field: 'Type', title: '类型', width: fixWidth(10) },
                { field: 'CreateDate', title: '上传时间', formatter: ftime_btnformatter, width: fixWidth(10) },
                { field: 'btn', title: '操作', width: fixWidth(15), formatter: pcFiles_btnformatter }
            ]]
        });
    });
</script>
