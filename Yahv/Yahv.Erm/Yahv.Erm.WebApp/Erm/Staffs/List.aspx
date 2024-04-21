<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs"
    Inherits="Yahv.Erm.WebApp.Erm.Staffs.List" %>




<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        //页面加载
        $(function () {
            $("#tab1").myDatagrid({
                toolbar: '#topper',
                pagination: true,
                fitColumns: true
            });
            //初始化Combobox
            $('#PostionID').combobox({
                data: model.PostionData,
            })

            $('#Status').combobox({
                data: model.Status,
            })

            $('#EnterpriseID').combobox({
                data: model.CompaniesData,
            });

            $("#btnImport").on("click",
                function () {
                    if ($("#wageDate").val() == '') {
                        top.$.messager.alert('提示', '请您先选择工资日期!');
                        return;
                    }

                    $("#<%=fileUpload.ClientID%>").click();
                });

            $("#<%=fileUpload.ClientID%>").on("change", function () {
                if (this.value === "") {
                    top.$.messager.alert('提示', '请选择要上传的Excel文件');
                    return;
                } else {
                    var index = this.value.lastIndexOf(".");
                    var extention = this.value.substr(index);
                    if (extention !== ".xls" && extention !== ".xlsx") {
                        top.$.messager.alert('提示', '请选择excel格式的文件!');
                        return;
                    }

                    $("#<%= btn_Import.ClientID %>").click();
                    this.value = '';
        <%--//清空值，确保每次导入都触发change事件--%>
                }
            });
        });

        //员工新增
        function Add() {
            var url = location.pathname.replace(/List.aspx/ig, 'Tab.aspx');
            top.$.myWindow({
                url: url,
                onClose: function () {
                    $("#tab1").myDatagrid('flush');
                },
                width: '80%',
                height: '630px'
            });
        }

        function Edit(id) {
            var url = location.pathname.replace(/List.aspx/ig, 'Tab.aspx') + '?ID=' + id;
            top.$.myWindow({
                url: url,
                onClose: function () {
                    $("#tab1").myDatagrid('flush');
                },
                width: '80%',
                height: '630px'
            });
        }

        //查询
        function Search() {
            var name = $("#Name").textbox("getValue").trim();
            var PostionID = $("#PostionID").combobox("getValue").trim();
            var Status = $("#Status").combobox("getValue").trim();
            var companyID = $("#EnterpriseID").textbox("getValue").trim();
            $("#tab1").myDatagrid('search', { Name: name, PostionID: PostionID, Status: Status, CompanyID: companyID });
        }

        //查询条件重置
        function Reset() {
            window.location.reload();
        }

        function Delete(id) {
            $.messager.confirm('确认', '请您再次确认是否删除所选项！', function (success) {
                if (success) {
                    $.post('?action=Delete', { ID: id }, function () {
                        //$.messager.alert('删除', '删除成功！');
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

        //列表内操作项
        function Operation(val, row, index) {
            var arry = [];
            arry.push('<span class="easyui-formatted">');
            arry.push('<a class="easyui-linkbutton" data-options="iconCls:\'icon-yg-edit\'" onclick="Edit(\'' + row.ID + '\');return false;" group>编辑</a> ');
            arry.push('<a class="easyui-linkbutton" data-options="iconCls:\'icon-yg-delete\'" onclick="Delete(\'' + row.ID + '\');return false;" group>删除</a>');
            arry.push('</span>');
            return arry.join('');
        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="form-group" style="display: none;">
        <asp:FileUpload ID="fileUpload" runat="server" />
        <input type="button" name="btn_Import" id="btn_Import" value="upload" runat="server" onserverclick="btnImport_Click" />
    </div>

    <div id="topper">
        <!--搜索按钮-->
        <table class="liebiao-compact">
            <tr>
                <td style="width: 90px;">姓名</td>
                <td>
                    <input id="Name" data-options="prompt:'姓名/员工编码/大赢家ID',validType:'length[1,50]',isKeydown:true" class="easyui-textbox" style="width: 140px;" />
                </td>
                <td style="width: 90px;">岗位</td>
                <td>
                    <input id="PostionID" data-options="valueField:'Value',textField:'Text'" class="easyui-combobox" />
                </td>
                <td style="width: 90px;">所属公司</td>
                <td>
                    <input id="EnterpriseID" data-options="valueField:'Value',textField:'Text'" style="width: 250px;" class="easyui-combobox" />
                </td>
                <td style="width: 90px;">状态</td>
                <td>
                    <input id="Status" data-options="valueField:'Value',textField:'Text'" class="easyui-combobox" />
                </td>
            </tr>
            <tr>
                <td colspan="5">
                    <a id="btnSearch" class="easyui-linkbutton" iconcls="icon-yg-search" onclick="Search()">搜索</a>
                    <a id="btnClear" class="easyui-linkbutton" iconcls="icon-yg-clear" onclick="Reset()">清空</a>
                    <em class="toolLine"></em>
                    <a id="btnCreator" class="easyui-linkbutton" iconcls="icon-yg-add" onclick="Add()">新建</a>
                    <a id="btnImport" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelImport'">Excel导入</a>
                    <a id="btnExport" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelExport'" runat="server" onserverclick="btnExport_Click">Excel导出</a>
                </td>
                <td style="text-align: right;">
                    <a id="btnDownload" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelImport'" runat="server" onserverclick="btnDownload_Click">模板下载</a>
                </td>
            </tr>
        </table>
    </div>
    <table id="tab1" title="员工信息列表">
        <thead>
            <tr>
                <th data-options="field:'Code',width:50">编码</th>
                <th data-options="field:'Name',width:50">名称</th>
                <%--<th data-options="field:'Gender',width:50">性别</th>--%>
                <th data-options="field:'WorkCity',width:50">所在城市</th>
                <th data-options="field:'PostionID',width:50">岗位</th>
                <th data-options="field:'EnterpriseCompany',width:100">所属公司</th>
                <%--<th data-options="field:'LeaveDate',width:50">离职时间</th>--%>
                <th data-options="field:'Status',width:50">状态</th>
                <th data-options="field:'CreateDate',width:50">创建时间</th>
                <th data-options="field:'btn',formatter:Operation,width:50">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>
