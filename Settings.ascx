<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Settings.ascx.cs" Inherits="TidyModules.DocumentExplorer.Settings" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<div class="dnnForm dnnClear">
    <h2 id="dnnPanel-CDN" class="dnnFormSectionHead"><a href=""><%= LocalizeString("CDN.Section") %></a></h2>
    <fieldset>
        <div class="dnnFormItem">
            <dnn:Label ID="lblJqueryUICSS" runat="server" ControlName="txtJqueryUICSS"></dnn:Label>
            <asp:TextBox ID="txtJqueryUICSS" runat="server"></asp:TextBox>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblPrimeUIJS" runat="server" ControlName="txtPrimeUIJS"></dnn:Label>
            <asp:TextBox ID="txtPrimeUIJS" runat="server"></asp:TextBox>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblPrimeUICSS" runat="server" ControlName="txtPrimeUICSS"></dnn:Label>
            <asp:TextBox ID="txtPrimeUICSS" runat="server"></asp:TextBox>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblFontAwesomeCSS" runat="server" ControlName="txtFontAwesomeCSS"></dnn:Label>
            <asp:TextBox ID="txtFontAwesomeCSS" runat="server"></asp:TextBox>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblThemes" runat="server" controlname="cboThemes"></dnn:Label>
            <asp:DropDownList ID="cboThemes" runat="server">
            </asp:DropDownList>
        </div>
    </fieldset>
    <h2 id="dnnPanel-Columns" class="dnnFormSectionHead"><a href=""><%= LocalizeString("FileGrid.Section") %></a></h2>
    <fieldset>
        <h6><%= LocalizeString("NameColumn") %></h6>
         <div class="dnnFormItem">
            <dnn:Label ID="lblShowIcon" runat="server" ControlName="chkShowIcon"></dnn:Label>
            <asp:CheckBox ID="chkShowIcon" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblNameHeaderClass" runat="server" controlname="txtNameHeaderClass"></dnn:Label>
            <asp:TextBox ID="txtNameHeaderClass" runat="server"></asp:TextBox>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblNameBodyClass" runat="server" controlname="txtNameBodyClass"></dnn:Label>
            <asp:TextBox ID="txtNameBodyClass" runat="server"></asp:TextBox>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblNameFilter" runat="server" ControlName="chkNameFilter"></dnn:Label>
            <asp:CheckBox ID="chkNameFilter" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblNameFilterMatchMode" runat="server" ControlName="rblNameFilterMatchModes"></dnn:Label>
            <asp:RadioButtonList ID="rblNameFilterMatchModes" runat="server" CssClass="dnnFormRadioButtons" RepeatDirection="Horizontal">
                <asp:ListItem Text="Start With" Value="startWith" ResourceKey="FilterMatchModeStartWith"></asp:ListItem>
                <asp:ListItem Text="Contains" Value="contains" ResourceKey="FilterMatchModeContains"></asp:ListItem>
            </asp:RadioButtonList>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblNameSortable" runat="server" ControlName="chkNameSortable"></dnn:Label>
            <asp:CheckBox ID="chkNameSortable" runat="server" />
        </div>
        <h6><%= LocalizeString("DateColumn") %></h6>
        <div class="dnnFormItem">
            <dnn:Label ID="lblShowDate" runat="server" ControlName="chkShowDate"></dnn:Label>
            <asp:CheckBox ID="chkShowDate" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblDateHeaderClass" runat="server" controlname="txtDateHeaderClass"></dnn:Label>
            <asp:TextBox ID="txtDateHeaderClass" runat="server"></asp:TextBox>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblDateBodyClass" runat="server" controlname="txtDateBodyClass"></dnn:Label>
            <asp:TextBox ID="txtDateBodyClass" runat="server"></asp:TextBox>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblDateFilter" runat="server" ControlName="chkDateFilter"></dnn:Label>
            <asp:CheckBox ID="chkDateFilter" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblDateFilterMatchMode" runat="server" ControlName="rblDateFilterMatchModes"></dnn:Label>
            <asp:RadioButtonList ID="rblDateFilterMatchModes" runat="server" CssClass="dnnFormRadioButtons" RepeatDirection="Horizontal">
                <asp:ListItem Text="Start With" Value="startWith" ResourceKey="FilterMatchModeStartWith"></asp:ListItem>
                <asp:ListItem Text="Contains" Value="contains" ResourceKey="FilterMatchModeContains"></asp:ListItem>
            </asp:RadioButtonList>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblDateSortable" runat="server" ControlName="chkDateSortable"></dnn:Label>
            <asp:CheckBox ID="chkDateSortable" runat="server" />
        </div>
        <h6><%= LocalizeString("SizeColumn") %></h6>
        <div class="dnnFormItem">
            <dnn:Label ID="lblShowSize" runat="server" ControlName="chkShowSize"></dnn:Label>
            <asp:CheckBox ID="chkShowSize" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblSizeHeaderClass" runat="server" controlname="txtSizeHeaderClass"></dnn:Label>
            <asp:TextBox ID="txtSizeHeaderClass" runat="server"></asp:TextBox>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblSizeBodyClass" runat="server" controlname="txtSizeBodyClass"></dnn:Label>
            <asp:TextBox ID="txtSizeBodyClass" runat="server"></asp:TextBox>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblSizeFilter" runat="server" ControlName="chkSizeFilter"></dnn:Label>
            <asp:CheckBox ID="chkSizeFilter" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblSizeFilterMatchMode" runat="server" ControlName="rblSizeFilterMatchModes"></dnn:Label>
            <asp:RadioButtonList ID="rblSizeFilterMatchModes" runat="server" CssClass="dnnFormRadioButtons" RepeatDirection="Horizontal">
                <asp:ListItem Text="Start With" Value="startWith" ResourceKey="FilterMatchModeStartWith"></asp:ListItem>
                <asp:ListItem Text="Contains" Value="contains" ResourceKey="FilterMatchModeContains"></asp:ListItem>
            </asp:RadioButtonList>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblSizeSortable" runat="server" ControlName="chkSizeSortable"></dnn:Label>
            <asp:CheckBox ID="chkSizeSortable" runat="server" />
        </div>
        <h6><%= LocalizeString("Miscellaneous") %></h6>
        <div class="dnnFormItem">
            <dnn:Label ID="lblRows" runat="server" controlname="txtRows"></dnn:Label>
            <asp:TextBox ID="txtRows" runat="server" Width="50"></asp:TextBox>
        </div>
         <div class="dnnFormItem">
            <dnn:Label ID="lblResetFilters" runat="server" ControlName="chkResetFilters"></dnn:Label>
            <asp:CheckBox ID="chkResetFilters" runat="server" />
        </div>
    </fieldset>
    <h2 id="dnnPanel-Features" class="dnnFormSectionHead"><a href=""><%= LocalizeString("Features.Section") %></a></h2>
    <fieldset>
         <div class="dnnFormItem">
            <dnn:Label ID="lblUserFolder" runat="server" ControlName="chkUserFolder"></dnn:Label>
            <asp:CheckBox ID="chkUserFolder" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblFileManagement" runat="server" ControlName="chkFileManagement"></dnn:Label>
            <asp:CheckBox ID="chkFileManagement" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblOpenOnDblclick" runat="server" ControlName="chkOpenOnDblclick"></dnn:Label>
            <asp:CheckBox ID="chkOpenOnDblclick" runat="server" />
        </div>
    </fieldset>
</div>

