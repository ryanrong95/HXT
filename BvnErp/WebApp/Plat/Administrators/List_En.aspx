<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Plat.Administrators.List" %>

<!DOCTYPE html>

<html>
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>管理员列表</title>
    <uc:EasyUI runat="server" />
    <!--全局变量配置-->
    <script>
        /* 每个需要颗粒化的页面都需要指定 menu ，否则不会写入菜单和颗粒化*/
        gvSettings.fatherMenu = '系统管理(en.Ic360.cn)';
        gvSettings.menu = '管理员管理';
        gvSettings.summary = '';

    </script>
    <script type="text/javascript">
        $(function () {


            $("#datagrid").bvgrid();

            $.Paging = function () {

                var params = {};

                if ($.trim($("#searchKey").textbox('getValue')) != "") {
      
                    params["key"] = $.trim($("#searchKey").val());
                }
                else {
                    params["key"] = "";
                }
                $("#datagrid").bvgrid("search",params);
          
            };
           

            //$.Paging();

            $('#btnAdd').click(function () {
                top.$.myWindow({ url: '/Plat/Administrators/Edit.aspx', onClose: function () { $.Paging(); } }).open();
            });


            $('#btnSearch').click(function () {
                $.Paging();
            });

            $('#btnReset').click(function () {
                $("#searchKey").textbox("setValue","");
                $.Paging();
            });


        });


        var btnformatter = function (val, rec) {
            var arry = [
                //'<button style="cursor:pointer;" onclick="edit(\'' + rec.ID + '\');">编辑</button>',
                '<button style="cursor:pointer;" onclick="myClients(\'' + rec.ID + '\');">分配IC360客户</button>',
                //'<button style="cursor:pointer;" onclick="del(\'' + rec.ID + '\');">删除</button>'
                
            ];

            return arry.join('');
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

        var edit = function (id) {
            top.$.myWindow({ url: '/Plat/Administrators/Edit.aspx?id=' + id, onClose: function () { $.Paging(); } }).open();
        };


        var mySubordinates = function (id) {
            top.$.myWindow({ url: '/Plat/AllotStaff/List.aspx?pid=' + id }).open();
        };

        var myClients = function (id) {
            top.$.myWindow({ url: '/wss_en/AllotClient/List.aspx?pid=' + id }).open();
        };

        var del = function (id) {
            $.messager.confirm($("#AbandonConfirmTitle").val(), $("#AbandonConfirmContent").val(), function (r) {
                if (r) {
                    $.post("?action=del", { id: id }, function (data) {
                        if (data) {
                            $.messager.alert($("#AlertTitle").val(), $("#AbandonSuccess").val());
                        }
                        else {
                            $.messager.alert($("#AlertTitle").val(), $("#AbandonError").val());
                        }
                        $.Paging();
                    })
                }
            });
        };
        var myVenders = function (id) {
            top.$.myWindow({ url: '/vrs/vrs/LeadCompanies/List.aspx?pid=' + id }).open();
        }
        var myContacts = function (id) {
            top.$.myWindow({ url: '/vrs/vrs/LeadContacts/List.aspx?pid=' + id }).open();
        }
    </script>
</head>
<body>
    <div class="easyui-panel"  title="管理员列表" data-options="fit:true">
        <div id="toolbar">           
            <input id="searchKey" class="easyui-textbox" data-options="prompt:'编号/登陆名/真实姓名'" value="" />
            <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
            <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
        </div>
        <table id="datagrid"  class="mygrid" data-options="toolbar:'#toolbar',fit:true">
             <thead frozen="true">
                <tr>
                    <th field="btn" data-options="formatter:btnformatter" style="width: 215px;">操作</th>
                    <th field="ID" data-option="align:'center'" style="width: 150px">编号</th>
                    <th field="UserName"  data-option="align:'center'" style="width: 100px">登陆名</th>
                    <th field="RealName"  data-option="align:'center'" style="width: 100px">真实姓名</th>  
                    <th field="Status" data-options="align:'center',formatter:statusformatter" style="width: 50px;" >状态</th>     
                </tr>
            </thead>
            <thead>
                <tr>
                    <th field="CreateDate" data-options="align:'center'" style="width: 150px;" >创建时间</th>
                    <th field="UpdateDate" data-options="align:'center'" style="width: 150px;" >更新时间</th>
                    <th field="Summary" data-options="align:'left'" style="width: 200px;" >摘要</th>
                </tr>
            </thead>
        </table>
    </div>
    <input type="hidden" id="AlertTitle" value="提示?"/> 
    <input type="hidden" id="AbandonConfirmTitle" value="删除提示?"/> 
    <input type="hidden" id="AbandonConfirmContent" value="您确定要删除选中的记录吗?"/> 
    <input type="hidden" id="AbandonSuccess" value="删除成功！"/> 
    <input type="hidden" id="AbandonError" value="删除失败！" />
</body>
</html>
