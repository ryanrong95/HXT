<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="ListVacation.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm_KQ.Staffs.ListVacation" %>

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
            $('#DepartmentType').combobox({
                editable: false,
                valueField: 'Value',
                textField: 'Text',
                data: model.DepartmentType,
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
            //初始化
            $("#btnInit").click(function () {
                var formData = new FormData($('#form1')[0]);
                ajaxLoading();
                $.ajax({
                    url: '?action=InitVacation',
                    type: 'POST',
                    data: formData,
                    dataType: 'JSON',
                    cache: false,
                    processData: false,
                    contentType: false,
                    success: function (res) {
                        ajaxLoadEnd();
                        $.messager.alert('消息', res.message, 'info', function () { });
                    }
                })
            });
        });
    </script>
    <script>
        var getQuery = function () {
            var params = {
                action: 'data',
                Name: $.trim($('#Name').textbox("getText")),
                DepartmentType: $.trim($('#DepartmentType').combobox("getValue")),
            };
            return params;
        };
        //转正
        function Init(id) {
            $.messager.confirm('确认', '请您确认是否初始化所有员工假期', function (success) {
                if (success) {
                    $.post('?action=TurnNormal', { ID: id }, function (result) {
                        var res = JSON.parse(result);
                        if (res.success) {
                            top.$.timeouts.alert({ position: "TC", msg: "成功!", type: "success" });
                        }
                        else {
                            top.$.timeouts.alert({ position: "TC", msg: "失败：" + res.message, type: "error" });
                        }
                        $("#tab1").myDatagrid('flush');
                    })
                }
            });
        }
        function cellStyler(value, row, index) {
            if (value == 0) {
                return 'background-color:red;';
            }
            else if (value <= 1) {
                return 'background-color:orange;';
            }
            else {
                return 'background-color:lightgreen;';
            }
        }
        function cellStyler2(value, row, index) {
            if (value > 5) {
                return 'background-color:orange;';
            }
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
                <td colspan="6">
                    <a id="btnSearch" class="easyui-linkbutton" iconcls="icon-yg-search">搜索</a>
                    <a id="btnClear" class="easyui-linkbutton" iconcls="icon-yg-clear">清空</a>
                    <a id="btnInit" class="easyui-linkbutton" iconcls="icon-ok" style="display:none">初始化假期</a>
                </td>
            </tr>
        </table>
    </div>
    <table id="tab1" title="员工假期情况" style="background-color:orange">
        <thead>
            <tr>
                <th data-options="field:'ID',align:'center',width:100">ID</th>
                <th data-options="field:'Name',align:'center',width:100">姓名</th>
                <th data-options="field:'Code',align:'center',width:100">编号</th>
                <th data-options="field:'SelCode',align:'center',width:100">编号(XDT)</th>
                <th data-options="field:'DepartmentCode',align:'center',width:100">部门</th>
                <th data-options="field:'PostionName',align:'center',width:100">岗位</th>
                <th data-options="field:'TotalYearDay',align:'center',width:100,styler:cellStyler2">总年假</th>
                <th data-options="field:'TotalSickDay',align:'center',width:100">总病假</th>
                <th data-options="field:'UsedYearDays',align:'center',width:100">已用年假</th>
                <th data-options="field:'RemainYearDays',align:'center',width:100,styler:cellStyler">剩余年假</th>
                <th data-options="field:'TotalOffDay',align:'center',width:100">剩余调休假</th>
                <th data-options="field:'UsedSickDays',align:'center',width:100">已用病假</th>
                <th data-options="field:'CasualLeaveDays',align:'center',width:100">已请事假</th>
                <th data-options="field:'UsedOffDays',align:'center',width:100">已请调休假</th>
                <th data-options="field:'OfficialBusinessDays',align:'center',width:100">已请公务</th>
                <th data-options="field:'BusinessTripDays',align:'center',width:100">已请公差</th>
            </tr>
        </thead>
    </table>
</asp:Content>
