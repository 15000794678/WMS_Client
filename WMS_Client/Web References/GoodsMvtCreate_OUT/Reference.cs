﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

// 
// 此源代码是由 Microsoft.VSDesigner 4.0.30319.42000 版自动生成。
// 
#pragma warning disable 1591

namespace Phicomm_WMS.GoodsMvtCreate_OUT {
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.Xml.Serialization;
    using System.ComponentModel;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2556.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="GoodsMvtCreate_OUTBinding", Namespace="http://phicomm.com/WMS/PI018/GoodsMvtCreate")]
    public partial class GoodsMvtCreate_OUTService : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback GoodsMvtCreate_OUTOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public GoodsMvtCreate_OUTService() {
            this.Url = global::Phicomm_WMS.Properties.Settings.Default.WMS_Client_GoodsMvtCreate_OUT_GoodsMvtCreate_OUTService;
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        public new string Url {
            get {
                return base.Url;
            }
            set {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true) 
                            && (this.useDefaultCredentialsSetExplicitly == false)) 
                            && (this.IsLocalFileSystemWebService(value) == false))) {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }
        
        public new bool UseDefaultCredentials {
            get {
                return base.UseDefaultCredentials;
            }
            set {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        /// <remarks/>
        public event GoodsMvtCreate_OUTCompletedEventHandler GoodsMvtCreate_OUTCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://sap.com/xi/WebService/soap1.1", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("GoodsMvtCreate_res", Namespace="http://phicomm.com/WMS/PI018/GoodsMvtCreate")]
        public GoodsMvtCreate_res GoodsMvtCreate_OUT([System.Xml.Serialization.XmlElementAttribute(Namespace="http://phicomm.com/WMS/PI018/GoodsMvtCreate")] GoodsMvtCreate_req GoodsMvtCreate_req) {
            object[] results = this.Invoke("GoodsMvtCreate_OUT", new object[] {
                        GoodsMvtCreate_req});
            return ((GoodsMvtCreate_res)(results[0]));
        }
        
        /// <remarks/>
        public void GoodsMvtCreate_OUTAsync(GoodsMvtCreate_req GoodsMvtCreate_req) {
            this.GoodsMvtCreate_OUTAsync(GoodsMvtCreate_req, null);
        }
        
        /// <remarks/>
        public void GoodsMvtCreate_OUTAsync(GoodsMvtCreate_req GoodsMvtCreate_req, object userState) {
            if ((this.GoodsMvtCreate_OUTOperationCompleted == null)) {
                this.GoodsMvtCreate_OUTOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGoodsMvtCreate_OUTOperationCompleted);
            }
            this.InvokeAsync("GoodsMvtCreate_OUT", new object[] {
                        GoodsMvtCreate_req}, this.GoodsMvtCreate_OUTOperationCompleted, userState);
        }
        
        private void OnGoodsMvtCreate_OUTOperationCompleted(object arg) {
            if ((this.GoodsMvtCreate_OUTCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GoodsMvtCreate_OUTCompleted(this, new GoodsMvtCreate_OUTCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
        
        private bool IsLocalFileSystemWebService(string url) {
            if (((url == null) 
                        || (url == string.Empty))) {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024) 
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0))) {
                return true;
            }
            return false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2612.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://phicomm.com/WMS/PI018/GoodsMvtCreate")]
    public partial class GoodsMvtCreate_req {
        
        private string i_GOACTIONField;
        
        private string i_REFDOCField;
        
        private GoodsMvtCreate_reqIS_GOODSMVT_HEADER iS_GOODSMVT_HEADERField;
        
        private GoodsMvtCreate_reqItem[] iT_GOODSMVT_ITEMField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string I_GOACTION {
            get {
                return this.i_GOACTIONField;
            }
            set {
                this.i_GOACTIONField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string I_REFDOC {
            get {
                return this.i_REFDOCField;
            }
            set {
                this.i_REFDOCField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public GoodsMvtCreate_reqIS_GOODSMVT_HEADER IS_GOODSMVT_HEADER {
            get {
                return this.iS_GOODSMVT_HEADERField;
            }
            set {
                this.iS_GOODSMVT_HEADERField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlArrayItemAttribute("item", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=false)]
        public GoodsMvtCreate_reqItem[] IT_GOODSMVT_ITEM {
            get {
                return this.iT_GOODSMVT_ITEMField;
            }
            set {
                this.iT_GOODSMVT_ITEMField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2612.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://phicomm.com/WMS/PI018/GoodsMvtCreate")]
    public partial class GoodsMvtCreate_reqIS_GOODSMVT_HEADER {
        
        private string pSTNG_DATEField;
        
        private string dOC_DATEField;
        
        private string hEADER_TXTField;
        
        private string bILL_OF_LADINGField;
        
        private string gR_GI_SLIP_NOField;
        
        private string rEF_DOC_NOField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string PSTNG_DATE {
            get {
                return this.pSTNG_DATEField;
            }
            set {
                this.pSTNG_DATEField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string DOC_DATE {
            get {
                return this.dOC_DATEField;
            }
            set {
                this.dOC_DATEField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string HEADER_TXT {
            get {
                return this.hEADER_TXTField;
            }
            set {
                this.hEADER_TXTField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string BILL_OF_LADING {
            get {
                return this.bILL_OF_LADINGField;
            }
            set {
                this.bILL_OF_LADINGField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string GR_GI_SLIP_NO {
            get {
                return this.gR_GI_SLIP_NOField;
            }
            set {
                this.gR_GI_SLIP_NOField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string REF_DOC_NO {
            get {
                return this.rEF_DOC_NOField;
            }
            set {
                this.rEF_DOC_NOField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2612.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://phicomm.com/WMS/PI018/GoodsMvtCreate")]
    public partial class GoodsMvtCreate_res {
        
        private string e_MATERIALDOCUMENTField;
        
        private string e_MATDOCUMENTYEARField;
        
        private GoodsMvtCreate_resES_RETURN eS_RETURNField;
        
        private GoodsMvtCreate_resItem[] iT_GOODSMVT_ITEMField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string E_MATERIALDOCUMENT {
            get {
                return this.e_MATERIALDOCUMENTField;
            }
            set {
                this.e_MATERIALDOCUMENTField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string E_MATDOCUMENTYEAR {
            get {
                return this.e_MATDOCUMENTYEARField;
            }
            set {
                this.e_MATDOCUMENTYEARField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public GoodsMvtCreate_resES_RETURN ES_RETURN {
            get {
                return this.eS_RETURNField;
            }
            set {
                this.eS_RETURNField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlArrayItemAttribute("item", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=false)]
        public GoodsMvtCreate_resItem[] IT_GOODSMVT_ITEM {
            get {
                return this.iT_GOODSMVT_ITEMField;
            }
            set {
                this.iT_GOODSMVT_ITEMField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2612.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://phicomm.com/WMS/PI018/GoodsMvtCreate")]
    public partial class GoodsMvtCreate_resES_RETURN {
        
        private string tYPEField;
        
        private string idField;
        
        private string nUMBERField;
        
        private string mESSAGEField;
        
        private string lOG_NOField;
        
        private string lOG_MSG_NOField;
        
        private string mESSAGE_V1Field;
        
        private string mESSAGE_V2Field;
        
        private string mESSAGE_V3Field;
        
        private string mESSAGE_V4Field;
        
        private string pARAMETERField;
        
        private string rOWField;
        
        private string fIELDField;
        
        private string sYSTEMField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string TYPE {
            get {
                return this.tYPEField;
            }
            set {
                this.tYPEField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ID {
            get {
                return this.idField;
            }
            set {
                this.idField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string NUMBER {
            get {
                return this.nUMBERField;
            }
            set {
                this.nUMBERField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string MESSAGE {
            get {
                return this.mESSAGEField;
            }
            set {
                this.mESSAGEField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string LOG_NO {
            get {
                return this.lOG_NOField;
            }
            set {
                this.lOG_NOField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string LOG_MSG_NO {
            get {
                return this.lOG_MSG_NOField;
            }
            set {
                this.lOG_MSG_NOField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string MESSAGE_V1 {
            get {
                return this.mESSAGE_V1Field;
            }
            set {
                this.mESSAGE_V1Field = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string MESSAGE_V2 {
            get {
                return this.mESSAGE_V2Field;
            }
            set {
                this.mESSAGE_V2Field = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string MESSAGE_V3 {
            get {
                return this.mESSAGE_V3Field;
            }
            set {
                this.mESSAGE_V3Field = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string MESSAGE_V4 {
            get {
                return this.mESSAGE_V4Field;
            }
            set {
                this.mESSAGE_V4Field = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string PARAMETER {
            get {
                return this.pARAMETERField;
            }
            set {
                this.pARAMETERField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ROW {
            get {
                return this.rOWField;
            }
            set {
                this.rOWField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string FIELD {
            get {
                return this.fIELDField;
            }
            set {
                this.fIELDField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string SYSTEM {
            get {
                return this.sYSTEMField;
            }
            set {
                this.sYSTEMField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2612.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://phicomm.com/WMS/PI018/GoodsMvtCreate")]
    public partial class GoodsMvtCreate_resItem {
        
        private string mATERIALField;
        
        private string pLANTField;
        
        private string sTGE_LOCField;
        
        private string mOVE_TYPEField;
        
        private string sTCK_TYPEField;
        
        private string sPEC_STOCKField;
        
        private string sALES_ORDField;
        
        private string s_ORD_ITEMField;
        
        private string eNTRY_QNTField;
        
        private string eNTRY_UOMField;
        
        private string vENDORField;
        
        private string pO_ITEMField;
        
        private string pO_NUMBERField;
        
        private string uNLOAD_PTField;
        
        private string iTEM_TEXTField;
        
        private string oRDERIDField;
        
        private string rESERV_NOField;
        
        private string rES_ITEMField;
        
        private string mOVE_PLANTField;
        
        private string mOVE_STLOCField;
        
        private string mVT_INDField;
        
        private string mOVE_REASField;
        
        private string nO_MORE_GRField;
        
        private string dELIV_NUMBField;
        
        private string dELIV_ITEMField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string MATERIAL {
            get {
                return this.mATERIALField;
            }
            set {
                this.mATERIALField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string PLANT {
            get {
                return this.pLANTField;
            }
            set {
                this.pLANTField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string STGE_LOC {
            get {
                return this.sTGE_LOCField;
            }
            set {
                this.sTGE_LOCField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string MOVE_TYPE {
            get {
                return this.mOVE_TYPEField;
            }
            set {
                this.mOVE_TYPEField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string STCK_TYPE {
            get {
                return this.sTCK_TYPEField;
            }
            set {
                this.sTCK_TYPEField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string SPEC_STOCK {
            get {
                return this.sPEC_STOCKField;
            }
            set {
                this.sPEC_STOCKField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string SALES_ORD {
            get {
                return this.sALES_ORDField;
            }
            set {
                this.sALES_ORDField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string S_ORD_ITEM {
            get {
                return this.s_ORD_ITEMField;
            }
            set {
                this.s_ORD_ITEMField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ENTRY_QNT {
            get {
                return this.eNTRY_QNTField;
            }
            set {
                this.eNTRY_QNTField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ENTRY_UOM {
            get {
                return this.eNTRY_UOMField;
            }
            set {
                this.eNTRY_UOMField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string VENDOR {
            get {
                return this.vENDORField;
            }
            set {
                this.vENDORField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string PO_ITEM {
            get {
                return this.pO_ITEMField;
            }
            set {
                this.pO_ITEMField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string PO_NUMBER {
            get {
                return this.pO_NUMBERField;
            }
            set {
                this.pO_NUMBERField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string UNLOAD_PT {
            get {
                return this.uNLOAD_PTField;
            }
            set {
                this.uNLOAD_PTField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ITEM_TEXT {
            get {
                return this.iTEM_TEXTField;
            }
            set {
                this.iTEM_TEXTField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ORDERID {
            get {
                return this.oRDERIDField;
            }
            set {
                this.oRDERIDField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string RESERV_NO {
            get {
                return this.rESERV_NOField;
            }
            set {
                this.rESERV_NOField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string RES_ITEM {
            get {
                return this.rES_ITEMField;
            }
            set {
                this.rES_ITEMField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string MOVE_PLANT {
            get {
                return this.mOVE_PLANTField;
            }
            set {
                this.mOVE_PLANTField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string MOVE_STLOC {
            get {
                return this.mOVE_STLOCField;
            }
            set {
                this.mOVE_STLOCField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string MVT_IND {
            get {
                return this.mVT_INDField;
            }
            set {
                this.mVT_INDField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string MOVE_REAS {
            get {
                return this.mOVE_REASField;
            }
            set {
                this.mOVE_REASField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string NO_MORE_GR {
            get {
                return this.nO_MORE_GRField;
            }
            set {
                this.nO_MORE_GRField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string DELIV_NUMB {
            get {
                return this.dELIV_NUMBField;
            }
            set {
                this.dELIV_NUMBField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string DELIV_ITEM {
            get {
                return this.dELIV_ITEMField;
            }
            set {
                this.dELIV_ITEMField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2612.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://phicomm.com/WMS/PI018/GoodsMvtCreate")]
    public partial class GoodsMvtCreate_reqItem {
        
        private string mATERIALField;
        
        private string pLANTField;
        
        private string sTGE_LOCField;
        
        private string mOVE_TYPEField;
        
        private string sTCK_TYPEField;
        
        private string sPEC_STOCKField;
        
        private string sALES_ORDField;
        
        private string s_ORD_ITEMField;
        
        private string eNTRY_QNTField;
        
        private string eNTRY_UOMField;
        
        private string vENDORField;
        
        private string pO_ITEMField;
        
        private string pO_NUMBERField;
        
        private string uNLOAD_PTField;
        
        private string iTEM_TEXTField;
        
        private string oRDERIDField;
        
        private string rESERV_NOField;
        
        private string rES_ITEMField;
        
        private string mOVE_PLANTField;
        
        private string mOVE_STLOCField;
        
        private string mVT_INDField;
        
        private string mOVE_REASField;
        
        private string nO_MORE_GRField;
        
        private string dELIV_NUMBField;
        
        private string dELIV_ITEMField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string MATERIAL {
            get {
                return this.mATERIALField;
            }
            set {
                this.mATERIALField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string PLANT {
            get {
                return this.pLANTField;
            }
            set {
                this.pLANTField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string STGE_LOC {
            get {
                return this.sTGE_LOCField;
            }
            set {
                this.sTGE_LOCField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string MOVE_TYPE {
            get {
                return this.mOVE_TYPEField;
            }
            set {
                this.mOVE_TYPEField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string STCK_TYPE {
            get {
                return this.sTCK_TYPEField;
            }
            set {
                this.sTCK_TYPEField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string SPEC_STOCK {
            get {
                return this.sPEC_STOCKField;
            }
            set {
                this.sPEC_STOCKField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string SALES_ORD {
            get {
                return this.sALES_ORDField;
            }
            set {
                this.sALES_ORDField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string S_ORD_ITEM {
            get {
                return this.s_ORD_ITEMField;
            }
            set {
                this.s_ORD_ITEMField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ENTRY_QNT {
            get {
                return this.eNTRY_QNTField;
            }
            set {
                this.eNTRY_QNTField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ENTRY_UOM {
            get {
                return this.eNTRY_UOMField;
            }
            set {
                this.eNTRY_UOMField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string VENDOR {
            get {
                return this.vENDORField;
            }
            set {
                this.vENDORField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string PO_ITEM {
            get {
                return this.pO_ITEMField;
            }
            set {
                this.pO_ITEMField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string PO_NUMBER {
            get {
                return this.pO_NUMBERField;
            }
            set {
                this.pO_NUMBERField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string UNLOAD_PT {
            get {
                return this.uNLOAD_PTField;
            }
            set {
                this.uNLOAD_PTField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ITEM_TEXT {
            get {
                return this.iTEM_TEXTField;
            }
            set {
                this.iTEM_TEXTField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ORDERID {
            get {
                return this.oRDERIDField;
            }
            set {
                this.oRDERIDField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string RESERV_NO {
            get {
                return this.rESERV_NOField;
            }
            set {
                this.rESERV_NOField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string RES_ITEM {
            get {
                return this.rES_ITEMField;
            }
            set {
                this.rES_ITEMField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string MOVE_PLANT {
            get {
                return this.mOVE_PLANTField;
            }
            set {
                this.mOVE_PLANTField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string MOVE_STLOC {
            get {
                return this.mOVE_STLOCField;
            }
            set {
                this.mOVE_STLOCField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string MVT_IND {
            get {
                return this.mVT_INDField;
            }
            set {
                this.mVT_INDField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string MOVE_REAS {
            get {
                return this.mOVE_REASField;
            }
            set {
                this.mOVE_REASField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string NO_MORE_GR {
            get {
                return this.nO_MORE_GRField;
            }
            set {
                this.nO_MORE_GRField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string DELIV_NUMB {
            get {
                return this.dELIV_NUMBField;
            }
            set {
                this.dELIV_NUMBField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string DELIV_ITEM {
            get {
                return this.dELIV_ITEMField;
            }
            set {
                this.dELIV_ITEMField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2556.0")]
    public delegate void GoodsMvtCreate_OUTCompletedEventHandler(object sender, GoodsMvtCreate_OUTCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2556.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GoodsMvtCreate_OUTCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GoodsMvtCreate_OUTCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public GoodsMvtCreate_res Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((GoodsMvtCreate_res)(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591