using System;
using System.Collections.Generic;
using System.Linq;

namespace CapaDeEntidades
{
    // base del composite: misma interfaz para hojas y compuestos
    public abstract class Componente
    {
        public string Nombre { get; set; }

        public abstract void Agregar(Componente c);

        public abstract bool TienePermiso(string nombrePermiso);
    }

    // hoja: representa un permiso individual por ejemplo "ControlStock"
    public class PermisoSimple : Componente
    {
        public override void Agregar(Componente c)
        {
            throw new InvalidOperationException("No se pueden agregar hijos a un permiso simple.");
        }

        public override bool TienePermiso(string nombrePermiso)
        {
            return Nombre.Equals(nombrePermiso, StringComparison.OrdinalIgnoreCase);
        }
    }

    // compuesto: agrupa permisos u otros perfiles bajo un mismo nombre
    public class PerfilComposite : Componente
    {
        private readonly List<Componente> _hijos = new List<Componente>();

        public override void Agregar(Componente c)
        {
            _hijos.Add(c);
        }

        public override bool TienePermiso(string nombrePermiso)
        {
            // matchea por nombre del perfil o por cualquiera de sus hijos
            if (Nombre.Equals(nombrePermiso, StringComparison.OrdinalIgnoreCase)) return true;

            return _hijos.Any(h => h.TienePermiso(nombrePermiso));
        }
    }
}