<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="StaffEdit.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm.Staffs.StaffEdit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <style>
        .datagrid-body {
            height: 100px !important;
        }

        .datagrid-wrap panel-body {
            height: 100px !important;
        }
    </style>

    <script>
        var id = getQueryString("ID");
        var Ischange = false;
        $(function () {
            $('#Code').textbox('readonly');
            //绑定下拉框
            $('#PostionID').combobox({
                data: model.PostionData,
                onChange: function () {
                    var PostionID = $(this).combobox('getValue');
                    //访问后台
                    $.post('?action=WageData', { PostionID: PostionID, ID: id }, function (result) {
                        var rel = JSON.parse(result);
                        if (rel.success) {
                            fetchData(rel.PositionItems);
                            Ischange = rel.Ischange;
                        }
                        else {
                            $.messager.alert('提示', rel.data);
                        }
                    })
                }
            });
            $('#WorkCity').combobox({
                data: model.CityData,
            });
            $('#Status').combobox({
                data: model.Status,
            });
            if (model.AllData) {
                $("#UserName").textbox("setValue", model.AllData.UserName);
                $('#UserName').textbox('readonly');
                $("#txtPassword1").textbox("setValue", model.AllData.Password);
                $("#txtPassword2").textbox("setValue", model.AllData.Password);
                $("#passwordfield").attr("style", "display:none;");
                $("#Code").textbox("setValue", model.AllData.Code);
                $("#SelCode").textbox("setValue", model.AllData.SelCode);
                $("#DyjCompanyCode").textbox("setValue", model.AllData.DyjCompanyCode);
                $("#DyjCode").textbox("setValue", model.AllData.DyjCode);
                $("#DyjDepartmentCode").textbox("setValue", model.AllData.DyjDepartmentCode);
                $("#Name").textbox("setValue", model.AllData.Name);
                $("#IDCard").textbox("setValue", model.AllData.IDCard);
                $("#PostionID").combobox("setValue", model.AllData.PostionID);
                $("#WorkCity").combobox("setValue", model.AllData.WorkCity);
                $("#Status").combobox("setValue", model.AllData.Status);
                $("input:radio[name='Gender'][value='" + model.AllData.Gender + "']").attr('checked', 'true');
                $("#Email").textbox("setValue", model.AllData.Email);
                $("#Mobile").textbox("setValue", model.AllData.Mobile);
            }
            else {
                $("#hidingname").attr("style", "display:none;");
                $("#hidingcode").attr("style", "display:none;");
            }
            //fetchData(model.PositionItems);


            $('#Name').textbox('textbox').bind('blur', function () {
                //根据名称生成账号
                if (!$("#UserName").val()) {
                    $.post("?action=createAccount", { name: $('#Name').val() }, function (data) {
                        if (data) {
                            $("#UserName").textbox("setValue", data);
                        }
                    });
                }
            });


        });
        function fetchData(data) {
            var s = "";
            s = "[[";
            $.each(data, function (index, value, array) {

                s += "{field:'" + value.ID + "',title:'" + value.Name + "',editor: { type: 'numberbox', options: { validType: 'length[1,50]' }}},";

            });
            s += "{ field: 'Btn', title: '操作', width: 100, align: 'center', formatter: Operation }";
            s = s + "]]";
            //使用js动态创建easyui的datagrid
            $('#tab1').myDatagrid({
                pagination: false,
                nowrap: true,
                columns: eval(s),
                height: 100,
                fitColumns: false,
                loadFilter: function (data) {
                    if (Ischange) {
                        data = "";
                    }
                    if (!data) {
                        debugger;
                        var fields = $(this).datagrid("getColumnFields");
                        var data1 = new Object();
                        for (var i = 0; i < fields.length - 1; i++) {
                            data1[fields[i]] = 0;
                        }
                        var datat = new Array();
                        datat[0] = data1;
                        return datat;
                    }
                    return data;
                }
            });
        }
        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Edit(' + index + ')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">编辑</span>' +
                '<span class="l-btn-icon icon-yg-edit">&nbsp;</span>' +
                '</span>' +
                '</a>';
            return buttons;
        }
        var editIndex = 0;
        function Edit(index) {
            $('#tab1').datagrid('selectRow', index)
                .datagrid('beginEdit', index);
            editIndex = index;
        }
        function EndEdit(id) {
            //if (editIndex == undefined) { return true }
            $('#tab1').datagrid('endEdit', editIndex);
            debugger;
            var rows = $('#tab1').datagrid('getRows');
            var row = rows[editIndex];
            $.post('?action=SaveDatagrid', { Model: JSON.stringify(row), ID: id }, function (res) {
                //var result = JSON.parse(res);
                //$.messager.alert('消息', result.message, 'info', function () {
                //    if (result.success) {
                //        //保存成功
                //    }
                //});
            });
        }
        function Save() {
            //验证表单数据
            if (!$("#form1").form('validate')) {
                return;
            }
            debugger;
            if (id == "") {
                if (window.parent.tempID != 0) {
                    id = window.parent.tempID;
                }
            }
            var uPattern = /^[a-zA-Z0-9_-]{4,16}$/;
            var re = new RegExp(uPattern);
            var s = $("#UserName").textbox("getValue");
            if (s.search(re) == -1) {
                $.messager.alert('提示', "请先保存正确的用户名");
                return false;
            }
            debugger;
            var data = new FormData($('#form1')[0]);
            data.append('ID', id)
            $.ajax({
                url: '?action=Save',
                type: 'POST',
                data: data,
                dataType: 'JSON',
                cache: false,
                processData: false,
                contentType: false,
                success: function (res) {
                    if (res.success) {
                        $.messager.alert('提示', res.message, 'info', function () {
                            window.parent.tempID = res.ID;
                            EndEdit(res.ID);
                        });
                    } else {
                        $.messager.alert('提示', res.message, 'info', function () {
                        });
                    }
                }
            }).done(function (res) {
            });
        }
        //关闭窗口
        function Close() {
            $.myWindow.close();
        }

        $.extend($.fn.validatebox.defaults.rules, {
            phoneNum: { //验证手机号   
                validator: function (value, param) {
                    return /^1[3-9]+\d{9}$/.test(value);
                },
                message: '请输入正确的手机号码!'
            },

            telNum: { //既验证手机号，又验证座机号
                validator: function (value, param) {
                    return /(^(0[0-9]{2,3}\-)?([2-9][0-9]{6,7})+(\-[0-9]{1,4})?$)|(^(()|(\d{3}\-))?(1[358]\d{9})$)/.test(value);
                },
                message: '请输入正确的电话号码!'
            }
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div style="text-align: left; padding: 5px">
        <a id="btnSave" class="easyui-linkbutton" data-options="iconCls:'icon-yg-save'" onclick="Save()">提交</a>
        <a class="easyui-linkbutton" data-options="iconCls:'icon-yg-cancel'" onclick="Close()">关闭</a>
    </div>
    <div title="员工明细" data-options="fit:true,border:false">
        <table class="liebiao">
            <tr style="display: none;">
                <td>自定义编码：</td>
                <td>
                    <input id="SelCode" name="SelCode" class="easyui-textbox" style="width: 250px;"
                        data-options="prompt:'',required:false,validType:'length[1,50]'" />
                </td>
                <td id="hidingname">员工编码：</td>
                <td id="hidingcode">
                    <input id="Code" name="Code" class="easyui-textbox" style="width: 250px;"
                        data-options="prompt:'',validType:'length[1,50]'" />
                </td>
            </tr>

            <tr>
                <td>大赢家公司编码：</td>
                <td>
                    <input id="DyjCompanyCode" name="DyjCompanyCode" class="easyui-textbox" style="width: 250px;"
                        data-options="prompt:'',required:true,validType:'length[1,50]'" />
                </td>
                <td>大赢家部门编码：</td>
                <td>
                    <input id="DyjDepartmentCode" name="DyjDepartmentCode" class="easyui-textbox" style="width: 250px;"
                        data-options="prompt:'',required:true,validType:'length[1,50]'" />
                </td>
            </tr>

            <tr>
                <td>姓名：</td>
                <td>
                    <input id="Name" name="Name" class="easyui-textbox" style="width: 250px;"
                        data-options="prompt:'',required:true,validType:'length[1,50]'" />
                </td>
                <td>大赢家ID</td>
                <td colspan="3">
                    <input id="DyjCode" name="DyjCode" class="easyui-textbox" style="width: 250px;"
                        data-options="prompt:'ID',required:true,validType:'length[1,50]'" />
                </td>
            </tr>

            <tr>
                <td>用户名</td>
                <td>
                    <input id="UserName" name="UserName" class="easyui-textbox" style="width: 250px;"
                        data-options="prompt:'',required:true,validType:'length[1,75]',tipPosition:'right',missingMessage:'用户名为4到16位（字母，数字，下划线，减号）'" />
                </td>
                <td>性别：</td>
                <td>
                    <input type="radio" name="Gender" value="1" checked />男
                        <input type="radio" name="Gender" value="0" style="margin-left: 10px" />女                       
                </td>
            </tr>

            <tr id="passwordfield" style="display: none;">
                <td>设置密码</td>
                <td>
                    <input id="txtPassword1" name="Password" class="easyui-passwordbox" style="width: 250px;"
                        data-options="prompt:'',required:false,validType:'length[1,50]'" /></td>

                <td>密码确认</td>
                <td>
                    <input id="txtPassword2" class="easyui-passwordbox" style="width: 250px;"
                        data-options="prompt:'',required:false,validType:['equalTo[\'#txtPassword1\']','length[1,50]'],invalidMessage:'两次输入密码不匹配'" />
                </td>
            </tr>

            <tr>
                <td>身份证号：</td>
                <td>
                    <input id="IDCard" name="IDCard" class="easyui-textbox" style="width: 250px;"
                        data-options="prompt:'',required:true,validType:'length[1,50]'" />
                </td>
                <td>所在城市：</td>
                <td>
                    <input id="WorkCity" name="WorkCity" data-options="valueField:'Value',textField:'Text',panelHeight:'160px',required:true" class="easyui-combobox" style="width: 250px" />
                    <input id="editorValue" type="hidden" />
                </td>
                <%--                <td>肖像照：</td>
                <td></td>--%>
            </tr>
            <tr>
                <td>手机号码：</td>
                <td>
                    <input id="Mobile" name="Mobile" class="easyui-textbox" validtype="phoneNum" style="width: 250px;"
                        data-options="prompt:'',required:false," />
                </td>
                <td>邮箱：</td>
                <td>
                    <input id="Email" name="Email" class="easyui-textbox" validtype="email" style="width: 250px;"
                        data-options="prompt:'',required:false," />
                </td>
            </tr>
            <tr>
                <td>分配岗位：</td>
                <td>
                    <input id="PostionID" name="PostionID" data-options="valueField:'Value',textField:'Text',panelHeight:'160px',required:true" class="easyui-combobox" style="width: 250px" />
                </td>
                <td>状态：</td>
                <td>
                    <input id="Status" name="Status" data-options="valueField:'Value',textField:'Text',panelHeight:'160px',required:true" class="easyui-combobox" style="width: 250px" />
                </td>
            </tr>
        </table>
        <table id="tab1" title="工资项默认值"></table>
    </div>
</asp:Content>
