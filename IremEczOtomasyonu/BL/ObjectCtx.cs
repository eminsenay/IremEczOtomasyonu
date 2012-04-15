using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IremEczOtomasyonu.BL
{
    public class ObjectCtx
    {
        private static Model1Container _context;
        public static Model1Container Context
        {
            get { return _context ?? (_context = new Model1Container()); }
        }

        public static void Reload()
        {
            _context = new Model1Container();
        }
    }
}
