using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Assignment2.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Logins_CustomerID",
                table: "Logins");

            migrationBuilder.DropColumn(
                name: "Locked",
                table: "Customers");

            migrationBuilder.AddColumn<bool>(
                name: "Locked",
                table: "Logins",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Logins_CustomerID",
                table: "Logins",
                column: "CustomerID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Logins_CustomerID",
                table: "Logins");

            migrationBuilder.DropColumn(
                name: "Locked",
                table: "Logins");

            migrationBuilder.AddColumn<bool>(
                name: "Locked",
                table: "Customers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Logins_CustomerID",
                table: "Logins",
                column: "CustomerID",
                unique: true);
        }
    }
}
