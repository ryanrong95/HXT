<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Tempplate.aspx.cs" Inherits="Yahv.Csrm.WebApp.Crm.Contracts.Tempplate" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <style>
        .container {
            color: black;
            line-height: 1.8;
            word-wrap: break-word;
            word-break: break-all;
            text-align: justify;
            padding: 20px 200px 0px 200px;
            overflow-y: auto;
            display: block;
            height: 788px;
        }

        .form-control {
            display: inline;
            margin-top: 2px;
        }

        .img-responsive {
            display: inline;
            margin-top: 10px;
            margin-bottom: 10px;
        }

        .h1, h1 {
            font-size: 24px;
        }

        .h2, h2 {
            font-size: 20px;
        }

        .h3, h3, .h4, h4 {
            font-size: 18px;
        }

        .h5, h5, .h6, h6 {
            font-size: 14px;
        }

        .h1, .h2, .h3, .h4, .h5, .h6, h1, h2, h3, h4, h5, h6 {
            line-height: 1.8;
            font-weight: bold;
        }

        .font-size-xx-small {
            font-size: 6px;
        }

        .font-size-x-small {
            font-size: 8px;
        }

        .font-size-small {
            font-size: 12px;
        }

        .font-size-normal {
            font-size: 14px;
        }

        .font-size-medium {
            font-size: 16px;
        }

        .font-size-large {
            font-size: 18px;
        }

        .font-size-x-large {
            font-size: 24px;
        }

        .font-size-xx-large {
            font-size: 28px;
        }

        td > p {
            margin: 0px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <%

        Dictionary<string, string> dic = this.Model.Infor as Dictionary<string, string>;
    %>

    <div class="container">
        <p style="text-align: center;">
            <span class="font-size-xx-large"><b>供应链服务协议</b></span>
        </p>
        <p style="text-align: right;">
            <span class="font-size-large"><b>编号：『CX-THTY-20170101』</b></span>
        </p>
        <br />
        <p>
            <span class="font-size-normal"><b>甲方（委托方）：<%=dic["ClientName"] %></b></span>
        </p>
        <p>
            <span class="font-size-normal">注册地址：<%=dic["A_RegAddress"] %></span>
        </p>
        <p>
            <span class="font-size-normal">法定代表人：<%=dic["A_Corporation"] %></span>
        </p>
        <p>
            <span class="font-size-normal"><b>乙方（受托方）：<%=dic["B_CompanyName"] %></b></span>
        </p>
        <p>
            <span class="font-size-normal">注册地址：<%=dic["B_RegAddress"] %></span>
        </p>
        <p>
            <span class="font-size-normal">办公地址：<%=dic["B_RegAddress"] %></span>
        </p>
        <p>
            <span class="font-size-normal">法定代表人：<%=dic["B_Corporation"] %></span>
        </p>
        <p>
            <span class="font-size-normal">&nbsp;&nbsp;&nbsp;&nbsp;根据《中华人民共和国对外贸易法》、《中华人民共和国合同法》及其他相关法律法规的规定，甲、乙双方经友好协商，就甲方委托乙方代理进口包括但不限于【电子元器等】(以下称“货物”)事宜，达成协议如下：</span>
        </p>
        <p>
            <span class="font-size-normal"><b>第一条进口代理</b></span>
        </p>
        <p>
            <span class="font-size-normal">&nbsp;&nbsp;&nbsp;&nbsp;甲方就每批需委托乙方代理进口的货物编制《委托进口货物确认单》，该《委托进口货物确认单》需经乙方书面认可后，乙方方提供本协议第三条约定的从【深圳】口岸代理进口货物的供应链服务。
            </span>
        </p>
        <p>
            <span class="font-size-normal"><b>第二条甲方责任和义务</b></span>
        </p>
        <p>
            <span class="font-size-normal">2.1甲方必须在乙方报关前及时向乙方提供完整的进口货物单证和资料（包括货物产品名称、规格型号、数量、进口单价、产地、装箱情况等）并保证其真实、准确；如海关、国检对乙方的进口申报提出质疑，甲方应当按要求提供采购订单、原厂发票、贸易关系说明、样品、运保单、资金结算凭证等资料；若因甲方提供的货物单证、资料与货物的实际情况不符及甲方其他原因而导致乙方报关、清关等过程中遭受任何损失的，甲方应负责足额赔偿；如因甲方货物未在香港完成进口清关程序，产生相对应的罚款由甲方承担。
            </span>
        </p>

        <p>
            <span class="font-size-normal">2.2甲方应按照本协议第四条的约定，及时将购买货物所需的全部货款、代理费、仓前费及委托乙方代为支付的各项费用付至乙方帐户。
            </span>
        </p>
        <p>
            <span class="font-size-normal">2.3除本协议第5.3条款中所述的货物责任之外，甲方不得就货物价格发生变动、货物质量纠纷、货物知识产权纠纷、货物安装、维修、退货、换货、迟延交货、不交货等其他任何纠纷或事宜向乙方要求索赔或要求乙方承担任何责任、履行任何义务。甲方如因与境外供货商或其他方解决上述纠纷或事宜导致乙方支付有关费用或遭受任何损失的，甲方须给予乙方充分补偿。</span>
        </p>

        <p>
            <span class="font-size-normal"><b>第三条乙方责任和义务</b></span>
        </p>
        <p>
            <span class="font-size-normal">3.1乙方按照甲方确认的《委托进口货物确认单》内容代甲方办理货物的进口报关手续。</span>
        </p>
        <p>
            <span class="font-size-normal">3.2乙方根据本协议以自己或者甲方代理人的名义单独或者与甲方一起与境外供货商或其他第三方签订进口合同（以下称“进口合同”）。
            </span>
        </p>
        <p>
            <span class="font-size-normal">3.3乙方在收到甲方按照本协议第四条支付的货款后，代甲方办理对外付汇手续。乙方按照甲方确认的《付款/汇委托确认书》内容代甲方对外付款。甲方须定时将业务委托文件原件邮寄给乙方。</span>
        </p>
        <p>
            <span class="font-size-normal">3.4甲方授权甲方人员（见附件 《授权书》）对《委托进口货物确认单》及《付款/汇委托确认书》进行确认。如甲方授权人员发生变化，须提前一周邮件或书面知会乙方，否则因甲方授权人员离职或其他原因导致的在货物委托进口、付汇/付款及收发货等方面产生的损失由甲方承担。
            </span>
        </p>
        <p>
            <span class="font-size-normal">3.5NEWMAY INTERNATIONAL TRADE LIMITED（以下简称“NEWMAY”）是乙方为进行国际贸易及跨境物流行业所必需的仓储、交易等活动而依法合作的香港公司。甲方同意其所委托的代理进口货物的境外部分将由NEWMAY负责实施。当甲方要求乙方先行支付境外货款时，乙方指定NEWMAY向甲方指定供货商支付美金或港币货款。当NEWMAY在境外支付货款给甲方指定的供货方时，甲方便不得以任何理由取消当次委托。否则，甲方同意向乙方支付全部损失。</span>
        </p>

        <p>
            <span class="font-size-normal">3.6乙方提供的本条约定的服务统称“供应链服务”，乙方有权将本协议约定的供应链服务分包给其它第三方。</span>
        </p>

        <p>
            <span class="font-size-normal"><b>第四条《进口合同》货款、代理费、通关及其支付和结算</b></span>
        </p>
        <p>
            <span class="font-size-normal">4.1有关货款、代理费、通关及其支付和结算的具体报价参考协议的<span style="background-color: #ff0000">附件2</span>。综合费具体包括：从乙方指定的香港仓库至甲方深圳指定地点的进口环节中所发生的香港至深圳的中港运输费、装卸费、保险费、乙方指定的香港仓库及乙方深圳仓库的仓储费、香港出口报关费、深圳进口报关费、打单费、查车费以及深圳地区的一次送货费。未在本款列明的费用（包括但不限于：法定商检费、银行费用、仓前费、国际运费/保险费/码头费/提货费、香港入口报关费、乙方仓库超期仓储费、深圳至甲方指定的其它目的地运输费等）由甲方自行承担。</span>
        </p>

        <p>
            <span class="font-size-normal">4.2甲方应按附件2约定给乙方支付货款，乙方按约定开具发票。</span>
        </p>

        <p>
            <span class="font-size-normal">4.3甲方应在有效期内安排付汇，付汇手续费实结。进口对外付汇额度90天内有效。</span>
        </p>

        <p>
            <span class="font-size-normal">4.4如因海关对甲方进口商品归类或货物价格进行重新核定，而导致增加的进口环节关税和增值税全部由甲方承担，甲方应在收到乙方书面通知之日起三日内依据海关出具的《补税通知书》中确认金额向乙方支付相应款项并赔偿乙方因此遭受的任何损失。</span>
        </p>

        <p>
            <span class="font-size-normal">第五条货物交付、验收以及风险承担</span>
        </p>

        <p>
            <span class="font-size-normal">5.1乙方在收到货物时，乙方有权对货物外包装做必要的检查。若外包装不是原厂包装而是其他包装（包括但不限于透明胶纸等），乙方可对每一型号至少打开一箱货物检验内在货物与外包装描述的货物是否一致；若外包装是原厂包装，乙方无需打开包装验货。若外包装完好，乙方或乙方指定的收货方可视其内在货物完好，并按包装箱标示及装箱清单验收；若外包装破损或有其他异样，乙方应及时通知甲方，由甲方决定相关货物进一步地处理并书面告知乙方，由此产生的费用和风险由甲方承担。</span>
        </p>

        <p>
            <span class="font-size-normal">5.2乙方向甲方或其指定收货方交货的，若外包装完好，甲方或甲方指定收货方不得向乙方提出异议；若甲方或甲方指定收货方收货验收时发现异样的，应由甲方与其指定境外供货商协商解决，与乙方无关。在此情况下，乙方可向甲方提供协助。</span>
        </p>

        <p>
            <span class="font-size-normal">5.3从乙方收到货物时起至乙方将货物交付给甲方或甲方指定收货方为止，货物由于乙方不当装运或者乙方提供供应链服务中的其他过失所遭受的损坏、灭失由乙方负责，除此之外，乙方不再对甲方承担与之相关的任何其它责任或风险。</span>
        </p>

        <p>
            <span class="font-size-normal">5.4从乙方收到货物时起至乙方将货物交付给甲方或甲方指定收货方为止，由甲方委托乙方办理货物保险。委托乙方投保的，乙方按照保险公司的赔偿金额赔偿给甲方，但甲方有责任和义务协助乙方向保险公司索赔，因甲方无法提供必要资料、迟延提供资料或无法配合保险公司查勘等事宜导致无法赔付或影响赔付金额的, 甲方应承担相应责任。</span>
        </p>

        <p>
            <span class="font-size-normal">第六条合同的变更</span>
        </p>
        <p>
            <span class="font-size-normal">乙方根据本协议与甲方指定境外供货商签署《进口合同》后，除乙方和境外供货商均书面同意的情况下，甲方不得要求更改《进口合同》的内容。</span>
        </p>
        <p>
            <span class="font-size-normal">第七条保密条款</span>
        </p>
        <p>
            <span class="font-size-normal">本协议任何一方均有义务保守对方商业秘密，未经对方的事先书面同意，不得向任何第三方披露对方商业秘密，亦不得将该等商业秘密用于履行本协议之外的其他目的。但根据有关政府机关或法院要求透露的不在此限，保密义务的有效期限为本协议签署之日起五年。</span>
        </p>
        <p>
            <span class="font-size-normal">第八条违约责任</span>
        </p>
        <p>
            <span class="font-size-normal">8.1如甲方未按照本协议第四条的规定及时支付有关款项，应按逾期支付款项的0.1％/日的标准向乙方支付滞纳金，且乙方有权暂停代理行为，由此造成的损失由甲方承担；逾期付款超过30日的，乙方有权解除本协议并要求甲方赔偿因此遭受的一切损失。</span>
        </p>

        <p>
            <span class="font-size-normal">8.2在甲方按照本合同第四条的规定支付全部款项前，乙方有权留置代理甲方进口的货物。若甲方迟延付款超过30日，乙方有权变卖留置的货物，并就变卖后的价款优先受偿。</span>
        </p>

        <p>
            <span class="font-size-normal">8.3如甲方未能及时提取货物，应承担延迟提货期间发生的货物仓储费及其它费用；甲方逾期超过90日不提货的，乙方有权解除本协议并变卖未提取货物，并就变卖后的价款优先受偿。</span>
        </p>

        <p>
            <span class="font-size-normal">8.4甲方拒绝履行本协议下的义务或乙方为履行本协议及《进口合同》而提出的合理要求，而导致乙方不能对外履行《进口合同》或提供本协议下供应链服务的，由此产生的一切损失由甲方全部承担，如造成乙方损失的，甲方应足额赔偿。</span>
        </p>

        <p>
            <span class="font-size-normal">第九条争议的解决</span>
        </p>

        <p>
            <span class="font-size-normal">9.1境外供货商存在违约行为，甲方决定向境外供货商索赔的，乙方应根据甲方的书面通知，协助甲方向境外供货商提出索赔。乙方因此而支付的有关费用或者受到的任何损失，应由甲方给予充分补偿。</span>
        </p>

        <p>
            <span class="font-size-normal">9.2甲乙双方同意，在执行本协议过程中所发生的纠纷应首先通过友好协商解决；协商不成的，任何一方均可向深圳市福田区人民法院提请诉讼，本协议适用中华人民共和国法律。</span>
        </p>

        <p>
            <span class="font-size-normal">9.3如因非乙方原因，境外供货商向乙方提起仲裁或诉讼时，乙方应及时书面通知甲方，甲方有义务协助乙方搜集证据，并应为有关交涉提供必要的支持和方便。乙方因此而支付的有关费用或者受到的任何损失，应由甲方给予充分补偿。</span>
        </p>

        <p>
            <span class="font-size-normal">第十条其他约定</span>
        </p>

        <p>
            <span class="font-size-normal">10.1本协议有效期自<%=dic["StartDate"] %>起至<%=dic["EndDate"] %>止，为期【<%=dic["Years"] %>】年。本协议到期前，如甲乙双方未重新签订，则本协议有效无限期自动顺延。若合同变更，则双方签署《补充协议》为主合同的有效附件，具有同等法律地位。</span>
        </p>

        <p>
            <span class="font-size-normal">10.2本协议一式两份，甲乙双方各持一份，具有同等法律效力。本协议由双方加盖公章（或合同专用章）后生效。</span>
        </p>

        <p>
            <span class="font-size-normal"><b>附件1：授权书</b></span>
        </p>
        <p>
            <span class="font-size-normal"><b>甲方：<%=dic["ClientName"] %></b></span>
        </p>
        <p>
            <span class="font-size-normal">签约代表：</span>
        </p>
        <p>
            <span class="font-size-normal">签约日期：</span>
        </p>

        <p>
            <span class="font-size-normal"><b>乙方：<%=dic["B_CompanyName"] %></b></span>
        </p>
        <p>
            <span class="font-size-normal">签约代表：</span>
        </p>
        <p>
            <span class="font-size-normal">签约日期：</span>
        </p>




        <p style="text-align: center;">
            <span class="font-size-xx-large"><b>授 权 书</b></span>
        </p>
        <p>
            <span class="font-size-normal">&nbsp;&nbsp;&nbsp;&nbsp;兹授权我司以下人员在与深圳市芯达通供应链管理有限公司的业务往来中所签署的业务单证（《委托进口货物确认单》、《付汇委托书》、《货物签收单》等）均视为有效。本授权书有效期限与供应链服务协议一致；以上授权人员如有更改，我司将以书面形式通知。</span>
        </p>

        <p>
            <span class="font-size-normal">&nbsp;&nbsp;&nbsp;&nbsp;授权代表签字具体如下：</span>
        </p>



        <table class="table" style="border-collapse: collapse; border-style: none;">
            <tr>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 10%">
                    <p style="text-align: center;">
                        <span class="font-size-normal">序号</span>
                    </p>
                </td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 15%">
                    <p style="text-align: center;">
                        <span class="font-size-normal">姓名</span>
                    </p>
                </td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 20%">
                    <p style="text-align: center;">
                        <span class="font-size-normal">签字样式</span>
                    </p>
                </td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 25%">
                    <p style="text-align: center;">
                        <span class="font-size-normal">授权范围</span>
                    </p>
                </td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 25%">
                    <p style="text-align: center;">
                        <span class="font-size-normal">联系电话及邮箱</span>
                    </p>
                </td>
            </tr>
            <tr>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 15%">
                    <p style="text-align: center;">
                        <span class="font-size-normal">1</span>
                    </p>

                </td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 15%"></td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 15%"></td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 15%">进口委托确认及货物相关信息确认	</td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 15%"></td>
            </tr>
            <tr>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 15%">
                    <p style="text-align: center;">
                        <span class="font-size-normal">2</span>
                    </p>
                </td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 15%"></td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 15%"></td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 15%">对帐确认	</td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 15%"></td>
            </tr>
            <tr>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 15%">
                    <p style="text-align: center;">
                        <span class="font-size-normal">3</span>
                    </p>
                </td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 15%"></td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 15%"></td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 15%">付汇委托确认	</td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 15%"></td>
            </tr>
            <tr>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 15%">
                    <p style="text-align: center;">
                        <span class="font-size-normal">4</span>
                    </p>
                </td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 15%"></td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 15%"></td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 15%">货物签收</td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 15%"></td>
            </tr>
        </table>



        <p>
            <span class="font-size-normal"><b>授权人：</b></span>
        </p>
        <p>
            <span class="font-size-normal"><b>（加盖公章）</b></span>
        </p>
        <p>
            <span class="font-size-normal"><b>2017年  月  日</b></span>
        </p>

        <p style="text-align: center;">
            <span class="font-size-xx-large"><b>附件2</b></span>
        </p>


        <p>
            <span class="font-size-normal">协议开始日期 <span style="text-decoration: underline;"><%=dic["StartDate"] %></span>  ，
                协议结束日期 <span style="text-decoration: underline;"><%=dic["EndDate"] %> </span>。
                信用等级 <span style="text-decoration: underline;"><%=dic["Grade"] %></span>  。
            </span>
        </p>
        <p>
            <span class="font-size-normal">一、结算公式</span>
        </p>
        <p>
            <span class="font-size-normal">人民币开票完税价=货款+增值税+代理费+关税（如有）+其它杂费。
            </span>
        </p>
        <p>
            <span class="font-size-normal">货款=美金货值*实时汇率；增值税=美金货值*海关汇率*1.002*0.17；
            </span>
        </p>
        <p>
            <span class="font-size-normal">货款=美金货值*实时汇率；增值税=美金货值*海关汇率*1.002*0.17；（其中千分之二为海关要求申报的运费、保费、杂费）；
            </span>
        </p>
        <p>
            <span class="font-size-normal">代理费=美金货值*汇率*费率*<%=dic["InvoicePoint"] %>；
            </span>
        </p>
        <p>
            <span class="font-size-normal">关税=美金货值*海关汇率*1.002*关税率; 杂费=实报实销*<%=dic["InvoicePoint"] %>。
            </span>
        </p>
        <p>
            <span class="font-size-normal">实时汇率按照进口当天的中国银行上午10点后第一个相应外汇卖出价为准。
            </span>
        </p>
        <p>
            <span class="font-size-normal">海关汇率按照进口当天海关汇率为准。
            </span>
        </p>
        <p>
            <span>二、具体报价及结算约定
            </span>
        </p>
        <p>
            <span class="font-size-normal">（1）货款：  <%=dic["GoodsPayExchange90"] %>90天内换汇； <%=dic["GoodsPayExchangePre"] %>预换汇；<br />
                <%=dic["GoodsIsMonth"] %>月结，间隔 <%=dic["GoodsMonth"] %> 个月的 <%=dic["GoodsMonthDay"] %> 日，垫付上限 <%=dic["GoodsCredits1"] %> 元；<br />
                <%=dic["GoodsIsCustom"] %>约定期限， <%=dic["GoodsCustomDay"] %> 天，垫付上限 <%=dic["GoodsCredits2"] %> 元；<br />
汇率：  <%=dic["GoodsIsCustoms"] %>海关汇率；<%=dic["GoodsIsFloating"] %>实时汇率；<%=dic["GoodsIsFixed"] %>固定汇率；<%=dic["GoodsIsPreset"] %>预设汇率。
            </span>
        </p>
        <p>
            <span>（2）税款：  <%=dic["TaxesIsMonth"] %>月结：间隔 <%=dic["TaxesMonth"] %> 个月的 <%=dic["TaxesMonthDay"] %> 日，垫付上限 <%=dic["TaxesCredits1"] %> 元；<br />
                <%=dic["TaxesIsCustom"] %>约定期限， <%=dic["TaxesCustomDay"] %> 天，垫付上限 <%=dic["TaxesCredits2"] %> 元<br />
汇率：  <%=dic["TaxesIsCustoms"] %>海关汇率；<%=dic["TaxesIsFloating"] %>实时汇率；<%=dic["TaxesIsFixed"] %>固定汇率；<%=dic["TaxesIsPreset"] %>预设汇率。
            </span>
        </p>
        <p>
            <span>（3）代理费： 费率 <%=dic["ServiceChargePoint"] %> %，最低收费 <%=dic["MinimumAgent"] %> 元/单。<br />
                <%=dic["AgentIsMonth"] %>月结：间隔 <%=dic["AgentMonth"] %> 个月的 <%=dic["AgentMonthDay"] %> 日，垫付上限 <%=dic["AgentCredits1"] %> 元；<br />
                <%=dic["AgentIsCustom"] %>约定期限， <%=dic["AgentCustomDay"] %> 天，垫付上限 <%=dic["AgentCredits2"] %> 元<br />
汇率：  <%=dic["AgentIsCustoms"] %>海关汇率；<%=dic["AgentIsFloating"] %>实时汇率；<%=dic["AgentIsFixed"] %>固定汇率；<%=dic["AgentIsPreset"] %>预设汇率。
            </span>
        </p>
        <p>
            <span>（4）杂费：  <%=dic["ExtrasIsMonth"] %>月结：间隔 <%=dic["ExtrasMonth"] %> 个月的 <%=dic["ExtrasMonthDay"] %> 日，垫付上限 <%=dic["ExtrasCredits1"] %> 元；<br />
                <%=dic["ExtrasIsCustom"] %>约定期限， <%=dic["ExtrasCustomDay"] %> 天，垫付上限 <%=dic["ExtrasCredits2"] %> 元<br />
                汇率：  <%=dic["ExtrasIsCustoms"] %>海关汇率；<%=dic["ExtrasIsFloating"] %>实时汇率；<%=dic["ExtrasIsFixed"] %>固定汇率；<%=dic["ExtrasIsPreset"] %>预设汇率。
            </span>
        </p>
        <p>
            <span>三、开具发票
            </span>
        </p>
        <p>
            <span class="font-size-normal">（1）开票类型：<%=dic["InvoiceType"] %>签署内贸合同，开票增值税专用发票；<%=dic["InvoiceTypeService"] %>海关发票+服务票发票。
            </span>
        </p>


        <p style="text-align: center;">
            <span class="font-size-x-large"><b>附件2</b></span>
        </p>
        <p style="text-align: center;">
            <span class="font-size-x-large"><b>超重货物额外收费明细表</b></span>
        </p>
        <p style="text-align: right;">
            <span class="font-size-large"><b>币种：RMB</b></span>
        </p>

        <table class="table" style="border-collapse: collapse; border-style: none;">
            <tr>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 5%">
                    <p style="text-align: center;">
                        <span class="font-size-normal">重量（KGS）/货值（USD）</span>
                    </p>
                </td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 5%">
                    <p style="text-align: center;">
                        <span class="font-size-normal">100</span>
                    </p>
                </td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 5%">
                    <p style="text-align: center;">
                        <span class="font-size-normal">200</span>
                    </p>
                </td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 5%">
                    <p style="text-align: center;">
                        <span class="font-size-normal">300</span>
                    </p>

                </td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 5%">
                    <p style="text-align: center;">
                        <span class="font-size-normal">400</span>
                    </p>
                </td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 5%">
                    <p style="text-align: center;">
                        <span class="font-size-normal">500</span>
                    </p>
                </td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 5%">
                    <p style="text-align: center;">
                        <span class="font-size-normal">600</span>
                    </p>
                </td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 5%">
                    <p style="text-align: center;">
                        <span class="font-size-normal">700</span>
                    </p>

                </td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 5%">
                    <p style="text-align: center;">
                        <span class="font-size-normal">800</span>
                    </p>
                </td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 5%">
                    <p style="text-align: center;">
                        <span class="font-size-normal">900</span>
                    </p>
                </td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 5%">
                    <p style="text-align: center;">
                        <span class="font-size-normal">1000</span>
                    </p>
                </td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 5%">
                    <p style="text-align: center;">
                        <span class="font-size-normal">1500</span>
                    </p>
                </td>
            </tr>
            <tr>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 5%">2W以下</td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 5%">0</td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 5%">100</td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 5%">200</td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 5%">300</td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 5%">400</td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 5%">500</td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 5%">700</td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 5%">800</td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 5%">900</td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 5%">1200</td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 5%">1400</td>
            </tr>
            <tr>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 5%">4W</td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 5%">0</td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 5%">0</td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 5%">100</td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 5%">200</td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 5%">300</td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 5%">400</td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 5%">600</td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 5%">700</td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 5%">800</td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 5%">1200</td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 5%">1400</td>
            </tr>
            <tr>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 5%">6W</td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 5%">0</td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 5%">0</td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 5%">0</td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 5%">0</td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 5%">100</td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 5%">200</td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 5%">200</td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 5%">300</td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 5%">400</td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 5%">400</td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 5%">500</td>
            </tr>
            <tr>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 5%">8W</td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 5%">0</td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 5%">0</td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 5%">0</td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 5%">0</td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 5%">0</td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 5%">0</td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 5%">100</td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 5%">200</td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 5%">300</td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 5%">400</td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 5%">400</td>
            </tr>
            <tr>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 5%">10W</td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 5%">0</td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 5%">0</td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 5%">0</td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 5%">0</td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 5%">0</td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 5%">0</td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 5%">0</td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 5%">0</td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 5%">0</td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 5%">100</td>
                <td style="border-width: 1px; border-style: solid; border-color: #000000; width: 5%">200</td>
            </tr>
        </table>

        <p>
            <span class="font-size-normal">1500KG以上的双方商议决定。</span>
        </p>
    </div>
</asp:Content>
