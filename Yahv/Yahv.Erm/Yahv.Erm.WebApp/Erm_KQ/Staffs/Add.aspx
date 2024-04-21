<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Add.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm_KQ.Staffs.Add" %>

<%@ Import Namespace="Yahv.Erm.Services" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script src="http://gerpfixed.for-ic.net/My/Scripts/area.data.js"></script>
    <script src="http://gerpfixed.for-ic.net/My/Scripts/areacombo.js"></script>
    <script>
        $(function () {
            $("#Gender").combobox({
                required: true,
                editable: false,
                valueField: 'Value',
                textField: 'Text',
                data: model.Gender,
            })
            $("#Volk").combobox({
                required: true,
                editable: false,
                valueField: 'Value',
                textField: 'Text',
                data: model.ChineseNationType,
            })
            $("#PoliticalOutlook").combobox({
                required: true,
                editable: false,
                valueField: 'Value',
                textField: 'Text',
                data: model.PoliticType,
            })
            $("#Healthy").combobox({
                required: true,
                editable: false,
                valueField: 'Value',
                textField: 'Text',
                data: model.HealthyType,
            })
            $("#Blood").combobox({
                required: true,
                editable: false,
                valueField: 'Value',
                textField: 'Text',
                data: model.BloodType,
            })
            $("#Education").combobox({
                required: true,
                editable: false,
                valueField: 'Value',
                textField: 'Text',
                data: model.EducationType,
            })
            $("#IsMarry").combobox({
                required: true,
                editable: false,
                valueField: 'Value',
                textField: 'Text',
                data: model.MaritalStatus,
            })
            //工作经历
            $("#dg").myDatagrid({
                toolbar: '#topper',
                fitColumns: false,
                fit: false,
                singleSelect: true,
                pagination: false,
                actionName: 'LoadWork',
                onClickRow: onClickRow,
                columns: [[
                    { field: 'StartTime', title: '开始日期', width: 120, align: 'center', editor: { type: 'datebox', options: { required: true, editable: false } } },
                    { field: 'EndTime', title: '结束日期', width: 120, align: 'center', editor: { type: 'datebox', options: { required: true, editable: false } } },
                    { field: 'Company', title: '工作单位', width: 200, align: 'center', editor: { type: 'textbox', options: { required: true } } },
                    { field: 'Position', title: '职务名称', width: 150, align: 'center', editor: { type: 'textbox', options: { required: true } } },
                    { field: 'Salary', title: '薪资', width: 100, align: 'center', editor: { type: 'numberbox', options: { required: true } } },
                    { field: 'LeaveReason', title: '离职原因', width: 250, align: 'center', editor: { type: 'textbox', options: { required: true } } },
                    { field: 'Phone', title: '单位电话', width: 120, align: 'center', editor: { type: 'textbox', options: { required: false } } },
                    { field: 'Btn', title: '操作', width: 100, align: 'center', formatter: OperationWork }
                ]],
            });
            //添加工作经历
            $("#btnAddWorkExperience").click(function () {
                AddWork();
            })
            //家庭成员
            $("#family").myDatagrid({
                toolbar: '#topperfamily',
                fitColumns: false,
                fit: false,
                singleSelect: true,
                pagination: false,
                actionName: 'LoadFamily',
                onClickRow: onClickRow2,
                columns: [[
                    { field: 'Name', title: '姓名', width: 120, align: 'center', editor: { type: 'textbox', options: { required: true } } },
                    { field: 'Relation', title: '与本人关系', width: 120, align: 'center', editor: { type: 'textbox', options: { required: true } } },
                    { field: 'Age', title: '年龄', width: 120, align: 'center', editor: { type: 'numberbox', options: { required: true } } },
                    { field: 'Company', title: '工作单位', width: 200, align: 'center', editor: { type: 'textbox', options: { required: true } } },
                    { field: 'Position', title: '职业', width: 150, align: 'center', editor: { type: 'textbox', options: { required: true } } },
                    { field: 'Phone', title: '手机号码', width: 150, align: 'center', editor: { type: 'textbox', options: { required: true, validType: 'phoneNum' } } },
                    { field: 'Btn', title: '操作', width: 100, align: 'center', formatter: OperationFamily }
                ]],
            });
            //添加家庭成员
            $("#btnAddFamily").click(function () {
                AddFamily();
            })
            //保存
            $("#btnSubmit").click(function () {
                //验证必填项
                var isValid = $('#form1').form('enableValidation').form('validate');
                if (!isValid) {
                    return false;
                }
                endEditing();
                endEditing2();
                var data = new FormData();
                //基本信息
                data.append('Name', $("#Name").textbox("getValue"));
                data.append('Gender', $("#Gender").combobox("getValue"));
                data.append('BirthDate', $("#BirthDate").datebox("getValue"));
                data.append('Volk', $('#Volk').combobox('getValue'));
                data.append('PoliticalOutlook', $("#PoliticalOutlook").combobox("getValue"));
                data.append('IsMarry', $("#IsMarry").combobox("getValue"));
                data.append('Healthy', $("#Healthy").combobox("getValue"));
                data.append('IDCard', $("#IDCard").textbox("getValue"));
                data.append('NativePlace', $("#NativePlace").area("getValue"));
                data.append('PassAddress', $("#PassAddress").area("getValue"));
                data.append('Blood', $("#Blood").textbox("getValue"));
                data.append('Height', $("#Height").textbox("getValue"));
                data.append('Weight', $("#Weight").textbox("getValue"));
                data.append('Education', $("#Education").combobox("getValue"));
                data.append('Major', $("#Major").textbox("getValue"));
                data.append('GraduationDate', $("#GraduationDate").datebox("getValue"));
                data.append('GraduatInstitutions', $("#GraduatInstitutions").textbox("getValue"));
                data.append('HomeAddress', $("#HomeAddress").area("getValue"));
                data.append('Mobile', $("#Mobile").textbox("getValue"));
                data.append('Email', $("#Email").textbox("getValue"));
                data.append('BeginWorkDate', $("#BeginWorkDate").datebox("getValue"));
                data.append('EmergencyContact', $("#EmergencyContact").textbox("getValue"));
                data.append('EmergencyMobile', $("#EmergencyMobile").textbox("getValue"));
                data.append('LanguageLevel', $("#LanguageLevel").textbox("getValue"));
                data.append('ComputerLevel', $("#ComputerLevel").textbox("getValue"));
                data.append('SelfEvaluation', $("#SelfEvaluation").textbox("getValue"));
                data.append('PositionName', $("#PositionName").textbox("getValue"));
                data.append('Treatment', $("#Treatment").textbox("getValue"));
                //工作经历
                var work = $('#dg').datagrid('getRows');
                var works = [];
                for (var i = 0; i < work.length; i++) {
                    works.push(work[i]);
                }
                data.append('works', JSON.stringify(works));
                //家庭成员
                var family = $('#family').datagrid('getRows');
                var familys = [];
                for (var i = 0; i < family.length; i++) {
                    familys.push(family[i]);
                }
                data.append('familys', JSON.stringify(familys));

                ajaxLoading();
                $.ajax({
                    url: '?action=Submit',
                    type: 'POST',
                    data: data,
                    dataType: 'JSON',
                    cache: false,
                    processData: false,
                    contentType: false,
                    success: function (res) {
                        ajaxLoadEnd();
                        var res = eval(res);
                        if (res.success) {
                            top.$.timeouts.alert({ position: "TC", msg: res.message, type: "success" });
                            $.myWindow.close();
                        }
                        else {
                            top.$.timeouts.alert({ position: "TC", msg: res.message, type: "error" });
                        }
                    }
                })
            })
            $("#btnClose").click(function () {
                $.myWindow.close();
            })
            //身份证号码初始化出生日期
            $("#IDCard").textbox({
                onChange: function () {
                    var idCard = $("#IDCard").textbox('getValue');
                    if (idCard.length == 18) {
                        var year = idCard.substring(6, 10);
                        var month = idCard.substring(10, 12);
                        var day = idCard.substring(12, 14);
                        $("#BirthDate").datebox("setValue", year + "-" + month + "-" + day)
                    }
                }
            })

            Init();
        });
    </script>
    <script>
        //初始化
        function Init() {
            $("#Gender").combobox("setValue", "1");
            $("#Volk").combobox("setValue", "汉族");
            $("#PoliticalOutlook").combobox("setValue", "群众");
            $("#IsMarry").combobox("setValue", "未婚");
            $("#Healthy").combobox("setValue", "优");
            $("#Blood").combobox("setValue", "A型");
            $("#Education").combobox("setValue", "本科");

            //$("#Name").textbox("setValue", "董健");
            //$("#BirthDate").datebox("setValue", "1988-10-2");           
            //$("#IDCard").textbox("setValue", "332527198810026010");
            //$("#NativePlace").area("setValue", "浙江 丽水 遂昌县");
            //$("#PassAddress").area("setValue", "浙江 丽水 遂昌县 王村口镇对正村20号");
            //$("#Height").numberbox("setValue", "176");
            //$("#Weight").numberbox("setValue", "70");
            //$("#Major").textbox("setValue", "自动化");
            //$("#GraduationDate").datebox("setValue", "2014-7-1");
            //$("#GraduatInstitutions").textbox("setValue", "扬州大学");
            //$("#HomeAddress").area("setValue", "江苏 苏州 工业园区 琼姬路60号");
            //$("#Mobile").textbox("setValue", "15051557989");
            //$("#Email").textbox("setValue", "578198163@qq.com");
            //$("#BeginWorkDate").datebox("setValue", "2019-4-10");
        }
    </script>
    <script>
        var editIndex = undefined;
        function endEditing() {
            if (editIndex == undefined) { return true }
            if ($('#dg').datagrid('validateRow', editIndex)) {
                $('#dg').datagrid('endEdit', editIndex);
                editIndex = undefined;
                return true;
            } else {
                return false;
            }
        }
        function onClickRow(index) {
            if (editIndex != index) {
                if (endEditing()) {
                    $('#dg').datagrid('selectRow', index).datagrid('beginEdit', index);
                    editIndex = index;
                } else {
                    $('#dg').datagrid('selectRow', editIndex);
                }
            }
        }
        function OperationWork(val, row, index) {
            return ['<span class="easyui-formatted">',
                , '<a class="easyui-linkbutton"  data-options="iconCls:\'icon-yg-delete\'" onclick="Delete(\'' + index + '\');return false;">删除</a> '
                , '</span>'].join('');
        }
        //删除行
        function Delete(index) {
            if (editIndex != undefined) {
                $('#dg').datagrid('endEdit', editIndex).datagrid('cancelEdit', editIndex);
                editIndex = undefined;
            }
            $('#dg').datagrid('deleteRow', index);
            loadData()
        }
        //添加工作经历
        function AddWork() {
            if (endEditing()) {
                $('#dg').datagrid('appendRow', {
                });
                editIndex = $('#dg').datagrid('getRows').length - 1;
                $('#dg').datagrid('selectRow', editIndex).datagrid('beginEdit', editIndex);
            }
        }
        //重新加载数据，作用：刷新列表操作按钮的样式
        function loadData() {
            var data = $('#dg').datagrid('getData');
            $('#dg').datagrid('loadData', data);
        }
    </script>
    <script>
        var editIndex2 = undefined;
        function endEditing2() {
            if (editIndex2 == undefined) { return true }
            if ($('#family').datagrid('validateRow', editIndex2)) {
                $('#family').datagrid('endEdit', editIndex2);
                editIndex2 = undefined;
                return true;
            } else {
                return false;
            }
        }
        function onClickRow2(index) {
            if (editIndex2 != index) {
                if (endEditing2()) {
                    $('#family').datagrid('selectRow', index).datagrid('beginEdit', index);
                    editIndex2 = index;
                } else {
                    $('#family').datagrid('selectRow', editIndex2);
                }
            }
        }
        function OperationFamily(val, row, index) {
            return ['<span class="easyui-formatted">',
                , '<a class="easyui-linkbutton"  data-options="iconCls:\'icon-yg-delete\'" onclick="Delete2(\'' + index + '\');return false;">删除</a> '
                , '</span>'].join('');
        }
        //删除行
        function Delete2(index) {
            if (editIndex2 != undefined) {
                $('#family').datagrid('endEdit', editIndex2).datagrid('cancelEdit', editIndex2);
                editIndex2 = undefined;
            }
            $('#family').datagrid('deleteRow', index);
            loadData2()
        }
        //添加工作经历
        function AddFamily() {
            if (endEditing2()) {
                $('#family').datagrid('appendRow', {
                });
                editIndex2 = $('#family').datagrid('getRows').length - 1;
                $('#family').datagrid('selectRow', editIndex2).datagrid('beginEdit', editIndex2);
            }
        }
        //重新加载数据，作用：刷新列表操作按钮的样式
        function loadData2() {
            var data = $('#family').datagrid('getData');
            $('#family').datagrid('loadData', data);
        }
    </script>
    <style>
        .lbl {
            width: 120px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-layout" style="width: 100%; height: 100%;">
        <div data-options="region:'center'" style="border: none">
            <table id="tab1" class="liebiao">
                <tr>
                    <td class="lbl">姓名：</td>
                    <td>
                        <input id="Name" class="easyui-textbox" style="width: 150px;" data-options="required:true" />
                    </td>
                    <td class="lbl">性别：</td>
                    <td>
                        <input id="Gender" class="easyui-combobox" style="width: 150px;" />
                    </td>
                    <td class="lbl">身份证号：</td>
                    <td colspan="3">
                        <input id="IDCard" class="easyui-textbox" style="width: 150px;" data-options="required:true,validType:'idcard'" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">民族：</td>
                    <td>
                        <input id="Volk" class="easyui-combobox" style="width: 150px;" />
                    </td>
                    <td class="lbl">政治面貌：</td>
                    <td>
                        <input id="PoliticalOutlook" class="easyui-combobox" style="width: 150px;" />
                    </td>
                    <td class="lbl">婚姻状况：</td>
                    <td>
                        <input id="IsMarry" class="easyui-combobox" style="width: 150px;" />
                    </td>
                    <td class="lbl">出生日期：</td>
                    <td>
                        <input id="BirthDate" class="easyui-datebox" style="width: 150px;" data-options="required:true,editable:false,disabled:true" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">身高(cm)：</td>
                    <td>
                        <input id="Height" class="easyui-numberbox" style="width: 150px;" data-options="required:true" />
                    </td>
                    <td class="lbl">体重(kg)：</td>
                    <td>
                        <input id="Weight" class="easyui-numberbox" style="width: 150px;" data-options="required:true" />
                    </td>
                    <td class="lbl">血型：</td>
                    <td>
                        <input id="Blood" class="easyui-combobox" style="width: 150px;" />
                    </td>
                    <td class="lbl">健康状况：</td>
                    <td>
                        <input id="Healthy" class="easyui-combobox" style="width: 150px;" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">籍贯：</td>
                    <td colspan="7">
                        <input id="NativePlace" class="easyui-area" style="width: 150px;" data-options="required:true,country:'中国',newline:false,detailBox:false" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">户口所在地：</td>
                    <td colspan="3">
                        <input id="PassAddress" class="easyui-area" style="width: 150px;" data-options="required:true,country:'中国',newline:true,newlinewidth:360" />
                    </td>
                    <td class="lbl">现居地：</td>
                    <td colspan="3">
                        <input id="HomeAddress" class="easyui-area" style="width: 150px;" data-options="required:true,country:'中国',newline:true,newlinewidth:360" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">毕业院校：</td>
                    <td>
                        <input id="GraduatInstitutions" class="easyui-textbox" style="width: 150px;" data-options="required:true" />
                    </td>
                    <td class="lbl">毕业时间：</td>
                    <td>
                        <input id="GraduationDate" class="easyui-datebox" style="width: 150px;" data-options="required:true,editable:false" />
                    </td>
                    <td class="lbl">学历：</td>
                    <td>
                        <input id="Education" class="easyui-combobox" style="width: 150px;" />
                    </td>
                    <td class="lbl">专业：</td>
                    <td>
                        <input id="Major" class="easyui-textbox" style="width: 150px;" data-options="required:true" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">外语水平：</td>
                    <td>
                        <input id="LanguageLevel" class="easyui-textbox" style="width: 150px;" />
                    </td>
                    <td class="lbl">计算机水平：</td>
                    <td>
                        <input id="ComputerLevel" class="easyui-textbox" style="width: 150px;" />
                    </td>
                    <td class="lbl">邮箱：</td>
                    <td>
                        <input id="Email" class="easyui-textbox" style="width: 150px;" data-options="required:true,validType:'email'" />
                    </td>
                    <td class="lbl">联系电话：</td>
                    <td>
                        <input id="Mobile" class="easyui-textbox" style="width: 150px;" data-options="required:true,validType:'phoneNum'" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">紧急联系人：</td>
                    <td>
                        <input id="EmergencyContact" class="easyui-textbox" style="width: 150px;" />
                    </td>
                    <td class="lbl">紧急联系人电话：</td>
                    <td colspan="5">
                        <input id="EmergencyMobile" class="easyui-textbox" style="width: 150px;" data-options="validType:'phoneNum'" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">自我评价：</td>
                    <td colspan="7">
                        <input id="SelfEvaluation" class="easyui-textbox" style="width: 510px; height: 60px"
                            data-options="required:true,multiline:true" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">应聘岗位：</td>
                    <td>
                        <input id="PositionName" class="easyui-textbox" style="width: 150px;" data-options="required:true" />
                    </td>
                    <td class="lbl">待遇需求：</td>
                    <td>
                        <input id="Treatment" class="easyui-textbox" style="width: 150px;" data-options="required:true" />
                    </td>
                    <td class="lbl">参加工作日期：</td>
                    <td colspan="3">
                        <input id="BeginWorkDate" class="easyui-datebox" style="width: 150px;" data-options="prompt:'必须有交社保',required:true,editable:false" />
                    </td>
                </tr>
            </table>
            <table id="dg" title="工作简历/社会实践">
            </table>
            <table id="family" title="家庭主要成员">
            </table>
            <div id="topper" style="padding: 5px">
                <a id="btnAddWorkExperience" class="easyui-linkbutton" iconcls="icon-yg-add">添加</a>
            </div>
            <div id="topperfamily" style="padding: 5px">
                <a id="btnAddFamily" class="easyui-linkbutton" iconcls="icon-yg-add">添加</a>
            </div>
        </div>
        <div data-options="region:'south',height:40" style="background-color: #f5f5f5">
            <div style="text-align:center;margin-top:8px;">
                <a id="btnSubmit" class="easyui-linkbutton" iconcls="icon-ok" style="font-weight:700;width:80px;color:green">提交</a>
            </div>
        </div>
    </div>
</asp:Content>
