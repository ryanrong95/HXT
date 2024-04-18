using Needs.Erp.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.Admin.Plat.Models
{
    public partial class Admin
    {
        public Declaration Declaration
        {
            get
            {
                return new Declaration(this);
            }
        }
    }
    public class Declaration
    {
        IGenericAdmin Admin;

        public Declaration(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        public Ccs.Services.Views.DeclarationNoticesView DelareNotice
        {
            get
            {
                return new Ccs.Services.Views.DeclarationNoticesView();
            }
        }

        public Ccs.Services.Views.DeclarationNoticesWaitView DelareNoticeWait
        {
            get
            {
                return new Ccs.Services.Views.DeclarationNoticesWaitView();
            }
        }

        public Ccs.Services.Views.DecNoticeListView DecNoticeListView
        {
            get
            {
                return new Ccs.Services.Views.DecNoticeListView();
            }
        }

        public Ccs.Services.Views.PackingListView PackingListView
        {
            get
            {
                return new Ccs.Services.Views.PackingListView();
            }
        }

        public Ccs.Services.Views.DeclarationNoticesMakedView DelareNoticeMaked
        {
            get
            {
                return new Ccs.Services.Views.DeclarationNoticesMakedView();
            }
        }

        public Ccs.Services.Views.DeclarationNoticeItemsView DeclareNotictItem
        {
            get
            {
                return new Ccs.Services.Views.DeclarationNoticeItemsView();
            }
        }

        public Ccs.Services.Views.DecHeadsView DeclareHead
        {
            get
            {
                return new Ccs.Services.Views.DecHeadsView();
            }
        }

        public Ccs.Services.Views.DecHeadsListView DeclareHeadList
        {
            get
            {
                return new Ccs.Services.Views.DecHeadsListView();
            }
        }

        public Ccs.Services.Views.DecHeadsDraftListView DeclareHeadDraftList
        {
            get
            {
                return new Ccs.Services.Views.DecHeadsDraftListView();
            }
        }

        public Ccs.Services.Views.DecHeadsMakedListView DeclareHeadMakedList
        {
            get
            {
                return new Ccs.Services.Views.DecHeadsMakedListView();
            }
        }

        public Ccs.Services.Views.DecHeadsImportedListView DeclareHeadImportedList
        {
            get
            {
                return new Ccs.Services.Views.DecHeadsImportedListView();
            }
        }
        /// <summary>
        /// 未上传的报关单
        /// </summary>
        public Ccs.Services.Views.UnUploadDecHeadsListView UnUploadDecHeadsList
        {
            get
            {
                return new Ccs.Services.Views.UnUploadDecHeadsListView();
            }
        }

        public Ccs.Services.Views.DecHeadsExcelImportedListView ExcelUploadDecHeadsList
        {
            get
            {
                return new Ccs.Services.Views.DecHeadsExcelImportedListView();
            }
        }

        /// <summary>
        /// 报关单--缴税
        /// </summary>
        public Ccs.Services.Views.DecTaxView DecTax
        {
            get
            {
                return new Ccs.Services.Views.DecTaxView();
            }
        }


        public Ccs.Services.Views.UploadedTaxFlowListView UploadedTaxFlowListView
        {
            get
            {
                return new Ccs.Services.Views.UploadedTaxFlowListView();
            }
        }

        public Ccs.Services.Views.DecTracesView DecTrace
        {
            get
            {
                return new Ccs.Services.Views.DecTracesView();
            }
        }

        public Ccs.Services.Views.DecListsView DeclareList
        {
            get
            {
                return new Ccs.Services.Views.DecListsView();
            }
        }

        public Ccs.Services.Views.DecOriginListsView DecOriginList
        {
            get
            {
                return new Ccs.Services.Views.DecOriginListsView();
            }
        }

        public Ccs.Services.Views.DecGoodsLimitsView DecGoodsLimits
        {
            get
            {
                return new Ccs.Services.Views.DecGoodsLimitsView();
            }
        }

        public Ccs.Services.Views.DecLicenseDocusView DeclareLicenseDocus
        {
            get
            {
                return new Ccs.Services.Views.DecLicenseDocusView();
            }
        }

        public Ccs.Services.Views.DecContainersView DeclareContainer
        {
            get
            {
                return new Ccs.Services.Views.DecContainersView();
            }
        }

        public Ccs.Services.Views.DecSearchListView DeclareSearchView
        {
            get
            {
                return new Ccs.Services.Views.DecSearchListView();
            }
        }

        public Ccs.Services.Views.EdocRealationsView EdcoRealation
        {
            get
            {
                return new Ccs.Services.Views.EdocRealationsView();
            }
        }

        public Ccs.Services.Views.DecNoticeVoyagesView DecNoticeVoyagesView
        {
            get
            {
                return new Ccs.Services.Views.DecNoticeVoyagesView();
            }
        }

        public Ccs.Services.Views.DecHeadSpecialTypesRoleView DecHeadSpecialTypes
        {
            get
            {
                return new Ccs.Services.Views.DecHeadSpecialTypesRoleView();
            }
        }
    }
}
