using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Optimus.Services.Customers.Infrastructure.Migrations
{
    public partial class addcrmtoken : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CrmToken",
                schema: "customers",
                table: "Customers",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CrmToken",
                schema: "customers",
                table: "Customers");
        }
    }
}
