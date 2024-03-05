using Microsoft.AspNetCore.Mvc;
using MvcCoreEmpleadosSession.Models;
using MvcCoreEmpleadosSession.Repositories;

namespace MvcCoreEmpleadosSession.Controllers
{
    public class EmpleadosController : Controller
    {
        private RepositoryEmpleados repo;

        public EmpleadosController(RepositoryEmpleados repo)
        {
            this.repo = repo;
        }
        public async Task<IActionResult> SessionSalario(int? salario)
        {
            if (salario != null)
            {
                //necesitamos almacenar el salario total de todos los
                //empleados de session
                int sumasalarial = 0;
                if (HttpContext.Session.GetString("SUMASALARIAL") != null)
                {
                    //recuperamos la suma salarial
                    sumasalarial = int.Parse
                        (HttpContext.Session.GetString("SUMASALARIAL"));
                }
                //realizamos la suma del salario recibido
                sumasalarial += salario.Value;
                //almacenamos la nueva suma
                HttpContext.Session.SetString("SUMASALARIAL", sumasalarial.ToString());
                ViewData["MENSAJE"] = "Salario almacenado: " + salario.Value;
            }
            List<Empleado> empleados = await
                this.repo.GetEmpleadosAsync();
            return View(empleados);
        }

        public IActionResult SumaSalarios()
        {
            return View();
        }

        public async Task<IActionResult> SessionEmpleados()
        {
            List<Empleado> empleados = await
                this.repo.GetEmpleadosAsync();
            return View(empleados);
        }


    }
}
