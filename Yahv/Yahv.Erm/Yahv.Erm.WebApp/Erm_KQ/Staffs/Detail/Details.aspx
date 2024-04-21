<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Details.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm_KQ.Staffs.Detail.Details" %>

<%@ Import Namespace="Yahv.Erm.Services" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script src="http://gerpfixed.for-ic.net/My/Scripts/area.data.js"></script>
    <script src="http://gerpfixed.for-ic.net/My/Scripts/areacombo.js"></script>
    <script>
        var StaffID = getQueryString("ID");
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
                fitColumns: false,
                fit: false,
                singleSelect: true,
                pagination: false,
                actionName: 'LoadWork',
                columns: [[
                    { field: 'StartTime', title: '开始日期', width: 120, align: 'center', editor: { type: 'datebox', options: { required: true } } },
                    { field: 'EndTime', title: '结束日期', width: 120, align: 'center', editor: { type: 'datebox', options: { required: true } } },
                    { field: 'Company', title: '工作单位', width: 200, align: 'center', editor: { type: 'textbox', options: { required: true } } },
                    { field: 'Position', title: '职务名称', width: 150, align: 'center', editor: { type: 'textbox', options: { required: true } } },
                    { field: 'Salary', title: '薪资', width: 100, align: 'center', editor: { type: 'numberbox', options: { required: true } } },
                    { field: 'LeaveReason', title: '离职原因', width: 250, align: 'center', editor: { type: 'textbox', options: { required: true } } },
                    { field: 'Phone', title: '单位电话', width: 120, align: 'center', editor: { type: 'textbox', options: { required: false } } },
                ]],
            });
            //家庭成员
            $("#family").myDatagrid({
                fitColumns: false,
                fit: false,
                singleSelect: true,
                pagination: false,
                actionName: 'LoadFamily',
                columns: [[
                    { field: 'Name', title: '姓名', width: 100, align: 'center', editor: { type: 'textbox', options: { required: true } } },
                    { field: 'Relation', title: '与本人关系', width: 100, align: 'center', editor: { type: 'textbox', options: { required: true } } },
                    { field: 'Age', title: '年龄', width: 100, align: 'center', editor: { type: 'numberbox', options: { required: true } } },
                    { field: 'Company', title: '工作单位', width: 200, align: 'center', editor: { type: 'textbox', options: { required: true } } },
                    { field: 'Position', title: '职业', width: 150, align: 'center', editor: { type: 'textbox', options: { required: true } } },
                    { field: 'Phone', title: '手机号码', width: 150, align: 'center', editor: { type: 'textbox', options: { required: true, validType: 'phoneNum' } } },
                ]],
            });
            Init();
        });
    </script>
    <script>
        //初始化
        function Init() {
            $("#Name").textbox("setValue", model.StaffData.Name);
            $("#Gender").combobox("setValue", model.StaffData.Gender);
            $("#BirthDate").datebox("setValue", model.StaffData.BirthDate);
            $("#Volk").combobox("setValue", model.StaffData.Volk);
            $("#PoliticalOutlook").combobox("setValue", model.StaffData.PoliticalOutlook);
            $("#IsMarry").combobox("setValue", model.StaffData.IsMarry);
            $("#Healthy").combobox("setValue", model.StaffData.Healthy);
            $("#IDCard").textbox("setValue", model.StaffData.IDCard);
            $("#NativePlace").area("setValue", model.StaffData.NativePlace);
            $("#PassAddress").area("setValue", model.StaffData.PassAddress);
            $("#Blood").combobox("setValue", model.StaffData.Blood);
            $("#Height").numberbox("setValue", model.StaffData.Height);
            $("#Weight").numberbox("setValue", model.StaffData.Weight);
            $("#Education").combobox("setValue", model.StaffData.Education);
            $("#Major").textbox("setValue", model.StaffData.Major);
            $("#GraduationDate").datebox("setValue", model.StaffData.GraduationDate);
            $("#GraduatInstitutions").textbox("setValue", model.StaffData.GraduatInstitutions);
            $("#HomeAddress").area("setValue", model.StaffData.HomeAddress);
            $("#Mobile").textbox("setValue", model.StaffData.Mobile);
            $("#Email").textbox("setValue", model.StaffData.Email);
            $("#BeginWorkDate").datebox("setValue", model.StaffData.BeginWorkDate);
            $("#EmergencyContact").textbox("setValue", model.StaffData.EmergencyContact);
            $("#EmergencyMobile").textbox("setValue", model.StaffData.EmergencyMobile);

            $("#SelfEvaluation").textbox("setValue", model.StaffData.SelfEvaluation);
            $("#LanguageLevel").textbox("setValue", model.StaffData.LanguageLevel);
            $("#ComputerLevel").textbox("setValue", model.StaffData.ComputerLevel);
            $("#Treatment").textbox("setValue", model.StaffData.Treatment);
            $("#PositionName").textbox("setValue", model.StaffData.PositionName);
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
                        <input id="Name" class="easyui-textbox" style="width: 180px;" data-options="required:true" />
                    </td>
                    <td class="lbl">性别：</td>
                    <td>
                        <input id="Gender" class="easyui-combobox" style="width: 180px;" />
                    </td>
                    <td class="lbl">身份证号：</td>
                    <td colspan="3">
                        <input id="IDCard" class="easyui-textbox" style="width: 180px;" data-options="required:true,validType:'idcard'" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">民族：</td>
                    <td>
                        <input id="Volk" class="easyui-combobox" style="width: 180px;" />
                    </td>
                    <td class="lbl">政治面貌：</td>
                    <td>
                        <input id="PoliticalOutlook" class="easyui-combobox" style="width: 180px;" />
                    </td>
                    <td class="lbl">婚姻状况：</td>
                    <td>
                        <input id="IsMarry" class="easyui-combobox" style="width: 180px;" />
                    </td>
                    <td class="lbl">出生日期：</td>
                    <td>
                        <input id="BirthDate" class="easyui-datebox" style="width: 180px;" data-options="required:true" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">身高(cm)：</td>
                    <td>
                        <input id="Height" class="easyui-numberbox" style="width: 180px;" data-options="required:true" />
                    </td>
                    <td class="lbl">体重(kg)：</td>
                    <td>
                        <input id="Weight" class="easyui-numberbox" style="width: 180px;" data-options="required:true" />
                    </td>
                    <td class="lbl">血型：</td>
                    <td>
                        <input id="Blood" class="easyui-combobox" style="width: 180px;" />
                    </td>
                    <td class="lbl">健康状况：</td>
                    <td>
                        <input id="Healthy" class="easyui-combobox" style="width: 180px;" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">籍贯：</td>
                    <td colspan="7">
                        <input id="NativePlace" class="easyui-area" style="width: 180px;" data-options="required:true,country:'中国',newline:false,detailBox:false" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">户口所在地：</td>
                    <td colspan="3">
                        <input id="PassAddress" class="easyui-area" style="width: 180px;" data-options="required:true,country:'中国',newline:true,newlinewidth:360" />
                    </td>
                    <td class="lbl">现居地：</td>
                    <td colspan="3">
                        <input id="HomeAddress" class="easyui-area" style="width: 180px;" data-options="required:true,country:'中国',newline:true,newlinewidth:360" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">毕业院校：</td>
                    <td>
                        <input id="GraduatInstitutions" class="easyui-textbox" style="width: 180px;" data-options="required:true" />
                    </td>
                    <td class="lbl">毕业时间：</td>
                    <td>
                        <input id="GraduationDate" class="easyui-datebox" style="width: 180px;" data-options="required:true" />
                    </td>
                    <td class="lbl">学历：</td>
                    <td>
                        <input id="Education" class="easyui-combobox" style="width: 180px;" />
                    </td>
                    <td class="lbl">专业：</td>
                    <td>
                        <input id="Major" class="easyui-textbox" style="width: 180px;" data-options="required:true" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">外语水平：</td>
                    <td>
                        <input id="LanguageLevel" class="easyui-textbox" style="width: 180px;" />
                    </td>
                    <td class="lbl">计算机水平：</td>
                    <td>
                        <input id="ComputerLevel" class="easyui-textbox" style="width: 180px;" />
                    </td>
                    <td class="lbl">邮箱：</td>
                    <td>
                        <input id="Email" class="easyui-textbox" style="width: 180px;" data-options="required:true,validType:'email'" />
                    </td>
                    <td class="lbl">联系电话：</td>
                    <td>
                        <input id="Mobile" class="easyui-textbox" style="width: 180px;" data-options="required:true,validType:'phoneNum'" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">紧急联系人：</td>
                    <td>
                        <input id="EmergencyContact" class="easyui-textbox" style="width: 180px;" />
                    </td>
                    <td class="lbl">紧急联系人电话：</td>
                    <td colspan="5">
                        <input id="EmergencyMobile" class="easyui-textbox" style="width: 180px;" data-options="validType:'phoneNum'" />
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
                        <input id="PositionName" class="easyui-textbox" style="width: 180px;" data-options="required:true" />
                    </td>
                    <td class="lbl">待遇需求：</td>
                    <td>
                        <input id="Treatment" class="easyui-textbox" style="width: 180px;" data-options="required:true" />
                    </td>
                    <td class="lbl">参加工作日期：</td>
                    <td colspan="3">
                        <input id="BeginWorkDate" class="easyui-datebox" style="width: 180px;" data-options="prompt:'必须有交社保',required:true" />
                    </td>
                </tr>
            </table>
            <table id="dg" title="工作简历/社会实践">
            </table>
            <table id="family" title="家庭主要成员">
            </table>
        </div>
    </div>
</asp:Content>
