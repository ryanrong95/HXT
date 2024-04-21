<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Admins.ascx.cs" Inherits="Yahv.CrmPlus.WebApp.Uc.Admins" %>
<script>
    var ArrIds = [];
    var ids = "";
    $(function () {
        $.each($('#dg').datagrid('getRows'), function (index, value) {
            ArrIds.push(value.ID);
        });
        ids = ArrIds.join(",");
       

        $("#Admin").combogrid({
            idField: 'ID',
            nowrap: false,
            multiple: true,
            panelWidth: 500,
            fitColumns: true,
            required: true,
            editable: true,
            mode: "local",
            data: eval('<%=AdminsList.Json() %>'),
            columns: [[
                { field: 'ID', title: 'ID', width: 10, align: 'left', hidden: 'true' },
                { field: 'Name', title: '人员名称', width: 100 },
                { field: 'RoleName', title: '角色', width: 100 },
            ]],
            onSelect: function (rowIndex, rowData) {
                if (ArrIds.indexOf(rowData.ID) == -1) {
                    $('#dg').datagrid('appendRow', { ID: rowData.ID, Name: rowData.Name, RoleName: rowData.RoleName });
                    ArrIds.push(rowData.ID);
                   ids = ArrIds.join(",");
                  $("#adminId").val(ids);
              
                } else
                    return;
            },
        });
        window.grid = $("#dg").myDatagrid({
            fitColumns: true,
            fit: true,
            pagination: false,
            rownumbers: true,
            nowrap: false,
            data: eval('<%=SelectedAdminList.Json()%>'),
        });
                 
    });



    function btnformatter(value, rowData, index) {
        var arry = ['<span class="easyui-formatted">'];
        arry.push('<a id="btnDel" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-delete\'" onclick="removeit(\'' + rowData.ID + '\',\'' + index + '\')">删除</a> ');
        arry.push('</span>');
        return arry.join('');
    }
    function removeit(id, index) {
        $('#dg').datagrid('deleteRow', index);
        var idindex = ArrIds.indexOf(id);
        ArrIds.splice(idindex, 1);
       ids = ArrIds.join(",");
        $("#adminId").val(ids);
        var rows = $('#dg').datagrid("getRows");    //重新获取数据生成行号
        $('#dg').datagrid("loadData", rows);
    }

</script>
<div>
    <input  id="adminId" type="hidden" value="" name="adminId" />
    <div>
        <input id="Admin" name="Admin" class="easyui-combogrid" style="width: 500px;" data-options="required:true,valueField:'ID',textField:'Name',editable:false" />
        <table class="easyui-datagrid" title="已选列表" id="dg" style="width: 700px; height: 250px"
            data-options="singleSelect:true,collapsible:true">
            <thead>
                <tr>
                    <th data-options="field:'ID',width:80">ID</th>
                    <th data-options="field:'Name',width:100">人员名称</th>
                    <th data-options="field:'RoleName',width:100">角色</th>
                    <th data-options="field:'otp',width:100,formatter:btnformatter">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</div>
