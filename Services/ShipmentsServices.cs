using CsvHelper;
using CsvHelper.Configuration;
using ExcelDataReader;
using MesaCore.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.FileIO;
using System.Data;
using System.Diagnostics;
using System.Formats.Asn1;
using System.Globalization;

namespace MesaCore.Services
{
    public class ShipmentsServices
    {
        private readonly MesaCoreContext _context;
        
        public ShipmentsServices(MesaCoreContext context)
        {
            _context = context;
        }

        public async Task<int> GetTotalPagesAsync(int PageSize)
        {
            var totalItems = await _context.Shipments.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)PageSize);
            return totalPages;
        }

        public async Task<List<Shipment>> GetPagedShipments(int PageNumber, int PageSize)
        {
            try
            {
                var skip = PageNumber == 1 ? 0 : (PageNumber - 1) * PageSize;
                return _context.Shipments.OrderBy(s => s.Id).Skip(skip).Take(PageSize).ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return new List<Shipment>();
        }

        public async Task<List<Shipment>> Filters(string shopOrder, string partNumber, int? packer, DateTime? startDate)
        {
            var query = _context.Shipments.AsQueryable();

            if (!string.IsNullOrWhiteSpace(shopOrder))
            {
                query = query.Where(s => s.ShopOrder.Contains(shopOrder));
            }

            if (!string.IsNullOrWhiteSpace(partNumber))
            {
                query = query.Where(p => p.PartNumber.Contains(partNumber));
            }

            if (packer.HasValue)
            {
                query = query.Where(p => p.Packer == packer.Value);
            }

            if (startDate.HasValue)
            {
                DateTime nextDay = startDate.Value.AddDays(1);
                query = query.Where(d => d.Date.Value.Date >= startDate.Value.Date && d.Date.Value.Date < nextDay.Date);
            }


            return await query.ToListAsync();
        }

        public List<Shipment> GetShipments()
        {
            var ShipmetsList = _context.Shipments.FromSqlRaw<Shipment>("Sp_GetShipments").ToList();

            return ShipmetsList;
        }

        public async Task InserDataIntoDatabase(DataTable dt)
        {
            try
            {
                if(dt == null || dt.Rows.Count == 0)
                {
                    Debug.WriteLine("El datatable es nulo o vacio");
                    return;
                }

                foreach (DataRow row in dt.Rows)
                {
                    var shipments = new Shipment
                    {
                        Packer = Convert.ToInt32(row["EMPACADOR"]),
                        DataNo = Convert.ToInt32(row["DATA No"]),
                        Date = Convert.ToDateTime(row["DATE"]),
                        ShopOrder = row["SHOP ORDER"].ToString(),
                        PartNumber = row["PART NUMBER"].ToString(),
                        Qty = Convert.ToInt32(row["QTY"])
                    };
                    string tiempoStr = row["TIME"].ToString();

                    // Intentar convertir la cadena a TimeSpan
                    if (TimeSpan.TryParse(tiempoStr, out TimeSpan tiempo))
                    {
                        shipments.Tiempo = tiempo; 
                    }
                    else
                    {
                        // Si la conversión falla, manejar el error o asignar un valor predeterminado
                        Debug.WriteLine($"No se pudo convertir '{tiempoStr}' a TimeSpan");
                        shipments.Tiempo = TimeSpan.Zero; 
                    }
                    _context.Shipments.Add(shipments);
                }
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al insertar datos en la base de datos: {ex.Message}");
            }
        }
    }
}