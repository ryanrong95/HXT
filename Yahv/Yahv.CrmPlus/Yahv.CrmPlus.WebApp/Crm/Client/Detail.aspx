<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" Title="基本信息" AutoEventWireup="true" CodeBehind="Detail.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.Client.Detail" %>

<%@ Import Namespace="Yahv.Underly" %>
<%@ Register Src="~/Uc/PcFiles.ascx" TagPrefix="uc1" TagName="PcFiles" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            if (model.Top10N) {
                $("#btnTop10").hide();
                $("#btnCancelTop10").show();
            } else {
                $("#btnCancelTop10").hide();
                $("#btnTop10").show();
            }
            if (model.EnterPriseStatus == '<%=(int)AuditStatus.Black%>')
            {
                $("#btnBlack").hide();
                $("#btnPublic").hide();
            }

              if (!jQuery.isEmptyObject(model.Licenses)) {
                var msgr = $("#licenseSuccess");
                var ul = $("<ul></ul>");
                for (var index = 0; index < model.Licenses.length; index++) {
                    var item = model.Licenses[index];
                    var li = $("<li><a href='" + item.Url + "' target='_blank'><i class='iconfont icon-wenjian'></i><em>" + item.FileName + "</em> </a></li>");
                    ul.append(li);
                }
                msgr.html(ul);
            };
            if (!jQuery.isEmptyObject(model.Entity)) {
               $("#logo").attr("src", model.LogoFile);
                $("#txtName").text(model.Entity.Name);
                $("#area").text(model.Entity.District);
                $("#Source").text(model.Entity.Source);
                if (model.Entity.ClientTypeValue == '<%=Yahv.Underly.CrmPlus.ClientType.University.GetHashCode()%>') {

                    $(".domestic").hide();
                    $(".IsInternation").hide();
                    $(".university").show();


                }
                else {
                    if (!model.Entity.IsInternational) {
                        $(".IsInternation").hide();
                        $(".Unicersity").hide();
                        $(".domestic").show();
                    }
                    else {

                        $(".IsInternation").show();
                        $(".Unicersity").hide();
                        $(".domestic").hide();


                    }

                }

                $('#IsInternational').text(model.Entity.IsInternationDes);
                $(".Place").text(model.Entity.Place);
                $(".Currency").text(model.Entity.Currency);
                $(".adderss").text(model.Entity.RegAddress);
                $("#clientType").text(model.Entity.ClientType);
                $("#nature").text(model.Entity.Nature);
                $("#industry").text(model.Entity.Industry);
                $("#Product").text(model.Entity.product);
                $("#Grade").text(model.Entity.ClientGrade);
                $("#Vip").text(model.Entity.Vip);
                $("#IsMajor").text(model.Entity.IsMajor);
                $("#Status").text(model.Entity.Status);
                $("#IsSpecial").text(model.Entity.IsSpecial);
                $("#ProfitRate").text(model.Entity.ProfitRate);
                $("#Owner").text(model.Entity.Owner);
                if (model.Entity.Protector) {

                    $("#IsProtect").text("是");
                    $("#btnProtect").hide();
                    $("#btnCancelProtect").show();
                } else {
                    $("#IsProtect").text("否");
                    $("#btnProtect").show();
                    $("#btnCancelProtect").hide();
                }
                if (model.Entity.EnterPriseStatus == '<%=Yahv.Underly.AuditStatus.Black.GetHashCode()%>') {
                    $("#btnBlack").hide();
                }
                else {
                    $("#btnBlack").show();
                }
                $("#IsSuplier").text(model.Entity.IsSuplier);
                $(".Uscc").text(model.Entity.Uscc);
                $("#Corperation").text(model.Entity.Corperation);
                $("#RegistDate").text(model.Entity.RegistDate);
                $("#Employees").text(model.Entity.Employees);
                $("#RegistCurrency").text(model.Entity.RegistCurrency)
                $("#RegistFund").text(model.Entity.RegistFund);
                $("#RegAddress").text(model.Entity.RegAddress);
                $("#Nature").text(model.Entity.Nature);
                $("#BusinessState").text(model.Entity.BusinessState);
                $("#Website").text(model.Entity.WebSite);

            }
            //方法Start
            $("#btnEdit").click(function () {
                $.myDialog({
                    title: '编辑',
                    url: '../Client/Approvals/Edit.aspx?id=' + model.Entity.ID,
                    width: '60%',
                    height: '80%',
                    onClose: function () {
                        location.reload();
                    }
                });
            });

            $("#btnProtect").click(function () {
                $.messager.confirm('确认', '您确定要申请保护该客户吗？', function (r) {
                    if (r) {
                        $.post('?action=ApplyProtect', { id: model.Entity.ID }, function (data) {
                            if (data.success) {
                                top.$.timeouts.alert({
                                    position: "TC",
                                    msg: "操作成功!",
                                    type: "success"
                                });
                                //location.reload();
                            }
                            else {
                                $.messager.alert('操作提示', data.message );
                            }
                        });
                    }

                })
            });
            //取消保护
            $("#btnCancelProtect").click(function () {
                $.messager.confirm('确认', '您确定要取消保护该客户吗？', function (r) {
                    if (r) {
                        $.post('?action=CancelProtect', { id: model.Entity.ID }, function (data) {
                            if (data.success) {
                                top.$.timeouts.alert({
                                    position: "TC",
                                    msg: "操作成功!",
                                    type: "success"
                                });
                                $("#btnCancelProtect").hide();
                                $("#btnProtect").show();
                            }
                            else {
                                $.messager.alert('操作提示', '操作失败!', data.message);
                            }
                        });
                    }

                })
            });
            //查看保护记录
            $("#btnProtectRecords").click(function () {
                $.myWindow({
                    title: '申请保护记录',
                    url: '../Client/ApprovalRecords/Protecteds.aspx?id=' + model.Entity.ID,
                    width: '60%',
                    height: '80%',
                    //onClose: function () {
                    //    location.reload();
                    //}
                });
            });
            //加入黑名单
            $("#btnBlack").click(function () {
                $.myDialogFuse({
                    title: '加入黑名单',
                    url: '../Client/BlackLists/JoinBlack.aspx?id=' + model.Entity.ID,
                    width: '600px',
                    height: '400px',
                    //onClose: function () {
                    //    window.location.reload();
                    //}
                });
            });
            //加入公海
            $("#btnPublic").click(function () {
                $.myDialog({
                    title: '加入公海',
                    url: '../Client/PublicSeas/JoinPublicSea.aspx?id=' + model.Entity.ID,
                    width: '650px',
                    height: '450px',
                    onClose: function () {
                        window.location.reload();
                    }
                });
            });
            $("#btnTop10").click(function () {
                $.messager.confirm('确认', '您确定要加入Top10吗？', function (r) {
                    if (r) {
                        $.post('?action=SetTop10', { id: model.Entity.ID }, function (data) {
                            if (data.success) {
                                top.$.timeouts.alert({
                                    position: "TC",
                                    msg: "操作成功!",
                                    type: "success"
                                });
                                //window.location.reload();
                                $("#btnTop10").hide();
                                $("#btnCancelTop10").show();
                            }
                            else {
                                $.messager.alert('操作提示', '操作失败!', data.message);
                            }
                        });
                    }

                })
            });
            //取消Top10
            $("#btnCancelTop10").click(function () {
                $.messager.confirm('确认', '您确定要取消Top10吗？', function (r) {
                    if (r) {
                        $.post('?action=CancelTop10', { id: model.Entity.ID }, function (data) {
                            if (data.success) {
                                top.$.timeouts.alert({
                                    position: "TC",
                                    msg: "操作成功!",
                                    type: "success"
                                });
                                //window.location.reload();
                                $("#btnTop10").show();
                                $("#btnCancelTop10").hide();
                            }
                            else {
                                $.messager.alert('操作提示', '操作失败!', data.message);
                            }
                        });
                    }

                })
            });

            //方法结束---end

        });
    </script>
    <style>
        .tdtitle {
            background-color: #F9FAFC;
            width: 150px;
            height: 30px;
            text-align: right;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div style="margin-top: 5px">
        <div class="tool" style="margin-bottom: 5px">
            <a id="btnEdit" particle="Name:'编辑资料',jField:'btnEdit'" class="easyui-linkbutton" data-options="iconCls:'icon-yg-edit'">编辑资料</a>
            <a id="btnProtect" particle="Name:'申请保护',jField:'btnProtect'" class="easyui-linkbutton" data-options="iconCls:'icon-yg-claim'">申请保护</a>
            <a id="btnCancelProtect" particle="Name:'取消保护',jField:'btnCancelProtect'" class="easyui-linkbutton" data-options="iconCls:'icon-yg-cancel'">取消保护</a>
            <a id="btnProtectRecords" particle="Name:'保护申请记录',jField:'ProtectRecords'" class="easyui-linkbutton" data-options="iconCls:'icon-yg-details'">保护申请记录</a>
            <a id="btnPublic" particle="Name:'加入公海',jField:'btnPublic'" class="easyui-linkbutton" data-options="iconCls:'icon-yg-claim'">加入公海</a>
            <a id="btnBlack" particle="Name:'加入黑名单',jField:'btnBlack'" class="easyui-linkbutton" data-options="iconCls:'icon-yg-blacklist'">加入黑名单</a>
            <a id="btnTop10" particle="Name:'设置Top10',jField:'btnTop10'" class="easyui-linkbutton" data-options="iconCls:'icon-yg-setting'">设置Top10</a>
            <a id="btnCancelTop10" particle="Name:'取消Top10',jField:'btnCancelTop10'" class="easyui-linkbutton" data-options="iconCls:'icon-yg-cancel'">取消Top10</a>
        </div>

        <table class="liebiao">
            <tr>
                <td colspan="6" class="csrmtitle">
                    <p>基本信息</p>
                </td>
            </tr>
            <tr>
                <td colspan="2" rowspan="4">
                    <img src="#" id="logo" style="width: 150px; height: 110px" />
                </td>
                <td class="tdtitle">客户名称：</td>
                <td>
                    <label id="txtName"></label>
                </td>
                <td class="tdtitle">是否国际</td>
                <td>
                    <label id="IsInternational"></label>
                </td>
            </tr>
            <tr>
                <td class="tdtitle">国别地区：</td>
                <td>
                    <label id="area"></label>
                </td>
                <td class="tdtitle">客户来源</td>
                <td>
                    <label id="Source"></label>
                </td>
            </tr>
            <tr>
                <td class="tdtitle">客户类型</td>
                <td>
                    <label id="clientType"></label>
                </td>
                <td class="tdtitle">所属行业 </td>
                <td>
                    <label id="industry"></label>
                </td>
            </tr>
            <tr>
                <td class="tdtitle">主要产品</td>
                <td colspan="3">
                    <label id="Product"></label>
                </td>
            </tr>
            <tr>
                <td class="tdtitle" style="width:50px" >客户等级</td>
                <td style="width:50px">
                    <label id="Grade"></label>
                </td>
                <td class="tdtitle">Vip等级 </td>
                <td>
                    <label id="Vip"></label>
                </td>
            </tr>
            <tr>
                <td class="tdtitle">是否重点</td>
                <td>
                    <label id="IsMajor"></label>
                </td>
                <td class="tdtitle">客户状态</td>
                <td>
                    <label id="Status"></label>
                </td>
                <td class="tdtitle">证照</td>
                <td>
                    <label id="licenseSuccess"></label>
                </td>
            </tr>
            <tr>
                <td class="tdtitle">是否特殊</td>
                <td>
                    <label id="IsSpecial"></label>
                </td>
                <td class="tdtitle">网址</td>
                <td>
                    <label id="Website"></label>
                </td>
                <td class="tdtitle">核定净毛利率</td>
                <td>
                    <label id="ProfitRate"></label>
                </td>
            </tr>
            <tr>
                <td class="tdtitle">是否保护</td>
                <td>
                    <label id="IsProtect"></label>
                </td>
                <td class="tdtitle">保护人</td>
                <td>
                    <label id="Owner"></label>
                </td>
                <td class="tdtitle">是否同为供应商</td>
                <td>
                    <label id="IsSuplier"></label>
                </td>
            </tr>
        </table>
        <table class="liebiao" id="BusinessInfo" style="margin-top:1px">
            <tr>
                <td colspan="6" class="csrmtitle">
                    <p>工商信息</p>
                </td>
            </tr>
            <tr class="domestic">
                <td class="tdtitle">统一社会信用编码</td>
                <td>
                    <label class="Uscc"></label>
                </td>
                <td class="tdtitle">法人代表</td>
                <td>
                    <label id="Corperation"></label>
                </td>
            </tr>
            <tr class="domestic">
                <td class="tdtitle">公司成立日期</td>
                <td>
                    <label id="RegistDate"></label>
                </td>
                <td class="tdtitle">员工人数</td>
                <td>
                    <label id="Employees"></label>
                </td>
            </tr>
            <tr class="domestic">
                <td class="tdtitle">注册币种</td>
                <td>
                    <label id="RegistCurrency"></label>
                </td>
                <td class="tdtitle">注册资金（万）</td>
                <td>
                    <label id="RegistFund"></label>
                </td>
            </tr>
            <tr class="domestic">
                <td class="tdtitle">注册地址</td>
                <td colspan="3">
                    <label id="RegAddress"></label>
                </td>
            </tr>
            <tr class="domestic">
                <td class="tdtitle">公司类型</td>
                <td>
                    <label id="Nature"></label>
                </td>
                <td class="tdtitle">经营状态</td>
                <td>
                    <label id="BusinessState"></label>
                </td>
            </tr>

            <tr class="IsInternation">
                <td class="tdtitle">国家地区</td>
                <td>
                    <label class="Place"></label>
                </td>
                <td class="tdtitle">币种</td>
                <td>
                    <label class="Currency"></label>
                </td>
            </tr>
            <tr class="IsInternation">
                <td class="tdtitle">详细地址</td>
                <td colspan="3">
                    <label class="adderss"></label>
            </tr>
            <tr class="Unicersity">
                <td class="tdtitle">统一社会信用编码</td>
                <td colspan="3">
                    <label class="Uscc"></label>
                </td>
            </tr>
        </table>
        <uc1:PcFiles runat="server" id="PcFiles" IsPc="false" />
    </div>
</asp:Content>
