# InventoryHub – Reflective Summary

## Project Overview
InventoryHub is a full-stack application built with a **Blazor WebAssembly** front-end (`ClientApp`) and an **ASP.NET Core Minimal API** back-end (`ServerApp`). The goal was to integrate both projects so the front-end could fetch and display a product list with nested category data served by the back-end.

---

## How AI Assisted Throughout Development

### Activity 1 – Integration Code Generation
AI generated the initial `FetchProducts.razor` component and wired it to the back-end using `HttpClient`. Key contributions:
- Scaffolded the `OnInitializedAsync` lifecycle method with an async HTTP call.
- Identified that `ClientApp`'s `HttpClient` base address pointed to the Blazor host URL by default, not the API server, and suggested updating `Program.cs` to point to `http://localhost:5048`.
- Recommended adding `builder.Services.AddCors()` to the server and updating `ClientApp/Program.cs` before the apps could communicate across ports.

### Activity 2 – Debugging Integration Issues
Three bugs were introduced and resolved with AI assistance:

| Issue | Root Cause | AI Contribution |
|---|---|---|
| Wrong route | Front-end called `/api/products`; back-end changed to `/api/productlist` | Spotted the mismatch and updated both sides simultaneously |
| CORS errors | Named policy was too restrictive for development | Suggested replacing it with `AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()` |
| Malformed JSON | `GetFromJsonAsync` gave no diagnostic info on bad payloads | Refactored to `GetAsync` + `ReadAsStringAsync` + `JsonSerializer.Deserialize` with a dedicated `catch (JsonException)` block |

### Activity 3 – JSON Structure Design
AI designed the nested `Category` object inside each product and updated the `Product` C# model to match. Notable suggestions:
- Made `Category` nullable (`Category?`) on the model so old or incomplete API responses would not crash deserialization.
- Added `PropertyNameCaseInsensitive = true` to the deserializer options to guard against casing mismatches between the C# model and JSON keys.

### Activity 4 – Performance Optimisation
AI identified two redundant-work hotspots and resolved both:

**Back-end – `IMemoryCache`**
- Added `builder.Services.AddMemoryCache()` and injected `IMemoryCache` into the endpoint.
- The product list is now built once and cached for five minutes. Subsequent requests are served from memory with no object allocation.

**Front-end – Static component cache**
- Added a `static Product[]? _cachedProducts` field in `FetchProducts.razor`.
- If the user navigates away and back, `OnInitializedAsync` detects the cached value and returns immediately — no HTTP call is made.

---

## Challenges and How AI Helped Overcome Them

**Challenge 1: Cross-origin requests failing silently**
The browser blocked requests from `localhost:5090` to `localhost:5048` with no useful error in the Blazor UI. AI immediately identified CORS as the cause, explained why Blazor WASM triggers CORS (it runs in the browser, not on the server), and provided the correct middleware order (`UseCors` before `MapGet`).

**Challenge 2: JSON deserialization giving no diagnostic information**
`GetFromJsonAsync` swallowed errors and returned `null`, making it impossible to tell whether the API was unreachable or returning invalid JSON. AI refactored the call into a three-step pipeline and added separate `catch` blocks for `HttpRequestException`, `JsonException`, and `TaskCanceledException` — each surfacing a different, actionable error message.

**Challenge 3: Keeping both projects in sync**
Any change to the back-end model (e.g., adding `Category`) required a matching change in the front-end `Product` class. AI handled both sides of every schema change in a single step, eliminating the risk of the models drifting apart.

---

## Lessons Learned

- **AI is most effective for cross-cutting changes** — updates that span multiple files (route rename, schema change, CORS) were handled faster and with fewer mistakes than manual edits.
- **Prompt specificity matters** — describing the exact error (e.g., "malformed JSON crashes silently") produced a more targeted solution than asking to "fix the fetch logic."
- **AI suggestions need verification** — the static cache approach works well for a demo but would need invalidation logic in a production app. Understanding *why* a suggestion works is as important as accepting it.
- **Separation of concerns pays off early** — because the `Product` class was cleanly separated from the display logic in the component, adding the `Category` field required only two targeted edits with no ripple effects.
