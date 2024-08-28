using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Optimus.Services.Identity.Infrastructure.Migrations
{
    public partial class addcrmaccountid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CrmAccountId",
                schema: "identities",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CrmAccountId",
                schema: "identities",
                table: "Users");
        }
    }
}
