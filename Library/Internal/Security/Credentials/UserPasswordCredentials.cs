﻿using System;

namespace sones.Security
{
    /// <summary>
    /// Used for authentication
    /// </summary>
    public sealed class UserPasswordCredentials : IUserCredentials
    {
        #region data

        /// <summary>
        /// The hashed password
        /// </summary>
        public readonly int PasswordHash;

        /// <summary>
        /// The login string
        /// </summary>
        private readonly String _login;

        #endregion

        #region Constructor

        /// <summary>
        /// Create new credentials
        /// </summary>
        /// <param name="myLogin">The login string</param>
        /// <param name="myPassword">The password</param>
        public UserPasswordCredentials(String myLogin, String myPassword)
        {
            _login = myLogin;
            PasswordHash = myPassword.GetHashCode();
        }

        #endregion

        #region overrides

        public override string ToString()
        {
            return String.Format("Login: {0}, PW-Hash: {1}", _login, PasswordHash);
        }

        #endregion

        #region IUserCredentials Members

        public string Login
        {
            get { return _login; }
        }

        #endregion
    }
}