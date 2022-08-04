using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebRouterApp.Core.Data.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Publishers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ApiKey = table.Column<string>(type: "TEXT", nullable: false),
                    ApiSecret = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    TradeAllOrdersAsMarket = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Publishers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Subscribers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ApiKey = table.Column<string>(type: "TEXT", nullable: false),
                    ApiSecret = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    CoeffKind = table.Column<int>(type: "INTEGER", nullable: false),
                    Multiplier = table.Column<double>(type: "REAL", nullable: false),
                    TradeKind = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscribers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", nullable: false),
                    LastName = table.Column<string>(type: "TEXT", nullable: false),
                    PasswordHash = table.Column<byte[]>(type: "BLOB", nullable: false),
                    Salt = table.Column<byte[]>(type: "BLOB", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RefreshToken",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Token = table.Column<string>(type: "TEXT", nullable: false),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Expires = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Revoked = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ReplacedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UserRecordId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshToken", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshToken_Users_UserRecordId",
                        column: x => x.UserRecordId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Publishers",
                columns: new[] { "Id", "ApiKey", "ApiSecret", "Description", "Name", "TradeAllOrdersAsMarket" },
                values: new object[] { new Guid("f9787c27-a4ae-488b-905b-2d58bb02c4af"), "d270a941bc83bc337b475c755d9b99ce8374dd30f18d9c77d0fd758dff09d926", "66ce28d2575e329f9fdfed24b9d2620b90278d5faecf8b01625601f4bd7cb884", "", "7amuil", false });

            migrationBuilder.InsertData(
                table: "Subscribers",
                columns: new[] { "Id", "ApiKey", "ApiSecret", "CoeffKind", "Description", "Multiplier", "Name", "TradeKind" },
                values: new object[] { new Guid("3799db3f-16f0-4a4f-8bd1-7d8e054f9f8b"), "3329ec7ae6bdffea7de1dd19a67a0b574f7a553c8e870bf9e062521e552b5615", "eec08f26c98eb96268cc813afa159570c4441b490a3c72df85121b4baeab3a0b", 0, "Every Tom", 1.0, "Tom", 0 });

            migrationBuilder.InsertData(
                table: "Subscribers",
                columns: new[] { "Id", "ApiKey", "ApiSecret", "CoeffKind", "Description", "Multiplier", "Name", "TradeKind" },
                values: new object[] { new Guid("e6ba2381-d4f3-4dad-88a0-e470a0f60377"), "a552521f2823e6583418462ff8e9d094ac6dda958bea017863f88d82908ea691", "6a6bd42fb8f3d956f6642962be712cc015d1736eebcb48cd2e974a7833a430af", 0, "Every Dick", 1.0, "Dick", 0 });

            migrationBuilder.InsertData(
                table: "Subscribers",
                columns: new[] { "Id", "ApiKey", "ApiSecret", "CoeffKind", "Description", "Multiplier", "Name", "TradeKind" },
                values: new object[] { new Guid("2b389379-4aa3-481b-b54e-05156ba6be8c"), "9cbca5494d0e66652aa933dfd16585e46ebb488ee10ea528f3be7684739ad4df", "50611c69847096dca50e1eaef774f884e593f30ccd82f4249fcc743e9b479a75", 0, "Every user1", 1.0, "user1@zgmd.onmicrosoft.com", 0 });

            migrationBuilder.InsertData(
                table: "Subscribers",
                columns: new[] { "Id", "ApiKey", "ApiSecret", "CoeffKind", "Description", "Multiplier", "Name", "TradeKind" },
                values: new object[] { new Guid("a2ef6e28-343d-4be2-86c2-bc235693f512"), "186f0d31c9b33a1426a5b2c70d7703e87c3a3704f947f2c2815a9e9194601bfa", "a8b90468eaf611228f2174a5eadbd1f1d0b9a8cb0f03e3be36ba425acc2978e2", 0, "Every user2", 1.0, "user2@zgmd.onmicrosoft.com", 0 });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "FirstName", "LastName", "PasswordHash", "Salt", "Username" },
                values: new object[] { new Guid("38fec690-e4e6-4ec9-97db-b4259609b8a8"), "Satoshi", "Nakamoto", new byte[] { 124, 245, 125, 93, 118, 88, 51, 236, 133, 238, 63, 30, 84, 186, 136, 69, 37, 161, 221, 251, 16, 46, 92, 201, 240, 197, 47, 146, 111, 114, 146, 184, 180, 46, 241, 29, 78, 182, 169, 167, 103, 194, 243, 252, 184, 77, 34, 80, 27, 120, 36, 208, 10, 60, 0, 188, 114, 58, 145, 211, 58, 142, 120, 210 }, new byte[] { 144, 140, 73, 103, 32, 178, 155, 64, 170, 207, 153, 54, 214, 3, 106, 21 }, "satoshi" });

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_UserRecordId",
                table: "RefreshToken",
                column: "UserRecordId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Publishers");

            migrationBuilder.DropTable(
                name: "RefreshToken");

            migrationBuilder.DropTable(
                name: "Subscribers");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
