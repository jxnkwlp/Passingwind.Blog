using Microsoft.EntityFrameworkCore.Migrations;

namespace Passingwind.Blog.EntityFramework.Sqlite.Data.Migrations
{
	public partial class Update_WidgetDynamicContent : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			//migrationBuilder.DropForeignKey(
			//    name: "FK_WidgetDynamicContentProperty_WidgetDynamicContents_WidgetDynamicContentId",
			//    table: "WidgetDynamicContentProperty");

			//migrationBuilder.DropPrimaryKey(
			//    name: "PK_WidgetDynamicContentProperty",
			//    table: "WidgetDynamicContentProperty");

			//migrationBuilder.DropIndex(
			//    name: "IX_WidgetDynamicContentProperty_WidgetDynamicContentId",
			//    table: "WidgetDynamicContentProperty");

			//migrationBuilder.DropColumn(
			//    name: "Id",
			//    table: "WidgetDynamicContentProperty");

			//migrationBuilder.AlterColumn<int>(
			//    name: "WidgetDynamicContentId",
			//    table: "WidgetDynamicContentProperty",
			//    nullable: false,
			//    oldClrType: typeof(int),
			//    oldType: "INTEGER",
			//    oldNullable: true);

			//migrationBuilder.AlterColumn<string>(
			//    name: "Name",
			//    table: "WidgetDynamicContentProperty",
			//    maxLength: 64,
			//    nullable: false,
			//    oldClrType: typeof(string),
			//    oldType: "TEXT",
			//    oldMaxLength: 64,
			//    oldNullable: true);

			//migrationBuilder.AddPrimaryKey(
			//    name: "PK_WidgetDynamicContentProperty",
			//    table: "WidgetDynamicContentProperty",
			//    columns: new[] { "WidgetDynamicContentId", "Name" });

			//migrationBuilder.AddForeignKey(
			//    name: "FK_WidgetDynamicContentProperty_WidgetDynamicContents_WidgetDynamicContentId",
			//    table: "WidgetDynamicContentProperty",
			//    column: "WidgetDynamicContentId",
			//    principalTable: "WidgetDynamicContents",
			//    principalColumn: "Id",
			//    onDelete: ReferentialAction.Cascade);




			migrationBuilder.Sql(
				@"DROP INDEX ""main"".""IX_WidgetDynamicContentProperty_WidgetDynamicContentId"";

ALTER TABLE ""main"".""WidgetDynamicContentProperty"" RENAME TO ""_WidgetDynamicContentProperty_old_20201016"";

CREATE TABLE ""main"".""WidgetDynamicContentProperty"" (
  ""Name"" TEXT NOT NULL,
  ""ValueType"" TEXT,
  ""IsArray"" INTEGER NOT NULL,
  ""Value"" TEXT,
  ""WidgetDynamicContentId"" INTEGER NOT NULL,
  CONSTRAINT ""PK_WidgetDynamicContentProperty"" PRIMARY KEY (""WidgetDynamicContentId"", ""Name""),
  CONSTRAINT ""FK_WidgetDynamicContentProperty_WidgetDynamicContents_WidgetDynamicContentId"" FOREIGN KEY (""WidgetDynamicContentId"") REFERENCES ""WidgetDynamicContents"" (""Id"") ON DELETE RESTRICT ON UPDATE NO ACTION
);

INSERT INTO ""main"".""WidgetDynamicContentProperty"" (""Name"", ""ValueType"", ""IsArray"", ""Value"", ""WidgetDynamicContentId"") SELECT ""Name"", ""ValueType"", ""IsArray"", ""Value"", ""WidgetDynamicContentId"" FROM ""main"".""_WidgetDynamicContentProperty_old_20201016"";

CREATE INDEX ""main"".""IX_WidgetDynamicContentProperty_WidgetDynamicContentId""
ON ""WidgetDynamicContentProperty"" (
  ""WidgetDynamicContentId"" ASC
);"

				, true);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			//migrationBuilder.DropForeignKey(
			//    name: "FK_WidgetDynamicContentProperty_WidgetDynamicContents_WidgetDynamicContentId",
			//    table: "WidgetDynamicContentProperty");

			//migrationBuilder.DropPrimaryKey(
			//    name: "PK_WidgetDynamicContentProperty",
			//    table: "WidgetDynamicContentProperty");

			//migrationBuilder.AlterColumn<string>(
			//    name: "Name",
			//    table: "WidgetDynamicContentProperty",
			//    type: "TEXT",
			//    maxLength: 64,
			//    nullable: true,
			//    oldClrType: typeof(string),
			//    oldMaxLength: 64);

			//migrationBuilder.AlterColumn<int>(
			//    name: "WidgetDynamicContentId",
			//    table: "WidgetDynamicContentProperty",
			//    type: "INTEGER",
			//    nullable: true,
			//    oldClrType: typeof(int));

			//migrationBuilder.AddColumn<long>(
			//    name: "Id",
			//    table: "WidgetDynamicContentProperty",
			//    type: "INTEGER",
			//    nullable: false,
			//    defaultValue: 0L)
			//    .Annotation("Sqlite:Autoincrement", true);

			//migrationBuilder.AddPrimaryKey(
			//    name: "PK_WidgetDynamicContentProperty",
			//    table: "WidgetDynamicContentProperty",
			//    column: "Id");

			//migrationBuilder.CreateIndex(
			//    name: "IX_WidgetDynamicContentProperty_WidgetDynamicContentId",
			//    table: "WidgetDynamicContentProperty",
			//    column: "WidgetDynamicContentId");

			//migrationBuilder.AddForeignKey(
			//    name: "FK_WidgetDynamicContentProperty_WidgetDynamicContents_WidgetDynamicContentId",
			//    table: "WidgetDynamicContentProperty",
			//    column: "WidgetDynamicContentId",
			//    principalTable: "WidgetDynamicContents",
			//    principalColumn: "Id",
			//    onDelete: ReferentialAction.Restrict);
		}
	}
}
