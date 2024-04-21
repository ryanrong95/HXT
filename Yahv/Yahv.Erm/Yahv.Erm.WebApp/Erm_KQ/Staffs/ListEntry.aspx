<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="ListEntry.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm_KQ.Staffs.ListEntry" %>

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
                $('#Name').textbox("setValue", "")
                $('#ApprovalStatus').combobox("setValue", "")
                grid.myDatagrid('search', getQuery());
                return false;
            });
            //员工入职登记表
            $("#btnInductionRegistration").click(function () {
                var row = $("#tab1").datagrid('getChecked');
                if(row.length==0){
                    top.$.timeouts.alert({position: "TC",msg: "请勾选员工信息",type: "error"});
                    return;
                }

                var formData = new FormData($('#form1')[0]);
                formData.append("ID", row[0].ID);
                ajaxLoading();
                $.ajax({
                    url: '?action=ExportInductionRegistration',
                    type: 'POST',
                    data: formData,
                    dataType: 'JSON',
                    cache: false,
                    processData: false,
                    contentType: false,
                    success: function (res) {
                        ajaxLoadEnd();
                        $.messager.alert('消息', res.message, 'info', function () {
                            if (res.success) {
                                //下载文件
                                try {
                                    let a = document.createElement('a');
                                    a.href = res.fileUrl;
                                    a.download = "";
                                    a.click();
                                } catch (e) {
                                    console.log(e);
                                }
                            }
                        });
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
                ApprovalStatus: $.trim($('#ApprovalStatus').textbox("getValue")),
            };
            return params;
        };
        var WaitingReport = <%=Yahv.Erm.Services.StaffEntryReportStatus.WaitingReport.GetHashCode()%>;
        var Applied = <%=Yahv.Erm.Services.StaffEntryReportStatus.Applied.GetHashCode()%>;
        //列表内操作项
        function Operation(val, row, index) {
            var arry = [];
            arry.push('<span class="easyui-formatted">');
            if(row.Status == WaitingReport){
                arry.push('<a class="easyui-linkbutton" data-options="iconCls:\'icon-yg-edit\'" onclick="Edit(\'' + row.ID + '\');return false;" group>编辑</a> ');
                arry.push('<a class="easyui-linkbutton" data-options="iconCls:\'icon-yg-assign\'" onclick="Pass(\'' + row.ID + '\');return false;" group>申请入职</a> ');
                arry.push('<a class="easyui-linkbutton" data-options="iconCls:\'icon-no\'" onclick="Fail(\'' + row.ID + '\');return false;" group>未报到</a> ');
            }
            else if(row.Status == Applied){
                arry.push('<a class="easyui-linkbutton" data-options="iconCls:\'icon-man\'" onclick="Approval(\'' + row.ID + '\');return false;" group>入职审批</a> ');
            }
            else{
                arry.push('<a class="easyui-linkbutton" data-options="iconCls:\'icon-yg-details\'" onclick="Detail(\'' + row.ID + '\');return false;" group>详情</a> ');
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
            var url = location.pathname.replace(/ListEntry.aspx/ig, 'Tab.aspx') + '?ID=' + id;
            $.myWindow({
                title: "员工综合信息",
                url: url,
                minWidth:1000,
                onClose: function () {
                    $("#tab1").myDatagrid('flush');
                }
            });
        }
        //详情
        function Detail(id) {
            var url = location.pathname.replace(/ListEntry.aspx/ig, 'TabDetail.aspx') + '?ID=' + id;
            $.myWindow({
                title: "员工综合信息",
                url: url,
                minWidth:1000,
                onClose: function () {
                    $("#tab1").myDatagrid('flush');
                }
            });
        }
        //入职申请
        function Pass(id) {
            $.messager.confirm('确认', '请再次确认是否申请入职。', function (success) {
                if (success) {
                    $.post('?action=Pass', { ID: id }, function (res) {
                        var res = JSON.parse(res);
                        if (res.success) {
                            top.$.timeouts.alert({ position: "TC", msg: res.message, type: "success" });
                        }
                        else {
                            top.$.timeouts.alert({ position: "TC", msg: res.message, type: "error" });
                        }
                        $("#tab1").myDatagrid('flush');
                    })
                }
            });
        }
        //员工未报到
        function Fail(id) {
            var url = location.pathname.replace(/ListEntry.aspx/ig, 'UnReport.aspx') + '?ID=' + id;
            $.myWindow({
                title: "员工未报到",
                url: url,
                width: 600,
                height: 350,
                onClose: function () {
                    $("#tab1").myDatagrid('flush');
                }
            });
        }
        //入职审批
        function Approval(id) {
            var url = location.pathname.replace(/ListEntry.aspx/ig, 'TabApproval.aspx') + '?ID=' + id;
            $.myWindow({
                title: "员工综合信息",
                url: url,
                minWidth: 1000,
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
                <td style="width: 90px;">报到状态</td>
                <td>
                    <input id="ApprovalStatus" data-options="isKeydown:true" class="easyui-combobox" style="width: 150px;" />
                </td>
            </tr>
            <tr>
                <td colspan="8">
                    <a id="btnSearch" class="easyui-linkbutton" iconcls="icon-yg-search">搜索</a>
                    <a id="btnClear" class="easyui-linkbutton" iconcls="icon-yg-clear">清空</a>
                    <em class="toolLine"></em>
                    <a id="btnInductionRegistration" class="easyui-linkbutton" iconcls="icon-yg-excelExport">导出入职登记表</a>
                </td>
            </tr>
        </table>
    </div>
    <table id="tab1" title="入职报到人员列表">
        <thead>
            <tr>
                <th data-options="field:'ck',checkbox:true"></th>
                <th data-options="field:'Code',align:'center',sortable:true,width:80">编号</th>
                <th data-options="field:'Name',align:'center',sortable:true,sorter:SorterOpetation,width:80">名称</th>
                <th data-options="field:'Gender',align:'center',sortable:true,width:80">性别</th>
                <th data-options="field:'Age',align:'center',sortable:true,width:80">年龄</th>
                <th data-options="field:'Education',align:'center',width:80">学历</th>
                <th data-options="field:'GraduatInstitutions',width:150">毕业院校</th>
                <th data-options="field:'Mobile',align:'center',width:100">联系电话</th>
                <th data-options="field:'Email',width:150">邮箱</th>
                <th data-options="field:'StatusDec',align:'center',width:80">报到状态</th>
                <th data-options="field:'ReportDate',align:'center',sortable:true,width:100">计划报到日期</th>
                <th data-options="field:'CreateDate',align:'center',width:100">实际报到日期</th>
                <th data-options="field:'AdminName',align:'center',width:80">报到审批人</th>
                <th data-options="field:'Summary',width:150">备注</th>
                <th data-options="field:'btn',formatter:Operation,width:320">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>
