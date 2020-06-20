using Plugin.Settings;
using Plugin.Settings.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinPR.Helpers
{
    public static class Settings
    {
        private static ISettings AppSettings
        {
            get
            {
                return CrossSettings.Current;
            }
        }

        #region Setting Constants
        private const string BaseUrl = "BaseUrl";
        private static readonly string BaseUrlDefault = @"http://10.0.2.2/prwebapi/";
        #endregion
        public static string GeneralUrl
        {
            get
            {
                return AppSettings.GetValueOrDefault(BaseUrl, BaseUrlDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(BaseUrl, value);
            }
        }

        #region user Constants
        private const string UserName = "UserName";
        private static readonly string UserNameDefault = string.Empty;
        #endregion
        public static string CurrentUser
        {
            get
            {
                return AppSettings.GetValueOrDefault(UserName, UserNameDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(UserName, value);
            }
        }
        #region password Constants
        private const string Password = "Password";
        private static readonly string PasswordDefault = string.Empty;
        #endregion
        public static string CurrentUserPassword
        {
            get
            {
                return AppSettings.GetValueOrDefault(Password, PasswordDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(Password, value);
            }
        }

        #region bearer Constants
        private const string bearer = "bearer";
        private static readonly string bearerDefault = string.Empty;
        #endregion
        public static string Currentbearer
        {
            get
            {
                return AppSettings.GetValueOrDefault(bearer, bearerDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(bearer, value);
            }
        }
    }
}
