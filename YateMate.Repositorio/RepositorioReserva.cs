using Microsoft.VisualBasic;
using YateMate.Aplicacion.Entidades;
using YateMate.Aplicacion.Interfaces;

namespace YateMate.Repositorio;

public class RepositorioReserva : IRepositorioReserva
{
    public void HacerReserva(Reserva reserva)
    {
        using (var context = ApplicationDbContext.CrearContexto())
        {
            var subalquiler = context.Subalquileres.FirstOrDefault(s => s.Id == reserva.SubalquilerId);
            if (subalquiler != null)
            {
                if (CheckearDisponibilidad(reserva, subalquiler))
                {
                    subalquiler.Reservas.Add(reserva);
                    context.Add(reserva);
                    context.SaveChanges();
                }
            }
        }
    }

    public void CancelarReserva(int id)
    {
        using (var context = ApplicationDbContext.CrearContexto())
        {
            var reserva = context.Reservas.FirstOrDefault(r => r.Id == id);
            if (reserva != null)
            {
                context.Remove(reserva);
                context.SaveChanges();
            }
        }
    }

    public void ModificarReserva(Reserva reserva)
    {
        using (var context = ApplicationDbContext.CrearContexto())
        {
            var vieja = context.Reservas.FirstOrDefault(r => r.Id == reserva.Id);
            if(vieja == null) return;
            vieja.FechaInicio = reserva.FechaInicio;
            vieja.FechaFin = reserva.FechaFin;
            context.SaveChanges();
        }
    }

    public List<Reserva> ListarReservasDe(string usuarioId)
    {
        using (var context = ApplicationDbContext.CrearContexto())
        {
            return context.Reservas.Where(r => r.UsuarioId.Equals(usuarioId)).ToList();
        }
    }

    private bool CheckearDisponibilidad(Reserva reserva, Subalquiler subalquiler)
    {
        return !subalquiler.Reservas.Any(r => (r.FechaInicio >= reserva.FechaInicio && r.FechaInicio <= reserva.FechaFin) 
                                             || (r.FechaFin >= reserva.FechaInicio && r.FechaFin <= reserva.FechaFin));
    }
}