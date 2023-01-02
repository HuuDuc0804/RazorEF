using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CS58___Entity_Framework.Migrations
{
    public partial class SeedUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            for (int i = 0; i < 150; i++)
            {
                migrationBuilder.InsertData(
                    table: "Users",
                    columns: new[] {
                        "Id",
                        "UserName",
                        "Email",
                        "EmailConfirmed",
                        "PhoneNumberConfirmed",
                        "TwoFactorEnabled",
                        "LockoutEnabled",
                        "AccessFailedCount",
                        "HomeAddress"
                    },
                    values: new object[]{
                        Guid.NewGuid().ToString(),
                        "Account-" + i.ToString("D3"),
                        "email-" +i.ToString("D3") + "@example.com",
                        true,
                        true,
                        false,
                        false,
                        0,
                        "Address"
                    }
                );
            }
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}