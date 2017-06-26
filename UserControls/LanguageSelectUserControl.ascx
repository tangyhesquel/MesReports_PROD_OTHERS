<%@ Control Language="C#" AutoEventWireup="true" CodeFile="LanguageSelectUserControl.ascx.cs" Inherits="UserControls_LanguageSelectUserControl" %>

<asp:RadioButton ID="RadioButton_ZH_CHS" runat="server" Text="中文简体"  
    AutoPostBack="true" GroupName="mlRadio" oncheckedchanged="RadioButton_ZH_CHS_CheckedChanged"
     />
<asp:RadioButton ID="RadioButton_EN_US" runat="server" Text="English"   
    AutoPostBack="true" GroupName="mlRadio" oncheckedchanged="RadioButton_EN_US_CheckedChanged"
    />

