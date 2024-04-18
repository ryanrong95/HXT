<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DetailList.aspx.cs" Inherits="WebApp.Crm.Reports.DetailList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
     <uc:EasyUI runat="server" />
        <script type="text/javascript">
        var ActionID = getQueryString('ID');
        $(function () {
            $('#datagrid').bvgrid({
                pageSize: 20,
                loadFilter: function (data) {                   
                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];
                        var jsonContext = JSON.parse(row.Context);                      
                        for (var name in jsonContext) {
                            //row[name] = jsonContext[name];                           
                            if (name == "FollowUpDate" || name == "NextFollowUpDate") {                               
                                row[name] = jsonContext[name].replace("T00:00:00", "");
                            } else {
                                row[name] = jsonContext[name];
                            }                            
                        }
                        delete row.Context;                     
                    }               
                    return data;
                }
            });
        });

    </script>

        <script type="text/javascript">
        //新增
            function Add() {           
            var url = location.pathname.replace(/DetailList.aspx/ig, 'Edit.aspx') + "?ActionID=" + ActionID;
            top.$.myWindow({
                url: url,
                width: '650px',
                height: '300px',
                onClose: function () {
                    $('#datagrid').bvgrid('reload');
                }
            }).open();
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = '<button id="btnEdit" onclick="Edit(' + index + ')">编辑</button>';         
            return buttons;
        }


        //编辑
        function Edit(Index) {          
            $('#datagrid').datagrid('selectRow', Index);
            var rowdata = $('#datagrid').datagrid('getSelected');          
            if (rowdata) {
                var url = location.pathname.replace(/DetailList.aspx/ig, 'Edit.aspx') + "?ID=" + rowdata.ID + "&ActionID=" + ActionID;
                top.$.myWindow({
                    iconCls: "",
                    url: url,
                    width: '650px',
                    height: '300px',
                    onClose: function () {
                        $('#datagrid').bvgrid('reload');
                    }
                }).open();
            }
        }

        //删除
        function Delete(id) {
            $.messager.confirm('确认', '请您再次确认是否删除所选数据！', function (success) {
                if (success) {
                    $.post('?action=Delete', { ID: id }, function () {
                        $.messager.alert('删除', '删除成功！');
                        $('#datagrid').bvgrid('reload');
                    })
                }
            });
        }

        //格式化单元格提示信息  
        function formatCellTooltip(value) {
            return "<span title='" + value + "'>" + value + "</span>";
        }
    </script>
</head>
<body class="easyui-layout">   
    <div  data-options="region:'north',border:false" style="height: 30px">
        <div>               
            <a id="btnAdd" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="Add()">新增</a>
        </div>
    </div>
    <div data-options="region:'center',border:false">
        <table id="datagrid" title="我的报表" data-options="fitColumns:true,border:false,fit:true,scrollbarSize:0" class="mygrid">
            <thead>
                <tr>
                    <th field="Btn" data-options="align:'center',formatter:Operation" style="width: 100px">操作</th>
                    <th field="ID" data-options="align:'center'" style="width: 60px;">ID</th>
                    <th field="FollowUpMethodName" data-options="align:'center'" style="width: 100px;">跟进方式</th>
                    <th field="FollowUpDate" data-options="align:'center'" style="width: 50px">跟进日期</th>
                    <th field="NextFollowUpDate" data-options="align:'center'" style="width: 50px;">下次跟踪日期</th>
                    <th field="FollowUpAdminName" data-options="align:'center'" style="width: 100px;">跟踪人</th>
                    <th field="FollowUpContent" data-options="align:'center',formatter:formatCellTooltip" style="width: 100px;">跟踪结果</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
