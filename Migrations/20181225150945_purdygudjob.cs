using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class purdygudjob : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "ProductType",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPiecePart",
                table: "ProductType",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSerialized",
                table: "ProductType",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ParentId",
                table: "ProductType",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ParentProductTypeId",
                table: "ProductType",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "GidSubLocationOption",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    CreatedById = table.Column<int>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Locked = table.Column<bool>(nullable: false),
                    GidLocationOptionId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GidSubLocationOption", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Scanner",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SerialNumber = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scanner", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ScannerLabelLogEntry",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<int>(nullable: true),
                    ScannerId = table.Column<int>(nullable: true),
                    ScannerStationId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScannerLabelLogEntry", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ScannerLabelType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    GidSubLocationOptionId = table.Column<int>(nullable: true),
                    ScannerType = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScannerLabelType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ScannerStation",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<int>(nullable: true),
                    GidSubLocationOptionId = table.Column<int>(nullable: true),
                    ScannerType = table.Column<string>(nullable: true),
                    ScannerLabelTypeId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScannerStation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ScannerStationLogEntry",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<int>(nullable: true),
                    ScannerId = table.Column<int>(nullable: true),
                    ScannerLabelId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScannerStationLogEntry", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ScannerEvent",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    ScannerEventLabelTypeId = table.Column<int>(nullable: true),
                    ScannerId = table.Column<int>(nullable: true),
                    ScannerStationId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScannerEvent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScannerEvent_ScannerLabelType_ScannerEventLabelTypeId",
                        column: x => x.ScannerEventLabelTypeId,
                        principalTable: "ScannerLabelType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ScannerLabel",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    BarcodeGuid = table.Column<Guid>(nullable: true),
                    ScannerLabelTypeId = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: true),
                    ScannerId = table.Column<int>(nullable: true),
                    ScannerLabelId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScannerLabel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScannerLabel_ScannerLabelType_ScannerLabelTypeId",
                        column: x => x.ScannerLabelTypeId,
                        principalTable: "ScannerLabelType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScannerLabelTypeVariable",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    ScannerLabelTypeVariableDataType = table.Column<string>(type: "nvarchar(24)", nullable: false),
                    ObjectName = table.Column<string>(nullable: true),
                    ObjectField = table.Column<string>(nullable: true),
                    ScannerLabelTypeId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScannerLabelTypeVariable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScannerLabelTypeVariable_ScannerLabelType_ScannerLabelTypeId",
                        column: x => x.ScannerLabelTypeId,
                        principalTable: "ScannerLabelType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ScannerActionRelatePieceParts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    ScannerEventId = table.Column<int>(nullable: true),
                    ParentPartScannerLabelTypeId = table.Column<int>(nullable: true),
                    ChildPartScannerLabelTypeId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScannerActionRelatePieceParts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScannerActionRelatePieceParts_ScannerLabelType_ChildPartScannerLabelTypeId",
                        column: x => x.ChildPartScannerLabelTypeId,
                        principalTable: "ScannerLabelType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ScannerActionRelatePieceParts_ScannerLabelType_ParentPartScannerLabelTypeId",
                        column: x => x.ParentPartScannerLabelTypeId,
                        principalTable: "ScannerLabelType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ScannerActionRelatePieceParts_ScannerEvent_ScannerEventId",
                        column: x => x.ScannerEventId,
                        principalTable: "ScannerEvent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ScannerActionUpdateLocation",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    ScannerEventId = table.Column<int>(nullable: true),
                    InventoryItemScannerLabelTypeId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScannerActionUpdateLocation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScannerActionUpdateLocation_ScannerLabelType_InventoryItemScannerLabelTypeId",
                        column: x => x.InventoryItemScannerLabelTypeId,
                        principalTable: "ScannerLabelType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ScannerActionUpdateLocation_ScannerEvent_ScannerEventId",
                        column: x => x.ScannerEventId,
                        principalTable: "ScannerEvent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ScannerActionUpdateSystemData",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    ScannerEventId = table.Column<int>(nullable: true),
                    ObjectName = table.Column<string>(nullable: true),
                    ObjectField = table.Column<string>(nullable: true),
                    ValueScannerLabelTypeId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScannerActionUpdateSystemData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScannerActionUpdateSystemData_ScannerEvent_ScannerEventId",
                        column: x => x.ScannerEventId,
                        principalTable: "ScannerEvent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ScannerActionUpdateSystemData_ScannerLabelType_ValueScannerLabelTypeId",
                        column: x => x.ValueScannerLabelTypeId,
                        principalTable: "ScannerLabelType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ScannerActionUpdateWorkLog",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    ScannerEventId = table.Column<int>(nullable: true),
                    UserScannerLabelTypeId = table.Column<int>(nullable: true),
                    InventoryItemScannerLabelTypeId = table.Column<int>(nullable: true),
                    EventName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScannerActionUpdateWorkLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScannerActionUpdateWorkLog_ScannerLabelType_InventoryItemScannerLabelTypeId",
                        column: x => x.InventoryItemScannerLabelTypeId,
                        principalTable: "ScannerLabelType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ScannerActionUpdateWorkLog_ScannerEvent_ScannerEventId",
                        column: x => x.ScannerEventId,
                        principalTable: "ScannerEvent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ScannerActionUpdateWorkLog_ScannerLabelType_UserScannerLabelTypeId",
                        column: x => x.UserScannerLabelTypeId,
                        principalTable: "ScannerLabelType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ScannerEventScannerLabelType",
                columns: table => new
                {
                    ScannerEventId = table.Column<int>(nullable: false),
                    ScannerLabelTypeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScannerEventScannerLabelType", x => new { x.ScannerEventId, x.ScannerLabelTypeId });
                    table.ForeignKey(
                        name: "FK_ScannerEventScannerLabelType_ScannerEvent_ScannerEventId",
                        column: x => x.ScannerEventId,
                        principalTable: "ScannerEvent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScannerEventScannerLabelType_ScannerLabelType_ScannerLabelTypeId",
                        column: x => x.ScannerLabelTypeId,
                        principalTable: "ScannerLabelType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductType_ParentId",
                table: "ProductType",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ScannerActionRelatePieceParts_ChildPartScannerLabelTypeId",
                table: "ScannerActionRelatePieceParts",
                column: "ChildPartScannerLabelTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ScannerActionRelatePieceParts_ParentPartScannerLabelTypeId",
                table: "ScannerActionRelatePieceParts",
                column: "ParentPartScannerLabelTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ScannerActionRelatePieceParts_ScannerEventId",
                table: "ScannerActionRelatePieceParts",
                column: "ScannerEventId",
                unique: true,
                filter: "[ScannerEventId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ScannerActionUpdateLocation_InventoryItemScannerLabelTypeId",
                table: "ScannerActionUpdateLocation",
                column: "InventoryItemScannerLabelTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ScannerActionUpdateLocation_ScannerEventId",
                table: "ScannerActionUpdateLocation",
                column: "ScannerEventId",
                unique: true,
                filter: "[ScannerEventId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ScannerActionUpdateSystemData_ScannerEventId",
                table: "ScannerActionUpdateSystemData",
                column: "ScannerEventId",
                unique: true,
                filter: "[ScannerEventId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ScannerActionUpdateSystemData_ValueScannerLabelTypeId",
                table: "ScannerActionUpdateSystemData",
                column: "ValueScannerLabelTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ScannerActionUpdateWorkLog_InventoryItemScannerLabelTypeId",
                table: "ScannerActionUpdateWorkLog",
                column: "InventoryItemScannerLabelTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ScannerActionUpdateWorkLog_ScannerEventId",
                table: "ScannerActionUpdateWorkLog",
                column: "ScannerEventId",
                unique: true,
                filter: "[ScannerEventId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ScannerActionUpdateWorkLog_UserScannerLabelTypeId",
                table: "ScannerActionUpdateWorkLog",
                column: "UserScannerLabelTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ScannerEvent_ScannerEventLabelTypeId",
                table: "ScannerEvent",
                column: "ScannerEventLabelTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ScannerEventScannerLabelType_ScannerLabelTypeId",
                table: "ScannerEventScannerLabelType",
                column: "ScannerLabelTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ScannerLabel_ScannerLabelTypeId",
                table: "ScannerLabel",
                column: "ScannerLabelTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ScannerLabelTypeVariable_ScannerLabelTypeId",
                table: "ScannerLabelTypeVariable",
                column: "ScannerLabelTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductType_ProductType_ParentId",
                table: "ProductType",
                column: "ParentId",
                principalTable: "ProductType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductType_ProductType_ParentId",
                table: "ProductType");

            migrationBuilder.DropTable(
                name: "GidSubLocationOption");

            migrationBuilder.DropTable(
                name: "Scanner");

            migrationBuilder.DropTable(
                name: "ScannerActionRelatePieceParts");

            migrationBuilder.DropTable(
                name: "ScannerActionUpdateLocation");

            migrationBuilder.DropTable(
                name: "ScannerActionUpdateSystemData");

            migrationBuilder.DropTable(
                name: "ScannerActionUpdateWorkLog");

            migrationBuilder.DropTable(
                name: "ScannerEventScannerLabelType");

            migrationBuilder.DropTable(
                name: "ScannerLabel");

            migrationBuilder.DropTable(
                name: "ScannerLabelLogEntry");

            migrationBuilder.DropTable(
                name: "ScannerLabelTypeVariable");

            migrationBuilder.DropTable(
                name: "ScannerStation");

            migrationBuilder.DropTable(
                name: "ScannerStationLogEntry");

            migrationBuilder.DropTable(
                name: "ScannerEvent");

            migrationBuilder.DropTable(
                name: "ScannerLabelType");

            migrationBuilder.DropIndex(
                name: "IX_ProductType_ParentId",
                table: "ProductType");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "ProductType");

            migrationBuilder.DropColumn(
                name: "IsPiecePart",
                table: "ProductType");

            migrationBuilder.DropColumn(
                name: "IsSerialized",
                table: "ProductType");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "ProductType");

            migrationBuilder.DropColumn(
                name: "ParentProductTypeId",
                table: "ProductType");
        }
    }
}
