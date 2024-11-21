using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using UspgPOS.Data;
using UspgPOS.Models;

namespace UspgPOS.Controllers
{
	public class VentasController : Controller
	{
		private readonly AppDbContext _context;

		public VentasController(AppDbContext context)
		{
			_context = context;
		}

		// GET: Ventas
		public async Task<IActionResult> Index()
		{
			var appDbContext = _context.Ventas.Include(v => v.Cliente).Include(v => v.Sucursal);
			return View(await appDbContext.ToListAsync());
		}

		// GET: Ventas/Details/5
		public async Task<IActionResult> Details(long? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var venta = await _context.Ventas
				.Include(v => v.Cliente)
				.Include(v => v.Sucursal)
				.FirstOrDefaultAsync(m => m.Id == id);
			if (venta == null)
			{
				return NotFound();
			}

			return View(venta);
		}

		// GET: Ventas/Create
		public IActionResult Create()
		{
			ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Nombre");
			ViewData["SucursalId"] = new SelectList(_context.Sucursales, "Id", "Nombre", HttpContext.Session.GetString("SucursalId"));
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("Id,Fecha,Total,ClienteId,SucursalId")] Venta venta)
		{
			if (ModelState.IsValid)
			{
				_context.Add(venta);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Nombre", venta.ClienteId);
			ViewData["SucursalId"] = new SelectList(_context.Sucursales, "Id", "Nombre", venta.SucursalId);
			return View(venta);
		}

		// GET: Ventas/Edit/5
		public async Task<IActionResult> Edit(long? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var venta = await _context.Ventas.FindAsync(id);
			if (venta == null)
			{
				return NotFound();
			}
			ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Nombre", venta.ClienteId);
			ViewData["SucursalId"] = new SelectList(_context.Sucursales, "Id", "Nombre", venta.SucursalId);
			return View(venta);
		}

		// POST: Ventas/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(long? id, [Bind("Id,Fecha,Total,ClienteId,SucursalId")] Venta venta)
		{
			if (id != venta.Id)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					_context.Update(venta);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!VentaExists(venta.Id))
					{
						return NotFound();
					}
					else
					{
						throw;
					}
				}
				return RedirectToAction(nameof(Index));
			}
			ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Nombre", venta.ClienteId);
			ViewData["SucursalId"] = new SelectList(_context.Sucursales, "Id", "Nombre", venta.SucursalId);
			return View(venta);
		}

		// GET: Ventas/Delete/5
		public async Task<IActionResult> Delete(long? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var venta = await _context.Ventas
				.Include(v => v.Cliente)
				.Include(v => v.Sucursal)
				.FirstOrDefaultAsync(m => m.Id == id);
			if (venta == null)
			{
				return NotFound();
			}

			return View(venta);
		}

		// POST: Ventas/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(long? id)
		{
			var venta = await _context.Ventas.FindAsync(id);
			if (venta != null)
			{
				_context.Ventas.Remove(venta);
			}

			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		private bool VentaExists(long? id)
		{
			return _context.Ventas.Any(e => e.Id == id);
		}

		public IActionResult ImprimirFactura(long id)
		{
			QuestPDF.Settings.License = LicenseType.Community;

			var venta = _context.Ventas
				.Include(v => v.Cliente)
				.Include(v => v.Sucursal)
				.Include(v => v.DetallesVenta)
					.ThenInclude(d => d.Producto)
				.FirstOrDefault(v => v.Id == id);

			if (venta == null)
			{
				return NotFound();
			}

			var logo = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/icons/apple-touch-icon-180x180.png");
			var monedaGuatemala = new System.Globalization.CultureInfo("es-GT");

			var document = Document.Create(container =>
			{
				container.Page(page =>
				{
					page.Size(PageSizes.A4);
					page.Margin(1, Unit.Centimetre);


					page.Header().Row(header =>
					{
						header.AutoItem().Text($"Fecha: {venta.Fecha.ToString("dd/MM/yyyy")} ");
						header.RelativeItem().Text("Factura Comercial").FontSize(24).Bold().AlignCenter();
						header.ConstantItem(80).Image(logo, ImageScaling.FitArea);

					});

					page.Content().Column(column =>
					{

						column.Item().Row(row =>
						{
							row.RelativeItem().Text($"Nombre: {venta.Cliente?.Nombre}");
						});

						column.Item().Row(row =>
						{
							row.RelativeItem().Text($"Dirección: ___________________");
							row.RelativeItem().Text($"Teléfono: ___________________");
						});



						column.Item().Row(row =>
						{
							row.RelativeItem().Text($"Sucursal: {venta.Sucursal?.Nombre}");
						});

						column.Item().PaddingVertical(1, Unit.Centimetre);

						column.Item().Table(table =>
						{
							table.ColumnsDefinition(columns =>
							{
								columns.RelativeColumn();
								columns.RelativeColumn();
								columns.RelativeColumn();
								columns.RelativeColumn();
        //                        columns.ConstantColumn(60);
								//columns.ConstantColumn(100);
								//columns.ConstantColumn(100);
							});

							table.Header(header =>
							{
								string fontColor = "#09506d";

                                header.Cell().Element(EstiloCelda).Text("Cantidad").FontColor(fontColor).FontSize(12).Bold();
								header.Cell().Element(EstiloCelda).Text("Descripción").FontColor(fontColor).FontSize(12).Bold();
								header.Cell().Element(EstiloCelda).Text("Unidad").FontColor(fontColor).FontSize(12).Bold();
								header.Cell().Element(EstiloCelda).Text("Total").FontColor(fontColor).FontSize(12).Bold();
								//header.Cell().Element(EstiloCelda).Text("Subtotal").FontSize(12).Bold();

								static IContainer EstiloCelda(IContainer container)
								{
									return container.Background("#3b91ad").Border(1).BorderColor("#3b91ad").Padding(5).AlignCenter();
								}

							});


							foreach (DetalleVenta detalle in venta.DetallesVenta)
							{
								table.Cell().Border(1).BorderColor("#c0c0c0").Padding(5).AlignCenter().Text(detalle.Cantidad.ToString());
								table.Cell().Border(1).BorderColor("#c0c0c0").Padding(5).AlignCenter().Text(detalle.Producto?.Nombre ?? "N/A");
								table.Cell().Border(1).BorderColor("#c0c0c0").Padding(5).AlignCenter().Text(detalle.PrecioUnitario.ToString("C", monedaGuatemala));
								table.Cell().Border(1).BorderColor("#c0c0c0").Padding(5).AlignCenter().Text((detalle.Cantidad * detalle.PrecioUnitario).ToString("C", monedaGuatemala));
                            }

							
						});

                        column.Item().PaddingVertical(1, Unit.Centimetre);

						column.Item().Row(row =>
						{

                            column.Item().Column(col =>
                            {

                                column.Item().AlignRight().MaxWidth(150).Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                    });

                                    table.Cell()
                                    .Background("#3b91ad").Border(1).BorderColor("#3b91ad")
                                        .Padding(5)
                                        .AlignRight()
                                        .Text("TOTAL")
                                        .FontSize(12)
                                        .Bold();

                                    table.Cell()
                                        .Background("#f0f0f0")
                                        .Border(1)
                                        .BorderColor("#c0c0c0")
                                        .Padding(5)
                                        .AlignRight()
                                        .Text(venta.Total.ToString("C", monedaGuatemala))
                                        .FontSize(12);
                                });
                            });
                            column.Item().Row(row =>
							{
								row.RelativeItem().Text($"Dirección: {venta.Sucursal?.Ciudad}, {venta.Sucursal?.Area}");
								row.RelativeItem().Text($"@sitioincreible");
							});

							column.Item().Row(row =>
							{
								row.RelativeItem().Text($"Hola@sitioincreible.com");
								row.RelativeItem().Text($"(55) 1234-5678");
							});


                        });

						column.Item().PaddingVertical(40);

                        column.Item().AlignCenter().Column(row =>
                        {
                            row.Item().Text($"________________________________");
                            row.Item().AlignCenter().Text($"FIRMA CLIENTE");
                        });
                    });

					page.Footer().Text($"USPG POS - 2024 - {venta.Sucursal?.Nombre}").FontSize(10).AlignCenter();
				});
			});

			var stream = new MemoryStream();
			document.GeneratePdf(stream);
			stream.Position = 0;

			return File(stream, "application/pdf", $"Factura_{id}.pdf");
		}

	}
}
