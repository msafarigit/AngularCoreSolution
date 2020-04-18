using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Common.Models;
using static Common.Constants;

namespace Infrastucture.Session
{
    public class SessionManager : ISessionManager
    {
        private ISession session = null;

        public SessionManager(IHttpContextAccessor httpContextAccessor)
        {
            if (httpContextAccessor.HttpContext != null)
                session = httpContextAccessor.HttpContext.Session;
        }

        public void RemoveSessionValue(string sessionKey) => session.Remove(sessionKey);

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
            return session?.Get<object>(sessionKey);
        }

        public T GetSessionValue<T>(string sessionKey)
        {
            if (session == null)
                return default;
            return session.Get<T>(sessionKey);
        }

        public void SetWorkflowSystemSettings(List<Setting> workflowSystemSettings) => SetSessionValue(WORK_FLOW_SYSTEM_SETTING_KEY, workflowSystemSettings);

        public void SetSepadSystemSettings(List<Setting> sepadSystemSettings) => SetSessionValue(SEPAD_SYSTEM_SETTING_KEY, sepadSystemSettings);

        public string GetSepadSystemSettings(string settingKey) => GetSystemSettingValue(SEPAD_SYSTEM_SETTING_KEY, settingKey);

        public string GetWorkflowSystemSettingValue(string settingKey) => GetSystemSettingValue(WORK_FLOW_SYSTEM_SETTING_KEY, settingKey);

        public void SetNimaSystemSettings(List<Setting> nimaSystemSettings) => SetSessionValue(NIMA_SYSTEM_SETTING_KEY, nimaSystemSettings);

        public string GetNimaSystemSettingValue(string settingKey) => GetSystemSettingValue(NIMA_SYSTEM_SETTING_KEY, settingKey);

        public void SetPmSystemSettings(List<Setting> pmSystemSettings) => SetSessionValue(PM_SYSTEM_SETTING_KEY, pmSystemSettings);

        public string GetPmSystemSettingValue(string settingKey) => GetSystemSettingValue(PM_SYSTEM_SETTING_KEY, settingKey);

        private string GetSystemSettingValue(string sessionKey, string settingKey)
        {
            List<Setting> settings = GetSessionValue<List<Setting>>(sessionKey);
            if (settings == null || settings.Count == 0)
                return String.Empty;
            Setting setting = settings.FirstOrDefault(w => w.SettingName == settingKey.ToUpper());
            if (setting == null || setting.SettingValue == null)
                return String.Empty;

            return setting.SettingValue;
        }

        public void SetUserActions(List<UserAction> userActions) => SetSessionValue(USER_ACTION_KEY, userActions);

        public void ClearUserActions() => SetSessionValue(USER_ACTION_KEY, null);

        public bool IsUserActionsContains(string actionName)
        {
            List<UserAction> userActions = GetSessionValue<List<UserAction>>(USER_ACTION_KEY);
            if (userActions == null || userActions.Count == 0)
                return default;
            return userActions.Any(w => w.EName == actionName);
        }

        public int GetUserActionsCount()
        {
            List<UserAction> userActions = GetSessionValue<List<UserAction>>(USER_ACTION_KEY);
            if (userActions == null)
                return default;
            return userActions.Count;
        }

        public void ClearUserName() => SetSessionValue(USER_NAME_KEY, null);

        public void SetUserName(string userName) => SetSessionValue(USER_NAME_KEY, userName);

        public string GetUserName()
        {
            string userName = GetSessionValue<string>(USER_NAME_KEY);
            return userName;
        }

        public string GetTakrimSystemSettingValue(string settingKey) => GetSystemSettingValue(TAKRIM_SYSTEM_SETTING_KEY, settingKey);

        public void SetTakrimSystemSettings(List<Setting> takrimSystemSettings) => SetSessionValue(TAKRIM_SYSTEM_SETTING_KEY, takrimSystemSettings);
    }

    public static class SessionExtention
    {
        public static void Set<T>(this ISession session, string key, T val) => session.SetString(key, JsonSerializer.Serialize(val));

        public static T Get<T>(this ISession session, string key)
        {
            string value = session.GetString(key);
            if (String.IsNullOrEmpty(value))
                return default;
            return JsonSerializer.Deserialize<T>(value);
        }
    }
}
