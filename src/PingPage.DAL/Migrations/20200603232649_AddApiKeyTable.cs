using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PingPage.DAL.Migrations
{
    public partial class AddApiKeyTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ping_Users_UserID",
                table: "Ping");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Ping",
                table: "Ping");

            // Messed something up in PingPageDbContext.cs, that's why this happens
            migrationBuilder.RenameTable(
                name: "Ping",
                newName: "Pings");

            migrationBuilder.RenameIndex(
                name: "IX_Ping_UserID",
                table: "Pings",
                newName: "IX_Pings_UserID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Pings",
                table: "Pings",
                column: "ID");

            migrationBuilder.CreateTable(
                name: "ApiKeys",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    KeyHash = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    UserID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiKeys", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ApiKeys_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeys_UserID",
                table: "ApiKeys",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Pings_Users_UserID",
                table: "Pings",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pings_Users_UserID",
                table: "Pings");

            migrationBuilder.DropTable(
                name: "ApiKeys");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Pings",
                table: "Pings");

            migrationBuilder.RenameTable(
                name: "Pings",
                newName: "Ping");

            migrationBuilder.RenameIndex(
                name: "IX_Pings_UserID",
                table: "Ping",
                newName: "IX_Ping_UserID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Ping",
                table: "Ping",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Ping_Users_UserID",
                table: "Ping",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
