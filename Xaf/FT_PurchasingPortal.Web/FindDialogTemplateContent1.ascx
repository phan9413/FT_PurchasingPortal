<%@ Control Language="C#" CodeBehind="FindDialogTemplateContent1.ascx.cs" ClassName="FindDialogTemplateContent1" Inherits="FT_PurchasingPortal.Web.FindDialogTemplateContent1" %>
<%@ Register Assembly="DevExpress.Web.v19.2, Version=19.2.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>
<%@ Register Assembly="DevExpress.ExpressApp.Web.v19.2, Version=19.2.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.ExpressApp.Web.Templates.ActionContainers"
    TagPrefix="xaf" %>
<%@ Register Assembly="DevExpress.ExpressApp.Web.v19.2, Version=19.2.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.ExpressApp.Web.Templates"
    TagPrefix="xaf" %>
<%@ Register Assembly="DevExpress.ExpressApp.Web.v19.2, Version=19.2.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.ExpressApp.Web.Controls"
    TagPrefix="xaf" %>
<%@ Register Assembly="DevExpress.ExpressApp.Web.v19.2, Version=19.2.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.ExpressApp.Web.Templates.Controls"
    TagPrefix="xaf" %>

<div class="searchDialogContent newStylePopupContent">
    <xaf:XafUpdatePanel ID="UPPopupWindowControl" runat="server">
        <xaf:XafPopupWindowControl runat="server" ID="PopupWindowControl" />
    </xaf:XafUpdatePanel>
    <table width="100%" height="100%" cellpadding="0" cellspacing="0" border="0">
        <tr>
            <td valign="top" height="100%">
                <table class="tableWrapperContent" border="0" cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td valign="top">
                            <div id="headerContent" class="headerContent">
                                <xaf:XafUpdatePanel ID="UPSAC" runat="server" CssClass="width100">
                                    <xaf:ActionContainerHolder runat="server" ID="SAC" ContainerStyle="Buttons" Orientation="Horizontal" CssClass="search">
                                        <menu width="100%"></menu>
                                        <actioncontainers>
                                            <xaf:WebActionContainer ContainerId="Search" />
                                            <xaf:WebActionContainer ContainerId="FullTextSearch" />
                                        </actioncontainers>
                                    </xaf:ActionContainerHolder>
                                </xaf:XafUpdatePanel>
                            </div>
                            <xaf:XafUpdatePanel ID="UPEI" runat="server" UpdatePanelForASPxGridListCallback="True">
                                <xaf:ErrorInfoControl ID="ErrorInfo" Style="margin: 10px 0px 10px 0px" runat="server" />
                            </xaf:XafUpdatePanel>
                            <xaf:XafUpdatePanel ID="UPVSC" runat="server">
                                <xaf:ViewSiteControl ID="VSC" runat="server" />
                            </xaf:XafUpdatePanel>
                        </td>
                    </tr>
                </table>
                <table class="tableWrapperActions" border="0" cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td>
                            <table style="width: 100%;">
                                <tr>
                                    <td>
                                        <div style="float: left;">
                                            <xaf:XafUpdatePanel ID="UPOC" runat="server">
                                                <xaf:ActionContainerHolder runat="server" ID="OC" CssClass="ACHOC" ContainerStyle="Buttons" Orientation="Horizontal">
                                                    <actioncontainers>
                                                        <xaf:WebActionContainer ContainerId="ObjectsCreation" />
                                                    </actioncontainers>
                                                </xaf:ActionContainerHolder>
                                            </xaf:XafUpdatePanel>
                                        </div>
                                    </td>
                                    <td>
                                        <div style="float: right;">
                                            <xaf:XafUpdatePanel ID="XafUpdatePanel2" runat="server">
                                                <xaf:ActionContainerHolder runat="server" ID="PopupActions" ContainerStyle="Buttons" Orientation="Horizontal">
                                                    <actioncontainers>
                                                        <xaf:WebActionContainer ContainerId="Diagnostic" />
                                                        <xaf:WebActionContainer ContainerId="PopupActions" />
                                                    </actioncontainers>
                                                </xaf:ActionContainerHolder>
                                            </xaf:XafUpdatePanel>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</div>
<script type="text/javascript">
    (function() {
        var mainWindow = xaf.Utils.GetMainWindow();
        mainWindow.pageLoaded = false;
        $(window).on("load", function() {
            var mainWindow = xaf.Utils.GetMainWindow();
            mainWindow.pageLoaded = true;
            PageLoaded();
            var activePopupControl = GetActivePopupControl();
            if(activePopupControl != null) {
                activePopupControl.SetHeaderText(document.title);
            }
        });
    })();
</script>

