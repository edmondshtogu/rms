using RMS.Messages;
using System.Web.Mvc;

namespace RequestsManagementSystem.Controllers
{
    public class AppController : Controller
    {
        protected readonly IBus _bus;

        /// <summary>
        /// the controller cunstructor
        /// </summary>
        /// <param name="bus"></param>
        protected AppController(IBus bus)
        {
            _bus = bus;
        }
    }
}