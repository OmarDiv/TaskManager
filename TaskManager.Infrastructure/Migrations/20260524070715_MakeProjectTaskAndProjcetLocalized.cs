using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeProjectTaskAndProjcetLocalized : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Projects");

            migrationBuilder.AlterColumn<long>(
                name: "ProjectId",
                table: "Tasks",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<long>(
                name: "DescriptionSetId",
                table: "Tasks",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TitleSetId",
                table: "Tasks",
                type: "bigint",
                nullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "CreatedById",
                table: "Projects",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<long>(
                name: "DescriptionSetId",
                table: "Projects",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "NameSetId",
                table: "Projects",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_DescriptionSetId",
                table: "Tasks",
                column: "DescriptionSetId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_TitleSetId",
                table: "Tasks",
                column: "TitleSetId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_DescriptionSetId",
                table: "Projects",
                column: "DescriptionSetId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_NameSetId",
                table: "Projects",
                column: "NameSetId");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_LocalizationSets_DescriptionSetId",
                table: "Projects",
                column: "DescriptionSetId",
                principalTable: "LocalizationSets",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_LocalizationSets_NameSetId",
                table: "Projects",
                column: "NameSetId",
                principalTable: "LocalizationSets",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_LocalizationSets_DescriptionSetId",
                table: "Tasks",
                column: "DescriptionSetId",
                principalTable: "LocalizationSets",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_LocalizationSets_TitleSetId",
                table: "Tasks",
                column: "TitleSetId",
                principalTable: "LocalizationSets",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_LocalizationSets_DescriptionSetId",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_LocalizationSets_NameSetId",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_LocalizationSets_DescriptionSetId",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_LocalizationSets_TitleSetId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_DescriptionSetId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_TitleSetId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Projects_DescriptionSetId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_NameSetId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "DescriptionSetId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "TitleSetId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "DescriptionSetId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "NameSetId",
                table: "Projects");

            migrationBuilder.AlterColumn<long>(
                name: "ProjectId",
                table: "Tasks",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<long>(
                name: "CreatedById",
                table: "Projects",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Projects",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Projects",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
