using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MvcCoreEmpleadosSession.Extensions;
using MvcCoreEmpleadosSession.Models;
using MvcCoreEmpleadosSession.Repositories;

namespace MvcCoreEmpleadosSession.Controllers
{
    public class EmpleadosController : Controller
    {
        private RepositoryEmpleados repo;
        private IMemoryCache memoryCache;

        public EmpleadosController(RepositoryEmpleados repo, IMemoryCache memoryCache)
        {
            this.repo = repo;
            this.memoryCache = memoryCache;
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

        public async Task<IActionResult> SessionEmpleados
            (int? idempleado)
        {
            if (idempleado != null)
            {
                //buscamos al empleado
                Empleado empleado = await
                    this.repo.FindEmpleadoAsync(idempleado.Value);
                //en session almacenaremos un conjunto de empleados
                List<Empleado> empleadosList;
                //preguntamos si hay empleados guardados en session
                if (HttpContext.Session.GetObject<List<Empleado>>("EMPLEADOS") != null)
                {
                    //si tenemos los recuperamos
                    empleadosList =
                        HttpContext.Session.GetObject<List<Empleado>>("EMPLEADOS");
                }
                else
                {
                    //si no hay creamos la coleccion para almacenarlos
                    empleadosList = new List<Empleado> ();
                }
                //almacenamos el nuevo empleado en session
                empleadosList.Add(empleado);
                //guardamos la coleccion dentro de session
                HttpContext.Session.SetObject("EMPLEADOS", empleadosList);
                ViewData["MENSAJE"] = "Empleado almacenado correctamente";
            }

            List<Empleado> empleados = await
                this.repo.GetEmpleadosAsync();
            return View(empleados);
        }

        public IActionResult EmpleadosAlmacenados()
        {
            return View();
        }

        //[ResponseCache(Duration = 80, Location = ResponseCacheLocation.Client)]
        public async Task<IActionResult> SessionEmpleadosOk
            (int? idempleado, int? idfavorito)
        {

            if (idfavorito != null)
            {
                List<Empleado> empleadosFav;
                if(this.memoryCache.Get("FAV") == null)
                {
                    //creamos coleccion
                    empleadosFav = new List<Empleado>();

                }
                else
                {
                    //recuperamos los emp
                    empleadosFav =
                        this.memoryCache.Get<List<Empleado>>("FAV");
                }

                //buscamos al emp por su id de favorito
                Empleado emp = await this.repo.FindEmpleadoAsync(idfavorito.Value);
                empleadosFav.Add(emp);
                //almacenamos los nuevos datos en el cache
                this.memoryCache.Set("FAV", empleadosFav);
            }

            if (idempleado != null)
            {
                //buscamos al emp
                Empleado empleado = await
                    this.repo.FindEmpleadoAsync(idempleado.Value);

                List<int> idEmpleadosList;

                if (HttpContext.Session.GetString("IDSEMPLEADOS") != null)
                {
                    idEmpleadosList =
                        HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS");
                }
                else
                {
                    idEmpleadosList = new List<int>();
                }
                //almacenamos en la coleccion
                idEmpleadosList.Add(idempleado.Value);
                //almacenamos la coleccion en session con los cambios
                HttpContext.Session.SetObject("IDSEMPLEADOS", idEmpleadosList);
                ViewData["MENSAJE"] = "Empleado almacenado correctamente";
            }

            /*comprobamos si hay algo en session
            List<int> ids = HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS");
            if (ids == null)
            {
                List<Empleado> empleados = await
               this.repo.GetEmpleadosAsync();
                return View(empleados);
            }
            else
            {
                List<Empleado> empleados = await
                    this.repo.GetEmpleadosNotSessionAsync(ids);
                return View(empleados);
            }*/
            List<Empleado> empleados = await
                this.repo.GetEmpleadosAsync();
            return View(empleados);
        }

        public async Task<IActionResult> EmpleadosAlmacenadosOk(int? ideliminar)
        {
            //recuperamos los empleados de session
            List<int> ids =
                HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS");

            if (ids != null)
            {
                //debemos eliminar de session
                if(ideliminar != null)
                {
                    //nos han enviado dato para borrar
                    ids.Remove(ideliminar.Value);

                    if (ids.Count() == 0)
                    {
                        HttpContext.Session.Remove("IDSEMPLEADOS");
                    }
                    else
                    {
                        //almacenamos de nuevo los datos de session 
                        HttpContext.Session.SetObject("IDSEMPLEADOS", ids);
                    }
                    
                }

                List<Empleado> empleados = await
               this.repo.GetEmpleadosSessionAsync(ids);
                return View(empleados);
            }
            else
            {
                ViewData["MENSAJE"] = "No hay empleados almacenados";
                return View();
            }
           
        }

        public IActionResult EmpleadosFav()
        {
            //preguntamos si hay favs
            if(this.memoryCache.Get("FAV") == null)
            {
                ViewData["MENSAJE"] = "No hay empleados favoritos";
                return View();

            }
            else
            {
                List<Empleado> favs =
                    this.memoryCache.Get<List<Empleado>>("FAV");
                return View(favs);
            }
        }

    }
}
