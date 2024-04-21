<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works_ncs.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm.Staffs.Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        var PostionData = eval('(<%=this.Model.PostionData%>)');
        var CityData = eval('(<%=this.Model.CityData%>)');
        var Status = eval('(<%=this.Model.Status%>)');
        var CompaniesDate = eval('(<%=this.Model.CompaniesDate%>)');
        $(function () {
            $('#Code').textbox('readonly');
            //绑定下拉框
            $('#PostionID').combobox({
                data: PostionData,
            })
            $('#WorkCity').combobox({
                data: CityData,
            })
            $('#Status').combobox({
                data: Status,
            })
            $('#EnterpriseID').combobox({
                data: CompaniesDate,
            })
        });
    </script>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="cphFormNorth" runat="server">
    <div class="easyui-panel" title="员工明细" data-options="fit:true,border:false">
        <table class="liebiao">
            <tr>
                <td>员工编码：</td>
                <td>
                    <input id="Code" name="Code" class="easyui-textbox" style="width: 200px;"
                        data-options="prompt:'',required:true,validType:'length[1,50]'" />
                </td>
                <td>自定义编码：</td>
                <td>
                    <input id="SelCode" name="SelCode" class="easyui-textbox" style="width: 200px;"
                        data-options="prompt:'',required:true,validType:'length[1,50]'" />
                </td>
            </tr>

            <tr>
                <td>大赢家公司编码：</td>
                <td>
                    <input id="DyjCompanyCode" name="DyjCompanyCode" class="easyui-textbox" style="width: 200px;"
                        data-options="prompt:'',required:true,validType:'length[1,50]'" />
                </td>
                <td>大赢家部门编码：</td>
                <td>
                    <input id="DyjDepartmentCode" name="DyjDepartmentCode" class="easyui-textbox" style="width: 200px;"
                        data-options="prompt:'',required:true,validType:'length[1,50]'" />
                </td>
            </tr>

            <tr>
                <td>姓名：</td>
                <td>
                    <input id="Name" name="Name" class="easyui-textbox" style="width: 200px;"
                        data-options="prompt:'',required:true,validType:'length[1,50]'" />
                </td>
                <td>性别：</td>
                <td>
                    <input type="radio" name="Gender" value="1" />男
                        <input type="radio" name="Gender" value="0" style="margin-left: 10px" />女                       
                </td>
            </tr>

            <tr>
                <td>身份证号：</td>
                <td>
                    <input id="IDCard" name="IDCard" class="easyui-textbox" style="width: 200px;"
                        data-options="prompt:'',required:true,validType:'length[1,50]'" />
                </td>
                <td>肖像照：</td>
                <td></td>
            </tr>

            <tr>
                <td>分配岗位：</td>
                <td>
                    <input id="PostionID" data-options="valueField:'Value',textField:'Text',panelHeight:'160px'" class="easyui-combobox" style="width: 200px" />
                </td>
                <td>所在城市：</td>
                <td>
                    <input id="WorkCity" data-options="valueField:'Value',textField:'Text',panelHeight:'160px'" class="easyui-combobox" style="width: 200px" />
                </td>
            </tr>

            <tr>
                <td>状态：</td>
                <td>
                    <input id="Status" data-options="valueField:'Value',textField:'Text',panelHeight:'160px'" class="easyui-combobox" style="width: 200px" />
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphFormCenter" runat="server">
    <div class="easyui-panel" title="劳资信息" data-options="border:false">
        <div>
            <table class="liebiao">
                <tr>
                    <td>所属公司：</td>
                    <td>
                        <input id="EnterpriseID" data-options="valueField:'Value',textField:'Text',panelHeight:'160px'" class="easyui-combobox" style="width: 200px" />
                    </td>
                </tr>
                <tr>
                    <td>入职时间：</td>
                    <td>
                        <input class="easyui-datebox" id="EntryDate" data-options="width:200px,editable:false" />
                    </td>
                    <td>离职时间：</td>
                    <td>
                        <input class="easyui-datebox" id="LeaveDate" data-options="width:200px,editable:false" />
                    </td>
                </tr>
            </table>
        </div>
    </div>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphFormSouth" runat="server">
    <div class="easyui-panel" title="薪资信息" data-options="border:false">
        <table id="tab1">
            <thead>
                <tr>
                    <th data-options="field:'Code',width:50">基本工资</th>
                    <th data-options="field:'Name',width:50">岗位工资</th>
                    <th data-options="field:'Gender',width:50">考勤基数</th>
                    <th data-options="field:'WorkCity',width:50">出勤率</th>
                    <th data-options="field:'btn',formatter:Operation,width:50">操作</th>
                </tr>
            </thead>
        </table>
        <div class="big-title-container" style="margin-left: 20px; margin-top: 20px;">
            <label style="font-size: 16px; font-weight: 600; color: #323232;">
                历史记录</label>
        </div>
        <table id="tab2">
            <thead>
                <tr>
                    <th data-options="field:'Code',width:50">基本工资</th>
                    <th data-options="field:'Name',width:50">岗位工资</th>
                    <th data-options="field:'Gender',width:50">考勤基数</th>
                    <th data-options="field:'WorkCity',width:50">出勤率</th>
                    <th data-options="field:'OperateDate',width:50">操作日期</th>
                    <th data-options="field:'Operator',width:50">操作人</th>
                </tr>
            </thead>
        </table>
    </div>
    <div style="text-align: center; padding: 5px">
        <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />
    </div>

</asp:Content>
