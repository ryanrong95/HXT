<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.AllotClient.List" %>

<!DOCTYPE html>

<html>
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>分配客户</title>
    <uc:EasyUI runat="server" />
    <script type="text/javascript">
        $(function () {
            $("#datagrid").bvgrid({ pageSize: 15 });

            $.Paging = function (pageIndex, pageSize) {
               
                var params = {};

                if ($.trim($("#searchKey").textbox('getValue')) != "") {

                    params["key"] = $.trim($("#searchKey").val());
                }
                else {
                    params["key"] = "";
                }
                $("#datagrid").bvgrid("search", params);

               
            };
           

            $('#btnSearch').click(function () {
                $.Paging();
            });

            $('#btnReset').click(function () {
                $("#searchKey").textbox("setValue", "");
                $.Paging();
            });

        });


        var btnformatter = function (val, rec) {

            var checked=""
            if (rec.Checked) {
                checked = ' checked=true';
            }
           

            var arry = [
                '<input type="checkbox"  id="chk' + rec.ID + '" style="cursor:pointer;" onclick="edit(\'' + rec.ID + '\');" ' + checked + '/>',
                
            ];

            return arry.join('');
        };

        var edit = function (id) {
            var checked = $("#chk" + id).prop('checked');
            var ac = checked ? "enter" : "remove";
            $.post("?action=" + ac, { id: id, pid: "<%=Request["pid"]?.Trim()%>", rnd: Math.random() })
           
        };


        var statusformatter = function (val, rec) {
            var status = "";
            if (rec.Status == 200) {
                status = "<span class=\"normal\">正常<span/>"
            }
            else {
                status = "<span class=\"delete\">删除<span/>"
            }
            return status
        };

    </script>
</head>
<body>
    <div class="easyui-panel" title="客户列表" data-options="border:false,fit:true,iconCls:'icon-edit',closable:true,onClose:function(){$.myWindow.close();}">    
       <div id="toolbar">
            <input id="searchKey" class="easyui-textbox" data-options="prompt:'编号/登陆名/手机号/邮箱'" value="" />
            <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
            <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
        </div> 
        <table id="datagrid" data-options="fitColumns:true,border:false,fit:true,toolbar:'#toolbar'"  class="mygrid">
             <thead>
                <tr>
                    <th field="btn" data-options="align:'center',formatter:btnformatter" style="width: 100px;">操作(选中建关系)</th>
                    <th field="ID" data-option="align:'center'" style="width: 150px">编号</th>
                     <th field="UserName"  data-option="align:'center'" style="width: 150px">登陆名</th>
                    <th field="Mobile"  data-option="align:'center'" style="width: 150px">手机号</th>  
                    <th field="Email"  data-option="align:'center'" style="width: 150px">邮箱</th> 
                    <th field="Status" data-options="align:'center',formatter:statusformatter" style="width: 150px;" >状态</th>     
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
