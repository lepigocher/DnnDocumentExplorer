using System;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;

using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;

namespace TidyModules.DocumentExplorer
{
    public partial class Settings : ModuleSettingsBase
    {
        #region Base Method Implementations

        /// <summary> 
        /// LoadSettings loads the settings from the Database and displays them 
        /// </summary> 
        public override void LoadSettings()
        {
            try
            {
                DocumentSettings settings = new DocumentSettings(ModuleConfiguration);

                FillThemes(settings.Theme);

                txtJqueryUICSS.Text = settings.JqueryUICSS;
                txtPrimeUIJS.Text = settings.PrimeUIJS;
                txtPrimeUICSS.Text = settings.PrimeUICSS;
                txtFontAwesomeCSS.Text = settings.FontAwesomeCSS;

                chkShowIcon.Checked = settings.ShowIcon;
                txtNameHeaderClass.Text = settings.NameHeaderClass;
                txtNameBodyClass.Text = settings.NameBodyClass;
                chkNameFilter.Checked = settings.NameFilter;
                ListItem nameMode = rblNameFilterMatchModes.Items.FindByValue(settings.NameFilterMatchMode);
                if (nameMode != null)
                    nameMode.Selected = true;
                chkNameSortable.Checked = settings.NameSortable;

                chkShowDate.Checked = settings.ShowDate;
                txtDateHeaderClass.Text = settings.DateHeaderClass;
                txtDateBodyClass.Text = settings.DateBodyClass;
                chkDateFilter.Checked = settings.DateFilter;
                ListItem dateMode = rblDateFilterMatchModes.Items.FindByValue(settings.DateFilterMatchMode);
                if (dateMode != null)
                    dateMode.Selected = true;
                chkDateSortable.Checked = settings.DateSortable;

                chkShowSize.Checked = settings.ShowSize;
                txtSizeHeaderClass.Text = settings.SizeHeaderClass;
                txtSizeBodyClass.Text = settings.SizeBodyClass;
                chkSizeFilter.Checked = settings.SizeFilter;
                ListItem sizeMode = rblSizeFilterMatchModes.Items.FindByValue(settings.SizeFilterMatchMode);
                if (sizeMode != null)
                    sizeMode.Selected = true;
                chkSizeSortable.Checked = settings.SizeSortable;

                txtRows.Text = settings.Rows.ToString();
                chkResetFilters.Checked = settings.ResetFilters;

                chkUserFolder.Checked = settings.UserFolder;
                chkFileManagement.Checked = settings.FileManagement;
                chkOpenOnDblclick.Checked = settings.OpenOnDblclick;
            }
            catch (Exception exc)
            {
                //Module failed to load 
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// <summary> 
        /// UpdateSettings saves the modified settings to the Database 
        /// </summary> 
        public override void UpdateSettings()
        {
            try
            {
                DocumentSettings settings = new DocumentSettings(ModuleConfiguration);

                settings.JqueryUICSS = txtJqueryUICSS.Text;
                settings.PrimeUIJS = txtPrimeUIJS.Text;
                settings.PrimeUICSS = txtPrimeUICSS.Text;
                settings.FontAwesomeCSS = txtFontAwesomeCSS.Text;
                settings.Theme = cboThemes.SelectedValue;

                settings.ShowIcon = chkShowIcon.Checked;
                settings.NameHeaderClass = txtNameHeaderClass.Text;
                settings.NameBodyClass = txtNameBodyClass.Text;
                settings.NameFilter = chkNameFilter.Checked;
                settings.NameFilterMatchMode = rblNameFilterMatchModes.SelectedValue;
                settings.NameSortable = chkNameSortable.Checked;

                settings.ShowDate = chkShowDate.Checked;
                settings.DateHeaderClass = txtDateHeaderClass.Text;
                settings.DateBodyClass = txtDateBodyClass.Text;
                settings.DateFilter = chkDateFilter.Checked;
                settings.DateFilterMatchMode = rblDateFilterMatchModes.SelectedValue;
                settings.DateSortable = chkDateSortable.Checked;

                settings.ShowSize = chkShowSize.Checked;
                settings.SizeHeaderClass = txtSizeHeaderClass.Text;
                settings.SizeBodyClass = txtSizeBodyClass.Text;
                settings.SizeFilter = chkSizeFilter.Checked;
                settings.SizeFilterMatchMode = rblSizeFilterMatchModes.SelectedValue;
                settings.SizeSortable = chkSizeSortable.Checked;

                settings.Rows = int.Parse(txtRows.Text);
                settings.ResetFilters = chkResetFilters.Checked;

                settings.UserFolder = chkUserFolder.Checked;
                settings.FileManagement = chkFileManagement.Checked;
                settings.OpenOnDblclick = chkOpenOnDblclick.Checked;

                settings.UpdateSettings();
            }
            catch (Exception exc)
            {
                //Module failed to load 
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        #endregion

        #region Private Methods

        private void FillThemes(string theme)
        {
            string basePath = MapPath(ControlPath);
            string folderPath = Path.Combine(basePath, @"Scripts\themes\");
            ListItem[] themes = Directory.GetDirectories(folderPath).Select(d => new ListItem(GetName(d))).ToArray();

            cboThemes.Items.Add(new ListItem(Localization.GetString("SelectTheme", LocalResourceFile), "(none)"));
            cboThemes.Items.AddRange(themes);

            ListItem item = cboThemes.Items.FindByText(theme);
            if (item != null)
                item.Selected = true;
        }

        private string GetName(string folderName)
        {
            string name = string.Empty;
            int pos = folderName.LastIndexOf(@"\");

            if (pos > -1)
                name = folderName.Substring(pos + 1);

            return name;
        }

        #endregion
    }
}