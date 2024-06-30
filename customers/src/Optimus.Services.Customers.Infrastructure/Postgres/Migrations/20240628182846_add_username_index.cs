using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Optimus.Services.Customers.Infrastructure.Migrations
{
    public partial class add_username_index : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Customers_Username",
                schema: "customers",
                table: "Customers",
                column: "Username",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Customers_Username",
                schema: "customers",
                table: "Customers");
        }
    }
}
