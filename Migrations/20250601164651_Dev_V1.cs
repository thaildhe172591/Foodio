using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodioAPI.Migrations
{
    /// <inheritdoc />
    public partial class Dev_V1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deliveries_DeliveryStatuses_DeliveryStatusId",
                schema: "dbo",
                table: "Deliveries");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItemStatusHistory_OrderItemStatuses_OrderItemStatusId",
                schema: "dbo",
                table: "OrderItemStatusHistory");

            migrationBuilder.DropIndex(
                name: "IX_OrderItemStatusHistory_OrderItemStatusId",
                schema: "dbo",
                table: "OrderItemStatusHistory");

            migrationBuilder.DropIndex(
                name: "IX_Deliveries_DeliveryStatusId",
                schema: "dbo",
                table: "Deliveries");

            migrationBuilder.DropColumn(
                name: "OrderItemStatusId",
                schema: "dbo",
                table: "OrderItemStatusHistory");

            migrationBuilder.DropColumn(
                name: "DeliveryStatusId",
                schema: "dbo",
                table: "Deliveries");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OrderItemStatusId",
                schema: "dbo",
                table: "OrderItemStatusHistory",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeliveryStatusId",
                schema: "dbo",
                table: "Deliveries",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemStatusHistory_OrderItemStatusId",
                schema: "dbo",
                table: "OrderItemStatusHistory",
                column: "OrderItemStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_DeliveryStatusId",
                schema: "dbo",
                table: "Deliveries",
                column: "DeliveryStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Deliveries_DeliveryStatuses_DeliveryStatusId",
                schema: "dbo",
                table: "Deliveries",
                column: "DeliveryStatusId",
                principalSchema: "dbo",
                principalTable: "DeliveryStatuses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItemStatusHistory_OrderItemStatuses_OrderItemStatusId",
                schema: "dbo",
                table: "OrderItemStatusHistory",
                column: "OrderItemStatusId",
                principalSchema: "dbo",
                principalTable: "OrderItemStatuses",
                principalColumn: "Id");
        }
    }
}
