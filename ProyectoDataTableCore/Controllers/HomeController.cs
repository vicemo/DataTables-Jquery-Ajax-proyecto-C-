using Microsoft.AspNetCore.Mvc;
using ProyectoDataTableCore.Models;
using System.Diagnostics;


using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace ProyectoDataTableCore.Controllers
{
    public class HomeController : Controller
    {

        private readonly string cadenaSQL;

        public HomeController(IConfiguration config)
        {
            cadenaSQL = config.GetConnectionString("CadenaSQL");
        }

        public IActionResult Index()
        {
            return View();
        }

        //USAR REFERENCIAS SQLCLIENT
        [HttpGet]
        public JsonResult ListaEmpleados()
        {
            List<Empleado> lista = new List<Empleado>();

            using (var conexion = new SqlConnection(cadenaSQL))
            {

                conexion.Open();
                var cmd = new SqlCommand("sp_lista_empleados", conexion);
                cmd.CommandType = CommandType.StoredProcedure;

                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new Empleado()
                        {
                            IdEmpleado = Convert.ToInt32(dr["IdEmpleado"]),
                            Nombre = dr["Nombre"].ToString(),
                            Cargo = dr["Cargo"].ToString(),
                            Oficina = dr["Oficina"].ToString(),
                            Salario = dr["Salario"].ToString(),
                            Telefono = Convert.ToInt32(dr["Telefono"]),
                            FechaIngreso = dr["FechaIngreso"].ToString()
                        });
                    }
                }
            }


            return Json(new { data = lista });
        }
        [HttpPost] // Cambiar a HttpPost ya que estás insertando datos
        public IActionResult InsertarEmpleados(Empleado empleado)
        {
            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();
                    var cmd = new SqlCommand("sp_insertar_empleado", conexion);
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Agregar parámetros al procedimiento almacenado
                    cmd.Parameters.AddWithValue("@IdEmpleado", empleado.IdEmpleado);
                    cmd.Parameters.AddWithValue("@Nombre", empleado.Nombre);
                    cmd.Parameters.AddWithValue("@Cargo", empleado.Cargo);
                    cmd.Parameters.AddWithValue("@Oficina", empleado.Oficina);
                    cmd.Parameters.AddWithValue("@Salario", empleado.Salario);
                    cmd.Parameters.AddWithValue("@Telefono", empleado.Telefono);
                    cmd.Parameters.AddWithValue("@FechaIngreso", empleado.FechaIngreso);

                    // Ejecutar el comando
                    cmd.ExecuteNonQuery();
                }

                // Si la inserción es exitosa, puedes devolver un mensaje de éxito
                return Json(new { success = true, message = "Empleado insertado correctamente." });
            }
            catch (Exception ex)
            {
                // En caso de error, devolver un mensaje de error
                return Json(new { success = false, message = "Error al insertar el empleado: " + ex.Message });
            }
        }
    }
}
    

