<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DecHead.aspx.cs" Inherits="WebApp.Declaration.Declare.DecHead" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js"></script>
    <script>
        var SourcePage = getQueryString("SourcePage");
        var Source = getQueryString("Source");
        $(function () {
            InitClientPage();
            var ID = getQueryString("ID");
            var DecHeadInfo = eval('(<%=this.Model.DecHeadInfo%>)');
            var OtherPacks = eval('(<%=this.Model.OtherPacks%>)');
            var CurrentDate = getNowFormatDate();
            var OrderID = getQueryString("OrderId");
            var NoticeID = getQueryString("NoticeID");
            var CurrentAdmin = eval('(<%=this.Model.CurrentAdmin%>)');
            var CustomMaster = eval('(<%=this.Model.CustomMaster%>)');
            var CustomMasterValue = eval('(<%=this.Model.CustomMasterValue%>)');
            var DeclareCompanies = eval('(<%=this.Model.DeclareCompanies%>)');
            var ConsignorCompany = eval('(<%=this.Model.ConsignorCompany%>)');
            var DeclareCompanySZHY = eval('(<%=this.Model.DeclareCompanySZHY%>)');
            var ForeignCompany = eval('(<%=this.Model.ForgienCompany%>)');
            var Client = eval('(<%=this.Model.Client%>)');
            var TrafMode = eval('(<%=this.Model.TrafMode%>)');
            var TradeMode = eval('(<%=this.Model.TradeMode%>)');
            var CutMode = eval('(<%=this.Model.CutMode%>)');
            var TradeCountry = eval('(<%=this.Model.TradeCountry%>)');
            var TransMode = eval('(<%=this.Model.TransMode%>)');
            var Currency = eval('(<%=this.Model.Currency%>)');
            var WrapType = eval('(<%=this.Model.WrapType%>)');
            var FeeMark = eval('(<%=this.Model.FeeMark%>)');
            var Mark = eval('(<%=this.Model.Mark%>)');
            var DistinatePort = eval('(<%=this.Model.DistinatePort%>)');
            var EntyPortCode = eval('(<%=this.Model.EntyPortCode%>)');
            var EntryType = eval('(<%=this.Model.EntryType%>)');
            var DeclTrnRel = eval('(<%=this.Model.DeclTrnRel%>)')
            var BillType = eval('(<%=this.Model.BillType%>)');
            var GrossWeight = eval('(<%=this.Model.GrossWeight%>)');
            var NetWeight = eval('(<%=this.Model.NetWeight%>)');
            var TotalPacks = eval('(<%=this.Model.TotalPacks%>)');
            var VoyaNos = eval('(<%=this.Model.VoyaNos%>)');

            var CandidateData = eval('(<%=this.Model.CandidateData%>)');

            $("#CustomMaster").combobox({
                data: CustomMaster,
                onSelect: function (record) {
                    $.post('?action=getBaseCustomMasterDefault', { Code: record.ID }, function (data) {
                        var company = JSON.parse(data);
                        setMasterDefault(company);
                    });
                }
            });
            $("#IEPort").combobox({
                data: CustomMaster
            });
            $("#ConsigneeName").combobox({
                data: DeclareCompanies,
                onSelect: function (record) {
                    $.post('?action=getDeclareCompnay', { CompanyCode: record.ID }, function (data) {
                        var company = JSON.parse(data);
                        setConsigneeAndAgent(company);
                    });
                }
            });
            $("#AgentName").combobox({
                data: DeclareCompanies
            });
            $("#ConsignorName").combobox({
                data: ForeignCompany
            });
            $("#TrafMode").combobox({
                data: TrafMode
            });
            $("#TradeMode").combobox({
                data: TradeMode
            });
            $("#CutMode").combobox({
                data: CutMode
            });
            $("#TradeCountry").combobox({
                data: TradeCountry
            });
            $("#TradeAreaCode").combobox({
                data: TradeCountry
            });
            $("#TransMode").combobox({
                data: TransMode
            });
            $("#FeeCurr").combobox({
                data: Currency
            });
            $("#InsurCurr").combobox({
                data: Currency
            });
            $("#OtherCurr").combobox({
                data: Currency
            });
            $("#WrapType").combobox({
                data: WrapType
            });
            $("#FeeMark").combobox({
                data: FeeMark
            });
            $("#InsurMark").combobox({
                data: Mark
            });
            $("#OtherMark").combobox({
                data: Mark
            });
            $("#DistinatePort").combobox({
                data: DistinatePort
            });
            $("#DespPortCode").combobox({
                data: DistinatePort
            });
            $("#EntyPortCode").combobox({
                data: EntyPortCode
            });
            $("#EntryType").combobox({
                data: EntryType
            });

            $("#DeclTrnRel").combobox({
                data: DeclTrnRel
            });
            $("#BillType").combobox({
                data: BillType
            });

            //发单员选项
            $("#Selectable").combobox({
                data: CandidateData,
                required: true,
                valueField: 'value',
                textField: 'text',
                onChange: function (record) {

                },
            });

            $("#OrderID").val(OrderID);

            //设置不可编辑
            $("#InputerID").textbox({ readonly: true });
            $("#DeclareName").textbox({ readonly: true });
            $("#TypistNo").textbox({ readonly: true });
            //如果ID为空，则为新增
            if (ID == undefined || ID == "") {
                $("#NoticeID").val(NoticeID);
                $("#CustomMaster").combobox("setValue", CustomMasterValue);
                //$("#IEPort").combobox("setValue", "5303");
                $("#IEDate").textbox("setValue", CurrentDate);
                //设置境内收发货人，申报单位初始值
                setConsigneeAndAgent(DeclareCompanySZHY);
                $("#ConsignorName").combobox("setValue", ConsignorCompany.Name);
                $("#ConsignorCode").textbox("setValue", ConsignorCompany.Code);
                $("#OwnerName").textbox("setValue", Client.Name);
                $("#OwnerScc").textbox("setValue", Client.Code);
                if (Client.CustomsCode != "") {
                    $("#OwnerCusCode").textbox("setValue", Client.CustomsCode);
                } else {
                    $("#OwnerCusCode").textbox("setValue", Client.Code.substring(8, 17));
                }
                $("#TrafMode").combobox("setValue", "4");
                $("#TradeMode").combobox("setValue", "0110");
                $("#CutMode").combobox("setValue", "101");
                $("#TradeCountry").combobox("setValue", "HKG");
                $("#TradeAreaCode").combobox("setValue", "HKG");
                $("#TransMode").combobox("setValue", "1");
                $("#WrapType").combobox("setValue", "22");
                $("#GoodsPlace").textbox("setValue", "深圳市龙华区龙华街道富康社区东环二路110号中执时代广场B栋16H");
                //$("#GoodsPlace").textbox("setValue", "蛇口码头SCT");
                $("#DistinatePort").combobox("setValue", "HKG000");
                $("#DespPortCode").combobox("setValue", "HKG000");
                //$("#EntyPortCode").combobox("setValue", "470501");
                $("#EntryType").combobox("setValue", "M");
                $("#MarkNo").textbox("setValue", "N/M");
                $("#GrossWt").textbox("setValue", GrossWeight);
                $("#NetWt").textbox("setValue", NetWeight);
                $("#PackNo").textbox("setValue", TotalPacks);
                $("#DeclTrnRel").combobox("setValue", "0");
                $("#BillType").combobox("setValue", "1");
                $("#CurrentAdminID").val(CurrentAdmin.ID);
                $("#InputerID").textbox("setValue", CurrentAdmin.Name);
                $("#VoyNo").textbox("setValue", VoyaNos);              
                $("input:radio[value='ZC']").attr('checked', 'true');

                //新增时 隐藏按钮                           
                $("#btnMake").hide();
                $("#btnDownload").hide();

                $("#btnSave").show();
            } else {
                $("#DecHeadID").val(ID);
                $("#OtherPacks").val(JSON.stringify(OtherPacks));
                setEditDefault(DecHeadInfo);
                if (SourcePage == "Excel") {
                    $("#btnMake").hide();
                }
                if (SourcePage == "Maked"&&Source=="View") {
                    $("#btnMake").hide();
                    //$("#btnDownload").hide();//允许导出excel，用于制作3C目录外商检单据
                }
            }
        });
    </script>
    <script>
        //获取当前时间，格式YYYY-MM-DD
        function getNowFormatDate() {
            var date = new Date();
            var seperator1 = "-";
            var year = date.getFullYear();
            var month = date.getMonth() + 1;
            var strDate = date.getDate();
            if (month >= 1 && month <= 9) {
                month = "0" + month;
            }
            if (strDate >= 0 && strDate <= 9) {
                strDate = "0" + strDate;
            }
            var currentdate = '' + year + month + strDate;
            return currentdate;
        }

        function setConsigneeAndAgent(DeclareCompanySZHY) {
            $("#ConsigneeName").combobox("setValue", DeclareCompanySZHY.Code);
            $("#ConsigneeScc").textbox("setValue", DeclareCompanySZHY.Code);
            $("#ConsigneeCusCode").textbox("setValue", DeclareCompanySZHY.CustomsCode);
            $("#ConsigneeCiqCode").textbox("setValue", DeclareCompanySZHY.CIQCode);

            $("#AgentName").combobox("setValue", DeclareCompanySZHY.Code);
            $("#AgentScc").textbox("setValue", DeclareCompanySZHY.Code);
            $("#AgentCusCode").textbox("setValue", DeclareCompanySZHY.CustomsCode);
            $("#AgentCiqCode").textbox("setValue", DeclareCompanySZHY.CIQCode);
            $("#TypistNo").textbox("setValue", DeclareCompanySZHY.TypistNo);
            $("#DeclareName").textbox("setValue", DeclareCompanySZHY.DeclareName);
        }

        function setMasterDefault(MasterDefault) {
            var ID = getQueryString("ID");
            if (ID == undefined || ID == "") {
                $("#IEPort").combobox("setValue", MasterDefault.IEPortCode);
                $("#EntyPortCode").combobox("setValue", MasterDefault.EntyPortCode);
            }
        }

        function Save() {
            var isValid = $("#form1").form("enableValidation").form("validate");
            if (!isValid) {
                //$.messager.alert('提示', '请按提示输入数据！');
                return;
            }

            $("#btnSave").hide();

            var model = {
                OrderID: $("#OrderID").val(),
                AdminID: $("#CurrentAdminID").val(),
                DecHeadID: $("#DecHeadID").val(),
                NoticeID: $("#NoticeID").val(),
                CustomMaster: $("#CustomMaster").combobox("getValue"),
                CusDecStatus: $("#CusDecStatus").textbox("getValue"),
                SeqNo: $("#SeqNo").textbox("getValue"),
                PreEntryId: $("#PreEntryId").textbox("getValue"),
                EntryId: $("#EntryId").textbox("getValue"),
                IEPort: $("#IEPort").combobox("getValue"),
                ManualNo: $("#ManualNo").textbox("getValue"),
                ContrNo: $("#ContrNo").textbox("getValue"),
                IEDate: $("#IEDate").textbox("getValue"),
                ConsigneeName: $("#ConsigneeName").combobox("getText"),
                ConsigneeScc: $("#ConsigneeScc").textbox("getValue"),
                ConsigneeCusCode: $("#ConsigneeCusCode").textbox("getValue"),
                ConsigneeCiqCode: $("#ConsigneeCiqCode").textbox("getValue"),
                ConsignorName: $("#ConsignorName").combobox("getValue"),
                ConsignorCode: $("#ConsignorCode").textbox("getValue"),
                OwnerName: $("#OwnerName").textbox("getValue"),
                OwnerScc: $("#OwnerScc").textbox("getValue"),
                OwnerCusCode: $("#OwnerCusCode").textbox("getValue"),
                OwnerCiqCode: $("#OwnerCiqCode").textbox("getValue"),
                AgentName: $("#AgentName").combobox("getText"),
                AgentScc: $("#AgentScc").textbox("getValue"),
                AgentCusCode: $("#AgentCusCode").textbox("getValue"),
                AgentCiqCode: $("#AgentCiqCode").textbox("getValue"),
                TrafMode: $("#TrafMode").combobox("getValue"),
                TrafName: $("#TrafName").textbox("getValue"),
                VoyNo: $("#VoyNo").textbox("getValue"),
                BillNo: $("#BillNo").textbox("getValue"),
                TradeMode: $("#TradeMode").combobox("getValue"),
                CutMode: $("#CutMode").combobox("getValue"),
                LicenseNo: $("#LicenseNo").textbox("getValue"),
                TradeCountry: $("#TradeCountry").combobox("getValue"),
                DistinatePort: $("#DistinatePort").combobox("getValue"),
                TransMode: $("#TransMode").combobox("getValue"),
                FeeMark: $("#FeeMark").combobox("getValue"),
                FeeCurr: $("#FeeCurr").combobox("getValue"),
                FeeRate: $("#FeeRate").textbox("getValue"),
                InsurMark: $("#InsurMark").combobox("getValue"),
                InsurCurr: $("#InsurCurr").combobox("getValue"),
                InsurRate: $("#InsurRate").textbox("getValue"),
                OtherMark: $("#OtherMark").combobox("getValue"),
                OtherCurr: $("#OtherCurr").combobox("getValue"),
                OtherRate: $("#OtherRate").textbox("getValue"),
                PackNo: $("#PackNo").textbox("getValue"),
                WrapType: $("#WrapType").combobox("getValue"),
                GrossWt: $("#GrossWt").textbox("getValue"),
                NetWt: $("#NetWt").textbox("getValue"),
                TradeAreaCode: $("#TradeAreaCode").combobox("getValue"),
                EntyPortCode: $("#EntyPortCode").combobox("getValue"),
                GoodsPlace: $("#GoodsPlace").textbox("getValue"),
                EntryType: $("#EntryType").combobox("getValue"),
                DespPortCode: $("#DespPortCode").combobox("getValue"),
                MarkNo: $("#MarkNo").textbox("getValue"),
                NoteS: $("#NoteS").textbox("getValue"),
                ApprNo: $("#ApprNo").textbox("getValue"),
                DeclTrnRel: $("#DeclTrnRel").combobox("getValue"),
                BillType: $("#BillType").combobox("getValue"),
                InputerID: $("#InputerID").textbox("getValue"),
                DeclareName: $("#DeclareName").textbox("getValue"),
                TypistNo: $("#TypistNo").textbox("getValue"),
                SpecialRelationShip: $('input[name=SpecialRelationShip]:checked').length,
                PriceConfirm: $('input[name=PriceConfirm]:checked').length,
                PayConfirm: $('input[name=PayConfirm]:checked').length,
                FormulaPrice: $('input[name=FormulaPrice]:checked').length, 
                ProvisionalPrice: $('input[name=ProvisionalPrice]:checked').length,
                Disinfect: $('input[name=Disinfect]:checked').length,
                ChkSurety: $('input[name=ChkSurety]:checked').length,
                Type: $('input:radio[name="Type"]:checked').val(),
                OtherPacks: $("#OtherPacks").val()
            };
            MaskUtil.mask();//遮挡层
            $.post('?action=SaveHead', model, function (data) {
                MaskUtil.unmask();//关闭遮挡层
                var Result = JSON.parse(data);
                if (Result.result) {
                    $.messager.alert('消息', Result.info, 'info', function () {
                        if ($("#DecHeadID").val() == "") {
                            var url = location.pathname.replace(/Declare\/DecHead.aspx/ig, 'Notice/List.aspx');
                            //top.document.getElementById('ifrmain').src = url;
                            window.parent.location.href = url;
                        }
                        else {
                            //var url = location.pathname.replace(/DecHead.aspx/ig, 'List.aspx');
                            //window.location = url;
                        }
                    });

                } else {
                    $.messager.alert('消息', Result.info);
                }
            });
        }

        //转换舱单
        function Transform() {
            var ID = getQueryString("ID");
            if (ID == undefined || ID == "") {
                $.messager.alert('info', '请先录入报关单信息！');
                return;
            }
            $.messager.confirm('确认', '请您再次确认转换舱单？', function (success) {
                MaskUtil.mask();//遮挡层
                if (success) {
                    $.post('?action=Transform', { ID: ID }, function (res) {
                        MaskUtil.unmask();//关闭遮挡层
                        var result = JSON.parse(res);
                        $.messager.alert('消息', result.message, function () {

                        });
                    });
                }
            });
        }


        function OtherPack() {
            var PackSource = "Add";
            if (window.parent.frames.Source != 'Add' && window.parent.frames.Source != 'Assign' && window.parent.frames.Source != 'Edit') {
                var PackSource = "Search";
            }
            var OtherPacks = $("#OtherPacks").val();
            var url = "";
            if (OtherPacks == "") {
                url = location.pathname.replace(/DecHead.aspx/ig, 'DecOtherPack.aspx?PackSource=' + PackSource + '&OtherPacks=[]');
            } else {
                url = location.pathname.replace(/DecHead.aspx/ig, 'DecOtherPack.aspx?PackSource=' + PackSource + '&OtherPacks=' + OtherPacks);
            }

            $.myWindow.setMyWindow("DecHeadWindow", window);
            $.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '其他包装',
                width: '540px',
                height: '480px'
            });
        }


        function Back() {
            if ($("#DecHeadID").val() == "") {
                var url = location.pathname.replace(/Declare\/DecHead.aspx/ig, 'Notice/List.aspx');
                //top.document.getElementById('ifrmain').src = url;
                window.parent.location.href = url;
            }
            else {
                if (SourcePage == "Maked") {
                    var url = location.pathname.replace(/Declare\/DecHead.aspx/ig, 'Declare/MakedList.aspx');
                    //top.document.getElementById('ifrmain').src = url;
                    window.parent.location.href = url;
                } else if (SourcePage == "Imported") {
                    var url = location.pathname.replace(/Declare\/DecHead.aspx/ig, 'Declare/ImportedList.aspx');
                    //top.document.getElementById('ifrmain').src = url;
                    window.parent.location.href = url;
                } else if (SourcePage == "Excel") {
                    var url = location.pathname.replace(/Declare\/DecHead.aspx/ig, 'Declare/ExcelList.aspx');
                    //top.document.getElementById('ifrmain').src = url;
                    window.parent.location.href = url;
                }else if (SourcePage == "Checking") {
                    var url = location.pathname.replace(/Declare\/DecHead.aspx/ig, 'Declare/CheckingList.aspx');
                    //top.document.getElementById('ifrmain').src = url;
                    window.parent.location.href = url;
                } else if (SourcePage == "Checked") {
                    var url = location.pathname.replace(/Declare\/DecHead.aspx/ig, 'Declare/CheckedList.aspx');
                    //top.document.getElementById('ifrmain').src = url;
                    window.parent.location.href = url;
                }else {
                    var url = location.pathname.replace(/Declare\/DecHead.aspx/ig, 'Declare/List.aspx');
                    //top.document.getElementById('ifrmain').src = url;
                    window.parent.location.href = url;
                }
            }
        }

        //批量制单
        function Make() {
            var id = $("#DecHeadID").val();
            $.messager.confirm('确认', "请您再次确认申报", function (success) {
                if (success) {
                    MaskUtil.mask();//遮挡层
                    $.post('?action=Make', { ID: id }, function (res) {
                        MaskUtil.unmask();//关闭遮挡层
                        var result = JSON.parse(res);
                        $.messager.alert('消息', result.message, 'info', function () {
                            Back();
                        });
                    });
                }
            });
        }

    </script>
    <script>
        function setEditDefault(DecHeadInfo) {
            $("#NoticeID").val(DecHeadInfo.NoticeID);
            $("#CustomMaster").combobox("setValue", DecHeadInfo.CustomMaster);
            $("#SeqNo").textbox("setValue", DecHeadInfo.SeqNo);
            $("#PreEntryId").textbox("setValue", DecHeadInfo.PreEntryId);
            $("#EntryId").textbox("setValue", DecHeadInfo.EntryId);
            $("#IEPort").combobox("setValue", DecHeadInfo.IEPort);
            $("#ManualNo").textbox("setValue", DecHeadInfo.ManualNo);
            $("#ContrNo").textbox("setValue", DecHeadInfo.ContrNo);
            $("#IEDate").textbox("setValue", DecHeadInfo.IEDate);
            $("#DDate").textbox("setValue", DecHeadInfo.DDate);
            $("#ConsigneeName").combobox("setValue", DecHeadInfo.ConsigneeName);
            $("#ConsigneeScc").textbox("setValue", DecHeadInfo.ConsigneeScc);
            $("#ConsigneeCusCode").textbox("setValue", DecHeadInfo.ConsigneeCusCode);
            $("#ConsigneeCiqCode").textbox("setValue", DecHeadInfo.ConsigneeCiqCode);
            $("#ConsignorName").combobox("setValue", DecHeadInfo.ConsignorCode);
            $("#ConsignorCode").textbox("setValue", DecHeadInfo.ConsignorName);
            $("#OwnerName").textbox("setValue", DecHeadInfo.OwnerName);
            $("#OwnerScc").textbox("setValue", DecHeadInfo.OwnerScc);
            if (DecHeadInfo.OwnerCusCode != "") {
                $("#OwnerCusCode").textbox("setValue", DecHeadInfo.OwnerCusCode);
            } else {
                $("#OwnerCusCode").textbox("setValue", DecHeadInfo.OwnerScc.substring(8, 17));
            }
            $("#OwnerCiqCode").textbox("setValue", DecHeadInfo.OwnerCiqCode);
            $("#AgentName").combobox("setValue", DecHeadInfo.AgentName);
            $("#AgentScc").textbox("setValue", DecHeadInfo.AgentScc);
            $("#AgentCusCode").textbox("setValue", DecHeadInfo.AgentCusCode);
            $("#AgentCiqCode").textbox("setValue", DecHeadInfo.AgentCiqCode);
            $("#TrafMode").combobox("setValue", DecHeadInfo.TrafMode);
            $("#TrafName").textbox("setValue", DecHeadInfo.TrafName);
            $("#VoyNo").textbox("setValue", DecHeadInfo.VoyNo);
            $("#BillNo").textbox("setValue", DecHeadInfo.BillNo);
            $("#TradeMode").combobox("setValue", DecHeadInfo.TradeMode);
            $("#CutMode").combobox("setValue", DecHeadInfo.CutMode);
            $("#LicenseNo").textbox("setValue", DecHeadInfo.LicenseNo);
            $("#TradeCountry").combobox("setValue", DecHeadInfo.TradeCountry);
            $("#DistinatePort").combobox("setValue", DecHeadInfo.DistinatePort);
            $("#TransMode").combobox("setValue", DecHeadInfo.TransMode);
            $("#FeeMark").combobox("setValue", DecHeadInfo.FeeMark);
            $("#FeeCurr").textbox("setValue", DecHeadInfo.FeeCurr);
            $("#FeeRate").textbox("setValue", DecHeadInfo.FeeRate);
            $("#InsurMark").combobox("setValue", DecHeadInfo.InsurMark);
            $("#InsurCurr").textbox("setValue", DecHeadInfo.InsurCurr);
            $("#InsurRate").textbox("setValue", DecHeadInfo.InsurRate);
            $("#OtherMark").combobox("setValue", DecHeadInfo.OtherMark);
            $("#OtherCurr").textbox("setValue", DecHeadInfo.OtherCurr);
            $("#OtherRate").textbox("setValue", DecHeadInfo.OtherRate);
            $("#PackNo").textbox("setValue", DecHeadInfo.PackNo);
            $("#WrapType").combobox("setValue", DecHeadInfo.WrapType);
            $("#GrossWt").textbox("setValue", DecHeadInfo.GrossWt);
            $("#NetWt").textbox("setValue", DecHeadInfo.NetWt);
            $("#TradeAreaCode").combobox("setValue", DecHeadInfo.TradeAreaCode);
            $("#EntyPortCode").combobox("setValue", DecHeadInfo.EntyPortCode);
            $("#GoodsPlace").textbox("setValue", DecHeadInfo.GoodsPlace);
            $("#EntryType").combobox("setValue", DecHeadInfo.EntryType);
            $("#DespPortCode").combobox("setValue", DecHeadInfo.DespPortCode);
            $("#MarkNo").textbox("setValue", DecHeadInfo.MarkNo);
            $("#NoteS").textbox("setValue", DecHeadInfo.NoteS);
            $("#ApprNo").textbox("setValue", DecHeadInfo.ApprNo);
            $("#DeclTrnRel").combobox("setValue", DecHeadInfo.DeclTrnRel);
            $("#BillType").combobox("setValue", DecHeadInfo.BillType);
            $("#InputerID").textbox("setValue", DecHeadInfo.InputerID);
            $("#DeclareName").textbox("setValue", DecHeadInfo.DeclareName);
            $("#TypistNo").textbox("setValue", DecHeadInfo.TypistNo);
            if (DecHeadInfo.SpecialRelationShip == "1") {
                $("#SpecialRelationShip").attr("checked", true);
            }
            if (DecHeadInfo.PriceConfirm == "1") {
                $("#PriceConfirm").attr("checked", true);
            }
            if (DecHeadInfo.PayConfirm == "1") {
                $("#PayConfirm").attr("checked", true);
            }
            if (DecHeadInfo.FormulaPrice == "1") {
                $("#FormulaPrice").attr("checked", true);
            }
            if (DecHeadInfo.ProvisionalPrice == "1") {
                $("#ProvisionalPrice").attr("checked", true);
            }
            if (DecHeadInfo.Disinfect == "1") {
                $("#Disinfect").attr("checked", true);
            }
            if (DecHeadInfo.ChkSurety == "1") {
                $("#ChkSurety").attr("checked", true);
            }
            if (DecHeadInfo.Type != null && DecHeadInfo.Type !== undefined) {
                $("input:radio[value='" + DecHeadInfo.Type + "']").attr('checked', 'true');
            }

        }

        function DownloadExcel() {
            $.messager.confirm('确认', '请您再次确认导出Excel!', function (success) {
                if (success) {
                    MaskUtil.mask();//遮挡层
                    var ID = $("#DecHeadID").val();
                    $.post("?action=DownloadExcel", { ID: ID }, function (data) {
                        MaskUtil.unmask();
                        var result = JSON.parse(data);
                        if (result.result) {
                            Download(result.url);
                            $.messager.alert('消息', result.info);
                        } else {
                            $.messager.alert('消息', result.info);
                        }
                    });
                }
            });

            //$("#Selectable").combobox("setValue", null);

            //$('#downloadExcel-dialog').dialog({
            //    title: '确认',
            //    width: 350,
            //    height: 180,
            //    closed: false,
            //    //cache: false,
            //    modal: true,
            //    buttons: [{
            //        id: 'btn-ok',
            //        text: '确定',
            //        width: 70,
            //        handler: function () {
            //            if (!$("#form2").form('validate')) {
            //                return;
            //            }

            //            var ID = $("#DecHeadID").val();
            //            var CustomSubmiterAdminID = $("#Selectable").combobox("getValue");
                        
            //            MaskUtil.mask();
                        
            //            $.post("?action=DownloadExcel", { ID: ID, CustomSubmiterAdminID: CustomSubmiterAdminID, }, function (data) {
            //                MaskUtil.unmask();
            //                var result = JSON.parse(data);
            //                if (result.result) {
            //                    Download(result.url);
            //                    $('#downloadExcel-dialog').dialog('close');
            //                    $('#orders').datagrid('reload');
            //                    $.messager.alert('消息', result.info);
            //                } else {
            //                    $.messager.alert('消息', result.info);
            //                }
            //            });
            //        }
            //    }, {
            //        id: 'btn-cancel',
            //        text: '取消',
            //        width: 70,
            //        handler: function () {
            //            $('#downloadExcel-dialog').dialog('close');
            //        }
            //    }],
            //});

            //$('#downloadExcel-dialog').window('center'); //dialog 居中

        }

        function Download(Url) {
            let a = document.createElement('a');
            document.body.appendChild(a);
            a.href = Url;
            a.download = "";
            a.click();
        }
    </script>
    <style>
        .lbl {
            font-size: 14px;
        }
    </style>
</head>
<body>
    <div style="margin-left: 10px;">
        <form id="form1" runat="server">
            <div style="margin-left: 28px">
                <table style="line-height: 25px;">
                    <tr>
                        <td colspan="2">
                            <div id="btndiv">
                                <a id="btnSave" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="Save()">保存草稿</a>
                                <%--<a id="btnMake" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-ok'" onclick="Make()">申报</a>--%>
                                <a id="btnDownload" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelExport'" onclick="DownloadExcel()">导出表格</a>
                                <a id="btnBack" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-back'" onclick="Back()">返回</a>
                            </div>
                        </td>
                        <td>
                            <input type="hidden" id="NoticeID" />
                            <input type="hidden" id="DecHeadID" />
                            <input type="hidden" id="CurrentAdminID" />
                            <input type="hidden" id="OtherPacks" />
                            <input type="hidden" id="OrderID" />
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 90px" class="lbl">申报地海关：</td>
                        <td style="width: 400px">
                            <input class="easyui-combobox" id="CustomMaster" name="CustomMaster"
                                data-options="valueField:'ID',textField:'Text',required:true" style="width: 350px" />
                        </td>
                        <td style="width: 90px" class="lbl">申报状态：</td>
                        <td style="width: 360px">
                            <input class="easyui-textbox" id="CusDecStatus" name="CusDecStatus" disabled="disabled" style="width: 350px" />
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 90px" class="lbl">统一编号：</td>
                        <td style="width: 400px">
                            <input class="easyui-textbox" id="SeqNo" name="SeqNo" disabled="disabled" style="width: 350px" />
                        </td>
                        <td style="width: 90px" class="lbl">预录入号：</td>
                        <td style="width: 360px">
                            <input class="easyui-textbox" id="PreEntryId" name="PreEntryId" disabled="disabled" style="width: 350px" />
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 90px" class="lbl">海关编号：</td>
                        <td style="width: 400px">
                            <input class="easyui-textbox" id="EntryId" name="EntryId" disabled="disabled" style="width: 350px" />
                        </td>
                        <td style="width: 90px" class="lbl">进境关别：</td>
                        <td style="width: 360px">
                            <input class="easyui-combobox" id="IEPort" name="IEPort"
                                data-options="valueField:'ID',textField:'Text',required:true" style="width: 350px" />
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 90px" class="lbl">备案号：</td>
                        <td style="width: 400px">
                            <input class="easyui-textbox" id="ManualNo" name="ManualNo" style="width: 350px" data-options="validType:'length[1,12]'" />
                        </td>
                        <td style="width: 90px" class="lbl">合同协议号：</td>
                        <td style="width: 360px">
                            <input class="easyui-textbox" id="ContrNo" name="ContrNo" style="width: 350px" readonly="true" />
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 90px" class="lbl">进口日期：</td>
                        <td style="width: 400px">
                            <input class="easyui-textbox" id="IEDate" name="IEDate" style="width: 350px" />
                        </td>
                        <td style="width: 90px" class="lbl">申报日期：</td>
                        <td style="width: 360px">
                            <input class="easyui-textbox" id="DDate" name="DDate" disabled="disabled" style="width: 350px" />
                        </td>
                    </tr>
                </table>
                <table style="line-height: 25px;">
                    <tr>
                        <td style="width: 100px" class="lbl">境内收发货人：</td>
                        <td style="width: 260px">
                            <input class="easyui-combobox" id="ConsigneeName" name="ConsigneeName"
                                data-options="valueField:'ID',textField:'Text',required:true" style="width: 250px" />
                        </td>
                        <td style="width: 70px" class="lbl">海关代码：</td>
                        <td style="width: 100px">
                            <input class="easyui-textbox" id="ConsigneeCusCode" name="ConsigneeCusCode" readonly="true" style="width: 90px" />
                        </td>
                        <td style="width: 100px" class="lbl">社会信用代码：</td>
                        <td style="width: 150px">
                            <input class="easyui-textbox" id="ConsigneeScc" name="ConsigneeScc" readonly="true" style="width: 140px" />
                        </td>
                        <td style="width: 100px" class="lbl">检验检疫代码：</td>
                        <td style="width: 100px">
                            <input class="easyui-textbox" id="ConsigneeCiqCode" name="ConsigneeCiqCode" readonly="true" style="width: 90px" />
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 90px" class="lbl">境外收发货人：</td>
                        <td style="width: 260px">
                            <input class="easyui-combobox" id="ConsignorName" name="ConsignorName"
                                data-options="valueField:'ID',textField:'Text',required:true" style="width: 250px" />
                        </td>
                        <td style="width: 60px" class="lbl">英文名称：</td>
                        <td colspan="5">
                            <input class="easyui-textbox" id="ConsignorCode" name="ConsignorCode" readonly="true" style="width: 530px" />
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 90px" class="lbl">消费使用单位：</td>
                        <td style="width: 260px">
                            <input class="easyui-textbox" id="OwnerName" name="OwnerName" readonly="true" style="width: 250px" />
                        </td>
                        <td style="width: 60px" class="lbl">海关代码：</td>
                        <td style="width: 100px">
                            <input class="easyui-textbox" id="OwnerCusCode" name="OwnerCusCode" style="width: 90px" data-options="required:'required'" />
                        </td>
                        <td style="width: 90px" class="lbl">社会信用代码：</td>
                        <td style="width: 150px">
                            <input class="easyui-textbox" id="OwnerScc" name="OwnerScc" style="width: 140px" data-options="validType:'length[1,18]'" />
                        </td>
                        <td style="width: 90px" class="lbl">检验检疫代码：</td>
                        <td style="width: 100px">
                            <input class="easyui-textbox" id="OwnerCiqCode" name="OwnerCiqCode" style="width: 90px" data-options="validType:'length[1,10]'" />
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 90px" class="lbl">申报单位：</td>
                        <td style="width: 260px">
                            <input class="easyui-textbox" id="AgentName" name="AgentName"
                                data-options="valueField:'ID',textField:'Text',required:true" readonly="true" style="width: 250px" />
                        </td>
                        <td style="width: 60px" class="lbl">海关代码：</td>
                        <td style="width: 100px">
                            <input class="easyui-textbox" id="AgentCusCode" name="AgentCusCode" readonly="true" style="width: 90px" />
                        </td>
                        <td style="width: 90px" class="lbl">社会信用代码：</td>
                        <td style="width: 150px">
                            <input class="easyui-textbox" id="AgentScc" name="AgentScc" readonly="true" style="width: 140px" />
                        </td>
                        <td style="width: 90px" class="lbl">检验检疫代码：</td>
                        <td style="width: 100px">
                            <input class="easyui-textbox" id="AgentCiqCode" name="AgentCiqCode" readonly="true" style="width: 90px" />
                        </td>
                    </tr>
                </table>
                <table style="line-height: 25px;">
                    <tr>
                        <td style="width: 90px" class="lbl">运输方式：</td>
                        <td style="width: 260px">
                            <input class="easyui-textbox" id="TrafMode" name="TrafMode"
                                data-options="valueField:'Value',textField:'Text',required:true" style="width: 250px" />
                        </td>
                        <td style="width: 100px" class="lbl">运输工具名称：</td>
                        <td style="width: 220px">
                            <input class="easyui-textbox" id="TrafName" name="TrafName" style="width: 210px" data-options="validType:'length[1,25]'" />
                        </td>
                        <td style="width: 70px" class="lbl">航次号：</td>
                        <td style="width: 220px">
                            <input class="easyui-textbox" id="VoyNo" name="VoyNo"
                                data-options="valueField:'Value',textField:'Text',required:true,validType:'length[1,50]',readonly:true," style="width: 210px" />
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 90px" class="lbl">提运单号：</td>
                        <td style="width: 260px">
                            <input class="easyui-textbox" id="BillNo" name="BillNo" style="width: 250px" />
                        </td>
                        <td style="width: 90px" class="lbl">监管方式：</td>
                        <td style="width: 220px">
                            <input class="easyui-combobox" id="TradeMode" name="TradeMode"
                                data-options="valueField:'Value',textField:'Text',required:true" style="width: 210px" />
                        </td>
                        <td style="width: 70px" class="lbl">征免性质：</td>
                        <td style="width: 220px">
                            <input class="easyui-combobox" id="CutMode" name="CutMode"
                                data-options="valueField:'Value',textField:'Text',required:true" style="width: 210px" />
                        </td>
                    </tr>
                </table>
                <table style="line-height: 25px;">
                    <tr>
                        <td style="width: 90px" class="lbl">许可证号：</td>
                        <td style="width: 150px">
                            <input class="easyui-textbox" id="LicenseNo" name="LicenseNo" style="width: 140px" data-options="validType:'length[1,20]'" />
                        </td>
                        <td style="width: 100px" class="lbl">启运国(地区)：</td>
                        <td style="width: 150px">
                            <input class="easyui-combobox" id="TradeCountry" name="TradeCountry"
                                data-options="valueField:'Value',textField:'Text',required:true" style="width: 140px" />
                        </td>
                        <td style="width: 60px" class="lbl">经停港：</td>
                        <td style="width: 175px">
                            <input class="easyui-combobox" id="DistinatePort" name="DistinatePort"
                                data-options="valueField:'Value',textField:'Text',required:true" style="width: 160px" />
                        </td>
                        <td style="width: 70px" class="lbl">成交方式：</td>
                        <td>
                            <input class="easyui-textbox" id="TransMode" name="TransMode"
                                data-options="valueField:'Value',textField:'Text',required:true" style="width: 150px" />
                        </td>
                    </tr>
                </table>
                <table style="line-height: 25px;">
                    <tr>
                        <td style="width: 90px" class="lbl">运费：</td>
                        <td>
                            <input class="easyui-combobox" id="FeeMark" name="FeeMark"
                                data-options="valueField:'Value',textField:'Text'" style="width: 50px" />
                        </td>
                        <td>
                            <input class="easyui-combobox" id="FeeCurr" name="FeeCurr"
                                data-options="valueField:'Value',textField:'Text'" style="width: 100px" />
                        </td>
                        <td>
                            <input class="easyui-textbox" id="FeeRate" name="FeeRate" style="width: 70px" />
                        </td>
                        <td class="lbl">保险费：</td>
                        <td>
                            <input class="easyui-combobox" id="InsurMark" name="InsurMark"
                                data-options="valueField:'Value',textField:'Text'" style="width: 50px" />
                        </td>
                        <td>
                            <input class="easyui-combobox" id="InsurCurr" name="InsurCurr"
                                data-options="valueField:'Value',textField:'Text'" style="width: 100px" />
                        </td>
                        <td>
                            <input class="easyui-textbox" id="InsurRate" name="InsurRate" style="width: 70px" />
                        </td>
                        <td class="lbl">杂费：</td>
                        <td>
                            <input class="easyui-combobox" id="OtherMark" name="OtherMark"
                                data-options="valueField:'Value',textField:'Text'" style="width: 50px" />
                        </td>
                        <td>
                            <input class="easyui-combobox" id="OtherCurr" name="OtherCurr"
                                data-options="valueField:'Value',textField:'Text'" style="width: 100px" />
                        </td>
                        <td>
                            <input class="easyui-textbox" id="OtherRate" name="OtherRate" style="width: 70px" />
                        </td>
                        <td class="lbl">件数：</td>
                        <td>
                            <input class="easyui-textbox" id="PackNo" name="PackNo" style="width: 50px" />
                        </td>
                    </tr>
                </table>
                <table style="line-height: 25px;">
                    <tr>
                        <td style="width: 90px" class="lbl">包装种类：</td>
                        <td style="width: 260px">
                            <input class="easyui-combobox" id="WrapType" name="WrapType"
                                data-options="valueField:'Value',textField:'Text'" style="width: 230px" />
                        </td>
                        <td style="width: 100px">
                            <a id="btnOtherPack" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-ok'" onclick="OtherPack()">其他包装</a>
                        </td>
                        <td style="width: 90px" class="lbl">毛重(KG)：</td>
                        <td style="width: 180px">
                            <input class="easyui-textbox" id="GrossWt" name="GrossWt" style="width: 150px" />
                        </td>
                        <td style="width: 90px" class="lbl">净重(KG)：</td>
                        <td>
                            <input class="easyui-textbox" id="NetWt" name="NetWt" style="width: 150px" />
                        </td>
                    </tr>
                </table>
                <table style="line-height: 25px;">
                    <tr>
                        <td class="lbl">贸易国别(地区)：</td>
                        <td style="width: 160px">
                            <input class="easyui-combobox" id="TradeAreaCode" name="TradeAreaCode"
                                data-options="valueField:'Value',textField:'Text',required:true" style="width: 150px" />
                        </td>
                        <td style="width: 70px" class="lbl">入境口岸：</td>
                        <td style="width: 160px">
                            <input class="easyui-combobox" id="EntyPortCode" name="EntyPortCode"
                                data-options="valueField:'Value',textField:'Text',required:true" style="width: 150px" />
                        </td>
                        <td style="width: 100px" class="lbl">货物存放地点：</td>
                        <td>
                            <input class="easyui-textbox" id="GoodsPlace" name="GoodsPlace" style="width: 370px" data-options="validType:'length[1,100]'" />
                        </td>
                    </tr>
                </table>
                <table style="line-height: 25px;">
                    <tr>
                        <td style="width: 90px" class="lbl">报关单类型：</td>
                        <td style="width: 160px">
                            <input class="easyui-combobox" id="EntryType" name="EntryType"
                                data-options="valueField:'Value',textField:'Text'" style="width: 150px" />
                        </td>
                        <td style="width: 70px" class="lbl">启运港：</td>
                        <td style="width: 160px">
                            <input class="easyui-combobox" id="DespPortCode" name="DespPortCode"
                                data-options="valueField:'Value',textField:'Text',required:true" style="width: 150px" />
                        </td>
                        <td style="width: 90px" class="lbl">标记唛码：</td>
                        <td>
                            <input class="easyui-textbox" id="MarkNo" name="MarkNo" style="width: 370px" data-options="validType:'length[1,400]'" />
                        </td>
                    </tr>
                </table>
                <table style="line-height: 25px;">
                    <tr>
                        <td style="width: 100px" class="lbl">其他确认事项：</td>
                        <td>
                            <input type="checkbox" name="SpecialRelationShip" id="SpecialRelationShip" value="1" />
                            <label for="check1" class="lbl">特殊关系确认</label>
                        </td>
                        <td>
                            <input type="checkbox" name="PriceConfirm" id="PriceConfirm" value="1" />
                            <label for="check1" class="lbl">价格影响确认</label>
                        </td>
                        <td>
                            <input type="checkbox" name="PayConfirm" id="PayConfirm" value="1" />
                            <label for="check1" class="lbl">支付特权使用费确认</label>
                        </td>
                        <td>
                            <input type="checkbox" name="FormulaPrice" id="FormulaPrice" value="1" />
                            <label for="check1" class="lbl">公式订单确认</label>
                        </td>
                        <td>
                            <input type="checkbox" name="ProvisionalPrice" id="ProvisionalPrice" value="1" />
                            <label for="check1" class="lbl">暂定价格确认</label>
                        </td>
                        <td style="display:none;">
                            <input type="checkbox" name="Disinfect" id="Disinfect" checked="checked" value="1" />
                            <label for="check1" class="lbl">已实施预防性消毒</label>
                        </td>
                        
                    </tr>
                    <tr>
                        <td style="width: 90px" class="lbl">业务事项：</td>
                        <td>
                            <input type="radio" name="Type" value="SW" />
                            <label class="lbl">税单无纸化</label>
                        </td>
                        <td>
                            <input type="radio" name="Type" value="ZB" />
                            <label class="lbl">自主报税</label>
                        </td>
                        <td>
                            <input type="radio" name="Type" value="ZC" />
                            <label class="lbl">自报自缴</label>
                        </td>
                        <td>
                            <input type="checkbox" name="ChkSurety" id="ChkSurety" />
                            <label for="check1" class="lbl">担保验放</label>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 90px" class="lbl">备注：</td>
                        <td colspan="5">
                            <input class="easyui-textbox" id="NoteS" name="NoteS" style="width: 850px" data-options="multiline:true,validType:'length[1,255]',tipPosition:'bottom'," />
                        </td>
                    </tr>
                </table>
                <table style="line-height: 25px;">
                    <tr>
                        <td style="width: 100px" class="lbl">外汇核销单号：</td>
                        <td style="width: 200px">
                            <input class="easyui-textbox" id="ApprNo" name="ApprNo" style="width: 190px" data-options="validType:'length[1,50]'" />
                        </td>
                        <td style="width: 140px" class="lbl">报关/转关关系标志：</td>
                        <td style="width: 200px">
                            <input class="easyui-combobox" id="DeclTrnRel" name="DeclTrnRel"
                                data-options="valueField:'Value',textField:'Text'" style="width: 190px" />
                        </td>
                        <td style="width: 100px" class="lbl">备案清单类型：</td>
                        <td style="width: 230px">
                            <input class="easyui-combobox" id="BillType" name="BillType"
                                data-options="valueField:'Value',textField:'Text'" style="width: 220px" />
                        </td>
                    </tr>
                </table>
                <table style="line-height: 25px;">
                    <tr>
                        <td style="width: 90px" class="lbl">录入人：</td>
                        <td style="width: 200px">
                            <input class="easyui-textbox" id="InputerID" name="InputerID" style="width: 190px" />
                        </td>
                        <td style="width: 90px" class="lbl">报关员：</td>
                        <td style="width: 200px">
                            <input class="easyui-textbox" id="DeclareName" name="DeclareName" style="width: 190px" />
                        </td>
                        <td style="width: 120px" class="lbl">报关员卡号：</td>
                        <td style="width: 200px">
                            <input class="easyui-textbox" id="TypistNo" name="TypistNo" style="width: 190px" />
                        </td>
                    </tr>
                </table>
            </div>
        </form>
    </div>


    <%--<div id="downloadExcel-dialog" class="easyui-dialog" data-options="resizable:false, modal:true, closed: true, closable: false,">
        <div style="margin: 15px 15px 15px 15px;">
            <span>请您再次确认导出Excel!</span>
        </div>
        <div style="margin: 15px 15px 15px 15px;">
            <form id="form2">
                <span>请选择发单员：</span>
                <input class="easyui-combobox" id="Selectable" name="Bank" panelHeight="120"
                                data-options="required:true,editable:false" style="height: 30px; width: 180px"" />
            </form>
        </div>
    </div>--%>

</body>
</html>
