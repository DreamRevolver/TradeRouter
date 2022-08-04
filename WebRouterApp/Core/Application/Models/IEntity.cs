using System;

namespace WebRouterApp.Core.Application.Models
{
    public interface IEntity
    {
        Guid Id { get; set; }
    }
}