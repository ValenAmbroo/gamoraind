# Solución: Error al cambiar categoría en ComboBox

## Problema identificado

Al cambiar de categoría en el ComboBox aparecía el error:

```
System.ArgumentException: 'No se puede modificar la colección Items cuando está establecida la propiedad DataSource.'
```

## Causa del problema

El ComboBox `cboTalle` tenía un `DataSource` asignado desde el método `CargarTallesPorCategoria()`, pero en el método `ActualizarCamposPorCategoria()` se intentaba limpiar usando `cboTalle.Items.Clear()`.

**❌ Código problemático:**

```csharp
private void ActualizarCamposPorCategoria()
{
    // Limpiar campos
    cboTalle.Items.Clear(); // ← ERROR: No se puede hacer esto cuando hay DataSource
    txtSabor.Text = "";
    // ...
}
```

## Solución implementada

Cuando un ComboBox tiene un `DataSource` asignado, primero hay que quitar el DataSource y luego limpiar los Items.

**✅ Código corregido:**

```csharp
private void ActualizarCamposPorCategoria()
{
    // Limpiar campos - usar DataSource = null para limpiar ComboBox con DataSource
    cboTalle.DataSource = null;  // ← Primero quitar el DataSource
    cboTalle.Items.Clear();      // ← Ahora sí se puede limpiar los Items
    txtSabor.Text = "";
    // ...
}
```

## Flujo correcto ahora

1. Usuario selecciona una categoría
2. Se ejecuta `cboCategoria_SelectedIndexChanged`
3. Se llama a `ActualizarCamposPorCategoria()`
4. Se quita el DataSource del ComboBox de talles
5. Se limpian los Items
6. Se cargan los nuevos talles según la categoría
7. Se asigna el nuevo DataSource con los talles correspondientes

## Estado actual

✅ **RESUELTO**: El cambio de categoría ahora funciona correctamente sin errores.

## Resultado

- ✅ Se pueden cambiar las categorías sin error
- ✅ Los talles se cargan correctamente según la categoría
- ✅ Los campos se muestran/ocultan apropiadamente (Talle, Sabor)
- ✅ La funcionalidad del formulario permanece intacta
