<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="ListHistory.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm_KQ.Staffs.ListHistory" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        //页面加载
        $(function () {
            window.grid = $("#tab1").myDatagrid({
                toolbar: '#topper',
                pagination: true,
                fitColumns: false,
                rownumbers: true,
                queryParams: getQuery(),
            });
            //员工花名册
            $("#btnExportRoster").click(function () {
                var formData = new FormData($('#form1')[0]);
                formData.append("Date", $("#Date").datebox("getValue"));
                ajaxLoading();
                $.ajax({
                    url: '?action=ExportRoster',
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

            //初始化日期
            $("#Date").datebox({
                onChange: function () {
                    $("#tab1").myDatagrid('search', getQuery());
                }
            })
            var date = new Date();
            var year = date.getFullYear();
            var month = date.getMonth() + 1;
            var day = date.getDate();
            $("#Date").datebox("setValue", year + "-" + month + "-" + day)
        });
    </script>
    <script>
        var getQuery = function () {
            var params = {
                action: 'data',
                Date: $.trim($('#Date').textbox("getText")),
            };
            return params;
        };
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div id="topper">
        <!--搜索按钮-->
        <table class="liebiao-compact">
            <tr>
                <td style="width: 90px;">历史时间点</td>
                <td>
                    <input id="Date" class="easyui-datebox" style="width: 150px;" />
                    <a id="btnExportRoster" class="easyui-linkbutton" iconcls="icon-yg-excelExport">导出公司员工花名册</a>
                </td>
            </tr>
        </table>
    </div>
    <table id="tab1" title="历史员工列表">
        <thead>
            <tr>
                <th data-options="field:'EntryDate',sortable:true,align:'center',width:100">入职日期</th>
                <th data-options="field:'Code',align:'center',width:100">编号</th>
                <th data-options="field:'SelCode',align:'center',width:100">工号</th>
                <th data-options="field:'Name',align:'center',width:100">姓名</th>
                <th data-options="field:'DepartmentCode',align:'center',width:100">部门</th>
                <th data-options="field:'PostionName',align:'center',width:100">岗位</th>
                <th data-options="field:'Gender',align:'center',width:100">性别</th>
                <th data-options="field:'BirthDate',align:'center',width:100">出生日期</th>
                <th data-options="field:'Volk',align:'center',width:100">民族</th>
                <th data-options="field:'IsMarry',align:'center',width:100">婚姻状况</th>
                <th data-options="field:'PassAddress',align:'left',width:250">户口所在地</th>
                <th data-options="field:'HomeAddress',align:'left',width:250">现居地</th>
                <th data-options="field:'IDCard',align:'center',width:150">身份证号</th>
                <th data-options="field:'Education',align:'center',width:100">学历</th>
                <th data-options="field:'GraduatInstitutions',align:'center',width:150">毕业院校</th>
                <th data-options="field:'Major',align:'center',width:100">专业</th>
                <th data-options="field:'Mobile',align:'center',width:100">联系电话</th>
                <th data-options="field:'EmergencyContact',align:'center',width:100">紧急联系人</th>
                <th data-options="field:'EmergencyMobile',align:'center',width:100">紧急联系人电话</th>
                <th data-options="field:'ContractPeriod',align:'center',width:100">合同期限</th>
                <th data-options="field:'LeaveDate',align:'center',width:100">离职日期</th>
                <th data-options="field:'StatusDec',align:'center',width:100">状态</th>
            </tr>
        </thead>
    </table>
</asp:Content>
