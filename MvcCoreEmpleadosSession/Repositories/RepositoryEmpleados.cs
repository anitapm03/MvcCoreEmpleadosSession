using Microsoft.EntityFrameworkCore;
using MvcCoreEmpleadosSession.Data;
using MvcCoreEmpleadosSession.Models;

namespace MvcCoreEmpleadosSession.Repositories
{
    public class RepositoryEmpleados
    {

        private EmpleadosContext context;
        public RepositoryEmpleados(EmpleadosContext context)
        {
            this.context = context;
        }

        public async Task<List<Empleado>> GetEmpleadosAsync()
        {
            var empleados =
                await this.context.Empleados.ToListAsync();
            return empleados;

        }

        public async Task<Empleado> FindEmpleadoAsync(int idempleado)
        {
            return await
                this.context.Empleados.FirstOrDefaultAsync
                (z => z.IdEmpleado == idempleado);
        }
    }
}
