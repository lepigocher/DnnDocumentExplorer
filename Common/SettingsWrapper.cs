using System;
using System.Collections;
using System.Linq;
using System.Reflection;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Controllers;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;

namespace TidyModules.DocumentExplorer.Common
{
    public enum SettingsType
    {
        Module,
        TabModule,
        Portal,
        Host
    }

    /// <summary>
    /// Abstract class used to manage module settings.
    /// </summary>
    public abstract class SettingsWrapper
    {
        #region Private Members

        private readonly int _moduleId;
        private readonly int _tabModuleId;
        private readonly int _portalId;

        #endregion

        #region Constructors

        protected SettingsWrapper(ModuleInfo module)
        {
            _moduleId = module.ModuleID;
            _tabModuleId = module.TabModuleID;
            _portalId = module.PortalID;

            LoadSettings(module);
        }

        #endregion

        #region Public Methods

        public void UpdateSettings()
        {
            ModuleController controller = new ModuleController();

            foreach (PropertyInfo propertyInfo in GetProperties())
            {
                if (propertyInfo.CanRead)
                {
                    string settingName = propertyInfo.Name;
                    object settingValue = propertyInfo.GetValue(this, null);
                    SettingsType settingType = SettingsType.TabModule;

                    ModuleSettingAttribute settingInfo = GetPropertyAttribute(propertyInfo);

                    if (settingInfo != null)
                    {
                        settingName = settingInfo.Name;
                        if (settingValue == null || string.IsNullOrEmpty(settingValue.ToString()))
                            settingValue = settingInfo.Default;
                        settingType = settingInfo.Scope;
                    }

                    switch (settingType)
                    {
                        case SettingsType.Module:
                            controller.UpdateModuleSetting(_moduleId, settingName, settingValue.ToString());
                            break;
                        case SettingsType.TabModule:
                            controller.UpdateTabModuleSetting(_tabModuleId, settingName, settingValue.ToString());
                            break;
                        case SettingsType.Portal:
                            PortalController.UpdatePortalSetting(_portalId, settingName, settingValue.ToString());
                            break;
                        case SettingsType.Host:
                            HostController.Instance.Update(settingName, settingValue.ToString());
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        #endregion

        #region Private Methods

        private PropertyInfo[] GetProperties()
        {
            string cacheKey = GetType().FullName;
            PropertyInfo[] propertyList = (PropertyInfo[])DataCache.GetCache(cacheKey);
            if (propertyList == null)
            {
                propertyList = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                DataCache.SetCache(cacheKey, propertyList);
            }
            return propertyList;
        }

        private ModuleSettingAttribute GetPropertyAttribute(PropertyInfo propertyInfo)
        {
            if (propertyInfo != null)
            {
                object[] attributes = propertyInfo.GetCustomAttributes(false);

                if (attributes.Length > 0)
                    return attributes.OfType<ModuleSettingAttribute>().FirstOrDefault();
            }

            return null;
        }

        private void LoadSettings(ModuleInfo module)
        {
            Hashtable settings = new Hashtable(module.ModuleSettings);
            foreach (string key in module.TabModuleSettings.Keys)
                settings[key] = module.TabModuleSettings[key];

            foreach (PropertyInfo propertyInfo in GetProperties())
            {
                if (propertyInfo.CanWrite)
                {
                    string settingName = propertyInfo.Name;
                    string settingDefault = string.Empty;
                    SettingsType settingScope = SettingsType.TabModule;
                    object setting = null;

                    ModuleSettingAttribute settingInfo = GetPropertyAttribute(propertyInfo);

                    if (settingInfo != null)
                    {
                        settingName = settingInfo.Name;
                        settingDefault = settingInfo.Default;
                        settingScope = settingInfo.Scope;
                    }

                    switch (settingScope)
                    {
                        case SettingsType.Module:
                        case SettingsType.TabModule:
                            setting = settings[settingName];

                            if (setting == null || string.IsNullOrEmpty(setting.ToString()))
                                setting = settingDefault;

                            break;
                        case SettingsType.Portal:
                            setting = PortalController.GetPortalSetting(settingName, _portalId, settingDefault);
                            break;
                        case SettingsType.Host:
                            setting = HostController.Instance.GetString(settingName, settingDefault);
                            break;
                        default:
                            break;
                    }

                    propertyInfo.SetValue(this, Convert.ChangeType(setting, propertyInfo.PropertyType), null);
                }
            }
        }

        #endregion
    }

    #region Custom Attribute

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ModuleSettingAttribute : Attribute
    {
        private readonly string _settingName;
        private readonly string _settingDefault;
        private readonly SettingsType _settingScope;

        public string Name
        {
            get { return _settingName; }
        }

        public string Default
        {
            get { return _settingDefault; }
        }

        public SettingsType Scope
        {
            get { return _settingScope; }
        }

        public ModuleSettingAttribute(string name, string defaultValue, SettingsType defaultScope = SettingsType.TabModule)
        {
            _settingName = name;
            _settingDefault = defaultValue;
            _settingScope = defaultScope;
        }
    }

    #endregion
}
