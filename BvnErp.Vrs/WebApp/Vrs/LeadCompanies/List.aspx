<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Vrs.LeadVenders.List" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>分配公司</title>
    <uc:EasyUI runat="server" />
    <script type="text/javascript">
        $(function () {
            $("#datagrid").bvgrid({ pageSize: 15 });
            $.Paging = function (pageIndex, pageSize) {
                var params = {};
                params["pid"] = "<%=Request["pid"]?.Trim()%>"
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
            var checked = ""
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
            $.post("List.aspx?action=" + ac, { id: id, pid: "<%=Request["pid"]?.Trim()%>", rnd: Math.random() })
        };

    </script>
</head>
<body>
    <div class="easyui-panel" title="管理员列表" data-options="border:false,fit:true,iconCls:'icon-edit',closable:true,onClose:function(){$.myWindow.close();}">
        <div id="toolbar">
            <input id="searchKey" class="easyui-textbox" data-options="prompt:'公司名称'" value="" />
            <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
            <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
        </div>
        <table id="datagrid" data-options="fitColumns:true,border:false,fit:true,toolbar:'#toolbar'" class="mygrid">
            <thead>
                <tr>
                    <th field="btn" data-options="align:'center',formatter:btnformatter" style="width: 100px;">操作(选中建关系)</th>
                    <th field="Name" data-option="align:'center'" style="width: 150px;">公司名称</th>
                    <th field="Type" data-options="width:150,align:'center'">公司类型</th>
                    <th field="Code" data-options="width:100,align:'center'">纳税人识别号</th>
                    <th field="RegisteredCapital" data-options="width:80,align:'center'">注册资金</th>
                    <th field="CorporateRepresentative" data-options="width:80,align:'center'">法人代表</th>
                     <th field="Address" data-options="width:200,align:'center'">公司地址</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
