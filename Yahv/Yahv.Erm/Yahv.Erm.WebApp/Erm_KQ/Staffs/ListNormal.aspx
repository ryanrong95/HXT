<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="ListNormal.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm_KQ.Staffs.ListNormal" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        //页面加载
        $(function () {
            window.grid = $("#tab1").myDatagrid({
                toolbar: '#topper',
                pagination: true,
                fitColumns: false,
                rownumbers: true,
            });
            $('#Position').combobox({
                editable: false,
                valueField: 'Value',
                textField: 'Text',
                data: model.PostionData,
                onChange: function () {
                    grid.myDatagrid('search', getQuery());
                }
            })
            $('#DepartmentType').combobox({
                editable: false,
                valueField: 'Value',
                textField: 'Text',
                data: model.DepartmentType,
                onChange: function () {
                    grid.myDatagrid('search', getQuery());
                }
            })
            $('#PostType').combobox({
                editable: false,
                valueField: 'Value',
                textField: 'Text',
                data: model.PostType,
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
                $('#Name').textbox("setValue", "")
                $('#Position').textbox("setValue", "")
                $('#DepartmentType').textbox("setValue", "")
                $('#PostType').textbox("setValue", "")
                $('#StartDate').datebox("setValue", "")
                $('#EndDate').datebox("setValue", "")
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
                Position: $.trim($('#Position').combobox("getValue")),
                DepartmentType: $.trim($('#DepartmentType').combobox("getValue")),
                PostType: $.trim($('#PostType').combobox("getValue")),
                StartDate: $.trim($('#StartDate').datebox("getValue")),
                EndDate: $.trim($('#EndDate').datebox("getValue")),
            };
            return params;
        };
        var Period = <%=Yahv.Erm.Services.StaffStatus.Period.GetHashCode()%>;
        //列表内操作项
        function Operation(val, row, index) {
            var arry = [];
            arry.push('<span class="easyui-formatted">');
            arry.push('<a class="easyui-linkbutton" data-options="iconCls:\'icon-yg-edit\'" onclick="Edit(\'' + row.ID + '\');return false;" group>编辑</a> ');
            if (row.Status==Period){
                arry.push('<a class="easyui-linkbutton" data-options="iconCls:\'icon-yg-confirm\'" onclick="TurnNormal(\'' + row.ID + '\');return false;" group>转正</a> ');
            }
            arry.push('</span>');
            return arry.join('');
        }
        //中文排序
        function SorterOpetation(a,b)
        {
            return a.localeCompare(b, 'zh');
        }
        //编辑
        function Edit(id) {
            var url = location.pathname.replace(/ListNormal.aspx/ig, 'Tab.aspx') + '?ID=' + id;
            $.myWindow({
                title: "员工综合信息",
                url: url,
                minWidth:1000,
                onClose: function () {
                    $("#tab1").myDatagrid('flush');
                }
            });
        }
        //转正
        function TurnNormal(id) {
            $.messager.confirm('确认', '请您再次确认是否同意转正！', function (success) {
                if (success) {
                    $.post('?action=TurnNormal', { ID: id }, function (result) {
                        var res = JSON.parse(result);
                        if(res.success){
                            top.$.timeouts.alert({position: "TC",msg: "成功!",type: "success"});
                        }
                        else{
                            top.$.timeouts.alert({position: "TC",msg: "失败："+res.message,type: "error"});
                        }
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
                <td style="width: 90px;">姓名或工号</td>
                <td style="width: 200px;">
                    <input id="Name" data-options="isKeydown:true" class="easyui-textbox" style="width: 150px;" />
                </td>
                <td style="width: 90px;">所在部门</td>
                <td colspan="3">
                    <input id="DepartmentType" data-options="isKeydown:true" class="easyui-combobox" style="width: 150px;" />
                </td>
            </tr>
            <tr>
                <td style="width: 90px;">岗位名称</td>
                <td style="width: 200px;">
                    <input id="Position" data-options="isKeydown:true" class="easyui-combobox" style="width: 150px;" />
                </td>
                <td style="width: 90px;">职务类型</td>
                <td style="width: 200px;">
                    <input id="PostType" data-options="isKeydown:true" class="easyui-combobox" style="width: 150px;" />
                </td>
                <td style="width: 90px;">入职日期</td>
                <td>
                    <input id="StartDate" class="easyui-datebox" style="width: 150px;" />
                    &nbsp&nbsp<span>至</span>&nbsp&nbsp
                    <input id="EndDate" class="easyui-datebox" style="width: 150px;" />
                </td>
            </tr>
            <tr>
                <td colspan="6">
                    <a id="btnSearch" class="easyui-linkbutton" iconcls="icon-yg-search">搜索</a>
                    <a id="btnClear" class="easyui-linkbutton" iconcls="icon-yg-clear">清空</a>
                </td>
            </tr>
        </table>
    </div>
    <table id="tab1" title="在职员工列表">
        <thead>
            <tr>
                <th data-options="field:'btn',align:'left',formatter:Operation,width:130">操作</th>
                <th data-options="field:'EntryDate',sortable:true,align:'center',width:100">入职日期</th>
                <th data-options="field:'Code',sortable:true,align:'center',width:100">编号</th>
                <th data-options="field:'SelCode',sortable:true,align:'center',width:100">工号</th>
                <th data-options="field:'Name',sortable:true,sorter:SorterOpetation,align:'center',width:100">姓名</th>
                <th data-options="field:'DepartmentCode',sortable:true,align:'center',width:100">部门</th>
                <th data-options="field:'PostionName',align:'center',width:100">岗位</th>
                <th data-options="field:'Gender',align:'center',width:100">性别</th>
                <th data-options="field:'BirthDate',sortable:true,align:'center',width:100">出生日期</th>
                <th data-options="field:'Volk',align:'center',width:100">民族</th>
                <th data-options="field:'IsMarry',align:'center',width:100">婚姻状况</th>
                <th data-options="field:'PassAddress',align:'left',width:250">户口所在地</th>
                <th data-options="field:'HomeAddress',align:'left',width:250">现居地</th>
                <th data-options="field:'IDCard',align:'center',width:150">身份证号</th>
                <th data-options="field:'Education',align:'center',width:100">学历</th>
                <th data-options="field:'GraduatInstitutions',align:'center',width:150">毕业院校</th>
                <th data-options="field:'Major',align:'center',width:100">专业</th>
                <th data-options="field:'BeginWorkDate',align:'center',width:100">参加工作时间</th>
                <th data-options="field:'Mobile',align:'center',width:100">联系电话</th>
                <th data-options="field:'EmergencyContact',align:'center',width:100">紧急联系人</th>
                <th data-options="field:'EmergencyMobile',align:'center',width:100">紧急联系人电话</th>
                <th data-options="field:'ContractPeriod',align:'center',width:100">合同期限</th>
                <th data-options="field:'StatusDec',align:'center',width:100">状态</th>
            </tr>
        </thead>
    </table>
</asp:Content>
