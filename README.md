# Sistema de Inventario - Gamora Indumentaria

## Configuración de la Base de Datos

Este sistema utiliza SQL Server LocalDB para almacenar la información del inventario. La base de datos está diseñada para manejar diferentes tipos de productos con sus respectivos talles y características específicas.

### Productos Soportados

#### Ropa con Talles de Letras (S, M, L, XL, XXL)

- **BUZOS**
- **CAMPERAS**
- **REMERAS**
- **CHALECOS**
- **JOGGING**
- **BOXER**

#### Ropa con Talles Numéricos

- **JEANS HOMBRE** (36, 38, 40, 42, 44, 46, 48)
- **JEANS DAMA** (34, 36, 38, 40, 42, 44, 46, 48)
- **ZAPATILLAS** (34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45)

#### Productos sin Talles

- **GORRAS** (solo cantidad)
- **CADENITAS** (solo cantidad)
- **VAPER** (cantidad + sabor)
- **RELOJ** (solo cantidad)
- **ANTEOJOS** (solo cantidad)

### Estructura de la Base de Datos

#### Tabla `Categorias`

- Almacena las categorías de productos
- Define si una categoría maneja talles y qué tipo de talles

#### Tabla `TiposTalle`

- Almacena los talles específicos para cada categoría
- Permite diferentes sistemas de talles por categoría

#### Tabla `Inventario`

- Tabla principal que almacena los productos
- Incluye campos para nombre, descripción, código de barras, talle, sabor (para vapers), cantidad y precio

### Instalación y Configuración

#### Opción 1: Usar SQL Server Management Studio (SSMS)

1. Abrir SQL Server Management Studio
2. Conectarse a `(LocalDB)\MSSQLLocalDB`
3. Buscar la base de datos `VentasDB`
4. Ejecutar el script `Scripts/ConfigurarBaseDatos.sql`

#### Opción 2: Usar Visual Studio

1. Abrir Visual Studio
2. Ir a `View` > `SQL Server Object Explorer`
3. Expandir `SQL Server` > `(localdb)\MSSQLLocalDB`
4. Hacer click derecho en `Databases` > `Add New Database`
5. Si ya existe `VentasDB`, hacer click derecho y seleccionar `New Query`
6. Copiar y ejecutar el contenido de `Scripts/ConfigurarBaseDatos.sql`

#### Opción 3: Primera Ejecución de la Aplicación

La aplicación creará automáticamente las tablas necesarias en la primera ejecución si no existen.

### Funcionalidades del Sistema

#### Gestión de Productos

- **Agregar productos**: Formulario dinámico que se adapta según la categoría seleccionada
- **Visualizar inventario**: Grid que muestra todos los productos con filtros por categoría
- **Control de stock**: Alertas visuales para productos con stock bajo (≤ 5 unidades)
- **Campos específicos**:
  - Sabor para vapers
  - Talles dinámicos según categoría
  - Precio de venta opcional
  - Código de barras
  - Descripción

#### Características Técnicas

- **Validaciones**: Campos requeridos según el tipo de producto
- **Interfaz adaptativa**: Los campos se muestran/ocultan según la categoría
- **Base de datos normalizada**: Estructura optimizada para diferentes tipos de productos
- **Vistas predefinidas**: Consultas optimizadas para reportes frecuentes

### Archivos Importantes

- `Data/InventarioDAL.cs`: Capa de acceso a datos
- `Scripts/ConfigurarBaseDatos.sql`: Script de configuración inicial
- `agregarpruducto.cs`: Formulario para agregar productos
- `productos.cs`: Formulario principal de inventario

### Notas de Desarrollo

- El sistema usa Entity Framework-like patterns para el acceso a datos
- Todas las operaciones de base de datos están encapsuladas en la clase `InventarioDAL`
- Los formularios se adaptan dinámicamente según el tipo de producto seleccionado
- Se incluyen procedimientos almacenados para operaciones frecuentes

### Ejemplos de Uso

#### Agregar un Buzo

1. Seleccionar categoría "BUZOS"
2. El sistema mostrará automáticamente los talles S, M, L, XL, XXL
3. Completar nombre, talle y cantidad
4. Opcionalmente agregar descripción, código de barras y precio

#### Agregar un Vaper

1. Seleccionar categoría "VAPER"
2. El sistema ocultará el campo de talle y mostrará el campo de sabor
3. Completar nombre, sabor y cantidad

#### Agregar Zapatillas

1. Seleccionar categoría "ZAPATILLAS"
2. El sistema mostrará los talles del 34 al 45
3. Completar nombre, talle y cantidad

### Troubleshooting

#### Error de Conexión a Base de Datos

- Verificar que SQL Server LocalDB esté instalado
- Ejecutar `sqllocaldb info` en el Command Prompt para verificar las instancias
- Crear la instancia si no existe: `sqllocaldb create MSSQLLocalDB`

#### Tablas No Encontradas

- Ejecutar el script `Scripts/ConfigurarBaseDatos.sql`
- Verificar que la base de datos `VentasDB` existe

#### Formularios No Cargan Datos

- Verificar la cadena de conexión en los archivos `.cs`
- Asegurar que las tablas tienen datos iniciales
