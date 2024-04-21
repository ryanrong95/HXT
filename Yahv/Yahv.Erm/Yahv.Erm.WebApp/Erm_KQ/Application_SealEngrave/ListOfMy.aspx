<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="ListOfMy.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm_KQ.Application_SealEngrave.ListOfMy" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script src="../../Content/Script/file.js"></script>
    <script>
        //页面加载
        $(function () {
            window.grid = $("#tab1").myDatagrid({
                toolbar: '#topper',
                pagination: true,
                fitColumns: false
            });
            $('#Status').combobox({
                editable: false,
                valueField: 'Value',
                textField: 'Text',
                data: model.Status,
            })
            //搜索
            $("#btnSearch").click(function () {
                grid.myDatagrid('search', getQuery());
            })
            //清空
            $("#btnClear").click(function () {
                $('#Status').textbox("setValue", "")
                grid.myDatagrid('search', getQuery());
                return false;
            });
        });
    </script>
    <script>
        var getQuery = function () {
            var params = {
                action: 'data',
                Status: $.trim($('#Status').combobox("getValue")),
            };
            return params;
        };
        var Draft = <%=Yahv.Erm.Services.ApplicationStatus.Draft.GetHashCode()%>;
        var Reject = <%=Yahv.Erm.Services.ApplicationStatus.Reject.GetHashCode()%>;
        //列表内操作项
        function Operation(val, row, index) {
            var arry = [];
            arry.push('<span class="easyui-formatted">');
            if(row.Status == Draft || row.Status == Reject){
                arry.push('<a class="easyui-linkbutton" data-options="iconCls:\'icon-yg-edit\'" onclick="Edit(\'' + row.ID + '\');return false;" group>编辑</a> ');
                arry.push('<a class="easyui-linkbutton" data-options="iconCls:\'icon-yg-delete\'" onclick="Delete(\'' + row.ID + '\');return false;" group>删除</a>');
            }
            else{
                arry.push('<a class="easyui-linkbutton" data-options="iconCls:\'icon-yg-details\'" onclick="Detail(\'' + row.ID + '\');return false;" group>详情</a> ');
            }
            arry.push('</span>');
            return arry.join('');
        }
        //编辑
        function Edit(id) {
            var title ="编辑申请"
            if(!CheckIsNullOrEmpty(id))
            {
                id="";
                title = "新增申请"
            }
            var url = location.pathname.replace(/ListOfMy.aspx/ig, 'Edit.aspx')+ '?ID=' + id+'&AdminID='+model.CurrentAdmin.ID;
            $.myWindow({
                title: title,
                url: url,
                onClose: function () {
                    $("#tab1").myDatagrid('flush');
                },
            });
        }
        //详情
        function Detail(id) {
            var url = location.pathname.replace(/ListOfMy.aspx/ig, 'Details.aspx')+ '?ID=' + id;
            $.myWindow({
                title: "申请详情",
                url: url,
                onClose: function () {
                    $("#tab1").myDatagrid('flush');
                },
            });
        }
        //删除
        function Delete(id) {
            $.messager.confirm('确认', '请您再次确认是否删除所选项！', function (success) {
                if (success) {
                    $.post('?action=Delete', { ID: id }, function () {
                        top.$.timeouts.alert({
                            position: "TC",
                            msg: "删除成功!",
                            type: "success"
                        });
                        $("#tab1").myDatagrid('flush');
                    })
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
                <td style="width: 90px;">状态</td>
                <td>
                    <input id="Status" class="easyui-combobox" style="width: 150px;" />
                </td>
            </tr>
            <tr>
                <td colspan="8">
                    <a id="btnSearch" class="easyui-linkbutton" iconcls="icon-yg-search">搜索</a>
                    <a id="btnClear" class="easyui-linkbutton" iconcls="icon-yg-clear">清空</a>
                    <em class="toolLine"></em>
                    <a id="btnCreator" class="easyui-linkbutton" iconcls="icon-yg-add" onclick="Edit()">新增</a>
                </td>
            </tr>
        </table>
    </div>
    <table id="tab1" title="我的印章刻制申请列表">
        <thead>
            <tr>
                <th data-options="field:'CreateDate',align:'center',width:100">申请日期</th>
                <th data-options="field:'ApplicantName',align:'center',width:100">申请人</th>
                <th data-options="field:'DepartmentType',align:'center',width:100">所在部门</th>
                <th data-options="field:'Manager',align:'center',width:100">部门负责人</th>
                <th data-options="field:'SealName',align:'center',width:150">印章名称</th>
                <th data-options="field:'SealShape',align:'center',width:100">印章形状</th>
                <th data-options="field:'SealSize',align:'center',width:150">尺寸规则</th>
                <th data-options="field:'Reason',align:'left',width:150">刻章事由</th>
                <th data-options="field:'StatusDec',align:'center',width:100">申请状态</th>
                <th data-options="field:'StepName',align:'center',width:150">当前审批步骤</th>
                <th data-options="field:'AdminName',align:'center',width:100">当前审批人</th>
                <th data-options="field:'btn',formatter:Operation,width:150">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>
