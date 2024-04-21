<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="ListManagerApproval.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm_KQ.Staffs.ListManagerApproval" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        //页面加载
        $(function () {
            window.grid = $("#tab1").myDatagrid({
                toolbar: '#topper',
                pagination: true,
                fitColumns: false,
            });
            $('#ApprovalStatus').combobox({
                editable: false,
                valueField: 'Value',
                textField: 'Text',
                data: model.ApprovalStatus,
                onChange: function () {
                    grid.myDatagrid('search', getQuery());
                }
            })
            //搜索
            $("#btnSearch").click(function () {
                grid.myDatagrid('search', getQuery());
            })
            //清空
            $("#btnClear").click(function () {
                $('#Name').textbox("setValue", "");
                $('#ApprovalStatus').combobox("setValue", "");
                grid.myDatagrid('search', getQuery());
                return false;
            });
        });
    </script>
    <script>
        var getQuery = function () {
            var params = {
                action: 'data',
                Name: $.trim($('#Name').textbox("getText")),
                ApprovalStatus: $.trim($('#ApprovalStatus').textbox("getValue")),
            };
            return params;
        };
        var Waiting = <%=Yahv.Erm.Services.StaffApprovalStatus.Waiting.GetHashCode()%>;
        //列表内操作项
        function Operation(val, row, index) {
            var arry = [];
            arry.push('<span class="easyui-formatted">');
            arry.push('<a class="easyui-linkbutton" data-options="iconCls:\'icon-yg-details\'" onclick="Detail(\'' + row.ID + '\');return false;" group>详情</a> ');
            if(row.Status == Waiting){
                arry.push('<a class="easyui-linkbutton" data-options="iconCls:\'icon-ok\'" onclick="Approval(\'' + row.ID + '\');return false;" group>审批</a>');
            }
            arry.push('</span>');
            return arry.join('');
        }
        //中文排序
        function SorterOpetation(a,b)
        {
            return a.localeCompare(b, 'zh');
        }
        //详情
        function Detail(id) {
            var url = location.pathname.replace(/ListManagerApproval.aspx/ig, 'TabDetail.aspx') + '?ID=' + id;
            $.myWindow({
                title: "员工综合信息",
                url: url,
                minWidth:1000,
                onClose: function () {
                    $("#tab1").myDatagrid('flush');
                }
            });
        }
        //审批
        function Approval(id) {
            var url = location.pathname.replace(/ListManagerApproval.aspx/ig, 'ManagerApproval.aspx') + '?ID=' + id;
            $.myWindow({
                title: "员工应聘审批",
                url: url,
                width:500,
                height:200,
                onClose: function () {
                    $("#tab1").myDatagrid('flush');
                }
            });
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div id="topper">
        <!--搜索按钮-->
        <table class="liebiao-compact">
            <tr>
                <td style="width: 90px;">姓名或编号</td>
                <td style="width: 200px;">
                    <input id="Name" data-options="isKeydown:true" class="easyui-textbox" style="width: 150px;" />
                </td>
                <td style="width: 90px;">审批状态</td>
                <td>
                    <input id="ApprovalStatus" data-options="isKeydown:true" class="easyui-combobox" style="width: 150px;" />
                </td>
            </tr>
            <tr>
                <td colspan="8">
                    <a id="btnSearch" class="easyui-linkbutton" iconcls="icon-yg-search">搜索</a>
                    <a id="btnClear" class="easyui-linkbutton" iconcls="icon-yg-clear">清空</a>
                </td>
            </tr>
        </table>
    </div>
    <table id="tab1" title="经理审批人员列表">
        <thead>
            <tr>
                <th data-options="field:'Code',sortable:true,width:80">编号</th>
                <th data-options="field:'Name',sortable:true,sorter:SorterOpetation,width:80">名称</th>
                <th data-options="field:'Gender',sortable:true,width:80">性别</th>
                <th data-options="field:'Age',sortable:true,width:80">年龄</th>
                <th data-options="field:'Education',width:80">学历</th>
                <th data-options="field:'GraduatInstitutions',width:150">毕业院校</th>
                <th data-options="field:'Mobile',width:100">联系电话</th>
                <th data-options="field:'Email',width:200">邮箱</th>
                <th data-options="field:'StatusDec',sortable:true,width:80">审批状态</th>
                <th data-options="field:'CreateDate',sortable:true,width:80">审批日期</th>
                <th data-options="field:'AdminName',width:80">审批经理</th>
                <th data-options="field:'btn',formatter:Operation,width:320">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>
