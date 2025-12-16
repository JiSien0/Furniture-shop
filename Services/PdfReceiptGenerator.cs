using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FurnitureShop.Database;
using FurnitureShop.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace FurnitureShop.Services
{
    public class PdfReceiptGenerator
    {
        public PdfReceiptGenerator()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public string GenerateReceipt(Receipt receipt, List<CartItem> items, DatabaseHelper database)
        {
            var fileName = $"Receipt_{receipt.ReceiptNumber}.pdf";
            var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header()
                        .Text("Меблевий магазин - Чек")
                        .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(column =>
                        {
                            column.Spacing(10);

                            // Информация о чеке
                            column.Item().Row(row =>
                            {
                                row.RelativeItem().Column(col =>
                                {
                                    col.Item().Text($"Номер чека: {receipt.ReceiptNumber}").SemiBold();
                                    col.Item().Text($"Дата: {receipt.Date:dd.MM.yyyy HH:mm}");
                                    col.Item().Text($"Тип отримання: {(receipt.Delivery ? "Доставка" : "Самовивіз")}");
                                });
                            });

                            column.Item().LineHorizontal(1);

                            // Таблица с товарами
                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(3);
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(1);
                                });

                                // Заголовок таблицы
                                table.Header(header =>
                                {
                                    header.Cell().Element(CellStyle).Text("Найменування").SemiBold();
                                    header.Cell().Element(CellStyle).Text("Ціна").SemiBold();
                                    header.Cell().Element(CellStyle).Text("Кіл-ть").SemiBold();
                                    header.Cell().Element(CellStyle).Text("Сума").SemiBold();

                                    static IContainer CellStyle(IContainer container)
                                    {
                                        return container.BorderBottom(1).BorderColor(Colors.Grey.Medium).PaddingVertical(5);
                                    }
                                });

                                // Строки с товарами
                                foreach (var item in items)
                                {
                                    table.Cell().Element(CellStyle).Text(item.Furniture.Name);
                                    table.Cell().Element(CellStyle).Text($"{item.Furniture.Price:N2} ₴");
                                    table.Cell().Element(CellStyle).Text(item.Quantity.ToString());
                                    table.Cell().Element(CellStyle).Text($"{item.TotalPrice:N2} ₴");

                                    static IContainer CellStyle(IContainer container)
                                    {
                                        return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                                    }
                                }
                            });

                            column.Item().LineHorizontal(1);

                            // Разом
                            column.Item().AlignRight().Text($"Разом: {receipt.TotalPrice:N2} ₴").FontSize(16).SemiBold();

                            // Координати для самовивозу
                            if (!receipt.Delivery)
                            {
                                column.Item().PaddingTop(20).Column(col =>
                                {
                                    col.Item().Text("Координати на складі:").SemiBold().FontSize(14);
                                    col.Item().PaddingTop(5);

                                    foreach (var item in items)
                                    {
                                        col.Item().Text($"{item.Furniture.Name}: Ряд {item.Furniture.StorageRow}, Полиця {item.Furniture.StorageShelf}");
                                    }
                                });
                            }
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Сторінка ");
                            x.CurrentPageNumber();
                            x.Span(" з ");
                            x.TotalPages();
                        });
                });
            })
            .GeneratePdf(filePath);

            return filePath;
        }
    }
}
