using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlfinfData.Models.Odoo;

namespace AlfinfData.Services.odoo
{
    
    public interface ITarjetaNFCServices
    {
        //Task<IEnumerable<TarjetaNFC>> GuardarTarjetaNFC();
    }
    public class TarjetaNFCServices : ITarjetaNFCServices
    {
        private readonly OdooService _odoo;

        public TarjetaNFCServices(OdooService odoo)
            => _odoo = odoo;

        //public async Task<IEnumerable<TarjetaNFC>> GuardarTarjetaNFC()
        //{
            
        //}


    }
}
