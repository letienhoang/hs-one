﻿using MediatR;

namespace HSOne.Core.Events.LoginSuccessed
{
    public class LoginSuccessedEvent : INotification
    {
        public string Email { get; set; }

        public LoginSuccessedEvent(string email)
        {
            Email = email;
        }
    }
}
