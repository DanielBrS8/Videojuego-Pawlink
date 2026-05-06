using System;
using System.Collections.Generic;

// ─────────────────────────────────────────────────────────────────────────────
//  MODELOS DE DATOS  —  Reflejan exactamente las tablas de la BD
//  Usado por JsonUtility para serializar/deserializar respuestas de la API
// ─────────────────────────────────────────────────────────────────────────────

[Serializable]
public class MascotaVirtual
{
    public int    id_virtual;
    public int    id_usuario;
    public int    id_mascota_real;    // puede ser 0/null
    public string nombre;
    public string especie;
    public int    nivel;
    public int    experiencia;
    public int    hambre;             // 0-100
    public int    felicidad;          // 0-100
    public int    energia;            // 0-100
    public int    higiene;            // 0-100
    public string ultima_interaccion; // timestamp ISO
}

[Serializable]
public class AccionRequest
{
    public int    id_mascota_virtual;
    public string tipo;              // "alimentar" | "jugar" | "bañar"
}

[Serializable]
public class AccionResponse
{
    public int           id_accion;
    public int           puntos_ganados;
    public MascotaVirtual mascota;   // Estado actualizado de la mascota
}

[Serializable]
public class ItemTienda
{
    public int    id_item;
    public string nombre;
    public string tipo;              // "comida" | "juguete" | "accesorio"
    public int    coste_monedas;
    public int    efecto_hambre;
    public int    efecto_felicidad;
    public int    efecto_energia;
    public int    efecto_higiene;
}

[Serializable]
public class ItemTiendaList
{
    public List<ItemTienda> items;
}

[Serializable]
public class InventarioEntry
{
    public int id;
    public int id_usuario;
    public int id_item;
    public int cantidad;
}

[Serializable]
public class InventarioList
{
    public List<InventarioEntry> inventario;
}

[Serializable]
public class Logro
{
    public int    id_logro;
    public string nombre;
    public string descripcion;
    public string icono;
    public int    puntos_necesarios;
}

[Serializable]
public class LogroUsuario
{
    public int    id;
    public int    id_usuario;
    public int    id_logro;
    public string fecha_obtencion;
}

[Serializable]
public class LogrosUsuarioList
{
    public List<LogroUsuario> logros;
}

[Serializable]
public class UsarItemRequest
{
    public int id_usuario;
    public int id_item;
    public int id_mascota_virtual;
}
