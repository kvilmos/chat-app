using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace chat_backend.Migrations
{
    /// <inheritdoc />
    public partial class initdb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "user_t",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_t", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "group_t",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CreatorId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_group_t", x => x.Id);
                    table.ForeignKey(
                        name: "FK_group_t_user_t_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "user_t",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "group_user_j",
                columns: table => new
                {
                    GroupId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_group_user_j", x => new { x.GroupId, x.UserId });
                    table.ForeignKey(
                        name: "FK_group_user_j_group_t_GroupId",
                        column: x => x.GroupId,
                        principalTable: "group_t",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_group_user_j_user_t_UserId",
                        column: x => x.UserId,
                        principalTable: "user_t",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "message_t",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Text = table.Column<string>(type: "text", nullable: false),
                    Date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    SenderId = table.Column<int>(type: "integer", nullable: false),
                    GroupId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_message_t", x => x.Id);
                    table.ForeignKey(
                        name: "FK_message_t_group_t_GroupId",
                        column: x => x.GroupId,
                        principalTable: "group_t",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_message_t_user_t_SenderId",
                        column: x => x.SenderId,
                        principalTable: "user_t",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_group_t_CreatorId",
                table: "group_t",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_group_t_Id",
                table: "group_t",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_group_user_j_UserId",
                table: "group_user_j",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_message_t_GroupId",
                table: "message_t",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_message_t_SenderId",
                table: "message_t",
                column: "SenderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "group_user_j");

            migrationBuilder.DropTable(
                name: "message_t");

            migrationBuilder.DropTable(
                name: "group_t");

            migrationBuilder.DropTable(
                name: "user_t");
        }
    }
}
