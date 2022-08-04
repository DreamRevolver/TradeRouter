using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebRouterApp.Features.Auth.Application.UseCases;
using WebRouterApp.Features.Users.Application.Models;

namespace WebRouterApp.Core.Data.EntityTypeConfigurations
{
    internal class UserRecordEntityTypeConfiguration: IEntityTypeConfiguration<UserRecord>
    {
        public void Configure(EntityTypeBuilder<UserRecord> builder)
        {
            builder.ToTable("Users");

            var saltBytes = Guid.Parse("67498c90-b220-409b-aacf-9936d6036a15").ToByteArray();

            byte[] passwordHashBytes;
            using (var passwordHasher = new PasswordHasher())
                passwordHashBytes = passwordHasher.Hash("1qaz", saltBytes);

            builder.HasData(
                new UserRecord(
                    username: "satoshi",
                    firstName: "Satoshi",
                    lastName: "Nakamoto",
                    passwordHash: passwordHashBytes,
                    salt: saltBytes)
                {
                    Id = Guid.Parse("38fec690-e4e6-4ec9-97db-b4259609b8a8"),
                }
            );
        }
    }
}
