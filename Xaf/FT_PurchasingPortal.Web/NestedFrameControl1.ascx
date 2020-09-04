<%@ Control Language="C#" CodeBehind="NestedFrameControl1.ascx.cs" ClassName="NestedFrameControl1" Inherits="FT_PurchasingPortal.Web.NestedFrameControl1" %>
<%@ Register Assembly="DevExpress.Web.v19.2, Version=19.2.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>
<%@ Register Assembly="DevExpress.ExpressApp.Web.v19.2, Version=19.2.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.ExpressApp.Web.Templates.ActionContainers"
    TagPrefix="xaf" %>
<%@ Register Assembly="DevExpress.ExpressApp.Web.v19.2, Version=19.2.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.ExpressApp.Web.Templates"
    TagPrefix="xaf" %>
<%@ Register Assembly="DevExpress.ExpressApp.Web.v19.2, Version=19.2.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.ExpressApp.Web.Controls"
    TagPrefix="xaf" %>
<%@ Register Assembly="DevExpress.ExpressApp.Web.v19.2, Version=19.2.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.ExpressApp.Web.Templates.Controls"
    TagPrefix="xaf" %>

<div class="NestedFrame" id="NestedFrame">
    <div class="nf_Menu">
        <table style="width: 100%">
            <tbody>
                <tr>
                    <td>
                        <div class="nf_leftMenu">
                            <xaf:XafUpdatePanel ID="XafUpdatePanel1" CssClass="ToolBarUpdatePanel" runat="server">
                                <xaf:ActionContainerHolder runat="server" ID="ObjectsCreation" ContainerStyle="Links" CssClass="nf_leftMenu_AC" Orientation="Horizontal"
                                    Menu-Width="100%" Menu-ItemAutoWidth="False">
                                    <actioncontainers>
                                        <xaf:WebActionContainer ContainerId="ObjectsCreation" />
                                        <xaf:WebActionContainer ContainerId="Link" />
                                    </actioncontainers>
                                </xaf:ActionContainerHolder>
                            </xaf:XafUpdatePanel>
                        </div>
                    </td>
                    <td>
                        <div class="nf_rightMenu" style="width: 100%">
                            <xaf:XafUpdatePanel ID="UPToolBar" CssClass="ToolBarUpdatePanel" runat="server">
                                <xaf:ActionContainerHolder runat="server" ID="ToolBar" ContainerStyle="Links" CssClass="nf_rightMenu_AC" Orientation="Horizontal"
                                    Menu-ItemAutoWidth="False">
                                    <menu width="100%" itemautowidth="False" clientinstancename="nestedFrameMenu" itemwrap="false">
                                        <SettingsAdaptivity Enabled="true" />
                                        <borderleft borderstyle="None" />
                                        <borderright borderstyle="None" />
                                    </menu>
                                    <actioncontainers>
                                        <xaf:WebActionContainer ContainerId="Edit" />
                                        <xaf:WebActionContainer ContainerId="RecordEdit" />
                                        <xaf:WebActionContainer ContainerId="View" />
                                        <xaf:WebActionContainer ContainerId="Reports" />
                                        <xaf:WebActionContainer ContainerId="Export" />
                                        <xaf:WebActionContainer ContainerId="Diagnostic" />
                                        <xaf:WebActionContainer ContainerId="Filters" />
                                    </actioncontainers>
                                </xaf:ActionContainerHolder>
                            </xaf:XafUpdatePanel>
                        </div>
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
    <b class="dx-clear"></b>
    <xaf:ViewSiteControl ID="viewSiteControl" runat="server" Control-CssClass="NestedFrameViewSite" />

</div>

