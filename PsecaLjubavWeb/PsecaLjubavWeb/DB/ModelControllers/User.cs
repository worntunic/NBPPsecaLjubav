﻿using Neo4jClient.Cypher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PsecaLjubavWeb.DB.Models
{
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }

        public User()
        {

        }
        public User(string username, string password, string email)
        {
            this.Username = username;
            this.Password = password;
            this.Email = email;
        }
    }

    public class UserResult
    {
        private User user;
        public enum UserResultStatus { FOUND, NOTFOUND }
        private UserResultStatus status;

        public UserResult(UserResultStatus status)
        {
            this.status = status;
        }
        public UserResult(User user)
        {
            this.status = UserResultStatus.FOUND;
            this.user = user;
        }

        public User GetUser()
        {
            if (status == UserResultStatus.NOTFOUND)
            {
                throw new Exception("User doesn't exist");
            }
            return user;
        }
        public UserResultStatus GetStatus()
        {
            return status;
        }
    }
    public class LoginResult
    {
        private UserResult userResult;
        public enum LoginStatus { LoginSuccess, InvalidUser, InvalidPassword }
        private LoginStatus status;
        
        public LoginResult(LoginStatus status)
        {
            this.status = status;
        }
        public LoginResult(UserResult user)
        {
            this.userResult = user;
            this.status = LoginStatus.LoginSuccess;
        }
        public User GetUser()
        {
            if (this.status != LoginStatus.LoginSuccess)
            {
                throw new Exception("Login unsuccessful, access not granted");
            }
            return userResult.GetUser();
        }
        public LoginStatus GetStatus()
        {
            return status;
        }
    }
    public class RegistrationResult
    {
        private UserResult userResult;
        public enum RegistrationStatus { RegistrationSuccess, UserAlreadyExists, UsernameTooShort, PasswordTooShort }
        private RegistrationStatus status;

        public RegistrationResult(RegistrationStatus status)
        {
            this.status = status;
        }
        public RegistrationResult(UserResult user)
        {
            this.userResult = user;
            this.status = RegistrationStatus.RegistrationSuccess;
        }
        public User GetUser()
        {
            if (this.status != RegistrationStatus.RegistrationSuccess)
            {
                throw new Exception("Registration unsuccessful, access not granted");
            }
            return userResult.GetUser();
        }
        public RegistrationStatus GetStatus()
        {
            return status;
        }
    }
    public class UserController : BaseController
    {
        private const int passwordMinLength = 4;
        private const int usernameMinLength = 4;

        public UserController(Database db) : base(db)
        {
        }

        public RegistrationResult RegisterUser(string username, string password, string email)
        {

            if (GetUser(username).GetStatus() == UserResult.UserResultStatus.FOUND)
            {
                return new RegistrationResult(RegistrationResult.RegistrationStatus.UserAlreadyExists);
            }
            if (username.Length < usernameMinLength)
            {
                return new RegistrationResult(RegistrationResult.RegistrationStatus.UsernameTooShort);
            }
            if (password.Length < passwordMinLength)
            {
                return new RegistrationResult(RegistrationResult.RegistrationStatus.PasswordTooShort);
            }
            string passHash = GetPasswordHash(password);
            User newUser = new User(username, passHash, email);

            graphClient.Cypher
                .Create("(user:User{newUser})")
                .WithParam("newUser", newUser)
                .ExecuteWithoutResults();
            /*if (results.Count() != 1)
            {
                throw new ControllerException($"Error during user registration");
            }*/
            return new RegistrationResult(new UserResult(newUser));
        }

        public LoginResult LoginUser(string username, string password)
        {
            UserResult userResult = GetUser(username);
            if (userResult.GetStatus() == UserResult.UserResultStatus.NOTFOUND)
            {
                return new LoginResult(LoginResult.LoginStatus.InvalidUser);
            }
            string passHash = GetPasswordHash(password);
            if (userResult.GetUser().Password != passHash)
            {
                return new LoginResult(LoginResult.LoginStatus.InvalidPassword);
            }
            return new LoginResult(userResult);
        }

        public UserResult GetUser(string username)
        {
            var results = graphClient.Cypher
                .Match("(user:User {Username: {wantedUsername}})")
                //.OptionalMatch("user {username: {wantedUsername}}")
                .WithParam("wantedUsername", username)
                .Return(user => user.As<User>()).Results;       

            if (results == null || results.Count() == 0)
            {
                return new UserResult(UserResult.UserResultStatus.NOTFOUND);
            }
            return new UserResult(results.First());
        }

        private static string GetPasswordHash(string password)
        {
            byte[] data = System.Text.Encoding.ASCII.GetBytes(password);
            data = new System.Security.Cryptography.SHA256Managed().ComputeHash(data);
            return System.Text.Encoding.ASCII.GetString(data);
        }
    }
}
