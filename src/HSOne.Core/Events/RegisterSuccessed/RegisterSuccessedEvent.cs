﻿using MediatR;

namespace HSOne.Core.Events.RegisterSuccessed
{
    public class RegisterSuccessedEvent : INotification
    {
        public string Email { get; set; }

        public RegisterSuccessedEvent(string email)
        {
            Email = email;
        }
    }
}
