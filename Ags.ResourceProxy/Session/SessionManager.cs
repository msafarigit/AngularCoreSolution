using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Infrastucture.Session
{
    public class SessionManager : ISessionManager
    {
        private ISession session = null;

        private readonly string USER_ACTION_KEY = "8181A775-9FA2-4D21-8CA5-08E5089DB2AE";
        private readonly string USER_NAME_KEY = "27943796-A689-4C60-BC7E-ED8B310F7DA5";
        private readonly string PM_SYSTEM_SETTING_KEY = "AB84AA01-E62C-4F12-B4E5-D0C2EE80C453";
        private readonly string NIMA_SYSTEM_SETTING_KEY = "6655C156-F751-43C3-BD2F-8E127DECF9F0";
        private readonly string TAKRIM_SYSTEM_SETTING_KEY = "BF48EE97-A016-45C2-BA95-4461894F4927";
        private readonly string WORK_FLOW_SYSTEM_SETTING_KEY = "F66C8F59-76DF-4300-9A50-74358103B8CB";
        private readonly string SEPAD_SYSTEM_SETTING_KEY = "4B369443-4F90-4502-AACA-1B6318169133";

        public SessionManager(IHttpContextAccessor httpContextAccessor)
        {
            if (httpContextAccessor.HttpContext != null)
                session = httpContextAccessor.HttpContext.Session;
        }

        public void RemoveSessionValue(string sessionKey)
        {
            session.Remove(sessionKey);
        }
        public void SetSessionValue(string sessionKey, object obj)
        {
            session.Set<object>(sessionKey, obj);
        }
        public void SetSessionValue<T>(string sessionKey, T obj)
        {
            session.Set<T>(sessionKey, obj);
        }

        public object GetSessionValue(string sessionKey)
        {
            if (session == null)
                return null;

            return session.Get<object>(sessionKey);
        }
        public T GetSessionValue<T>(string sessionKey)
        {
            if (session == null)
                return default;

            return session.Get<T>(sessionKey);
        }

        public void SetWorkflowSystemSettings(List<SettingModel> workflowSystemSettings)
        {
            SetSessionValue(WORK_FLOW_SYSTEM_SETTING_KEY, workflowSystemSettings);
        }
        public void SetSepadSystemSettings(List<SettingModel> sepadSystemSettings)
        {
            SetSessionValue(SEPAD_SYSTEM_SETTING_KEY, sepadSystemSettings);
        }
        public string GetSepadSystemSettings(string settingKey)
        {
            return GetSystemSettingValue(SEPAD_SYSTEM_SETTING_KEY, settingKey);
        }

        public string GetWorkflowSystemSettingValue(string settingKey)
        {
            return GetSystemSettingValue(WORK_FLOW_SYSTEM_SETTING_KEY, settingKey);
        }

        public void SetNimaSystemSettings(List<SettingModel> nimaSystemSettings)
        {
            SetSessionValue(NIMA_SYSTEM_SETTING_KEY, nimaSystemSettings);
        }

        public string GetNimaSystemSettingValue(string settingKey)
        {
            return GetSystemSettingValue(NIMA_SYSTEM_SETTING_KEY, settingKey);
        }

        public void SetPmSystemSettings(List<SettingModel> pmSystemSettings)
        {
            SetSessionValue(PM_SYSTEM_SETTING_KEY, pmSystemSettings);
        }

        public string GetPmSystemSettingValue(string settingKey)
        {
            return GetSystemSettingValue(PM_SYSTEM_SETTING_KEY, settingKey);
        }

        private string GetSystemSettingValue(string sessionKey, string settingKey)
        {
            var settings = GetSessionValue<List<SettingModel>>(sessionKey);
            if (settings == null || settings.Count == 0)
                return string.Empty;

            var setting = settings.FirstOrDefault(w => w.SettingName == settingKey.ToUpper());
            if (setting == null || setting.SettingValue == null)
                return string.Empty;

            return setting.SettingValue;
        }


        public void SetUserActions(List<UserActionModel> userActions)
        {
            SetSessionValue(USER_ACTION_KEY, userActions);
        }

        public void ClearUserActions()
        {
            SetSessionValue(USER_ACTION_KEY, null);
        }

        public bool IsUserActionsContains(string actionName)
        {
            var userActions = GetSessionValue<List<UserActionModel>>(USER_ACTION_KEY);
            if (userActions == null || userActions.Count == 0)
                return false;

            return userActions.Any(w => w.EName == actionName);
        }

        public int GetUserActionsCount()
        {
            var userActions = GetSessionValue<List<UserActionModel>>(USER_ACTION_KEY);
            if (userActions == null)
                return 0;

            return userActions.Count;
        }

        public void ClearUserName()
        {
            SetSessionValue(USER_NAME_KEY, null);
        }

        public void SetUserName(string userName)
        {
            SetSessionValue(USER_NAME_KEY, userName);
        }

        public string GetUserName()
        {
            var userName = GetSessionValue<string>(USER_NAME_KEY);
            return userName;
        }

        public string GetTakrimSystemSettingValue(string settingKey)
        {
            return GetSystemSettingValue(TAKRIM_SYSTEM_SETTING_KEY, settingKey);
        }

        public void SetTakrimSystemSettings(List<SettingModel> takrimSystemSettings)
        {
            SetSessionValue(TAKRIM_SYSTEM_SETTING_KEY, takrimSystemSettings);
        }
    }

    public static class SessionExtention
    {
        public static void Set<T>(this ISession session, string key, T val)
        {
            session.SetString(key, JsonSerializer.Serialize(val));
        }

        public static T Get<T>(this ISession session, string key)
        {
            string value = session.GetString(key);
            if (string.IsNullOrEmpty(value))
                return default;

            return JsonSerializer.Deserialize<T>(value);
        }
    }
}
