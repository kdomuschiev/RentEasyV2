using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace renteasy_api.Migrations
{
    /// <inheritdoc />
    public partial class AddPropertyPaymentMethodsAndBillCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "bill_categories",
                table: "properties",
                type: "text",
                nullable: false,
                defaultValueSql: "'[]'");

            migrationBuilder.AddColumn<string>(
                name: "iban",
                table: "properties",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "iris_pay_phone_number",
                table: "properties",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "revolut_me_link",
                table: "properties",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "bill_categories",
                table: "properties");

            migrationBuilder.DropColumn(
                name: "iban",
                table: "properties");

            migrationBuilder.DropColumn(
                name: "iris_pay_phone_number",
                table: "properties");

            migrationBuilder.DropColumn(
                name: "revolut_me_link",
                table: "properties");
        }
    }
}
