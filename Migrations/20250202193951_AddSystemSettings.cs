using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace erpv0._1.Migrations
{
    /// <inheritdoc />
    public partial class AddSystemSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "production");

            migrationBuilder.EnsureSchema(
                name: "sales");

            migrationBuilder.EnsureSchema(
                name: "Purchases");

            migrationBuilder.EnsureSchema(
                name: "inventory");

            //migrationBuilder.CreateTable(
            //    name: "brands",
            //    schema: "production",
            //    columns: table => new
            //    {
            //        brand_id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        brand_name = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK__brands__5E5A8E27DF49EE5C", x => x.brand_id);
            //    });

            




            migrationBuilder.CreateTable(
                name: "SystemSettings",
                columns: table => new
                {
                    SettingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SystemName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LogoPath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DefaultLanguage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TimeZone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateFormat = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EnableNotifications = table.Column<bool>(type: "bit", nullable: false),
                    LowStockThreshold = table.Column<int>(type: "int", nullable: false),
                    EnableAuditLog = table.Column<bool>(type: "bit", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrencyFormat = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SystemEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DefaultTaxRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemSettings", x => x.SettingId);
                });

            //migrationBuilder.CreateTable(
            //    name: "Warehouse",
            //    schema: "inventory",
            //    columns: table => new
            //    {
            //        WarehouseId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
            //        Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
            //        Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
            //        CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
            //        UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: true),
            //        CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
            //        UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK__Warehous__2608AFF91A845B36", x => x.WarehouseId);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "products",
            //    schema: "production",
            //    columns: table => new
            //    {
            //        product_id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        product_name = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
            //        brand_id = table.Column<int>(type: "int", nullable: false),
            //        model_year = table.Column<short>(type: "smallint", nullable: false),
            //        list_price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
            //        category_id = table.Column<int>(type: "int", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK__products__47027DF53DF9F473", x => x.product_id);
            //        table.ForeignKey(
            //            name: "FK__products__brand___787EE5A0",
            //            column: x => x.brand_id,
            //            principalSchema: "production",
            //            principalTable: "brands",
            //            principalColumn: "brand_id",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_products_ProductCategory",
            //            column: x => x.category_id,
            //            principalSchema: "production",
            //            principalTable: "ProductCategory",
            //            principalColumn: "CategoryId");
            //    });

            //migrationBuilder.CreateTable(
            //    name: "staffs",
            //    schema: "sales",
            //    columns: table => new
            //    {
            //        staff_id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        first_name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
            //        last_name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
            //        email = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
            //        phone = table.Column<string>(type: "varchar(25)", unicode: false, maxLength: 25, nullable: true),
            //        active = table.Column<byte>(type: "tinyint", nullable: false),
            //        store_id = table.Column<int>(type: "int", nullable: false),
            //        manager_id = table.Column<int>(type: "int", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK__staffs__1963DD9CF3FDC3B6", x => x.staff_id);
            //        table.ForeignKey(
            //            name: "FK__staffs__manager___06CD04F7",
            //            column: x => x.manager_id,
            //            principalSchema: "sales",
            //            principalTable: "staffs",
            //            principalColumn: "staff_id");
            //        table.ForeignKey(
            //            name: "FK__staffs__store_id__07C12930",
            //            column: x => x.store_id,
            //            principalSchema: "sales",
            //            principalTable: "stores",
            //            principalColumn: "store_id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "PriceHistory",
            //    schema: "inventory",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Product_Id = table.Column<int>(type: "int", nullable: false),
            //        CostPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
            //        SellingPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
            //        EffectiveDate = table.Column<DateTime>(type: "datetime", nullable: false),
            //        Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
            //        UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: true),
            //        CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
            //        UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK__PriceHis__3214EC07A283209B", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_PriceHistory_Product",
            //            column: x => x.Product_Id,
            //            principalSchema: "production",
            //            principalTable: "products",
            //            principalColumn: "product_id");
            //    });

            //migrationBuilder.CreateTable(
            //    name: "ProductTranslation",
            //    schema: "inventory",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        ProductId = table.Column<int>(type: "int", nullable: false),
            //        Language = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
            //        Name = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: false),
            //        Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Specifications = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
            //        UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: true),
            //        CreatedBy = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
            //        UpdatedBy = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK__ProductT__3214EC0700D40C59", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_ProductTranslation_Product",
            //            column: x => x.ProductId,
            //            principalSchema: "production",
            //            principalTable: "products",
            //            principalColumn: "product_id");
            //    });

            //migrationBuilder.CreateTable(
            //    name: "ProductVariant",
            //    schema: "inventory",
            //    columns: table => new
            //    {
            //        VariantId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        ProductId = table.Column<int>(type: "int", nullable: false),
            //        Size = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
            //        Color = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
            //        Style = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
            //        PriceAdjustment = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
            //        IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
            //        CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
            //        UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: true),
            //        CreatedBy = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
            //        UpdatedBy = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK__ProductV__0EA2338446890398", x => x.VariantId);
            //        table.ForeignKey(
            //            name: "FK_ProductVariant_Product",
            //            column: x => x.ProductId,
            //            principalSchema: "production",
            //            principalTable: "products",
            //            principalColumn: "product_id");
            //    });

            //migrationBuilder.CreateTable(
            //    name: "stocks",
            //    schema: "production",
            //    columns: table => new
            //    {
            //        store_id = table.Column<int>(type: "int", nullable: false),
            //        product_id = table.Column<int>(type: "int", nullable: false),
            //        quantity = table.Column<int>(type: "int", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK__stocks__E68284D33A63CFB0", x => new { x.store_id, x.product_id });
            //        table.ForeignKey(
            //            name: "FK__stocks__product___7A672E12",
            //            column: x => x.product_id,
            //            principalSchema: "production",
            //            principalTable: "products",
            //            principalColumn: "product_id",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK__stocks__store_id__7B5B524B",
            //            column: x => x.store_id,
            //            principalSchema: "sales",
            //            principalTable: "stores",
            //            principalColumn: "store_id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "WarehouseTransfer",
            //    schema: "inventory",
            //    columns: table => new
            //    {
            //        TransferId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        SourceWarehouseId = table.Column<int>(type: "int", nullable: false),
            //        DestWarehouseId = table.Column<int>(type: "int", nullable: false),
            //        ProductId = table.Column<int>(type: "int", nullable: false),
            //        Quantity = table.Column<int>(type: "int", nullable: false),
            //        Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
            //        RequestedDate = table.Column<DateTime>(type: "datetime", nullable: false),
            //        CompletedDate = table.Column<DateTime>(type: "datetime", nullable: true),
            //        ApprovedBy = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
            //        CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
            //        UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: true),
            //        CreatedBy = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
            //        UpdatedBy = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK__Warehous__95490091D8707CFF", x => x.TransferId);
            //        table.ForeignKey(
            //            name: "FK_WarehouseTransfer_DestWarehouse",
            //            column: x => x.DestWarehouseId,
            //            principalSchema: "inventory",
            //            principalTable: "Warehouse",
            //            principalColumn: "WarehouseId");
            //        table.ForeignKey(
            //            name: "FK_WarehouseTransfer_Product",
            //            column: x => x.ProductId,
            //            principalSchema: "production",
            //            principalTable: "products",
            //            principalColumn: "product_id");
            //        table.ForeignKey(
            //            name: "FK_WarehouseTransfer_SourceWarehouse",
            //            column: x => x.SourceWarehouseId,
            //            principalSchema: "inventory",
            //            principalTable: "Warehouse",
            //            principalColumn: "WarehouseId");
            //    });

            //migrationBuilder.CreateTable(
            //    name: "orders",
            //    schema: "sales",
            //    columns: table => new
            //    {
            //        order_id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        customer_id = table.Column<int>(type: "int", nullable: true),
            //        order_status = table.Column<byte>(type: "tinyint", nullable: false),
            //        order_date = table.Column<DateOnly>(type: "date", nullable: false),
            //        required_date = table.Column<DateOnly>(type: "date", nullable: false),
            //        shipped_date = table.Column<DateOnly>(type: "date", nullable: true),
            //        store_id = table.Column<int>(type: "int", nullable: false),
            //        staff_id = table.Column<int>(type: "int", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK__orders__465962295D0344EE", x => x.order_id);
            //        table.ForeignKey(
            //            name: "FK__orders__customer__03F0984C",
            //            column: x => x.customer_id,
            //            principalSchema: "sales",
            //            principalTable: "customers",
            //            principalColumn: "customer_id",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK__orders__staff_id__04E4BC85",
            //            column: x => x.staff_id,
            //            principalSchema: "sales",
            //            principalTable: "staffs",
            //            principalColumn: "staff_id");
            //        table.ForeignKey(
            //            name: "FK__orders__store_id__05D8E0BE",
            //            column: x => x.store_id,
            //            principalSchema: "sales",
            //            principalTable: "stores",
            //            principalColumn: "store_id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "sub_Orders",
            //    schema: "Purchases",
            //    columns: table => new
            //    {
            //        order_id = table.Column<int>(type: "int", nullable: false),
            //        placed_date = table.Column<DateOnly>(type: "date", nullable: true),
            //        fulfilled_date = table.Column<DateOnly>(type: "date", nullable: true),
            //        status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
            //        total_amount = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
            //        staff_id = table.Column<int>(type: "int", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK__sub_Orde__4659622948A1A9C0", x => x.order_id);
            //        table.ForeignKey(
            //            name: "FK__sub_Order__staff__01142BA1",
            //            column: x => x.staff_id,
            //            principalSchema: "sales",
            //            principalTable: "staffs",
            //            principalColumn: "staff_id");
            //    });

            //migrationBuilder.CreateTable(
            //    name: "StockEntry",
            //    schema: "inventory",
            //    columns: table => new
            //    {
            //        EntryId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Product_Id = table.Column<int>(type: "int", nullable: false),
            //        WarehouseId = table.Column<int>(type: "int", nullable: false),
            //        BatchNumber = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
            //        Quantity = table.Column<int>(type: "int", nullable: false),
            //        CostPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
            //        SellingPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
            //        MaxDiscount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
            //        ReceiptDate = table.Column<DateTime>(type: "datetime", nullable: false),
            //        SupplierInvoice = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
            //        ProductVariantVariantId = table.Column<int>(type: "int", nullable: true),
            //        CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
            //        UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: true),
            //        CreatedBy = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
            //        UpdatedBy = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK__StockEnt__F57BD2F741B063C1", x => x.EntryId);
            //        table.ForeignKey(
            //            name: "FK_StockEntry_Product",
            //            column: x => x.Product_Id,
            //            principalSchema: "production",
            //            principalTable: "products",
            //            principalColumn: "product_id");
            //        table.ForeignKey(
            //            name: "FK_StockEntry_ProductVariant",
            //            column: x => x.ProductVariantVariantId,
            //            principalSchema: "inventory",
            //            principalTable: "ProductVariant",
            //            principalColumn: "VariantId");
            //        table.ForeignKey(
            //            name: "FK_StockEntry_Warehouse",
            //            column: x => x.WarehouseId,
            //            principalSchema: "inventory",
            //            principalTable: "Warehouse",
            //            principalColumn: "WarehouseId");
            //    });

            //migrationBuilder.CreateTable(
            //    name: "order_items",
            //    schema: "sales",
            //    columns: table => new
            //    {
            //        order_id = table.Column<int>(type: "int", nullable: false),
            //        item_id = table.Column<int>(type: "int", nullable: false),
            //        product_id = table.Column<int>(type: "int", nullable: false),
            //        quantity = table.Column<int>(type: "int", nullable: false),
            //        list_price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
            //        discount = table.Column<decimal>(type: "decimal(4,2)", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK__order_it__837942D4D41A47A3", x => new { x.order_id, x.item_id });
            //        table.ForeignKey(
            //            name: "FK__order_ite__order__02084FDA",
            //            column: x => x.order_id,
            //            principalSchema: "sales",
            //            principalTable: "orders",
            //            principalColumn: "order_id",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK__product_category",
            //            column: x => x.product_id,
            //            principalSchema: "production",
            //            principalTable: "products",
            //            principalColumn: "product_id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Invoices",
            //    schema: "Purchases",
            //    columns: table => new
            //    {
            //        invoice_no = table.Column<int>(type: "int", nullable: false),
            //        supplier_id = table.Column<int>(type: "int", nullable: true),
            //        invoice_date = table.Column<DateOnly>(type: "date", nullable: true),
            //        total_amount = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
            //        notes = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
            //        order_id = table.Column<int>(type: "int", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK__Invoices__F58CA1E3318C8D40", x => x.invoice_no);
            //        table.ForeignKey(
            //            name: "FK__Invoices__suppli__282DF8C2",
            //            column: x => x.supplier_id,
            //            principalSchema: "Purchases",
            //            principalTable: "Suppliers",
            //            principalColumn: "supplier_id");
            //        table.ForeignKey(
            //            name: "fk_invoice_order",
            //            column: x => x.order_id,
            //            principalSchema: "Purchases",
            //            principalTable: "sub_Orders",
            //            principalColumn: "order_id");
            //    });

            //migrationBuilder.CreateTable(
            //    name: "sub_Order_Items",
            //    schema: "Purchases",
            //    columns: table => new
            //    {
            //        order_item_id = table.Column<int>(type: "int", nullable: false),
            //        order_id = table.Column<int>(type: "int", nullable: true),
            //        product_id = table.Column<int>(type: "int", nullable: true),
            //        quantity = table.Column<int>(type: "int", nullable: true),
            //        price = table.Column<decimal>(type: "decimal(10,2)", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK__sub_Orde__3764B6BC046D8F72", x => x.order_item_id);
            //        table.ForeignKey(
            //            name: "FK__sub_Order__product",
            //            column: x => x.product_id,
            //            principalSchema: "production",
            //            principalTable: "products",
            //            principalColumn: "product_id");
            //        table.ForeignKey(
            //            name: "FK_sub_Order_Orders",
            //            column: x => x.order_id,
            //            principalSchema: "Purchases",
            //            principalTable: "sub_Orders",
            //            principalColumn: "order_id");
            //    });

            //migrationBuilder.CreateTable(
            //    name: "StockMovement",
            //    schema: "inventory",
            //    columns: table => new
            //    {
            //        MovementId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Product_Id = table.Column<int>(type: "int", nullable: false),
            //        StockEntryId = table.Column<int>(type: "int", nullable: false),
            //        TransferId = table.Column<int>(type: "int", nullable: true),
            //        MovementType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
            //        Quantity = table.Column<int>(type: "int", nullable: false),
            //        MovementDate = table.Column<DateTime>(type: "datetime", nullable: false),
            //        Reference = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
            //        CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
            //        UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: true),
            //        CreatedBy = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
            //        UpdatedBy = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK__StockMov__D1822446A9F704B9", x => x.MovementId);
            //        table.ForeignKey(
            //            name: "FK_StockMovement_Product",
            //            column: x => x.Product_Id,
            //            principalSchema: "production",
            //            principalTable: "products",
            //            principalColumn: "product_id");
            //        table.ForeignKey(
            //            name: "FK_StockMovement_StockEntry",
            //            column: x => x.StockEntryId,
            //            principalSchema: "inventory",
            //            principalTable: "StockEntry",
            //            principalColumn: "EntryId");
            //        table.ForeignKey(
            //            name: "FK_StockMovement_Transfer",
            //            column: x => x.TransferId,
            //            principalSchema: "inventory",
            //            principalTable: "WarehouseTransfer",
            //            principalColumn: "TransferId");
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Payment",
            //    schema: "Purchases",
            //    columns: table => new
            //    {
            //        shipment_id = table.Column<int>(type: "int", nullable: false),
            //        payment_id = table.Column<int>(type: "int", nullable: true),
            //        shipment_date = table.Column<DateOnly>(type: "date", nullable: true),
            //        shipment_status = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
            //        shipment_method = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
            //        invoice_no = table.Column<int>(type: "int", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK__Payment__41466E59BB412024", x => x.shipment_id);
            //        table.ForeignKey(
            //            name: "fk_invoice_payment",
            //            column: x => x.invoice_no,
            //            principalSchema: "Purchases",
            //            principalTable: "Invoices",
            //            principalColumn: "invoice_no");
            //    });

            //migrationBuilder.CreateIndex(
            //    name: "IX_Invoices_order_id",
            //    schema: "Purchases",
            //    table: "Invoices",
            //    column: "order_id");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Invoices_supplier_id",
            //    schema: "Purchases",
            //    table: "Invoices",
            //    column: "supplier_id");

            //migrationBuilder.CreateIndex(
            //    name: "IX_order_items_product_id",
            //    schema: "sales",
            //    table: "order_items",
            //    column: "product_id");

            //migrationBuilder.CreateIndex(
            //    name: "IX_orders_customer_id",
            //    schema: "sales",
            //    table: "orders",
            //    column: "customer_id");

            //migrationBuilder.CreateIndex(
            //    name: "IX_orders_staff_id",
            //    schema: "sales",
            //    table: "orders",
            //    column: "staff_id");

            //migrationBuilder.CreateIndex(
            //    name: "IX_orders_store_id",
            //    schema: "sales",
            //    table: "orders",
            //    column: "store_id");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Payment_invoice_no",
            //    schema: "Purchases",
            //    table: "Payment",
            //    column: "invoice_no");

            //migrationBuilder.CreateIndex(
            //    name: "IX_PriceHistory_EffectiveDate",
            //    schema: "inventory",
            //    table: "PriceHistory",
            //    column: "EffectiveDate");

            //migrationBuilder.CreateIndex(
            //    name: "IX_PriceHistory_Product_Id",
            //    schema: "inventory",
            //    table: "PriceHistory",
            //    column: "Product_Id");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Product_Name",
            //    schema: "production",
            //    table: "products",
            //    column: "product_name");

            //migrationBuilder.CreateIndex(
            //    name: "IX_products_brand_id",
            //    schema: "production",
            //    table: "products",
            //    column: "brand_id");

            //migrationBuilder.CreateIndex(
            //    name: "IX_products_category_id",
            //    schema: "production",
            //    table: "products",
            //    column: "category_id");

            //migrationBuilder.CreateIndex(
            //    name: "IX_ProductTranslation_Language",
            //    schema: "inventory",
            //    table: "ProductTranslation",
            //    column: "Language");

            //migrationBuilder.CreateIndex(
            //    name: "IX_ProductTranslation_ProductId",
            //    schema: "inventory",
            //    table: "ProductTranslation",
            //    column: "ProductId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_ProductVariant_ProductId",
            //    schema: "inventory",
            //    table: "ProductVariant",
            //    column: "ProductId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_staffs_manager_id",
            //    schema: "sales",
            //    table: "staffs",
            //    column: "manager_id");

            //migrationBuilder.CreateIndex(
            //    name: "IX_staffs_store_id",
            //    schema: "sales",
            //    table: "staffs",
            //    column: "store_id");

            //migrationBuilder.CreateIndex(
            //    name: "UQ__staffs__AB6E6164A69EC0CF",
            //    schema: "sales",
            //    table: "staffs",
            //    column: "email",
            //    unique: true);

            //migrationBuilder.CreateIndex(
            //    name: "IX_StockEntry_BatchNumber",
            //    schema: "inventory",
            //    table: "StockEntry",
            //    column: "BatchNumber");

            //migrationBuilder.CreateIndex(
            //    name: "IX_StockEntry_Product_Id",
            //    schema: "inventory",
            //    table: "StockEntry",
            //    column: "Product_Id");

            //migrationBuilder.CreateIndex(
            //    name: "IX_StockEntry_ProductVariantVariantId",
            //    schema: "inventory",
            //    table: "StockEntry",
            //    column: "ProductVariantVariantId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_StockEntry_WarehouseId",
            //    schema: "inventory",
            //    table: "StockEntry",
            //    column: "WarehouseId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_StockMovement_MovementDate",
            //    schema: "inventory",
            //    table: "StockMovement",
            //    column: "MovementDate");

            //migrationBuilder.CreateIndex(
            //    name: "IX_StockMovement_Product_Id",
            //    schema: "inventory",
            //    table: "StockMovement",
            //    column: "Product_Id");

            //migrationBuilder.CreateIndex(
            //    name: "IX_StockMovement_StockEntryId",
            //    schema: "inventory",
            //    table: "StockMovement",
            //    column: "StockEntryId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_StockMovement_TransferId",
            //    schema: "inventory",
            //    table: "StockMovement",
            //    column: "TransferId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_stocks_product_id",
            //    schema: "production",
            //    table: "stocks",
            //    column: "product_id");

            //migrationBuilder.CreateIndex(
            //    name: "IX_sub_Order_Items_order_id",
            //    schema: "Purchases",
            //    table: "sub_Order_Items",
            //    column: "order_id");

            //migrationBuilder.CreateIndex(
            //    name: "IX_sub_Order_Items_product_id",
            //    schema: "Purchases",
            //    table: "sub_Order_Items",
            //    column: "product_id");

            //migrationBuilder.CreateIndex(
            //    name: "IX_sub_Orders_staff_id",
            //    schema: "Purchases",
            //    table: "sub_Orders",
            //    column: "staff_id");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Warehouse_Code",
            //    schema: "inventory",
            //    table: "Warehouse",
            //    column: "Code");

            //migrationBuilder.CreateIndex(
            //    name: "UQ_Warehouse_Code",
            //    schema: "inventory",
            //    table: "Warehouse",
            //    column: "Code",
            //    unique: true);

            //migrationBuilder.CreateIndex(
            //    name: "IX_WarehouseTransfer_DestWarehouseId",
            //    schema: "inventory",
            //    table: "WarehouseTransfer",
            //    column: "DestWarehouseId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_WarehouseTransfer_ProductId",
            //    schema: "inventory",
            //    table: "WarehouseTransfer",
            //    column: "ProductId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_WarehouseTransfer_SourceWarehouseId",
            //    schema: "inventory",
            //    table: "WarehouseTransfer",
            //    column: "SourceWarehouseId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_WarehouseTransfer_Status",
            //    schema: "inventory",
            //    table: "WarehouseTransfer",
            //    column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "order_items",
                schema: "sales");

            migrationBuilder.DropTable(
                name: "Payment",
                schema: "Purchases");

            migrationBuilder.DropTable(
                name: "PriceHistory",
                schema: "inventory");

            migrationBuilder.DropTable(
                name: "ProductTranslation",
                schema: "inventory");

            migrationBuilder.DropTable(
                name: "StockMovement",
                schema: "inventory");

            migrationBuilder.DropTable(
                name: "stocks",
                schema: "production");

            migrationBuilder.DropTable(
                name: "sub_Order_Items",
                schema: "Purchases");

            migrationBuilder.DropTable(
                name: "SystemSettings");

            migrationBuilder.DropTable(
                name: "orders",
                schema: "sales");

            migrationBuilder.DropTable(
                name: "Invoices",
                schema: "Purchases");

            migrationBuilder.DropTable(
                name: "StockEntry",
                schema: "inventory");

            migrationBuilder.DropTable(
                name: "WarehouseTransfer",
                schema: "inventory");

            migrationBuilder.DropTable(
                name: "customers",
                schema: "sales");

            migrationBuilder.DropTable(
                name: "Suppliers",
                schema: "Purchases");

            migrationBuilder.DropTable(
                name: "sub_Orders",
                schema: "Purchases");

            migrationBuilder.DropTable(
                name: "ProductVariant",
                schema: "inventory");

            migrationBuilder.DropTable(
                name: "Warehouse",
                schema: "inventory");

            migrationBuilder.DropTable(
                name: "staffs",
                schema: "sales");

            migrationBuilder.DropTable(
                name: "products",
                schema: "production");

            migrationBuilder.DropTable(
                name: "stores",
                schema: "sales");

            migrationBuilder.DropTable(
                name: "brands",
                schema: "production");

            migrationBuilder.DropTable(
                name: "ProductCategory",
                schema: "production");
        }
    }
}
