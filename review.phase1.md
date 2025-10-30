## **Revisión técnica — Fase 1**

### 🧠 **Sobre ingeniería**

* Cuando se pidió que **no se use MediatR**, la idea no era que implementes uno propio.

  El siguiente código está bien:

  ```csharp
  builder.Services.AddScoped<ICommandHandler<CreateInventoryItemCommand>, CreateInventoryItemCommandHandler>();
  ```

  Pero lo que se buscaba era algo más parecido a esto:

  ```csharp
  grp.MapPost("", async (CreateItemCommand req, ICommandHandler<CreateInventoryItemCommand> createInventoryHandler, WarehouseDbContext db) =>
  ```

  De esa forma, se inyecta el handler directamente en el endpoint y se mantiene el flujo más simple y explícito.

---

### 🧩 **Diseño**

No me queda del todo claro si buscaste aplicar **DDD** o **Arquitectura Hexagonal**.
Tenés una mezcla de conceptos:

* Usás nombres de Hexagonal como *use cases* y *ports*, pero no diferenciás entre **puertos de entrada y de salida**.
* En Hexagonal debería haber un solo *Core*, pero en tu caso tenés `Application` y `Domain`.
* En lugar de un `Adapter.PostgreSQL`, tenés un proyecto más típico de DDD (`Infrastructure`).
* Tampoco encaja del todo con DDD, porque las interfaces del repositorio están en `Application`, y deberían estar en `Domain`.
* Lo mismo con los DTOs: en una arquitectura Hexagonal, **nunca** deberían llegar al *Core*.

Igualmente, ninguna de las dos hubiera sido la arquitectura que yo hubiera elegido. El pedido era mantener las cosas simples, hubiera tirado por `Vertical Slice` .


#### 🗂️ Repositorios

La capa de repositorio debería devolver **solo el tipo que almacena**.
Si necesitás una proyección, hay dos formas de hacerlo:

1. **Devolver un `IQueryable<T>`**

   ```csharp
   IQueryable<InventoryItem>
   ```

   Esto a veces no gusta porque `Application` queda atado a Entity Framework.

2. **Usar una función de proyección**
   El repositorio se mantiene agnóstico del tipo de retorno:

   ```csharp
   repo.GetDetails(sku, x => new ItemDetailsDto(x.Sku, x.Name, x.TotalQuantity), ct);
   ```

#### ⚙️ Inyección de dependencias

Agregá un archivo de **DI por proyecto**, donde cada capa registre solo lo que le corresponde.
Así evitás registrar todo desde la API y exponés únicamente lo necesario.

#### 🚫 Comandos vs DTOs

No uses los **comandos como DTOs**. Son conceptos distintos:

* El **DTO** representa el contenido del *body*.
* El **comando** puede incluir datos adicionales (por ejemplo, tomados de la URL o del contexto de la request HTTP).


#### 💾 Transacciones y `SaveChanges`

El `SaveChanges` **no pertenece a la API ni al controlador**.
Debe estar dentro del *Application Layer*.
Cuando salís de un `CommandHandler`, la operación tiene que ser consistente, es decir, **la transacción tiene que estar cerrada**. Cada handler es una unidad transaccional independiente.

#### ♻️ Repositorio único para lectura/escritura

Podés aplicar dos interfaces sobre una misma clase:

```csharp
public sealed class InventoryRepository(WarehouseDbContext db)
    : IInventoryWriteRepository, IInventoryReadRepository
```

#### 🧮 Validación de claves

Cuando manejás datos que forman parte de una clave primaria o un filtro, hay tres opciones:

1. Lo respetás (porque está bien).
2. Lo rechazás (porque está mal formado).
3. Lo corregís, **pero devolvés el valor corregido**.

El `SKU` actual no cumple con ninguna.
Le hacés un `Trim`, pero devolvés el valor original → esto genera **inconsistencias**, ya que puede no encontrarse después.

Usá un Value Object liviano (record) que valide internamente el formato. Y no, hacer un dominio rico no se considera sobreingenieria. Un buen diseño es aquel que no permite los estados invalidos.

```csharp
public readonly record struct Sku(string Value)
{
    public Sku(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("El SKU no puede estar vacío.", nameof(value));

        Value = value.Trim();
    }

    public override string ToString() => Value;
}
```

#### 🚪 Endpoints confusos

No está bueno que el usuario no sepa dónde poner un dato.
En los endpoints de modificación de stock, hoy se pide el SKU **dos veces** (en la URL y en el body).
Deberías definir una sola fuente de verdad y hacerla prevalecer.

---

### 🎨 **Estilo**

Este bloque está muy bien formateado:

```csharp
var failures = validators.Select(v => v.Validate(arg))
                        .SelectMany(r => r.Errors)
                        .Where(f => f is not null)
                        .ToList();
```

Es preferible mantener este estilo en múltiples líneas, ya que en las PRs permite identificar exactamente qué línea cambió, en lugar de marcar todo el bloque.
El equipo usa esta misma convención incluso para parámetros de métodos.

De esto:

```csharp
Task UpdateAsync(InventoryItem item, CancellationToken ct = default);
```

A esto:

```csharp
Task UpdateAsync(InventoryItem item, 
                CancellationToken ct = default);
```

O incluso más legible:

```csharp
Task UpdateAsync(
    InventoryItem item, 
    CancellationToken ct = default);
```

---

### 🧰 **Otros detalles**

En lugar de mapear campo por campo con `HasColumnName`, podés simplificar usando *naming conventions*:
👉 [EFCore.NamingConventions – UseSnakeCaseNamingConvention](https://github.com/efcore/EFCore.NamingConventions)

