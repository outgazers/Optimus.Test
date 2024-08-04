using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Optimus.Services.Customers.Infrastructure.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "customers");

            migrationBuilder.CreateTable(
                name: "Customers",
                schema: "customers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Username = table.Column<string>(type: "text", nullable: false),
                    FullName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CompanyName = table.Column<string>(type: "text", nullable: false),
                    MC = table.Column<string>(type: "text", nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: false),
                    NetTerms = table.Column<string>(type: "text", nullable: false),
                    TMS = table.Column<string>(type: "text", nullable: false),
                    IsAssetBase = table.Column<bool>(type: "boolean", nullable: true),
                    ModsOfTransportation = table.Column<int[]>(type: "integer[]", nullable: true),
                    Industry = table.Column<string>(type: "text", nullable: false),
                    YearsInBusiness = table.Column<int>(type: "integer", nullable: true),
                    IsVip = table.Column<bool>(type: "boolean", nullable: false),
                    State = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Inbox",
                schema: "customers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inbox", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Outbox",
                schema: "customers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    OriginatedMessageId = table.Column<string>(type: "text", nullable: true),
                    CorrelationId = table.Column<string>(type: "text", nullable: true),
                    SpanContext = table.Column<string>(type: "text", nullable: true),
                    Headers = table.Column<string>(type: "json", nullable: true),
                    MessageType = table.Column<string>(type: "text", nullable: true),
                    MessageContextType = table.Column<string>(type: "text", nullable: true),
                    Message = table.Column<string>(type: "json", nullable: true),
                    MessageContext = table.Column<string>(type: "json", nullable: true),
                    SerializedMessage = table.Column<string>(type: "text", nullable: true),
                    SerializedMessageContext = table.Column<string>(type: "text", nullable: true),
                    SentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Outbox", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Email",
                schema: "customers",
                table: "Customers",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Username",
                schema: "customers",
                table: "Customers",
                column: "Username",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Customers",
                schema: "customers");

            migrationBuilder.DropTable(
                name: "Inbox",
                schema: "customers");

            migrationBuilder.DropTable(
                name: "Outbox",
                schema: "customers");
        }
    }
}
