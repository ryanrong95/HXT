<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm_KQ.Application_Recruit.Edit" %>

<%@ Import Namespace="Yahv.Erm.Services" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script src="../../Content/Script/file.js"></script>
    <script>
        var ID = getQueryString("ID");
        var BussnessTripRequirement = <%=Yahv.Erm.Services.BussnessTripRequirement.NotNeed.GetHashCode()%>;
        var EmergentRequirement = <%=Yahv.Erm.Services.EmergentRequirement.NotEmergent.GetHashCode()%>;
        var GenderRequirement = <%=Yahv.Erm.Services.GenderRequirement.Male.GetHashCode()%>;
        var EducationRequirement = <%=Yahv.Erm.Services.EducationRequirement.CollegeDegree.GetHashCode()%>;

        $(function () {
            $("#Department").combobox({
                required: true,
                valueField: 'Value',
                textField: 'Text',
                data: model.DepartmentType,
            })
            //各种需求
            BussnessTripRequirement();
            EmergentRequirement();
            GenderRequirement();
            EducationRequirementMethod();
            //审批日志
            $("#logs").myDatagrid({
                fitColumns: true,
                fit: false,
                pagination: false,
                actionName: 'LoadLogs',
            });
            //提交
            $("#btnSubmit").click(function () {
                //验证必填项
                var isValid = $('#form1').form('enableValidation').form('validate');
                if (!isValid) {
                    return false;
                }
                var data = new FormData();
                //基本信息
                data.append('ID', ID);
                data.append('Department', $("#Department").combobox("getValue"));
                data.append('PostionName', $("#PostionName").textbox("getValue"));
                data.append('WorkAddress', $("#WorkAddress").textbox("getValue"));
                data.append('NumberOfNeeds', $("#NumberOfNeeds").numberbox("getValue"));
                data.append('NumberOfPositions', $("#NumberOfPositions").numberbox("getValue"));
                data.append('NumberOfNow', $("#NumberOfNow").numberbox("getValue"));
                data.append('NumberOfRecruiters', $("#NumberOfRecruiters").numberbox("getValue"));
                data.append('PeriodSalary', $("#PeriodSalary").numberbox("getValue"));
                data.append('NormalSalary', $("#NormalSalary").numberbox("getValue"));
                data.append('ExpectedArrivalTime', $("#ExpectedArrivalTime").datebox("getValue"));
                //招聘途经
                data.append('SocialRecruitment', $('#SocialRecruitment').checkbox('options').checked);
                data.append('CampusRecruitment', $('#CampusRecruitment').checkbox('options').checked);
                data.append('InternalTransfer', $('#InternalTransfer').checkbox('options').checked);
                data.append('OtherWay', $('#OtherWay').checkbox('options').checked);
                //岗位需求
                data.append('LeaveSupplement', $('#LeaveSupplement').checkbox('options').checked);
                data.append('CoordinateSupplement', $('#CoordinateSupplement').checkbox('options').checked);
                data.append('PostAddition', $('#PostAddition').checkbox('options').checked);
                data.append('PostExpansion', $('#PostExpansion').checkbox('options').checked);
                //其它需求
                data.append('BussnessTripRequirement', BussnessTripRequirement);
                data.append('EmergentRequirement', EmergentRequirement);
                data.append('GenderRequirement', GenderRequirement);
                data.append('EducationRequirement', EducationRequirement);
                data.append('AgeRequirement', $("#AgeRequirement").textbox("getValue"));
                data.append('MajorRequirement', $("#MajorRequirement").textbox("getValue"));
                data.append('ExperienceRequirement', $("#ExperienceRequirement").textbox("getValue"));
                data.append('OtherRequirement', $("#OtherRequirement").textbox("getValue"));
                data.append('PositionDescription', $("#PositionDescription").textbox("getValue"));

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
            //保存草稿
            $("#btnSave").click(function () {
                //验证必填项
                var isValid = $('#form1').form('enableValidation').form('validate');
                if (!isValid) {
                    return false;
                }
                var data = new FormData();
                //基本信息
                data.append('Department', $("#Department").combobox("getValue"));
                data.append('PostionName', $("#PostionName").textbox("getValue"));
                data.append('WorkAddress', $("#WorkAddress").textbox("getValue"));
                data.append('NumberOfNeeds', $("#NumberOfNeeds").numberbox("getValue"));
                data.append('NumberOfPositions', $("#NumberOfPositions").numberbox("getValue"));
                data.append('NumberOfNow', $("#NumberOfNow").numberbox("getValue"));
                data.append('NumberOfRecruiters', $("#NumberOfRecruiters").numberbox("getValue"));
                data.append('PeriodSalary', $("#PeriodSalary").numberbox("getValue"));
                data.append('NormalSalary', $("#NormalSalary").numberbox("getValue"));
                data.append('ExpectedArrivalTime', $("#ExpectedArrivalTime").datebox("getValue"));
                //招聘途经
                data.append('SocialRecruitment', $('#SocialRecruitment').checkbox('options').checked);
                data.append('CampusRecruitment', $('#CampusRecruitment').checkbox('options').checked);
                data.append('InternalTransfer', $('#InternalTransfer').checkbox('options').checked);
                data.append('OtherWay', $('#OtherWay').checkbox('options').checked);
                //岗位需求
                data.append('LeaveSupplement', $('#LeaveSupplement').checkbox('options').checked);
                data.append('CoordinateSupplement', $('#CoordinateSupplement').checkbox('options').checked);
                data.append('PostAddition', $('#PostAddition').checkbox('options').checked);
                data.append('PostExpansion', $('#PostExpansion').checkbox('options').checked);
                //其它需求
                data.append('BussnessTripRequirement', BussnessTripRequirement);
                data.append('EmergentRequirement', EmergentRequirement);
                data.append('GenderRequirement', GenderRequirement);
                data.append('EducationRequirement', EducationRequirement);
                data.append('AgeRequirement', $("#AgeRequirement").textbox("getValue"));
                data.append('MajorRequirement', $("#MajorRequirement").textbox("getValue"));
                data.append('ExperienceRequirement', $("#ExperienceRequirement").textbox("getValue"));
                data.append('OtherRequirement', $("#OtherRequirement").textbox("getValue"));
                data.append('PositionDescription', $("#PositionDescription").textbox("getValue"));

                ajaxLoading();
                $.ajax({
                    url: '?action=SaveDraft',
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
            //关闭
            $("#btnClose").click(function () {
                $.myWindow.close();
            }) 
            
            //初始化
            Init();
        });
    </script>
    <script>
        //初始化申请
        function Init() {
            if (model.ApplicationData != null) {
                $("#Department").combobox("setValue",model.ApplicationData.DepartmentCode)
                $("#PostionName").textbox("setValue",model.ApplicationData.PostionName)
                $("#WorkAddress").textbox("setValue",model.ApplicationData.WorkAddress)
                $("#NumberOfNeeds").numberbox("setValue",model.ApplicationData.NumberOfNeeds)
                $("#NumberOfPositions").numberbox("setValue",model.ApplicationData.NumberOfPositions)
                $("#NumberOfNow").numberbox("setValue",model.ApplicationData.NumberOfNow)
                $("#NumberOfRecruiters").numberbox("setValue",model.ApplicationData.NumberOfRecruiters)

                $("#PeriodSalary").numberbox("setValue",model.ApplicationData.PeriodSalary)
                $("#NormalSalary").numberbox("setValue",model.ApplicationData.NormalSalary)
                $("#ExpectedArrivalTime").datebox("setValue",model.ApplicationData.ExpectedArrivalTime)

                $("#AgeRequirement").textbox("setValue",model.ApplicationData.AgeRequirement)
                $("#MajorRequirement").textbox("setValue",model.ApplicationData.MajorRequirement)
                $("#ExperienceRequirement").textbox("setValue",model.ApplicationData.ExperienceRequirement)
                $("#OtherRequirement").textbox("setValue",model.ApplicationData.OtherRequirement)
                $("#PositionDescription").textbox("setValue",model.ApplicationData.PositionDescription)

                $('#SocialRecruitment').checkbox({checked:model.ApplicationData.SocialRecruitment});
                $('#CampusRecruitment').checkbox({checked:model.ApplicationData.CampusRecruitment});
                $('#InternalTransfer').checkbox({checked:model.ApplicationData.InternalTransfer});
                $('#OtherWay').checkbox({checked:model.ApplicationData.OtherWay});

                $('#LeaveSupplement').checkbox({checked:model.ApplicationData.LeaveSupplement});
                $('#CoordinateSupplement').checkbox({checked:model.ApplicationData.CoordinateSupplement});
                $('#PostAddition').checkbox({checked:model.ApplicationData.PostAddition});
                $('#PostExpansion').checkbox({checked:model.ApplicationData.PostExpansion});

                InitRequirement();
            }
            else {
                if(model.ManageData != null){
                    $("#Department").combobox('setValue', model.ManageData.Department);
                }
                $("#btnSave").css("display", "inline-block");
            }
        }
    </script>
    <script>
        function BussnessTripRequirement(){
            $("#NotNeed").radiobutton({
                onChange:function(checked){
                    if(checked){
                        BussnessTripRequirement = <%=Yahv.Erm.Services.BussnessTripRequirement.NotNeed.GetHashCode()%>;
                    }
                }
            })
            $("#NotOften").radiobutton({
                onChange:function(checked){
                    if(checked){
                        BussnessTripRequirement = <%=Yahv.Erm.Services.BussnessTripRequirement.NotOften.GetHashCode()%>;
                    }
                }
            })
            $("#Often").radiobutton({
                onChange:function(checked){
                    if(checked){
                        BussnessTripRequirement = <%=Yahv.Erm.Services.BussnessTripRequirement.Often.GetHashCode()%>;
                    }
                }
            })
        }

        function EmergentRequirement(){
            $("#NotEmergent").radiobutton({
                onChange:function(checked){
                    if(checked){
                        EmergentRequirement = <%=Yahv.Erm.Services.EmergentRequirement.NotEmergent.GetHashCode()%>;
                    }
                }
            })
            $("#GeneralEmergent").radiobutton({
                onChange:function(checked){
                    if(checked){
                        EmergentRequirement = <%=Yahv.Erm.Services.EmergentRequirement.GeneralEmergent.GetHashCode()%>;
                    }
                }
            })
            $("#Emergent").radiobutton({
                onChange:function(checked){
                    if(checked){
                        EmergentRequirement = <%=Yahv.Erm.Services.EmergentRequirement.Emergent.GetHashCode()%>;
                    }
                }
            })
        }

        function GenderRequirement(){
            $("#Male").radiobutton({
                onChange:function(checked){
                    if(checked){
                        GenderRequirement = <%=Yahv.Erm.Services.GenderRequirement.Male.GetHashCode()%>;
                    }
                }
            })
            $("#Female").radiobutton({
                onChange:function(checked){
                    if(checked){
                        GenderRequirement = <%=Yahv.Erm.Services.GenderRequirement.Female.GetHashCode()%>;
                    }
                }
            })
            $("#MaleOrFemale").radiobutton({
                onChange:function(checked){
                    if(checked){
                        GenderRequirement = <%=Yahv.Erm.Services.GenderRequirement.MaleOrFemale.GetHashCode()%>;
                    }
                }
            })
        }

        function EducationRequirementMethod(){
            $("#CollegeDegree").radiobutton({
                onChange:function(checked){
                    if(checked){
                        EducationRequirement = <%=Yahv.Erm.Services.EducationRequirement.CollegeDegree.GetHashCode()%>;
                    }
                }
            })
            $("#BachelorDegree").radiobutton({
                onChange:function(checked){
                    if(checked){
                        EducationRequirement = <%=Yahv.Erm.Services.EducationRequirement.BachelorDegree.GetHashCode()%>;
                    }
                }
            })
            $("#MasterDegree").radiobutton({
                onChange:function(checked){
                    if(checked){
                        EducationRequirement = <%=Yahv.Erm.Services.EducationRequirement.MasterDegree.GetHashCode()%>;
                    }
                }
            })
            $("#OtherDegree").radiobutton({
                onChange:function(checked){
                    if(checked){
                        EducationRequirement = <%=Yahv.Erm.Services.EducationRequirement.OtherDegree.GetHashCode()%>;
                    }
                }
            })
        }

        function InitRequirement() {
                var BussnessTripRequirement = model.ApplicationData.BussnessTripRequirement
                var EmergentRequirement = model.ApplicationData.EmergentRequirement
                var GenderRequirement = model.ApplicationData.GenderRequirement
                var EducationRequirements = model.ApplicationData.EducationRequirements
                //出差需求
                if(BussnessTripRequirement == '<%=Yahv.Erm.Services.BussnessTripRequirement.NotNeed.GetHashCode()%>'){
                    $('#NotNeed').radiobutton({ checked:true});  
                }else if(BussnessTripRequirement == '<%=Yahv.Erm.Services.BussnessTripRequirement.NotNeed.GetHashCode()%>'){
                    $('#NotOften').radiobutton({ checked:true});  
                }else{
                    $('#Often').radiobutton({ checked:true});  
                }
                //需求等级
                if(EmergentRequirement == '<%=Yahv.Erm.Services.EmergentRequirement.NotEmergent.GetHashCode()%>'){
                    $('#NotEmergent').radiobutton({ checked:true});  
                }else if(EmergentRequirement == '<%=Yahv.Erm.Services.EmergentRequirement.GeneralEmergent.GetHashCode()%>'){
                    $('#GeneralEmergent').radiobutton({ checked:true});  
                }else{
                    $('#Emergent').radiobutton({ checked:true});  
                }
                //需求等级
                if(GenderRequirement == '<%=Yahv.Erm.Services.GenderRequirement.Male.GetHashCode()%>'){
                    $('#Male').radiobutton({ checked:true});  
                }else if(GenderRequirement == '<%=Yahv.Erm.Services.GenderRequirement.Female.GetHashCode()%>'){
                    $('#Female').radiobutton({ checked:true});  
                }else{
                    $('#MaleOrFemale').radiobutton({ checked:true});  
                }
                //需求等级
                if(EducationRequirements == '<%=Yahv.Erm.Services.EducationRequirement.CollegeDegree.GetHashCode()%>'){
                    $('#CollegeDegree').radiobutton({ checked:true});  
                }else if(EducationRequirements == '<%=Yahv.Erm.Services.EducationRequirement.BachelorDegree.GetHashCode()%>'){
                    $('#BachelorDegree').radiobutton({ checked:true});  
                }else if(EducationRequirements == '<%=Yahv.Erm.Services.EducationRequirement.MasterDegree.GetHashCode()%>'){
                    $('#MasterDegree').radiobutton({ checked:true});  
                }else{
                    $('#OtherDegree').radiobutton({ checked:true});  
                }
            }
    
    </script>
    <style>
        .lbl {
            width: 110px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-layout" style="width: 100%; height: 100%; border: none">
        <div data-options="region:'center'" style="border: none">
            <table id="tab1" class="liebiao">
                <tr>
                    <td class="lbl">申请部门：</td>
                    <td>
                        <input id="Department" class="easyui-combobox" style="width: 200px;" data-options="required: true" />
                    </td>
                    <td class="lbl">岗位名称：</td>
                    <td>
                        <input id="PostionName" class="easyui-textbox" style="width: 200px;" data-options="required: true" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">需求人数：</td>
                    <td>
                        <input id="NumberOfNeeds" class="easyui-numberbox" style="width: 200px;" data-options="required: true" />
                    </td>
                    <td class="lbl">工作地点：</td>
                    <td>
                        <input id="WorkAddress" class="easyui-textbox" style="width: 200px;" data-options="required: true" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">现有岗位数量：</td>
                    <td>
                        <input id="NumberOfPositions" class="easyui-numberbox" style="width: 200px;" data-options="required: true" />
                    </td>
                    <td class="lbl">现有在岗人数：</td>
                    <td>
                        <input id="NumberOfNow" class="easyui-numberbox" style="width: 200px;" data-options="required: true" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">拟招聘人数：</td>
                    <td colspan="3">
                        <input id="NumberOfRecruiters" class="easyui-numberbox" style="width: 200px;" data-options="required: true" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">招聘途经：</td>
                    <td>
                        <input id="SocialRecruitment" name="SocialRecruitment" class="easyui-checkbox" data-options="label:'社会招聘',labelPosition:'after'">
                        <input id="CampusRecruitment" name="CampusRecruitment" class="easyui-checkbox" data-options="label:'校园招聘',labelPosition:'after'">
                        <input id="InternalTransfer" name="InternalTransfer" class="easyui-checkbox" data-options="label:'内部调动',labelPosition:'after'">
                        <input id="OtherWay" name="OtherWay" class="easyui-checkbox" data-options="label:'其它',labelPosition:'after'">
                    </td>
                    <td class="lbl">岗位需求：</td>
                    <td>
                        <input id="LeaveSupplement" name="LeaveSupplement" class="easyui-checkbox" data-options="label:'离职补充',labelPosition:'after'">
                        <input id="CoordinateSupplement" name="CoordinateSupplement" class="easyui-checkbox" data-options="label:'调协补充',labelPosition:'after'">
                        <input id="PostAddition" name="PostAddition" class="easyui-checkbox" data-options="label:'岗位新增',labelPosition:'after'">
                        <input id="PostExpansion" name="PostExpansion" class="easyui-checkbox" data-options="label:'岗位扩员',labelPosition:'after'">
                    </td>
                </tr>
                <tr>
                    <td class="lbl">出差需求：</td>
                    <td>
                        <input class="easyui-radiobutton" id="NotNeed" name="BusinessTripRequirement" data-options="labelPosition:'after',checked: true,label:'不需要'">
                        <input class="easyui-radiobutton" id="NotOften" name="BusinessTripRequirement" data-options="labelPosition:'after',label:'偶尔'">
                        <input class="easyui-radiobutton" id="Often" name="BusinessTripRequirement" data-options="labelPosition:'after',label:'经常'">
                    </td>
                    <td class="lbl">需求等级：</td>
                    <td>
                        <input class="easyui-radiobutton" id="NotEmergent" name="EmergentRequirement" data-options="labelPosition:'after',checked: true,label:'不紧急'">
                        <input class="easyui-radiobutton" id="GeneralEmergent" name="EmergentRequirement" data-options="labelPosition:'after',label:'一般紧急'">
                        <input class="easyui-radiobutton" id="Emergent" name="EmergentRequirement" data-options="labelPosition:'after',label:'紧急'">
                    </td>
                </tr>
                <tr>
                    <td class="lbl">试用期薪资：</td>
                    <td>
                        <input id="PeriodSalary" class="easyui-numberbox" style="width: 200px;" data-options="required: true" />
                    </td>
                    <td class="lbl">转正后薪资：</td>
                    <td>
                        <input id="NormalSalary" class="easyui-numberbox" style="width: 200px;" data-options="required: true" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">期望到岗时间：</td>
                    <td colspan="3">
                        <input id="ExpectedArrivalTime" class="easyui-datebox" style="width: 200px;" data-options="required: true" />
                    </td>
                </tr>
            </table>
            <table id="tab2" class="liebiao">
                <tr style="background-color: #eeeeee">
                    <td class="lbl" colspan="4">岗位任职要求</td>
                </tr>
                <tr>
                    <td class="lbl">性别要求：</td>
                    <td>
                        <div style="width: 400px">
                            <input class="easyui-radiobutton" id="Male" name="GenderRequirement" data-options="labelPosition:'after',checked: true,label:'男'">
                            <input class="easyui-radiobutton" id="Female" name="GenderRequirement" data-options="labelPosition:'after',label:'女'">
                            <input class="easyui-radiobutton" id="MaleOrFemale" name="GenderRequirement" data-options="labelPosition:'after',label:'不限'">
                        </div>
                    </td>
                    <td class="lbl">学历要求：</td>
                    <td>
                        <div style="width: 400px">
                            <input class="easyui-radiobutton" id="CollegeDegree" name="EducationRequirement" data-options="labelPosition:'after',checked: true,label:'大专及以上'">
                            <input class="easyui-radiobutton" id="BachelorDegree" name="EducationRequirement" data-options="labelPosition:'after',label:'本科及以上'">
                            <input class="easyui-radiobutton" id="MasterDegree" name="EducationRequirement" data-options="labelPosition:'after',label:'硕士及以上'">
                            <input class="easyui-radiobutton" id="OtherDegree" name="EducationRequirement" data-options="labelPosition:'after',label:'其它'">
                        </div>
                    </td>
                </tr>
                <tr>
                    <td class="lbl">年龄要求：</td>
                    <td>
                        <input id="AgeRequirement" class="easyui-textbox" style="width: 200px;" />
                    </td>
                    <td class="lbl">专业要求：</td>
                    <td>
                        <input id="MajorRequirement" class="easyui-textbox" style="width: 200px;" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">经验要求：</td>
                    <td>
                        <input id="ExperienceRequirement" class="easyui-textbox" style="width: 200px;" />
                    </td>
                    <td class="lbl">其他要求：</td>
                    <td>
                        <input id="OtherRequirement" class="easyui-textbox" style="width: 200px;" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">岗位职责：</td>
                    <td colspan="3">
                        <input id="PositionDescription" class="easyui-textbox" style="width: 600px; height: 50px"
                            data-options="multiline:true,required: true" />
                    </td>
                </tr>
            </table>
            <table id="logs" title="审批日志">
                <thead>
                    <tr>
                        <th data-options="field:'CreateDate',align:'left'" style="width: 80px">审批时间</th>
                        <th data-options="field:'VoteStepName',align:'left'" style="width: 50px">审批步骤</th>
                        <th data-options="field:'AdminID',align:'left'" style="width: 50px">审批人</th>
                        <th data-options="field:'Status',align:'left'" style="width: 50px;">审批结果</th>
                        <th data-options="field:'Summary',align:'left'" style="width: 200px">审批意见</th>
                    </tr>
                </thead>
            </table>
        </div>
        <div data-options="region:'south',height:40" style="background-color: #f5f5f5">
            <div style="float: right; margin-right: 5px; margin-top: 8px;">
                <a id="btnSubmit" class="easyui-linkbutton" iconcls="icon-yg-confirm">提交</a>
                <a id="btnSave" class="easyui-linkbutton" iconcls="icon-yg-save" style="display: none">保存草稿</a>
                <a id="btnClose" class="easyui-linkbutton" iconcls="icon-yg-cancel">关闭</a>
            </div>
        </div>
    </div>
</asp:Content>
