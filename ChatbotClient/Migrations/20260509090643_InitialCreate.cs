using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatbotClient.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AiModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ModelName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AiModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SessionFolders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ParentFolderId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionFolders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemPrompts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Guid = table.Column<Guid>(type: "TEXT", nullable: false),
                    PromptText = table.Column<string>(type: "TEXT", nullable: true),
                    Hash = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemPrompts", x => x.Id);
                    table.UniqueConstraint("AK_SystemPrompts_Guid", x => x.Guid);
                });

            migrationBuilder.CreateTable(
                name: "TalkSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ParentFolderId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TalkSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TalkSessions_SessionFolders_ParentFolderId",
                        column: x => x.ParentFolderId,
                        principalTable: "SessionFolders",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TalkEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Guid = table.Column<Guid>(type: "TEXT", nullable: false),
                    Content = table.Column<string>(type: "TEXT", nullable: false),
                    Role = table.Column<string>(type: "TEXT", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TalkSessionGuid = table.Column<Guid>(type: "TEXT", nullable: false),
                    AiModelType = table.Column<int>(type: "INTEGER", nullable: false),
                    AiModelId = table.Column<Guid>(type: "TEXT", nullable: true),
                    SystemPromptGuid = table.Column<Guid>(type: "TEXT", nullable: true),
                    Index = table.Column<int>(type: "INTEGER", nullable: false),
                    IsFavorite = table.Column<bool>(type: "INTEGER", nullable: false),
                    GenerationId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TalkEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TalkEntries_SystemPrompts_SystemPromptGuid",
                        column: x => x.SystemPromptGuid,
                        principalTable: "SystemPrompts",
                        principalColumn: "Guid");
                    table.ForeignKey(
                        name: "FK_TalkEntries_TalkSessions_TalkSessionGuid",
                        column: x => x.TalkSessionGuid,
                        principalTable: "TalkSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TalkEntries_SystemPromptGuid",
                table: "TalkEntries",
                column: "SystemPromptGuid");

            migrationBuilder.CreateIndex(
                name: "IX_TalkEntries_TalkSessionGuid",
                table: "TalkEntries",
                column: "TalkSessionGuid");

            migrationBuilder.CreateIndex(
                name: "IX_TalkSessions_ParentFolderId",
                table: "TalkSessions",
                column: "ParentFolderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AiModels");

            migrationBuilder.DropTable(
                name: "TalkEntries");

            migrationBuilder.DropTable(
                name: "SystemPrompts");

            migrationBuilder.DropTable(
                name: "TalkSessions");

            migrationBuilder.DropTable(
                name: "SessionFolders");
        }
    }
}
