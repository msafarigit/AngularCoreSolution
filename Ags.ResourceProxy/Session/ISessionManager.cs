using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastucture.Session
{
    public interface ISessionManager
    {
        void SetSessionValue<T>(string sessionKey, T obj);
        T GetSessionValue<T>(string sessionKey);
        void RemoveSessionValue(string sessionKey);
    }
}
