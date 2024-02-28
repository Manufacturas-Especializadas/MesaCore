using CsvHelper;
using CsvHelper.Configuration;
using ExcelDataReader;
using MesaCore.Models;
using Microsoft.Data.SqlClient;
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

        public async Task<List<Shipment>> GetShipments()
        {
            return _context.Shipments.ToList();
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
                        Date = Convert.ToDateTime(row["DATE"]),
                        ShopOrder = row["SHOP ORDER"].ToString(),
                        PartNumber = row["PART NUMBER"].ToString(),
                        Qty = Convert.ToInt32(row["QTY"])
                    };
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