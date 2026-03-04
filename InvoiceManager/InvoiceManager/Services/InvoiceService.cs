using System.Text.Json;
using AutoMapper;
using InvoiceManager.Common;
using InvoiceManager.Data;
using InvoiceManager.DTO;
using InvoiceManager.Models;
using InvoiceManager.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace InvoiceManager.Services
{
    public class InvoiceService(AppDbContext context, IMapper mapper) : IInvoiceService
    {
        public async Task<InvoiceResponseDTO> CreateAsync(CreateInvoiceDTO dto)
        {
            var customerExists = await context.Customers
                .AnyAsync(c => c.Id == dto.CustomerId && c.DeletedAt == null);

            if (!customerExists)
            {
                throw new KeyNotFoundException("Customer not found");
            }

            var invoice = mapper.Map<Invoice>(dto);

            foreach (var row in invoice.InvoiceRows)
            {
                row.Amount = row.Quantity * row.Rate;
            }

            invoice.TotalAmount = invoice.InvoiceRows.Sum(x => x.Amount);

            invoice.Status = InvoiceStatus.Created;
            invoice.CreatedAt = DateTimeOffset.UtcNow;
            invoice.UpdatedAt = DateTimeOffset.UtcNow;

            context.Invoices.Add(invoice);
            await context.SaveChangesAsync();

            var saved = await context.Invoices
                .Include(i => i.InvoiceRows)
                .FirstAsync(i => i.Id == invoice.Id);

            return mapper.Map<InvoiceResponseDTO>(saved);
        }

        public async Task<PagedResult<InvoiceResponseDTO>> GetPagedAsync(InvoicesQueryParams queryParams)
        {
            queryParams.Validate();

            var query = context.Invoices
                .Where(i => !i.DeletedAt.HasValue)
                .Include(i => i.InvoiceRows)
                .AsQueryable();

            if (queryParams.CustomerId.HasValue)
            {
                query = query.Where(x => x.CustomerId == queryParams.CustomerId);
            }

            if (!string.IsNullOrEmpty(queryParams.SortByStatus))
            {
                if (Enum.TryParse(queryParams.SortByStatus, out InvoiceStatus status))
                {
                    query = query.Where(x => x.Status == status);
                }
            }

            if (!string.IsNullOrWhiteSpace(queryParams.Search))
            {
                var search = queryParams.Search;
                query = query.Where(x => x.Comment.ToLower().Contains(search) ||
                                         x.Comment.ToLower().Contains(search));
            }

            query = !string.IsNullOrWhiteSpace(queryParams.Sort)
                ? ApplySorting(query, queryParams.Sort, queryParams.SortDirection!)
                : query.OrderByDescending(t => t.CreatedAt);

            var totalCount = await query.CountAsync();
            var skip = (queryParams.PageNumber - 1) * queryParams.PageSize;
            var invoices = await query.Skip(skip).Take(queryParams.PageSize).ToListAsync();
            var invoicesDto = mapper.Map<IEnumerable<InvoiceResponseDTO>>(invoices);
            return PagedResult<InvoiceResponseDTO>.Create(
                invoicesDto,
                totalCount,
                queryParams.PageNumber,
                queryParams.PageSize);
        }

        public async Task<IEnumerable<InvoiceResponseDTO>> GetAllAsync()
        {
            var invoices = await context.Invoices
                .Where(i => !i.DeletedAt.HasValue)
                .Include(i => i.InvoiceRows)
                .ToListAsync();
            return mapper.Map<IEnumerable<InvoiceResponseDTO>>(invoices);
        }

        public async Task<InvoiceResponseDTO?> GetByIdAsync(int id)
        {
            var invoice = await context.Invoices
                .Where(i => !i.DeletedAt.HasValue)
                .Include(i => i.InvoiceRows)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (invoice == null)
            {
                return null;
            }

            return mapper.Map<InvoiceResponseDTO>(invoice);
        }

        public async Task<InvoiceResponseDTO?> UpdateAsync(int id, InvoiceUpdateDTO dto)
        {
            var existingInvoice = await context.Invoices
                .FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null);

            if (existingInvoice == null)
            {
                return null;
            }

            if (existingInvoice.Status != InvoiceStatus.Created)
            {
                return null;
            }

            mapper.Map(dto, existingInvoice);

            await context.SaveChangesAsync();
            return mapper.Map<InvoiceResponseDTO>(existingInvoice);
        }

        public async Task<bool> UpdateStatusAsync(int id, InvoiceStatus newStatus)
        {
            var existingInvoice = await context.Invoices
                .FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null);
            if (existingInvoice == null)
            {
                return false;
            }

            existingInvoice.Status = newStatus;
            existingInvoice.UpdatedAt = DateTimeOffset.UtcNow;

            await context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var existingInvoice = await context.Invoices
                .FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null);
            if (existingInvoice == null)
            {
                return false;
            }

            existingInvoice.DeletedAt = DateTimeOffset.UtcNow;

            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> HardDeleteAsync(int id)
        {
            var existingInvoice = await context.Invoices
                .FirstOrDefaultAsync(x => x.Id == id);

            if (existingInvoice == null)
                return false;

            if (existingInvoice.Status != InvoiceStatus.Created)
                return false;

            context.Invoices.Remove(existingInvoice);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<InvoiceFileDTO?> DownloadInvoice(int id)
        {
            var invoice = await context
                .Invoices
                .Include(x => x.InvoiceRows)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (invoice == null)
            {
                return null;
            }

            var pdf = GeneratePdf(invoice);
            return new InvoiceFileDTO
            {
                FileBytes = pdf,
                FileName = $"{invoice.Id}.pdf"
            };
        }

        private byte[] GeneratePdf(Invoice invoice)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Header().Column(col =>
                    {
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text($"Invoice #{invoice.Id}").FontSize(22).Bold();
                                c.Item().Text($"Status: {invoice.Status}").FontSize(11)
                                    .FontColor(GetStatusColor(invoice.Status));
                            });

                            row.ConstantItem(150).AlignRight().Column(c =>
                            {
                                c.Item().Text($"Created: {invoice.CreatedAt:dd.MM.yyyy}");
                                c.Item().Text($"Period: {invoice.StartDate:dd.MM.yyyy} – {invoice.EndDate:dd.MM.yyyy}");
                            });
                        });

                        col.Item().PaddingTop(8).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                    });

                    page.Content().PaddingVertical(20).Column(col =>
                    {
                        col.Item().Background(Colors.Grey.Lighten4).Padding(12).Column(c =>
                        {
                            c.Item().Text("Customer").Bold().FontSize(12);
                            c.Item().Text($"{invoice.Customer?.Name ?? "N/A"}");
                            c.Item().Text($"{invoice.Customer?.Email ?? ""}").FontColor(Colors.Grey.Medium);
                        });

                        col.Item().PaddingTop(20).Text("Items").Bold().FontSize(13);
                        col.Item().PaddingTop(8).Table(table =>
                        {
                            table.ColumnsDefinition(c =>
                            {
                                c.RelativeColumn(4);
                                c.RelativeColumn(1);
                                c.RelativeColumn(2);
                                c.RelativeColumn(2);
                            });

                            table.Header(h =>
                            {
                                h.Cell().Background(Colors.Blue.Darken2).Padding(6)
                                    .Text(t => t.Span("Description").Bold().FontColor(Colors.White));
                                h.Cell().Background(Colors.Blue.Darken2).Padding(6)
                                    .Text(t => t.Span("Qty").Bold().FontColor(Colors.White));
                                h.Cell().Background(Colors.Blue.Darken2).Padding(6)
                                    .Text(t => t.Span("Unit Price").Bold().FontColor(Colors.White));
                                h.Cell().Background(Colors.Blue.Darken2).Padding(6)
                                    .Text(t => t.Span("Total").Bold().FontColor(Colors.White));
                            });

                            foreach (var (row, index) in invoice.InvoiceRows.Select((r, i) => (r, i)))
                            {
                                var bg = index % 2 == 0 ? Colors.White : Colors.Grey.Lighten5;

                                table.Cell().Background(bg).Padding(6).Text(row.Amount);
                                table.Cell().Background(bg).Padding(6).AlignCenter().Text(row.Quantity.ToString());
                                table.Cell().Background(bg).Padding(6).AlignRight().Text(row.Amount.ToString("C"));
                                table.Cell().Background(bg).Padding(6).AlignRight()
                                    .Text((row.Quantity * row.Amount).ToString("C"));
                            }
                        });

                        col.Item().PaddingTop(12).AlignRight().Row(row =>
                        {
                            row.ConstantItem(200).Background(Colors.Blue.Darken2).Padding(10).Row(r =>
                            {
                                r.RelativeItem().Text(t => t.Span("Total Amount:").Bold().FontColor(Colors.White));
                                r.AutoItem().Text(t =>
                                    t.Span(invoice.TotalAmount.ToString("C")).Bold().FontColor(Colors.White));
                            });
                        });

                        if (!string.IsNullOrWhiteSpace(invoice.Comment))
                        {
                            col.Item().PaddingTop(20).Column(c =>
                            {
                                c.Item().Text("Comment").Bold();
                                c.Item().PaddingTop(4).Background(Colors.Yellow.Lighten4).Padding(10)
                                    .Text(invoice.Comment);
                            });
                        }
                    });

                    page.Footer().AlignCenter().Text(t =>
                    {
                        t.Span("Page ").FontColor(Colors.Grey.Medium);
                        t.CurrentPageNumber().FontColor(Colors.Grey.Medium);
                        t.Span(" / ").FontColor(Colors.Grey.Medium);
                        t.TotalPages().FontColor(Colors.Grey.Medium);
                    });
                });
            });

            return document.GeneratePdf();
        }

        private string GetStatusColor(InvoiceStatus status) => status switch
        {
            InvoiceStatus.Paid => Colors.Green.Darken2,
            InvoiceStatus.Cancelled => Colors.Red.Darken2,
            InvoiceStatus.Rejected => Colors.Red.Medium,
            InvoiceStatus.Sent => Colors.Blue.Medium,
            InvoiceStatus.Received => Colors.Teal.Medium,
            _ => Colors.Grey.Darken1
        };

        private IQueryable<Invoice> ApplySorting(
            IQueryable<Invoice> query,
            string sortField,
            string sortDirection)
        {
            var isDescending = sortDirection.ToLower() == "desc";

            return sortField.ToLower() switch
            {
                "title" => isDescending
                    ? query.OrderByDescending(t => t.Comment)
                    : query.OrderBy(t => t.Comment),

                "createdat" => isDescending
                    ? query.OrderByDescending(t => t.CreatedAt)
                    : query.OrderBy(t => t.CreatedAt),

                "status" => isDescending
                    ? query.OrderByDescending(t => t.Status)
                    : query.OrderBy(t => t.Status),

                "priority" => isDescending
                    ? query.OrderByDescending(t => t.Status)
                    : query.OrderBy(t => t.Status),

                _ => query.OrderByDescending(t => t.CreatedAt)
            };
        }
    }
}
