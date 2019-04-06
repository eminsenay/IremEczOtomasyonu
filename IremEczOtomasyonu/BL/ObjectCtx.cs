using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IremEczOtomasyonu.Models;

namespace IremEczOtomasyonu.BL
{
    public class ObjectCtx
    {
        private static PharmacyContext _context;
        public static PharmacyContext Context
        {
            get { return _context ?? (_context = new PharmacyContext()); }
        }

        public static void Reload()
        {
            _context = new PharmacyContext();
        }
    }
}
