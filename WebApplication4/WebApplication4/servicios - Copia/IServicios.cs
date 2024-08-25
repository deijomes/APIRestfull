namespace WebApplication4.servicios
{
    public interface IServicios
    {
        void RealizarTarea();
    }

    public class servicioA : IServicios
    {
        private readonly ILogger<servicioA> logger;

        public servicioA(ILogger<servicioA>logger)
        {
            this.logger = logger;
        }

        public void RealizarTarea()
        {
            
        }
    }

    public class servicioB : IServicios
    {
        public void RealizarTarea()
        {
           
        }
    }
}
