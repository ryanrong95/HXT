<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.Crm.Charges.Edit" ValidateRequest="false"%>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <uc:EasyUI runat="server" />
    <script>
        var editIndex = undefined;
        function endEditing() {
       
            if (editIndex == undefined) { return true }
            if ($('#dg').datagrid('validateRow', editIndex)) {               
                var ed3 = $('#dg').datagrid('getEditor', { index: editIndex, field: 'CreateDate' });
                var txtDate = $(ed3.target).datebox('getText');             
                $('#dg').datagrid('getRows')[editIndex]['CreateDate'] = txtDate;
                $('#dg').datagrid('endEdit', editIndex);
                editIndex = undefined;
                return true;
            } else {
                return false;
            }
        }
        function onClickRow(index) {
      
            if (editIndex != index) {
                if (endEditing()) {
                    $('#dg').datagrid('selectRow', index)
							.datagrid('beginEdit', index);
                    editIndex = index;
                } else {
                    $('#dg').datagrid('selectRow', editIndex);
                }
            }
        }

        function append() {        
            if (endEditing()) {
                $('#dg').datagrid('appendRow', { status: 'P' });
                editIndex = $('#dg').datagrid('getRows').length - 1;
                $('#dg').datagrid('selectRow', editIndex)
						.datagrid('beginEdit', editIndex);
            }
        }

        function removeit() {
      
            if (editIndex == undefined) { return }
            $('#dg').datagrid('cancelEdit', editIndex)
					.datagrid('deleteRow', editIndex);
            editIndex = undefined;
        }
        function accept() {
            var clientid = getQueryString('ClientID');      
            if (endEditing()) {
                var rows = $('#dg').datagrid('getRows');
                var data = new Array();           
                for (var i = 0; i < rows.length; i++) {
                    var json = {                                        
                        "Count": rows[i].Count,
                        "Price": rows[i].Price,
                        "CreateDate": rows[i].CreateDate,
                        "Summary": rows[i].Summary,
                        "Name":rows[i].Name,
                    }
                    data[i] = json;
                }
                $.post('?action=SavePost', { data: JSON.stringify(data), ClientID: clientid }, function () {
                    $.messager.alert('保存', '保存成功！');
                    $.myWindow.close();
                })
            }
            else {
                alert("请填好资料!");
                return;
            }
        }
        function reject() {     
            $('#dg').datagrid('rejectChanges');
            editIndex = undefined;
        }
        function Save() {
            var isValid = $("#form1").form("enableValidation").form("validate");
            if (!isValid) {
                $.messager.alert('提示', '请按提示输入数据！');
                return false;
            }
            else {
                accept();
            }
        }
    </script>
</head>
<body>  
    <div id="Edit" class="easyui-panel" data-options="border:false,fit:true" >
        <form id="form1" runat="server" method="post">                 
            <table id="dg" class="easyui-datagrid" style="width: auto; height: auto"
                data-options="
				iconCls: 'icon-edit',
				singleSelect: true,
				toolbar: '#tb',
				method: 'get',
				onClickRow: onClickRow
			">
                <thead>
                    <tr>  
                          <th data-options="field:'Name',width:120,editor:{type:'textbox',options:{validType:'length[1,50]',required:true
							},align:'center'}">费用名称</th>
                        <th data-options="field:'Count',width:60,align:'right',editor:{type:'numberbox',options:{validType:'length[1,10]',tipPosition:'bottom'}}">件数</th>                     
                        <th data-options="field:'Price',width:100,align:'right',editor:{type:'numberbox',options:{required:true,validType:'length[1,10]',tipPosition:'bottom'
							},align:'center'}">金额</th>
                        <th data-options="field:'CreateDate',width:120,align:'right',sortable:false,
                            editor:{type:'datebox',options:{editable:false,required:true,tipPosition:'bottom'}},align:'center'">日期</th>
                        <th data-options="field:'Summary',width:400,editor:{type:'textbox',options:{required:true,validType:'length[1,300]',tipPosition:'bottom'
							},align:'center'}">费用描述</th>
                    </tr>
                </thead>
            </table>
            <div id="tb" style="height: auto">
                <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-add',plain:true" onclick="append()">新增</a>
                <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-remove',plain:true" onclick="removeit()">删除</a>
                <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-save',plain:true"  onclick="Save()">保存</a>
                <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-undo',plain:true" onclick="reject()">清空</a>
            </div>
        </form>
    </div>
</body>
</html>
