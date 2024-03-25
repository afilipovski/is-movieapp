using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieApp.Migrations
{
    /// <inheritdoc />
    public partial class tickets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MyProperty",
                table: "Orders");

            migrationBuilder.AddColumn<Guid>(
                name: "Orderid",
                table: "Tickets",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_Orderid",
                table: "Tickets",
                column: "Orderid");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Orders_Orderid",
                table: "Tickets",
                column: "Orderid",
                principalTable: "Orders",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Orders_Orderid",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_Orderid",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "Orderid",
                table: "Tickets");

            migrationBuilder.AddColumn<int>(
                name: "MyProperty",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
