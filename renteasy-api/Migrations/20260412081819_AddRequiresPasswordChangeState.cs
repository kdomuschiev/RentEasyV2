using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace renteasy_api.Migrations
{
    /// <inheritdoc />
    public partial class AddRequiresPasswordChangeState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "size_sqm",
                table: "properties",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "photo_blob_paths",
                table: "maintenance_requests",
                type: "character varying(8192)",
                maxLength: 8192,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "condition_reports",
                type: "text",
                nullable: false,
                defaultValue: "InProgress",
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "photo_blob_paths",
                table: "condition_report_items",
                type: "character varying(8192)",
                maxLength: 8192,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "account_state",
                table: "AspNetUsers",
                type: "text",
                nullable: false,
                defaultValue: "Active",
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateIndex(
                name: "ix_maintenance_requests_tenant_id",
                table: "maintenance_requests",
                column: "tenant_id");

            migrationBuilder.AddForeignKey(
                name: "fk_maintenance_requests_users_tenant_id",
                table: "maintenance_requests",
                column: "tenant_id",
                principalTable: "AspNetUsers",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_maintenance_requests_users_tenant_id",
                table: "maintenance_requests");

            migrationBuilder.DropIndex(
                name: "ix_maintenance_requests_tenant_id",
                table: "maintenance_requests");

            migrationBuilder.AlterColumn<decimal>(
                name: "size_sqm",
                table: "properties",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "photo_blob_paths",
                table: "maintenance_requests",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(8192)",
                oldMaxLength: 8192);

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "condition_reports",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldDefaultValue: "InProgress");

            migrationBuilder.AlterColumn<string>(
                name: "photo_blob_paths",
                table: "condition_report_items",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(8192)",
                oldMaxLength: 8192);

            migrationBuilder.AlterColumn<string>(
                name: "account_state",
                table: "AspNetUsers",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldDefaultValue: "Active");
        }
    }
}
