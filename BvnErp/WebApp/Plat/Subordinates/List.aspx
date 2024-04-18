<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Plat.Subordinates.List" %>

<!DOCTYPE html>

<html >
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>管理员关系</title>
    <uc:EasyUI runat="server" />
     <script>
        /* 每个需要颗粒化的页面都需要指定 menu ，否则不会写入菜单和颗粒化*/
        gvSettings.fatherMenu = '系统管理';
        gvSettings.menu = '我的员工';
        gvSettings.summary = '';

    </script>
    <script type="text/javascript">
        $(function () { 
            $("#datagrid").bvgrid({ pageSize: 20 });

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
   <div class="easyui-panel" title="管理员列表"  data-options="toolbar:'#toolbar',fit:true">    
       <div id="toolbar">
            <input id="searchKey" class="easyui-textbox" data-options="prompt:'编号/登陆名/真实姓名'" value="" />
            <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
            <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
        </div> 
        <table id="datagrid"  data-options="toolbar:'#toolbar',fit:true,toolbar:'#toolbar'" class="mygrid">
             <thead>
                <tr>
                    <th field="ID" data-option="align:'center'" style="width: 150px">编号</th>
                    <th field="UserName"  data-option="align:'center'" style="width: 150px">登陆名</th>
                    <th field="RealName"  data-option="align:'center'" style="width: 150px">真实姓名</th>  
                    <th field="Status" data-options="align:'center',formatter:statusformatter" style="width: 150px;" >状态</th>     
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
