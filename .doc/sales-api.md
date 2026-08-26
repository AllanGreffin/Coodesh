[Back to README](../README.md)

## Sales API

Complete CRUD for sales records, plus cancellation operations, built as a vertical slice
(`Domain` → `ORM` → `Application` → `WebApi`) consistent with the existing `Users` feature.

### Endpoints

| Method | Route | Description |
|--------|-------|-------------|
| `POST` | `/api/sales` | Register a new sale |
| `GET` | `/api/sales/{id}` | Get a sale by id |
| `GET` | `/api/sales` | List sales (paging, filtering, ordering) |
| `PUT` | `/api/sales/{id}` | Update header data and items of a sale |
| `DELETE` | `/api/sales/{id}` | Delete a sale |
| `POST` | `/api/sales/{id}/cancel` | Cancel a whole sale |
| `POST` | `/api/sales/{saleId}/items/{itemId}/cancel` | Cancel a single item |

`GET /api/sales` follows the conventions in [general-api.md](/.doc/general-api.md):

| Query param | Meaning |
|---|---|
| `_page`, `_size` | paging (defaults 1 / 10) |
| `_order` | ordering, e.g. `_order=saleDate desc` or `_order=totalAmount desc, saleNumber asc` |
| `_minDate`, `_maxDate` | inclusive sale-date range |
| `customerId`, `branchId`, `isCancelled` | `field=value` filters |

### Data model

A sale is the **aggregate root**; each product line is a `SaleItem` entity that owns its
own quantity, unit price, discount, total and cancelled flag, and lives in its own
`SaleItems` table (FK + cascade). This is what lets an item be cancelled independently
(`ItemCancelled`) and keeps a price snapshot per line.

`Customer`, `Branch` and `Product` are referenced with the **External Identities pattern**:
only the identifier crosses the domain boundary, and the description (`customerName`,
`branchName`, `productName`) is denormalized onto the sale / item.

### Business rules (enforced in the domain)

| Quantity of identical items | Discount |
|---|---|
| 1 – 3 | none |
| 4 – 9 | 10% |
| 10 – 20 | 20% |
| above 20 | not allowed (`400`) |

* Item total = `(unitPrice * quantity) - discount`.
* Sale total = sum of the totals of the **non-cancelled** items.
* The same product added twice in one request is consolidated into a single line
  (the rules speak of "identical items").
* A cancelled sale/item no longer contributes to the total and cannot be changed.

### Domain events

`SaleCreated`, `SaleModified`, `SaleCancelled` and `ItemCancelled` are raised by the
aggregate and dispatched after persistence through `IEventPublisher`. The default
implementation (`LoggingEventPublisher`) writes a structured line to the application log;
replacing it with a real broker (Rebus) only means registering another implementation.

### Error handling

`ValidationExceptionMiddleware` maps `ValidationException` → `400`,
`KeyNotFoundException` → `404`, `DomainException` → `400` and `InvalidOperationException`
→ `409`, all using the standard `ApiResponse` envelope.

<div style="display: flex; justify-content: space-between;">
  <a href="./frameworks.md">Previous: Frameworks</a>
  <a href="./project-structure.md">Next: Project Structure</a>
</div>
